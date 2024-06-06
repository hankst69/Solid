//----------------------------------------------------------------------------------
// File: "EventAggregatorTests.cs"
// Date: 2016-2022
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.EventAggregator;
//using Solid.TestInfrastructure.FluentAssertions;
using NUnit.Framework;

namespace Solid.Infrastructure_uTest.EventAggregator
{
    public class EventAggregatorTests
    {
        private Infrastructure.EventAggregator.Impl.EventAggregator m_Target;
        private Mock<ITracer> m_TracerMock;

        [SetUp]
        public void SetUp()
        {
            m_TracerMock = new Mock<ITracer>();

            //m_TracerMock.Setup(x => x.CreateBaseDomainTracer()).Returns(m_TracerMock.Object);
            m_TracerMock.Setup(x => x.CreateSubDomainTracer(It.IsAny<string>())).Returns(m_TracerMock.Object);
            m_TracerMock.Setup(x => x.CreateScopeTracer(It.IsAny<string>())).Returns(m_TracerMock.Object);

            m_Target = new Infrastructure.EventAggregator.Impl.EventAggregator(m_TracerMock.Object);
        }

        [Test]
        public void Ctor_ShouldNotThrow_WhenTracerIsNull()
        {
            // Arrange
            // Act
            Action action = () => new Infrastructure.EventAggregator.Impl.EventAggregator(null);

            // Assert
            action.Should().NotThrow<ArgumentNullException>();
        }

        [Test]
        public void Publish_ShouldThrow_WhenEventNull()
        {
            // Arrange
            // Act
            Action action = () => m_Target.Publish(null);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Publish_ShouldNotThrow_WhenEventNotNullButNoSubscribers()
        {
            // Arrange
            // Act
            Action action = () => m_Target.Publish(new TestEvent("Foo"));

            // Assert
            action.Should().NotThrow<Exception>();
        }

        [Test]
        public void Publish_ShouldDeliver_WhenEventNotNullAndSubscriber()
        {
            // Arrange
            var testEvent = new TestEvent("0815");
            var testEventHandler = new TestEventHandler<TestEvent>();
            m_Target.Subscribe(testEventHandler);

            // Act
            m_Target.Publish(testEvent);

            // Assert
            testEventHandler.HandledEvents.Should().BeEquivalentTo(new[] { testEvent });
        }

        [Test]
        public void Publish_ShouldDeliverToAll_WhenPublishingEventToMutipleSubscribers()
        {
            // Arrange
            var testEvent = new TestEvent("4711");
            var testEventHandler1 = new TestEventHandler<TestEvent>();
            var testEventHandler2 = new TestEventHandler<TestEvent>();
            m_Target.Subscribe(testEventHandler1);
            m_Target.Subscribe(testEventHandler2);

            // Act
            m_Target.Publish(testEvent);

            // Assert
            testEventHandler1.HandledEvents.Should().BeEquivalentTo(new[] { testEvent });
            testEventHandler2.HandledEvents.Should().BeEquivalentTo(new[] { testEvent });
        }

        [Test]
        public void Publish_ShouldNotDeliver_WhenPublishingEventAndSubscriberDisposed()
        {
            // Arrange
            var testEvent = new TestEvent("Message");
            var testEventHandler1 = new TestEventHandler<TestEvent>();
            var testEventHandler2 = new TestEventHandler<TestEvent>();
            var subscription1 = m_Target.Subscribe(testEventHandler1);
            m_Target.Subscribe(testEventHandler2);

            // Act
            subscription1.Dispose();
            m_Target.Publish(testEvent);

            // Assert
            testEventHandler1.HandledEvents.Should().BeEmpty();
            testEventHandler2.HandledEvents.Should().BeEquivalentTo(new[] { testEvent });
        }

        [Test]
        public void Dump_ShouldReturnListOfEventSubscriptions()
        {
            // Arrange
            var handler = new TestEventHandler<TestEvent>();
            m_Target.Subscribe(handler);

            // Act
            var dump = m_Target.Dump();

            // Assert
            SerializeToString(dump).Should().NotBeNull().And.Be(
                "<ArrayOfEventAggregator.EventSubscriptions xmlns=\"http://schemas.datacontract.org/2004/07/Solid.Infrastructure.EventAggregator.Impl\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><EventAggregator.EventSubscriptions><EventType>TestEvent</EventType><Subscribers xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\"><a:string>TestEventHandler`1</a:string></Subscribers></EventAggregator.EventSubscriptions></ArrayOfEventAggregator.EventSubscriptions>"
                );
        }

        private static string SerializeToString(object toSerialize)
        {
            if (toSerialize == null)
            {
                return null;
            }
            using (var stream = new System.IO.MemoryStream())
            {
                var serializer = new System.Runtime.Serialization.DataContractSerializer(toSerialize.GetType());
                serializer.WriteObject(stream, toSerialize);

                stream.Seek(0, System.IO.SeekOrigin.Begin);

                using (var streamReader = new System.IO.StreamReader(stream))
                {
                    var result = streamReader.ReadToEnd();
                    return result;
                }
            }
        }

        [Test]
        public void Publish_ShouldDeliverDifferentEventsWithSameTagInterface_WhenHandlerSubscribedForTagInterfaceType()
        {
            // Arrange
            var testEvent = new TestEvent("0815");
            var taggedTestEvent1 = new TaggedTestEvent1("4711");
            var taggedTestEvent2 = new TaggedTestEvent2("1234");
            var eventHandler1 = new TestEventHandler<TestEvent>();
            var eventHandler2 = new TestEventHandler<TaggedTestEvent1>();
            var eventHandler3 = new TestEventHandler<TaggedTestEvent2>();
            var eventHandler4 = new TestEventHandler<ITaggedEvent>();
            m_Target.Subscribe(eventHandler1);
            m_Target.Subscribe(eventHandler2);
            m_Target.Subscribe(eventHandler3);
            m_Target.Subscribe(eventHandler4);

            // Act
            m_Target.Publish(testEvent);
            m_Target.Publish(taggedTestEvent1);
            m_Target.Publish(taggedTestEvent2);

            // Assert
            eventHandler1.HandledEvents.Should().BeEquivalentTo(new[] { testEvent });
            eventHandler2.HandledEvents.Should().BeEquivalentTo(new[] { taggedTestEvent1 });
            eventHandler3.HandledEvents.Should().BeEquivalentTo(new[] { taggedTestEvent2 });
            eventHandler4.HandledEvents.Should().BeEquivalentTo(new ITaggedEvent[] { taggedTestEvent1, taggedTestEvent2 });
        }

        [Test]
        public void Subscribe_ShouldThrow_WhenHandlerIsNull()
        {
            // Arrange
            // Act
            Action action = () => m_Target.Subscribe((IHandleEvent<IEvent>)null);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Subscribe_ShouldAddHandlersToSubscriptions()
        {
            // Arrange
            // Act
            m_Target.Subscribe(new TestEventHandler<TestEvent>());
            m_Target.Subscribe(new TestEventHandler<TaggedTestEvent1>());
            m_Target.Subscribe(new TestEventHandler<ITaggedEvent>());

            // Assert
            SerializeToString(m_Target.Dump()).Should().NotBeNull().And.Be(
                "<ArrayOfEventAggregator.EventSubscriptions xmlns=\"http://schemas.datacontract.org/2004/07/Solid.Infrastructure.EventAggregator.Impl\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><EventAggregator.EventSubscriptions><EventType>TestEvent</EventType><Subscribers xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\"><a:string>TestEventHandler`1</a:string></Subscribers></EventAggregator.EventSubscriptions><EventAggregator.EventSubscriptions><EventType>TaggedTestEvent1</EventType><Subscribers xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\"><a:string>TestEventHandler`1</a:string></Subscribers></EventAggregator.EventSubscriptions><EventAggregator.EventSubscriptions><EventType>ITaggedEvent</EventType><Subscribers xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\"><a:string>TestEventHandler`1</a:string></Subscribers></EventAggregator.EventSubscriptions></ArrayOfEventAggregator.EventSubscriptions>"
                );
        }

        [TestCase("none", "<ArrayOfEventAggregator.EventSubscriptions xmlns=\"http://schemas.datacontract.org/2004/07/Solid.Infrastructure.EventAggregator.Impl\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><EventAggregator.EventSubscriptions><EventType>TaggedTestEvent1</EventType><Subscribers xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\"><a:string>TestSingleEventHandler</a:string></Subscribers></EventAggregator.EventSubscriptions><EventAggregator.EventSubscriptions><EventType>TestEvent</EventType><Subscribers xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\"><a:string>TestMultiEventHandler</a:string></Subscribers></EventAggregator.EventSubscriptions><EventAggregator.EventSubscriptions><EventType>ITaggedEvent</EventType><Subscribers xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\"><a:string>TestMultiEventHandler</a:string></Subscribers></EventAggregator.EventSubscriptions></ArrayOfEventAggregator.EventSubscriptions>")]
        [TestCase("singleHandler", "<ArrayOfEventAggregator.EventSubscriptions xmlns=\"http://schemas.datacontract.org/2004/07/Solid.Infrastructure.EventAggregator.Impl\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><EventAggregator.EventSubscriptions><EventType>TaggedTestEvent1</EventType><Subscribers xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\"/></EventAggregator.EventSubscriptions><EventAggregator.EventSubscriptions><EventType>TestEvent</EventType><Subscribers xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\"><a:string>TestMultiEventHandler</a:string></Subscribers></EventAggregator.EventSubscriptions><EventAggregator.EventSubscriptions><EventType>ITaggedEvent</EventType><Subscribers xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\"><a:string>TestMultiEventHandler</a:string></Subscribers></EventAggregator.EventSubscriptions></ArrayOfEventAggregator.EventSubscriptions>")]
        [TestCase("multiHandler", "<ArrayOfEventAggregator.EventSubscriptions xmlns=\"http://schemas.datacontract.org/2004/07/Solid.Infrastructure.EventAggregator.Impl\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><EventAggregator.EventSubscriptions><EventType>TaggedTestEvent1</EventType><Subscribers xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\"><a:string>TestSingleEventHandler</a:string></Subscribers></EventAggregator.EventSubscriptions><EventAggregator.EventSubscriptions><EventType>TestEvent</EventType><Subscribers xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\"/></EventAggregator.EventSubscriptions><EventAggregator.EventSubscriptions><EventType>ITaggedEvent</EventType><Subscribers xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\"/></EventAggregator.EventSubscriptions></ArrayOfEventAggregator.EventSubscriptions>")]
        [TestCase("both", "<ArrayOfEventAggregator.EventSubscriptions xmlns=\"http://schemas.datacontract.org/2004/07/Solid.Infrastructure.EventAggregator.Impl\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><EventAggregator.EventSubscriptions><EventType>TaggedTestEvent1</EventType><Subscribers xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\"/></EventAggregator.EventSubscriptions><EventAggregator.EventSubscriptions><EventType>TestEvent</EventType><Subscribers xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\"/></EventAggregator.EventSubscriptions><EventAggregator.EventSubscriptions><EventType>ITaggedEvent</EventType><Subscribers xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\"/></EventAggregator.EventSubscriptions></ArrayOfEventAggregator.EventSubscriptions>")]
        public void UnSubscribeAll_ShouldRemoveAllSubscriptionsOfGivenHanlderInstance(string unregisterFrom, string expectedDumpSerialization)
        {
            // Arrange
            var singleHandler = new TestSingleEventHandler();
            var multiHandler = new TestMultiEventHandler();

            m_Target.Subscribe(singleHandler);
            m_Target.Subscribe<TestEvent>(multiHandler);
            m_Target.Subscribe<ITaggedEvent>(multiHandler);

            // Act
            switch (unregisterFrom)
            {
                case "singleHandler":
                    m_Target.UnSubscribeAll(singleHandler);
                    break;

                case "multiHandler":
                    m_Target.UnSubscribeAll(multiHandler);
                    break;

                case "both":
                    m_Target.UnSubscribeAll(singleHandler);
                    m_Target.UnSubscribeAll(multiHandler);
                    break;
            }

            // Assert
            SerializeToString(m_Target.Dump()).Should().NotBeNull().And.Be(expectedDumpSerialization);
        }

        private class TestSingleEventHandler : IHandleEvent<TaggedTestEvent1>
        {
            public void Handle(TaggedTestEvent1 theEvent) { }
        }

        private class TestMultiEventHandler : IHandleEvent<TestEvent>, IHandleEvent<ITaggedEvent>
        {
            public void Handle(TestEvent theEvent) { }
            public void Handle(ITaggedEvent theEvent) { }
        }

        private class TestEventHandler<TEventType> : IHandleEvent<TEventType> where TEventType : IEvent
        {
            public TestEventHandler()
            {
                HandledEvents = new List<TEventType>();
            }

            public IList<TEventType> HandledEvents { get; private set; }

            public void Handle(TEventType theEvent)
            {
                HandledEvents.Add(theEvent);
            }
        }

        private class TestEvent : IEvent, IEquatable<TestEvent>
        {
            private string Message { get; }

            public TestEvent(string message)
            {
                Message = message;
            }

            public bool Equals(TestEvent other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }

                return Message == other.Message;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                return Equals(obj as TestEvent);
            }

            public override int GetHashCode()
            {
                return Message.GetHashCode();
            }

            public override string ToString()
            {
                return Message;
            }
        }

        public interface ITaggedEvent : IEvent { }

        private sealed class TaggedTestEvent1 : TestEvent, ITaggedEvent
        {
            public TaggedTestEvent1(string message) : base(message) { }
        }

        private sealed class TaggedTestEvent2 : TestEvent, ITaggedEvent
        {
            public TaggedTestEvent2(string message) : base(message) { }
        }
    }
}

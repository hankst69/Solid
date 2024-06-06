using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Solid.TestInfrastructure.ParameterConditionTesting;

namespace Solid.TestInfrastructure_uTest.ParameterConditionTesting
{
    [TestFixture]
    public class ParameterMockerTests
    {
        [Test]
        public void Api_ShouldThrow_WhenNullChecksMissing()
        {
            // Arrange
            // Act
            // Assert
            ParameterConditionsChecker
                .For<ParameterMocker>()
                .CheckCtorParameters()
                .CheckApi()
                .Verify();
        }

        // build in value types
        [TestCase(typeof(bool))]
        [TestCase(typeof(int))]
        [TestCase(typeof(decimal))]
        [TestCase(typeof(double))]

        // structs
        [TestCase(typeof(DateTime))]

        // enums
        [TestCase(typeof(ControlEvent))]

        // enumerations
        [TestCase(typeof(IList<int>))]
        [TestCase(typeof(IList<string>))]
        [TestCase(typeof(IList<IHandleDto>))]
        [TestCase(typeof(IEnumerable<IList<IHandleDto>>))]

        // classes
        [TestCase(typeof(string))]
        [TestCase(typeof(ModifierKeysDuringInteractionEvent))]

        // classes with ctor contains class itself
        [TestCase(typeof(ClassWithCtorContainsTypeItself))]

        // abstract classes
        [TestCase(typeof(Dto))]

        // interfaces
        [TestCase(typeof(IHandleDto))]

        // func
        [TestCase(typeof(Func<bool>))]
        [TestCase(typeof(Func<int, bool>))]
        public void CreateMockedObjectFor_ShouldNotThrow(Type typeToMock)
        {
            // Arrange
            var target = new ParameterMocker();

            try
            {
                // Act
                var instance = target.CreateMockedObjectFor(typeToMock);

                // Assert
                instance.Should().NotBeNull();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void AddMockedObject_ShouldWorkProperly()
        {
            // Arrange
            var stringList = new List<string> { "vlksd" };
            int intValue = 42;

            var target = new ParameterMocker();

            // Act
            target.AddMockedObject(intValue);
            target.AddMockedObject(stringList);

            var resultForInt = target.CreateMockedObjectFor(typeof(int));
            var resultForStringList = target.CreateMockedObjectFor(typeof(List<string>));

            // Assert
            resultForInt.Should().Be(intValue);
            resultForStringList.Should().Be(stringList);
        }

        [Test]
        public void AddMockedObject_ShouldWorkWithMockedObjectsCreatedByMoq()
        {
            // Arrange
            var mockedObject = new Mock<IAmEmpty>().Object;

            var target = new ParameterMocker();

            // Act
            target.AddMockedObject(mockedObject);
            var result = target.CreateMockedObjectFor(typeof(IAmEmpty));

            // Assert
            result.Should().BeSameAs(mockedObject);
        }

        public interface IAmEmpty
        {
        }

        public class ClassWithCtorContainsTypeItself
        {
            public ClassWithCtorContainsTypeItself(ClassWithCtorContainsTypeItself self)
            {
            }

            public ClassWithCtorContainsTypeItself(int a, int b)
            {
            }
        }
    }
}
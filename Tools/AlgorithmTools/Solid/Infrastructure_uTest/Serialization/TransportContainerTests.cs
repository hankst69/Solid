//----------------------------------------------------------------------------------
// <copyright file="TransportContainerTests.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017. All Rights Reserved. Confidential.
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using FluentAssertions;
using Solid.Infrastructure.Serialization;
using NUnit.Framework;

namespace Solid.Infrastructure_uTest.Serialization
{
    class TransportContainerTests
    {
        [Test, Ignore("")]
        public void ShouldBeSerializable()
        {
        }

        [Test]
        public void Ctor_ShouldThrow_WhenPayloadIsNull()
        {
            // Arrange
            // Act
            Action action = () => new TransportContainer(null);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Ctor_ShouldThrow_WhenPayloadIsNotSerializable()
        {
            // Arrange
            var nonSerializable = new NonSerializable("4711");

            // Act
            Action action = () => new TransportContainer(nonSerializable);

            // Assert
            action.Should().Throw<InvalidDataContractException>();
        }

        [Test]
        public void Ctor_ShouldNotThrow_WhenPayloadIsSerializable()
        {
            // Arrange
            var serializablePayload = new SerializablePayload(false, "4711", new string(new[] { '0', '8', '1', '5', (char)0 }));

            // Act
            Action action = () => new TransportContainer(serializablePayload);

            // Assert
            action.Should().NotThrow<Exception>();
        }

        [Test]
        public void Ctor_ShouldThrow_WhenPayloadIsSerializableButAnyInnerObjectIsNotSerializable()
        {
            // Arrange
            var serializableInnerNonserializable = new SerializablePayload(false, "4711", new NonSerializable("0815"));

            // Act
            Action action = () => new TransportContainer(serializableInnerNonserializable);

            // Assert
            action.Should().Throw<InvalidDataContractException>();
        }

        [Test]
        public void Ctor_ShouldNotThrow_WhenPayloadIsSerializableAndAllInnerObjectsSerializable()
        {
            // Arrange
            var serializablePayload = new SerializablePayload(false, "4711", new SerializablePayload(true, "innerSerializable", new string(new[] { '0', '8', '1', '5', (char)0 })));

            // Act
            Action action = () => new TransportContainer(serializablePayload);

            // Assert
            action.Should().NotThrow<Exception>();
        }

        [Test]
        public void Payload_ShouldReturnDifferentInstanceThatEqualsTypeAndValues()
        {
            // Arrange
            var payload = new SerializablePayload(true, "test1", new string('x', 10));
            var target = new TransportContainer(payload);

            // Act
            var result = target.Payload;

            // Assert
            result.Should().NotBeSameAs(payload);
            result.GetType().FullName.Should().Be(typeof(SerializablePayload).FullName);
            var typedResult = (SerializablePayload)result;

            typedResult.Value1.Should().Be(payload.Value1);
            typedResult.Value2.Should().Be(payload.Value2);
            typedResult.Value3.Should().Be(payload.Value3);
        }

        private class NonSerializable
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            private string Message { get; }

            public NonSerializable(string message)
            {
                Message = message;
            }
        }

        private interface ISerializablePayload
        {
            bool Value1 { get; set; }
            string Value2 { get; set; }
            object Value3 { get; set; }
        }

        [DataContract]
        private class SerializablePayload : ISerializablePayload
        {
            public SerializablePayload(bool value1, string value2, object value3)
            {
                Value1 = value1;
                Value2 = value2;
                Value3 = value3;
            }

            [DataMember]
            public bool Value1 { get; set; }
            [DataMember]
            public string Value2 { get; set; }
            [DataMember]
            public object Value3 { get; set; }
        }

    }
}

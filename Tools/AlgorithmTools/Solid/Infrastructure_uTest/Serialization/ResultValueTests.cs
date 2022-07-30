//----------------------------------------------------------------------------------
// <copyright file="ResultValueTests.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2020. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Globalization;
using FluentAssertions;
using Solid.Infrastructure.Serialization;
using NUnit.Framework;

namespace Solid.Infrastructure_uTest.Serialization
{
    internal class ResultValueTests
    {
        [Test]
        public void Ctor_ShouldThrow_WhenNameIsNull()
        {
            // Arrange
            int value = 4;
            // Act
            Action action = () => new ResultValue(null, value);
            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Ctor_ShouldThrow_WhenNameIsEmpty()
        {
            // Arrange
            // Act
            Action action = () => new ResultValue(string.Empty, 123);
            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Ctor_ShouldThrow_WhenValueIsNull()
        {
            // Arrange
            // Act
            Action action = () => new ResultValue("name", null);
            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Ctor_ShouldNotThrow_WhenFuncIsNull()
        {
            // Arrange
            // Act
            Action action = () => new ResultValue("name", 123, null);
            // Assert
            action.Should().NotThrow();
        }

        [Test]
        public void Name_ShouldBeIdenticalWithInput()
        {
            // Arrange
            const string name = "AnyName";
            // Act
            var target = new ResultValue(name, 123);
            // Assert
            target.Name.Should().Be(name);
        }

        [Test]
        public void ToString_ShouldReturnFuncOutput_WhenFuncSpecified()
        {
            // Arrange
            var value = 123;
            var expectedToString = "***123***";
            var funcIsCalled = false;
            Func<object, string> func = s => { funcIsCalled = true; return string.Concat("***", s.ToString(), "***"); };
            var target = new ResultValue("name", value, func);
            // Act
            var result = target.ToString();
            // Assert
            funcIsCalled.Should().BeTrue();
            result.Should().Be(expectedToString);
        }

        [Test]
        public void ToString_ShouldReturnValuesToString_WhenFuncNotSpecified()
        {
            // Arrange
            const int value = 123;
            var expectedToString = value.ToString(CultureInfo.InvariantCulture);
            var target = new ResultValue("name", value);
            // Act
            var result = target.ToString();
            // Assert
            result.Should().Be(expectedToString);
        }

        [Test]
        public void Equals_ShouldReturnFalse_WhenOtherIsNull()
        {
            // Arrange
            var target = new ResultValue("name", 123);
            // Act
            var result = target.Equals(null);
            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void Equals_ShouldReturnTrue_WhenOtherIsSelf()
        {
            // Arrange
            var target = new ResultValue("name", 123);
            // Act
            var result = target.Equals(target);
            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void Equals_ShouldReturnTrue_WhenOtherIsIdentical()
        {
            // Arrange
            var other = new ResultValue("name", 123);
            var target = new ResultValue("name", 123);
            // Act
            var result = target.Equals(other);
            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void Equals_ShouldReturnFalse_WhenOtherDiffersInName()
        {
            // Arrange
            var other = new ResultValue("otherName", 123);
            var target = new ResultValue("name", 123);
            // Act
            var result = target.Equals(other);
            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void Equals_ShouldReturnFalse_WhenOtherDiffersInValue()
        {
            // Arrange
            var other = new ResultValue("name", 123);
            var target = new ResultValue("name", 321);
            // Act
            var result = target.Equals(other);
            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void Equals_ShouldReturnFalse_WhenOtherDiffersInValueType()
        {
            // Arrange
            var other = new ResultValue("name", 123);
            var target = new ResultValue("name", 123.0);
            // Act
            var result = target.Equals(other);
            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void Value_ShouldBeIdenticalWithInput_ForValueTypes()
        {
            // Arrange
            const int value = 123;
            // Act
            var target = new ResultValue("name", value);
            // Assert
            target.Value.Should().Be(value);
        }

        [Test]
        public void Value_ShouldBeIdenticalWithInput_ForReferenceTypes()
        {
            // Arrange
            const string value = "123";
            // Act
            var target = new ResultValue("name", value);
            // Assert
            target.Value.Should().Be(value);
        }

        [Test]
        public void ValueType_ShouldBeIdenticalWithInput_ForValueTypes()
        {
            // Arrange
            const double value = 123;
            Type expectedType = typeof(double);
            // Act
            var target = new ResultValue("name", value);
            // Assert
            target.Value.Should().BeOfType(expectedType);
            target.ValueType.Should().Be(expectedType);
        }

        [Test]
        public void ValueType_ShouldBeIdenticalWithInput_ForReferenceTypes()
        {
            // Arrange
            const string value = "123";
            Type expectedType = typeof(string);
            // Act
            var target = new ResultValue("name", value);
            // Assert
            target.Value.Should().BeOfType(expectedType);
            target.ValueType.Should().Be(expectedType);
        }

        [Test]
        public void Value_ShouldBeIdenticalWithInput_ForUserValueTypes()
        {
            // Arrange
            var value = new TestValueType(123);
            // Act
            var target = new ResultValue("name", value);
            // Assert
            //value.Value = 124;
            target.Value.Should().Be(value);
        }

        [Test]
        public void Value_ShouldBeIdenticalWithInput_ForUserReferenceTypes()
        {
            // Arrange
            var value = new TestReferenceType();
            // Act
            var target = new ResultValue("name", value);
            // Assert
            target.Value.Should().Be(value);
        }

        [Test]
        public void ValueType_ShouldBeIdenticalWithInput_ForUserValueTypes()
        {
            // Arrange
            var value = new TestValueType();
            Type expectedType = typeof(TestValueType);
            // Act
            var target = new ResultValue("name", value);
            // Assert
            target.Value.Should().BeOfType(expectedType);
            target.ValueType.Should().Be(expectedType);
        }

        [Test]
        public void ValueType_ShouldBeIdenticalWithInput_ForUserReferenceTypes()
        {
            // Arrange
            var value = new TestReferenceType();
            Type expectedType = typeof(TestReferenceType);
            // Act
            var target = new ResultValue("name", value);
            // Assert
            target.Value.Should().BeOfType(expectedType);
            target.ValueType.Should().Be(expectedType);
        }

        internal class TestReferenceType
        {
        }

        internal struct TestValueType
        {
            public TestValueType(int value) : this()
            {
                Value = value;
            }

            internal int Value {get; set;}
        }
    }
}

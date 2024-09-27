//----------------------------------------------------------------------------------
// File: "ObjectExtensionsTests.cs"
// Date: 2015-2019
//----------------------------------------------------------------------------------
using System;
using System.Globalization;
using FluentAssertions;
using Solid.Infrastructure.RuntimeTypeExtensions;
using NUnit.Framework;

namespace Solid.Infrastructure_uTest.RuntimeTypeExtensions
{
    public class ObjectExtensionsTests
    {
        private const double c_DefaultTolerance = 1E-06;

        [Test]
        public void As_ShouldReturnNull_WhenValueIsNotType()
        {
            // Arrange
            var value = new object();

            // Act
            //var result = value.As<string>();
            var result = ObjectExtensions.As<string>(value);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void As_ShouldReturnNull_WhenValueIsNull()
        {
            // Arrange
            object value = null;

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull since we like to test this
            //var result = value.As<object>();
            var result = ObjectExtensions.As<object>(value);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void As_ShouldCastValue_WhenValueIsType()
        {
            // Arrange
            var value = "123";

            // Act
            //var result = value.As<string>();
            var result = ObjectExtensions.As<string>(value);

            // Assert
            result.Should().Be(value);
        }

        [Test]
        public void CastTo_ShouldThrow_WhenNoTypeConversionExistsForValueTypes()
        {
            // Arrange
            var value = new ValueValue();

            // Act
            Action action = () => value.CastTo<OtherValueValue>();

            // Assert
            action.Should().Throw<InvalidCastException>();
        }

        private struct ValueValue
        {
        }

        private struct OtherValueValue
        {
        }

        [Test]
        public void CastTo_ShouldThrow_WhenNoTypeConversionExistsForReferenceTypes()
        {
            // Arrange
            var value = new ReferenceValue();

            // Act
            Action action = () => value.CastTo<OtherReferenceValue>();

            // Assert
            action.Should().Throw<InvalidCastException>();
        }

        private class ReferenceValue
        {
        }

        private class OtherReferenceValue
        {
        }


        [Test]
        public void CastTo_ShouldThrow_WhenObjValueTypeAndCastInvalid()
        {
            // Arrange
            var value = "abc";

            // Act
            Action action = () => value.CastTo<int>();

            // Assert
            action.Should().Throw<FormatException>();
        }

        [Test]
        public void CastTo_ShouldThrow_WhenObjReferenceTypeAndCastInvalid()
        {
            // Arrange
            var value = new object();

            // Act
            Action action = () => value.CastTo<string>();

            // Assert
            action.Should().Throw<InvalidCastException>();
        }

        [Test]
        public void CastTo_ShouldThrow_WhenStringRepresentationOfDoubleIsCastToInt()
        {
            // Arrange
            var value = (1.5).ToString(CultureInfo.CurrentCulture);

            // Act
            Action action = () => value.CastTo<int>();

            // Assert
            action.Should().Throw<FormatException>();
        }

        [Test]
        public void CastTo_ShouldThrow_WhenNullCastToValueType()
        {
            // Arrange
            object value = null;

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull since we like to test this
            Action action = () => value.CastTo<int>();

            // Assert
            action.Should().Throw<InvalidCastException>();
        }

        [Test]
        public void CastTo_ShouldCast_WhenNullCastToReferenceType()
        {
            // Arrange
            object value = null;

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull since we like to test this
            var result = value.CastTo<string>();

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void CastTo_ShouldCast_WhenNullCastToNullableType()
        {
            // Arrange
            object value = null;

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull since we like to test this
            var result = value.CastTo<int?>();

            // Assert
            result.HasValue.Should().BeFalse();
        }

        [Test]
        public void CastTo_ShouldCast_WhenValueTypeWithValidCast()
        {
            // Arrange
            var value = 17;

            // Act
            var result = value.CastTo<double>();

            // Assert
            result.Should().BeApproximately(value, c_DefaultTolerance);
        }

        [Test]
        public void CastTo_ShouldCast_WhenReferenceTypeWithValidCast()
        {
            // Arrange
            var value = "abc";

            // Act
            var result = value.CastTo<object>();

            // Assert
            result.Should().Be(value);
        }

        [Test]
        public void CastTo_ShouldCast_WhenNullableTypeWithValidCastToUnterlyingType()
        {
            // Arrange
            var value = new int?(4711);

            // Act
            var result = value.CastTo<int>();

            // Assert
            result.Should().Be(value.Value);
        }


        [Test]
        public void CastTo_ShouldCast_WhenNullableTypeWithValidCastToReferenceType()
        {
            // Arrange
            var value = new int?();

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull since we like to test this
            var result = value.CastTo<object>();

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void CastTo_ShouldCastValue_WhenCastToSameReferenceType()
        {
            // Arrange
            const string value = "abc";

            // Act
            var result = value.CastTo<string>();

            // Assert
            result.Should().Be(value);
        }

        [Test]
        public void CastTo_ShouldCastValue_WhenCastToSameValueType()
        {
            // Arrange
            var value = new double();

            // Act
            var result = value.CastTo<double>();

            // Assert
            result.Should().Be(value);
        }

        [Test]
        public void CastTo_ShouldCastValue_WhenCastToImplementedInterface()
        {
            // Arrange
            var value = new double();

            // Act
            var result = value.CastTo<IComparable>();

            // Assert
            result.Should().Be(value);
        }

        [Test]
        public void CastTo_ShouldCastValue_WhenCastToConvertibleValue()
        {
            // Arrange
            var value = new int();

            // Act
            var result = value.CastTo<double>();

            // Assert
            result.Should().Be(value);
        }

        [Test]
        public void CastTo_ShouldCastValue_WhenStringRepresentationOfDoubleCastToDouble()
        {
            // Arrange
            var value = (1.5).ToString(CultureInfo.CurrentCulture);

            // Act
            var result = value.CastTo<double>();

            // Assert
            result.ToString(CultureInfo.CurrentCulture).Should().Be(value);
        }

        [Test]
        public void CastTo_ShouldCastValue_WhenStringRepresentationOfIntCastToDouble()
        {
            // Arrange
            var value = (2).ToString();

            // Act
            var result = value.CastTo<double>();

            // Assert
            result.ToString(CultureInfo.InvariantCulture).Should().Be(value);
        }

        [Test]
        public void CastTo_ShouldCastValue_WhenStringRepresentationOfIntCastToInt()
        {
            // Arrange
            var value = (2).ToString();

            // Act
            var result = value.CastTo<int>();

            // Assert
            result.ToString().Should().Be(value);
        }

        [Test]
        public void AsEnumerable_ShouldReturnEnumerableWithOnlyTheGivenObject()
        {
            // Arrange
            var obj = new object();

            // Act
            var result = obj.AsEnumerable();

            // Assert
            Assert.That(result, Is.EquivalentTo(new[] {obj}));
        }

        [Test]
        public void AsEnumerable_ShouldReturnEnumerableWithOnlyNull_WhenGivenObjectNull()
        {
            // Arrange
            object obj = null;

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull since we like to test this
            var result = obj.AsEnumerable();

            // Assert
            // ReSharper disable once ExpressionIsAlwaysNull since we like to test this
            Assert.That(result, Is.EquivalentTo(new[] { obj }));
        }

        [Test]
        public void AsEnumerable_ShouldReturnEnumerableWithOnlyTheGivenValueType()
        {
            // Arrange
            const int obj = 4711;

            // Act
            var result = obj.AsEnumerable();

            // Assert
            Assert.That(result, Is.EquivalentTo(new[] { obj }));
        }

/*
        [Test]
        public void CastTo_ShouldReturnDefault_WhenUsedWithDefaultAndObjNull()
        {
            // Arrange
            var defaultValue = 4711;
            int? value = null;

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull since we like to test this
            var result = value.CastTo(defaultValue);

            // Assert
            result.Should().Be(defaultValue);
        }

        [Test]
        public void CastTo_ShouldCastNormally_WhenUsedWithDefaultAndObjNotNull()
        {
            // Arrange
            var defaultValue = 4711;
            int? value = 15;

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull since we like to test this
            var result = value.CastTo(defaultValue);

            // Assert
            result.Should().Be(value.Value);
        }

        [Test]
        public void AsArray_ShouldReturnArrayWithOnlyTheGivenObject()
        {
            // Arrange
            var obj = new object();

            // Act
            var result = obj.AsArray();

            // Assert
            Assert.That(result, Is.EquivalentTo(new[] { obj }));
        }

        [Test]
        public void AsArray_ShouldReturnArrayWithOnlyNull_WhenGivenObjectNull()
        {
            // Arrange
            object obj = null;

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull since we like to test this
            var result = obj.AsArray();

            // Assert
            Assert.That(result, Is.EquivalentTo(new[] { obj }));
            // ReSharper disable once ExpressionIsAlwaysNull since we like to test this
        }

        [Test]
        public void AsArray_ShouldReturnArrayWithOnlyTheGivenValueType()
        {
            // Arrange
            const int obj = 4711;

            // Act
            var result = obj.AsArray();

            // Assert
            Assert.That(result, Is.EquivalentTo(new[] { obj }));
        }
*/
    }
}
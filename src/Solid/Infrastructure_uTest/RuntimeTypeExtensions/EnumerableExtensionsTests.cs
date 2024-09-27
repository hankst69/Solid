//----------------------------------------------------------------------------------
// File: "EnumerableExtensionsTests.cs"
// Date: 2017-2018
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Solid.Infrastructure.RuntimeTypeExtensions;
using NUnit.Framework;

namespace Solid.Infrastructure_uTest.RuntimeTypeExtensions
{
    public class EnumerableExtensionsTests
    {
        private readonly IEnumerable<int> m_Ints = new List<int> { 1, 2, 3, 4 };
        private readonly IEnumerable<int> m_NullEnum = null;

        [Test]
        public void ForEach_ShouldThrow_WhenEnumIsNull()
        {
            // Arrange
            // Act
            Action action = () => m_NullEnum.ForEach(_ => { });

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ForEach_ShouldThrow_WheActionIsNull()
        {
            // Arrange
            // Act
            Action action = () => m_Ints.ForEach(null);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ForEach_ShouldIterateAllItems_WhenEnumAndActionNotNull()
        {
            // Arrange
            var results = new List<int>();

            // Act
            m_Ints.ForEach(results.Add);

            // Assert
            m_Ints.Should().Equal(results);
        }

        [Test]
        public void SequenceEquivalent_ShouldBeTrue_WhenBothEnumsAreNull()
        {
            // Arrange
            IEnumerable<int> items1 = null;
            IEnumerable<int> items2 = null;

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull
            var result = items1.SequenceEquivalent(items2);

            // Assert
            result.Should().BeTrue();
        }


        [Test]
        public void SequenceEquivalent_ShouldBeFalse_WhenUsingEqualsAndFirstEnumerableNull()
        {
            // Arrange
            IEnumerable<int> items1 = null;
            IEnumerable<int> items2 = new[] { 1, 2, 3, 4, 5 };

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull
            var result = items1.SequenceEquivalent(items2);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void SequenceEquivalent_ShouldBeFalse_WhenUsingEqualsAndSecondEnumerableNull()
        {
            IEnumerable<int> items = new[] { 1, 2, 3, 4, 5 };

            // Act
            var result = items.SequenceEquivalent(null);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void SequenceEquivalent_ShouldBeFalse_WhenUsingEqualsAndDifferentSize()
        {
            // Arrange
            IEnumerable<int> items1 = new[] { 1 };
            IEnumerable<int> items2 = new[] { 1, 2 };

            // Act
            var result = items1.SequenceEquivalent(items2);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void SequenceEquivalent_ShouldBeFalse_WhenUsingEqualsAndDisjointItems()
        {
            // Arrange
            IEnumerable<int> items1 = new[] { 1, 2 };
            IEnumerable<int> items2 = new[] { 3, 4 };

            // Act
            var result = items1.SequenceEquivalent(items2);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void SequenceEquivalent_ShouldBeFalse_WhenUsingEqualsAndOnlySomeItemsShared()
        {
            // Arrange
            IEnumerable<int> items1 = new[] { 1, 2, 3, 4, 5 };
            IEnumerable<int> items2 = new[] { 1, 2, 4, 4, 5 };

            // Act
            var result = items1.SequenceEquivalent(items2);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void SequenceEquivalent_ShouldBeTrue_WhenUsingEqualsAndBothEnumerablesNull()
        {
            // Arrange
            IEnumerable<object> items = null;

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull
            var result = items.SequenceEquivalent(null);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void SequenceEquivalent_ShouldBeTrue_WhenUsingEqualsAndBothEnumerablesEmpty()
        {
            // Arrange
            var items1 = new List<object>();
            var items2 = Enumerable.Empty<object>();

            // Act
            var result = items1.SequenceEquivalent(items2);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void SequenceEquivalent_ShouldBeTrue_WhenUsingEqualsAndSameItems()
        {
            // Arrange
            var items = new[] { 1, 2, 3, 4, 5 };

            // Act
            var result = items.SequenceEquivalent(items);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void SequenceEquivalent_ShouldBeTrue_WhenUsingEqualsAndSameItemsInSameOrder()
        {
            // Arrange
            var items1 = new[] { 1, 2, 3, 4, 5 };
            var items2 = items1.ToList();

            // Act
            var result = items1.SequenceEquivalent(items2);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void SequenceEquivalent_ShouldBeTrue_WhenUsingEqualsAndSameItemsInReverseOrder()
        {
            // Arrange
            var items1 = new[] { 1, 2, 3, 4, 5 };
            var items2 = items1.Reverse().ToList();

            // Act
            var result = items1.SequenceEquivalent(items2);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void SequenceEquivalent_ShouldBeTrue_WhenUsingEqualsAndSameItemsInDifferentOrder()
        {
            // Arrange
            var items1 = new[] { 1, 2, 3, 4, 5 };
            var items2 = new[] { 2, 4, 5, 1, 3 };

            // Act
            var result = items1.SequenceEquivalent(items2);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void SequenceEquivalent_ShouldBeTrue_WhenUsingEqualsAndSameItemsWithDuplicatesInSameOrder()
        {
            // Arrange
            var items1 = new[] { 1, 2, 2, 2, 3 };
            var items2 = items1.ToList();

            // Act
            var result = items1.SequenceEquivalent(items2);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void SequenceEquivalent_ShouldBeTrue_WhenUsingEqualsAndSameItemsWithDuplicatesInDifferent()
        {
            // Arrange
            var items1 = new[] { 1, 2, 2, 2, 3 };
            var items2 = new[] { 2, 3, 2, 1, 2 };

            // Act
            var result = items1.SequenceEquivalent(items2);

            // Assert
            result.Should().BeTrue();
        }

    }
}
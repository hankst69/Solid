//----------------------------------------------------------------------------------
// File: "ListExtensionsTests.cs"
// Date: 2015-2019
//----------------------------------------------------------------------------------

using NUnit.Framework;
using System;
using System.Collections.Generic;
using Solid.Infrastructure.RuntimeTypeExtensions;

namespace Solid.Infrastructure_uTest.RuntimeTypeExtensions
{
    [TestFixture]
    public class ListExtensionsTests
    {
        [Test]
        public void IndexOf_ShouldReturnCorrectIndexForCondition()
        {
            //Arrange
            var list = new List<int> { 0, 1, 2, 3, 4, 5, 6 };

            //Act
            var result = list.IndexOf(o => o == 3);

            //Assert
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void IndexOf_ShouldReturnMinusOneIfNoItemMeetsCondition()
        {
            //Arrange
            var list = new List<int> { 0, 1, 2, 4, 5, 6 };

            //Act
            var result = list.IndexOf(o => o == 3);

            //Assert
            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void IndexOf_ShouldThrow_WhenListIsNull()
        {
            List<int> foo = null;
            Assert.Throws(typeof(ArgumentNullException), () => foo.IndexOf(o => true));
        }

        [Test]
        public void IndexOf_ShouldThrow_WhenConditionIsNull()
        {
            List<int> foo = new List<int> { 1, 2, 3 };
            Assert.Throws(typeof(ArgumentNullException), () => foo.IndexOf(null));
        }

        [Test]
        public void Range_ShouldReturnRange()
        {
            //Arrange
            var target = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var expected = new List<int> { 3, 4, 5 };

            //Act
            var result = target.Range(2, 4);

            //Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void AddRange_ShouldThrow_WhenListNull()
        {
            //Arrange
            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => ((IList<string>)null).AddRange(new List<string>()));
        }

        [Test]
        public void AddRange_ShouldThrow_WhenItemsNull()
        {
            //Arrange
            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => new List<string>().As<IList<string>>().AddRange(null));
        }


        [TestCase(-1)]
        [TestCase(42)]
        public void Range_ShouldThrow_WhenStartIndexOutOfBounds(int index)
        {
            //Arrange
            var target = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            //Act
            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Range(index, 2));
        }

        [Test]
        public void Range_ShouldThrow_WhenListNull()
        {
            //Arrange
            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => ((IList<string>)null).Range(0, 1));
        }

        [TestCase(-1)]
        [TestCase(42)]
        public void Range_ShouldThrow_WhenEndindexOutOfBounds(int index)
        {
            //Arrange
            var target = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            //Act
            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Range(2, index));
        }

        [Test]
        public void Range_ShouldThrow_WhenEndIndexSmallerThanStartIndex()
        {
            //Arrange
            var target = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            //Act
            //Assert
            Assert.Throws<ArgumentException>(() => target.Range(4, 2));
        }
    }
}

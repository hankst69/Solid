//----------------------------------------------------------------------------------
// File: "DataSetBuilderTests.cs"
// Author: Steffen Hanke
// Date: 2015-2019
//----------------------------------------------------------------------------------

using System;
using FluentAssertions;
using Solid.TestInfrastructure.Dicom;
using NUnit.Framework;
using syngo.Services.DataUtilities.DataDefinition.Constants;

namespace Solid.TestInfrastructure_uTest.Dicom
{
    [TestFixture]
    public class DataSetBuilderTests
    {
        [Test]
        public void ToDataSet_ShouldReturnIDataSet()
        {
            // Arrange
            var builder = new DataSetBuilder();

            // Act
            var dataset = builder.ToDataSet();

            // Assert
            dataset.Should().NotBeNull();
            dataset.GetType().Name.Should().Be("IDataSetProxy");
        }

        [Test]
        public void ToFrameMock_ShouldReturnMockOfFrame()
        {
            // Arrange
            var builder = new DataSetBuilder();

            // Act
            var frameMock = builder.ToFrameMock();

            // Assert
            frameMock.Should().NotBeNull();
            frameMock.Object.GetType().Name.Should().Be("IFrameProxy");
        }

        [Test]
        public void ToDataItemMock_ShouldReturnMockOfDataItem()
        {
            // Arrange
            var builder = new DataSetBuilder();

            // Act
            var itemMock = builder.ToDataItemMock();

            // Assert
            itemMock.Should().NotBeNull();
            itemMock.Object.GetType().Name.Should().Be("IFrameItemProxy");
        }

        [Test]
        public void SetupTag_ShouldAddDataElementWithExpectedValue_WhenTagWasNotYetSpecified()
        {
            // Arrange
            var builder = new DataSetBuilder();
            const long tag = LocalTag.Tag.InstanceNumber;
            const string tagValue = "1";
            const int tagIndex = 0;

            // Act
            builder.SetupTag(tag, tagValue, tagIndex);

            // Assert
            builder.ToDataSet()[tag][tagIndex].Should().BeEquivalentTo(tagValue);
        }

        [Test]
        public void SetupTag_ShouldThrow_WhenTagWasAlreadySpecified()
        {
            // Arrange
            var builder = new DataSetBuilder();
            const long tag = LocalTag.Tag.InstanceNumber;
            const string tagValue = "1";
            const int tagIndex = 0;
            builder.SetupTag(tag, tagValue, tagIndex);

            // Act
            Action result = () => builder.SetupTag(tag, tagValue, tagIndex);

            // Assert
            result.Should().Throw<ArgumentException>();
        }

        [Test]
        public void SetupEmptyTag_ShouldAddEmptyDataElement_WhenTagWasNotYetSpecified()
        {
            // Arrange
            var builder = new DataSetBuilder();
            const long tag = LocalTag.Tag.InstanceNumber;

            // Act
            builder.SetupEmptyTag(tag);

            // Assert
            builder.ToDataSet()[tag].Count.Should().Be(0);
            builder.ToDataSet()[tag].IsElementEmpty().Should().Be(true);
            Action access = () => { var foo = builder.ToDataSet()[tag][0]; };
            access.Should().Throw<IndexOutOfRangeException>("");
        }

        [Test]
        public void SetupEmptyTag_ShouldThrow_WhenTagWasAlreadySpecified()
        {
            // Arrange
            var builder = new DataSetBuilder();
            const long tag = LocalTag.Tag.InstanceNumber;
            builder.SetupEmptyTag(tag);

            // Act
            Action result = () => builder.SetupEmptyTag(tag);

            // Assert
            result.Should().Throw<ArgumentException>();
        }

        [Test]
        public void SetupMvTag_ShouldAddDataElementWithExpectedValue_WhenTagWasNotYetSpecified()
        {
            // Arrange
            var builder = new DataSetBuilder();
            const long tag = LocalTag.Tag.ImageType;
            const string tagValue = "1";
            const int tagMultiplicity = 4;

            // Act
            builder.SetupMvTag(tag, tagValue, tagMultiplicity);

            // Assert
            builder.ToDataSet()[tag].Count.Should().Be(1);
            builder.ToDataSet()[tag][0].Should().BeEquivalentTo(tagValue);
        }

        [Test]
        public void SetupMvTag_ShouldIncreaseElementCountAndAddDataElementWithExpectedValue_WhenTagMultiplicityWasNotYetReached()
        {
            // Arrange
            var builder = new DataSetBuilder();
            const long tag = LocalTag.Tag.ImageType;
            const string tagValue1 = "1";
            const string tagValue2 = "2";
            const int tagMultiplicity = 4;

            // Act
            builder.SetupMvTag(tag, tagValue1, tagMultiplicity);
            builder.SetupMvTag(tag, tagValue2, tagMultiplicity);

            // Assert
            builder.ToDataSet()[tag].Count.Should().Be(2);
            builder.ToDataSet()[tag][0].Should().BeEquivalentTo(tagValue1);
            builder.ToDataSet()[tag][1].Should().BeEquivalentTo(tagValue2);
        }

        [Test]
        public void SetupMvTag_ShouldThrow_WhenTagMultiplicityWasAlreadyReached()
        {
            // Arrange
            var builder = new DataSetBuilder();
            const long tag = LocalTag.Tag.ImageType;
            const string tagValue1 = "1";
            const string tagValue2 = "2";
            const string tagValue3 = "3";
            const string tagValue4 = "4";
            const string tagValue5 = "5";
            const int tagMultiplicity = 4;

            // Act
            builder.SetupMvTag(tag, tagValue1, tagMultiplicity);
            builder.SetupMvTag(tag, tagValue2, tagMultiplicity);
            builder.SetupMvTag(tag, tagValue3, tagMultiplicity);
            builder.SetupMvTag(tag, tagValue4, tagMultiplicity);
            Action action = () => builder.SetupMvTag(tag, tagValue5, tagMultiplicity);

            // Assert
            action.Should().Throw<IndexOutOfRangeException>();
        }

        [Test]
        public void RemoveTag_ShouldRemoveDataElement()
        {
            // Arrange
            var builder = new DataSetBuilder();
            const long tag = LocalTag.Tag.InstanceNumber;
            const string tagValue = "1";
            const int tagIndex = 0;
            builder.SetupTag(tag, tagValue, tagIndex);

            // Act
            builder.RemoveTag(tag);

            // Assert
            builder.ToDataSet()[tag].Should().Be(null);
        }

        [Test]
        public void GetOrCreateSequence_ShouldReturnNewSequence_WhenCalledFirstTime()
        {
            // Arrange
            var builder = new DataSetBuilder();
            const long tag = LocalTag.Tag.SharedFunctionalGroupsSequence;
            const int tagIndex = 0;

            // Act
            var sequence = builder.GetOrCreateSequence(tag, tagIndex);

            // Assert
            sequence.Should().NotBeNull();
        }

        [Test]
        public void GetOrCreateSequence_ShouldReturnExistingSequence_WhenCalledSecondTime()
        {
            // Arrange
            var builder = new DataSetBuilder();
            const long tag = LocalTag.Tag.SharedFunctionalGroupsSequence;
            const int tagIndex = 0;
            var firstSequence = builder.GetOrCreateSequence(tag, tagIndex);

            // Act
            var secondSequence = builder.GetOrCreateSequence(tag, tagIndex);

            // Assert
            firstSequence.Should().NotBeNull();
            secondSequence.Should().NotBeNull();
            ReferenceEquals(firstSequence, secondSequence).Should().BeTrue();
        }

    }
}

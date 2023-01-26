//----------------------------------------------------------------------------------
// <copyright file="DataItemMockTests.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2015-2019. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using FluentAssertions;
using Solid.TestInfrastructure.Dicom;
using NUnit.Framework;
using syngo.Services.DataUtilities.DataDefinition.Constants;

namespace Solid.TestInfrastructure_uTest.Dicom
{
    [TestFixture]
    public class DataItemMockTests
    {
        [Test]
        public void SyngoUID_ShouldDelegateToDataSet()
        {
            // Arrange
            var dataSetBuilder = new DataSetBuilder();
            var dataItemMock = dataSetBuilder.ToDataItemMock();

            // Act
            var syngoUid = dataItemMock.Object.SyngoUID;

            // Assert
            dataSetBuilder.Verify(x => x.SyngoUid, Moq.Times.Once);
        }

        [Test]
        public void IndexOperator_ShouldDelegateToDataSet()
        {
            // Arrange
            var tag = LocalTag.Tag.SeriesDescription;
            var value = "testName";
            var dataSetBuilder = new DataSetBuilder();
            dataSetBuilder.SetupTag(tag, value, 0);
            var dataItemMock = dataSetBuilder.ToDataItemMock();

            // Act
            var dataElement = dataItemMock.Object[tag];

            // Assert
            dataSetBuilder.Verify(x => x[tag], Moq.Times.Once);
            ReferenceEquals(dataItemMock.Object[tag], dataSetBuilder.ToDataSet()[tag]).Should().BeTrue();
        }

        [Test]
        public void Contains_ShouldDelegateToDataSet()
        {
            // Arrange
            var tag = LocalTag.Tag.SourceofAnteriorChamberDepthDataCodeSequence;
            var dataSetBuilder = new DataSetBuilder();
            var dataItemMock = dataSetBuilder.ToDataItemMock();

            // Act
            var exists = dataItemMock.Object.Contains(tag);

            // Assert
            dataSetBuilder.Verify(x => x.Contains(tag), Moq.Times.Once);
        }

        [Test]
        public void RetrieveDataSet_ShouldReturnReferenceToDataSet()
        {
            // Arrange
            var dataSetBuilder = new DataSetBuilder();
            var dataItemMock = dataSetBuilder.ToDataItemMock();

            // Act
            var dataSet = dataItemMock.Object.RetrieveDataSet();

            // Assert
            ReferenceEquals(dataSet, dataSetBuilder.ToDataSet()).Should().BeTrue();
        }

        [Test]
        public void Frame_ShouldReturnProperFrameMockObject()
        {
            // Arrange
            var frameNumber = 2;
            var dataSetBuilder = new DataSetBuilder();
            var dataItemMock = dataSetBuilder.ToDataItemMock(frameNumber);

            // Act
            var frame = dataItemMock.Object.Frame;

            // Assert
            frame.Should().NotBeNull();
            frame.FrameNumber.Should().Be(frameNumber);
            frame.GetType().Name.Should().Be("IFrameProxy");
        }
    }
}

//----------------------------------------------------------------------------------
// File: "FrameMockTests.cs"
// Author: Steffen Hanke
// Date: 2015-2019
//----------------------------------------------------------------------------------

using FluentAssertions;
using Solid.Infrastructure.RuntimeTypeExtensions;
using Solid.TestInfrastructure.Dicom;
using Solid.TestInfrastructure.FluentAssertions;
using NUnit.Framework;
using syngo.Services.DataUtilities.DataDefinition.Constants;
using syngo.Services.ImageProcessing.Maths;

namespace Solid.TestInfrastructure_uTest.Dicom
{
    [TestFixture]
    public class FrameMockTests
    {
        [Test]
        public void SyngoUID_ShouldDelegateToDataSet()
        {
            // Arrange
            var dataSetBuilder = new DataSetBuilder();
            var frameMock = dataSetBuilder.ToFrameMock();

            // Act
            var syngoUid = frameMock.Object.SyngoUID;

            // Assert
            dataSetBuilder.Verify(x => x.SyngoUid, Moq.Times.Once);
        }

        [Test]
        public void IndexOperator_ShouldDelegateToDataSet()
        {
            // Arrange
            var dataSetBuilder = new DataSetBuilder();
            dataSetBuilder.SetupTag(LocalTag.Tag.SopClassUid, "testSopClassUId", 0);
            var frameMock = dataSetBuilder.ToFrameMock();
            var tag = LocalTag.Tag.SourceofAnteriorChamberDepthDataCodeSequence;

            // Act
            var dataElement = frameMock.Object[tag];

            // Assert
            dataSetBuilder.Verify(x => x[tag], Moq.Times.Once);
            ReferenceEquals(frameMock.Object[tag], dataSetBuilder.ToDataSet()[tag]).Should().BeTrue();
        }

        [Test]
        public void Contains_ShouldDelegateToDataSet()
        {
            // Arrange
            var dataSetBuilder = new DataSetBuilder();
            var frameMock = dataSetBuilder.ToFrameMock();
            var tag = LocalTag.Tag.SourceofAnteriorChamberDepthDataCodeSequence;

            // Act
            var exists = frameMock.Object.Contains(tag);

            // Assert
            dataSetBuilder.Verify(x => x.Contains(tag), Moq.Times.Once);
        }

        [Test]
        public void FrameNumber_ShouldMatchNumberSpecifiedInCtor()
        {
            // Arrange
            var expectedFrameNumber = 55;
            var dataSetBuilder = new DataSetBuilder();
            var frameMock = dataSetBuilder.ToFrameMock(expectedFrameNumber);

            // Act
            var frameNumber = frameMock.Object.FrameNumber;

            // Assert
            //frameNumber--;
            frameNumber.Should().Be(expectedFrameNumber);
        }

        [Test]
        public void FrameInfo_ShouldNotBeNull()
        {
            // Arrange
            var dataSetBuilder = new DataSetBuilder();
            var frameMock = dataSetBuilder.ToFrameMock();

            // Act
            var frameInfo = frameMock.Object.FrameInfo;

            // Assert
            frameInfo.Should().NotBeNull();
        }

        [Test]
        public void FrameInfoImagePlaneValues_ShouldMatchMrImageDatasetValues()
        {
            // Arrange
            var frameNumber = 1;
            var position = new Vector3D(50,50,50);
            var row = new Vector3D(1,0,0);
            var column = new Vector3D(0,1,0);
            
            var mrDataSet = new MrImageBuilder()
                .WithImagePlane(position, row, column);
            var frameMock = mrDataSet.ToFrameMock(frameNumber);

            // Act
            var frameInfo = frameMock.Object.FrameInfo;

            // Assert
            frameInfo.ImagePosition.Should().BeAlmostEqual(position);
            frameInfo.ImageOrientationRow.Should().BeAlmostEqual(row);
            frameInfo.ImageOrientationColumn.Should().BeAlmostEqual(column);
        }

        [Test]
        public void FrameInfoImageMatrixValues_ShouldMatchMrImageDatasetValues()
        {
            // Arrange
            var frameNumber = 1;
            var rows = 100;
            var cols = 400;
            var pixelX = 0.5;
            var pixelY = 2.0;
            
            var mrDataSet = new MrImageBuilder()
                .WithImageMatrix(rows, cols, pixelX, pixelY);
            var frameMock = mrDataSet.ToFrameMock(frameNumber);

            // Act
            var frameInfo = frameMock.Object.FrameInfo;

            // Assert
            frameInfo.Slice.GetImageInfo().PixelCountX.Should().Be(cols);
            frameInfo.Slice.GetImageInfo().PixelCountY.Should().Be(rows);
            frameInfo.Slice.GetImageInfo().PixelSizeX.Should().Be(pixelX);
            frameInfo.Slice.GetImageInfo().PixelSizeY.Should().Be(pixelY);
        }

        [Test]
        public void FrameInfoImagePlaneValues_ShouldMatchEnhancedMrImageDatasetPerFrameValues()
        {
            // Arrange
            var frameNumber = 33;
            var position = new Vector3D(50, 50, 50);
            var row = new Vector3D(1, 0, 0);
            var column = new Vector3D(0, 1, 0);
            
            var mrDataSet = new EnhancedMrImageBuilder()
                .UsingFrame(frameNumber)
                .WithImagePlane(position, row, column);
            var frameMock = mrDataSet.ToFrameMock(frameNumber);

            // Act
            var frameInfo = frameMock.Object.FrameInfo;

            // Assert
            frameMock.Object.FrameNumber.Should().Be(frameNumber);
            frameInfo.ImagePosition.Should().BeAlmostEqual(position);
            frameInfo.ImageOrientationRow.Should().BeAlmostEqual(row);
            frameInfo.ImageOrientationColumn.Should().BeAlmostEqual(column);
        }

        [Test]
        public void FrameInfoImageMatrixValues_ShouldMatchEnhancedMrImageDatasetValues()
        {
            // Arrange
            var frameNumber = 1;
            var rows = 100;
            var cols = 400;
            var pixelX = 0.5;
            var pixelY = 2.0;
            
            var mrDataSet = new EnhancedMrImageBuilder()
                .WithImageMatrix(rows, cols, pixelX, pixelY);
            var frameMock = mrDataSet.ToFrameMock(frameNumber);

            // Act
            var frameInfo = frameMock.Object.FrameInfo;

            // Assert
            frameMock.Object.FrameNumber.Should().Be(frameNumber);
            frameInfo.Slice.GetImageInfo().PixelCountX.Should().Be(cols);
            frameInfo.Slice.GetImageInfo().PixelCountY.Should().Be(rows);
            frameInfo.Slice.GetImageInfo().PixelSizeX.Should().Be(pixelX);
            frameInfo.Slice.GetImageInfo().PixelSizeY.Should().Be(pixelY);
        }

        [Test]
        public void FrameIndexOperatorForTagImagePositionPatientAndTagImageOrientationPatient_ShouldReturnDatasetPerFrameValues()
        {
            // Arrange
            var frameNumber = 33;
            var position = new Vector3D(50, 50, 50);
            var row = new Vector3D(1, 0, 0);
            var column = new Vector3D(0, 1, 0);

            var mrDataSet = new EnhancedMrImageBuilder()
                .UsingFrame(frameNumber)
                .WithImagePlane(position, row, column);
            var frameMock = mrDataSet.ToFrameMock(frameNumber);

            // Act
            // Assert
            frameMock.Object.FrameNumber.Should().Be(frameNumber);
            frameMock.Object.Contains(LocalTag.Tag.ImagePositionPatient).Should().BeTrue();
            new Vector3D(
                frameMock.Object[LocalTag.Tag.ImagePositionPatient][0].CastTo<double>(),
                frameMock.Object[LocalTag.Tag.ImagePositionPatient][1].CastTo<double>(),
                frameMock.Object[LocalTag.Tag.ImagePositionPatient][2].CastTo<double>())
                .Should().BeAlmostEqual(position);
            new Vector3D(
                frameMock.Object[LocalTag.Tag.ImageOrientationPatient][0].CastTo<double>(),
                frameMock.Object[LocalTag.Tag.ImageOrientationPatient][1].CastTo<double>(),
                frameMock.Object[LocalTag.Tag.ImageOrientationPatient][2].CastTo<double>())
                .Should().BeAlmostEqual(row);
            frameMock.Object.Contains(LocalTag.Tag.ImageOrientationPatient).Should().BeTrue();
            new Vector3D(
                frameMock.Object[LocalTag.Tag.ImageOrientationPatient][3].CastTo<double>(),
                frameMock.Object[LocalTag.Tag.ImageOrientationPatient][4].CastTo<double>(),
                frameMock.Object[LocalTag.Tag.ImageOrientationPatient][5].CastTo<double>())
                .Should().BeAlmostEqual(column);
        }

    }
}

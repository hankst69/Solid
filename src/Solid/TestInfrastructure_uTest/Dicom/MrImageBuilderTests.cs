//----------------------------------------------------------------------------------
// File: "MrImageBuilderTests.cs"
// Author: Steffen Hanke
// Date: 2015-2019
//----------------------------------------------------------------------------------

using System;
using System.Globalization;
using NUnit.Framework;
using FluentAssertions;
using Solid.Infrastructure.RuntimeTypeExtensions;
using Solid.TestInfrastructure.Dicom;
using Solid.Infrastructure.Math;

using MrPrivateDicomTags = Solid.Dicom.DicomTags.MrPrivateDicomTags;
using LocalTag = Solid.Dicom.DicomTags;
using OrientationType = Solid.Dicom.ImageInfo.Types.OrientationType;
using SopClassUids = Solid.Dicom.DicomTags.MrSopClassUids;

namespace Solid.TestInfrastructure_uTest.Dicom
{
    // resolve ambiguity between: 'Solid.Infrastructure.RuntimeTypeExtensions.ObjectExtensions.As<T>(object)' and 'FluentAssertions.AssertionExtensions.As<TTo>(object)'
    internal static class LocalObjectExtensions
    {
        internal static T As<T>(this object obj) where T : class
        {
            return ObjectExtensions.As<T>(obj);
        }
    }

    [TestFixture]
    public class MrImageBuilderTests
    {
        private MrImageBuilder m_Builder;

        [SetUp]
        public void Setup()
        {
            m_Builder = new MrImageBuilder();
        }

        [Test]
        public void Ctor_ShouldSetModalityMR()
        {
            // Arrange
            // Act
            var builder = new MrImageBuilder();

            // Assert
            builder.ToDataSet()[LocalTag.Tag.Modality][0].As<string>().Should().BeEquivalentTo("MR");
        }

        [Test]
        public void Ctor_ShouldSetSopClassUidMagneticResonanceImage()
        {
            // Arrange
            // Act
            var builder = new MrImageBuilder();

            // Assert
            builder.ToDataSet()[LocalTag.Tag.SopClassUid][0].As<string>().Should().BeEquivalentTo(SopClassUids.MR_IMAGE);
        }

        [Test]
        public void WithoutTag_ShouldRemoveTagFromDataset()
        {
            // Arrange
            var tag = LocalTag.Tag.FrameOfReferenceUid;
            var value = "test";
            var builder = new MrImageBuilder().WithFrameOfReferenceUid(value);

            // Act
            Action action = () => builder.WithoutTag(tag);

            // Assert
            builder.ToDataSet().Contains(tag).Should().BeTrue();
            builder.ToDataSet()[tag].Count.Should().Be(1);
            builder.ToDataSet()[tag][0].Should().BeEquivalentTo(value);
            action();
            builder.ToDataSet().Contains(tag).Should().BeFalse();
            builder.ToDataSet()[tag].Should().BeNull();
        }

        [Test]
        public void WithEmptyTag_ShouldAddEmptyTagToDataset()
        {
            // Arrange
            var tag = LocalTag.Tag.FrameOfReferenceUid;
            var builder = new MrImageBuilder();

            // Act
            builder.WithEmptyTag(tag);

            // Assert
            builder.ToDataSet().Contains(tag).Should().BeTrue();
            builder.ToDataSet()[tag].Should().NotBeNull();
            builder.ToDataSet()[tag].Count.Should().Be(0);
            builder.ToDataSet()[tag].IsElementEmpty().Should().BeTrue();
        }

        [Test]
        public void WithFrameOfReferenceUid_ShouldCreateTagFrameOfReferenceUidWithMatchingValue()
        {
            // Arrange
            var tag = LocalTag.Tag.FrameOfReferenceUid;
            var value = "123456789";

            // Act
            var dataset = m_Builder
                .WithFrameOfReferenceUid(value)
                .ToDataSet();

            // Assert
            dataset.Contains(tag).Should().BeTrue();
            dataset[tag].Count.Should().Be(1);
            dataset[tag][0].Should().BeEquivalentTo(value);
        }

        [Test]
        public void WithImageType_ShouldCreateTagImageTypeWithMatchingValue()
        {
            // Arrange
            var tag = LocalTag.Tag.ImageType;
            var value = "123456789";

            // Act
            var dataset = m_Builder
                .WithImageType(value)
                .ToDataSet();

            // Assert
            dataset.Contains(tag).Should().BeTrue();
            dataset[tag].Count.Should().Be(1);
            dataset[tag][0].Should().BeEquivalentTo(value);
        }

        [Test]
        public void WithStudyInstanceUid_ShouldCreateTagStudyInstanceUidWithMatchingValue()
        {
            // Arrange
            var tag = LocalTag.Tag.StudyInstanceUid;
            var value = "123456789";

            // Act
            var dataset = m_Builder
                .WithStudyInstanceUid(value)
                .ToDataSet();

            // Assert
            dataset.Contains(tag).Should().BeTrue();
            dataset[tag].Count.Should().Be(1);
            dataset[tag][0].Should().BeEquivalentTo(value);
        }

        [Test]
        public void WithSeriesInstanceUid_ShouldCreateTagSeriesInstanceUidWithMatchingValue()
        {
            // Arrange
            var tag = LocalTag.Tag.SeriesInstanceUid;
            var value = "123456789";

            // Act
            var dataset = m_Builder
                .WithSeriesInstanceUid(value)
                .ToDataSet();

            // Assert
            dataset.Contains(tag).Should().BeTrue();
            dataset[tag].Count.Should().Be(1);
            dataset[tag][0].Should().BeEquivalentTo(value);
        }

        [Test]
        public void WithSopInstanceUid_ShouldCreateTagSopInstanceUidWithMatchingValue()
        {
            // Arrange
            var tag = LocalTag.Tag.SopInstanceUid;
            var value = "123456789";

            // Act
            var dataset = m_Builder
                .WithSopInstanceUid(value)
                .ToDataSet();

            // Assert
            dataset.Contains(tag).Should().BeTrue();
            dataset[tag].Count.Should().Be(1);
            dataset[tag][0].Should().BeEquivalentTo(value);
        }

        [Test]
        public void WithStudyDescription_ShouldCreateTagStudyDescriptionWithMatchingValue()
        {
            // Arrange
            var tag = LocalTag.Tag.StudyDescription;
            var value = "123456789";

            // Act
            var dataset = m_Builder
                .WithStudyDescription(value)
                .ToDataSet();

            // Assert
            dataset.Contains(tag).Should().BeTrue();
            dataset[tag].Count.Should().Be(1);
            dataset[tag][0].Should().BeEquivalentTo(value);
        }

        [Test]
        public void WithSeriesDescription_ShouldCreateTagSeriesDescriptionWithMatchingValue()
        {
            // Arrange
            var tag = LocalTag.Tag.SeriesDescription;
            var value = "123456789";

            // Act
            var dataset = m_Builder
                .WithSeriesDescription(value)
                .ToDataSet();

            // Assert
            dataset.Contains(tag).Should().BeTrue();
            dataset[tag].Count.Should().Be(1);
            dataset[tag][0].Should().BeEquivalentTo(value);
        }

        [Test]
        public void WithSeriesNumber_ShouldCreateTagSeriesNumberWithMatchingValue()
        {
            // Arrange
            var tag = LocalTag.Tag.SeriesNumber;
            var value = 123456789;

            // Act
            var dataset = m_Builder
                .WithSeriesNumber(value)
                .ToDataSet();

            // Assert
            dataset.Contains(tag).Should().BeTrue();
            dataset[tag].Count.Should().Be(1);
            dataset[tag][0].Should().Be(value.ToString());
        }

        [Test]
        public void WithSeriesDate_ShouldCreateTagSeriesDateWithMatchingValue()
        {
            // Arrange
            var tag = LocalTag.Tag.SeriesDate;
            var value = new DateTime(1969,9,21);

            // Act
            var dataset = m_Builder
                .WithSeriesDate(value)
                .ToDataSet();

            // Assert
            dataset.Contains(tag).Should().BeTrue();
            dataset[tag].Count.Should().Be(1);
            dataset[tag][0].Should().BeEquivalentTo(value.ToString("yyyyMMdd"));
        }

        [Test]
        public void WithInstanceNumber_ShouldCreateTagInstanceNumberWithMatchingValue()
        {
            // Arrange
            var tag = LocalTag.Tag.InstanceNumber;
            var value = 123456789;

            // Act
            var dataset = m_Builder
                .WithInstanceNumber(value)
                .ToDataSet();

            // Assert
            dataset.Contains(tag).Should().BeTrue();
            dataset[tag].Count.Should().Be(1);
            dataset[tag][0].Should().Be(value.ToString());
        }

        [Test]
        public void WithAcquisitionDateTime_FromDateAndTimeStrings_ShouldCreateTagAcquisitionDateAndTagAcquisitioTimeWithMatchingValues()
        {
            // Arrange
            var acquisitionDateTime = new DateTime(2015, 10, 30, 13, 45, 59);
            var acquisitionDate = acquisitionDateTime.ToString("yyyyMMdd");             //"20151030";
            var acquisitionTime = acquisitionDateTime.ToString("HHmmss.ffffff");        //"101505.000000";

            // Act
            var dataset = m_Builder
                 .WithAcquisitionDateTime(acquisitionDate, acquisitionTime)
                .ToDataSet();

            // Assert
            dataset.Contains(LocalTag.Tag.AcquisitionDate).Should().BeTrue();
            dataset[LocalTag.Tag.AcquisitionDate].Count.Should().Be(1);
            dataset[LocalTag.Tag.AcquisitionDate][0].Should().BeEquivalentTo(acquisitionDate);
            dataset.Contains(LocalTag.Tag.AcquisitionTime).Should().BeTrue();
            dataset[LocalTag.Tag.AcquisitionTime].Count.Should().Be(1);
            dataset[LocalTag.Tag.AcquisitionTime][0].Should().BeEquivalentTo(acquisitionTime);
        }

        [Test]
        public void WithAcquisitionDateTime_FromDateTime_ShouldCreateTagAcquisitionDateAndTagAcquisitioTimeWithMatchingValues()
        {
            // Arrange
            var acquisitionDateTime = new DateTime(2015, 10, 30, 13, 45, 59);
            var acquisitionDate = acquisitionDateTime.ToString("yyyyMMdd");             //"20151030";
            var acquisitionTime = acquisitionDateTime.ToString("HHmmss.ffffff");        //"101505.000000";

            // Act
            var dataset = m_Builder
                .WithAcquisitionDateTime(acquisitionDateTime)
                .ToDataSet();

            // Assert
            dataset.Contains(LocalTag.Tag.AcquisitionDate).Should().BeTrue();
            dataset[LocalTag.Tag.AcquisitionDate].Count.Should().Be(1);
            dataset[LocalTag.Tag.AcquisitionDate][0].Should().BeEquivalentTo(acquisitionDate);
            dataset.Contains(LocalTag.Tag.AcquisitionTime).Should().BeTrue();
            dataset[LocalTag.Tag.AcquisitionTime].Count.Should().Be(1);
            dataset[LocalTag.Tag.AcquisitionTime][0].Should().BeEquivalentTo(acquisitionTime);
        }

        [Test]
        public void WithImagePlane_FromVectors_ShouldCreateTagImagePositionPatientAndTagImageOrientationPatientWithMatchingValues()
        {
            // Arrange
            var position = new Vector3D(50, 60, 70);
            var row = new Vector3D(1, 0, 0);
            var column = new Vector3D(0, 1, 0);

            // Act
            var dataset = m_Builder
                .WithImagePlane(position, row, column)
                .ToDataSet();
 
            // Assert
            dataset.Contains(LocalTag.Tag.ImagePositionPatient).Should().BeTrue();
            dataset[LocalTag.Tag.ImagePositionPatient].Count.Should().Be(3);
            dataset[LocalTag.Tag.ImagePositionPatient][0].CastTo<double>().Should().Be(position.X);
            dataset[LocalTag.Tag.ImagePositionPatient][1].CastTo<double>().Should().Be(position.Y);
            dataset[LocalTag.Tag.ImagePositionPatient][2].CastTo<double>().Should().Be(position.Z);

            dataset.Contains(LocalTag.Tag.ImageOrientationPatient).Should().BeTrue();
            dataset[LocalTag.Tag.ImageOrientationPatient].Count.Should().Be(6);
            dataset[LocalTag.Tag.ImageOrientationPatient][0].CastTo<double>().Should().Be(row.X);
            dataset[LocalTag.Tag.ImageOrientationPatient][1].CastTo<double>().Should().Be(row.Y);
            dataset[LocalTag.Tag.ImageOrientationPatient][2].CastTo<double>().Should().Be(row.Z);
            dataset[LocalTag.Tag.ImageOrientationPatient][3].CastTo<double>().Should().Be(column.X);
            dataset[LocalTag.Tag.ImageOrientationPatient][4].CastTo<double>().Should().Be(column.Y);
            dataset[LocalTag.Tag.ImageOrientationPatient][5].CastTo<double>().Should().Be(column.Z);
        }

        [Test]
        public void WithImagePlane_FromDoubleArrays_ShouldCreateTagImagePositionPatientAndTagImageOrientationPatientWithMatchingValues()
        {
            // Arrange
            var position = new double[] { 50, 60, 70 };
            var rowcol = new double[]{ 1, 0, 0, 0, 1, 0};

            // Act
            var dataset = m_Builder
                .WithImagePlane(position, rowcol)
                .ToDataSet();

            // Assert
            dataset.Contains(LocalTag.Tag.ImagePositionPatient).Should().BeTrue();
            dataset[LocalTag.Tag.ImagePositionPatient].Count.Should().Be(3);
            dataset[LocalTag.Tag.ImagePositionPatient][0].CastTo<double>().Should().Be(position[0]);
            dataset[LocalTag.Tag.ImagePositionPatient][1].CastTo<double>().Should().Be(position[1]);
            dataset[LocalTag.Tag.ImagePositionPatient][2].CastTo<double>().Should().Be(position[2]);
            dataset.Contains(LocalTag.Tag.ImageOrientationPatient).Should().BeTrue();
            dataset[LocalTag.Tag.ImageOrientationPatient].Count.Should().Be(6);
            dataset[LocalTag.Tag.ImageOrientationPatient][0].CastTo<double>().Should().Be(rowcol[0]);
            dataset[LocalTag.Tag.ImageOrientationPatient][1].CastTo<double>().Should().Be(rowcol[1]);
            dataset[LocalTag.Tag.ImageOrientationPatient][2].CastTo<double>().Should().Be(rowcol[2]);
            dataset[LocalTag.Tag.ImageOrientationPatient][3].CastTo<double>().Should().Be(rowcol[3]);
            dataset[LocalTag.Tag.ImageOrientationPatient][4].CastTo<double>().Should().Be(rowcol[4]);
            dataset[LocalTag.Tag.ImageOrientationPatient][5].CastTo<double>().Should().Be(rowcol[5]);
        }

        [Test]
        public void WithImageMatrix_ShouldCreateTagRowsAndTagColumnsAndTagPixelSpacingWithMatchingValues()
        {
            // Arrange
            var rows = 100;
            var cols = 400;
            var pixelX = 0.5;
            var pixelY = 2.0;

            // Act
            var dataset = m_Builder
                .WithImageMatrix(rows, cols, pixelX, pixelY)
                .ToDataSet();

            // Assert
            dataset.Contains(LocalTag.Tag.Rows).Should().BeTrue();
            dataset[LocalTag.Tag.Rows][0].CastTo<int>().Should().Be(rows);
            dataset.Contains(LocalTag.Tag.Columns).Should().BeTrue();
            dataset[LocalTag.Tag.Columns][0].CastTo<int>().Should().Be(cols);

            dataset.Contains(LocalTag.Tag.PixelSpacing).Should().BeTrue();
            dataset[LocalTag.Tag.PixelSpacing].Count.Should().Be(2);
            double.Parse(dataset[LocalTag.Tag.PixelSpacing][0].As<string>(), CultureInfo.InvariantCulture).Should().Be(pixelX);
            double.Parse(dataset[LocalTag.Tag.PixelSpacing][1].As<string>(), CultureInfo.InvariantCulture).Should().Be(pixelY);
        }

        [Test]
        public void UsingFrame_ShouldThrowNotImplementedException()
        {
            // Arrange
            // Act
            Action action = () => m_Builder.UsingFrame(0);

            // Assert
            action.Should().Throw<NotImplementedException>();
        }

        [Test]
        public void WithFrameAcquisitionDateTime_ShouldThrowNotImplementedException()
        {
            // Arrange
            // Act
            Action action = () => m_Builder.WithFrameAcquisitionDateTime(string.Empty, string.Empty);

            // Assert
            action.Should().Throw<NotImplementedException>();
        }

        [Test]
        public void WithFrameType_ShouldThrowNotImplementedException()
        {
            // Arrange
            // Act
            Action action = () => m_Builder.WithFrameType(string.Empty);

            // Assert
            action.Should().Throw<NotImplementedException>();
        }

        [Test]
        public void WithTriggerTime_ShouldCreateTagTriggerTimeWithMatchingValue()
        {
            // Arrange
            var tag = LocalTag.Tag.TriggerTime;
            var value = 1.23456789;

            // Act
            var dataset = m_Builder
                .WithTriggerTime(value)
                .ToDataSet();

            // Assert
            dataset.Contains(tag).Should().BeTrue();
            dataset[tag].Count.Should().Be(1);
            dataset[tag][0].Should().Be(value.ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        public void WithTimeAfterStart_ShouldCreateTagTimeAfterStartWithMatchingValue()
        {
            // Arrange
            var tag = MrPrivateDicomTags.TimeAfterStart;
            var value = 1.23456789;

            // Act
            var dataset = m_Builder
                .WithTimeAfterStart(value)
                .ToDataSet();

            // Assert
            dataset.Contains(tag).Should().BeTrue();
            dataset[tag].Count.Should().Be(1);
            dataset[tag][0].Should().Be(value.ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        public void WithImaRelTablePos_ShouldCreateTagImaAbsTablePosWithMatchingValues()
        {
            // Arrange
            var tag = MrPrivateDicomTags.RelTablePosition;
            var position = new Vector3D(50, 60, 70);

            // Act
            var dataset = m_Builder
                .WithImaRelTablePos(position)
                .ToDataSet();

            // Assert
            dataset.Contains(tag).Should().BeTrue();
            dataset[tag].Count.Should().Be(3);
            dataset[tag][0].CastTo<double>().Should().Be(position.X);
            dataset[tag][1].CastTo<double>().Should().Be(position.Y);
            dataset[tag][2].CastTo<double>().Should().Be(position.Z);
        }

        [Test]
        public void WithDistortionCorrectionType_ShouldCreateTagDistortionCorrectionTypeWithMatchingValue()
        {
            // Arrange
            var tag = MrPrivateDicomTags.DistortionCorrectionType;
            var value = "AnyDistCor";

            // Act
            var dataset = m_Builder
                .WithDistortionCorrectionType(value)
                .ToDataSet();

            // Assert
            dataset.Contains(tag).Should().BeTrue();
            dataset[tag].Count.Should().Be(1);
            dataset[tag][0].Should().Be(value);
        }

        [Test]
        public void WithInPlanePhaseEncodingDirection_ShouldCreateTagInPlanePhaseEncodingDirectionWithMatchingValue()
        {
            // Arrange
            var tag = LocalTag.Tag.InPlanePhaseEncodingDirection;
            var value = "AnyDirection";

            // Act
            var dataset = m_Builder
                .WithInPlanePhaseEncodingDirection(value)
                .ToDataSet();

            // Assert
            dataset.Contains(tag).Should().BeTrue();
            dataset[tag].Count.Should().Be(1);
            dataset[tag][0].Should().Be(value);
        }

        [Theory]
        public void WithPhaseEncodingDirectionPositive_ShouldCreateTagWithMatchingValue(bool isPhaseEncodingDirectionPositive)
        {
            // Arrange
            var tag = MrPrivateDicomTags.PhaseEncodingDirectionPositive;
            var expectedValue = isPhaseEncodingDirectionPositive ? "1" : "0";

            // Act
            var dataset = m_Builder
                .WithPhaseEncodingDirectionPositive(isPhaseEncodingDirectionPositive)
                .ToDataSet();

            // Assert
            dataset.Contains(tag).Should().BeTrue();
            dataset[tag].Count.Should().Be(1);
            dataset[tag][0].Should().Be(expectedValue);
        }
        
        //[TestCase(ImageMode.Distortion, "DIS2D")]
        //[TestCase(ImageMode.NonDistortion, "ND")]
        //public void WithImageMode_ShouldCreateTagDistortionCorrectionTypeWithMatchingValue(ImageMode imageMode, string expectedCode)
        //{
        //    // Arrange
        //    var tag = MrPrivateDicomTags.DistortionCorrectionType;

        //    // Act
        //    var dataset = m_Builder
        //        .WithImageMode(imageMode)
        //        .ToDataSet();

        //    // Assert
        //    dataset.Contains(tag).Should().BeTrue();
        //    dataset[tag].Count.Should().Be(1);
        //    dataset[tag][0].Should().Be(expectedCode);
        //}

        //public void WithImageMode_ShouldThrowNotSupportedException_WhenUnknow()
        //{
        //    // Arrange
        //    // Act
        //    Action action = () => m_Builder.WithImageMode(ImageMode.Unknown);

        //    // Assert
        //    action.Should().Throw<NotSupportedException>();
        //}

        public void WithMainOrientation_ShouldCreateTagImagePositionPatientAndTagImageOrientationPatient()
        {
            // Arrange
            var orientationType = OrientationType.Sagittal;

            // Act
            var dataset = m_Builder
                .WithMainOrientation(orientationType)
                .ToDataSet();

            // Assert
            dataset.Contains(LocalTag.Tag.ImagePositionPatient).Should().BeTrue();
            dataset[LocalTag.Tag.ImagePositionPatient].Count.Should().Be(3);
            dataset.Contains(LocalTag.Tag.ImageOrientationPatient).Should().BeTrue();
            dataset[LocalTag.Tag.ImageOrientationPatient].Count.Should().Be(6);
        }

        [Test]
        public void WithVolumetricProperties_ShouldCreateTagVolumetricPropertiesWithMatchingValue()
        {
            // Arrange
            var tag = LocalTag.Tag.VolumetricProperties;
            var value = "AnyVolPropr";

            // Act
            var dataset = m_Builder
                .WithVolumetricProperties(value)
                .ToDataSet();

            // Assert
            dataset.Contains(tag).Should().BeTrue();
            dataset[tag].Count.Should().Be(1);
            dataset[tag][0].Should().Be(value);
        }

        [Test]
        public void WithFrameLevelVolumetricProperties_ShouldThrowNotImplementedException()
        {
            // Arrange
            // Act
            Action action = () => m_Builder.WithFrameLevelVolumetricProperties(string.Empty);

            // Assert
            action.Should().Throw<NotImplementedException>();
        }

    }
}

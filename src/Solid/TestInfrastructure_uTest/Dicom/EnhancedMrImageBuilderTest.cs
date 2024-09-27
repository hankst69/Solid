//----------------------------------------------------------------------------------
// File: "EnhancedMrImageBuilderTests.cs"
// Author: Steffen Hanke
// Date: 2015-2019
//----------------------------------------------------------------------------------
using System;
using System.Globalization;
using Solid.TestInfrastructure.Dicom;
using Solid.Infrastructure.RuntimeTypeExtensions;
using Solid.Infrastructure.Math;
using FluentAssertions;
using NUnit.Framework;

using MrPrivateDicomTags = Solid.Dicom.DicomTags.MrPrivateDicomTags;
using LocalTag = Solid.Dicom.DicomTags;
using SopClassUids = Solid.Dicom.DicomTags.MrSopClassUids;
using OrientationType = Solid.Dicom.ImageInfo.Types.OrientationType;

namespace Solid.TestInfrastructure_uTest.Dicom
{
    [TestFixture]
    public class EnhancedMrImageBuilderTests
    {
        private EnhancedMrImageBuilder m_Builder;

        [SetUp]
        public void Setup()
        {
            m_Builder = new EnhancedMrImageBuilder();
        }

        [Test]
        public void Ctor_ShouldSetModalityMR()
        {
            // Arrange
            // Act
            var builder = new EnhancedMrImageBuilder();

            // Assert
            builder.ToDataSet()[LocalTag.Tag.Modality][0].As<string>().Should().BeEquivalentTo("MR");
        }

        [Test]
        public void Ctor_ShouldSetSopClassUidEnhancedMagneticResonanceImage()
        {
            // Arrange
            // Act
            var builder = new EnhancedMrImageBuilder();

            // Assert
            builder.ToDataSet()[LocalTag.Tag.SopClassUid][0].As<string>().Should().BeEquivalentTo(SopClassUids.ENHANCED_MR_IMAGE);
        }

        [Test]
        public void WithAcquisitionDateTime_ShouldCreateTagAcquisitionDateTimeWithMatchingValues()
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
            dataset.Contains(LocalTag.Tag.AcquisitionDatetime).Should().BeTrue();
            dataset[LocalTag.Tag.AcquisitionDatetime].Count.Should().Be(1);
            dataset[LocalTag.Tag.AcquisitionDatetime][0].Should().BeEquivalentTo(string.Concat(acquisitionDate, acquisitionTime));
        }

        [Test]
        public void WithImagePlane_ShouldCreatePerFrameTagImagePositionPatientAndTagImageOrientationPatientWithMatchingValues()
        {
            // Arrange
            var frameNumber = 17;
            var position = new double[] { 50, 60, 70 };
            var rowcol = new double[] { 1, 0, 0, 0, 1, 0 };
            var builder = new EnhancedMrImageBuilder();

            // Act
            var dataset = builder
                .UsingFrame(frameNumber)
                .WithImagePlane(position, rowcol).ToDataSet();

            // Assert
            dataset.Contains(LocalTag.Tag.NumberOfFrames).Should().BeTrue();
            dataset[LocalTag.Tag.NumberOfFrames].Count.Should().Be(1);
            dataset[LocalTag.Tag.NumberOfFrames][0].CastTo<int>().Should().Be(frameNumber);

            dataset.Contains(LocalTag.Tag.PerFrameFunctionalGroupsSequence).Should().BeTrue();
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence].Count.Should().Be(frameNumber);
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber-1].Should().NotBeNull();
            var frameSequ = dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber - 1].As<IDataSet>();

            frameSequ.Contains(LocalTag.Tag.PlanePositionSequence).Should().BeTrue();
            frameSequ[LocalTag.Tag.PlanePositionSequence].Count.Should().Be(1);
            frameSequ[LocalTag.Tag.PlanePositionSequence][0].Should().NotBeNull();
            var planePosSequ = frameSequ[LocalTag.Tag.PlanePositionSequence][0].As<IDataSet>();

            planePosSequ.Contains(LocalTag.Tag.ImagePositionPatient).Should().BeTrue();
            planePosSequ[LocalTag.Tag.ImagePositionPatient].Count.Should().Be(3);
            planePosSequ[LocalTag.Tag.ImagePositionPatient][0].CastTo<double>().Should().Be(position[0]);
            planePosSequ[LocalTag.Tag.ImagePositionPatient][1].CastTo<double>().Should().Be(position[1]);
            planePosSequ[LocalTag.Tag.ImagePositionPatient][2].CastTo<double>().Should().Be(position[2]);

            frameSequ.Contains(LocalTag.Tag.PlaneOrientationSequence).Should().BeTrue();
            frameSequ[LocalTag.Tag.PlaneOrientationSequence].Count.Should().Be(1);
            frameSequ[LocalTag.Tag.PlaneOrientationSequence][0].Should().NotBeNull();
            var planeOriSequ = frameSequ[LocalTag.Tag.PlaneOrientationSequence][0].As<IDataSet>();

            planeOriSequ.Contains(LocalTag.Tag.ImageOrientationPatient).Should().BeTrue();
            planeOriSequ[LocalTag.Tag.ImageOrientationPatient].Count.Should().Be(6);
            planeOriSequ[LocalTag.Tag.ImageOrientationPatient][0].CastTo<double>().Should().Be(rowcol[0]);
            planeOriSequ[LocalTag.Tag.ImageOrientationPatient][1].CastTo<double>().Should().Be(rowcol[1]);
            planeOriSequ[LocalTag.Tag.ImageOrientationPatient][2].CastTo<double>().Should().Be(rowcol[2]);
            planeOriSequ[LocalTag.Tag.ImageOrientationPatient][3].CastTo<double>().Should().Be(rowcol[3]);
            planeOriSequ[LocalTag.Tag.ImageOrientationPatient][4].CastTo<double>().Should().Be(rowcol[4]);
            planeOriSequ[LocalTag.Tag.ImageOrientationPatient][5].CastTo<double>().Should().Be(rowcol[5]);
        }

        [Test]
        public void WithImageMatrix_ShouldCreateTagRowsAndTagColumnsAndSharedFrameTagPixelSpacingWithMatchingValues()
        {
            // Arrange
            var rows = 100;
            var cols = 400;
            var pixelX = 0.5;
            var pixelY = 2.0;
            var builder = new EnhancedMrImageBuilder();

            // Act
            var dataset = builder.WithImageMatrix(rows, cols, pixelX, pixelY).ToDataSet();

            // Assert
            dataset.Contains(LocalTag.Tag.Rows).Should().BeTrue();
            dataset[LocalTag.Tag.Rows][0].CastTo<int>().Should().Be(rows);
            dataset.Contains(LocalTag.Tag.Columns).Should().BeTrue();
            dataset[LocalTag.Tag.Columns][0].CastTo<int>().Should().Be(cols);

            dataset.Contains(LocalTag.Tag.SharedFunctionalGroupsSequence).Should().BeTrue();
            dataset[LocalTag.Tag.SharedFunctionalGroupsSequence].Count.Should().Be(1);
            dataset[LocalTag.Tag.SharedFunctionalGroupsSequence][0].Should().NotBeNull();
            var sharedSequ = dataset[LocalTag.Tag.SharedFunctionalGroupsSequence][0].As<IDataSet>();

            sharedSequ.Contains(LocalTag.Tag.PixelMeasuresSequence).Should().BeTrue();
            sharedSequ[LocalTag.Tag.PixelMeasuresSequence].Count.Should().Be(1);
            sharedSequ[LocalTag.Tag.PixelMeasuresSequence][0].Should().NotBeNull();
            var pixelMeasSequ = sharedSequ[LocalTag.Tag.PixelMeasuresSequence][0].As<IDataSet>();

            pixelMeasSequ.Contains(LocalTag.Tag.PixelSpacing).Should().BeTrue();
            pixelMeasSequ[LocalTag.Tag.PixelSpacing].Count.Should().Be(2);
            double.Parse(pixelMeasSequ[LocalTag.Tag.PixelSpacing][0].As<string>(), CultureInfo.InvariantCulture).Should().Be(pixelX);
            double.Parse(pixelMeasSequ[LocalTag.Tag.PixelSpacing][1].As<string>(), CultureInfo.InvariantCulture).Should().Be(pixelY);
        }

        public void WithImageMatrix_ShouldCreateTagRowsAndTagColumnsAndPerFrameFrameTagPixelSpacingWithMatchingValues()
        {
            // Arrange
            var rows = 100;
            var cols = 400;
            var pixelX = 0.5;
            var pixelY = 2.0;
            var frameNumber = 3;
            var builder = new EnhancedMrImageBuilder()
                .UsingFrame(frameNumber);

            // Act
            var dataset = builder.WithImageMatrix(rows, cols, pixelX, pixelY).ToDataSet();

            // Assert
            dataset.Contains(LocalTag.Tag.Rows).Should().BeTrue();
            dataset[LocalTag.Tag.Rows][0].CastTo<int>().Should().Be(rows);
            dataset.Contains(LocalTag.Tag.Columns).Should().BeTrue();
            dataset[LocalTag.Tag.Columns][0].CastTo<int>().Should().Be(cols);

            dataset.Contains(LocalTag.Tag.PerFrameFunctionalGroupsSequence).Should().BeTrue();
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence].Count.Should().BeGreaterOrEqualTo(frameNumber);
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber - 1].Should().NotBeNull();
            var perFrameSequ = dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber - 1].As<IDataSet>();

            perFrameSequ.Contains(LocalTag.Tag.PixelMeasuresSequence).Should().BeTrue();
            perFrameSequ[LocalTag.Tag.PixelMeasuresSequence].Count.Should().Be(1);
            perFrameSequ[LocalTag.Tag.PixelMeasuresSequence][0].Should().NotBeNull();
            var pixelMeasSequ = perFrameSequ[LocalTag.Tag.PixelMeasuresSequence][0].As<IDataSet>();

            pixelMeasSequ.Contains(LocalTag.Tag.PixelSpacing).Should().BeTrue();
            pixelMeasSequ[LocalTag.Tag.PixelSpacing].Count.Should().Be(2);
            double.Parse(pixelMeasSequ[LocalTag.Tag.PixelSpacing][0].As<string>(), CultureInfo.InvariantCulture).Should().Be(pixelX);
            double.Parse(pixelMeasSequ[LocalTag.Tag.PixelSpacing][1].As<string>(), CultureInfo.InvariantCulture).Should().Be(pixelY);
        }

        [Test]
        public void WithTriggerTime_ShouldCreateTagNominalCardiacTriggerDelayTimeWithMatchingValue()
        {
            // Arrange
            var value = 1.23456789;
            var frameNumber = 3;

            // Act
            var dataset = m_Builder
                .UsingFrame(frameNumber)
                .WithTriggerTime(value)
                .ToDataSet();

            // Assert
            dataset.Contains(LocalTag.Tag.PerFrameFunctionalGroupsSequence).Should().BeTrue();
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence].Count.Should().BeGreaterOrEqualTo(frameNumber);
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber-1].Should().NotBeNull();
            var perFrameSequ = dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber-1].As<IDataSet>();

            perFrameSequ.Contains(LocalTag.Tag.CardiacSynchronizationSequence).Should().BeTrue();
            perFrameSequ[LocalTag.Tag.CardiacSynchronizationSequence].Count.Should().Be(1);
            perFrameSequ[LocalTag.Tag.CardiacSynchronizationSequence][0].Should().NotBeNull();
            var cardiacSyncSequ = perFrameSequ[LocalTag.Tag.CardiacSynchronizationSequence][0].As<IDataSet>();

            cardiacSyncSequ.Contains(LocalTag.Tag.NominalCardiacTriggerDelayTime).Should().BeTrue();
            cardiacSyncSequ[LocalTag.Tag.NominalCardiacTriggerDelayTime].Count.Should().Be(1);
            cardiacSyncSequ[LocalTag.Tag.NominalCardiacTriggerDelayTime][0].CastTo<double>().Should().Be(value);
        }

        [Test]
        public void WithTimeAfterStart_ShouldCreateTagTimeAfterStartWithMatchingValue()
        {
            // Arrange
            var value = 1.23456789;
            var frameNumber = 5;

            // Act
            var dataset = m_Builder
                .UsingFrame(frameNumber)
                .WithTimeAfterStart(value)
                .ToDataSet();

            // Assert
            dataset.Contains(LocalTag.Tag.PerFrameFunctionalGroupsSequence).Should().BeTrue();
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence].Count.Should().BeGreaterOrEqualTo(frameNumber);
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber - 1].Should().NotBeNull();
            var perFrameSequ = dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber - 1].As<IDataSet>();

            perFrameSequ.Contains(MrPrivateDicomTags.SiemensMrSdiSequence).Should().BeTrue();
            perFrameSequ[MrPrivateDicomTags.SiemensMrSdiSequence].Count.Should().Be(1);
            perFrameSequ[MrPrivateDicomTags.SiemensMrSdiSequence][0].Should().NotBeNull();
            var siemensMrSdiSequ = perFrameSequ[MrPrivateDicomTags.SiemensMrSdiSequence][0].As<IDataSet>();

            siemensMrSdiSequ.Contains(MrPrivateDicomTags.TimeAfterStart).Should().BeTrue();
            siemensMrSdiSequ[MrPrivateDicomTags.TimeAfterStart].Count.Should().Be(1);
            siemensMrSdiSequ[MrPrivateDicomTags.TimeAfterStart][0].Should().Be(value.ToString(CultureInfo.InvariantCulture));
        }

        [Theory]
        public void WithPhaseEncodingDirectionPositive_ShouldCreateTagWithMatchingValue(bool isPhaseEncodingDirectionPositive)
        {
            // Arrange
            var frameNumber = 5;

            var expectedValue = isPhaseEncodingDirectionPositive ? "1" : "0";

            // Act
            var dataset = m_Builder
                .UsingFrame(frameNumber)
                .WithPhaseEncodingDirectionPositive(isPhaseEncodingDirectionPositive)
                .ToDataSet();

            // Assert
            dataset.Contains(LocalTag.Tag.PerFrameFunctionalGroupsSequence).Should().BeTrue();
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence].Count.Should().BeGreaterOrEqualTo(frameNumber);
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber - 1].Should().NotBeNull();
            var perFrameSequ = dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber - 1].As<IDataSet>();

            perFrameSequ.Contains(MrPrivateDicomTags.SiemensMrSdiSequence).Should().BeTrue();
            perFrameSequ[MrPrivateDicomTags.SiemensMrSdiSequence].Count.Should().Be(1);
            perFrameSequ[MrPrivateDicomTags.SiemensMrSdiSequence][0].Should().NotBeNull();
            var siemensMrSdiSequ = perFrameSequ[MrPrivateDicomTags.SiemensMrSdiSequence][0].As<IDataSet>();

            siemensMrSdiSequ.Contains(MrPrivateDicomTags.PhaseEncodingDirectionPositive).Should().BeTrue();
            siemensMrSdiSequ[MrPrivateDicomTags.PhaseEncodingDirectionPositive].Count.Should().Be(1);
            siemensMrSdiSequ[MrPrivateDicomTags.PhaseEncodingDirectionPositive][0].Should().Be(expectedValue);
        }

        [Test]
        public void WithImaRelTablePos_ShouldCreateTagImaAbsTablePosWithMatchingValues()
        {
            // Arrange
            var position = new Vector3D(50, 60, 70);
            var frameNumber = 7;

            // Act
            var dataset = m_Builder
                .UsingFrame(frameNumber)
                .WithImaRelTablePos(position)
                .ToDataSet();

            // Assert
            dataset.Contains(LocalTag.Tag.PerFrameFunctionalGroupsSequence).Should().BeTrue();
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence].Count.Should().BeGreaterOrEqualTo(frameNumber);
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber - 1].Should().NotBeNull();
            var perFrameSequ = dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber - 1].As<IDataSet>();

            perFrameSequ.Contains(LocalTag.Tag.MrImageFrameTypeSequence).Should().BeTrue();
            perFrameSequ[LocalTag.Tag.MrImageFrameTypeSequence].Count.Should().Be(1);
            perFrameSequ[LocalTag.Tag.MrImageFrameTypeSequence][0].Should().NotBeNull();
            var mrImageFrameTypeSequ = perFrameSequ[LocalTag.Tag.MrImageFrameTypeSequence][0].As<IDataSet>();

            mrImageFrameTypeSequ.Contains(MrPrivateDicomTags.ImaRelTablePosition).Should().BeTrue();
            mrImageFrameTypeSequ[MrPrivateDicomTags.ImaRelTablePosition].Count.Should().Be(3);
            mrImageFrameTypeSequ[MrPrivateDicomTags.ImaRelTablePosition][0].CastTo<double>().Should().Be(position.X);
            mrImageFrameTypeSequ[MrPrivateDicomTags.ImaRelTablePosition][1].CastTo<double>().Should().Be(position.Y);
            mrImageFrameTypeSequ[MrPrivateDicomTags.ImaRelTablePosition][2].CastTo<double>().Should().Be(position.Z);
        }

        [Test]
        public void WithDistortionCorrectionType_ShouldCreateTagDistortionCorrectionTypeWithMatchingValue()
        {
            // Arrange
            var value = "AnyDistCor";
            var frameNumber = 11;

            // Act
            var dataset = m_Builder
                .UsingFrame(frameNumber)
                .WithDistortionCorrectionType(value)
                .ToDataSet();

            // Assert
            dataset.Contains(LocalTag.Tag.PerFrameFunctionalGroupsSequence).Should().BeTrue();
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence].Count.Should().BeGreaterOrEqualTo(frameNumber);
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber - 1].Should().NotBeNull();
            var perFrameSequ = dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber - 1].As<IDataSet>();

            perFrameSequ.Contains(LocalTag.Tag.MrImageFrameTypeSequence).Should().BeTrue();
            perFrameSequ[LocalTag.Tag.MrImageFrameTypeSequence].Count.Should().Be(1);
            perFrameSequ[LocalTag.Tag.MrImageFrameTypeSequence][0].Should().NotBeNull();
            var frameTypeSequ = perFrameSequ[LocalTag.Tag.MrImageFrameTypeSequence][0].As<IDataSet>();

            frameTypeSequ.Contains(MrPrivateDicomTags.DistortionCorrectionType).Should().BeTrue();
            frameTypeSequ[MrPrivateDicomTags.DistortionCorrectionType].Count.Should().Be(1);
            frameTypeSequ[MrPrivateDicomTags.DistortionCorrectionType][0].Should().Be(value);
        }

        [Test]
        public void WithInPlanePhaseEncodingDirection_ShouldCreateTagInPlanePhaseEncodingDirectionWithMatchingValue()
        {
            // Arrange
            var value = "AnyDirection";
            var frameNumber = 13;

            // Act
            var dataset = m_Builder
                .UsingFrame(frameNumber)
                .WithInPlanePhaseEncodingDirection(value)
                .ToDataSet();

            // Assert
            dataset.Contains(LocalTag.Tag.SharedFunctionalGroupsSequence).Should().BeTrue();
            dataset[LocalTag.Tag.SharedFunctionalGroupsSequence].Count.Should().Be(1);
            dataset[LocalTag.Tag.SharedFunctionalGroupsSequence][0].Should().NotBeNull();
            var sharedSequ = dataset[LocalTag.Tag.SharedFunctionalGroupsSequence][0].As<IDataSet>();

            sharedSequ.Contains(LocalTag.Tag.MrFovGeometrySequence).Should().BeTrue();
            sharedSequ[LocalTag.Tag.MrFovGeometrySequence].Count.Should().Be(1);
            sharedSequ[LocalTag.Tag.MrFovGeometrySequence][0].Should().NotBeNull();
            var mrFovGeomSequ = sharedSequ[LocalTag.Tag.MrFovGeometrySequence][0].As<IDataSet>();

            mrFovGeomSequ.Contains(LocalTag.Tag.InPlanePhaseEncodingDirection).Should().BeTrue();
            mrFovGeomSequ[LocalTag.Tag.InPlanePhaseEncodingDirection].Count.Should().Be(1);
            mrFovGeomSequ[LocalTag.Tag.InPlanePhaseEncodingDirection][0].Should().Be(value);
        }

        [Test]
        public void UsingFrame_ShouldSwitchFrameNumberForPerFrameValuesToExpectedValue()
        {
            // Arrange
            var expectedFrameNumber = 17;

            // Act
            var dataset = m_Builder
                .UsingFrame(expectedFrameNumber)
                .WithTriggerTime(1.234)
                .ToDataSet();

            // Assert
            dataset.Contains(LocalTag.Tag.PerFrameFunctionalGroupsSequence).Should().BeTrue();
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence].Count.Should().BeGreaterOrEqualTo(expectedFrameNumber);
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][expectedFrameNumber - 1].Should().NotBeNull();
        }

        [Test]
        public void WithFrameAcquisitionDateTime_ShouldCreateTagFrameAcquisitionDatetimeWithMatchingValues()
        {
            // Arrange
            var acquisitionDateTime = new DateTime(2015, 10, 30, 13, 45, 59);
            var acquisitionDate = acquisitionDateTime.ToString("yyyyMMdd");             //"20151030";
            var acquisitionTime = acquisitionDateTime.ToString("HHmmss.ffffff");        //"101505.000000";
            var frameNumber = 19;

            // Act
            var dataset = m_Builder
                .UsingFrame(frameNumber)
                .WithFrameAcquisitionDateTime(acquisitionDate, acquisitionTime)
                .ToDataSet();

            // Assert
            dataset.Contains(LocalTag.Tag.PerFrameFunctionalGroupsSequence).Should().BeTrue();
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence].Count.Should().BeGreaterOrEqualTo(frameNumber);
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber - 1].Should().NotBeNull();
            var perFrameSequ = dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber - 1].As<IDataSet>();

            perFrameSequ.Contains(LocalTag.Tag.FrameContentSequence).Should().BeTrue();
            perFrameSequ[LocalTag.Tag.FrameContentSequence].Count.Should().Be(1);
            perFrameSequ[LocalTag.Tag.FrameContentSequence][0].Should().NotBeNull();
            var frameContSequ = perFrameSequ[LocalTag.Tag.FrameContentSequence][0].As<IDataSet>();

            frameContSequ.Contains(LocalTag.Tag.FrameAcquisitionDatetime).Should().BeTrue();
            frameContSequ[LocalTag.Tag.FrameAcquisitionDatetime].Count.Should().Be(1);
            frameContSequ[LocalTag.Tag.FrameAcquisitionDatetime][0].Should().BeEquivalentTo(string.Concat(acquisitionDate, acquisitionTime));
        }

        [Test]
        public void WithFrameType_ShouldCreateTagFrameTypeWithMatchingValue()
        {
            // Arrange
            var value = "123456789";
            var frameNumber = 23;

            // Act
            var dataset = m_Builder
                .UsingFrame(frameNumber)
                .WithFrameType(value)
                .ToDataSet();

            // Assert
            dataset.Contains(LocalTag.Tag.PerFrameFunctionalGroupsSequence).Should().BeTrue();
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence].Count.Should().BeGreaterOrEqualTo(frameNumber);
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber - 1].Should().NotBeNull();
            var perFrameSequ = dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber - 1].As<IDataSet>();

            perFrameSequ.Contains(LocalTag.Tag.MrImageFrameTypeSequence).Should().BeTrue();
            perFrameSequ[LocalTag.Tag.MrImageFrameTypeSequence].Count.Should().Be(1);
            perFrameSequ[LocalTag.Tag.MrImageFrameTypeSequence][0].Should().NotBeNull();
            var frameTypeSequ = perFrameSequ[LocalTag.Tag.MrImageFrameTypeSequence][0].As<IDataSet>();

            frameTypeSequ.Contains(LocalTag.Tag.FrameType).Should().BeTrue();
            frameTypeSequ[LocalTag.Tag.FrameType].Count.Should().Be(1);
            frameTypeSequ[LocalTag.Tag.FrameType][0].CastTo<string>().Should().Be(value);
        }

        [Test]
        public void WithFrameLevelVolumetricProperties_ShouldCreateTagVolumetricPropertiesWithMatchingValue()
        {
            // Arrange
            var value = "AnyFrameLevelVolProp";
            var frameNumber = 23;

            // Act
            var dataset = m_Builder
                .UsingFrame(frameNumber)
                .WithFrameLevelVolumetricProperties(value)
                .ToDataSet();

            // Assert
            dataset.Contains(LocalTag.Tag.PerFrameFunctionalGroupsSequence).Should().BeTrue();
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence].Count.Should().BeGreaterOrEqualTo(frameNumber);
            dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber - 1].Should().NotBeNull();
            var perFrameSequ = dataset[LocalTag.Tag.PerFrameFunctionalGroupsSequence][frameNumber - 1].As<IDataSet>();

            perFrameSequ.Contains(LocalTag.Tag.MrImageFrameTypeSequence).Should().BeTrue();
            perFrameSequ[LocalTag.Tag.MrImageFrameTypeSequence].Count.Should().Be(1);
            perFrameSequ[LocalTag.Tag.MrImageFrameTypeSequence][0].Should().NotBeNull();
            var frameTypeSequ = perFrameSequ[LocalTag.Tag.MrImageFrameTypeSequence][0].As<IDataSet>();

            frameTypeSequ.Contains(LocalTag.Tag.VolumetricProperties).Should().BeTrue();
            frameTypeSequ[LocalTag.Tag.VolumetricProperties].Count.Should().Be(1);
            frameTypeSequ[LocalTag.Tag.VolumetricProperties][0].CastTo<string>().Should().Be(value);
        }

    }
}

//----------------------------------------------------------------------------------
// File: "EnhancedMrImageBuilder.cs"
// Author: Steffen Hanke
// Date: 2015-2019
//----------------------------------------------------------------------------------
using System;
using System.Globalization;
using Solid.Infrastructure.RuntimeTypeExtensions;
using Solid.Infrastructure.Math;

using MrPrivateDicomTags = Solid.Dicom.DicomTags.MrPrivateDicomTags;
using LocalTag = Solid.Dicom.DicomTags;
using SopClassUids = Solid.Dicom.DicomTags.MrSopClassUids;

namespace Solid.TestInfrastructure.Dicom
{
    public class EnhancedMrImageBuilder : MrImageBuilder
    {
        private int m_UsingFrameNumber = 1;
        private bool m_UsingFrameSpecified = false;

        public int UsingFrameNumber => m_UsingFrameNumber;

        public EnhancedMrImageBuilder()
        {
            RemoveTag(LocalTag.Tag.SopClassUid);
            SetupTag(LocalTag.Tag.SopClassUid, SopClassUids.ENHANCED_MR_IMAGE/*SopClass.ToValue(SopClass.Uid.EnhancedMagneticResonanceImage)*/, 0);
        }

        public override IImageBuilder WithTriggerTime(double triggerTime)
        {
            var cardiacTriggerSequence = GetOrCreateSequence(LocalTag.Tag.PerFrameFunctionalGroupsSequence, m_UsingFrameNumber - 1)
                .GetOrCreateSequence(LocalTag.Tag.CardiacSynchronizationSequence, 0);
            cardiacTriggerSequence.SetupTag(LocalTag.Tag.NominalCardiacTriggerDelayTime, /*VR: FD*/triggerTime, 0);
            return this;
        }

        public override IImageBuilder WithIsPhaseEncodingDirectionPositive(bool isPhaseEncodingDirectionPositive)
        {
            var siemensMrSdiSequence = GetOrCreateSequence(LocalTag.Tag.PerFrameFunctionalGroupsSequence, m_UsingFrameNumber - 1)
                .GetOrCreateSequence(MrPrivateDicomTags.SiemensMrSdiSequence, 0);
            siemensMrSdiSequence.SetupTag(MrPrivateDicomTags.PhaseEncodingDirectionPositive, isPhaseEncodingDirectionPositive ? "1" : "0", 0);
            return this;
        }

        public override IImageBuilder WithTimeAfterStart(double timeAfterStart)
        {
            var siemensMrSdiSequence = GetOrCreateSequence(LocalTag.Tag.PerFrameFunctionalGroupsSequence, m_UsingFrameNumber - 1)
                .GetOrCreateSequence(MrPrivateDicomTags.SiemensMrSdiSequence, 0);
            siemensMrSdiSequence.SetupTag(MrPrivateDicomTags.TimeAfterStart, /*VR: DS*/timeAfterStart.ToString(CultureInfo.InvariantCulture), 0);
            return this;
        }

        public override IImageBuilder WithScanningSequence(string scanningSequence)
        {
            var siemensMrSdsSequence = GetOrCreateSequence(LocalTag.Tag.SharedFunctionalGroupsSequence, 0)
                .GetOrCreateSequence(MrPrivateDicomTags.SiemensMrSdsSequence, 0);
            siemensMrSdsSequence.SetupTag(MrPrivateDicomTags.ScanningSequence, scanningSequence, 0);
            return this;
        }

        public override IImageBuilder WithSequenceVariant(string sequenceVariant)
        {
            var siemensMrSdsSequence = GetOrCreateSequence(LocalTag.Tag.SharedFunctionalGroupsSequence, 0)
                .GetOrCreateSequence(MrPrivateDicomTags.SiemensMrSdsSequence, 0);
            siemensMrSdsSequence.SetupTag(MrPrivateDicomTags.SequenceVariant, sequenceVariant, 0);
            return this;
        }

        public override IImageBuilder WithGradientCoilName(string gradCoilName)
        {
            var siemensMrSdsSequence = GetOrCreateSequence(LocalTag.Tag.SharedFunctionalGroupsSequence, 0)
                .GetOrCreateSequence(MrPrivateDicomTags.SiemensMrSdsSequence, 0);
            siemensMrSdsSequence.SetupTag(MrPrivateDicomTags.GradientCoilName, gradCoilName, 0);
            return this;
        }

        public override IImageBuilder WithImaRelTablePos(Vector3D tabelPos)
        {
            var imageFrameTypeSequence = GetOrCreateSequence(LocalTag.Tag.PerFrameFunctionalGroupsSequence, m_UsingFrameNumber - 1)
                .GetOrCreateSequence(LocalTag.Tag.MrImageFrameTypeSequence, 0);
            imageFrameTypeSequence.SetupTag(MrPrivateDicomTags.ImaRelTablePosition, /*VR: IS*/Convert.ToInt32(tabelPos.X), 0);
            imageFrameTypeSequence.SetupTag(MrPrivateDicomTags.ImaRelTablePosition, /*VR: IS*/Convert.ToInt32(tabelPos.Y), 1);
            imageFrameTypeSequence.SetupTag(MrPrivateDicomTags.ImaRelTablePosition, /*VR: IS*/Convert.ToInt32(tabelPos.Z), 2);
            return this;
        }

        public override IImageBuilder WithAcquisitionDateTime(string date, string time)
        {
            SetupTag(LocalTag.Tag.AcquisitionDatetime, /*VR: DT*/string.Concat(date, time), 0);
            return this;
        }

        public override IImageBuilder UsingFrame(int frameNumber)
        {
            if (frameNumber < 1)  throw new ArgumentOutOfRangeException(nameof(frameNumber));
            m_UsingFrameNumber = frameNumber;
            m_UsingFrameSpecified = true;
            return this;
        }

        public override IImageBuilder WithImagePlane(Vector3D position, Vector3D row, Vector3D column)
        {
            return WithImagePlane(
                new double[] { position.X, position.Y, position.Z },
                new double[] { row.X, row.Y, row.Z, column.X, column.Y, column.Z });
        }

        public override IImageBuilder WithImagePlane(double[] positions, double[] orientations)
        {
            if (positions.Length != 3) throw new ArgumentOutOfRangeException(nameof(positions));
            if (orientations.Length != 6) throw new ArgumentOutOfRangeException(nameof(orientations));

            var planePositionSequence = GetOrCreateSequence(LocalTag.Tag.PerFrameFunctionalGroupsSequence, m_UsingFrameNumber - 1).GetOrCreateSequence(LocalTag.Tag.PlanePositionSequence, 0);
            foreach (double dbl in positions)
            {
                planePositionSequence.SetupMvTag(LocalTag.Tag.ImagePositionPatient, /*VR: DS*/dbl.ToString(CultureInfo.InvariantCulture), 3);
            }

            var planeOrientationSequence = GetOrCreateSequence(LocalTag.Tag.PerFrameFunctionalGroupsSequence, m_UsingFrameNumber - 1).GetOrCreateSequence(LocalTag.Tag.PlaneOrientationSequence, 0);
            foreach (double dbl in orientations)
            {
                planeOrientationSequence.SetupMvTag(LocalTag.Tag.ImageOrientationPatient, /*VR: DS*/dbl.ToString(CultureInfo.InvariantCulture), 6);
            }
            return this;
        }

        public override IImageBuilder WithFrameAcquisitionDateTime(string date, string time)
        {
            var frameContentSequence = GetOrCreateSequence(LocalTag.Tag.PerFrameFunctionalGroupsSequence, m_UsingFrameNumber - 1).GetOrCreateSequence(LocalTag.Tag.FrameContentSequence, 0);
            frameContentSequence.SetupTag(LocalTag.Tag.FrameAcquisitionDatetime, /*VR: DT*/string.Concat(date, time), 0);
            return this;
        }

        public override IImageBuilder WithImageMatrix(int rowCount, int columCount, double pixelSpacingX, double pixelSpacingY)
        {
            SetupTag(LocalTag.Tag.Rows, /*VR: US*/rowCount.CastTo<ushort>(), 0);
            SetupTag(LocalTag.Tag.Columns, /*VR: US*/columCount.CastTo<ushort>(), 0);

            IDataSetBuilder pixelMeasuresSequence = null;
            if (m_UsingFrameSpecified)
            {
                pixelMeasuresSequence = GetOrCreateSequence(LocalTag.Tag.PerFrameFunctionalGroupsSequence, m_UsingFrameNumber - 1).GetOrCreateSequence(LocalTag.Tag.PixelMeasuresSequence, 0);
            }
            else
            {
                pixelMeasuresSequence = GetOrCreateSequence(LocalTag.Tag.SharedFunctionalGroupsSequence, 0).GetOrCreateSequence(LocalTag.Tag.PixelMeasuresSequence, 0);
            }

            pixelMeasuresSequence.SetupTag(LocalTag.Tag.PixelSpacing, /*VR: DS*/pixelSpacingX.ToString(CultureInfo.InvariantCulture), 0);
            pixelMeasuresSequence.SetupTag(LocalTag.Tag.PixelSpacing, /*VR: DS*/pixelSpacingY.ToString(CultureInfo.InvariantCulture), 1);
            return this;
        }

        public override IImageBuilder WithInPlanePhaseEncodingDirection(string code)
        {
            var mrFovGeometrySequence = GetOrCreateSequence(LocalTag.Tag.SharedFunctionalGroupsSequence, 0).GetOrCreateSequence(LocalTag.Tag.MrFovGeometrySequence, 0);
            mrFovGeometrySequence.SetupTag(LocalTag.Tag.InPlanePhaseEncodingDirection, code, 0);
            return this;
        }

        public override IImageBuilder WithPhaseEncodingDirectionPositive(bool isPositive)
        {
            var siemensMrSdiSequence = GetOrCreateSequence(LocalTag.Tag.PerFrameFunctionalGroupsSequence, m_UsingFrameNumber - 1)
                .GetOrCreateSequence(MrPrivateDicomTags.SiemensMrSdiSequence, 0);
            siemensMrSdiSequence.SetupTag(MrPrivateDicomTags.PhaseEncodingDirectionPositive, isPositive ? "1" : "0", 0);
            return this;
        }

        public override IImageBuilder WithDistortionCorrectionType(string disCorTypeName)
        {
            var frameTypeSequence = GetOrCreateSequence(LocalTag.Tag.PerFrameFunctionalGroupsSequence, m_UsingFrameNumber - 1).GetOrCreateSequence(LocalTag.Tag.MrImageFrameTypeSequence, 0);
            frameTypeSequence.SetupTag(MrPrivateDicomTags.DistortionCorrectionType, disCorTypeName, 0);
            return this;
        }

        public override IImageBuilder WithFrameType(string type)
        {
            var frameTypeSequence = GetOrCreateSequence(LocalTag.Tag.PerFrameFunctionalGroupsSequence, m_UsingFrameNumber - 1).GetOrCreateSequence(LocalTag.Tag.MrImageFrameTypeSequence, 0);
            frameTypeSequence.SetupMvTag(LocalTag.Tag.FrameType, type, 4);
            return this;
        }

        public override IImageBuilder WithFrameLevelVolumetricProperties(string volPropTypeName)
        {
            var frameTypeSequence = GetOrCreateSequence(LocalTag.Tag.PerFrameFunctionalGroupsSequence, m_UsingFrameNumber - 1).GetOrCreateSequence(LocalTag.Tag.MrImageFrameTypeSequence, 0);
            frameTypeSequence.SetupTag(LocalTag.Tag.VolumetricProperties, volPropTypeName, 0);
            return this;
        }

        public override IImageBuilder WithThickness(double imageThickness)
        {
            return WithSliceThickness(imageThickness.ToString(CultureInfo.InvariantCulture));
        }

        public override IImageBuilder WithSliceThickness(string dsValue)
        {
            var pixelMeasuresSequence = GetOrCreateSequence(LocalTag.Tag.PerFrameFunctionalGroupsSequence, m_UsingFrameNumber - 1).GetOrCreateSequence(LocalTag.Tag.PixelMeasuresSequence, 0);
            pixelMeasuresSequence.SetupTag(LocalTag.Tag.SliceThickness, dsValue, 0);
            return this;
        }

        //public override IImageBuilder WithPatientPosition(string patPos)
        //{
        //    SetupTag(LocalTag.Tag.PatientPosition, patPos, 0);
        //    return this;
        //}
    }
}

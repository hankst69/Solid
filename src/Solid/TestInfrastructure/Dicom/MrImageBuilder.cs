//----------------------------------------------------------------------------------
// File: "MrImageBuilder.cs"
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
using OrientationType = Solid.Dicom.ImageInfo.Types.OrientationType;

namespace Solid.TestInfrastructure.Dicom
{
    public class MrImageBuilder : DataSetBuilder, IImageBuilder
    {
        public MrImageBuilder()
        {
            SetupTag(LocalTag.Tag.Modality, "MR", 0);
            SetupTag(LocalTag.Tag.SopClassUid, SopClassUids.MR_IMAGE/*SopClass.ToValue(SopClass.Uid.MagneticResonanceImage)*/, 0);
        }

        public new IImageBuilder DisablePlausibilityChecks()
        {
            base.DisablePlausibilityChecks();
            return this;
        }

        public IImageBuilder WithoutTag(long tag)
        {
            RemoveTag(tag);
            return this;
        }

        public IImageBuilder WithEmptyTag(long tag)
        {
            SetupEmptyTag(tag);
            return this;
        }

        public IImageBuilder WithFrameOfReferenceUid(string forUid)
        {
            SetupTag(LocalTag.Tag.FrameOfReferenceUid, forUid, 0);
            return this;
        }

        public IImageBuilder WithImageType(string type)
        {
            SetupMvTag(LocalTag.Tag.ImageType, type, 4);
            return this;
        }

        public IImageBuilder WithStudyInstanceUid(string studyUid)
        {
            SetupTag(LocalTag.Tag.StudyInstanceUid, studyUid, 0);
            return this;
        }
        public IImageBuilder WithSeriesInstanceUid(string seriesUid)
        {
            SetupTag(LocalTag.Tag.SeriesInstanceUid, seriesUid, 0);
            return this;
        }
        public IImageBuilder WithSopInstanceUid(string instanceUid)
        {
            SetupTag(LocalTag.Tag.SopInstanceUid, instanceUid, 0);
            return this;
        }
        
        public IImageBuilder WithStudyDescription(string studyDescription)
        {
            SetupTag(LocalTag.Tag.StudyDescription, studyDescription, 0);
            return this;
        }
        public IImageBuilder WithSeriesDescription(string seriesDescription)
        {
            SetupTag(LocalTag.Tag.SeriesDescription, seriesDescription, 0);
            return this;
        }

        public IImageBuilder WithSeriesNumber(int seriesNumber)
        {
            SetupTag(LocalTag.Tag.SeriesNumber, seriesNumber.ToString(), 0);
            return this;
        }
        public IImageBuilder WithInstanceNumber(int instanceNumber)
        {
            SetupTag(LocalTag.Tag.InstanceNumber, instanceNumber.ToString(), 0);
            return this;
        }

        public virtual IImageBuilder WithTriggerTime(double triggerTime)
        {
            SetupTag(LocalTag.Tag.TriggerTime, triggerTime.ToString(CultureInfo.InvariantCulture), 0);
            return this;
        }

        public virtual IImageBuilder WithTimeAfterStart(double timeAfterStart)
        {
            SetupTag(MrPrivateDicomTags.TimeAfterStart, timeAfterStart.ToString(CultureInfo.InvariantCulture), 0);
            return this;
        }

        public virtual IImageBuilder WithIsPhaseEncodingDirectionPositive(bool isPhaseEncodingDirectionPositive)
        {
            SetupTag(MrPrivateDicomTags.PhaseEncodingDirectionPositive, isPhaseEncodingDirectionPositive ? "1" : "0", 0);
            return this;
        }

        public virtual IImageBuilder WithScanningSequence(string scanningSequence)
        {
            SetupTag(LocalTag.Tag.ScanningSequence, scanningSequence, 0);
            return this;
        }

        public virtual IImageBuilder WithSequenceVariant(string sequenceVariant)
        {
            SetupTag(LocalTag.Tag.SequenceVariant, sequenceVariant, 0);
            return this;
        }

        public virtual IImageBuilder WithGradientCoilName(string gradCoilName)
        {
            SetupTag(MrPrivateDicomTags.GradientCoilName, gradCoilName, 0);
            return this;
        }

        public virtual IImageBuilder WithImaRelTablePos(Vector3D tabelPos)
        {
            SetupTag(MrPrivateDicomTags.RelTablePosition, Convert.ToInt32(tabelPos.X), 0);
            SetupTag(MrPrivateDicomTags.RelTablePosition, Convert.ToInt32(tabelPos.Y), 1);
            SetupTag(MrPrivateDicomTags.RelTablePosition, Convert.ToInt32(tabelPos.Z), 2);
            return this;
        }

        public virtual IImageBuilder WithPresentationStateSopInstanceUid(string sopInstanceUid)
        {
            var referencedGrayscalePresentationStateSequence = this.GetOrCreateSequence(LocalTag.Tag.ReferencedGrayscalePresentationStateSequence, 0);
            var referencedSeriesSequence = referencedGrayscalePresentationStateSequence.GetOrCreateSequence(LocalTag.Tag.ReferencedSeriesSequence, 0);
            var referencedSopSequence = referencedSeriesSequence.GetOrCreateSequence(LocalTag.Tag.ReferencedSopSequence, 0);
            referencedSopSequence.SetupTag(LocalTag.Tag.ReferencedSopInstanceUid, sopInstanceUid, 0);
            return this;
        }

        public virtual IImageBuilder WithAcquisitionDateTime(string date, string time)
        {
            SetupTag(LocalTag.Tag.AcquisitionDate, date, 0);
            SetupTag(LocalTag.Tag.AcquisitionTime, time, 0);
            return this;
        }
        
        public IImageBuilder WithAcquisitionDateTime(DateTime dateTime)
        {
            var acquisitionDate = dateTime.ToString("yyyyMMdd");
            var acquisitionTime = dateTime.ToString("HHmmss.ffffff");

            return WithAcquisitionDateTime(acquisitionDate, acquisitionTime);
        }

        public virtual IImageBuilder WithImagePlane(double[] position, double[] orientation)
        {
            if (position.Length != 3) throw new ArgumentOutOfRangeException(nameof(position));
            if (orientation.Length != 6) throw new ArgumentOutOfRangeException(nameof(orientation));

            foreach (double dbl in position)
            {
                SetupMvTag(LocalTag.Tag.ImagePositionPatient, /*VR: DS*/dbl.ToString(CultureInfo.InvariantCulture), 3);
            }
            foreach (double dbl in orientation)
            {
                SetupMvTag(LocalTag.Tag.ImageOrientationPatient, /*VR: DS*/dbl.ToString(CultureInfo.InvariantCulture), 6);
            }
            return this;
        }

        public virtual IImageBuilder WithImagePlane(Vector3D position, Vector3D row, Vector3D column)
        {
            return WithImagePlane(
                new double[] {position.X, position.Y, position.Z},
                new double[] {row.X, row.Y, row.Z, column.X, column.Y, column.Z});
        }

        public virtual IImageBuilder WithImageMatrix(int rowCount, int columCount, double pixelSpacingX, double pixelSpacingY)
        {
            SetupTag(LocalTag.Tag.Rows, /*VR: US*/rowCount.CastTo<ushort>(), 0);
            SetupTag(LocalTag.Tag.Columns, /*VR: US*/columCount.CastTo<ushort>(), 0);
            SetupTag(LocalTag.Tag.PixelSpacing, /*VR: DS*/pixelSpacingX.ToString(CultureInfo.InvariantCulture), 0);
            SetupTag(LocalTag.Tag.PixelSpacing, /*VR: DS*/pixelSpacingY.ToString(CultureInfo.InvariantCulture), 1);
            return this;
        }

        public virtual IImageBuilder WithInPlanePhaseEncodingDirection(string code)
        {
            SetupTag(LocalTag.Tag.InPlanePhaseEncodingDirection, code, 0);
            return this;
        }

        public virtual IImageBuilder WithPhaseEncodingDirectionPositive(bool isPositive)
        {
            SetupTag(MrPrivateDicomTags.PhaseEncodingDirectionPositive, isPositive ? "1" : "0", 0);
            return this;
        }

        public virtual IImageBuilder WithDistortionCorrectionType(string disCorTypeName)
        {
            SetupTag(MrPrivateDicomTags.DistortionCorrectionType, disCorTypeName, 0);
            return this;
        }

        //public IImageBuilder WithImageMode(ImageMode mode)
        //{
        //    if (mode == ImageMode.Unknown)
        //        throw new NotSupportedException("ImageMode unknown not supported");

        //    var disCorTypeName = ImageModeToDistortionCorrectionTypeString(mode);
        //    return WithDistortionCorrectionType(disCorTypeName);
        //}

        //private static string ImageModeToDistortionCorrectionTypeString(ImageMode mode)
        //{
        //    switch (mode)
        //    {
        //        case ImageMode.Distortion:
        //            return "DIS2D";
        //        case ImageMode.DistortionBent:
        //            return "DISTORTED";
        //        case ImageMode.NonDistortion:
        //            return "ND";
        //    }
        //    return "ND";
        //}

        public IImageBuilder WithMainOrientation(OrientationType orientation, double sliceShift = 0)
        {
            OrientationTypeToImagePlaneVectors(orientation, out Vector3D rowVector, out Vector3D columnVector);

            var normal = columnVector.GetOuterProduct(rowVector).GetNormalized();
            if (orientation != OrientationType.Sagittal)
            {
                sliceShift *= -1;
            }
            return WithImagePlane(sliceShift*normal, rowVector, columnVector);
        }

        public IImageBuilder WithVolumetricProperties(string volPropTypeName)
        {
            SetupTag(LocalTag.Tag.VolumetricProperties, volPropTypeName, 0);
            return this;
        }

        public virtual IImageBuilder WithFrameLevelVolumetricProperties(string volPropTypeName)
        {
            throw new System.NotImplementedException();
        }

        public virtual IImageBuilder UsingFrame(int frameNumber)
        {
            throw new System.NotImplementedException();
        }

        public virtual IImageBuilder WithFrameAcquisitionDateTime(string date, string time)
        {
            throw new System.NotImplementedException();
        }

        public virtual IImageBuilder WithFrameType(string type)
        {
            throw new System.NotImplementedException();
        }

        public virtual IImageBuilder WithThickness(double imageThickness)
        {
            return WithSliceThickness(imageThickness.ToString(CultureInfo.InvariantCulture));
        }

        public virtual IImageBuilder WithSliceThickness(string dsValue)
        {
            SetupTag(LocalTag.Tag.SliceThickness, dsValue, 0);
            return this;
        }

        public virtual IImageBuilder WithPatientPosition(string patPos)
        {
            SetupTag(LocalTag.Tag.PatientPosition, patPos, 0);
            return this;
        }


        private static void OrientationTypeToImagePlaneVectors(OrientationType orientation, out Vector3D row, out Vector3D column)
        {
            switch (orientation)
            {
                case OrientationType.Coronal:
                    row = new Vector3D(1, 0, 0);
                    column = new Vector3D(0, 0, -1);
                    break;
                case OrientationType.Transversal:
                    row = new Vector3D(1, 0, 0);
                    column = new Vector3D(0, 1, 0);
                    break;
                case OrientationType.Sagittal:
                    row = new Vector3D(0, 1, 0);
                    column = new Vector3D(0, 0, -1);
                    break;
                default:
                    row = new Vector3D(1, 1, 1);
                    column = new Vector3D(1, 1, 1);
                    break;
            }
        }

        public IImageBuilder WithSeriesDate(DateTime date)
        {
            SetupTag(LocalTag.Tag.SeriesDate, date.ToString("yyyyMMdd"),0);
            return this;
        }
    }
}

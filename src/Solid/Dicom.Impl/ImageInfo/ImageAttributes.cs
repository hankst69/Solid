//----------------------------------------------------------------------------------
// File: "ImageAttributes.cs"
// Author: Steffen Hanke
// Date: 2019-2022
//----------------------------------------------------------------------------------

using System;
using System.Linq;
using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.Math;

namespace Solid.Dicom.ImageInfo.Impl
{
    /// <summary>
    /// API:NO
    /// ImageAttributes
    /// </summary>
    public class ImageAttributes : IImageAttributes
    {
        private readonly Lazy<IImageClassInfo> m_ImageClassInfo;
        private readonly Lazy<IImageDistortionInfo> m_ImageDistortionInfo;
        private readonly Lazy<IImageOrderInfo> m_ImageOrderInfo;
        private readonly Lazy<IImagePixelInfo> m_ImagePixelInfo;
        private readonly Lazy<IImagePlaneInfo> m_ImagePlaneInfo;
        private readonly Lazy<IImageScanInfo> m_ImageScanInfo;

        private readonly Lazy<bool> m_IsEnhancedMrImage;
        private readonly Lazy<string> m_SopClassUid;
        private readonly Lazy<string> m_SopInstanceUid;
        private readonly Lazy<string> m_SeriesInstanceUid;
        private readonly Lazy<string> m_FrameOfReferenceUid;
        private readonly Lazy<string> m_PresentationStateSopInstanceUid;
        private readonly Lazy<string> m_Modality;
        private readonly Lazy<string[]> m_ImageTypes;
        private readonly Lazy<DateTime> m_AcquisitionDateTime;
        private readonly Lazy<DateTime> m_SeriesDateTime;
        private readonly Lazy<string> m_StudyDescription;
        private readonly Lazy<string> m_SeriesDescription;
        private readonly Lazy<int> m_SeriesNumber;
        private readonly Lazy<int> m_InstanceNumber;
        private readonly Lazy<string> m_ProtocolName;
        private readonly Lazy<string> m_SequenceName;
        private readonly Lazy<string> m_ScanningSequence;
        private readonly Lazy<string> m_SequenceVariant;
        private readonly Lazy<string> m_InPlanePhaseEncodingDirection;
        private readonly Lazy<string> m_PhaseEncodingDirectionPositive;
        private readonly Lazy<double> m_SliceThickness;
        private readonly Lazy<double> m_TimeAfterStart;
        private readonly Lazy<double> m_TriggerTime;
        private readonly Lazy<Vector3D> m_ImaRelTablePosition;
        private readonly Lazy<string> m_PatientPosition;
        private readonly Lazy<string> m_BodyPartExamined;
        private readonly Lazy<string> m_MrAcquisitionType;
        private readonly Lazy<int> m_AcquisitionNumber;
        private readonly Lazy<string> m_DistortionCorrectionType;
        private readonly Lazy<string> m_VolumetricProperties;
        private readonly Lazy<string> m_FrameLevelVolumetricProperties;
        private readonly Lazy<string> m_GradientCoilName;
        private readonly Lazy<Vector3D> m_ImagePosition;
        private readonly Lazy<Vector3D> m_OrientationRow;
        private readonly Lazy<Vector3D> m_OrientationCol;
        private readonly Lazy<int> m_NumberOfFrames;
        private readonly Lazy<ushort> m_MatrixRows;
        private readonly Lazy<ushort> m_MatrixCols;
        private readonly Lazy<double> m_PixelSpacingRow;
        private readonly Lazy<double> m_PixelSpacingCol;
        private readonly Lazy<string> m_TransferSyntaxUid;
        private readonly Lazy<string> m_LossyImageCompression;
        private readonly Lazy<string> m_PhotometricInterpretation;
        private readonly Lazy<ushort> m_SamplesPerPixel;
        private readonly Lazy<ushort> m_PlanarConfiguration;
        private readonly Lazy<ushort> m_BitsAllocated;
        private readonly Lazy<ushort> m_BitsStored;
        private readonly Lazy<ushort> m_HighBit;
        private readonly Lazy<ushort> m_PixelRepresentation;
        private readonly Lazy<double> m_WindowCenter;
        private readonly Lazy<double> m_WindowWidth;
        private readonly Lazy<double> m_RescaleIntercept;
        private readonly Lazy<double> m_RescaleSlope;

        public ImageAttributes(IMrDicomAccess mrDicomAccess, IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(mrDicomAccess).IsNotNull();
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();

            // ----- store address information -----
            DataSet = dataSet;
            FrameNumber = dataSet.FrameNumber;

            m_ImageClassInfo = new Lazy<IImageClassInfo>(() => new ImageClassInfo(this));
            m_ImageDistortionInfo = new Lazy<IImageDistortionInfo>(() => new ImageDistortionInfo(this));
            m_ImageOrderInfo = new Lazy<IImageOrderInfo>(() => new ImageOrderInfo(this));
            m_ImagePixelInfo = new Lazy<IImagePixelInfo>(() => new ImagePixelInfo(this));
            m_ImagePlaneInfo = new Lazy<IImagePlaneInfo>(() => new ImagePlaneInfo(this));
            m_ImageScanInfo = new Lazy<IImageScanInfo>(() => new ImageScanInfo(this));

            //m_LazySliceShift = new Lazy<double>(() => Slice.GetSliceShift(frame.FrameInfo.ImagePosition, frame.FrameInfo.Slice.GetImageNormal(), MathsGlobal.DEFAULT_TOLERANCE));
            m_IsEnhancedMrImage = new Lazy<bool>(() => mrDicomAccess.IsEnhancedMrImage(dataSet));
            m_SopClassUid = new Lazy<string>(() => mrDicomAccess.GetSopClassUid(dataSet));
            m_SopInstanceUid = new Lazy<string>(() => mrDicomAccess.GetSopInstanceUid(dataSet));
            m_SeriesInstanceUid = new Lazy<string>(() => mrDicomAccess.GetSeriesInstanceUid(dataSet));
            m_FrameOfReferenceUid = new Lazy<string>(() => mrDicomAccess.GetFrameOfReferenceUid(dataSet));
            m_PresentationStateSopInstanceUid = new Lazy<string>(() => mrDicomAccess.GetPresentationStateSopInstanceUid(dataSet));
            m_Modality = new Lazy<string>(() => mrDicomAccess.GetModality(dataSet));
            m_ImageTypes = new Lazy<string[]>(() => mrDicomAccess.GetImageTypes(dataSet).ToArray());
            m_AcquisitionDateTime = new Lazy<DateTime>(() => mrDicomAccess.GetAcquisitionDateTime(dataSet));
            m_SeriesDateTime = new Lazy<DateTime>(() => mrDicomAccess.GetSeriesDateTime(dataSet));
            m_StudyDescription = new Lazy<string>(() => mrDicomAccess.GetStudyDescription(dataSet));
            m_SeriesDescription = new Lazy<string>(() => mrDicomAccess.GetSeriesDescription(dataSet));
            m_SeriesNumber = new Lazy<int>(() => mrDicomAccess.GetSeriesNumber(dataSet));
            m_InstanceNumber = new Lazy<int>(() => mrDicomAccess.GetInstanceNumber(dataSet));
            m_ProtocolName = new Lazy<string>(() => mrDicomAccess.GetProtocolName(dataSet));
            m_SequenceName = new Lazy<string>(() => mrDicomAccess.GetSequenceName(dataSet));
            m_ScanningSequence = new Lazy<string>(() => mrDicomAccess.GetScanningSequence(dataSet));
            m_SequenceVariant = new Lazy<string>(() => mrDicomAccess.GetSequenceVariant(dataSet));
            m_InPlanePhaseEncodingDirection = new Lazy<string>(() => mrDicomAccess.GetInPlanePhaseEncodingDirection(dataSet));
            m_PhaseEncodingDirectionPositive = new Lazy<string>(() => mrDicomAccess.GetPhaseEncodingDirectionPositive(dataSet));
            m_SliceThickness = new Lazy<double>(() => mrDicomAccess.GetSliceThickness(dataSet));
            m_TimeAfterStart = new Lazy<double>(() => mrDicomAccess.GetTimeAfterStart(dataSet));
            m_TriggerTime = new Lazy<double>(() => mrDicomAccess.GetTriggerTime(dataSet));
            m_ImaRelTablePosition = new Lazy<Vector3D>(() => mrDicomAccess.GetImaRelTablePosition(dataSet));
            m_PatientPosition = new Lazy<string>(() => mrDicomAccess.GetPatientPosition(dataSet));
            m_BodyPartExamined = new Lazy<string>(() => mrDicomAccess.GetBodyPartExamined(dataSet));
            m_MrAcquisitionType = new Lazy<string>(() => mrDicomAccess.GetMrAcquisitionType(dataSet));
            m_AcquisitionNumber = new Lazy<int>(() => mrDicomAccess.GetAcquisitionNumber(dataSet));
            m_DistortionCorrectionType = new Lazy<string>(() => mrDicomAccess.GetDistortionCorrectionType(dataSet));
            m_VolumetricProperties = new Lazy<string>(() => mrDicomAccess.GetVolumetricProperties(dataSet));
            m_FrameLevelVolumetricProperties = new Lazy<string>(() => mrDicomAccess.GetFrameLevelVolumetricProperties(dataSet));
            m_GradientCoilName = new Lazy<string>(() => mrDicomAccess.GetGradientCoilName(dataSet));
            m_ImagePosition = new Lazy<Vector3D>(() => mrDicomAccess.GetImagePosition(dataSet));
            m_OrientationRow = new Lazy<Vector3D>(() => mrDicomAccess.GetOrientationRow(dataSet));
            m_OrientationCol = new Lazy<Vector3D>(() => mrDicomAccess.GetOrientationCol(dataSet));
            m_NumberOfFrames = new Lazy<int>(() => mrDicomAccess.GetNumberOfFrames(dataSet));
            m_MatrixRows = new Lazy<ushort>(() => mrDicomAccess.GetMatrixRows(dataSet));
            m_MatrixCols = new Lazy<ushort>(() => mrDicomAccess.GetMatrixCols(dataSet));
            m_PixelSpacingRow = new Lazy<double>(() => mrDicomAccess.GetPixelSpacingRow(dataSet));
            m_PixelSpacingCol = new Lazy<double>(() => mrDicomAccess.GetPixelSpacingCol(dataSet));
            m_TransferSyntaxUid = new Lazy<string>(() => mrDicomAccess.GetTransferSyntaxUid(dataSet));
            m_LossyImageCompression = new Lazy<string>(() => mrDicomAccess.GetLossyImageCompression(dataSet));
            m_PhotometricInterpretation = new Lazy<string>(() => mrDicomAccess.GetPhotometricInterpretation(dataSet));
            m_SamplesPerPixel = new Lazy<ushort>(() => mrDicomAccess.GetSamplesPerPixel(dataSet));
            m_PlanarConfiguration = new Lazy<ushort>(() => mrDicomAccess.GetPlanarConfiguration(dataSet));
            m_BitsAllocated = new Lazy<ushort>(() => mrDicomAccess.GetBitsAllocated(dataSet));
            m_BitsStored = new Lazy<ushort>(() => mrDicomAccess.GetBitsStored(dataSet));
            m_HighBit = new Lazy<ushort>(() => mrDicomAccess.GetHighBit(dataSet));
            m_PixelRepresentation = new Lazy<ushort>(() => mrDicomAccess.GetPixelRepresentation(dataSet));
            m_WindowCenter = new Lazy<double>(() => mrDicomAccess.GetWindowCenter(dataSet));
            m_WindowWidth = new Lazy<double>(() => mrDicomAccess.GetWindowWidth(dataSet));
            m_RescaleIntercept = new Lazy<double>(() => mrDicomAccess.GetRescaleIntercept(dataSet));
            m_RescaleSlope = new Lazy<double>(() => mrDicomAccess.GetRescaleSlope(dataSet));
        }

        // ----- address information -----
        public IDicomFrameDataSet DataSet { get; }
        public int FrameNumber { get; }

        // ----- ImageInfos -----
        public IImageClassInfo ImageClassInfo => m_ImageClassInfo.Value;
        public IImageDistortionInfo ImageDistortionInfo => m_ImageDistortionInfo.Value;
        public IImageOrderInfo ImageOrderInfo => m_ImageOrderInfo.Value;
        public IImagePixelInfo ImagePixelInfo => m_ImagePixelInfo.Value;
        public IImagePlaneInfo ImagePlaneInfo => m_ImagePlaneInfo.Value;
        public IImageScanInfo ImageScanInfo => m_ImageScanInfo.Value;

        // ----- basic attributes (for sorting, tooltip, etc.) -----
        public bool IsEnhancedMrImage => m_IsEnhancedMrImage.Value;
        public string SopClassUid => m_SopClassUid.Value;
        public string SopInstanceUid => m_SopInstanceUid.Value;
        public string SeriesInstanceUid => m_SeriesInstanceUid.Value;
        public string FrameOfReferenceUid => m_FrameOfReferenceUid.Value;
        public string Modality => m_Modality.Value;
        public string[] ImageTypes => m_ImageTypes.Value;
        public DateTime AcquisitionDateTime => m_AcquisitionDateTime.Value;
        public DateTime SeriesDateTime => m_SeriesDateTime.Value;
        public string StudyDescription => m_StudyDescription.Value;
        public string SeriesDescription => m_SeriesDescription.Value;
        public int SeriesNumber => m_SeriesNumber.Value;
        public int InstanceNumber => m_InstanceNumber.Value;
        // ----- scan related attributes -----
        public string ProtocolName => m_ProtocolName.Value;
        public string SequenceName => m_SequenceName.Value;
        public string ScanningSequence => m_ScanningSequence.Value;
        public string SequenceVariant => m_SequenceVariant.Value;
        public string InPlanePhaseEncodingDirection => m_InPlanePhaseEncodingDirection.Value;
        public string PhaseEncodingDirectionPositive => m_PhaseEncodingDirectionPositive.Value;
        public double SliceThickness => m_SliceThickness.Value;
        public double TimeAfterStart => m_TimeAfterStart.Value;
        public double TriggerTime => m_TriggerTime.Value;
        public Vector3D ImaRelTablePosition => m_ImaRelTablePosition.Value;
        public string PatientPosition => m_PatientPosition.Value;
        public string BodyPartExamined => m_BodyPartExamined.Value;
        public string MrAcquisitionType => m_MrAcquisitionType.Value;
        public int AcquisitionNumber => m_AcquisitionNumber.Value;

        // ----- DistortionCorrection related attributes -----
        public string DistortionCorrectionType => m_DistortionCorrectionType.Value;
        public string VolumetricProperties => m_VolumetricProperties.Value;
        public string FrameLevelVolumetricProperties => m_FrameLevelVolumetricProperties.Value;
        public string GradientCoilName => m_GradientCoilName.Value;

        // ----- ImagePlane related attributes -----
        public Vector3D ImagePosition => m_ImagePosition.Value;
        public Vector3D OrientationRow => m_OrientationRow.Value;
        public Vector3D OrientationCol => m_OrientationCol.Value;

        // ----- ImageMatrix related attributes -----
        public int NumberOfFrames => m_NumberOfFrames.Value;
        public ushort MatrixRows => m_MatrixRows.Value;
        public ushort MatrixCols => m_MatrixCols.Value;
        public double PixelSpacingRow => m_PixelSpacingRow.Value;
        public double PixelSpacingCol => m_PixelSpacingCol.Value;

        // --- PixelData information ---
        public string TransferSyntaxUid => m_TransferSyntaxUid.Value;
        public string LossyImageCompression => m_LossyImageCompression.Value;
        public string PhotometricInterpretation => m_PhotometricInterpretation.Value;
        public ushort SamplesPerPixel => m_SamplesPerPixel.Value;
        public ushort PlanarConfiguration => m_PlanarConfiguration.Value;
        public ushort BitsAllocated => m_BitsAllocated.Value;
        public ushort BitsStored => m_BitsStored.Value;
        public ushort HighBit => m_HighBit.Value;
        public ushort PixelRepresentation => m_PixelRepresentation.Value;

        // --- PixelTransformation and presentation information ---
        public double WindowCenter => m_WindowCenter.Value;
        public double WindowWidth => m_WindowWidth.Value;
        public double RescaleIntercept => m_RescaleIntercept.Value;
        public double RescaleSlope => m_RescaleSlope.Value;
        public string PresentationStateSopInstanceUid => m_PresentationStateSopInstanceUid.Value;
    }
}

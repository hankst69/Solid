//----------------------------------------------------------------------------------
// File: "IImageAttributes.cs"
// Author: Steffen Hanke
// Date: 2020-2021
//----------------------------------------------------------------------------------

using System;
using Solid.Infrastructure.Math;

namespace Solid.Dicom.ImageInfo
{
    /// <summary>
    /// IImageAttributes
    /// </summary>
    public interface IImageAttributes
    {
        // ----- address information -----
        IDicomFrameDataSet DataSet { get; }
        int FrameNumber { get; }

        // ----- ImageInfos -----
        IImageClassInfo ImageClassInfo { get; }
        IImageDistortionInfo ImageDistortionInfo { get; }
        IImageOrderInfo ImageOrderInfo { get; }
        IImagePixelInfo ImagePixelInfo { get; }
        IImagePlaneInfo ImagePlaneInfo { get; }
        IImageScanInfo ImageScanInfo { get; }

        // ----- basic attributes (for sorting, tooltip, etc.) -----
        bool IsEnhancedMrImage { get; }
        string SopClassUid { get; }
        string SopInstanceUid { get; }
        string SeriesInstanceUid { get; }
        string FrameOfReferenceUid { get; }
        string Modality { get; }
        string[] ImageTypes { get; }
        DateTime AcquisitionDateTime { get; }
        DateTime SeriesDateTime { get; }
        string StudyDescription { get; }
        string SeriesDescription { get; }
        int SeriesNumber { get; }
        int InstanceNumber { get; }

        // ----- scan related attributes -----
        string ProtocolName { get; }
        string SequenceName { get; }
        string ScanningSequence { get; }
        string SequenceVariant { get; }
        string InPlanePhaseEncodingDirection { get; }
        string PhaseEncodingDirectionPositive { get; }
        double SliceThickness { get; }
        double TimeAfterStart { get; }
        double TriggerTime { get; }
        Vector3D ImaRelTablePosition { get; }
        string PatientPosition { get; }
        string BodyPartExamined { get; }
        string MrAcquisitionType { get; }
        int AcquisitionNumber { get; }

        // ----- DistortionCorrection related attributes -----
        string DistortionCorrectionType { get; }
        string VolumetricProperties { get; }
        string FrameLevelVolumetricProperties { get; }
        string GradientCoilName { get; }

        // ----- ImagePlane related attributes -----
        Vector3D ImagePosition { get; }
        Vector3D OrientationRow { get; }
        Vector3D OrientationCol { get; }

        // ----- ImageMatrix related attributes -----
        int NumberOfFrames { get; }
        ushort MatrixRows { get; }
        ushort MatrixCols { get; }
        double PixelSpacingRow { get; }
        double PixelSpacingCol { get; }

        // --- PixelData information ---
        string TransferSyntaxUid { get; }
        string LossyImageCompression { get; }
        string PhotometricInterpretation { get; }
        ushort SamplesPerPixel { get; }
        ushort PlanarConfiguration { get; }
        ushort BitsAllocated { get; }
        ushort BitsStored { get; }
        ushort HighBit { get; }
        ushort PixelRepresentation { get; }

        // --- PixelTransformation and presentation information ---
        double WindowCenter { get; }
        double WindowWidth { get; }
        double RescaleIntercept { get; }
        double RescaleSlope { get; }
        string PresentationStateSopInstanceUid { get; }
    }
}

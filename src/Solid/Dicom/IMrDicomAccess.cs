//----------------------------------------------------------------------------------
// File: "IMrDicomAccess.cs"
// Author: Steffen Hanke
// Date: 2020-2021
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Solid.Dicom.ImageInfo;
using Solid.Infrastructure.Math;

namespace Solid.Dicom
{
    /// <summary>
    /// IMrDicomAccess
    /// </summary>
    public interface IMrDicomAccess
    {
        IImageAttributes CreateImageAttributes(IDicomFrameDataSet dataSet);
        bool IsEnhancedMrImage(IDicomFrameDataSet dataSet);

        // ----- basic attributes (for sorting, tooltip, etc.) -----
        string GetSopClassUid(IDicomFrameDataSet dataSet);
        string GetSopInstanceUid(IDicomFrameDataSet dataSet);
        string GetSeriesInstanceUid(IDicomFrameDataSet dataSet);
        string GetFrameOfReferenceUid(IDicomFrameDataSet dataSet);
        string GetModality(IDicomFrameDataSet dataSet);
        IEnumerable<string> GetImageTypes(IDicomFrameDataSet dataSet);
        DateTime GetAcquisitionDateTime(IDicomFrameDataSet dataSet);
        DateTime GetSeriesDateTime(IDicomFrameDataSet dataSet);
        string GetStudyDescription(IDicomFrameDataSet dataSet);
        string GetSeriesDescription(IDicomFrameDataSet dataSet);
        int GetSeriesNumber(IDicomFrameDataSet dataSet);
        int GetInstanceNumber(IDicomFrameDataSet dataSet);

        // ----- scan related attributes -----
        string GetProtocolName(IDicomFrameDataSet dataSet);
        string GetSequenceName(IDicomFrameDataSet dataSet);
        string GetScanningSequence(IDicomFrameDataSet dataSet);
        string GetSequenceVariant(IDicomFrameDataSet dataSet);
        string GetInPlanePhaseEncodingDirection(IDicomFrameDataSet dataSet);
        string GetPhaseEncodingDirectionPositive(IDicomFrameDataSet dataSet);
        double GetSliceThickness(IDicomFrameDataSet dataSet);
        double GetTimeAfterStart(IDicomFrameDataSet dataSet);
        double GetTriggerTime(IDicomFrameDataSet dataSet);
        Vector3D GetImaRelTablePosition(IDicomFrameDataSet dataSet);
        string GetPatientPosition(IDicomFrameDataSet dataSet);
        string GetBodyPartExamined(IDicomFrameDataSet dataSet);
        string GetMrAcquisitionType(IDicomFrameDataSet dataSet);
        int GetAcquisitionNumber(IDicomFrameDataSet dataSet);

        // ----- DistortionCorrection related attributes -----
        string GetDistortionCorrectionType(IDicomFrameDataSet dataSet);
        string GetVolumetricProperties(IDicomFrameDataSet dataSet);
        string GetFrameLevelVolumetricProperties(IDicomFrameDataSet dataSet);
        string GetGradientCoilName(IDicomFrameDataSet dataSet);

        // ----- ImagePlane related attributes -----
        Vector3D GetImagePosition(IDicomFrameDataSet dataSet);
        Vector3D GetOrientationRow(IDicomFrameDataSet dataSet);
        Vector3D GetOrientationCol(IDicomFrameDataSet dataSet);

        // ----- ImageMatrix related attributes -----
        int GetNumberOfFrames(IDicomFrameDataSet dataSet);
        ushort GetMatrixRows(IDicomFrameDataSet dataSet);
        ushort GetMatrixCols(IDicomFrameDataSet dataSet);
        double GetPixelSpacingRow(IDicomFrameDataSet dataSet);
        double GetPixelSpacingCol(IDicomFrameDataSet dataSet);

        // ----- PixelData related attributes -----
        string GetTransferSyntaxUid(IDicomFrameDataSet dataSet);
        string GetLossyImageCompression(IDicomFrameDataSet dataSet);
        string GetPhotometricInterpretation(IDicomFrameDataSet dataSet);
        ushort GetSamplesPerPixel(IDicomFrameDataSet dataSet);
        ushort GetPlanarConfiguration(IDicomFrameDataSet dataSet);
        ushort GetBitsAllocated(IDicomFrameDataSet dataSet);
        ushort GetBitsStored(IDicomFrameDataSet dataSet);
        ushort GetHighBit(IDicomFrameDataSet dataSet);
        ushort GetPixelRepresentation(IDicomFrameDataSet dataSet);

        // --- PixelTransformation information ---
        double GetWindowCenter(IDicomFrameDataSet dataSet);
        double GetWindowWidth(IDicomFrameDataSet dataSet);
        double GetRescaleIntercept(IDicomFrameDataSet dataSet);
        double GetRescaleSlope(IDicomFrameDataSet dataSet);


        // ----- data visualization related attributes -----
        string GetPresentationStateSopInstanceUid(IDicomFrameDataSet dataSet);
        //void SetImageTextConfig(syngo.Services.IDicomDataSet dataSet, string GetimageTextViewName);
    }
}

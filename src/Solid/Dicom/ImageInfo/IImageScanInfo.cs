//----------------------------------------------------------------------------------
// File: "IImageScanInfo.cs"
// Author: Steffen Hanke
// Date: 2019
//----------------------------------------------------------------------------------

namespace Solid.Dicom.ImageInfo
{
    /// <summary>
    /// IImageScanInfo
    /// </summary>
    public interface IImageScanInfo
    {
        // A) dicom attributes
        string ProtocolName { get; }
        string SequencelName { get; }
        string InPlanePhaseEncodingDirection { get; }

        // B) calculated predicates
        bool IsInplanePhaseInRowDirection { get; }
        bool IsPhaseEncodingDirectionPositive { get; }
    }
}

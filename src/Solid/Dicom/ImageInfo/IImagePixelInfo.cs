//----------------------------------------------------------------------------------
// File: "IImagePixelInfo.cs"
// Author: Steffen Hanke
// Date: 2020-2021
//----------------------------------------------------------------------------------

namespace Solid.Dicom.ImageInfo
{
    /// <summary>
    /// IImagePixelInfo
    /// </summary>
    public interface IImagePixelInfo
    {
        // A) dicom attributes
        // --- PixelMatrix information ---
        int NumberOfFrames { get; }
        int PixelRows { get; }
        int PixelColumns { get; }
        double PixelSizeInRowDir { get; }
        double PixelSizeInColDir { get; }
        double SliceThickness { get; }

        // --- PixelData information ---
        string TransferSyntaxUid { get; }
        string PhotometricInterpretation { get; }
        ushort SamplesPerPixel { get; }
        ushort PlanarConfiguration { get; }
        ushort BitsAllocated { get; }
        ushort BitsStored { get; }
        ushort HighBit { get; }
        ushort PixelRepresentation { get; }

        // --- PixelTransformation information ---
        double WindowCenter { get; }
        double WindowWidth { get; }
        double RescaleIntercept { get; }
        double RescaleSlope { get; }
        string LossyImageCompression { get; }


        // B) calculated predicates
        bool PixelDataIsBigEndian { get; }
        bool PixelDataIsSigned { get; }
        bool PixelDataIsLossyCompressed { get; }
    }
}

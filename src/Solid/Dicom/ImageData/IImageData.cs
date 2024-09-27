//----------------------------------------------------------------------------------
// File: "IImageData.cs"
// Author: Steffen Hanke
// Date: 2020
//----------------------------------------------------------------------------------
using System;
using System.Drawing;
using Solid.Dicom.ImageInfo;

namespace Solid.Dicom.ImageData
{
    /// <summary>
    /// IImageData
    /// </summary>
    public interface IImageData
    {
        // --- Main dicom information ---
        string SopInstanceUid { get; }
        int FrameNumber { get; }
        IImageAttributes ImageAttributes { get; }

        // --- Structured dicom information  ---
        IImageClassInfo ImageClassInfo { get; }
        IImagePlaneInfo ImagePlaneInfo { get; }
        IImagePixelInfo ImagePixelInfo { get; }
        IImageDistortionInfo ImageDistortionInfo { get; }
        IImageOrderInfo ImageOrderInfo { get; }

        // --- ImagePlane handling ---
        bool InvertImagePlaneNormal { get; set; }

        // --- PixelData handling ---
        void LoadPixelData();

        bool IsPixelDataLoaded { get; }

        byte[] PixelData { get; } // the uninterpreted pixels as stored in dicom file

        ushort[] GetPixelsAsUshortGreyValues();
        int GetPixelsAsUshortGreyValues(ushort[] targetArray, int targetStartIndex);

        T[] GetPixelsAsGreyValues<T>() where T: struct, IComparable;
        int GetPixelsAsGreyValues<T>(T[] targetArray, int targetStartIndex) where T : struct, IComparable;
        Bitmap GetPixelsAsBitmap();
    }
}

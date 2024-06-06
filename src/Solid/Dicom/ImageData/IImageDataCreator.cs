//----------------------------------------------------------------------------------
// File: "IImageDataCreator.cs"
// Author: Steffen Hanke
// Date: 2019
//----------------------------------------------------------------------------------

namespace Solid.Dicom.ImageData
{
    /// <summary>
    /// IImageDataCreator
    /// </summary>
    public interface IImageDataCreator
    {
        IImageData CreateImageData(IDicomFrameDataSet inputDicomFrameDataSet);
        IImageData CreateImageDataWithLoadedPixelData(IDicomFrameDataSet inputDicomFrameDataSet);
    }
}

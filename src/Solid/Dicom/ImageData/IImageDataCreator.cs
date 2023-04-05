//----------------------------------------------------------------------------------
// <copyright file="IImageDataCreator.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2019. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
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

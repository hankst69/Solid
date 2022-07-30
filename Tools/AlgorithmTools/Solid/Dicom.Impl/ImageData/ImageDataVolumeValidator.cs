//----------------------------------------------------------------------------------
// <copyright file="ImageDataVolumeValidator.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2019. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Solid.Dicom.ImageData.Impl
{
    /// <summary>
    /// API:NO
    /// ImageDataVolumeGrouper
    /// </summary>
    public class ImageDataVolumeValidator : IImageDataVolumeValidator
    {
        public bool ValidateVolume(IEnumerable<IImageData> inputImages)
        {
            return true;
        }
    }
}

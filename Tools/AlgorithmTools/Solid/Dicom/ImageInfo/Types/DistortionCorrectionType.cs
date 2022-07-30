//----------------------------------------------------------------------------------
// <copyright file="DistortionCorrectionType.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2019-2021. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

namespace Solid.Dicom.ImageInfo.Types
{
    /// <summary>
    /// DistortionCorrectionType
    /// </summary>
    public enum DistortionCorrectionType
    {
        Unknown,
        ND,
        DIS2D,
        DIS2DBent,
        DIS3D
    }
}

//----------------------------------------------------------------------------------
// <copyright file="IImagePlaneInfo.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using Solid.Dicom.ImageInfo.Types;
using Solid.Infrastructure.Math;

namespace Solid.Dicom.ImageInfo
{
    /// <summary>
    /// IImagePlaneInfo
    /// </summary>
    public interface IImagePlaneInfo
    {
        // A) dicom attributes
        /// <summary>
        /// the position of center of first voxel in patient coordinate system
        /// </summary>
        Vector3D Position { get; }
        /// <summary>
        /// the orientation of a pixel matrix row in patient coordinate system
        /// </summary>
        Vector3D OrientationRow { get; }
        /// <summary>
        /// the orientation of a pixel matrix column in patient coordinate system
        /// </summary>
        Vector3D OrientationCol { get; }

        // B) calculated predicates
        bool ContainsImagePlane { get; }
        Vector3D ImageOrientationNormal { get; }
        OrientationType ImageOrientationMain { get; }
        double ImagePositionInNormalDirection { get; }

        bool InvertImageNormal { get; set; }
    }
}

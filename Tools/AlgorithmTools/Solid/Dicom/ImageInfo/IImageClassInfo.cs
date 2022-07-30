//----------------------------------------------------------------------------------
// <copyright file="IImageClassInfo.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2019-2021. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

namespace Solid.Dicom.ImageInfo
{
    /// <summary>
    /// IImageClassInfo
    /// </summary>
    public interface IImageClassInfo
    {
        // A) dicom attributes
        string SopClassUid { get; }
        string Modality { get; }
        string ProtocolName { get; }
        string SequencelName { get; }
        string FrameOfReferenceUid { get; }
        string[] ImageTypes { get; }

        // B) calculated predicates
        bool IsMrImage { get; }
        bool IsDerivedImage { get; }
        bool IsDerivedSubImage { get; }
        bool IsDerivedCompImage { get; }
        bool IsDerivedCprImage { get; }
        //bool Is3d { get; }
    }
}

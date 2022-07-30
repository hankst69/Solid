//----------------------------------------------------------------------------------
// <copyright file="ImageDistortionInfo.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2019-2020. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Linq;
using Solid.Dicom.ImageInfo.Types;
using Solid.Infrastructure.Diagnostics;

namespace Solid.Dicom.ImageInfo.Impl
{
    /// <summary>
    /// API:NO
    /// ImageDistortionInfo
    /// </summary>
    public class ImageDistortionInfo : IImageDistortionInfo
    {
        private readonly IImageAttributes m_ImageAttributes;

        private readonly Lazy<DistortionCorrectionType> m_ConsolidatedDistortioncorrectionType;


        internal ImageDistortionInfo(IImageAttributes dicomAccess)
        {
            ConsistencyCheck.EnsureArgument(dicomAccess).IsNotNull();
            m_ImageAttributes = dicomAccess;

            // --- calculate derived predicates ---
            m_ConsolidatedDistortioncorrectionType = new Lazy<DistortionCorrectionType>(() =>
                CalculateConsolidatedDistortionCorrectionType(m_ImageAttributes));
        }

        // A) dicom attributes
        public string DistortionCorrectionType => m_ImageAttributes.DistortionCorrectionType;

        public string VolumetricProperties => m_ImageAttributes.VolumetricProperties;
        public string FrameLevelVolumetricProperties => m_ImageAttributes.FrameLevelVolumetricProperties;
        public string GradientCoilName => m_ImageAttributes.GradientCoilName;
        public string FrameOfReferenceUid => m_ImageAttributes.FrameOfReferenceUid;

        // B) calculated predicates
        public DistortionCorrectionType ConsolidatedDistortionCorrectionType =>
            m_ConsolidatedDistortioncorrectionType.Value;

        private DistortionCorrectionType CalculateConsolidatedDistortionCorrectionType(IImageAttributes imageAttributes)
        {
            //using (new FTrace(s_TraceDomain))
            {
                ConsistencyCheck.EnsureArgument(imageAttributes).IsNotNull();

                // detetct distortion corrected based on Siemens private attribute DistortionCorrectionType (frame level)
                var distortionTypeName = imageAttributes.DistortionCorrectionType.ToUpper();
                switch (distortionTypeName)
                {
                    case "DIS2D":
                        return Types.DistortionCorrectionType.DIS2D;

                    case "DISTORTED":
                        return Types.DistortionCorrectionType.DIS2DBent;

                    case "DIS3D":
                        return Types.DistortionCorrectionType.DIS3D;

                    case "ND":
                        return Types.DistortionCorrectionType.ND;
                }

                // DistortionCorrectionType did not exist (e.g. data prior NumarisX or range images from RHS; see charm MR_00473736) or contained unexpected values
                // -> detect distortion correction based on VolumetricProperties
                // WARNING: 
                // as of 2016/08/18 the VolumetricProperties attribute is not always correctly filled 
                // the value remains on VOLUME if inverse DistortionCorrection was applied to previously corrected images (see charm MR_00475336)

                // read and analyze instance level VolumetricProperties
                var volumetricProperties = imageAttributes.VolumetricProperties.ToUpper();

                if (volumetricProperties.Equals("VOLUME"))
                {
                    // all frames are 2D or 3D distortion corrected
                    return Types.DistortionCorrectionType.DIS3D; // ?DIS2D?
                }

                if (volumetricProperties.Equals("MIXED"))
                {
                    // a mix of successfully distortion corrected frames and bent frames (red slices)
                    // -> read and analyze frame level VolumetricProperties
                    var frameLevelVolProps = imageAttributes.FrameLevelVolumetricProperties.ToUpper();
                    switch (frameLevelVolProps)
                    {
                        case "VOLUME":
                            return Types.DistortionCorrectionType.DIS2D; // ?DIS3D?

                        case "DISTORTED":
                            return Types.DistortionCorrectionType.DIS2DBent;
                    }
                }

                // TODO: handle MRFrameOfReference encoding
                /*
                // detection of distortion correction based on VolumentricProperties was not possible
                // -> detect distortion correction based on MR FrameOfReferenceUid
                var frameOfReferenceUid = imageAttributes.FrameOfReferenceUid;
                if (MRFoR.IsValidMRFoR(frameOfReferenceUid))
                {
                    string dicomUidRoot;
                    DateTime timeStamp;
                    MRFoR.DistortionCorrectionType distortionCorrectionType;
                    MRFoR.TablePosition tablePosition;
                    bool containsDistInfo;
                    bool containsTablePosition;

                    var result = MRFoR.SplitFoRString(frameOfReferenceUid, out dicomUidRoot, out timeStamp, out distortionCorrectionType, out tablePosition, out containsDistInfo, out containsTablePosition);

                    if (result && containsDistInfo)
                    {
                        switch (distortionCorrectionType)
                        {
                            case MRFoR.DistortionCorrectionType.DistortionCorrectionIn2D:
                                return DistortionCorrectionType.DIS2D;

                            case MRFoR.DistortionCorrectionType.DistortionCorrectionIn2DBent:
                                return DistortionCorrectionType.DIS2DBent;

                            case MRFoR.DistortionCorrectionType.DistortionCorrectionIn3D:
                                return DistortionCorrectionType.DIS3D;

                            case MRFoR.DistortionCorrectionType.NotDistortionCorrected:
                                return DistortionCorrectionType.ND;
                        }
                    }
                }*/

                // detection of distortion correction based on MR FrameOfReferenceUid was not possible
                // -> detect distortion correction based on ImageType
                var imageTypes = imageAttributes.ImageTypes.ToList();

                if (imageTypes.Contains("ND"))
                    return Types.DistortionCorrectionType.ND;

                if (imageTypes.Contains("DIS3D"))
                    return Types.DistortionCorrectionType.DIS3D;

                if (imageTypes.Contains("DISTORTED"))
                    return Types.DistortionCorrectionType.DIS2DBent;

                if (imageTypes.Contains("DIS2D"))
                    return Types.DistortionCorrectionType.DIS2D;

                // DistortionCorrectionType still unknown
                return Types.DistortionCorrectionType.Unknown;
            }
        }
    }
}
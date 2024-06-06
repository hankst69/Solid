//----------------------------------------------------------------------------------
// File: "ImagePlaneInfo.cs"
// Author: Steffen Hanke
// Date: 2020
//----------------------------------------------------------------------------------

using System;
using Solid.Dicom.ImageInfo.Types;
using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.Math;

namespace Solid.Dicom.ImageInfo.Impl
{
    /// <summary>
    /// API:NO
    /// ImagePlaneInfo
    /// </summary>
    public class ImagePlaneInfo : IImagePlaneInfo
    {
        private readonly IImageAttributes m_ImageAttributes;

        private readonly Lazy<bool> m_ContainsImagePlane;
        private readonly Lazy<OrientationType> m_ImageOrientationMain;
        private Lazy<Vector3D> m_ImageOrientationNormal;
        private Lazy<double> m_ImagePositionInNormalDirection;
        private bool m_InvertImageNormal;

        internal ImagePlaneInfo(IImageAttributes dicomAccess)
        {
            ConsistencyCheck.EnsureArgument(dicomAccess).IsNotNull();
            m_ImageAttributes = dicomAccess;


            // --- calculate derived predicates ---
            InvertImageNormal = false;
            DefineImageNormalPredicates();

            m_ContainsImagePlane = new Lazy<bool>(() =>
            {
                if (Position == null || OrientationRow == null || OrientationCol == null ||
                    Position.IsNil() || OrientationRow.IsNil() || OrientationCol.IsNil())
                {
                    return false;
                }
                return true;
            });

            m_ImageOrientationMain = new Lazy<OrientationType>(() =>
            {
                if (ImageOrientationNormal == null || ImageOrientationNormal.IsNil())
                {
                    return OrientationType.Undefined;
                }
                // LPH -> xyz == SagCorTra
                var x = Math.Abs(ImageOrientationNormal.X);
                var y = Math.Abs(ImageOrientationNormal.Y);
                var z = Math.Abs(ImageOrientationNormal.Z);
                if (x > y && x > z)
                {
                    return OrientationType.Sagittal;
                }
                if (y > x && y > z)
                {
                    return OrientationType.Coronal;
                }
                if (z > x && z > y)
                {
                    return OrientationType.Transversal;
                }
                return OrientationType.Undefined;
            });
        }

        private void DefineImageNormalPredicates()
        {
            if (m_ImageOrientationNormal == null || m_ImageOrientationNormal.IsValueCreated)
            {
                // (re)create m_ImageOrientationNormal Lazy to reflect current InvertImageNormal setting
                m_ImageOrientationNormal = new Lazy<Vector3D>(() =>
                {
                    if (OrientationRow == null || OrientationCol == null || OrientationRow.IsNil() || OrientationCol.IsNil())
                    {
                        return null;
                    }
                    var normalizedImageNormal = OrientationCol.GetOuterProduct(OrientationRow).GetNormalized();
                    return InvertImageNormal ? normalizedImageNormal.GetInverted() : normalizedImageNormal;
                });
            }

            if (m_ImagePositionInNormalDirection == null || m_ImagePositionInNormalDirection.IsValueCreated)
            {
                // (re)create m_ImagePositionInNormalDirection Lazy to reflect current InvertImageNormal setting
                m_ImagePositionInNormalDirection = new Lazy<double>(() =>
                {
                    if (Position == null || Position.IsNil() || ImageOrientationNormal == null || ImageOrientationNormal.IsNil())
                    {
                        return double.NaN;
                    }
                    return Position.GetInnerProduct(ImageOrientationNormal);
                });
            }
        }


        // A) dicom attributes
        public Vector3D Position => m_ImageAttributes.ImagePosition;
        public Vector3D OrientationRow => m_ImageAttributes.OrientationRow;
        public Vector3D OrientationCol => m_ImageAttributes.OrientationCol;

        // B) calculated predicates
        public bool ContainsImagePlane => m_ContainsImagePlane.Value;
        public Vector3D ImageOrientationNormal => m_ImageOrientationNormal.Value;
        public OrientationType ImageOrientationMain => m_ImageOrientationMain.Value;
        public double ImagePositionInNormalDirection => m_ImagePositionInNormalDirection.Value;

        public bool InvertImageNormal
        {
            get => m_InvertImageNormal;
            set
            {
                if (value == m_InvertImageNormal)
                {
                    return;
                }
                m_InvertImageNormal = value;
                DefineImageNormalPredicates();
            }
        }
    }
}
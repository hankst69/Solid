//----------------------------------------------------------------------------------
// File: "ImageScanInfo.cs"
// Author: Steffen Hanke
// Date: 2019
//----------------------------------------------------------------------------------
using System;
using Solid.Infrastructure.Diagnostics;

namespace Solid.Dicom.ImageInfo.Impl
{
    /// <summary>
    /// API:NO
    /// ImageScanInfo
    /// </summary>
    public class ImageScanInfo : IImageScanInfo
    {
        private readonly IImageAttributes m_ImageAttributes;

        private readonly Lazy<bool> m_IsInplanePhaseInRowDirection;
        private readonly Lazy<bool> m_IsPhaseEncodingDirectionPositive;

        internal ImageScanInfo(IImageAttributes dicomAccess)
        {
            ConsistencyCheck.EnsureArgument(dicomAccess).IsNotNull();
            m_ImageAttributes = dicomAccess;

            // --- calculate derived predicates ---
            m_IsInplanePhaseInRowDirection = new Lazy<bool>(() =>
            {
                if (string.IsNullOrEmpty(InPlanePhaseEncodingDirection))
                {
                    // failed to read optional dicom tag InPlanePhaseEncodingDirection (0018, 1312) -> Assuming default value "ROW" -> reurn true
                    return true;
                }
                return InPlanePhaseEncodingDirection.ToUpper().Equals("ROW");
            });

            m_IsPhaseEncodingDirectionPositive = new Lazy<bool>(() =>
            {
                if (string.IsNullOrEmpty(PhaseEncodingDirectionPositive))
                {
                    // failed to read optional dicom tag PhaseEncodingDirectionPositive (0018, 1312) -> Assuming default value "1" -> reurn true
                    return true;
                }
                return PhaseEncodingDirectionPositive.ToUpper().Equals("1");
            });
        }

        // A) dicom attributes
        public string ProtocolName => m_ImageAttributes.ProtocolName;
        public string SequencelName => m_ImageAttributes.SequenceName;
        public string InPlanePhaseEncodingDirection => m_ImageAttributes.InPlanePhaseEncodingDirection;
        public string PhaseEncodingDirectionPositive => m_ImageAttributes.PhaseEncodingDirectionPositive;

        // B) calculated predicates
        public bool IsInplanePhaseInRowDirection => m_IsInplanePhaseInRowDirection.Value;
        public bool IsPhaseEncodingDirectionPositive => m_IsPhaseEncodingDirectionPositive.Value;
    }
}

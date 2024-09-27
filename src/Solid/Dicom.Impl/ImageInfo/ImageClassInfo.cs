//----------------------------------------------------------------------------------
// File: "ImageClassInfo.cs"
// Author: Steffen Hanke
// Date: 2019-2021
//----------------------------------------------------------------------------------
using System;
using System.Linq;
using Solid.Infrastructure.Diagnostics;

namespace Solid.Dicom.ImageInfo.Impl
{
    /// <summary>
    /// API:NO
    /// ImageClassInfo
    /// </summary>
    public class ImageClassInfo : IImageClassInfo
    {
        private readonly IImageAttributes m_ImageAttributes;

        private readonly Lazy<bool> m_IsMr;
        private readonly Lazy<bool> m_IsDerived;
        private readonly Lazy<bool> m_IsDerivedSub;
        private readonly Lazy<bool> m_IsDerivedComposed;
        private readonly Lazy<bool> m_IsDerivedCpr;

        internal ImageClassInfo(IImageAttributes dicomAccess)
        {
            ConsistencyCheck.EnsureArgument(dicomAccess).IsNotNull();
            m_ImageAttributes = dicomAccess;

            // --- calculate derived predicates ---
            m_IsMr = new Lazy<bool>(() =>
            {
                // todo clarify: will Modality=="MR" automatically exclude spectroscopy images or is it necessry also to check against set of valid MR-SopClassUids?
                var isMr = !string.IsNullOrEmpty(Modality) && Modality.ToUpper().Equals("MR");
                return isMr;
            });

            m_IsDerived = new Lazy<bool>(() =>
            {
                var isDerived = ImageTypes.Any(type => type.ToUpper().Equals("DERIVED"));
                if (!isDerived)
                {
                    ConsistencyCheck.EnsureValue(IsDerivedSubImage).IsNotEqual(true);
                    ConsistencyCheck.EnsureValue(IsDerivedCompImage).IsNotEqual(true);
                    ConsistencyCheck.EnsureValue(IsDerivedCprImage).IsNotEqual(true);
                }
                return isDerived;
            });

            m_IsDerivedSub = new Lazy<bool>(() =>
            {
                return ImageTypes.Any(IsSubImageType);
            });

            m_IsDerivedComposed = new Lazy<bool>(() =>
            {
                // todo clarify: in GSP IsComposed is implemented as follows (why? is CPR always composed?)
                return ImageTypes.Any(type => IsComposedImageType(type) || IsCprImageType(type));
            });

            m_IsDerivedCpr = new Lazy<bool>(() =>
            {
                return ImageTypes.Any(IsCprImageType);
            });
        }

        // A) dicom attributes
        public string SopClassUid => m_ImageAttributes.SopClassUid;
        public string Modality => m_ImageAttributes.Modality;
        public string ProtocolName => m_ImageAttributes.ProtocolName;
        public string SequencelName => m_ImageAttributes.SequenceName;
        public string FrameOfReferenceUid => m_ImageAttributes.FrameOfReferenceUid;
        public string[] ImageTypes => m_ImageAttributes.ImageTypes;

        // B) calculated predicates
        public bool IsMrImage => m_IsMr.Value;
        public bool IsDerivedImage => m_IsDerived.Value;
        public bool IsDerivedSubImage => IsDerivedImage && m_IsDerivedSub.Value;
        public bool IsDerivedCompImage => IsDerivedImage && m_IsDerivedComposed.Value;
        public bool IsDerivedCprImage => IsDerivedImage && m_IsDerivedCpr.Value;

        private static bool IsSubImageType(string imageTypeValue)
        {
            ConsistencyCheck.EnsureArgument(imageTypeValue).IsNotNull();
            switch (imageTypeValue.ToUpper())
            {
                case "SUB":
                case "SUBTRACTION":
                    return true;
            }
            return false;
        }

        private static bool IsComposedImageType(string imageTypeValue)
        {
            ConsistencyCheck.EnsureArgument(imageTypeValue).IsNotNull();
            switch (imageTypeValue.ToUpper())
            {
                case "COMPOSED":
                case "COMP_SP":
                case "COMP_MIP":
                case "COMP_AN":
                case "COMP_AD":
                    return true;
            }
            return false;
        }

        private static bool IsCprImageType(string imageTypeValue)
        {
            ConsistencyCheck.EnsureArgument(imageTypeValue).IsNotNull();
            switch (imageTypeValue.ToUpper())
            {
                case "CPR":
                case "CPR_STAR":
                    return true;
            }
            return false;
        }
    }
}

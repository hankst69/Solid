//----------------------------------------------------------------------------------
// File: "DicomTagHandling.cs"
// Author: Steffen Hanke
// Date: 2019
//----------------------------------------------------------------------------------

using System.Globalization;
using System.Text;

namespace Solid.Dicom.Impl
{
    public static class DicomTagHandling
    {
        /// <apiflag>yes</apiflag>
        /// <summary>API:yes Describes the tag type supported by the syngo Data Model.</summary>
        public enum TagType : short
        {
            Missing,
            Standard,
            Private,
        }

        /// <apiflag>yes</apiflag>
        /// <summary>API:yes Returns an Tag Type describing the tag origin.</summary>
        /// <param name="theLocalTag">The Local Tag.</param>
        /// <returns>The Tag Type describing the tag origin.</returns>
        public static DicomTagHandling.TagType GetTagType(long theLocalTag)
        {
            return DicomTagHandling.IsStandard(theLocalTag) ? DicomTagHandling.TagType.Standard : DicomTagHandling.TagType.Private;
        }

        /// <apiflag>yes</apiflag>
        /// <summary>API:yes Gets a value indicating whether the data element is standard (defined by the DICOM standard).</summary>
        /// <remarks>Standard data elements have even group numbers and are listed in the DICOM Data Element Dictionary in PS 3.6.</remarks>
        /// <param name="theLocalTag">The Local Tag.</param>
        /// <returns>True if the local tag describes a standard element, otherwise false.</returns>
        public static bool IsStandard(long theLocalTag)
        {
            return ((theLocalTag & 0xFFFF0000) >> 16) % 2L == 0L;
        }

        /// <apiflag>yes</apiflag>
        /// <summary>API:yes Gets a value indicating whether the data element is private (not defined by the DICOM standard).</summary>
        /// <remarks>Private elements are additional data elements, defined by an implementor, to communicate information that is not contained in the standard data elements. Private data elements have odd group numbers.</remarks>
        /// <param name="theLocalTag">The Local Tag.</param>
        /// <returns>True if the local tag describes a private element, otherwise false.</returns>
        public static bool IsPrivate(long theLocalTag)
        {
            return ((theLocalTag & 0xFFFF0000) >> 16) % 2L == 1L;
        }

        //-------------------------------------------------------------------------------------------------------------

        /// <apiflag>yes</apiflag>
        /// <summary>API:yes Returns the creator tag part of a local tag.</summary>
        /// <param name="theLocalTag">The Local Tag.</param>
        /// <returns>The creator tag part of the local tag.</returns>
        public static int GetCreatorTag(long theLocalTag)
        {
            return (int)(((ulong)theLocalTag & 0xFFFFFFFF00000000) >> 32);
        }

        /// <apiflag>yes</apiflag>
        /// <summary>API:yes Returns the element tag part of a local tag.</summary>
        /// <param name="theLocalTag">The Local Tag.</param>
        /// <returns>The element tag part of the local tag.</returns>
        public static int GetElementTag(long theLocalTag)
        {
            return (int)(theLocalTag & 0xFFFFFFFF);
        }

        /// <apiflag>yes</apiflag>
        /// <summary>API:yes Returns the business unit code part of a local tag.</summary>
        /// <param name="theLocalTag">The Local Tag.</param>
        /// <returns>The business unit code part of the local tag.</returns>
        public static short GetBusinessUnitCode(long theLocalTag)
        {
            return (short)(((ulong)theLocalTag & 0xFFFF000000000000) >> 48);
        }

        /// <apiflag>yes</apiflag>
        /// <summary>API:yes Returns the resource number part of a local tag.</summary>
        /// <param name="theLocalTag">The Local Tag.</param>
        /// <returns>The resource number part of the local tag.</returns>
        public static short GetResourceNumber(long theLocalTag)
        {
            return (short)((theLocalTag & 0xFFFF00000000) >> 32);
        }

        public static string GetPrivateCreatorCode(long theLocalTag)
        {
            var buCode = GetBusinessUnitCode(theLocalTag);
            var resCode = GetResourceNumber(theLocalTag);

            var sb = new StringBuilder();
            sb.Append((char) ((buCode & 0xff00) >> 8));
            sb.Append((char) (buCode & 0xff));
            var bucStr = sb.ToString().ToUpper();
            var resStr = resCode.ToString("X4").ToUpper();

            var rows = PrivateCreatorCodes.PrivateCreatorCodesMappingTable.GetLength(0);
            var cols = PrivateCreatorCodes.PrivateCreatorCodesMappingTable.GetLength(1);
            if (rows < 1 || cols < 3)
            {
                return string.Empty;
            }
            for (var i = 0; i < rows; i++)
            {
                if (PrivateCreatorCodes.PrivateCreatorCodesMappingTable[i,1].ToUpper() == bucStr
                    && PrivateCreatorCodes.PrivateCreatorCodesMappingTable[i, 2].ToUpper() == resStr)
                {
                    return PrivateCreatorCodes.PrivateCreatorCodesMappingTable[i, 0];
                }
            }
            return string.Empty;
        }

        public static bool IsPrivateLocalTagOfVrTypeSequence(long localTag)
        {
            return localTag == DicomTags.MrPrivateDicomTags.SiemensMrSdiSequence ||
                   localTag == DicomTags.MrPrivateDicomTags.SiemensMrSdsSequence;
        }

        /// <apiflag>yes</apiflag>
        /// <summary>API:yes Combines the given codes and numbers to a syngo Local Tag.</summary>
        /// <param name="theGroupNumber">A group number to identify the DICOM group of the local tag.</param>
        /// <param name="theElementNumber">a element number to identify the DICOM standard element and a element byte to identify the the DICOM private element, respectively.</param>
        /// <param name="privateCreatorCode"></param>
        /// <returns>The syngo Local Tag.</returns>
        public static long MakeLocalTag(ushort theGroupNumber, ushort theElementNumber, string privateCreatorCode)
        {
            var rows = PrivateCreatorCodes.PrivateCreatorCodesMappingTable.GetLength(0);
            var cols = PrivateCreatorCodes.PrivateCreatorCodesMappingTable.GetLength(1);
            if (rows < 1 || cols < 3)
            {
                return 0;
            }
            var prvcode = privateCreatorCode.ToUpper();
            for (var i = 0; i < rows; i++)
            {
                if (PrivateCreatorCodes.PrivateCreatorCodesMappingTable[i, 0].ToUpper() == prvcode)
                {
                    var buStr = PrivateCreatorCodes.PrivateCreatorCodesMappingTable[i, 1].ToUpper();
                    var resStr = PrivateCreatorCodes.PrivateCreatorCodesMappingTable[i, 2].ToUpper();

                    var theBusinessUnitCode = (buStr[0] << 8) + buStr[1];
                    var theResourceNumber = ushort.Parse(resStr, NumberStyles.HexNumber);

                    return MakeLocalTag((ushort)theBusinessUnitCode, theResourceNumber, theGroupNumber, theElementNumber);
                }
            }
            return 0;
        }

        /// <apiflag>yes</apiflag>
        /// <summary>API:yes Returns the group number part of a local tag.</summary>
        /// <param name="theLocalTag">The Local Tag.</param>
        /// <returns>The group number part of the local tag.</returns>
        public static short GetGroupNumber(long theLocalTag)
        {
            return (short)((theLocalTag & 0xFFFF0000) >> 16);
        }

        /// <apiflag>yes</apiflag>
        /// <summary>API:yes Returns the element number part of a local tag.</summary>
        /// <param name="theLocalTag">The Local Tag.</param>
        /// <returns>The element number part of the local tag.</returns>
        public static short GetElementNumber(long theLocalTag)
        {
            return (short)(theLocalTag & 0xFFFF);
        }

        /// <apiflag>yes</apiflag>
        /// <summary>API:yes Combines the given codes and numbers to a syngo Local Tag.</summary>
        /// <param name="theBusinessUnitCode">A business unit code to identify the business unit responsible for a local tag, e.g. 0x4D52 (MR), 0x4354 (CT).</param>
        /// <param name="theResourceNumber">A resource number to identify the creator of the local tag.</param>
        /// <param name="theGroupNumber">A group number to identify the DICOM group of the local tag.</param>
        /// <param name="theElementNumber">a element number to identify the DICOM standard element and a element byte to identify the the DICOM private element, respectively.</param>
        /// <returns>The syngo Local Tag.</returns>
        public static long MakeLocalTag(ushort theBusinessUnitCode, ushort theResourceNumber, ushort theGroupNumber, ushort theElementNumber)
        {
            return ((long)theBusinessUnitCode << 48) + ((long)theResourceNumber << 32) +
                   ((long)theGroupNumber << 16) + (long)theElementNumber;
        }

        //-------------------------------------------------------------------------------------------------------------

        ///// <apiflag>no</apiflag>
        ///// <summary>API:no The current task data.</summary>
        //private static LocalTagDictionary myLocalTagDictionary = new LocalTagDictionary();
        //
        ///// <apiflag>yes</apiflag>
        ///// <summary>API:yes Gets the Value Representation (AE, AS, AT, CS, ...) associated with the specified Local Tag.</summary>
        ///// <param name="theLocalTag">The Local Tag.</param>
        ///// <returns>The Value Representation associated with the specified Local Tag.</returns>
        //public static ValueRepresentation.Id GetValueRepresentation(long theLocalTag)
        //{
        //    if (DicomTags.myLocalTagDictionary.ContainsLocalTag(theLocalTag))
        //        return DicomTags.myLocalTagDictionary.GetValueRepresentationEnum(theLocalTag);
        //    return ValueRepresentation.Id.xx;
        //}
        ///// <apiflag>yes</apiflag>
        ///// <summary>API:yes Gets the Creator Code associated with the specified Local Tag.</summary>
        ///// <param name="theLocalTag">The Local Tag.</param>
        ///// <param name="theCreatorCode">The Creator Code associated with the specified Local Tag. A blank string is returned if the given Local Tag specifies a Standard Data Element and null is returned if the Creator Code could not be determined.</param>
        ///// <returns>True if the Creator Code of the specified Loacal Tag could be found; otherwise, false.</returns>
        //public static bool TryGetCreatorCode(long theLocalTag, out string theCreatorCode)
        //{
        //    if (DicomTags.IsStandard(theLocalTag))
        //    {
        //        theCreatorCode = "";
        //        return true;
        //    }
        //    int creatorTag = DicomTags.GetCreatorTag(theLocalTag);
        //    if (DicomTags.myLocalTagDictionary.ContainsCreatorTag(creatorTag))
        //    {
        //        theCreatorCode = DicomTags.myLocalTagDictionary.GetCreatorCode(creatorTag);
        //        return true;
        //    }
        //    theCreatorCode = (string)null;
        //    return false;
        //}
    }
}
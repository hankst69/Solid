//----------------------------------------------------------------------------------
// <copyright file="DicomValues.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2019. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Globalization;
using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.RuntimeTypeExtensions;

namespace Solid.Dicom
{
    public static class DicomValues
    {
        public static ushort ConvertDicomUsToUshort(object dicomUs)
        {
            ConsistencyCheck.EnsureArgument(dicomUs).IsNotNull();
            return dicomUs.CastTo<ushort>(); //return (ushort)dicomFd;
        }

        public static double ConvertDicomFdToDouble(object dicomFd)
        {
            ConsistencyCheck.EnsureArgument(dicomFd).IsNotNull();
            return dicomFd.CastTo<double>(); //return (double)dicomFd;
        }

        public static int ConvertDicomIsToInt(object dicomIs)
        {
            ConsistencyCheck.EnsureArgument(dicomIs).IsNotNull();
            if (dicomIs is int)
            {
                return dicomIs.CastTo<int>();
            }
            var ds = dicomIs.As<string>();
            if (string.IsNullOrWhiteSpace(ds))
            {
                return 0;
            }
            if (!int.TryParse(
                    ds.Trim(),
                    NumberStyles.AllowLeadingSign,
                    (IFormatProvider)new CultureInfo("en-us"),
                    out int result))
            {
                return 0;
            }
            return result;
        }

        public static double ConvertDicomDsToDouble(object dicomDs)
        {
            ConsistencyCheck.EnsureArgument(dicomDs).IsNotNull();
            if (dicomDs is decimal || dicomDs is double || dicomDs is float)
            {
                return dicomDs.CastTo<double>();
            }
            var ds = dicomDs.As<string>();
            if (string.IsNullOrWhiteSpace(ds))
            {
                return double.NaN;
            }
            if (!double.TryParse(
                    ds.Trim(),
                    NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
                    (IFormatProvider) new CultureInfo("en-us"),
                    out double result)
                || double.IsInfinity(result) || (double.IsNaN(result) || double.IsNegativeInfinity(result)) ||
                double.IsPositiveInfinity(result))
            {
                return double.NaN;
            }
            return result;
        }

        public static bool CanConvertDicomIsToInt(object dicomIs)
        {
            ConsistencyCheck.EnsureArgument(dicomIs).IsNotNull();
            if (dicomIs is int)
            {
                return true;
            }
            var ds = dicomIs.As<string>();
            if (string.IsNullOrWhiteSpace(ds))
            {
                return false;
            }
            if (!int.TryParse(
                ds.Trim(),
                NumberStyles.AllowLeadingSign,
                (IFormatProvider)new CultureInfo("en-us"),
                out int result))
            {
                return false;
            }
            return true;
        }

        public static bool CanConvertDicomDsToDouble(object dicomDs)
        {
            ConsistencyCheck.EnsureArgument(dicomDs).IsNotNull();
            if (dicomDs is double || dicomDs is float)
            {
                return true;
            }
            var ds = dicomDs.As<string>();
            if (string.IsNullOrWhiteSpace(ds))
            {
                return false;
            }
            if (!double.TryParse(
                    ds.Trim(),
                    NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
                    (IFormatProvider)new CultureInfo("en-us"),
                    out double result)
                || double.IsInfinity(result) || (double.IsNaN(result) || double.IsNegativeInfinity(result)) ||
                double.IsPositiveInfinity(result))
            {
                return false;
            }
            return true;
        }

        public static DateTime ConvertDicomDTtoDateTime(object dicomDt)
        {
            ConsistencyCheck.EnsureArgument(dicomDt).IsNotNull();
            if (dicomDt is DateTime)
            {
                return dicomDt.CastTo<DateTime>();
            }
            string dateTime = dicomDt.As<string>();
            if (string.IsNullOrEmpty(dateTime))
            {
                return DateTime.MinValue;
            }

            //syngo.MR.Common.DataAccess.DicomFormatConverter.ParseDateTimeFromString(datetime) implementation:
            // DT - Date Time
            // The Date Time common data type. Indicates a concatenated date-time ASCII string in 
            // the format: "YYYYMMDDHHMMSS.FFFFFF&ZZZZ". 
            // The components of this string, from left to right, are YYYY = Year, MM = Month, 
            // DD = Day, HH = Hour, MM = Minute, SS = Second, FFFFFF = Fractional Second, 
            // & = "+" or "-", and ZZZZ = Hours and Minutes of offset. &ZZZZ is an optional 
            // suffix for plus/minus offset from Coordinated Universal Time. A component that 
            // is omitted from the string is termed a null component. Trailing null components 
            // of Date Time are ignored. Non-trailing null components are prohibited, given that 
            // the optional suffix is not considered as a component. 
            var strDateTime = dateTime.Trim();
            int len = strDateTime.Length;
            int year = 0, month = 1, day = 1, hour = 0, min = 0, sec = 0, msec = 0;

            if (len >= 4)
                year = Convert.ToInt32(strDateTime.Substring(0, 4));
            if (len >= 6)
                month = Convert.ToInt32(strDateTime.Substring(4, 2));
            if (len >= 8)
                day = Convert.ToInt32(strDateTime.Substring(6, 2));
            if (len >= 10)
                hour = Convert.ToInt32(strDateTime.Substring(8, 2));
            if (len >= 12)
                min = Convert.ToInt32(strDateTime.Substring(10, 2));
            if (len >= 14)
                sec = Convert.ToInt32(strDateTime.Substring(12, 2));

            if (len > 14)
            {
                int dotIdx = strDateTime.IndexOf('.', 14);
                int ampIdx = strDateTime.IndexOfAny(new char[] {'+', '-'}, 14);

                // note that for retrieving the correct msec value, we first have to pad 
                // zeros, because the msec value can also be "123" or "1234" or simply "1".
                // Minimum is 0 chars, maximum is 6 chars due to dicom standard.
                if (dotIdx >= 0 && ampIdx < 0)
                    msec = Convert.ToInt32(strDateTime.Substring(dotIdx + 1).PadRight(6, '0').Substring(0, 3));
                else if (dotIdx >= 0 && ampIdx >= 0)
                    msec = Convert.ToInt32(strDateTime.Substring(dotIdx + 1, ampIdx - dotIdx - 1).PadRight(6, '0')
                        .Substring(0, 3));
            }
            // plus/minus offset is not considered here
            return new DateTime(year, month, day, hour, min, sec, msec);

            // syngo.MR.Tools.MeanCurve.BizLogic.Core.MecDicomAccess.DicomDTValueAsDateToime() implementation:
            ////20110222085201.550000
            //var strDateTime = dateTime.Trim();
            //if (strDateTime.Length < 14)
            //{
            //    return DateTime.MinValue;
            //}
            //int y = 0, m = 0, d = 0, hh = 0, mm = 0;
            //double ss = 0.0;
            //y = int.Parse(strDateTime.Substring(0, 4));
            //m = int.Parse(strDateTime.Substring(4, 2));
            //d = int.Parse(strDateTime.Substring(6, 2));
            //hh = int.Parse(strDateTime.Substring(8, 2));
            //mm = int.Parse(strDateTime.Substring(10, 2));
            //ss = ConvertDicomDsToDouble(strDateTime.Substring(12) as object);
            //double dms = ss * 1000.0;
            //int nms = (int)Math.Truncate(dms);
            //int ms = nms % 1000;
            ////System.Globalization.Calendar cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            //return new DateTime(y, m, d, hh, mm, (int)Math.Truncate(ss), ms); //, cal);
        }

        public static DateTime ConvertDicomDAandDicomTMtoDateTime(object dicomDa, object dicomTm)
        {
            ConsistencyCheck.EnsureArgument(dicomDa).IsNotNull();
            ConsistencyCheck.EnsureArgument(dicomTm).IsNotNull();

            var strDate = dicomDa.As<string>();
            if (string.IsNullOrEmpty(strDate))
            {
                return DateTime.MinValue;
            }
            var strTime = dicomTm.As<string>();
            if (string.IsNullOrEmpty(strTime))
            {
                return DateTime.MinValue;
            }

            return ConvertDicomDTtoDateTime(strDate + strTime);
            //var dicomDate = new DicomDA(strDate);
            //var dicomTime = new DicomTM(strTime);
            //var date = dicomDate.Date;
            //var time = dicomTime.Time;
            //// the DICOM time is now a DateTime where the date part is filled with DicomDA.BASE_DATE so we have to remove that before adding to date
            //var pureTime = time - DicomDA.BASE_DATE;
            //var dateTime = date + pureTime;
            //return dateTime;
        }

        public static DateTime ConvertDicomDAtoDateTime(object dicomDa)
        {
            ConsistencyCheck.EnsureArgument(dicomDa).IsNotNull();
            return ConvertDicomDAandDicomTMtoDateTime(dicomDa, "000000.000000");
        }
    }
}
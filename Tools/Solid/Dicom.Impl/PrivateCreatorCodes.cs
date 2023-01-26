/* Generation:
>cd D:\TFS\NXMainline_Ant\x64_d\bin\Services\ElementDictionary 
>del syngoPrivateCreatorCodes.txt
>for /F %f in ('dir /b /s *.xml') do @type "%f" | @grep PrivateCreatorIdentificationCode >>syngoPrivateCreatorCodes.txt
>echo public static class PrivateCreatorCodes { >PrivateCreatorCodes.cs
>echo public static readonly string[,] PrivateCreatorCodesMappingTable = { >>PrivateCreatorCodes.cs
>for /F "tokens=2,3,4* delims===^>" %i in (syngoPrivateCreatorCodes.txt) do @echo { %i, %j, %k }, | sed s/.BusinessUnitCode// | sed s/.ResourceCode// >>PrivateCreatorCodes.cs
>echo }; } >>PrivateCreatorCodes.cs
*/

namespace Solid.Dicom.Impl
{
    public static class PrivateCreatorCodes
    {
        public static readonly string[,] PrivateCreatorCodesMappingTable = 
        {
            { "SIEMENS CT APPL ALG PARAMS", "CT", "0005" },
            { "SIEMENS CT APPL DATASET", "CT", "0000" },
            { "SIEMENS CT APPL EVIDENCEDOCUMENT", "CT", "0003" },
            { "SIEMENS CT APPL MEASUREMENT", "CT", "0001" },
            { "SIEMENS CT APPL PRESENTATION", "CT", "0002" },
            { "SIEMENS CT APPL TMP DATAMODEL", "CT", "0004" },
            { "SIEMENS CT EXAM APP SHARED", "CT", "0199" },
            { "SIEMENS CT EXAM IMAGE", "CT", "0101" },
            { "SIEMENS CT SPP HEADER", "CT", "0006" },
            { "SIEMENS CT VA0  COAD", "CT", "0010" },
            { "SIEMENS MED", "CT", "0011" },
            { "GEMS_PARM_01", "CT", "1000" },
            { "SIEMENS CSA ENVELOPE", "SW", "1003" },
            { "SIEMENS CSA REPORT", "SW", "1005" },
            { "SIEMENS MEDCOM HEADER", "SW", "1000" },
            { "SIEMENS SYNGO 3D FUSION MATRIX", "SW", "200C" },
            { "SIEMENS SYNGO ALPHA CAD", "SW", "2016" },
            { "SIEMENS SYNGO DATA PADDING", "SW", "200B" },
            { "SIEMENS SYNGO ENCAPSULATED DOCUMENT DATA", "SW", "200F" },
            { "SIEMENS SYNGO EVIDENCE DOCUMENT DATA", "SW", "2006" },
            { "SIEMENS SYNGO FRAME SET", "SW", "2002" },
            { "SIEMENS SYNGO FUNCTION ASSIGNMENT", "SW", "2013" },
            { "SIEMENS SYNGO INDEX SERVICE", "SW", "2004" },
            { "SIEMENS SYNGO INSTANCE MANIFEST", "SW", "200A" },
            { "SIEMENS SYNGO LAYOUT PROTOCOL", "SW", "2003" },
            { "SIEMENS SYNGO MODULES", "SW", "2014" },
            { "SIEMENS SYNGO OBJECT GRAPHICS", "SW", "2011" },
            { "SIEMENS SYNGO PRINT PREVIEW", "SW", "2020" },
            { "SIEMENS SYNGO PRINT SERVICE", "SW", "2000" },
            { "SIEMENS SYNGO REGISTRATION", "SW", "2009" },
            { "SIEMENS SYNGO RWVM", "SW", "2017" },
            { "SIEMENS SYNGO SEGMENTATION", "SW", "2018" },
            { "SIEMENS SYNGO SEGMENTATION DICOM MESH PRIVATEDATA", "SW", "2019" },
            { "SIEMENS SYNGO SOP CLASS PACKING", "SW", "2015" },
            { "SIEMENS SYNGO TIME POINT SERVICE", "SW", "2008" },
            { "SIEMENS SYNGO ULTRA-SOUND TOYON DATA STREAMING", "SW", "2012" },
            { "SIEMENS SYNGO VOLUME", "SW", "2001" },
            { "SIEMENS SYNGO WORKFLOW", "SW", "2007" },
            { "GEMS_PETD_01", "MI", "1002" },
            { "Philips PET Private Group", "MI", "1003" },
            { "SIEMENS MED ORIENTATION RESULT", "MI", "1007" },
            { "SIEMENS MED BRAIN ORIENTATION DATA", "MI", "1008" },
            { "SIEMENS MED MEASUREMENT", "MI", "1006" },
            { "SIEMENS MED ECAT FILE INFO", "MI", "100A" },
            { "SIEMENS MED MI", "MI", "100C" },
            { "SIEMENS MED NM", "MI", "1000" },
            { "SIEMENS MED PT", "MI", "1001" },
            { "SIEMENS MED RTSTRUCT", "MI", "1005" },
            { "SIEMENS PET DERIVED", "MI", "100B" },
            { "SIEMENS MI RWVM SUV", "MI", "1004" },
            { "GEMS_ACQU_01", "MR", "2020" },
            { "GEMS_IMAG_01", "MR", "2023" },
            { "GEMS_PARM_01", "MR", "2024" },
            { "GEMS_RELA_01", "MR", "2022" },
            { "Philips MR Imaging DD 001", "MR", "2005" },
            { "SIEMENS MR CARDIAC", "MR", "1051" },
            { "SIEMENS MR SDI 02", "MR", "1002" },
            { "SIEMENS MR HEADER", "MR", "1010" },
            { "SIEMENS MR WIPF 01", "MR", "1032" },
            { "SIEMENS MR IMA", "MR", "1008" },
            { "SIEMENS MR DATAMAPPING ATTRIBUTES", "MR", "3001" },
            { "SIEMENS MR HEADER", "MR", "1010" },
            { "SIEMENS MR EXTRACTED CSA HEADER", "MR", "2001" },
            { "SIEMENS MR FMRI", "MR", "2021" },
            { "SIEMENS MR HISTOGRAM", "MR", "100A" },
            { "SIEMENS MR SDR 01", "MR", "100C" },
            { "SIEMENS MR TRENDING", "MR", "100D" },
            { "SIEMENS MR VOLUME", "MR", "1009" },
            { "SIEMENS MR N3D", "MR", "1007" },
            { "SIEMENS MR NEURO", "MR", "1038" },
            { "SIEMENS MR PHOENIX ATTRIBUTES", "MR", "4001" },
            { "SIEMENS MR RAW DATA", "MR", "1008" },
            { "SIEMENS MR SDI PHOENIXZIP", "MR", "100B" },
            { "SIEMENS MR SDS 01", "MR", "1001" },
            { "SIEMENS MR WIPS 01", "MR", "1031" },
            { "SIEMENS MR MRS 05", "MR", "1005" },
            { "SIEMENS MR T4D", "MR", "1050" },
            { "Philips Imaging DD 001", "MR", "2006" },
            { "Philips MR Imaging DD 005", "MR", "2007" },
        };
    }
}

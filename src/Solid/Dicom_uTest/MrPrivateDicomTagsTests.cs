//----------------------------------------------------------------------------------
// File: "MrPrivateDicomTagsTests.cs"
// Author: Steffen Hanke
// Date: 2016-2020
//----------------------------------------------------------------------------------

using FluentAssertions;
using NUnit.Framework;
using MrPrivateDicomTags = Solid.Dicom.DicomTags.MrPrivateDicomTags;

namespace Solid.Dicom_uTest
{
    class MrPrivateDicomTagsTests
    {
        // \bin\Services\SemanticalDataSetAccess\Schemas\MR\MagneticResonanceImage.xml
        // \bin\Services\SemanticalDataSetAccess\Schemas\MR\EnhancedMagneticResonanceImage.xml
        // \bin\Services\ElementDictionary\MR\ElementDictionary--MR-SIEMENS_SERIES_SHADOW_ATTRIBUTES.xml
        // \bin\Services\ElementDictionary\MR\ElementDictionary--MR-SIEMENS_IMAGE_SHADOW_ATTRIBUTES.xml
        // \bin\Services\ElementDictionary\MR\ElementDictionary--MR-SIEMENS_MR_SDR_01.xml

        [Test]
        public void Calculate_PrivateTag_SiemensMrSdsSequence()
        {
            // \bin\Services\ElementDictionary\MR\ElementDictionary--MR-SIEMENS_SERIES_SHADOW_ATTRIBUTES.xml
            //<DataElements PrivateCreatorIdentificationCode="SIEMENS MR SDS 01" BusinessUnitCode="MR" ResourceCode="1001">
            //    <PrivateDataElement GroupNumber="0021" ElementID="FE" Name="Siemens MR SDS Sequence" VR="SQ" VM="1"/><!-- NumX - eMR Image only -->
            //</DataElements>
            var privateTag = CalculatePrivateLocalTag(
                businessUnitCode: "MR", 
                privateCreatorIdentificationCode: "SIEMENS MR SDS 01", 
                privateCreatorRessourceCode: 0x1001, 
                privateGroup: 0x0021, 
                privateId: 0xFE);
            privateTag.Should().Be(MrPrivateDicomTags.SiemensMrSdsSequence);
        }

        [Test]
        public void Calculate_PrivateTag_SiemensMrSdiSequence()
        {
            // \bin\Services\ElementDictionary\MR\ElementDictionary--MR-SIEMENS_IMAGE_SHADOW_ATTRIBUTES.xml
            //<DataElements PrivateCreatorIdentificationCode="SIEMENS MR SDI 02" BusinessUnitCode="MR" ResourceCode="1002">
            //    <PrivateDataElement GroupNumber="0021" ElementID="FE" Name="Siemens MR SDI Sequence" VR="SQ" VM="1"/><!-- NumX - eMR Image only -->
            //</DataElements>
            var privateTag = CalculatePrivateLocalTag(
                businessUnitCode: "MR", 
                privateCreatorIdentificationCode: "SIEMENS MR SDI 02", 
                privateCreatorRessourceCode: 0x1002, 
                privateGroup: 0x0021, 
                privateId: 0xFE);
            privateTag.Should().Be(MrPrivateDicomTags.SiemensMrSdiSequence);
        }

        [Test]
        public void Calculate_PrivateTag_DistortionCorrectionType()
        {
            // \bin\Services\ElementDictionary\MR\ElementDictionary--MR-SIEMENS_IMAGE_SHADOW_ATTRIBUTES.xml
            //<DataElements PrivateCreatorIdentificationCode="SIEMENS MR SDI 02" BusinessUnitCode="MR" ResourceCode="1002">
            //  <PrivateDataElement GroupNumber="0021" ElementID="79" Name="DistortionCorrectionType" Constant="DistortionCorrectionType" VR="CS" VM="1"/><!-- syngo classic - eMR Image/MR Image -->
            //</DataElements>
            var privateTag = CalculatePrivateLocalTag(
                businessUnitCode: "MR", 
                privateCreatorIdentificationCode: "SIEMENS MR SDI 02", 
                privateCreatorRessourceCode: 0x1002, 
                privateGroup: 0x0021, 
                privateId: 0x79);
            privateTag.Should().Be(MrPrivateDicomTags.DistortionCorrectionType);
        }

        [Test]
        public void Calculate_PrivateTag_ImaRelTablePosition()
        {
            // \bin\Services\ElementDictionary\MR\ElementDictionary--MR-SIEMENS_IMAGE_SHADOW_ATTRIBUTES.xml
            //<DataElements PrivateCreatorIdentificationCode="SIEMENS MR SDI 02" BusinessUnitCode="MR" ResourceCode="1002">
            //  <PrivateDataElement GroupNumber="0021" ElementID="59" Name="ImaRelTablePosition" Constant="ImaRelTablePosition" VR="IS" VM="3"/><!-- syngo classic - eMR Image/MR Image -->
            //</DataElements>
            var privateTag = CalculatePrivateLocalTag(
                businessUnitCode: "MR", 
                privateCreatorIdentificationCode: "SIEMENS MR SDI 02", 
                privateCreatorRessourceCode: 0x1002, 
                privateGroup: 0x0021, 
                privateId: 0x59);
            privateTag.Should().Be(MrPrivateDicomTags.ImaRelTablePosition);
        }

        [Test]
        public void Calculate_PrivateTag_RelTablePosition()
        {
            // \bin\Services\ElementDictionary\MR\ElementDictionary--MR-SIEMENS_SERIES_SHADOW_ATTRIBUTES.xml
            //<DataElements PrivateCreatorIdentificationCode="SIEMENS MR SDS 01" BusinessUnitCode="MR" ResourceCode="1001">
            //  <PrivateDataElement GroupNumber="0021" ElementID="05" Name="RelTablePosition" Constant="RelTablePosition" VR="IS" VM="3"/><!-- syngo classic - eMR Image/MR Image -->
            //</DataElements>
            var privateTag = CalculatePrivateLocalTag(
                businessUnitCode: "MR",
                privateCreatorIdentificationCode: "SIEMENS MR SDS 01",
                privateCreatorRessourceCode: 0x1001,
                privateGroup: 0x0021,
                privateId: 0x05);
            privateTag.Should().Be(MrPrivateDicomTags.RelTablePosition);
        }

        [Test]
        public void Calculate_PrivateTag_ImageType4MF()
        {
            // \bin\Services\ElementDictionary\MR\ElementDictionary--MR-SIEMENS_IMAGE_SHADOW_ATTRIBUTES.xml
            //<DataElements PrivateCreatorIdentificationCode="SIEMENS MR SDI 02" BusinessUnitCode="MR" ResourceCode="1002">
            //  <PrivateDataElement GroupNumber="0021" ElementID="75" Name="ImageType4MF" Constant="ImageType4MF" VR="CS" VM="1-n"/><!-- meaning has been changed !!!-->
            //</DataElements>
            var privateTag = CalculatePrivateLocalTag(
                businessUnitCode: "MR",
                privateCreatorIdentificationCode: "SIEMENS MR SDI 02",
                privateCreatorRessourceCode: 0x1002,
                privateGroup: 0x0021,
                privateId: 0x75);
            privateTag.Should().Be(MrPrivateDicomTags.ImageType4MF);
        }

        [Test]
        public void Calculate_PrivateTag_ScanningSequence()
        {
            // \bin\Services\ElementDictionary\MR\ElementDictionary--MR-SIEMENS_SERIES_SHADOW_ATTRIBUTES.xml
            //<DataElements PrivateCreatorIdentificationCode="SIEMENS MR SDS 01" BusinessUnitCode="MR" ResourceCode="1001">
            //  <PrivateDataElement GroupNumber="0021" ElementID="5A" Name="ScanningSequence" Constant="ScanningSequence" VR="CS" VM="1"/><!-- NumX - eMR Image only -->
            //</DataElements>
            var privateTag = CalculatePrivateLocalTag(
                businessUnitCode: "MR",
                privateCreatorIdentificationCode: "SIEMENS MR SDS 01",
                privateCreatorRessourceCode: 0x1001,
                privateGroup: 0x0021,
                privateId: 0x5A);
            privateTag.Should().Be(MrPrivateDicomTags.ScanningSequence);
        }

        [Test]
        public void Calculate_PrivateTag_SequenceVariant()
        {
            // \bin\Services\ElementDictionary\MR\ElementDictionary--MR-SIEMENS_SERIES_SHADOW_ATTRIBUTES.xml
            //<DataElements PrivateCreatorIdentificationCode="SIEMENS MR SDS 01" BusinessUnitCode="MR" ResourceCode="1001">
            //  <PrivateDataElement GroupNumber="0021" ElementID="5B" Name="SequenceVariant" Constant="SequenceVariant" VR="CS" VM="1"/><!-- NumX - eMR Image only -->
            //</DataElements>
            var privateTag = CalculatePrivateLocalTag(
                businessUnitCode: "MR",
                privateCreatorIdentificationCode: "SIEMENS MR SDS 01",
                privateCreatorRessourceCode: 0x1001,
                privateGroup: 0x0021,
                privateId: 0x5B);
            privateTag.Should().Be(MrPrivateDicomTags.SequenceVariant);
        }

        [Test]
        public void Calculate_PrivateTag_TimeAfterStart()
        {
            // \bin\Services\ElementDictionary\MR\ElementDictionary--MR-SIEMENS_IMAGE_SHADOW_ATTRIBUTES.xml
            //<DataElements PrivateCreatorIdentificationCode="SIEMENS MR SDI 02" BusinessUnitCode="MR" ResourceCode="1002">
            //  <PrivateDataElement GroupNumber="0021" ElementID="04" Name="TimeAfterStart" Constant="TimeAfterStart" VR="DS" VM="1"/><!-- syngo classic - eMR Image/MR Image -->
            //</DataElements>
            var privateTag = CalculatePrivateLocalTag(
                businessUnitCode: "MR",
                privateCreatorIdentificationCode: "SIEMENS MR SDI 02",
                privateCreatorRessourceCode: 0x1002,
                privateGroup: 0x0021,
                privateId: 0x04);
            privateTag.Should().Be(MrPrivateDicomTags.TimeAfterStart);
        }


        [Test]
        public void Calculate_PrivateTag_PhaseEncodingDirectionPositive()
        {
            // \bin\Services\ElementDictionary\MR\ElementDictionary--MR-SIEMENS_IMAGE_SHADOW_ATTRIBUTES.xml
            //<DataElements PrivateCreatorIdentificationCode="SIEMENS MR SDI 02" BusinessUnitCode="MR" ResourceCode="1002">
            //  <PrivateDataElement GroupNumber="0021" ElementID="1C" Name="PhaseEncodingDirectionPositive" Constant="PhaseEncodingDirectionPositive" VR="IS" VM="1"/><!-- syngo classic - eMR Image/MR Image -->
            //</DataElements>
            var privateTag = CalculatePrivateLocalTag(
                businessUnitCode: "MR",
                privateCreatorIdentificationCode: "SIEMENS MR SDI 02",
                privateCreatorRessourceCode: 0x1002,
                privateGroup: 0x0021,
                privateId: 0x1C);
            privateTag.Should().Be(MrPrivateDicomTags.PhaseEncodingDirectionPositive);
        }

        [Test]
        public void Calculate_PrivateTag_CreatorIdentifier()
        {
            // \bin\Services\ElementDictionary\MR\ElementDictionary--MR-SIEMENS_MR_SDR_01.xml
            //<DataElements PrivateCreatorIdentificationCode="SIEMENS MR SDR 01" BusinessUnitCode="MR" ResourceCode="100C">
            //  <PrivateDataElement GroupNumber="0021" ElementID="01" Name="Creator Identifier" VR="LO" VM="1"/>
            //</DataElements>
            var privateTag = CalculatePrivateLocalTag(
                businessUnitCode: "MR",
                privateCreatorIdentificationCode: "SIEMENS MR SDR 01",
                privateCreatorRessourceCode: 0x100C,
                privateGroup: 0x0021,
                privateId: 0x01);
            privateTag.Should().Be(MrPrivateDicomTags.CreatorIdentifier);
        }

        [Test]
        public void Calculate_PrivateTag_ApplicationIdentifier()
        {
            // \bin\Services\ElementDictionary\MR\ElementDictionary--MR-SIEMENS_MR_SDR_01.xml
            //<DataElements PrivateCreatorIdentificationCode="SIEMENS MR SDR 01" BusinessUnitCode="MR" ResourceCode="100C">
            //  <PrivateDataElement GroupNumber="0021" ElementID="02" Name="Application Identifier" VR="LO" VM="1"/>
            //</DataElements>
            var privateTag = CalculatePrivateLocalTag(
                businessUnitCode: "MR",
                privateCreatorIdentificationCode: "SIEMENS MR SDR 01",
                privateCreatorRessourceCode: 0x100C,
                privateGroup: 0x0021,
                privateId: 0x02);
            privateTag.Should().Be(MrPrivateDicomTags.ApplicationIdentifier);
        }

        [Test]
        public void Calculate_PrivateTag_GradientCoilName()
        {
            //<DataElements PrivateCreatorIdentificationCode="SIEMENS MR SDS 01" BusinessUnitCode="MR" ResourceCode="1001">
            //  <PrivateDataElement GroupNumber="0021" ElementID="33" Name="CoilForGradient2" VR="SH" VM="1"/>
            //</DataElements>
            //public const long GradientCoilName = 0x4D52100100210033;
            var privateTag = CalculatePrivateLocalTag(
                businessUnitCode: "MR",
                privateCreatorIdentificationCode: "SIEMENS MR SDS 01",
                privateCreatorRessourceCode: 0x1001,
                privateGroup: 0x0021,
                privateId: 0x33);
            privateTag.Should().Be(MrPrivateDicomTags.GradientCoilName);
        }

        //[Test]
        //public void Calculate_PrivateTag_ImageTextViewName()
        //{
        //    // PrivateCreatorIdentificationCode="SIEMENS SYNGO ADVANCED PRESENTATION" BusinessUnitCode="SW" ResourceCode="2005" 
        //    // GroupNumber="0029" ElementID="B1" Name="ImageText View Name" VR="ST" VM="1"
        //    var privateTag = CalculatePrivateLocalTag(
        //        businessUnitCode: "SW",
        //        privateCreatorIdentificationCode: "SIEMENS SYNGO ADVANCED PRESENTATION",
        //        privateCreatorRessourceCode: 0x2005,
        //        privateGroup: 0x0029,
        //        privateId: 0xB1);
        //    privateTag.Should().Be(MrPrivateDicomTags.ImageTextViewName);
        //}


        private long CalculatePrivateLocalTag(string businessUnitCode, string privateCreatorIdentificationCode, short privateCreatorRessourceCode, short privateGroup, byte privateId)
        {
            // input validation
            businessUnitCode.Length.Should().Be(2);
            privateCreatorIdentificationCode.Should().NotBeNullOrEmpty();
            privateCreatorRessourceCode.Should().BeGreaterThan(0);
            privateGroup.Should().BeGreaterThan(0);
            privateId.Should().BeGreaterThan(0);

            // tag calculation
            long localTag = ((((((((long)businessUnitCode[0] << 8) + businessUnitCode[1]) << 16) + privateCreatorRessourceCode) << 16) + privateGroup) << 16) + privateId;

            //// validation of tupel <businessUnitCode, privateCreatorIdentificationCode, privateCreatorRessourceCode> against hardcoded dictionaary 
            //var creatorCodeDictionary = new Dictionary<string, long>
            //{
            //    //<DataElements PrivateCreatorIdentificationCode="SIEMENS MR SDS 01" BusinessUnitCode="MR" ResourceCode="1001">
            //    //<DataElements PrivateCreatorIdentificationCode="SIEMENS MR SDI 02" BusinessUnitCode="MR" ResourceCode="1002">
            //    //<DataElements PrivateCreatorIdentificationCode="SIEMENS MR SDR 01" BusinessUnitCode="MR" ResourceCode="100C">
            //    //<DataElements PrivateCreatorIdentificationCode="SIEMENS MEDCOM HEADER" BusinessUnitCode="SW" ResourceCode="1000">
            //    {"SIEMENS MR SDS 01",  0x4d521001},      //4d52="MR"(BusinessUnitCode) 1001="SIEMENS MR SDS 01"(ResourceCode)
            //    {"SIEMENS MR SDI 02",  0x4d521002},      //4d52="MR"(BusinessUnitCode) 1002="SIEMENS MR SDI 02"(ResourceCode)
            //    {"SIEMENS MR SDR 01",  0x4d52100c},      //4d52="MR"(BusinessUnitCode) 100c="SIEMENS MR SDR 01"(ResourceCode)
            //    {"SIEMENS MEDCOM HEADER", 0x53571000},   //5357="SW"(BusinessUnitCode) 1000="SIEMENS MEDCOM HEADER"(ResourceCode)
            //};
            //creatorCodeDictionary.Should().ContainKey(privateCreatorIdentificationCode);
            //long localTagFromDict = (creatorCodeDictionary[privateCreatorIdentificationCode] << 32) + (0x00000000FFFFFFFF & (unchecked((UInt16)privateGroup) << 16)) + unchecked(privateId);
            //localTagFromDict.Should().Be(localTag);

            return localTag;
        }
    }
}

//<DataElements PrivateCreatorIdentificationCode="SIEMENS MR VOLUME" BusinessUnitCode="MR" ResourceCode="1009">
//  <PrivateDataElement GroupNumber="0021" ElementID="01" Name="Frame Derivation Description" VR="ST" VM="1"/>
//  <PrivateDataElement GroupNumber="0021" ElementID="02" Name="Frame Source Image Sequence" VR="SQ" VM="0-n"/>
//</DataElements>

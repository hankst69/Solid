//----------------------------------------------------------------------------------
// <copyright file="MrDicomAccessTests.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using FluentAssertions;
using Moq;
using Solid.Dicom;
using Solid.Dicom.Impl;
using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.Math;
using Solid.TestInfrastructure.Dicom;
using NUnit.Framework;

using LocalTag = Solid.Dicom.DicomTags;

namespace Solid.Dicom_uTest
{
    internal class MrDicomAccessTests
    {
        private IMrDicomAccess m_Target;
        private Mock<ITracer> m_TracerMock;

        [SetUp]
        public void Setup()
        {
            m_TracerMock = new Mock<ITracer>();
            m_TracerMock.Setup(x => x.CreateScopeTracer(It.IsAny<string>())).Returns(m_TracerMock.Object);

            m_Target = new MrDicomAccess(m_TracerMock.Object);
        }

        [Test]
        public void Ctor_ShouldThrow_WhenTracerIsNull()
        {
            // Arrange
            // Act
            Action action = () => new MrDicomAccess((ITracer)null);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CreateImageAttributes_ShouldThrow_WhenDataSetIsNull()
        {
            // Arrange
            // Act
            Action action = () => m_Target.CreateImageAttributes(null);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CreateImageAttributes_ShouldReturnImageAttributesForGivenDataSet()
        {
            // Arrange
            var dataSetMock = new Mock<IDicomFrameDataSet>();
            
            // Act
            var result = m_Target.CreateImageAttributes(dataSetMock.Object);

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void AccessInterfaceMethods_ShouldThrow_WhenDataSetIsNull()
        {
            // Arrange
            var mrDicomAccessType = typeof(Solid.Dicom.IMrDicomAccess);
            var mrDicomAccessTypeMethods = mrDicomAccessType.GetMethods();

            foreach (var methodInfo in mrDicomAccessTypeMethods)
            {
                var methodParameters = methodInfo.GetParameters();
                if (methodParameters.Length < 1 || methodParameters[0].ParameterType != typeof(IDicomFrameDataSet))
                {
                    Console.WriteLine("IMrDicomAccess member '" + methodInfo.Name + "' does not have an IDicomFrameDataSet paramater");
                    continue;
                }

                // Act
                Console.WriteLine("testing IMrDicomAccess member '" + methodInfo.Name + "'");
                Action action = () => methodInfo.Invoke(m_Target, new object[] { (IDicomFrameDataSet)null });

                // Assert
                action.Should()
                    .Throw<TargetInvocationException>()
                    .WithInnerException<ArgumentNullException>()
                    .Where(e => e.Message.StartsWith("Value cannot be null"))
                    .Where(e => e.Message.EndsWith("(Parameter 'dataSet')"));
            }
        }

        [Test]
        public void IsEnhancedMrImage_ShouldReturnTrue_WhenDataSetIsEnhancedMrImage()
        {
            var dataSet = new EnhancedMrImageBuilder()
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.IsEnhancedMrImage(dataSet);

            // Assert
            readValue.Should().BeTrue();
        }

        [Test]
        public void IsEnhancedMrImage_ShouldReturnFalse_WhenDataSetIsMrImage()
        {
            var dataSet = new MrImageBuilder()
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.IsEnhancedMrImage(dataSet);

            // Assert
            readValue.Should().BeFalse();
        }

        [Test]
        public void GetAcquisitionDateTime_ShouldReturnCombinedAcquisitionDateAndTime_WhenDataSetIsMrImage()
        {
            var acquisitionDateTime = new DateTime(2015, 10, 30, 13, 45, 59);

            var dataSet = new MrImageBuilder()
                .WithAcquisitionDateTime(acquisitionDateTime)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetAcquisitionDateTime(dataSet);

            // Assert
            readValue.Should().Be(acquisitionDateTime);
        }

        [Test]
        public void GetAcquisitionDateTime_ShouldReturnFrameAcquisitionDateTime_WhenDataSetIsEnhancedMrImage()
        {
            var frameNumber = 2;
            var frameAcquisitionDateTime = new DateTime(2015, 11, 02, 09, 32, 01);
            var frameAcquisitionDate = frameAcquisitionDateTime.ToString("yyyyMMdd");      //"20151102";
            var frameAcquisitionTime = frameAcquisitionDateTime.ToString("HHmmss.ffffff"); //"093201.000000";
            var dataSet = new EnhancedMrImageBuilder()
                .UsingFrame(frameNumber)
                .WithFrameAcquisitionDateTime(frameAcquisitionDate, frameAcquisitionTime)
                .ToDicomFrameDataSetMock(frameNumber).Object;

            // Act
            var readValue = m_Target.GetAcquisitionDateTime(dataSet);

            // Assert
            readValue.Should().Be(frameAcquisitionDateTime);
        }

        [Test]
        public void GetAcquisitionDateTime_ShouldReturnDateTimeMinValue_WhenTagsAreNotAvailable()
        {
            // Arrange
            var frameNumber = 2;
            var frame = new MrImageBuilder()
                .ToDicomFrameDataSetMock(frameNumber).Object;

            // Act
            var result = m_Target.GetAcquisitionDateTime(frame);

            // Assert
            result.Should().Be(DateTime.MinValue);
        }


        [Test]
        public void GetSeriesDescription_ShouldReturnExpectedValue_WhenDataSetIsMrImage()
        {
            // Arrange
            var expectedValue = "123456789";
            var dataSet = new MrImageBuilder()
                .WithSeriesDescription(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSeriesDescription(dataSet);

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [Test]
        public void GetSeriesDescription_ShouldReturnExpectedValue_WhenDataSetIsEnhancedMrImage()
        {
            // Arrange
            var expectedValue = "123456789";
            var dataSet = new EnhancedMrImageBuilder()
                .WithSeriesDescription(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSeriesDescription(dataSet);

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [Test]
        public void GetSeriesDescription_ShouldReturnEmptyString_WhenTagIsNotAvailable()
        {
            // Arrange
            var frame = new DataSetBuilder()
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSeriesDescription(frame);

            // Assert
            readValue.Should().BeNullOrEmpty();
        }


        [Test]
        public void GetDistortionCorrectionType_ShouldReturnExpectedValue_WhenDataSetIsMrImage()
        {
            // Arrange
            var expectedValue = "ND5D";
            var dataSet = new MrImageBuilder()
                .WithDistortionCorrectionType(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetDistortionCorrectionType(dataSet);

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [Test]
        public void GetDistortionCorrectionType_ShouldReturnExpectedValue_WhenDataSetIsEnhancedMrImage()
        {
            // Arrange
            var expectedValue = "ND5D";
            var dataSet = new EnhancedMrImageBuilder()
                .WithDistortionCorrectionType(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetDistortionCorrectionType(dataSet);

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [Test]
        public void GetDistortionCorrectionType_ShouldReturnEmptyString_WhenTagIsNotAvailable()
        {
            // Arrange
            var frame = new DataSetBuilder()
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetDistortionCorrectionType(frame);

            // Assert
            readValue.Should().BeEmpty();
        }


        [Test]
        public void GetVolumetricProperties_ShouldReturnEmptyString_WhenTagIsNotAvailable()
        {
            // Arrange
            var frame = new MrImageBuilder()
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetVolumetricProperties(frame);

            // Assert
            readValue.Should().BeEmpty();
        }

        [Test]
        public void GetVolumetricProperties_ShouldReturnExpectedValue_WhenImageIsMrImage()
        {
            // Arrange
            var expectedValue = "VolPropValue";
            var frame = new MrImageBuilder()
                .WithVolumetricProperties(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetVolumetricProperties(frame);

            // Assert
            readValue.Should().Be(expectedValue);
        }


        [Test]
        public void GetFrameLevelVolumetricProperties_ShouldReturnEmptyString_WhenTagIsNotAvailable()
        {
            // Arrange
            var frame = new EnhancedMrImageBuilder()
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetFrameLevelVolumetricProperties(frame);

            // Assert
            readValue.Should().BeEmpty();
        }

        [Test]
        public void GetFrameLevelVolumetricProperties_ShouldReturnEmptyString_WhenImageIsNotEnhancedMrImage()
        {
            // Arrange
            var frame = new MrImageBuilder()
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetFrameLevelVolumetricProperties(frame);

            // Assert
            readValue.Should().BeEmpty();
        }

        [Test]
        public void GetFrameLevelVolumetricProperties_ShouldReturnExpectedValue_WhenImageIsEnhancedMrImage()
        {
            // Arrange
            var expectedValue = "VolPropValue";
            var frame = new EnhancedMrImageBuilder()
                .WithFrameLevelVolumetricProperties(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetFrameLevelVolumetricProperties(frame);

            // Assert
            readValue.Should().Be(expectedValue);
        }


        [Test]
        public void GetPatientPosition_ShouldReturnExpectedValue_WhenFrameIsMrImage()
        {
            // Arrange
            var expectedValue = "HFS";
            var frame = new MrImageBuilder()
                .WithPatientPosition(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetPatientPosition(frame);

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [Test]
        public void GetPatientPosition_ShouldReturnExpectedValue_WhenFrameIsEnhancedMrImage()
        {
            // Arrange
            var expectedValue = "HFS";
            var frame = new EnhancedMrImageBuilder()
                .WithPatientPosition(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetPatientPosition(frame);

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [Test]
        public void GetPatientPosition_ShouldReturnEmptyString_WhenTagIsNotAvailableInFrame()
        {
            // Arrange
            var frame = new DataSetBuilder()
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetPatientPosition(frame);

            // Assert
            readValue.Should().BeEmpty();
        }


        [Test]
        public void GetSliceThickness_ShouldReturnNaN_WhenTagIsNotAvailable()
        {
            // Arrange
            var frame = new DataSetBuilder().ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSliceThickness(frame);

            // Assert
            readValue.Should().Be(double.NaN);
        }

        [Test]
        public void GetSliceThickness_ShouldReturnNaN_WhenFrameIsMrImageAndTagIsNull()
        {
            // Arrange
            var frame = new MrImageBuilder()
                .WithSliceThickness(null)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSliceThickness(frame);

            // Assert
            readValue.Should().Be(double.NaN);
        }

        [Test]
        public void GetSliceThickness_ShouldReturnNaN_WhenFrameIsEnhancedMrImageAndTagIsNull()
        {
            // Arrange
            var frame = new EnhancedMrImageBuilder()
                .WithSliceThickness(null)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSliceThickness(frame);

            // Assert
            readValue.Should().Be(double.NaN);
        }

        [TestCase("nl-BE", "0", 0.0)]
        [TestCase("nl-BE", "1", 1.0)]
        [TestCase("nl-BE", "0.9", 0.9)]
        [TestCase("nl-BE", "1.9", 1.9)]
        [TestCase("nl-BE", "10", 10.0)]
        [TestCase("nl-BE", "", double.NaN)]
        [TestCase("nl-BE", "a", double.NaN)]
        [TestCase("nl-BE", "1.234,5", double.NaN)]
        [TestCase("nl-BE", "1,234.5", double.NaN)]
        [TestCase("de-DE", "0", 0.0)]
        [TestCase("de-DE", "1", 1.0)]
        [TestCase("de-DE", "0.9", 0.9)]
        [TestCase("de-DE", "1.9", 1.9)]
        [TestCase("de-DE", "10", 10.0)]
        [TestCase("de-DE", "", double.NaN)]
        [TestCase("de-DE", "a", double.NaN)]
        [TestCase("de-DE", "1.234,5", double.NaN)]
        [TestCase("de-DE", "1,234.5", double.NaN)]
        [TestCase("en-US", "0", 0.0)]
        [TestCase("en-US", "1", 1.0)]
        [TestCase("en-US", "0.9", 0.9)]
        [TestCase("en-US", "1.9", 1.9)]
        [TestCase("en-US", "10", 10.0)]
        [TestCase("en-US", "", double.NaN)]
        [TestCase("en-US", "a", double.NaN)]
        [TestCase("en-US", "1.234,5", double.NaN)]
        [TestCase("en-US", "1,234.5", double.NaN)]
        public void GetSliceThickness_ShouldReturnExpectedValueIndependentOfCultureSetting_WhenFrameIsMrImage(string culture, string dicomDsValue, double expectedValue)
        {
            // Arrange
            //0x0813 = "nl-BE" = "Dutch (Belgium)"
            //0x0409 = "en-US" = "English (United States)"
            //0x0407 = "de-DE" = "German (Germany)"
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);

            var frame = new MrImageBuilder()
                .WithSliceThickness(dicomDsValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSliceThickness(frame);

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [TestCase("nl-BE", "0", 0.0)]
        [TestCase("nl-BE", "1", 1.0)]
        [TestCase("nl-BE", "0.9", 0.9)]
        [TestCase("nl-BE", "1.9", 1.9)]
        [TestCase("nl-BE", "10", 10.0)]
        [TestCase("nl-BE", "", double.NaN)]
        [TestCase("nl-BE", "a", double.NaN)]
        [TestCase("nl-BE", "1.234,5", double.NaN)]
        [TestCase("nl-BE", "1,234.5", double.NaN)]
        [TestCase("de-DE", "0", 0.0)]
        [TestCase("de-DE", "1", 1.0)]
        [TestCase("de-DE", "0.9", 0.9)]
        [TestCase("de-DE", "1.9", 1.9)]
        [TestCase("de-DE", "10", 10.0)]
        [TestCase("de-DE", "", double.NaN)]
        [TestCase("de-DE", "a", double.NaN)]
        [TestCase("de-DE", "1.234,5", double.NaN)]
        [TestCase("de-DE", "1,234.5", double.NaN)]
        [TestCase("en-US", "0", 0.0)]
        [TestCase("en-US", "1", 1.0)]
        [TestCase("en-US", "0.9", 0.9)]
        [TestCase("en-US", "1.9", 1.9)]
        [TestCase("en-US", "10", 10.0)]
        [TestCase("en-US", "", double.NaN)]
        [TestCase("en-US", "a", double.NaN)]
        [TestCase("en-US", "1.234,5", double.NaN)]
        [TestCase("en-US", "1,234.5", double.NaN)]
        public void GetSliceThickness_ShouldReturnExpectedValueIndependentOfCultureSetting_WhenFrameIsEnhancedMrImage(string culture, string dicomDsValue, double expectedValue)
        {
            // Arrange
            //0x0813 = "nl-BE" = "Dutch (Belgium)"
            //0x0409 = "en-US" = "English (United States)"
            //0x0407 = "de-DE" = "German (Germany)"
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);

            var frame = new EnhancedMrImageBuilder()
                .WithSliceThickness(dicomDsValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSliceThickness(frame);

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [TestCase("ROW")]
        [TestCase("COLUMN")]
        public void GetInPlanePhaseEncodingDirection_ShouldReturnExpectedValue_WhenMrImage(string direction)
        {
            // Arrange
            var frame = new MrImageBuilder()
                .WithInPlanePhaseEncodingDirection(direction)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetInPlanePhaseEncodingDirection(frame);

            // Assert
            readValue.Should().Be(direction);
        }

        [TestCase("ROW")]
        [TestCase("COLUMN")]
        public void GetInPlanePhaseEncodingDirection_ShouldReturnExpectedValue_WhenEnhancedMrImage(string direction)
        {
            // Arrange
            var frame = new EnhancedMrImageBuilder()
                .WithInPlanePhaseEncodingDirection(direction)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetInPlanePhaseEncodingDirection(frame);

            // Assert
            readValue.Should().Be(direction);
        }

        [Test]
        public void GetInPlanePhaseEncodingDirection_ShouldReturnEmptyString_WhenTagIsNotAvailable()
        {
            // Arrange
            var frameNumber = 2;
            var frame = new MrImageBuilder()
                .ToDicomFrameDataSetMock(frameNumber).Object;

            // Act
            var readValue = m_Target.GetInPlanePhaseEncodingDirection(frame);

            // Assert
            readValue.Should().Be(string.Empty);//.BeNullOrEmpty();
        }


        [TestCase(true, "1")]
        [TestCase(false, "0")]
        public void GetPhaseEncodingDirectionPositive_ShouldReturnExpectedValue_WhenMrImage(bool isPositive, string expected)
        {
            // Arrange
            var frame = new MrImageBuilder()
                .WithPhaseEncodingDirectionPositive(isPositive)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetPhaseEncodingDirectionPositive(frame);

            // Assert
            readValue.Should().Be(expected);
        }

        [TestCase(true, "1")]
        [TestCase(false, "0")]
        public void GetPhaseEncodingDirectionPositive_ShouldReturnExpectedValue_WhenEnhancedMrImage(bool isPositive, string expected)
        {
            // Arrange
            var frame = new EnhancedMrImageBuilder()
                .WithPhaseEncodingDirectionPositive(isPositive)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetPhaseEncodingDirectionPositive(frame);

            // Assert
            readValue.Should().Be(expected);
        }

        [Test]
        public void GetPhaseEncodingDirectionPositive_ShouldReturnEmptyString_WhenTagIsNotAvailable()
        {
            // Arrange
            var frameNumber = 2;
            var frame = new MrImageBuilder()
                .ToDicomFrameDataSetMock(frameNumber).Object;

            // Act
            var readValue = m_Target.GetPhaseEncodingDirectionPositive(frame);

            // Assert
            readValue.Should().Be(string.Empty);//.BeNullOrEmpty();
        }

        [Test]
        public void GetStudyDescription_ShouldReturnExpectedValue_WhenDataSetIsMrImage()
        {
            // Arrange
            var expectedValue = "123456789";
            var dataSet = new MrImageBuilder()
                .WithStudyDescription(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetStudyDescription(dataSet);

            // Assert
            readValue.Should().BeEquivalentTo(expectedValue);
        }

        [Test]
        public void GetStudyDescription_ShouldReturnExpectedValue_WhenDataSetIsEnhancedMrImage()
        {
            // Arrange
            var expectedValue = "123456789";
            var dataSet = new EnhancedMrImageBuilder()
                .WithStudyDescription(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetStudyDescription(dataSet);

            // Assert
            readValue.Should().BeEquivalentTo(expectedValue);
        }

        [Test]
        public void GetStudyDescription_ShouldReturnEmptyString_WhenTagIsNotAvailable()
        {
            // Arrange
            var frame = new DataSetBuilder()
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetStudyDescription(frame);

            // Assert
            readValue.Should().BeNullOrEmpty();
        }


        [Test]
        public void GetSeriesNumber_ShouldReturnExpectedValue_WhenDataSetIsMrImage()
        {
            // Arrange
            var expectedValue = 123456789;
            var dataSet = new MrImageBuilder()
                .WithSeriesNumber(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSeriesNumber(dataSet);

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [Test]
        public void GetSeriesNumber_ShouldReturnExpectedValue_WhenDataSetIsEnhancedMrImage()
        {
            // Arrange
            var expectedValue = 123456789;
            var dataSet = new EnhancedMrImageBuilder()
                .WithSeriesNumber(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSeriesNumber(dataSet);

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [Test]
        public void GetSeriesNumber_ShouldReturnZero_WhenTagIsNotAvailable()
        {
            // Arrange
            var frame = new DataSetBuilder()
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSeriesNumber(frame);

            // Assert
            readValue.Should().Be(0);
        }


        [Test]
        public void GetSeriesDateTime_ShouldReturnExpectedValue_WhenDataSetIsMrImage()
        {
            // Arrange
            var expectedValue = new DateTime(2014, 12, 23);
            var dataSet = new MrImageBuilder()
                .WithSeriesDate(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSeriesDateTime(dataSet);

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [Test]
        public void GetSeriesDateTime_ShouldReturnExpectedValue_WhenDataSetIsEnhancedMrImage()
        {
            // Arrange
            var expectedValue = new DateTime(2014, 12, 23);
            var dataSet = new EnhancedMrImageBuilder()
                .WithSeriesDate(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSeriesDateTime(dataSet);

            // Assert
            readValue.Should().Be(expectedValue);
        }


        [Test]
        public void GetInstanceNumber_ShouldReturnExpectedValue_WhenDataSetIsMrImage()
        {
            // Arrange
            var expectedValue = 123456789;
            var dataSet = new MrImageBuilder()
                .WithInstanceNumber(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetInstanceNumber(dataSet);

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [Test]
        public void GetInstanceNumber_ShouldReturnExpectedValue_WhenDataSetIsEnhancedMrImage()
        {
            // Arrange
            var expectedValue = 123456789;
            var dataSet = new EnhancedMrImageBuilder()
                .WithInstanceNumber(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetInstanceNumber(dataSet);

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [Test]
        public void GetInstanceNumber_ShouldReturnZero_WhenTagIsNotAvailable()
        {
            // Arrange
            var frame = new DataSetBuilder()
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetInstanceNumber(frame);

            // Assert
            readValue.Should().Be(0);
        }


        [Test]
        public void GetSeriesInstanceUID_ShouldReturnExpectedValue_WhenDataSetIsMrImage()
        {
            // Arrange
            var expectedValue = "123456789";
            var dataSet = new MrImageBuilder()
                .WithSeriesInstanceUid(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSeriesInstanceUid(dataSet);

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [Test]
        public void GetSeriesInstanceUID_ShouldReturnExpectedValue_WhenDataSetIsEnhancedMrImage()
        {
            // Arrange
            var expectedValue = "123456789";
            var dataSet = new EnhancedMrImageBuilder()
                .WithSeriesInstanceUid(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSeriesInstanceUid(dataSet);

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [Test]
        public void GetSeriesInstanceUID_ShouldReturnEmptyString_WhenTagIsNotAvailable()
        {
            // Arrange
            var frame = new DataSetBuilder()
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSeriesInstanceUid(frame);
            
            // Assert
            readValue.Should().BeNullOrEmpty();
        }


        [Test]
        public void GetSopInstanceUid_ShouldReturnExpectedValue_WhenDataSetIsMrImage()
        {
            // Arrange
            var expectedValue = "123456789";
            var dataSet = new MrImageBuilder()
                .WithSopInstanceUid(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSopInstanceUid(dataSet);

            // Assert
            readValue.Should().BeEquivalentTo(expectedValue);
        }

        [Test]
        public void GetSopInstanceUid_ShouldReturnExpectedValue_WhenDataSetIsEnhancedMrImage()
        {
            // Arrange
            var expectedValue = "123456789";
            var dataSet = new EnhancedMrImageBuilder()
                .WithSopInstanceUid(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSopInstanceUid(dataSet);

            // Assert
            readValue.Should().BeEquivalentTo(expectedValue);
        }

        [Test]
        public void GetSopInstanceUid_ShouldReturnEmptyString_WhenTagIsNotAvailable()
        {
            // Arrange
            var frame = new DataSetBuilder()
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSopInstanceUid(frame);

            // Assert
            readValue.Should().BeNullOrEmpty();
        }


        [Test]
        public void GetSopClassUid_ShouldReturnExpectedValue_WhenTagIsAvailableInFrame()
        {
            // Arrange
            var expectedValue = "123456789";
            var frame = new DataSetBuilder()
                .SetupTag(LocalTag.Tag.SopClassUid, expectedValue, 0)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSopClassUid(frame);

            // Assert
            readValue.Should().BeEquivalentTo(expectedValue);
        }

        [Test]
        public void GetSopClassUid_ShouldReturnEmptyString_WhenTagIsNotAvailableInFrame()
        {
            // Arrange
            var frame = new DataSetBuilder()
                .RemoveTag(LocalTag.Tag.SopClassUid)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSopClassUid(frame);

            // Assert
            readValue.Should().BeNullOrEmpty();
        }


        [Test]
        public void GetModality_ShouldReturnExpectedValue_WhenTagIsAvailable()
        {
            // Arrange
            var expectedValue = "123456789";
            var frame = new DataSetBuilder()
                .SetupTag(LocalTag.Tag.Modality, expectedValue, 0)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetModality(frame);

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [Test]
        public void GetModality_ShouldReturnEmptyString_WhenTagIsNotAvailable()
        {
            // Arrange
            var frame = new DataSetBuilder()
                .RemoveTag(LocalTag.Tag.Modality)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetModality(frame);

            // Assert
            readValue.Should().BeNullOrEmpty();
        }



        [Test]
        public void GetFrameOfReferenceUid_ShouldReturnExpectedValue_WhenTagIsAvailable()
        {
            // Arrange
            var expectedValue = "123456789";
            var frame = new DataSetBuilder()
                .SetupTag(LocalTag.Tag.FrameOfReferenceUid, expectedValue, 0)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetFrameOfReferenceUid(frame);

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [Test]
        public void GetFrameOfReferenceUid_ShouldReturnEmptyString_WhenTagIsNotAvailable()
        {
            // Arrange
            var frame = new DataSetBuilder()
                .RemoveTag(LocalTag.Tag.FrameOfReferenceUid)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetFrameOfReferenceUid(frame);

            // Assert
            readValue.Should().BeNullOrEmpty();
        }


        [Test]
        public void GetImageTypes_ShouldReturnExpectedValue_WhenTagIsAvailableAndImageIsMrImage()
        {
            // Arrange
            var expectedValue1 = "test1";
            var expectedValue2 = "test2";
            var expectedValue3 = "test3";
            var expectedValue4 = "test4";
            var frame = new MrImageBuilder()
                .WithImageType(expectedValue1)
                .WithImageType(expectedValue2)
                .WithImageType(expectedValue3)
                .WithImageType(expectedValue4)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValues = m_Target.GetImageTypes(frame);

            // Assert
            readValues.Should().HaveCount(4)
                .And.Equal(expectedValue1, expectedValue2, expectedValue3, expectedValue4);
        }

        [Test]
        public void GetImageTypes_ShouldReturnExpectedValue_WhenTagIsAvailableAndImageIsEnhancedMrImage()
        {
            // Arrange
            var frameNumber = 5;
            var expectedValue1 = "test1";
            var expectedValue2 = "test2";
            var expectedValue3 = "test3";
            var expectedValue4 = "test4";
            var expectedValue5 = "test5";
            var expectedValue6 = "test6";
            var expectedValue7 = "test7";
            var expectedValue8 = "test8";
            var frame = new EnhancedMrImageBuilder()
                .WithImageType(expectedValue1)
                .WithImageType(expectedValue2)
                .WithImageType(expectedValue3)
                .WithImageType(expectedValue4)
                .UsingFrame(frameNumber)
                .WithFrameType(expectedValue5)
                .WithFrameType(expectedValue6)
                .WithFrameType(expectedValue7)
                .WithFrameType(expectedValue8)
                .ToDicomFrameDataSetMock(frameNumber).Object;

            // Act
            var readValues = m_Target.GetImageTypes(frame);

            // Assert
            readValues.Should().HaveCount(8)
                .And.Equal(expectedValue1, expectedValue2, expectedValue3, expectedValue4, expectedValue5, expectedValue6, expectedValue7, expectedValue8);
        }

        [Test]
        public void GetImageTypes_ShouldReturnEmptyList_WhenTagIsNotAvailable()
        {
            // Arrange
            var frame = new DataSetBuilder()
                .RemoveTag(LocalTag.Tag.ImageType)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetImageTypes(frame);

            // Assert
            readValue.Should().BeEmpty();
        }


        [Test]
        public void GetImaRelTablePosition_ShouldReturnExpectedValue_WhenFrameIsMrImage()
        {
            // Arrange
            var expectedValue = new Vector3D(1, 2, 3);
            var frame = new MrImageBuilder()
                .WithImaRelTablePos(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetImaRelTablePosition(frame);

            // Assert
            readValue.Should().BeEquivalentTo(expectedValue);
        }

        [Test]
        public void GetImaRelTablePosition_ShouldReturnExpectedValue_WhenFrameIsEnhancedMrImage()
        {
            // Arrange
            var expectedValue = new Vector3D(1, 2, 3);
            var frame = new EnhancedMrImageBuilder()
                .WithImaRelTablePos(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetImaRelTablePosition(frame);

            // Assert
            readValue.Should().BeEquivalentTo(expectedValue);
        }

        [Test]
        public void GetImaRelTablePosition_ShouldReturnNull_WhenTagIsNotAvailableInFrame()
        {
            // Arrange
            var frame = new DataSetBuilder()
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetImaRelTablePosition(frame);

            // Assert
            readValue.Should().BeNull();
        }


        [Test]
        public void GetTriggerTime_ShouldReturnExpectedValue_WhenDataSetIsMrImage()
        {
            // Arrange
            var expectedValue = 12.3456789;
            var dataSet = new MrImageBuilder()
                .WithTriggerTime(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetTriggerTime(dataSet);

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [Test]
        public void GetTriggerTime_ShouldReturnExpectedValue_WhenDataSetIsEnhancedMrImage()
        {
            // Arrange
            var expectedValue = 12.3456789;
            var dataSet = new EnhancedMrImageBuilder()
                .WithTriggerTime(expectedValue)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetTriggerTime(dataSet);

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [Test]
        public void GetTriggerTime_ShouldReturnNaN_WhenTagIsNotAvailable()
        {
            // Arrange
            var frame = new MrImageBuilder()
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetTriggerTime(frame);

            // Assert
            double.IsNaN(readValue).Should().BeTrue();
        }


        [TestCase(12.3456789)]
        [TestCase(456)]
        public void GetTimeAfterStart_ShouldReturnExpectedValue_WhenDataSetIsMrImage(double expectedResult)
        {
            // Arrange
            var dataSet = new MrImageBuilder()
                .WithTimeAfterStart(expectedResult)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetTimeAfterStart(dataSet);

            // Assert
            readValue.Should().Be(expectedResult);
        }

        [TestCase(12.3456789)]
        [TestCase(456)]
        public void GetTimeAfterStart_ShouldReturnExpectedValue_WhenDataSetIsEnhancedMrImage(double expectedResult)
        {
            // Arrange
            var dataSet = new EnhancedMrImageBuilder()
                .WithTimeAfterStart(expectedResult)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetTimeAfterStart(dataSet);

            // Assert
            readValue.Should().Be(expectedResult);
        }

        [Test]
        public void GetTimeAfterStart_ShouldReturnNaN_WhenTagIsNotAvailable()
        {
            // Arrange
            var frame = new MrImageBuilder()
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetTimeAfterStart(frame);

            // Assert
            double.IsNaN(readValue).Should().BeTrue();
        }


        [TestCase("abc")]
        [TestCase("def")]
        public void GetScanningSequence_ShouldReturnExpectedValue_WhenDataSetIsMrImage(string expectedResult)
        {
            // Arrange
            var dataSet = new MrImageBuilder()
                .WithScanningSequence(expectedResult)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetScanningSequence(dataSet);

            // Assert
            readValue.Should().Be(expectedResult);
        }

        [TestCase("abc")]
        [TestCase("def")]
        public void GetScanningSequence_ShouldReturnExpectedValue_WhenDataSetIsEnhancedMrImage(string expectedResult)
        {
            // Arrange
            var dataSet = new EnhancedMrImageBuilder()
                .WithScanningSequence(expectedResult)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetScanningSequence(dataSet);

            // Assert
            readValue.Should().Be(expectedResult);
        }

        [Test]
        public void GetScanningSequence_ShouldReturnEmptyString_WhenTagIsNotAvailable()
        {
            // Arrange
            var frame = new MrImageBuilder()
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetScanningSequence(frame);

            // Assert
            readValue.Should().BeNullOrEmpty();
        }


        [TestCase("abc")]
        [TestCase("def")]
        public void GetSequenceVariant_ShouldReturnExpectedValue_WhenDataSetIsMrImage(string expectedResult)
        {
            // Arrange
            var dataSet = new MrImageBuilder()
                .WithSequenceVariant(expectedResult)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSequenceVariant(dataSet);

            // Assert
            readValue.Should().Be(expectedResult);
        }

        [TestCase("abc")]
        [TestCase("def")]
        public void GetSequenceVariant_ShouldReturnExpectedValue_WhenDataSetIsEnhancedMrImage(string expectedResult)
        {
            // Arrange
            var dataSet = new EnhancedMrImageBuilder()
                .WithSequenceVariant(expectedResult)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSequenceVariant(dataSet);

            // Assert
            readValue.Should().Be(expectedResult);
        }

        [Test]
        public void GetSequenceVariant_ShouldReturnEmptyString_WhenTagIsNotAvailable()
        {
            // Arrange
            var frame = new MrImageBuilder()
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetSequenceVariant(frame);

            // Assert
            readValue.Should().BeNullOrEmpty();
        }


        [TestCase("abc")]
        [TestCase("def")]
        public void GetGradientCoilName_ShouldReturnExpectedValue_WhenDataSetIsMrImage(string expectedResult)
        {
            // Arrange
            var dataSet = new MrImageBuilder()
                .WithGradientCoilName(expectedResult)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetGradientCoilName(dataSet);

            // Assert
            readValue.Should().Be(expectedResult);
        }

        [TestCase("abc")]
        [TestCase("def")]
        public void GetGradientCoilName_ShouldReturnExpectedValue_WhenDataSetIsEnhancedMrImage(string expectedResult)
        {
            // Arrange
            var dataSet = new EnhancedMrImageBuilder()
                .WithGradientCoilName(expectedResult)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetGradientCoilName(dataSet);

            // Assert
            readValue.Should().Be(expectedResult);
        }

        [Test]
        public void GetGradientCoilName_ShouldReturnEmptyString_WhenTagIsNotAvailable()
        {
            // Arrange
            var frame = new MrImageBuilder()
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetGradientCoilName(frame);

            // Assert
            readValue.Should().BeNullOrEmpty();
        }

        [Test]
        public void GetPresentationStateSopInstanceUid_ShouldReturnSopInstanceUid_WhenDataSetIsMrImage()
        {
            //Arrange
            var dataSet = new MrImageBuilder()
                .WithPresentationStateSopInstanceUid("foo")
                .ToDicomFrameDataSetMock()
                .Object;

            // Act
            var result = m_Target.GetPresentationStateSopInstanceUid(dataSet);

            //Assert
            result.Should().Be("foo");
        }

        [Test]
        public void GetPresentationStateSopInstanceUid_ShouldReturnSopInstanceUid_WhenDataSetIsEnhancedMrImage()
        {
            //Arrange
            var dataSet = new EnhancedMrImageBuilder()
                .WithPresentationStateSopInstanceUid("foo")
                .ToDicomFrameDataSetMock()
                .Object;

            // Act
            var result = m_Target.GetPresentationStateSopInstanceUid(dataSet);

            //Assert
            result.Should().Be("foo");
        }

        [Test]
        public void GetPresentationStateSopInstanceUidForFrame_ShouldReturnNull_WhenTagIsNotAvailable()
        {
            //Arrange
            var frame = new MrImageBuilder().ToDicomFrameDataSetMock().Object;

            // Act
            var result = m_Target.GetPresentationStateSopInstanceUid(frame);

            //Assert
            result.Should().BeNull();
        }


        [Test]
        public void GetImagePosition_ShouldReturnNull_WhenTagIsNotAvailable()
        {
            //Arrange
            var frame = new MrImageBuilder().ToDicomFrameDataSetMock().Object;

            // Act
            var result = m_Target.GetImagePosition(frame);

            //Assert
            result.Should().BeNull();
        }
        [Test]
        public void GetImagePosition_ShouldReturnExpectedValue_WhenTagIsAvailableAndImageIsMrImage()
        {
            // Arrange
            var expected = new Vector3D(1, 2, 3);
            var frame = new MrImageBuilder()
                .WithImagePlane(expected, new Vector3D(1, 0, 0), new Vector3D(0, 1, 0))
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetImagePosition(frame);

            // Assert
            readValue.IsAlmostEqual(expected).Should().BeTrue();
        }
        [Test]
        public void GetImagePosition_ShouldReturnExpectedValue_WhenTagIsAvailableAndImageIsEnhancedMrImage()
        {
            // Arrange
            var expected = new Vector3D(1, 2, 3);
            var frame = new EnhancedMrImageBuilder()
                .WithImagePlane(expected, new Vector3D(1, 0, 0), new Vector3D(0, 1, 0))
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetImagePosition(frame);

            // Assert
            readValue.IsAlmostEqual(expected).Should().BeTrue();
        }

        [Test]
        public void GetOrientationRow_ShouldReturnNull_WhenTagIsNotAvailable()
        {
            //Arrange
            var frame = new MrImageBuilder().ToDicomFrameDataSetMock().Object;

            // Act
            var result = m_Target.GetOrientationRow(frame);

            //Assert
            result.Should().BeNull();
        }
        [Test]
        public void GetOrientationRow_ShouldReturnExpectedValue_WhenTagIsAvailableAndImageIsMrImage()
        {
            // Arrange
            var expected = new Vector3D(0.2, 0.7, -0.1);
            var frame = new MrImageBuilder()
                .WithImagePlane(new Vector3D(1, 2, 3), expected, new Vector3D(0, 1, 0))
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetOrientationRow(frame);

            // Assert
            readValue.IsAlmostEqual(expected).Should().BeTrue();
        }
        [Test]
        public void GetOrientationRow_ShouldReturnExpectedValue_WhenTagIsAvailableAndImageIsEnhancedMrImage()
        {
            // Arrange
            var expected = new Vector3D(0.2, 0.7, -0.1);
            var frame = new EnhancedMrImageBuilder()
                .WithImagePlane(new Vector3D(1, 2, 3), expected, new Vector3D(0, 1, 0))
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetOrientationRow(frame);

            // Assert
            readValue.IsAlmostEqual(expected).Should().BeTrue();
        }

        [Test]
        public void GetOrientationCol_ShouldReturnNull_WhenTagIsNotAvailable()
        {
            //Arrange
            var frame = new MrImageBuilder().ToDicomFrameDataSetMock().Object;

            // Act
            var result = m_Target.GetOrientationCol(frame);

            //Assert
            result.Should().BeNull();
        }
        [Test]
        public void GetOrientationCol_ShouldReturnExpectedValue_WhenTagIsAvailableAndImageIsMrImage()
        {
            // Arrange
            var expected = new Vector3D(0.2, 0.7, -0.1);
            var frame = new MrImageBuilder()
                .WithImagePlane(new Vector3D(1, 2, 3), new Vector3D(1, 0, 0), expected)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetOrientationCol(frame);

            // Assert
            readValue.IsAlmostEqual(expected).Should().BeTrue();
        }
        [Test]
        public void GetOrientationCol_ShouldReturnExpectedValue_WhenTagIsAvailableAndImageIsEnhancedMrImage()
        {
            // Arrange
            var expected = new Vector3D(0.2, 0.7, -0.1);
            var frame = new EnhancedMrImageBuilder()
                .WithImagePlane(new Vector3D(1, 2, 3), new Vector3D(1, 0, 0), expected)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetOrientationCol(frame);

            // Assert
            readValue.IsAlmostEqual(expected).Should().BeTrue();
        }

        [Test]
        public void GetMatrixRows_ShouldReturnZero_WhenTagIsNotAvailable()
        {
            //Arrange
            var frame = new MrImageBuilder().ToDicomFrameDataSetMock().Object;

            // Act
            var result = m_Target.GetMatrixRows(frame);

            //Assert
            result.Should().Be(0);
        }
        [Test]
        public void GetMatrixRows_ShouldReturnExpectedValue_WhenTagIsAvailableAndImageIsMrImage()
        {
            // Arrange
            ushort expected = 42;
            var frame = new MrImageBuilder()
                .WithImageMatrix(expected, 123, 0.2, 0.3)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetMatrixRows(frame);

            // Assert
            readValue.Should().Be(expected);
        }
        [Test]
        public void GetMatrixRows_ShouldReturnExpectedValue_WhenTagIsAvailableAndImageIsEnhancedMrImage()
        {
            // Arrange
            ushort expected = 42;
            var frame = new EnhancedMrImageBuilder()
                .WithImageMatrix(expected, 123, 0.2, 0.3)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetMatrixRows(frame);

            // Assert
            readValue.Should().Be(expected);
        }

        [Test]
        public void GetMatrixCols_ShouldReturnZero_WhenTagIsNotAvailable()
        {
            //Arrange
            var frame = new MrImageBuilder().ToDicomFrameDataSetMock().Object;

            // Act
            var result = m_Target.GetMatrixCols(frame);

            //Assert
            result.Should().Be(0);
        }
        [Test]
        public void GetMatrixCols_ShouldReturnExpectedValue_WhenTagIsAvailableAndImageIsMrImage()
        {
            // Arrange
            ushort expected = 24;
            var frame = new MrImageBuilder()
                .WithImageMatrix(321, expected, 0.5, 0.4)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetMatrixCols(frame);

            // Assert
            readValue.Should().Be(expected);
        }
        [Test]
        public void GetMatrixCols_ShouldReturnExpectedValue_WhenTagIsAvailableAndImageIsEnhancedMrImage()
        {
            // Arrange
            ushort expected = 42;
            var frame = new EnhancedMrImageBuilder()
                .WithImageMatrix(321, expected, 0.5, 0.4)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetMatrixCols(frame);

            // Assert
            readValue.Should().Be(expected);
        }

        [Test]
        public void GetPixelSpacingRow_ShouldReturnNaN_WhenTagIsNotAvailable()
        {
            //Arrange
            var frame = new MrImageBuilder().ToDicomFrameDataSetMock().Object;

            // Act
            var result = m_Target.GetPixelSpacingRow(frame);

            //Assert
            double.IsNaN(result).Should().BeTrue();
        }
        [Test]
        public void GetPixelSpacingRow_ShouldReturnExpectedValue_WhenTagIsAvailableAndImageIsMrImage()
        {
            // Arrange
            double expected = 0.234;
            var frame = new MrImageBuilder()
                .WithImageMatrix(33, 44, expected, 2.3)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetPixelSpacingRow(frame);

            // Assert
            readValue.Should().Be(expected);
        }
        [Test]
        public void GetPixelSpacingRow_ShouldReturnExpectedValue_WhenTagIsAvailableInSharedSequenceWhileImageIsEnhancedMrImage()
        {
            // Arrange
            double expected = 0.234;
            var frame = new EnhancedMrImageBuilder()
                .WithImageMatrix(33, 44, expected, 2.3)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetPixelSpacingRow(frame);

            // Assert
            readValue.Should().Be(expected);
        }
        [Test]
        public void GetPixelSpacingRow_ShouldReturnExpectedValue_WhenTagIsAvailableInPerFrameSequenceWhileImageIsEnhancedMrImage()
        {
            // Arrange
            double expected = 4.32;
            var frame = new EnhancedMrImageBuilder()
                .UsingFrame(3)
                .WithImageMatrix(33, 44, expected, 2.3)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetPixelSpacingRow(frame);

            // Assert
            readValue.Should().Be(expected);
        }

        [Test]
        public void GetPixelSpacingCol_ShouldReturnNaN_WhenTagIsNotAvailable()
        {
            //Arrange
            var frame = new MrImageBuilder().ToDicomFrameDataSetMock().Object;

            // Act
            var result = m_Target.GetPixelSpacingCol(frame);

            //Assert
            double.IsNaN(result).Should().BeTrue();
        }
        [Test]
        public void GetPixelSpacingCol_ShouldReturnExpectedValue_WhenTagIsAvailableAndImageIsMrImage()
        {
            // Arrange
            double expected = 0.234;
            var frame = new MrImageBuilder()
                .WithImageMatrix(33, 44, 2.3, expected)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetPixelSpacingCol(frame);

            // Assert
            readValue.Should().Be(expected);
        }
        [Test]
        public void GetPixelSpacingCol_ShouldReturnExpectedValue_WhenTagIsAvailableInSharedSequenceWhileImageIsEnhancedMrImage()
        {
            // Arrange
            double expected = 0.234;
            var frame = new EnhancedMrImageBuilder()
                .WithImageMatrix(33, 44, 2.3, expected)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetPixelSpacingCol(frame);

            // Assert
            readValue.Should().Be(expected);
        }
        [Test]
        public void GetPixelSpacingCol_ShouldReturnExpectedValue_WhenTagIsAvailableInPerFrameSequenceWhileImageIsEnhancedMrImage()
        {
            // Arrange
            double expected = 4.32;
            var frame = new EnhancedMrImageBuilder()
                .UsingFrame(3)
                .WithImageMatrix(33, 44, 2.3, expected)
                .ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.GetPixelSpacingCol(frame);

            // Assert
            readValue.Should().Be(expected);
        }

        // todo: write tests for;
        //string GetBodyPartExamined(IDicomFrameDataSet dataSet);
        //string GetMrAcquisitionType(IDicomFrameDataSet dataSet);
        //int GetAcquisitionNumber(IDicomFrameDataSet dataSet);
        //string GetTransferSyntaxUid(IDicomFrameDataSet dataSet);
        //string GetLossyImageCompression(IDicomFrameDataSet dataSet);
    }
}

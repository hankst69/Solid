//----------------------------------------------------------------------------------
// <copyright file="FoDicomDataSetProviderTests.cs" company="Siemens Healthcare GmbH">
// Author: Steffen Hanke
//----------------------------------------------------------------------------------

using System;
using FluentAssertions;
using Moq;
using Solid.Dicom;
using NUnit.Framework;
using Solid.DicomAdapters.FoDicom;
using Solid.DicomAdapters.FoDicom.Impl;
using Solid.Infrastructure.StateMachine.Impl;

namespace Solid.DicomAdapters.FoDicom_uTest
{
    /// <summary>
    /// API:NO
    /// FoDicomDataSetAdapterTests
    /// </summary>
    public class FoDicomDataSetProviderTests
    {
        private IFoDicomDataSetProvider m_Target;

        [SetUp]
        public void SetUp()
        {
            m_Target = new FoDicomDataSetProvider();
        }

        [Test]
        public void Ctor_ShouldNotThrow()
        {
            // Arrange
            // Act
            Action action = () => new FoDicomDataSetProvider();

            // Assert
            action.Should().NotThrow<ArgumentNullException>();
        }

        [Test]
        public void Ctor_ShouldThrow_WhenTracerNull()
        {
            // Arrange
            // Act
            Action action = () => new FoDicomDataSetProvider(null);

            // Assert
            action.Should().Throw<ArgumentNullException>();//.WithMessage("...");
        }


    }
}

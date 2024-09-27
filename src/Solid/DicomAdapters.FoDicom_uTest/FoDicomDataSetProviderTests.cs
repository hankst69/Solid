//----------------------------------------------------------------------------------
// File: "FoDicomDataSetProviderTests.cs"
// Author: Steffen Hanke
// Date: 2020
//----------------------------------------------------------------------------------
using System;
using FluentAssertions;
using NUnit.Framework;
using Solid.DicomAdapters.FoDicom;
using Solid.DicomAdapters.FoDicom.Impl;

namespace Solid.DicomAdapters.FoDicom_uTest
{
    /// <summary>
    /// API:NO
    /// FoDicomDataSetAdapterTests
    /// </summary>
    public class FoDicomDataSetProviderTests
    {
        private IFoDicomDataSetProvider _target;

        [SetUp]
        public void SetUp()
        {
            _target = new FoDicomDataSetProvider();
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

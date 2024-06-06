//----------------------------------------------------------------------------------
// File: "DicomFrameDataSetTests.cs"
// Author: Steffen Hanke
// Date: 2020
//----------------------------------------------------------------------------------

using System;
using FluentAssertions;
using Moq;
using Solid.Dicom;
using Solid.Dicom.Impl;
using NUnit.Framework;

namespace Solid.Dicom_uTest
{
    internal class DicomFrameDataSetTests
    {
        private Mock<IDicomDataSet> m_DicomDataSetMock;
        private IDicomFrameDataSet m_Target;
        private const long c_Tag = 1;
        private const int c_Idx = 0;

        [SetUp]
        public void Setup()
        {
            m_DicomDataSetMock = new Mock<IDicomDataSet>();
            m_Target = new DicomFrameDataSet(m_DicomDataSetMock.Object, 1);
        }

        [Test]
        public void Ctor_ShouldThrow_WhenDicomDataSetIsNull()
        {
            // Arrange
            // Act
            Action action = () => new DicomFrameDataSet(null, 1);
            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Ctor_ShouldThrow_WhenFrameNumberIsLessThanOne()
        {
            // Arrange
            // Act
            Action action = () => new DicomFrameDataSet(new Mock<IDicomDataSet>().Object, 0);
            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void DataSetLocationUid_ShouldDelegateToDicomDataSet()
        {
            // Arrange
            // Act
            var value = m_Target.DataSetLocationUid;
            // Assert
            m_DicomDataSetMock.VerifyGet(x => x.DataSetLocationUid, Times.Once);
        }
        [Test]
        public void DataSetInstanceUid_ShouldDelegateToDicomDataSet()
        {
            // Arrange
            // Act
            var value = m_Target.DataSetSopInstanceUid;
            // Assert
            m_DicomDataSetMock.VerifyGet(x => x.DataSetSopInstanceUid, Times.Once);
        }
        [Test]
        public void IsEmpty_ShouldDelegateToDicomDataSet()
        {
            // Arrange
            // Act
            var value = m_Target.IsEmpty();
            // Assert
            m_DicomDataSetMock.Verify(x => x.IsEmpty(), Times.Once);
        }
        [Test]
        public void GetNumberOfElements_ShouldDelegateToDicomDataSet()
        {
            // Arrange
            // Act
            var value = m_Target.GetNumberOfElements();
            // Assert
            m_DicomDataSetMock.Verify(x => x.GetNumberOfElements(), Times.Once);
        }
        [Test]
        public void GetElements_ShouldDelegateToDicomDataSet()
        {
            // Arrange
            // Act
            var value = m_Target.GetElements();
            // Assert
            m_DicomDataSetMock.Verify(x => x.GetElements(), Times.Once);
        }
        [Test]
        public void Contains_ShouldDelegateToDicomDataSet()
        {
            // Arrange
            // Act
            var value = m_Target.Contains(c_Tag);
            // Assert
            m_DicomDataSetMock.Verify(x => x.Contains(c_Tag), Times.Once);
        }
        [Test]
        public void IsElementEmpty_ShouldDelegateToDicomDataSet()
        {
            // Arrange
            // Act
            var value = m_Target.IsElementEmpty(c_Tag);
            // Assert
            m_DicomDataSetMock.Verify(x => x.IsElementEmpty(c_Tag), Times.Once);
        }
        [Test]
        public void GetNumberOfValues_ShouldDelegateToDicomDataSet()
        {
            // Arrange
            // Act
            var value = m_Target.GetNumberOfValues(c_Tag);
            // Assert
            m_DicomDataSetMock.Verify(x => x.GetNumberOfValues(c_Tag), Times.Once);
        }
        [Test]
        public void ContainsValue_ShouldDelegateToDicomDataSet()
        {
            // Arrange
            // Act
            var value = m_Target.ContainsValue(c_Tag);
            // Assert
            m_DicomDataSetMock.Verify(x => x.ContainsValue(c_Tag), Times.Once);
        }
        [Test]
        public void ContainsValueAt_ShouldDelegateToDicomDataSet()
        {
            // Arrange
            // Act
            var value = m_Target.ContainsValueAt(c_Tag, c_Idx);
            // Assert
            m_DicomDataSetMock.Verify(x => x.ContainsValueAt(c_Tag, c_Idx), Times.Once);
        }
        [Test]
        public void IsValueEmpty_ShouldDelegateToDicomDataSet()
        {
            // Arrange
            // Act
            var value = m_Target.IsValueEmpty(c_Tag);
            // Assert
            m_DicomDataSetMock.Verify(x => x.IsValueEmpty(c_Tag), Times.Once);
        }
        [Test]
        public void IsValueEmptyAt_ShouldDelegateToDicomDataSet()
        {
            // Arrange
            // Act
            var value = m_Target.IsValueEmptyAt(c_Tag, c_Idx);
            // Assert
            m_DicomDataSetMock.Verify(x => x.IsValueEmptyAt(c_Tag, c_Idx), Times.Once);
        }
        [Test]
        public void GetValue_ShouldDelegateToDicomDataSet()
        {
            // Arrange
            // Act
            var value = m_Target.GetValue(c_Tag);
            // Assert
            m_DicomDataSetMock.Verify(x => x.GetValue(c_Tag), Times.Once);
        }
        [Test]
        public void GetValueAt_ShouldDelegateToDicomDataSet()
        {
            // Arrange
            // Act
            var value = m_Target.GetValueAt(c_Tag, c_Idx);
            // Assert
            m_DicomDataSetMock.Verify(x => x.GetValueAt(c_Tag, c_Idx), Times.Once);
        }
        [Test]
        public void GetValueAsByteStream_ShouldDelegateToDicomDataSet()
        {
            // Arrange
            // Act
            var value = m_Target.GetValueAsByteStream(c_Tag);
            // Assert
            m_DicomDataSetMock.Verify(x => x.GetValueAsByteStream(c_Tag), Times.Once);
        }
        [Test]
        public void GetItem_ShouldDelegateToDicomDataSet()
        {
            // Arrange
            // Act
            var value = m_Target.GetItem(c_Tag);
            // Assert
            m_DicomDataSetMock.Verify(x => x.GetItem(c_Tag), Times.Once);
        }
        [Test]
        public void GetItemAt_ShouldDelegateToDicomDataSet()
        {
            // Arrange
            // Act
            var value = m_Target.GetItemAt(c_Tag, c_Idx);
            // Assert
            m_DicomDataSetMock.Verify(x => x.GetItemAt(c_Tag, c_Idx), Times.Once);
        }
    }
}

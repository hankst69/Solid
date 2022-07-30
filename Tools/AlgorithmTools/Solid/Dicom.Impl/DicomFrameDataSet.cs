//----------------------------------------------------------------------------------
// <copyright file="DicomFrameDataSet.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------


using System.Collections.Generic;
using Solid.Infrastructure.Diagnostics;

namespace Solid.Dicom.Impl
{
    public class DicomFrameDataSet : IDicomFrameDataSet
    {
        private readonly IDicomDataSet m_DicomDataSet;

        public DicomFrameDataSet(IDicomDataSet dicomDataSet, int frameNumber)
        {
            ConsistencyCheck.EnsureArgument(dicomDataSet).IsNotNull();
            ConsistencyCheck.EnsureArgument(frameNumber).IsGreaterOrEqual(1);

            m_DicomDataSet = dicomDataSet;
            FrameNumber = frameNumber;
        }


        public int FrameNumber { get; }

        public string DataSetLocationUid => m_DicomDataSet.DataSetLocationUid;
        public string DataSetSopClassUid => m_DicomDataSet.DataSetSopClassUid;
        public string DataSetSopInstanceUid => m_DicomDataSet.DataSetSopInstanceUid;

        public bool IsEmpty()
        {
            return m_DicomDataSet.IsEmpty();
        }

        public int GetNumberOfElements()
        {
            return m_DicomDataSet.GetNumberOfElements();
        }

        public IEnumerable<long> GetElements()
        {
            return m_DicomDataSet.GetElements();
        }

        public bool Contains(long tag)
        {
            return m_DicomDataSet.Contains(tag);
        }

        public bool IsElementEmpty(long tag)
        {
            return m_DicomDataSet.IsElementEmpty(tag);
        }

        public int GetNumberOfValues(long tag)
        {
            return m_DicomDataSet.GetNumberOfValues(tag);
        }

        public bool ContainsValue(long tag)
        {
            return m_DicomDataSet.ContainsValue(tag);
        }

        public bool ContainsValueAt(long tag, int index)
        {
            return m_DicomDataSet.ContainsValueAt(tag, index);
        }

        public bool IsValueEmpty(long tag)
        {
            return m_DicomDataSet.IsValueEmpty(tag);
        }

        public bool IsValueEmptyAt(long tag, int index)
        {
            return m_DicomDataSet.IsValueEmptyAt(tag, index);
        }

        public object GetValue(long tag)
        {
            return m_DicomDataSet.GetValue(tag);
        }

        public object GetValueAt(long tag, int index)
        {
            return m_DicomDataSet.GetValueAt(tag, index);
        }

        public byte[] GetValueAsByteStream(long tag)
        {
            return m_DicomDataSet.GetValueAsByteStream(tag);
        }

        public IDicomDataSet GetItem(long tag)
        {
            return m_DicomDataSet.GetItem(tag);
        }

        public IDicomDataSet GetItemAt(long tag, int index)
        {
            return m_DicomDataSet.GetItemAt(tag, index);
        }
    }
}

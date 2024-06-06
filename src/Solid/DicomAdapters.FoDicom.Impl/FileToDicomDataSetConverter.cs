//----------------------------------------------------------------------------------
// <copyright file="FileToDicomDataSetConverter.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System.Collections.Generic;

using FellowOakDicom;
using Solid.Dicom;
using Solid.Infrastructure.Diagnostics;

namespace Solid.DicomAdapters.FoDicom.Impl
{
    /// <summary>
    /// API:NO
    /// FileToDicomDataSetConverter
    /// </summary>
    public class FileToDicomDataSetConverter : IDicomDataSet
    {
        private readonly FoDicomDatasetToDicomDataSetConverter m_DataSetConverter;

        public FileToDicomDataSetConverter(string dicomFileName)
        {
            ConsistencyCheck.EnsureArgument(dicomFileName).IsNotNullOrEmpty();

            DicomFile df = DicomFile.Open(dicomFileName);
            DicomDataset ds = df.Dataset;
            var locationUid = string.Concat("File~~", dicomFileName);
            m_DataSetConverter = new FoDicomDatasetToDicomDataSetConverter(ds, locationUid);
        }

        public string DataSetLocationUid => m_DataSetConverter.DataSetLocationUid;
        public string DataSetSopClassUid => m_DataSetConverter.DataSetSopClassUid;
        public string DataSetSopInstanceUid => m_DataSetConverter.DataSetSopInstanceUid;

        public bool IsEmpty()
        {
            return m_DataSetConverter.IsEmpty();
        }

        public int GetNumberOfElements()
        {
            return m_DataSetConverter.GetNumberOfElements();
        }

        public IEnumerable<long> GetElements()
        {
            return m_DataSetConverter.GetElements();
        }

        public bool Contains(long tag)
        {
            return m_DataSetConverter.Contains(tag);
        }

        public bool IsElementEmpty(long tag)
        {
            return m_DataSetConverter.IsElementEmpty(tag);
        }

        public int GetNumberOfValues(long tag)
        {
            return m_DataSetConverter.GetNumberOfValues(tag);
        }

        public bool ContainsValue(long tag)
        {
            return m_DataSetConverter.ContainsValue(tag);
        }

        public bool ContainsValueAt(long tag, int index)
        {
            return m_DataSetConverter.GetNumberOfValues(tag) > index;
        }

        public bool IsValueEmpty(long tag)
        {
            return m_DataSetConverter.IsValueEmpty(tag);
        }

        public bool IsValueEmptyAt(long tag, int index)
        {
            return m_DataSetConverter.IsValueEmptyAt(tag, 0);
        }

        public object GetValue(long tag)
        {
            return m_DataSetConverter.GetValue(tag);
        }

        public object GetValueAt(long tag, int index)
        {
            return m_DataSetConverter.GetValueAt(tag, index);
        }

        public byte[] GetValueAsByteStream(long tag)
        {
            return m_DataSetConverter.GetValueAsByteStream(tag);
        }

        public IDicomDataSet GetItem(long tag)
        {
            return m_DataSetConverter.GetItem(tag);
        }

        public IDicomDataSet GetItemAt(long tag, int index)
        {
            return m_DataSetConverter.GetItemAt(tag, index);
        }
    }
}

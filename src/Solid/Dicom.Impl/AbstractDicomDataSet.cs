//----------------------------------------------------------------------------------
// File: "AbstractDicomDataSet.cs"
// Author: Steffen Hanke
// Date: 2020
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Solid.Infrastructure.RuntimeTypeExtensions;

namespace Solid.Dicom.Impl
{
    /// <summary>
    /// API:NO
    /// AbstractDicomDataSet
    /// </summary>
    public abstract class AbstractDicomDataSet : IDicomDataSet
    {
        private readonly Lazy<string> m_SopClassUidLazy;
        private readonly Lazy<string> m_SopInstanceUidLazy;
        private string m_SopClassUid;
        private string m_SopInstanceUid;

        private readonly IDictionary<string, IDicomDataSet> m_SequenceDataSetAtCache =
            new /*Concurrent*/Dictionary<string, IDicomDataSet>();

        protected AbstractDicomDataSet()
        {
            m_SopClassUidLazy = new Lazy<string>(() => GetValue(DicomTags.Tag.SopClassUid).As<string>());
            m_SopInstanceUidLazy = new Lazy<string>(() => GetValue(DicomTags.Tag.SopInstanceUid).As<string>());
        }

        public string DataSetLocationUid { get; protected set; }

        public string DataSetSopClassUid
        {
            get => m_SopClassUid ?? m_SopClassUidLazy.Value;
            set => m_SopClassUid = value;
        }

        public string DataSetSopInstanceUid
        {
            get => m_SopInstanceUid ?? m_SopInstanceUidLazy.Value;
            set => m_SopInstanceUid = value;
        }


        public bool IsEmpty()
        {
            return GetNumberOfElements() < 1;
        }

        public abstract int GetNumberOfElements();

        public abstract IEnumerable<long> GetElements();

        public bool Contains(long tag)
        {
            return GetElements().Contains(tag);
        }

        public abstract bool IsElementEmpty(long tag);

        public abstract int GetNumberOfValues(long tag);

        public bool ContainsValue(long tag)
        {
            return GetNumberOfValues(tag) > 0;
        }

        public bool ContainsValueAt(long tag, int index)
        {
            return GetNumberOfValues(tag) > index;
        }

        public bool IsValueEmpty(long tag)
        {
            return IsValueEmptyAt(tag, 0);
        }

        public abstract bool IsValueEmptyAt(long tag, int index);

        public object GetValue(long tag)
        {
            return GetValueAt(tag, 0);
        }

        public abstract object GetValueAt(long tag, int index);

        public abstract byte[] GetValueAsByteStream(long tag);

        public IDicomDataSet GetItem(long tag)
        {
            return GetItemAt(tag, 0);
        }

        public IDicomDataSet GetItemAt(long tag, int index)
        {
            if (!ContainsValueAt(tag, index))
            {
                return null;
            }

            var key = string.Concat(tag, "_", index);
            if (m_SequenceDataSetAtCache.ContainsKey(key))
            {
                return m_SequenceDataSetAtCache[key];
            }

            var dataset = CreateDicomDataSetFromSequenceAt(tag, index);

            m_SequenceDataSetAtCache[key] = dataset;
            return dataset;
        }

        protected abstract IDicomDataSet CreateDicomDataSetFromSequenceAt(long tag, int index);
    }
}

//----------------------------------------------------------------------------------
// <copyright file="FoDicomDatasetToDicomDataSetConverter.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

using FellowOakDicom;
using Solid.Dicom;
using Solid.Dicom.Impl;
using Solid.Infrastructure.Diagnostics;

namespace Solid.DicomAdapters.FoDicom.Impl
{
    internal interface IParentDicomDataSetAccess
    {
        string DataSetLocationUid { get; }
        string DataSetSopClassUid { get; }
        string DataSetSopInstanceUid { get; }

        IDictionary<long, byte[]> ByteStreamCache { get; }
        IDictionary<long, DicomTag> DicomItemCache { get; }
    }
    
    /// <summary>
    /// API:NO
    /// FoDicomDatasetToDicomDataSetConverter
    /// </summary>
    internal class FoDicomDatasetToDicomDataSetConverter : 
        AbstractDicomDataSet, 
        IDicomDataSet, 
        IParentDicomDataSetAccess
    {
        private readonly DicomDataset _foDicomDataset;

        private readonly IDictionary<long, DicomTag> _dicomItemCache;
        private readonly IDictionary<long, byte[]> _byteStreamCache;
        private readonly IDictionary<long, int> _tagNumberOfValuesCache = new /*Concurrent*/Dictionary<long, int>();
        private readonly IDictionary<string, bool> _tagIsValueAtEmptyCache = new /*Concurrent*/Dictionary<string, bool>();

        IDictionary<long, DicomTag> IParentDicomDataSetAccess.DicomItemCache => _dicomItemCache;
        IDictionary<long, byte[]> IParentDicomDataSetAccess.ByteStreamCache => _byteStreamCache;

        /// <inheritdoc />
        internal FoDicomDatasetToDicomDataSetConverter(DicomDataset foDicomDataset, string locationUid)
        {
            ConsistencyCheck.EnsureArgument(foDicomDataset, nameof(foDicomDataset)).IsNotNull();
            ConsistencyCheck.EnsureArgument(locationUid, nameof(locationUid)).IsNotNull();

            _foDicomDataset = foDicomDataset;
            DataSetLocationUid = locationUid;

            _dicomItemCache = new /*Concurrent*/Dictionary<long, DicomTag>();
            _byteStreamCache = new /*Concurrent*/Dictionary<long, byte[]>();
        }

        internal FoDicomDatasetToDicomDataSetConverter(DicomDataset foDicomSubSequenceDataset, IParentDicomDataSetAccess parentDataSet)
        {
            ConsistencyCheck.EnsureArgument(foDicomSubSequenceDataset, nameof(foDicomSubSequenceDataset)).IsNotNull();
            ConsistencyCheck.EnsureArgument(parentDataSet, nameof(parentDataSet)).IsNotNull();

            _foDicomDataset = foDicomSubSequenceDataset;

            _dicomItemCache = parentDataSet.DicomItemCache;
            _byteStreamCache = parentDataSet.ByteStreamCache;

            DataSetLocationUid = parentDataSet.DataSetLocationUid;
            DataSetSopClassUid = parentDataSet.DataSetSopClassUid;
            DataSetSopInstanceUid = parentDataSet.DataSetSopInstanceUid;
        }

        private DicomTag SyngoTagToDicomTag(long tag)
        {
            if (_dicomItemCache.ContainsKey(tag))
            {
                return _dicomItemCache[tag];
            }

            var groupNr = DicomTagHandling.GetGroupNumber(tag);
            var elementNr = DicomTagHandling.GetElementNumber(tag);
            if (!DicomTagHandling.IsPrivate(tag))
            {
                var dt = new DicomTag((ushort)groupNr, (ushort)elementNr);
                _dicomItemCache[tag] = dt;
                return dt;
            }

            //https://gitter.im/fo-dicom/fo-dicom?at=57e2e485fa660dd95ffdb0b1
            var privateCreator = DicomTagHandling.GetPrivateCreatorCode(tag);

            var dicomTag = new DicomTag((ushort)groupNr, (ushort)elementNr, privateCreator);

            // fix VR types wrongly assigned by FoDicom
            if (DicomTagHandling.IsPrivateLocalTagOfVrTypeSequence(tag))
            {
                dicomTag.DictionaryEntry.ValueRepresentations[0] = DicomVR.SQ;
            }
            else if (dicomTag.DictionaryEntry.ValueRepresentations[0].Code == "SQ")
            {
                dicomTag.DictionaryEntry.ValueRepresentations[0] = DicomVR.LO;
            }

            _dicomItemCache[tag] = dicomTag;
            return dicomTag;
        }

        private long DicomItemToSyngoTag(DicomItem dicomItem)
        {
            if (!dicomItem.Tag.IsPrivate)
            {
                return (long)dicomItem.Tag.Group << 16 + dicomItem.Tag.Element;
            }

            return DicomTagHandling.MakeLocalTag(dicomItem.Tag.Group, dicomItem.Tag.Element, dicomItem.Tag.PrivateCreator.ToString());
        }


        public new bool IsEmpty()
        {
            return !(_foDicomDataset.Any());
        }

        public override int GetNumberOfElements()
        {
            return _foDicomDataset.Count();
        }

        public override IEnumerable<long> GetElements()
        {
            return _foDicomDataset.Select(DicomItemToSyngoTag);
        }

        public new bool Contains(long tag)
        {
            return _foDicomDataset.Contains(SyngoTagToDicomTag(tag));
        }

        public override bool IsElementEmpty(long tag)
        {
            return IsValueEmpty(tag);
        }

        public override int GetNumberOfValues(long tag)
        {
            if (_tagNumberOfValuesCache.ContainsKey(tag))
            {
                return _tagNumberOfValuesCache[tag];
            }

            var numOfValues = 0;
            var dt = SyngoTagToDicomTag(tag);
            if (!_foDicomDataset.Contains(dt))
            {
                // tag does not exist ->  throw specific exception here?
                _tagNumberOfValuesCache[tag] = numOfValues;
                return numOfValues;
            }

            numOfValues = _foDicomDataset.GetValueCount(dt);
            _tagNumberOfValuesCache[tag] = numOfValues;
            return numOfValues;
        }

        public override bool IsValueEmptyAt(long tag, int index)
        {
            if (GetNumberOfValues(tag) <= index)
            {
                return true;
            }

            var key = string.Concat(tag, "_", index);
            if (_tagIsValueAtEmptyCache.ContainsKey(key))
            {
                return _tagIsValueAtEmptyCache[key];
            }

            var hasAnyValue = false;
            var dt = SyngoTagToDicomTag(tag);
            if (_foDicomDataset.TryGetSequence(dt, out DicomSequence ds))
            {
                hasAnyValue = ds.Items[index] != null;
            }
            else
            {
                hasAnyValue = _foDicomDataset.TryGetValue(dt, index, out object de);
            }
            _tagIsValueAtEmptyCache[key] = !hasAnyValue;
            return !hasAnyValue;
        }

        public override object GetValueAt(long tag, int index)
        {
            if (!ContainsValueAt(tag, index))
            {
                // tag or value does not exist ->  throw specific exception here?
                return null;
            }

            var dt = SyngoTagToDicomTag(tag);
            //var val = m_FoDicomDataset.Get<object>(dt, index, null);
            var val = _foDicomDataset.GetValue<object>(dt, index);

            // todo: clarify still necessary?
            // (unfortunately) we have to undo FoDicom type conversion:
            var vr = dt.DictionaryEntry.ValueRepresentations[0];
            var stringtypeName = typeof(string).Name;
            if (vr.ValueType.Name == stringtypeName &&
                val.GetType().Name != stringtypeName)
            {
                var str = val.ToString();
                if (str.Contains("[") && str.EndsWith("]"))
                {
                    int first = str.IndexOf("[", StringComparison.Ordinal);
                    return str.Substring(first + 1, str.Length - first - 2);
                }
                return val.ToString();
            }
            return val;
        }

        public override byte[] GetValueAsByteStream(long tag)
        {
            if (!_byteStreamCache.ContainsKey(tag))
            {
                // https://github.com/fo-dicom/fo-dicom/wiki/Getting-data
                try
                {
                    var di = SyngoTagToDicomTag(tag);
                    //if (!m_FoDicomDataset.Contains(di))
                    //{
                    //    // tag does not exist ->  throw specific exception here?
                    //    return null;
                    //}
                    var val = _foDicomDataset.GetValues<byte>(di);
                    _byteStreamCache[tag] = val;
                }
                catch (DicomDataException)
                {
                    // there is no value for desired index within tag -> just rethrow exception here?
                    return null;
                }
            }
            return _byteStreamCache[tag];
        }

        protected override IDicomDataSet CreateDicomDataSetFromSequenceAt(long tag, int index)
        {
            // https://stackoverflow.com/questions/46690392/how-to-read-nested-child-dicom-tags-from-sequences-using-fo-dicom
            var dt = SyngoTagToDicomTag(tag);
            var seq = _foDicomDataset.GetSequence(dt);

            var dataset = seq?.Items.Count > index
                ? new FoDicomDatasetToDicomDataSetConverter(seq.Items[index], this)
                : null;

            return dataset;
        }
    }
}

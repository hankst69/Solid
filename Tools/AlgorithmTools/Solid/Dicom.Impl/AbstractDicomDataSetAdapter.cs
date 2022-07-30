//----------------------------------------------------------------------------------
// <copyright file="AbstractDicomDataSetAdapter.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.Diagnostics.Impl;

namespace Solid.Dicom.Impl
{
    /// <summary>
    /// API:NO
    /// AbstractDicomDataSetAdapter
    /// </summary>
    public abstract class AbstractDicomDataSetAdapter : IDicomDataSetAdapter
    {
        private readonly IDictionary<string, IDicomDataSet> m_DataSetCache;
        protected readonly ITracer m_Tracer;

        protected AbstractDicomDataSetAdapter() : this(new NullTracer())
        {
        }

        protected AbstractDicomDataSetAdapter(ITracer tracer)
        {
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            m_Tracer = tracer;

            m_DataSetCache = new ConcurrentDictionary<string, IDicomDataSet>();
        }

        public IDicomDataSet GetFromCacheOrCreateNew(Func<string> getCacheKey, Func<IDicomDataSet> createDataSet)
        {
            using (m_Tracer.CreateScopeTracer())
            {
                ConsistencyCheck.EnsureArgument(getCacheKey).IsNotNull();
                ConsistencyCheck.EnsureArgument(createDataSet).IsNotNull();

                // fetch from cache if possible
                var cacheKeyBase = getCacheKey();
                var key = CalculateDataSetCacheKey(cacheKeyBase);
                if (m_DataSetCache.ContainsKey(key))
                {
                    return m_DataSetCache[key];
                }

                // create and add to cache
                var dataSet = createDataSet();
                m_DataSetCache[key] = dataSet;

                return dataSet;
            }
        }

        public IDicomFrameDataSet CreateFrameDataSet(IDicomDataSet dicomDataSet, int frameNumber)
        {
            //using (m_Tracer.CreateScopeTracer())
            {
                ConsistencyCheck.EnsureArgument(dicomDataSet).IsNotNull();

                return new DicomFrameDataSet(dicomDataSet, frameNumber);
            }
        }

        public IEnumerable<IDicomFrameDataSet> CreateFrameDataSets(IDicomDataSet dicomDataSet)
        {
            using (m_Tracer.CreateScopeTracer())
            {
                ConsistencyCheck.EnsureArgument(dicomDataSet).IsNotNull();

                var numberOfFrames = dicomDataSet.GetValue(DicomTags.Tag.NumberOfFrames);
                var frameCount = numberOfFrames == null ? 1 : DicomValues.ConvertDicomIsToInt(numberOfFrames);

                var framedataSets = new List<IDicomFrameDataSet>();
                for (var frameNo = 1; frameNo <= frameCount; frameNo++)
                {
                    framedataSets.Add(CreateFrameDataSet(dicomDataSet, frameNo));
                }
                return framedataSets;
            }
        }

        public IEnumerable<IDicomFrameDataSet> CreateFrameDataSets(IEnumerable<IDicomDataSet> dicomDataSets)
        {
            using (m_Tracer.CreateScopeTracer())
            {
                ConsistencyCheck.EnsureArgument(dicomDataSets).IsNotNull();
                return dicomDataSets.SelectMany(CreateFrameDataSets);
            }
        }

        public void ClearCache()
        {
            using (m_Tracer.CreateScopeTracer())
            {
                //var dataSets = m_DataSetCache.Values.ToList();
                //foreach (var ds in dataSets)
                //{
                //    if (ds == null) continue;
                //    ds.ClearCache();
                //    ds.Dispose();
                //}
                m_DataSetCache.Clear();
            }
        }

        private string CalculateDataSetCacheKey(string keyPredicate)
        {
            ConsistencyCheck.EnsureArgument(keyPredicate).IsNotNullOrEmpty();
            return keyPredicate.ToLower();
        }
    }
}

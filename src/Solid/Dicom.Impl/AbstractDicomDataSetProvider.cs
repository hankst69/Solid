//----------------------------------------------------------------------------------
// File: "AbstractDicomDataSetProvider.cs"
// Author: Steffen Hanke
// Date: 2020-2022
//----------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Solid.Infrastructure.Diagnostics;

namespace Solid.Dicom.Impl
{
    /// <summary>
    /// API:NO
    /// AbstractDicomDataSetAdapter
    /// </summary>
    public abstract class AbstractDicomDataSetProvider : IDicomDataSetProvider
    {
        protected readonly ITracer _tracer;
        private readonly IDictionary<string, IDicomDataSet> _dataSetCache = new ConcurrentDictionary<string, IDicomDataSet>();

        protected AbstractDicomDataSetProvider()
        {
            _tracer = null;
        }

        protected AbstractDicomDataSetProvider(ITracer tracer)
        {
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            _tracer = tracer;
        }

        public IDicomDataSet GetFromCacheOrCreateNew(Func<string> getCacheKey, Func<IDicomDataSet> createDataSet)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(getCacheKey).IsNotNull();
            ConsistencyCheck.EnsureArgument(createDataSet).IsNotNull();

            // fetch from cache if possible
            var cacheKeyBase = getCacheKey();
            var key = CalculateDataSetCacheKey(cacheKeyBase);
            if (_dataSetCache.ContainsKey(key))
            {
                return _dataSetCache[key];
            }

            // create and add to cache
            var dataSet = createDataSet();
            _dataSetCache[key] = dataSet;

            return dataSet;
        }
        private string CalculateDataSetCacheKey(string keyPredicate)
        {
            ConsistencyCheck.EnsureArgument(keyPredicate).IsNotNullOrEmpty();
            return keyPredicate.ToLower();
        }

        public void ClearCache()
        {
            using var tracer = _tracer?.CreateScopeTracer();
            //var dataSets = _dataSetCache.Values.ToList();
            //foreach (var ds in dataSets)
            //{
            //    if (ds == null) continue;
            //    ds.ClearCache();
            //    ds.Dispose();
            //}
            _dataSetCache.Clear();
        }

        public IDicomDataSet GetDataSetFromFile(string dicomFileName) => throw new NotImplementedException();
        public IDicomDataSet GetDataSetFromInstance(object dicomDataSet) => throw new NotImplementedException();
        public IEnumerable<IDicomDataSet> GetDataSetsFromFileOrDirectory(string fileOrDirectoryName, bool recurseIntoSubDirectories = true) => throw new NotImplementedException();
    }
}

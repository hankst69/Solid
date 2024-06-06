//----------------------------------------------------------------------------------
// <copyright file="FoDicomDataSetProvider.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

using FellowOakDicom;
using FellowOakDicom.IO;

using Solid.Dicom;
using Solid.Dicom.Impl;
using Solid.Infrastructure.Diagnostics;

namespace Solid.DicomAdapters.FoDicom.Impl
{
    /// <summary>
    /// API:NO
    /// FoDicomDataSetAdapter
    /// </summary>
    public class FoDicomDataSetProvider : AbstractDicomDataSetProvider, IFoDicomDataSetProvider
    {
        public FoDicomDataSetProvider() 
            : base()
        {
        }

        public FoDicomDataSetProvider(ITracer tracer)
            : base(tracer)
        {
        }

        public IDicomDataSet GetDataSetFromFoDicomInstance(FellowOakDicom.DicomDataset foDicomDataSet)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(foDicomDataSet).IsNotNull();

            var getLocationString = new Func<string>(() => $"FoDicomDataset~~{foDicomDataSet.GetHashCode()}");

            return GetFromCacheOrCreateNew(
                getLocationString,
                () =>
                {
                    try
                    {
                        return new FoDicomDatasetToDicomDataSetConverter(foDicomDataSet, getLocationString());
                    }
                    catch (DicomFileException)
                    {
                        return null;
                    }
                });
        }

        public new IDicomDataSet GetDataSetFromInstance(object foDicomDataSet) => GetDataSetFromFoDicomInstance((FellowOakDicom.DicomDataset)foDicomDataSet);

        public new IDicomDataSet GetDataSetFromFile(string dicomFileName)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(dicomFileName).IsNotNullOrEmpty();

            return GetFromCacheOrCreateNew(
                () => dicomFileName,
                () =>
                {
                    try
                    {
                        return new FileToDicomDataSetConverter(dicomFileName);
                    }
                    catch (DicomFileException)
                    {
                        return null;
                    }
                });
        }

        public new IEnumerable<IDicomDataSet> GetDataSetsFromFileOrDirectory(string fileOrDirectoryName, bool recurseIntoSubDirectories)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(fileOrDirectoryName).IsNotNullOrEmpty();

            var dicomDataSets = new List<IDicomDataSet>();

            if (System.IO.File.Exists(fileOrDirectoryName))
            {
                dicomDataSets.Add(GetDataSetFromFile(fileOrDirectoryName));
            }
            else if (System.IO.Directory.Exists(fileOrDirectoryName))
            {
                CreateDataSetsForFilesInDirectory(dicomDataSets, fileOrDirectoryName, recurseIntoSubDirectories);
            }
            return dicomDataSets.Where(x => x != null);
        }

        private void CreateDataSetsForFilesInDirectory(IList<IDicomDataSet> dataSetList, string directoryName, bool recurseSubDirectories)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(directoryName).IsNotNullOrEmpty();
            tracer?.Info($"reading files from directory '{directoryName}'");

            IDirectoryReference dref = new DirectoryReference(directoryName);
            if (!dref.Exists)
            {
                return;
            }
            foreach (var fileName in dref.EnumerateFileNames("*"))
            {
                dataSetList.Add(GetDataSetFromFile(fileName));
            }
            if (!recurseSubDirectories)
            {
                return;
            }
            foreach (var subDirectoryName in dref.EnumerateDirectoryNames())
            {
                CreateDataSetsForFilesInDirectory(dataSetList, subDirectoryName, recurseSubDirectories: true);
            }
        }
    }
}

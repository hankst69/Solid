//----------------------------------------------------------------------------------
// <copyright file="IDicomDataSetAdapter.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Solid.Dicom
{
    /// <summary>
    /// API:YES
    /// IDicomDataSetAdapter
    /// </summary>
    public interface IDicomDataSetAdapter
    {
        IDicomFrameDataSet CreateFrameDataSet(IDicomDataSet dataSet, int frameNumber);
        IEnumerable<IDicomFrameDataSet> CreateFrameDataSets(IDicomDataSet dicomDataSet);
        IEnumerable<IDicomFrameDataSet> CreateFrameDataSets(IEnumerable<IDicomDataSet> dicomDataSets);

        void ClearCache();
    }
}

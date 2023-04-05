//----------------------------------------------------------------------------------
// File: "IDicomFrameDataSetCreator.cs"
// Author: Steffen Hanke
// Date: 2022
//----------------------------------------------------------------------------------
using System.Collections.Generic;

namespace Solid.Dicom
{
    /// <summary>
    /// API:YES
    /// IDicomFrameDataSetCreator
    /// </summary>
    public interface IDicomFrameDataSetCreator
    {
        IDicomFrameDataSet CreateFrameDataSet(IDicomDataSet dataSet, int frameNumber);
        IEnumerable<IDicomFrameDataSet> CreateFrameDataSets(IDicomDataSet dicomDataSet);
        IEnumerable<IDicomFrameDataSet> CreateFrameDataSets(IEnumerable<IDicomDataSet> dicomDataSets);
    }
}

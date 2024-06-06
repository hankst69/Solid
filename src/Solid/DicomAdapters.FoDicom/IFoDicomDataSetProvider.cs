//----------------------------------------------------------------------------------
// File: "IFoDicomDataSetProvider.cs"
// Author: Steffen Hanke
// Date: 2020-2022
//----------------------------------------------------------------------------------

using Solid.Dicom;

namespace Solid.DicomAdapters.FoDicom
{
    /// <inheritdoc />
    /// <summary>
    /// API:YES
    /// IFoDicomDataSetAdapter
    /// Adapter to create DicomDataSets out of files or directories or from an FoDicom DataSet
    /// </summary>
    public interface IFoDicomDataSetProvider : IDicomDataSetProvider
    {
        /// <summary>
        /// Provides an instance of type Solid.Dicom.IDicomDataSet out of a Fo-Dicom.DicomDataSet instance
        /// </summary>
        /// <param name="foDicomDataSet">the Fo-Dicom dataset</param>
        /// <returns>an IDicomDataSet</returns>
        IDicomDataSet GetDataSetFromFoDicomInstance(FellowOakDicom.DicomDataset foDicomDataSet);
    }
}

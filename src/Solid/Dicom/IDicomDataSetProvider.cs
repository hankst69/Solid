//----------------------------------------------------------------------------------
// File: "IDicomDataSetProvider.cs"
// Author: Steffen Hanke
// Date: 2022
//----------------------------------------------------------------------------------
using System.Collections.Generic;

namespace Solid.Dicom
{
    /// <summary>
    /// API:YES
    /// IDicomDataSet
    /// </summary>
    public interface IDicomDataSetProvider
    {
        /// <summary>
        /// Provides a Solid.Dicom.IDicomDataSet instance from a filename representing a dicom file on filesystem
        /// </summary>
        /// <param name="dicomFileName">the filename of the dicom file</param>
        /// <returns>an IDicomDataSet</returns>
        IDicomDataSet GetDataSetFromFile(string dicomFileName);

        /// <summary>
        /// Provides a Solid.Dicom.IDicomDataSet instance from a external dataset instance in memory
        /// There needs to be a specific Provider implementation for each specific dataset implementation
        /// (e.g. SyngoDicomDataSetProvider would interprete the dicomDataSet to be a syngo IDataset)
        /// </summary>
        /// <param name="dicomDataSet">the filename of the dicom file</param>
        /// <returns>an IDicomDataSet</returns>
        IDicomDataSet GetDataSetFromInstance(object dicomDataSet);

        /// <summary>
        /// Provides one or more instances of type Solid.Dicom.IDicomDataSet out of filesystem name representing a single dicom file or a directory containing dicom files
        /// </summary>
        /// <param name="fileOrDirectoryName">name of the file or directory representing the dicom file(s)</param>
        /// <param name="recurseIntoSubDirectories">enables or disables recursion into subdirectories (in case the first param was a directory)</param>
        /// <returns>an IDicomDataSet</returns>
        IEnumerable<IDicomDataSet> GetDataSetsFromFileOrDirectory(string fileOrDirectoryName, bool recurseIntoSubDirectories = true);

        void ClearCache();
    }
}

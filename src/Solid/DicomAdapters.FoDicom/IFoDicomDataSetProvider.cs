//----------------------------------------------------------------------------------
// <copyright file="IFoDicomDataSetProvider.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
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
        /// Provides an instance of type MrFoundation.Dicom.IDicomDataSet out of a Fo-Dicom.DicomDataSet instance
        /// </summary>
        /// <param name="foDicomDataSet">the Fo-Dicom dataset</param>
        /// <returns>an IDicomDataSet</returns>
        IDicomDataSet GetDataSetFromFoDicomInstance(FellowOakDicom.DicomDataset foDicomDataSet);
    }
}

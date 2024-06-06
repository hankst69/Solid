//----------------------------------------------------------------------------------
// File: "IDicomFrameDataSet.cs"
// Author: Steffen Hanke
// Date: 2022
//----------------------------------------------------------------------------------

namespace Solid.Dicom
{
    /// <summary>
    /// API:YES
    /// IDicomFrameDataSet
    /// </summary>
    public interface IDicomFrameDataSet : IDicomDataSet
    {
        /// <summary>API:YES
        /// The number of the frame (in case this DataSet represents a specific frame out of an EnhancedMrMultiframeImage)
        /// </summary>
        int FrameNumber { get; }
    }
}

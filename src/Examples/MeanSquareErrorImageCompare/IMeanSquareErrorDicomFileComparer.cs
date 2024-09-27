//----------------------------------------------------------------------------------
// File: "IMeanSquareErrorDicomFileComparer.cs"
// Author: Steffen Hanke
// Date: 2020-2024
//----------------------------------------------------------------------------------
using System.Collections.Generic;

namespace MeanSquareErrorImageCompare
{
    public interface IMeanSquareErrorDicomFileComparer
    {
        IEnumerable<string> CompareDicomFiles(string dicomFileName1, string dicomFileName2, bool printFileNames = false, bool verboseMode = false);
    }
}

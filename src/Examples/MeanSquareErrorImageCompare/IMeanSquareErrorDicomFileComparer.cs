//----------------------------------------------------------------------------------
// 2020-2024
// Author: Steffen Hanke
//----------------------------------------------------------------------------------
using System.Collections.Generic;

namespace MeanSquareErrorImageCompare
{
    public interface IMeanSquareErrorDicomFileComparer
    {
        IEnumerable<string> CompareDicomFiles(string dicomFileName1, string dicomFileName2, bool silent = true);
    }
}

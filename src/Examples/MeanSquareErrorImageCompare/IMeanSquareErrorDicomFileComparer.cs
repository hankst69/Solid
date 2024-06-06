//----------------------------------------------------------------------------------
// <copyright file="ICrossImageMeanSquareErrorExecutor.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Examples.MeanSquareErrorImageCompare
{
    public interface IMeanSquareErrorDicomFileComparer
    {
        IEnumerable<string> CompareDicomFiles(string dicomFileName1, string dicomFileName2, bool silent = true);
    }
}

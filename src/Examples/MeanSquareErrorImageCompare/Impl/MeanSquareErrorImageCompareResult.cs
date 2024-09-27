//----------------------------------------------------------------------------------
// File: "MeanSquareErrorImageCompareResult.cs"
// Author: Steffen Hanke
// Date: 2020-2024
//----------------------------------------------------------------------------------
using System.Collections.Generic;
using Solid.Infrastructure.Diagnostics;

namespace MeanSquareErrorImageCompare.Impl
{
    /// <inheritdoc />
    internal class MeanSquareErrorImageCompareResult : IMeanSquareErrorImageCompareResult
    {
        internal MeanSquareErrorImageCompareResult(double mse, IEnumerable<InputValidationFinding> errors, IEnumerable<InputValidationFinding> warnings)
        {
            ConsistencyCheck.EnsureArgument(errors).IsNotNull();
            ConsistencyCheck.EnsureArgument(warnings).IsNotNull();

            MeanSquareError = mse;
            Errors = errors;
            Warnings = warnings;
        }

        public double MeanSquareError { get; }
        public IEnumerable<InputValidationFinding> Errors { get; }
        public IEnumerable<InputValidationFinding> Warnings { get; }
    }
}

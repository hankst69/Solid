//----------------------------------------------------------------------------------
// File: "MeanSquareErrorImageComparer.cs"
// Author: Steffen Hanke
// Date: 2020-2024
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Solid.Dicom.ImageData;
using Solid.Infrastructure.Diagnostics;

namespace MeanSquareErrorImageCompare.Impl
{
    /// <inheritdoc />
    public class MeanSquareErrorImageComparer : IMeanSquareErrorImageComparer
    {
        private readonly ITracer _tracer;

        public MeanSquareErrorImageComparer(ITracer tracer)
        {
            using var trace = tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            _tracer = tracer;
        }

        /// <inheritdoc />
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            using var tracer = _tracer.CreateScopeTracer();
            // since this algorithm does not rely on any underlying unmanaged cpp implementation there is nothing to be done here
        }

        /// <inheritdoc />
        public IMeanSquareErrorImageCompareResult CompareImages(IImageData image1, IImageData image2)
        {
            using var tracer = _tracer.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(image1).IsNotNull();
            ConsistencyCheck.EnsureArgument(image2).IsNotNull();

            double mse = double.NaN;
            var errors = new List<InputValidationFinding>();
            var warnings = new List<InputValidationFinding>();

            const double tolerance = 0.00001;

            // validate mandatory input data prerequisites:
            image1.LoadPixelData();
            image2.LoadPixelData();
            if (image1.PixelData == null || image2.PixelData == null)
            {
                errors.Add(InputValidationFinding.PixelDataMissing);
            }
            if (image1.ImagePixelInfo.PixelRows != image2.ImagePixelInfo.PixelRows
                || image1.ImagePixelInfo.PixelColumns != image2.ImagePixelInfo.PixelColumns)
            {
                errors.Add(InputValidationFinding.DifferInMatrixSize);
            }
            if (Math.Abs(image1.ImagePixelInfo.PixelSizeInRowDir - image2.ImagePixelInfo.PixelSizeInRowDir) > tolerance
                || Math.Abs(image1.ImagePixelInfo.PixelSizeInColDir - image2.ImagePixelInfo.PixelSizeInColDir) > tolerance)
            {
                errors.Add(InputValidationFinding.DifferInPixelSpacing);
            }
            if (errors.Any())
            {
                return new MeanSquareErrorImageCompareResult(mse, errors, warnings);
            }

            // validate optional input data prerequisites:
            if (Math.Abs(image1.ImagePixelInfo.SliceThickness - image2.ImagePixelInfo.SliceThickness) > tolerance)
            {
                warnings.Add(InputValidationFinding.DifferInSliceThickness);
            }
            if (!image1.ImagePlaneInfo.ContainsImagePlane || !image2.ImagePlaneInfo.ContainsImagePlane)
            {
                warnings.Add(InputValidationFinding.ImagePlaneInfoMissing);
            }
            else
            {
                if (!image1.ImagePlaneInfo.Position.IsAlmostEqual(image2.ImagePlaneInfo.Position))
                {
                    warnings.Add(InputValidationFinding.DifferInImagePosition);
                }
                if (!image1.ImagePlaneInfo.OrientationRow.IsAlmostEqual(image2.ImagePlaneInfo.OrientationRow)
                    || !image1.ImagePlaneInfo.OrientationCol.IsAlmostEqual(image2.ImagePlaneInfo.OrientationCol))
                {
                    warnings.Add(InputValidationFinding.DifferInImageOrientation);
                }
            }
            if (image1.ImagePixelInfo.PixelDataIsLossyCompressed != image2.ImagePixelInfo.PixelDataIsLossyCompressed
                || image1.ImagePixelInfo.LossyImageCompression != image2.ImagePixelInfo.LossyImageCompression)
            {
                warnings.Add(InputValidationFinding.DifferInCompressionType);
            }
            if (image1.ImageDistortionInfo.ConsolidatedDistortionCorrectionType !=
                image2.ImageDistortionInfo.ConsolidatedDistortionCorrectionType)
            {
                warnings.Add(InputValidationFinding.DifferInDistortionCorrectionType);
            }

            // the real MeanSquareError algorithm comes here:
            var rows = image1.ImagePixelInfo.PixelRows;
            var cols = image1.ImagePixelInfo.PixelColumns;
            var pixels1 = image1.GetPixelsAsUshortGreyValues();
            var pixels2 = image2.GetPixelsAsUshortGreyValues();
            var isSigned1 = image1.ImagePixelInfo.PixelDataIsSigned;
            var isSigned2 = image2.ImagePixelInfo.PixelDataIsSigned;

            double sum = 0.0;
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    var idx = row * cols + col;

                    long pixel1 = isSigned1 ? (short)pixels1[idx] : pixels1[idx];
                    long pixel2 = isSigned2 ? (short)pixels2[idx] : pixels2[idx];

                    sum += (pixel1 - pixel2) * (pixel1 - pixel2);
                }
            }
            mse = sum / (rows * cols);

            return new MeanSquareErrorImageCompareResult(mse, errors, warnings);
        }
    }
}
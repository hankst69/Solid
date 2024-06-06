//----------------------------------------------------------------------------------
// <copyright file="MeanSquareErrorDicomFileComparer.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020-2024. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System.Linq;
using System.Collections.Generic;

using Solid.Dicom.ImageData;
using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.RuntimeTypeExtensions;
using Solid.Dicom;

namespace Examples.MeanSquareErrorImageCompare.Impl
{
    public class MeanSquareErrorDicomFileComparer : IMeanSquareErrorDicomFileComparer
    {
        private readonly ITracer _tracer;
        private readonly IMeanSquareErrorImageComparer _crossImageMeanSquareError;
        private readonly IDicomDataSetProvider _dicomDataSetProvider;
        private readonly IDicomFrameDataSetCreator _frameDataSetCreator;
        private readonly IImageDataCreator _imageDataCreator;

        public MeanSquareErrorDicomFileComparer(
            ITracer tracer,
            IMeanSquareErrorImageComparer crossImageMeanSquareError,
            IDicomDataSetProvider dicomDataSetProvider,
            IDicomFrameDataSetCreator frameDataSetCreator,
            IImageDataCreator imageDataCreator)
        {
            using var trace = tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            ConsistencyCheck.EnsureArgument(crossImageMeanSquareError).IsNotNull();
            ConsistencyCheck.EnsureArgument(dicomDataSetProvider).IsNotNull();
            ConsistencyCheck.EnsureArgument(frameDataSetCreator).IsNotNull();
            ConsistencyCheck.EnsureArgument(imageDataCreator).IsNotNull();

            _tracer = tracer;
            _crossImageMeanSquareError = crossImageMeanSquareError;
            _dicomDataSetProvider = dicomDataSetProvider;
            _frameDataSetCreator = frameDataSetCreator;
            _imageDataCreator = imageDataCreator;
        }

        public IEnumerable<string> CompareDicomFiles(string dicomFileName1, string dicomFileName2, bool silentMode)
        {
            using var tracer = _tracer.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(dicomFileName1).IsNotNullOrEmpty();
            ConsistencyCheck.EnsureArgument(dicomFileName2).IsNotNullOrEmpty();
            tracer.Info($"running the algorithm with dicom data from file1 {dicomFileName1} and file2 {dicomFileName2}");

            var dataSets1 = _dicomDataSetProvider.GetDataSetsFromFileOrDirectory(dicomFileName1).ToIList();
            var frameDataSets1 = _frameDataSetCreator.CreateFrameDataSets(dataSets1);
            var imageDatas1 = frameDataSets1.Select(fs => _imageDataCreator.CreateImageData(fs)).ToIList();

            var dataSets2 = _dicomDataSetProvider.GetDataSetsFromFileOrDirectory(dicomFileName2).ToIList();
            var frameDataSets2 = _frameDataSetCreator.CreateFrameDataSets(dataSets2);
            var imageDatas2 = frameDataSets2.Select(fs => _imageDataCreator.CreateImageData(fs)).ToIList();

            // some high level input validation
            if (dataSets1.Count < 1)
            {
                yield return $"The file or directory {dicomFileName1} could not be found";
                yield break;
            }
            if (dataSets2.Count < 1)
            {
                yield return $"The file or directory {dicomFileName2} could not be found";
                yield break;
            }

            if (imageDatas1.Count == 1 && imageDatas2.Count == 1)
            {
                foreach (var s in Compare2Images(imageDatas1.FirstOrDefault(), imageDatas2.FirstOrDefault(), silentMode))
                {
                    yield return s;
                }
                yield break;
            }

            if (imageDatas1.Count != imageDatas2.Count)
            {
                yield return "The images or frames to compare differ in number!";
                yield return "Comparing only the first image of both sources!";
                foreach (var s in Compare2Images(imageDatas1.FirstOrDefault(), imageDatas2.FirstOrDefault(), silentMode))
                {
                    yield return s;
                }
                yield break;
            }

            yield return "Comparing all frames from all dicom files given";
            // ?maybe we should reorder the images before comparison based on their SopInstanceUids and FrameNumbers?

            yield return "'MeanSquareError', 'Left SopInstUid_FrameNr', 'Right SopInstUid_FrameNr', 'Errors_Warnings'";
            for (var idx = 0; idx < imageDatas1.Count; idx++)
            {
                var image1 = imageDatas1[idx];
                var image2 = imageDatas2[idx];

                var result = _crossImageMeanSquareError.CompareImages(image1, image2);
                var errors_warnings = $"{string.Join(",", result.Errors/*.Union(result.Warnings)*/)}_{string.Join(",", result.Warnings)}";
                errors_warnings = errors_warnings.Trim();
                yield return errors_warnings == "_"
                    ? $"'{result.MeanSquareError}', '{image1.SopInstanceUid}_{image1.FrameNumber}', '{image2.SopInstanceUid}_{image2.FrameNumber}'"
                    : $"'{result.MeanSquareError}', '{image1.SopInstanceUid}_{image1.FrameNumber}', '{image2.SopInstanceUid}_{image2.FrameNumber}', '{errors_warnings}'";
            }
        }

        private IEnumerable<string> Compare2Images(IImageData image1, IImageData image2, bool silentMode)
        {
            using var tracer = _tracer.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(image1).IsNotNull();
            ConsistencyCheck.EnsureArgument(image2).IsNotNull();

            var result = _crossImageMeanSquareError.CompareImages(image1, image2);

            // optional: basic result proof
            ConsistencyCheck.EnsureValue(result).IsNotNull();

            // optional: we can now free cached dicom pixel data after it was used to create the volume (volume contains a copy of the transformed pixel values)
            _dicomDataSetProvider.ClearCache();

            //if (!result.Errors.Any())
            {
                yield return $"{result.MeanSquareError}";
            }

            if (!silentMode && result.Errors.Any())
            {
                yield return "";
                yield return "ERRORS:";
                foreach (var error in result.Errors)
                {
                    yield return $"- {error.ToString()}";
                }
            }

            if (!silentMode && result.Warnings.Any())
            {
                yield return "";
                yield return "WARNINGS:";
                foreach (var warning in result.Warnings)
                {
                    yield return $"- {warning.ToString()}";
                }
            }
        }
    }
}

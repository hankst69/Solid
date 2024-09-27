//----------------------------------------------------------------------------------
// File: "ImageDataVolumeValidator.cs"
// Author: Steffen Hanke
// Date: 2019-2022
//----------------------------------------------------------------------------------
using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.Math;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Solid.Dicom.ImageData.Impl
{
    internal class VolumeValidationResult : IVolumeValidationResult
    {
        public bool ImageCountSufficient { get; internal set; }
        public bool ImageMatrixesMatch { get; internal set; }
        public bool ImageFoVsMatch { get; internal set; }
        public bool ImageNormalsParallel { get; internal set; }
        public bool ImagePositionsInLine { get; internal set; }
        public bool ImagePositionsLineParallelToImageNormals { get; internal set; }
        public bool ImageDistancesEquidistant { get; internal set; }

        public bool Valid => ImageCountSufficient && ImageMatrixesMatch && ImageNormalsParallel && ImagePositionsInLine;
        public bool AllValid => Valid && ImageFoVsMatch && ImagePositionsLineParallelToImageNormals && ImageDistancesEquidistant;
    }

    /// <summary>
    /// API:NO
    /// ImageDataVolumeValidator
    /// </summary>
    public class ImageDataVolumeValidator : IImageDataVolumeValidator
    {
        private readonly ITracer _tracer;

        public ImageDataVolumeValidator() {}

        public ImageDataVolumeValidator(ITracer tracer)
        {
            using var trace = tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            _tracer = tracer;
        }

        public IVolumeValidationResult ValidateVolumeImageData(IEnumerable<IImageData> inputImages, int minimumSliceCount = 3, double tolerance = 0.0001)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(inputImages).IsNotNull();

            // sort input images in space
            var images = inputImages.OrderBy(x => x.ImagePlaneInfo.ImagePositionInNormalDirection).ToList();
            var result = new VolumeValidationResult
            {
                ImageCountSufficient = images.Count >= minimumSliceCount
            };

            var firstImage = images.Count > 0 ? images[0] : null;
            if (firstImage == null)
            {
                return result;
            }

            // check for identical matrix size across all images
            result.ImageMatrixesMatch = firstImage.ImagePixelInfo.PixelColumns * firstImage.ImagePixelInfo.PixelRows > 0 &&
                images.All(image =>
                    image.ImagePixelInfo.PixelColumns == firstImage.ImagePixelInfo.PixelColumns && 
                    image.ImagePixelInfo.PixelRows == firstImage.ImagePixelInfo.PixelRows);

            // check for identical FoV across all images
            var firstFoVRow = firstImage.ImagePixelInfo.PixelColumns > 0 && firstImage.ImagePixelInfo.PixelSizeInRowDir > 0
                ? firstImage.ImagePixelInfo.PixelColumns * firstImage.ImagePixelInfo.PixelSizeInRowDir : 0;
            var firstFoVCol = firstImage.ImagePixelInfo.PixelRows > 0 && firstImage.ImagePixelInfo.PixelSizeInColDir > 0
                ? firstImage.ImagePixelInfo.PixelRows * firstImage.ImagePixelInfo.PixelSizeInColDir : 0;
            result.ImageFoVsMatch = firstFoVCol > 0 && firstFoVRow > 0 &&
                images.All(image =>
                    image.ImagePixelInfo.PixelColumns * image.ImagePixelInfo.PixelSizeInRowDir == firstFoVRow &&
                    image.ImagePixelInfo.PixelRows * image.ImagePixelInfo.PixelSizeInColDir == firstFoVCol);

            // check for ImagePositions being aligned in a straight line
            var firstPos = firstImage.ImagePlaneInfo.Position;
            if (firstPos != null)
            {
                Vector3D baseDir = null;
                var allInLine = true;
                foreach (var image in images)
                {
                    if (image.ImagePlaneInfo.Position == null)
                    {
                        allInLine = false;
                        break;
                    }
                    if (firstPos.IsAlmostEqual(image.ImagePlaneInfo.Position))
                    {
                        continue;
                    }
                    if (baseDir == null)
                    {
                        baseDir = image.ImagePlaneInfo.Position - firstPos;
                        continue;
                    }
                    var newDir = image.ImagePlaneInfo.Position - firstPos;
                    if (!baseDir.IsAlmostParallel(newDir))
                    {
                        allInLine = false;
                        break;
                    }
                }
                result.ImagePositionsInLine = allInLine;
            }

            // check for image normals beeing all parallel
            var firstNormal = firstImage.ImagePlaneInfo.ImageOrientationNormal;
            if (firstNormal != null)
            {
                var allParallel = true;
                foreach (var image in images)
                {
                    if (image.ImagePlaneInfo.ImageOrientationNormal == null)
                    {
                        allParallel = false;
                        break;
                    }
                    if (!firstNormal.IsAlmostParallel(image.ImagePlaneInfo.ImageOrientationNormal))
                    {
                        allParallel = false;
                        break;
                    }
                }
                result.ImageNormalsParallel = allParallel;
            }

            // check for line of ImagePositions being parallel to image normals
            if (result.ImagePositionsInLine && result.ImageNormalsParallel)
            {
                var dir = images.Last().ImagePlaneInfo.Position - firstImage.ImagePlaneInfo.Position;
                result.ImagePositionsLineParallelToImageNormals = dir.IsAlmostParallel(firstImage.ImagePlaneInfo.ImageOrientationNormal);
            }

            // check for equidistant sölice distances
            var distances = new List<double>();
            images.Aggregate((cur, next) =>
            {
                distances.Add(Math.Abs(cur.ImagePlaneInfo.ImagePositionInNormalDirection - next.ImagePlaneInfo.ImagePositionInNormalDirection));
                return next;
            });
            var firstDistance = distances.FirstOrDefault();
            result.ImageDistancesEquidistant = distances.All(distance => distance == firstDistance);

            return result;
        }

        public bool IsImageDataValidForVolumeCreation(IEnumerable<IImageData> inputImages, bool extendedRequirements)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            var validationResult = ValidateVolumeImageData(inputImages);

            if (extendedRequirements)
            {
                return validationResult.AllValid;
            }
            return validationResult.Valid;
        }

    }
}

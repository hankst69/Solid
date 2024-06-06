//----------------------------------------------------------------------------------
// <copyright file="ImageDataVolumeGrouper.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2019-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Solid.Dicom.ImageInfo.Types;
using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.Math;

namespace Solid.Dicom.ImageData.Impl
{
    /// <summary>
    /// API:NO
    /// ImageDataVolumeGrouper
    /// </summary>
    public class ImageDataVolumeGrouper : IImageDataVolumeGrouper
    {
        private readonly ITracer _tracer;
        private readonly IImageDataVolumeValidator _imageDataVolumeValidator;

        public ImageDataVolumeGrouper(IImageDataVolumeValidator imageDataVolumeValidator)
        {
            using var trace = _tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(imageDataVolumeValidator).IsNotNull();
            _imageDataVolumeValidator = imageDataVolumeValidator;
        }

        public ImageDataVolumeGrouper(ITracer tracer, IImageDataVolumeValidator imageDataVolumeValidator)
            :this (imageDataVolumeValidator)
        {
            using var trace = tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            _tracer = tracer;
        }

        private IEnumerable<KeyValuePair<string, IOrderedEnumerable<IImageData>>> BuildBasicVolumeGroups(
            IEnumerable<IImageData> inputImages)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            // group into sets of common orientation, matrix size and pixel spacing
            var basicVolumeGroups = inputImages
                // check for availability of pixel matrix and image plane attributes
                .Where(x => x.ImagePixelInfo.PixelColumns > 0 && x.ImagePixelInfo.PixelRows > 0 && x.ImagePixelInfo.PixelSizeInColDir > 0 && x.ImagePixelInfo.PixelSizeInRowDir > 0
                            && x.ImagePlaneInfo.ImageOrientationMain != OrientationType.Undefined &&
                            !double.IsNaN(x.ImagePlaneInfo.ImagePositionInNormalDirection))
                // build group of slices with identical MainOrientation-Matirix-FoV
                .GroupBy(x => string.Concat(
                    // key part: 'image classification'
                    "-sopclass:", x.ImageClassInfo.SopClassUid,
                    "-protocol", x.ImageClassInfo.ProtocolName,
                    //"-sequence", x.ImageClassInfo.SequencelName,
                    //"-isDerived:", x.ImageClassInfo.IsDerivedImage, 
                    //"for:", x.ImageClassInfo.FrameOfReferenceUid,
                    "-types:[", string.Join(",", x.ImageClassInfo.ImageTypes), "]",
                    // key part: 'immage geometry' (plane and matrix)
                    "-mainori:", x.ImagePlaneInfo.ImageOrientationMain, 
                    "-cols:", x.ImagePixelInfo.PixelColumns,
                    "-rows:", x.ImagePixelInfo.PixelRows, 
                    "-pxlX:", x.ImagePixelInfo.PixelSizeInRowDir, 
                    "-pxlY:", x.ImagePixelInfo.PixelSizeInColDir))
                // sort the groups by their key (not really necessary)
                .OrderBy(x => x.Key)
                #if DEBUG
                .ToList()
                #endif
                ;

            var basicVolumeGroupsWithSortedImages = basicVolumeGroups
               .Select(group => new KeyValuePair<string, IOrderedEnumerable<IImageData>>(
                    // maintain grouping key
                    group.Key,
                    // sort elements of these groups by their ImagePosition and AcquisitionDateTime
                    group.OrderBy(x => x.ImagePlaneInfo.ImagePositionInNormalDirection)
                         .ThenBy(x => x.ImageOrderInfo.AcquisitionDateTime)
                    ))
                #if DEBUG
                .ToList()
                #endif
                ;

            var validVolumeGroups = basicVolumeGroupsWithSortedImages
                .Where(group => _imageDataVolumeValidator.IsImageDataValidForVolumeCreation(group.Value, extendedRequirements: false))
                #if DEBUG
                .ToList()
                #endif
                ;

            return validVolumeGroups;
        }

        public IEnumerable<IEnumerable<IImageData>> GroupIntoVolumes(IEnumerable<IImageData> inputImages)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(inputImages).IsNotNull();

            var volumeGroups = BuildBasicVolumeGroups(inputImages);

            // detect dynamics by grouping images into groups of common imageposition (and splitting into separate volumes afterwards)
            // todo: currently we fail to detect separate volumes of common orientation/MatrixSize/PixelSpacing but just starting at different position (in syngo this might be beds composing)
            var vectorComparer = new Vector3DComparer();
            foreach (var volGroup in volumeGroups)
            {
                var groupedByPosition = volGroup.Value
                    .GroupBy(x => x.ImagePlaneInfo.Position, vectorComparer)
                    .Select(group => new
                    {
                        //ImagePos = group.Key,
                        ImagePosInNormalDirection = group.First().ImagePlaneInfo.ImagePositionInNormalDirection,
                        Images = group.OrderBy(x => x.ImageOrderInfo.AcquisitionDateTime).ToList()
                    })
                    //.OrderBy(group => group.ImagePos, dicomVectorComparer)
                    .OrderBy(group => group.ImagePosInNormalDirection)
                    .ToList();

                var maxCount = groupedByPosition.Max(group => group.Images.Count());
                if (maxCount < 2)
                {
                    yield return volGroup.Value;
                }

                // split into separate sets
                var splittedSets = new IList<IImageData>[maxCount];
                for (int idx = 0; idx < maxCount; idx++)
                {
                    splittedSets[idx] = new List<IImageData>();
                    foreach (var group in groupedByPosition)
                    {
                        if (group.Images.Count > idx)
                        {
                            splittedSets[idx].Add(group.Images[idx]);
                        }
                    }
                }
                foreach (var subSet in splittedSets)
                {
                    // check for at least 2 slices
                    if (subSet.Count > 2)
                    {
                        yield return subSet;
                    }
                }
            }
        }

        public IEnumerable<IEnumerable<IImageData>> Extract4dVolumes(IEnumerable<IImageData> inputImages)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(inputImages).IsNotNull();

            var volumeGroups = BuildBasicVolumeGroups(inputImages);

            // detect dynamics by grouping images into groups of common imageposition
            var vectorComparer = new Vector3DComparer();
            foreach (var volGroup in volumeGroups)
            {
                var groupedByPosition = volGroup.Value
                    .GroupBy(x => x.ImagePlaneInfo.Position, vectorComparer);

                var maxCount = groupedByPosition.Max(group => group.Count());
                if (maxCount > 1)
                {
                    yield return volGroup.Value;
                }
            }
        }
    }
}

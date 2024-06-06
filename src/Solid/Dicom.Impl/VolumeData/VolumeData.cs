//----------------------------------------------------------------------------------
// File: "VolumeData.cs"
// Author: Steffen Hanke
// Date: 2019-2021
//----------------------------------------------------------------------------------

//#define DUMP_IMAGE_TO_CONSOLE

using System;
using System.Collections.Generic;
using System.Linq;
using Solid.Dicom.ImageData;
using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.Math;
using Solid.Infrastructure.RuntimeTypeExtensions;

namespace Solid.Dicom.VolumeData.Impl
{
    /// <inheritdoc/>
    /// <summary>
    /// API:NO
    /// VoulmeData
    /// </summary>
    public class VolumeData : IVolumeData
    {
        public const int MinimumNumbeOfImages = 1;

        internal VolumeData(IEnumerable<IImageData> inputImages)
        {
            ConsistencyCheck.EnsureArgument(inputImages).IsNotNull();

            var sourceImageDatas = inputImages.OrderBy(x => x.ImagePlaneInfo.ImagePositionInNormalDirection).ToList();
            SourceImageDatas = sourceImageDatas;

            if (sourceImageDatas.Count < MinimumNumbeOfImages)
            {
                return;
            }

            // assign basic attributes from reference image
            var refImage = sourceImageDatas.First();

            DimensionSizeX = refImage.ImagePixelInfo.PixelColumns.CastTo<uint>();
            DimensionSizeY = refImage.ImagePixelInfo.PixelRows.CastTo<uint>();
            DimensionSizeZ = SourceImageDatas.Count().CastTo<uint>();
            VoxelSizeX = refImage.ImagePixelInfo.PixelSizeInRowDir;
            VoxelSizeY = refImage.ImagePixelInfo.PixelSizeInColDir;
            VoxelSizeZ = CalculateAverageSliceDistance(sourceImageDatas);
            OrientationX = refImage.ImagePlaneInfo.OrientationRow;
            OrientationY = refImage.ImagePlaneInfo.OrientationCol;
            OrientationZ = refImage.ImagePlaneInfo.ImageOrientationNormal;
            Position = refImage.ImagePlaneInfo.Position;

            IsSigned = refImage.ImagePixelInfo.PixelDataIsSigned;

            // sum up images pixel datas into volume pixel data (into voxels)
            var numberOfVoxels = DimensionSizeX * DimensionSizeY * DimensionSizeZ;
            Voxels = new ushort[numberOfVoxels];
            var indexInVolume = 0;
            foreach (var image in SourceImageDatas)
            {
                var numberOfPixels = image.GetPixelsAsUshortGreyValues(Voxels, indexInVolume);
                indexInVolume += numberOfPixels;
            }
#if DUMP_IMAGE_TO_CONSOLE
            ConsoleHelper.CreateConsole();
            var middleImageIndex = DimensionSizeX * DimensionSizeY * DimensionSizeZ / 2;
            DumpPixels(Voxels, (int)middleImageIndex);
#endif
        }

        private void DumpPixels(ushort[] voxels, int startIndex)
        {
            Console.WriteLine("image pixel dump");
            Int32 maxVal = 0;
            Int32 minVal = Int32.MaxValue;
            for (int r = 0; r < DimensionSizeY; r++)
            {
                for (int c = 0; c < DimensionSizeX; c++)
                {
                    var uval = voxels[startIndex + r * DimensionSizeX + c];
                    if (IsSigned)
                    {
                        var sval = (short) uval;
                        if (sval > maxVal) maxVal = sval;
                        if (sval < minVal) minVal = sval;
                    }
                    else
                    {
                        if (uval > maxVal) maxVal = uval;
                        if (uval < minVal) minVal = uval;
                    }
                }
            }
            var lowBorder = minVal + (maxVal - minVal) / 3;
            var highBorder = maxVal - (maxVal - minVal) / 3;
            for (int r = 0; r < DimensionSizeY; r++)
            {
                for (int c = 0; c < DimensionSizeX; c++)
                {
                    var uval = voxels[startIndex + r * DimensionSizeX + c];
                    char p = ' ';
                    if (IsSigned)
                    {
                        var sval = (short) uval;
                        p = sval > lowBorder ? (sval > highBorder ? '#' : '+') : ' ';
                    }
                    else
                    {
                        p = uval > lowBorder ? (uval > highBorder ? '#' : '+') : ' ';
                    }
                    Console.Write(p);
                }
                Console.WriteLine();
            }
        }

        private double CalculateAverageSliceDistance(IList<IImageData> inputImages)
        {
            ConsistencyCheck.EnsureArgument(inputImages).IsNotNull();
            ConsistencyCheck.EnsureArgument(inputImages.Count).IsGreaterOrEqual(MinimumNumbeOfImages);

            // calculate medium SliceDistance
            var distances = new List<double>();
            inputImages.Aggregate((cur, next) =>
            {
                distances.Add(Math.Abs(cur.ImagePlaneInfo.ImagePositionInNormalDirection - next.ImagePlaneInfo.ImagePositionInNormalDirection));
                return next;
            });

            var mediumDistance = distances.Count > 0 ? distances.Sum() / distances.Count : 0;
            return mediumDistance;
        }

        public uint DimensionSizeX { get; }
        public uint DimensionSizeY { get; }
        public uint DimensionSizeZ { get; }
        public double VoxelSizeX { get; }
        public double VoxelSizeY { get; }
        public double VoxelSizeZ { get; }
        public Vector3D OrientationX { get; }
        public Vector3D OrientationY { get; }
        public Vector3D OrientationZ { get; }
        public Vector3D Position { get; }
        public ushort[] Voxels { get; }
        public bool IsSigned { get; }
        public IEnumerable<IImageData> SourceImageDatas { get; }
    }
}

//----------------------------------------------------------------------------------
// <copyright file="ImageData.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Drawing.Imaging;
using Solid.Dicom.ImageInfo;
using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.Diagnostics.Impl;
using Solid.Infrastructure.RuntimeTypeExtensions;

namespace Solid.Dicom.ImageData.Impl
{
    /// <inheritdoc />
    public class ImageData : IImageData
    {
        private readonly ITracer m_Tracer;
        private readonly IImageAttributes m_ImageAttributes;
        private readonly long m_PixelReadingMaskSignbit;
        private readonly long m_PixelReadingMaskDatabits;
        private readonly int m_PixelReadingBytesPerPixel;

        internal ImageData(IImageAttributes imageAttributes)
            : this(new NullTracer(), imageAttributes)
        {}

        internal ImageData(ITracer tracer, IImageAttributes imageAttributes)
        {
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            ConsistencyCheck.EnsureArgument(imageAttributes).IsNotNull();
            m_Tracer = tracer;
            m_ImageAttributes = imageAttributes;

            // --- PixelData access ---
            PixelData = null;

            // prepare extraction of pixel values
            m_PixelReadingMaskSignbit = 0;
            var databits = ImagePixelInfo.BitsStored;
            if (ImagePixelInfo.PixelDataIsSigned)
            {
                m_PixelReadingMaskSignbit = 1 << ImagePixelInfo.BitsStored;
                databits--;
            }
            m_PixelReadingMaskDatabits = 1;
            for (var idx = 1; idx < databits; idx++)
            {
                m_PixelReadingMaskDatabits <<= 1;
                m_PixelReadingMaskDatabits |= 1;
            }
            m_PixelReadingBytesPerPixel = ImagePixelInfo.BitsAllocated / 8;
        }

        // --- Main dicom information ---
        public string SopInstanceUid => m_ImageAttributes.SopInstanceUid;
        public int FrameNumber => m_ImageAttributes.FrameNumber;
        public IImageAttributes ImageAttributes => m_ImageAttributes;

        // --- ImageInfo instances ---
        public IImageClassInfo ImageClassInfo => m_ImageAttributes.ImageClassInfo;
        public IImagePlaneInfo ImagePlaneInfo => m_ImageAttributes.ImagePlaneInfo;
        public IImagePixelInfo ImagePixelInfo => m_ImageAttributes.ImagePixelInfo;
        public IImageDistortionInfo ImageDistortionInfo => m_ImageAttributes.ImageDistortionInfo;
        public IImageOrderInfo ImageOrderInfo => m_ImageAttributes.ImageOrderInfo;

        // --- ImagePlane handling ---
        public bool InvertImagePlaneNormal
        {
            get => m_ImageAttributes.ImagePlaneInfo.InvertImageNormal;
            set => m_ImageAttributes.ImagePlaneInfo.InvertImageNormal = value;
        }

        // --- PixelData access ---
        public byte[] PixelData { get; private set; }

        public bool IsPixelDataLoaded => PixelData != null;

        public void LoadPixelData()
        {
            using (m_Tracer.CreateScopeTracer())
            {
                if (IsPixelDataLoaded)
                {
                    return;
                }

                const long tag = DicomTags.Tag.PixelData;
                var dataSet = m_ImageAttributes.DataSet;
                if (!dataSet.Contains(tag) || !dataSet.ContainsValue(tag))
                {
                    //throw new ApplicationException();
                    return;
                }

                var blob = dataSet.GetValueAsByteStream(tag);
                if (blob == null)
                {
                    //throw new ApplicationException();
                    return;
                }

                // see if the dicom file contains just 1 image (not a MultiFrame or a MultiFrame with just 1 frame)
                if (ImagePixelInfo.NumberOfFrames < 2)
                {
                    PixelData = blob;
                    return;
                }

                // extract the pixels of the frame
                var numberOfPixels = ImagePixelInfo.PixelRows * ImagePixelInfo.PixelColumns *
                                     ImagePixelInfo.SamplesPerPixel;
                var bytesPerPixel = ImagePixelInfo.BitsAllocated / 8;
                var bytesPerFrame = numberOfPixels * bytesPerPixel;
                var framePixels = new byte[bytesPerFrame];
                var startIndexOfFramePixels = bytesPerFrame * (FrameNumber - 1);
                Array.Copy(blob, startIndexOfFramePixels, framePixels, 0, bytesPerFrame);
                PixelData = framePixels;
            }
        }

        private int CalculateNumberOfPixelBitsOverTargetBits(int numOfTargetBits)
        {
            using (m_Tracer.CreateScopeTracer())
            {
                ConsistencyCheck.EnsureValue(numOfTargetBits).IsGreaterOrEqual(1);

                // prepare case where stored pixels beeing to big to be stored in a targeting data type with numOfTargetBits (for currently used ushort this is 16)
                var numberOfPixelBitsOverNumberOfTargetBits = ImagePixelInfo.BitsStored > numOfTargetBits
                    ? ImagePixelInfo.BitsStored - numOfTargetBits
                    : 0;

                return numberOfPixelBitsOverNumberOfTargetBits;
            }
        }

        private long ReadPixelValue(int pixelStartIdx, bool invert = false)
        {
            //using (m_Tracer.CreateScopeTracer())
            {
                // build the pixel bitarray : assuming it is fitting into a 64bit QWORD (aka long)
                long pxlbits = 0;
                for (var idx = 0; idx < m_PixelReadingBytesPerPixel; idx++)
                {
                    // handle little endian vs big endian
                    var byteIdx = ImagePixelInfo.PixelDataIsBigEndian ? idx : m_PixelReadingBytesPerPixel - 1 - idx;
                    pxlbits <<= 8;
                    var bt = PixelData[pixelStartIdx + byteIdx];
                    pxlbits |= bt;
                }

                // calc pixel value - handle signed pixels
                // remark: for unsigned pixels MaskSignbit is always 0
                //         for signed pixels MaskSignbit is the abs value of the most negative number
                var pixlValue = pxlbits & m_PixelReadingMaskDatabits;
                if ((pxlbits & m_PixelReadingMaskSignbit) != 0)
                {
                    pixlValue -= m_PixelReadingMaskSignbit;
                }
                // now the long variable pixlValue contains the value that was encoded in the input pixelbytes with the correct sign

                // handle inverted pixels (due Photometrix Interpretation)
                // remark: for unsigned pixels inversion means flip around middle -> (invertetValue = MaxValue - pixlValue;)
                //         for signed pixels inversion means flip around 0        -> (invertetValue = -pixlValue;)
                if (invert)
                {
                    pixlValue = m_PixelReadingMaskSignbit != 0 ? -pixlValue : m_PixelReadingMaskDatabits - pixlValue;
                }

                return pixlValue;
            }
        }

        public ushort[] GetPixelsAsUshortGreyValues()
        {
            return GetPixelsAsGreyValues<ushort>();
        }

        public int GetPixelsAsUshortGreyValues(ushort[] targetArray, int targetStartIndex)
        {
            return GetPixelsAsGreyValues(targetArray, targetStartIndex);
        }

        public T[] GetPixelsAsGreyValues<T>() 
            where T : struct, IComparable
        {
            //using (m_Tracer.CreateScopeTracer())
            {
                var numberOfTargetPixels = ImagePixelInfo.PixelRows * ImagePixelInfo.PixelColumns;
                var targetPixels = new T[numberOfTargetPixels];

                var copiedPixels = GetPixelsAsGreyValues(targetPixels, 0);

                ConsistencyCheck.EnsureValue(copiedPixels).IsEqual(numberOfTargetPixels);

                return targetPixels;
            }
        }

        public int GetPixelsAsGreyValues<T>(T[] targetArray, int targetStartIndex) 
            where T : struct, IComparable
        {
            using (m_Tracer.CreateScopeTracer())
            {
                ConsistencyCheck.EnsureArgument(targetArray).IsNotNull();
                ConsistencyCheck.EnsureArgument(targetStartIndex).IsGreaterOrEqual(0);

                var numberOfPixelsToCopy = ImagePixelInfo.PixelRows * ImagePixelInfo.PixelColumns;
                ConsistencyCheck.EnsureValue(targetArray.Length, nameof(targetArray.Length))
                    .IsGreaterOrEqual(targetStartIndex + numberOfPixelsToCopy);

                ConsistencyCheck.EnsureArgument(typeof(T)) //,"typeof(T)")
                    .IsNotOfAnyType(new[] {
                        typeof(char),
                        typeof(bool)
                    })
                    .IsOfAnyType(new[] {
                        typeof(sbyte),
                        typeof(byte),
                        typeof(short),
                        typeof(ushort),
                        typeof(int),
                        typeof(uint),
                        typeof(long),
                        typeof(ulong),
                        typeof(decimal),
                        typeof(float),
                        typeof(double)
                    });

                switch (ImagePixelInfo.PhotometricInterpretation.ToUpper())
                {
                    case "MONOCHROME1":
                        return ConvertMonochromeToGrey(targetArray, targetStartIndex, invert: true);
                    case "MONOCHROME2":
                        return ConvertMonochromeToGrey(targetArray, targetStartIndex, invert: false);
                    case "RGB":
                        return ConvertRgbToGrey(targetArray, targetStartIndex);

                    //case "YBR_FULL":
                    //case "YBR_FULL_422":
                    //case "YCBCR":
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private int ConvertMonochromeToGrey<T>(T[] targetArray, int targetStartIndex, bool invert)
            where T : struct, IComparable
        {
            return ConvertMonochromeToGrey(targetArray, targetStartIndex, invert, out _, out _);
        }

        private int ConvertMonochromeToGrey<T>(T[] targetArray, int targetStartIndex, bool invert, out long minValue, out long maxValue) 
            where T : struct, IComparable
        {
            using (m_Tracer.CreateScopeTracer())
            {
                ConsistencyCheck.EnsureValue(ImagePixelInfo.SamplesPerPixel, nameof(ImagePixelInfo.SamplesPerPixel))
                    .IsEqual((ushort)1);

                LoadPixelData();

                var sourceLength = PixelData.Length;
                var tgtIdx = targetStartIndex;

                minValue = long.MaxValue;
                maxValue = long.MinValue;
                
                // handle FloatingPoint target types
                if (ValueTypeAttributes<T>.IsFloatingPointType)
                {
                    for (var srcIdx = 0; srcIdx < sourceLength; srcIdx += m_PixelReadingBytesPerPixel)
                    {
                        var pixlValue = ReadPixelValue(srcIdx, invert);
                        if (pixlValue < minValue) minValue = pixlValue;
                        if (pixlValue > maxValue) maxValue = pixlValue;

                        // write resulting pixel value into target array (cast value to type T)
                        targetArray[tgtIdx] = pixlValue.CastTo<T>();
                        tgtIdx++;
                    }
                    return tgtIdx - targetStartIndex;
                }

                // handle Interger target types
                var targetBits = ValueTypeAttributes<T>.TotalBits;
                var numberOfPixelBitsOverTargetBits = CalculateNumberOfPixelBitsOverTargetBits(targetBits);
                for (var srcIdx = 0; srcIdx < sourceLength; srcIdx += m_PixelReadingBytesPerPixel)
                {
                    var pixlValue = ReadPixelValue(srcIdx, invert);
                    if (pixlValue < minValue) minValue = pixlValue;
                    if (pixlValue > maxValue) maxValue = pixlValue;

                    // handle case where stored pixels beeing to big be stored in the targeting values of type T
                    if (numberOfPixelBitsOverTargetBits > 0)
                    {
                        var isNeg = m_PixelReadingMaskSignbit > 0 && pixlValue < 0;
                        if (isNeg)
                        {
                            pixlValue = -pixlValue;
                        }
                        pixlValue >>= numberOfPixelBitsOverTargetBits;
                        if (isNeg)
                        {
                            pixlValue = -pixlValue;
                        }
                    }

                    // write resulting pixel value into target array (cast value to type T)
                    // remark: the pixel value should now fit into the target type T (no more data loss except the explicite one above)
                    targetArray[tgtIdx] = pixlValue.CastTo<T>();
                    tgtIdx++;
                }
                return tgtIdx - targetStartIndex;
            }
        }

        private int ConvertRgbToGrey<T>(T[] targetArray, int targetStartIndex)
            where T : struct, IComparable
        {
            using (m_Tracer.CreateScopeTracer())
            {
                ConsistencyCheck.EnsureValue(ImagePixelInfo.SamplesPerPixel, nameof(ImagePixelInfo.SamplesPerPixel))
                    .IsEqual((ushort)3);

                LoadPixelData();

                var tgtIdx = targetStartIndex;
                var pixelComponents = new long[ImagePixelInfo.SamplesPerPixel];

                for (var rowIdx = 0; rowIdx < ImagePixelInfo.PixelRows; rowIdx++)
                {
                    for (var colIdx = 0; colIdx < ImagePixelInfo.PixelColumns; colIdx++)
                    {
                        for (var compIdx = 0; compIdx < ImagePixelInfo.SamplesPerPixel; compIdx++)
                        {
                            int pixelIdx;
                            if (ImagePixelInfo.PlanarConfiguration == 0)
                            {
                                pixelIdx = rowIdx * ImagePixelInfo.PixelColumns
                                           + colIdx * ImagePixelInfo.SamplesPerPixel
                                           + compIdx;
                            }
                            else if (ImagePixelInfo.PlanarConfiguration == 1)
                            {
                                pixelIdx = compIdx * ImagePixelInfo.PixelRows * ImagePixelInfo.PixelColumns
                                           + rowIdx * ImagePixelInfo.PixelColumns
                                           + colIdx;
                            }
                            else
                            {
                                throw new ApplicationException("unxepected PlanarConfiguration");
                            }
                            pixelComponents[compIdx] = ReadPixelValue(pixelIdx);
                        }

                        // convert RGB to Grey
                        var greyValue = 0.2126 * pixelComponents[0] +
                                        0.7152 * pixelComponents[1] +
                                        0.0722 * pixelComponents[2];

                        // write resulting pixel value into target array (cast signed pixels tu ushort)
                        targetArray[tgtIdx] = greyValue.CastTo<T>();
                        tgtIdx++;
                    }
                }
                return tgtIdx - targetStartIndex;
            }
        }

        // ------------------------------------------------------------------------

        public Bitmap GetPixelsAsBitmap()
        {
            using (m_Tracer.CreateScopeTracer())
            {
                var numberOfPixels = ImagePixelInfo.PixelRows * ImagePixelInfo.PixelColumns;
                ConsistencyCheck.EnsureValue(numberOfPixels, nameof(numberOfPixels))
                    .IsGreaterOrEqual(1);

                var width = ImagePixelInfo.PixelColumns;
                var height = ImagePixelInfo.PixelRows;

                var targetPixelFormat = PixelFormat.Format24bppRgb;

                var bitmap = new Bitmap(width, height, targetPixelFormat);
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, targetPixelFormat);

                // the size of the image in bytes
                var bitmapSize = bitmapData.Stride * bitmapData.Height;

                // allocate buffer
                var bitmapBuffer = new byte[bitmapSize];

                // this overload copies data of /size/ into /data/ from location specified (/Scan0/)
                System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, bitmapBuffer, 0, bitmapSize);

                switch (ImagePixelInfo.PhotometricInterpretation.ToUpper())
                {
                    case "MONOCHROME1":
                        ConvertMonochromeTo24BppRgb(bitmapBuffer, 0, invert: true);
                        break;

                    case "MONOCHROME2":
                        ConvertMonochromeTo24BppRgb(bitmapBuffer, 0, invert: false);
                        break;

                    case "RGB":
                        ConvertRgbTo24BppRgb(bitmapBuffer, 0);
                        break;

                    //case "YBR_FULL":
                    //case "YBR_FULL_422":
                    //case "YCBCR":
                    default:
                        throw new NotImplementedException();
                }

                // this override copies the data back into the location specified */
                System.Runtime.InteropServices.Marshal.Copy(bitmapBuffer, 0, bitmapData.Scan0, bitmapBuffer.Length);

                bitmap.UnlockBits(bitmapData);

                return bitmap;
            }
        }

        private int ConvertMonochromeTo24BppRgb(byte[] targetArray, int targetStartIndex, bool invert)
        {
            using (m_Tracer.CreateScopeTracer())
            {
                ConsistencyCheck.EnsureArgument(targetArray).IsNotNull();
                ConsistencyCheck.EnsureArgument(targetStartIndex).IsGreaterOrEqual(0);

                //var targetPixelFormat = PixelFormat.Format24bppRgb;
                var targetBytesPerPixel = 3;

                var numberOfPixelsToCopy = ImagePixelInfo.PixelRows * ImagePixelInfo.PixelColumns;
                var numberOfBytesToCopy = numberOfPixelsToCopy * targetBytesPerPixel;
                ConsistencyCheck.EnsureValue(targetArray.Length, nameof(targetArray.Length))
                    .IsGreaterOrEqual(targetStartIndex + numberOfBytesToCopy);

                ConsistencyCheck.EnsureValue(ImagePixelInfo.SamplesPerPixel, nameof(ImagePixelInfo.SamplesPerPixel))
                    .IsEqual((ushort)1);

                var tmpBuffer = new float[numberOfPixelsToCopy];

                ConvertMonochromeToGrey(tmpBuffer, 0, invert, out var minValue, out var maxValue);

                long dynamik = maxValue > minValue ? maxValue - minValue : 1;

                var tgtIdx = targetStartIndex;

                for (var idx = 0; idx < numberOfPixelsToCopy; idx++)
                {
                    var grey = tmpBuffer[idx];

                    var greyScaled = (grey - minValue) / dynamik * 256.0;
                    var greyByte = (byte)(greyScaled + 0.5);

                    targetArray[tgtIdx] = greyByte;
                    targetArray[tgtIdx + 1] = greyByte;
                    targetArray[tgtIdx + 2] = greyByte;
                    tgtIdx += targetBytesPerPixel;
                }

                return tgtIdx - targetStartIndex;
            }
        }

        private int ConvertRgbTo24BppRgb(byte[] targetArray, int targetStartIndex)
        {
            using (m_Tracer.CreateScopeTracer())
            {
                ConsistencyCheck.EnsureArgument(targetArray).IsNotNull();
                ConsistencyCheck.EnsureArgument(targetStartIndex).IsGreaterOrEqual(0);

                //var targetPixelFormat = PixelFormat.Format24bppRgb;
                var targetBytesPerPixel = 3;
                var targetBitsPerPixelComponent = 8;
                //var targetBitsPerPixel = 24;
                var numberOfPixelBitsOverTargetBits = CalculateNumberOfPixelBitsOverTargetBits(targetBitsPerPixelComponent);

                var numberOfPixelsToCopy = ImagePixelInfo.PixelRows * ImagePixelInfo.PixelColumns;
                var numberOfBytesToCopy = numberOfPixelsToCopy * targetBytesPerPixel;
                ConsistencyCheck.EnsureValue(targetArray.Length, nameof(targetArray.Length))
                    .IsGreaterOrEqual(targetStartIndex + numberOfBytesToCopy);

                ConsistencyCheck.EnsureValue(ImagePixelInfo.SamplesPerPixel, nameof(ImagePixelInfo.SamplesPerPixel))
                    .IsEqual((ushort)3);

                LoadPixelData();

                var tgtIdx = targetStartIndex;
                var pixelComponents = new byte[ImagePixelInfo.SamplesPerPixel];

                for (var rowIdx = 0; rowIdx < ImagePixelInfo.PixelRows; rowIdx++)
                {
                    for (var colIdx = 0; colIdx < ImagePixelInfo.PixelColumns; colIdx++)
                    {
                        for (var compIdx = 0; compIdx < ImagePixelInfo.SamplesPerPixel; compIdx++)
                        {
                            int pixelIdx;
                            if (ImagePixelInfo.PlanarConfiguration == 0)
                            {
                                pixelIdx = rowIdx * ImagePixelInfo.PixelColumns
                                           + colIdx * ImagePixelInfo.SamplesPerPixel
                                           + compIdx;
                            }
                            else if (ImagePixelInfo.PlanarConfiguration == 1)
                            {
                                pixelIdx = compIdx * ImagePixelInfo.PixelRows * ImagePixelInfo.PixelColumns
                                           + rowIdx * ImagePixelInfo.PixelColumns
                                           + colIdx;
                            }
                            else
                            {
                                throw new ApplicationException("unxepected PlanarConfiguration");
                            }

                            var pixlValue = ReadPixelValue(pixelIdx);

                            // handle case where stored pixels beeing to big be stored in the targeting values of type T
                            if (numberOfPixelBitsOverTargetBits > 0)
                            {
                                var isNeg = m_PixelReadingMaskSignbit > 0 && pixlValue < 0;
                                if (isNeg)
                                {
                                    pixlValue = -pixlValue;
                                }
                                pixlValue >>= numberOfPixelBitsOverTargetBits;
                                if (isNeg)
                                {
                                    pixlValue = -pixlValue;
                                }
                            }

                            pixelComponents[compIdx] = (byte)pixlValue;
                        }

                        // convert RGB<long> to 24bppRgb
                        var red = pixelComponents[0];
                        var green = pixelComponents[1];
                        var blue = pixelComponents[2];

                        targetArray[tgtIdx] = blue;
                        targetArray[tgtIdx + 1] = green;
                        targetArray[tgtIdx + 2] = red;
                        tgtIdx += targetBytesPerPixel;
                    }
                }
                return tgtIdx - targetStartIndex;
            }
        }
    }
}

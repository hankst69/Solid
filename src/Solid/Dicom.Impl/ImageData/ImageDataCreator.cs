//----------------------------------------------------------------------------------
// File: "ImageDataCreator.cs"
// Author: Steffen Hanke
// Date: 2020-2022
//----------------------------------------------------------------------------------
using Solid.Infrastructure.Diagnostics;

namespace Solid.Dicom.ImageData.Impl
{
    /// <inheritdoc />
    public class ImageDataCreator : IImageDataCreator
    {
        private readonly ITracer _tracer;
        private readonly IMrDicomAccess _dicomAccess;

        public ImageDataCreator(IMrDicomAccess dicomAccess)
        {
            using var trace = _tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(dicomAccess).IsNotNull();
            _dicomAccess = dicomAccess;
        }

        public ImageDataCreator(ITracer tracer, IMrDicomAccess dicomAccess)
            : this(dicomAccess)
        {
            using var trace = tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            _tracer = tracer;
        }

        public IImageData CreateImageData(IDicomFrameDataSet inputDicomFrameDataSet)
        {
            using var trace = _tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(inputDicomFrameDataSet).IsNotNull();
            var imageAttributes = _dicomAccess.CreateImageAttributes(inputDicomFrameDataSet);
            return new ImageData(_tracer, imageAttributes);
        }

        public IImageData CreateImageDataWithLoadedPixelData(IDicomFrameDataSet inputDicomFrameDataSet)
        {
            using var trace = _tracer?.CreateScopeTracer();
            var imageData = CreateImageData(inputDicomFrameDataSet);
            imageData.LoadPixelData();
            return imageData;
        }
    }
}

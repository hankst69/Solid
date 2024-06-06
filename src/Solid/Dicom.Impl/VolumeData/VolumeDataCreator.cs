//----------------------------------------------------------------------------------
// File: "VolumeDataCreator.cs"
// Author: Steffen Hanke
// Date: 2019-2022
//----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Solid.Dicom.ImageData;
using Solid.Infrastructure.Diagnostics;

namespace Solid.Dicom.VolumeData.Impl
{
    /// <inheritdoc/>
    public class VolumeDataCreator : IVolumeDataCreator
    {
        private readonly ITracer _tracer;
        private readonly IImageDataCreator _imageDataCreator;
        private readonly IImageDataVolumeValidator _imageDataVolumeValidator;

        public VolumeDataCreator(
            IImageDataCreator imageDataCreator,
            IImageDataVolumeValidator imageDataVolumeValidator)
        {
            ConsistencyCheck.EnsureArgument(imageDataCreator).IsNotNull();
            ConsistencyCheck.EnsureArgument(imageDataVolumeValidator).IsNotNull();
            _imageDataCreator = imageDataCreator;
            _imageDataVolumeValidator = imageDataVolumeValidator;
            //_tracer = new NullTracer();
        }

        public VolumeDataCreator(
            ITracer tracer,
            IImageDataCreator imageDataCreator,
            IImageDataVolumeValidator imageDataVolumeValidator)
            : this(imageDataCreator, imageDataVolumeValidator)
        {
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            _tracer = tracer;
        }

        public IVolumeData CreateVolumeData(IEnumerable<IImageData> inputImages)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(inputImages).IsNotNull();

            var images = inputImages as IList<IImageData> ?? inputImages.ToList();
            if (!_imageDataVolumeValidator.ValidateVolumeImageData(images).Valid)
            {
                //throw new ApplicationException("no valid volume");
                return null;
            }

            return new VolumeData(images);
        }

        public IVolumeData CreateVolumeData(IEnumerable<IDicomFrameDataSet> inputFrameDataSets)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(inputFrameDataSets).IsNotNull();

            var inputImages = inputFrameDataSets.Select(x => _imageDataCreator.CreateImageData(x));
            return CreateVolumeData(inputImages);
        }
    }
}

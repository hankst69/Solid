//----------------------------------------------------------------------------------
// <copyright file="VolumeDataCreator.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Solid.Dicom.ImageData;
using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.Diagnostics.Impl;

namespace Solid.Dicom.VolumeData.Impl
{
    /// <inheritdoc/>
    public class VolumeDataCreator : IVolumeDataCreator
    {
        private readonly ITracer m_Tracer;
        private readonly IImageDataCreator m_ImageDataCreator;
        private readonly IImageDataVolumeValidator m_ImageDataVolumeValidator;

        public VolumeDataCreator(
            IImageDataCreator imageDataCreator,
            IImageDataVolumeValidator imageDataVolumeValidator)
            : this (new NullTracer(), imageDataCreator, imageDataVolumeValidator)
        {}

        public VolumeDataCreator(
            ITracer tracer,
            IImageDataCreator imageDataCreator,
            IImageDataVolumeValidator imageDataVolumeValidator)
        {
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            ConsistencyCheck.EnsureArgument(imageDataCreator).IsNotNull();
            ConsistencyCheck.EnsureArgument(imageDataVolumeValidator).IsNotNull();
            m_Tracer = tracer;
            m_ImageDataCreator = imageDataCreator;
            m_ImageDataVolumeValidator = imageDataVolumeValidator;
        }

        public IVolumeData CreateVolumeData(IEnumerable<IImageData> inputImages)
        {
            using (m_Tracer.CreateScopeTracer())
            {
                ConsistencyCheck.EnsureArgument(inputImages).IsNotNull();

                var images = inputImages as IList<IImageData> ?? inputImages.ToList();
                if (!m_ImageDataVolumeValidator.ValidateVolume(images))
                {
                    //throw new ApplicationException("no valid volume");
                    return null;
                }

                return new VolumeData(images);
            }
        }

        public IVolumeData CreateVolumeData(IEnumerable<IDicomFrameDataSet> inputFrameDataSets)
        {
            using (m_Tracer.CreateScopeTracer())
            {
                ConsistencyCheck.EnsureArgument(inputFrameDataSets).IsNotNull();

                var inputImages = inputFrameDataSets.Select(x => m_ImageDataCreator.CreateImageData(x));
                return CreateVolumeData(inputImages);
            }
        }
    }
}

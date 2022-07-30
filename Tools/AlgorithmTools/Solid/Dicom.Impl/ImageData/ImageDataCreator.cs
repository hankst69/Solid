//----------------------------------------------------------------------------------
// <copyright file="ImageDataCreator.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.Diagnostics.Impl;

namespace Solid.Dicom.ImageData.Impl
{
    /// <inheritdoc />
    public class ImageDataCreator : IImageDataCreator
    {
        private readonly ITracer m_Tracer;
        private readonly IMrDicomAccess m_DicomAccess;

        public ImageDataCreator(IMrDicomAccess dicomAccess)
            : this(new NullTracer(), dicomAccess)
        {}

        public ImageDataCreator(ITracer tracer, IMrDicomAccess dicomAccess)
        {
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            ConsistencyCheck.EnsureArgument(dicomAccess).IsNotNull();
            m_Tracer = tracer;
            m_DicomAccess = dicomAccess;
        }

        public IImageData CreateImageData(IDicomFrameDataSet inputDicomFrameDataSet)
        {
            using (m_Tracer.CreateScopeTracer())
            {
                ConsistencyCheck.EnsureArgument(inputDicomFrameDataSet).IsNotNull();
                var imageAttributes = m_DicomAccess.CreateImageAttributes(inputDicomFrameDataSet);
                return new ImageData(m_Tracer, imageAttributes);
            }
        }

        public IImageData CreateImageDataWithLoadedPixelData(IDicomFrameDataSet inputDicomFrameDataSet)
        {
            using (m_Tracer.CreateScopeTracer())
            {
                var imageData = CreateImageData(inputDicomFrameDataSet);
                imageData.LoadPixelData();
                return imageData;
            }
        }
    }
}

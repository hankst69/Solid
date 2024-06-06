//----------------------------------------------------------------------------------
// File: "DicomFrameDataSetCreator.cs"
// Author: Steffen Hanke
// Date: 2022
//----------------------------------------------------------------------------------
using Solid.Infrastructure.Diagnostics;

using System.Collections.Generic;
using System.Linq;

namespace Solid.Dicom.Impl
{
    public class DicomFrameDataSetCreator : IDicomFrameDataSetCreator
    { 
        protected readonly ITracer _tracer;

        public DicomFrameDataSetCreator()
        {
            _tracer = null;
        }

        public DicomFrameDataSetCreator(ITracer tracer)
        {
            using var trace = tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            _tracer = tracer;
        }

        public IDicomFrameDataSet CreateFrameDataSet(IDicomDataSet dicomDataSet, int frameNumber)
        {
            //using var tracer = _tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(dicomDataSet).IsNotNull();

            return new DicomFrameDataSet(dicomDataSet, frameNumber);
        }

        public IEnumerable<IDicomFrameDataSet> CreateFrameDataSets(IDicomDataSet dicomDataSet)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(dicomDataSet).IsNotNull();

            var numberOfFrames = dicomDataSet.GetValue(DicomTags.Tag.NumberOfFrames);
            var frameCount = numberOfFrames == null ? 1 : DicomValues.ConvertDicomIsToInt(numberOfFrames);

            var framedataSets = new List<IDicomFrameDataSet>();
            for (var frameNo = 1; frameNo <= frameCount; frameNo++)
            {
                framedataSets.Add(CreateFrameDataSet(dicomDataSet, frameNo));
            }
            return framedataSets;
        }

        public IEnumerable<IDicomFrameDataSet> CreateFrameDataSets(IEnumerable<IDicomDataSet> dicomDataSets)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(dicomDataSets).IsNotNull();
            return dicomDataSets.SelectMany(CreateFrameDataSets);
        }
    }
}

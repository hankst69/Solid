//----------------------------------------------------------------------------------
// <copyright file="DataSetBuilder.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2015-2019. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Moq;
using Solid.Dicom;
using Solid.Infrastructure.Math;
using Solid.Infrastructure.RuntimeTypeExtensions;

using LocalTag = Solid.Dicom.DicomTags;
using OrientationType = Solid.Dicom.ImageInfo.Types.OrientationType;
using SopClassUids = Solid.Dicom.DicomTags.MrSopClassUids;

namespace Solid.TestInfrastructure.Dicom
{
    public interface IDataSet
    {
        int Count { get; }
        IDataElement this[long tag] { get; }

        bool Contains(long tag);
        bool ContainsValue(long tag);
        int GetNumberOfValues(long tag);

        object GetValueAt(long tag, int index);
        IDataSet GetItemAt(long tag, int index);
        IInstanceSyngoUid SyngoUid { get; }

        bool IsElementEmpty(long tag);
    }

    public interface IDataElement
    {
        int Count { get; }

        object this[long index] { get; }

        bool IsElementEmpty();

        byte[] GetValueAsByteStream();
    }

    public interface IInstanceSyngoUid
    {
        string StudyInstanceUid { get; }
        string SeriesInstanceUid { get; }
        string SopInstanceUid { get; }
        string SopClassUid { get; }
    }

    internal interface IImageInfo
    {
        int Rows { get; }
        int Cols { get; }
        double PixelSpacingInColDirection { get; }
        double PixelSpacingInRowDirection { get; }
    }
    internal interface ISlice
    {
        Vector3D Position { get; }
        Vector3D RowDir { get; }
        Vector3D ColDir { get; }

        IImageInfo ImageInfo { get; }
    }

    internal interface IFrameInfo
    {
        ISlice Slice { get; }
        //Slice SliceNoCopy { get; }
        Vector3D ImagePosition { get; }
        Vector3D ImageOrientationRow { get; }
        Vector3D ImageOrientationColumn { get; }
        bool IsTagAvailable { get; }
        bool IsImageOrientationTagAvailable { get; }
        void GetPixelSpacing(out double pixelSpacingX_out, out double pixelSpacingY_out);
    }



    public interface IDataSetBuilder
    {
        IDataSet ToDataSet();

        //Mock<IFrame> ToFrameMock();
        //Mock<IFrame> ToFrameMock(int frameNumber);
        //Mock<IFrameItem> ToDataItemMock();
        //Mock<IFrameItem> ToDataItemMock(int frameNumber);
        Mock<IDicomFrameDataSet> ToDicomFrameDataSetMock();
        Mock<IDicomFrameDataSet> ToDicomFrameDataSetMock(int frameNumber);

        IDataSetBuilder DisablePlausibilityChecks();
        IDataSetBuilder RemoveTag(long tag);
        IDataSetBuilder SetupEmptyTag(long tag);
        IDataSetBuilder SetupTag(long tag, object value, int valueIndex);
        IDataSetBuilder SetupMvTag(long tag, object value, int valueMultiplicity);
        IDataSetBuilder GetOrCreateSequence(long sequenceTag, int valueIndex);
    }

    public interface IImageBuilder
    {
        IDataSet ToDataSet();

        //Mock<IFrame> ToFrameMock();
        //Mock<IFrame> ToFrameMock(int frameNumber);
        //Mock<IFrameItem> ToDataItemMock();
        //Mock<IFrameItem> ToDataItemMock(int frameNumber);
        Mock<IDicomFrameDataSet> ToDicomFrameDataSetMock();
        Mock<IDicomFrameDataSet> ToDicomFrameDataSetMock(int frameNumber);

        IImageBuilder DisablePlausibilityChecks();
        IImageBuilder WithoutTag(long tag);
        IImageBuilder WithEmptyTag(long tag);

        IImageBuilder WithFrameOfReferenceUid(string forUid);
        IImageBuilder WithImageType(string type);
        IImageBuilder WithStudyInstanceUid(string studyUid);
        IImageBuilder WithSeriesInstanceUid(string seriesUid);
        IImageBuilder WithSopInstanceUid(string instanceUid);
        IImageBuilder WithStudyDescription(string studyDescription);
        IImageBuilder WithSeriesDescription(string seriesDescription);
        IImageBuilder WithSeriesNumber(int seriesNumber);
        IImageBuilder WithSeriesDate(DateTime date);
        IImageBuilder WithInstanceNumber(int instanceNumber);

        IImageBuilder WithTriggerTime(double triggerTime);
        IImageBuilder WithTimeAfterStart(double timeAfterStart);
        IImageBuilder WithIsPhaseEncodingDirectionPositive(bool isPhaseEncodingDirectionPositive);
        IImageBuilder WithImaRelTablePos(Vector3D tabelPos);
        IImageBuilder WithDistortionCorrectionType(string disCorTypeName);
        IImageBuilder WithVolumetricProperties(string volPropTypeName);
        IImageBuilder WithFrameLevelVolumetricProperties(string volPropTypeName);
        IImageBuilder WithScanningSequence(string scanningSequence);
        IImageBuilder WithSequenceVariant(string scanningSequence);
        IImageBuilder WithGradientCoilName(string gradCoilName);

        IImageBuilder WithAcquisitionDateTime(string date, string time);
        IImageBuilder WithAcquisitionDateTime(DateTime dateTime);
        IImageBuilder WithImageMatrix(int rowCount, int columCount, double pixelSpacingX, double pixelSpacingY);
        IImageBuilder WithInPlanePhaseEncodingDirection(string code);
        IImageBuilder WithPhaseEncodingDirectionPositive(bool isPositive);
        IImageBuilder WithImagePlane(double[] position, double[] orientation);
        IImageBuilder WithImagePlane(Vector3D position, Vector3D row, Vector3D column);
        //IImageBuilder WithImageMode(ImageMode mode);
        IImageBuilder WithMainOrientation(OrientationType orientation, double sliceShift = 0);

        IImageBuilder UsingFrame(int frameNumber);
        IImageBuilder WithFrameAcquisitionDateTime(string date, string time);
        IImageBuilder WithFrameType(string type);
        IImageBuilder WithThickness(double imageThickness);
        IImageBuilder WithSliceThickness(string dsValue);
        IImageBuilder WithPatientPosition(string patPos);
    }

    public class DataSetBuilder : Mock<IDataSet>, IDataSetBuilder
    {
        public interface IBuilderDataElement : IDataElement
        {
            IDataSetBuilder GetDataSetBuilder(int index);
        }

        private readonly IDictionary<long, Mock<IBuilderDataElement>> m_MockedTags = new Dictionary<long, Mock<IBuilderDataElement>>();
        private bool m_CheckPlausibility = true;

        public DataSetBuilder()
        {
            Setup(ds => ds.Contains(It.IsAny<long>())).Returns((long tag) => m_MockedTags.ContainsKey(tag));
            Setup(ds => ds.ContainsValue(It.IsAny<long>())).Returns((long tag) => m_MockedTags.ContainsKey(tag) && m_MockedTags[tag].Object.Count > 0);
            Setup(ds => ds.GetNumberOfValues(It.IsAny<long>())).Returns((long tag) => m_MockedTags.ContainsKey(tag) ? m_MockedTags[tag].Object.Count : 0);
            Setup(ds => ds[It.IsAny<long>()]).Returns((long tag) => m_MockedTags.ContainsKey(tag) ? m_MockedTags[tag].Object : null);
            Setup(ds => ds.GetValueAt(It.IsAny<long>(), It.IsAny<int>())).Returns((long tag, int index) => m_MockedTags.ContainsKey(tag) && m_MockedTags[tag].Object.Count > index ? m_MockedTags[tag].Object[index] : null);
            Setup(ds => ds.GetItemAt(It.IsAny<long>(), It.IsAny<int>())).Returns((long tag, int index) => m_MockedTags.ContainsKey(tag) && m_MockedTags[tag].Object.Count > index ? m_MockedTags[tag].Object[index] as IDataSet : null);
            //Setup(ds => ds.GetItemAt(It.IsAny<long>(), It.IsAny<int>())).Returns((long tag, int index) => this.Object.GetValueAt(tag,index) as IDataSet);
            SetupGet(ds => ds.SyngoUid).Returns(CreateInstanceSyngoUid);
        }

        public IDataSetBuilder DisablePlausibilityChecks()
        {
            m_CheckPlausibility = false;
            return this;
        }

        public IDataSet ToDataSet()
        {
            return Object;
        }

        public Mock<IDicomFrameDataSet> ToDicomFrameDataSetMock()
        {
            if (this is EnhancedMrImageBuilder builder)
            {
                return ToDicomFrameDataSetMock(builder.UsingFrameNumber);
            }
            return ToDicomFrameDataSetMock(1);
        }

        public Mock<IDicomFrameDataSet> ToDicomFrameDataSetMock(int frameNumber)
        {
            if (frameNumber < 1) throw new ArgumentOutOfRangeException(nameof(frameNumber));
            return DicomFrameDataSetMockFromDataSet(ToDataSet(), frameNumber);
        }

        private Mock<IDicomFrameDataSet> DicomFrameDataSetMockFromDataSet(IDataSet dataSet, int frameNumber)
        {
            var frameDataSetMock = new Mock<IDicomFrameDataSet>();
            frameDataSetMock.SetupGet(f => f.FrameNumber).Returns(frameNumber);
            frameDataSetMock.Setup(f => f.Contains(It.IsAny<long>())).Returns((long tag) => dataSet.Contains(tag));
            frameDataSetMock.Setup(f => f.IsElementEmpty(It.IsAny<long>())).Returns((long tag) => dataSet.IsElementEmpty(tag));
            frameDataSetMock.Setup(f => f.GetNumberOfValues(It.IsAny<long>())).Returns((long tag) => dataSet[tag].Count);
            frameDataSetMock.Setup(f => f.ContainsValue(It.IsAny<long>())).Returns((long tag) => dataSet[tag].Count > 0);
            frameDataSetMock.Setup(f => f.ContainsValueAt(It.IsAny<long>(), It.IsAny<int>())).Returns((long tag, int idx) => dataSet[tag].Count > idx);
            frameDataSetMock.Setup(f => f.IsValueEmpty(It.IsAny<long>())).Returns((long tag) => string.IsNullOrEmpty(dataSet[tag][0] as string));
            frameDataSetMock.Setup(f => f.IsValueEmptyAt(It.IsAny<long>(), It.IsAny<int>())).Returns((long tag, int idx) => string.IsNullOrEmpty(dataSet[tag][idx] as string));
            frameDataSetMock.Setup(f => f.GetValue(It.IsAny<long>())).Returns((long tag) => dataSet[tag][0]);
            frameDataSetMock.Setup(f => f.GetValueAt(It.IsAny<long>(), It.IsAny<int>())).Returns((long tag, int idx) => dataSet[tag][idx]);
            frameDataSetMock.Setup(f => f.GetValueAsByteStream(It.IsAny<long>())).Returns((long tag) => dataSet[tag].GetValueAsByteStream());
            frameDataSetMock.Setup(f => f.GetItemAt(It.IsAny<long>(), It.IsAny<int>())).Returns((long tag, int idx) => DicomFrameDataSetMockFromDataSet(dataSet[tag][idx] as IDataSet, frameNumber).Object);
            return frameDataSetMock;
        }

        /*
        public Mock<IFrame> ToFrameMock()
        {
            if (this is EnhancedMrImageBuilder builder)
            {
                return ToFrameMock(builder.UsingFrameNumber);
            }
            return ToFrameMock(1);
        }

        public Mock<IFrame> ToFrameMock(int frameNumber)
        {
            if (frameNumber < 1) throw new ArgumentOutOfRangeException(nameof(frameNumber));
            //IFrame
            //{
            //    int FrameNumber { get; }
            //    IDataElement this[long attributeKey_in] { get; }
            //    ISyngoUid SyngoUID { get; }
            //    int OriginalLevel { get; }
            //    IFrameInfo FrameInfo { get; }
            //    void BeginAccess();
            //    void EndAccess();
            //    IMemoryHandle LoadPixel(bool loadOption_in);
            //    IMemoryHandle LoadOriginalPixel(bool loadOption_in);
            //    void UnloadPixel(IMemoryHandle handle_in);
            //    bool Contains(long groupElementKey_in);
            //    bool IsPixelLoaded(int level_in);
            //}

            var frameMock = new Mock<IFrame>();
            frameMock.Setup(f => f[It.IsAny<long>()]).Returns((long tag) => ToDataSet()[tag]);
            frameMock.Setup(f => f.Contains(It.IsAny<long>())).Returns((long tag) => ToDataSet().Contains(tag));
            frameMock.SetupGet(f => f.FrameNumber).Returns(frameNumber);
            frameMock.SetupGet(f => f.SyngoUID).Returns(() => ToDataSet().SyngoUid);
            frameMock.SetupGet(f => f.FrameInfo).Returns(() => CreateFrameInfo(frameNumber));
            
            if (IsEnhancedMrImage(ToDataSet()))
            {
                // provide access to per frame image Position via tag LocalTag.Tag.ImagePositionPatient
                frameMock.Setup(f => f.Contains(LocalTag.Tag.ImagePositionPatient)).Returns(true);
                frameMock.Setup(f => f[LocalTag.Tag.ImagePositionPatient]).Returns(() =>
                {
                    var pos = ReadPositionVector(ToDataSet(), frameNumber);
                    if (pos == null) 
                        return CreateEmptyDataElementMock().Object;
                    var dataElementMock = new Mock<IDataElement>();
                    dataElementMock.Setup(de => de.IsElementEmpty()).Returns(false);
                    dataElementMock.SetupGet(de => de.Count).Returns(3);
                    dataElementMock.SetupGet(de => de[0]).Returns(pos.X);
                    dataElementMock.SetupGet(de => de[1]).Returns(pos.Y);
                    dataElementMock.SetupGet(de => de[2]).Returns(pos.Z);
                    return dataElementMock.Object;
                });

                // provide access to per frame image Orientation via tag LocalTag.Tag.ImageOrientationPatient
                frameMock.Setup(f => f.Contains(LocalTag.Tag.ImageOrientationPatient)).Returns(true);
                frameMock.Setup(f => f[LocalTag.Tag.ImageOrientationPatient]).Returns(() =>
                {
                    var row = ReadRowVector(ToDataSet(), frameNumber);
                    var col = ReadColumnVector(ToDataSet(), frameNumber);
                    if (row == null || col == null) 
                        return CreateEmptyDataElementMock().Object;
                    var dataElementMock = new Mock<IDataElement>();
                    dataElementMock.Setup(de => de.IsElementEmpty()).Returns(false);
                    dataElementMock.SetupGet(de => de.Count).Returns(6);
                    dataElementMock.SetupGet(de => de[0]).Returns(row.X);
                    dataElementMock.SetupGet(de => de[1]).Returns(row.Y);
                    dataElementMock.SetupGet(de => de[2]).Returns(row.Z);
                    dataElementMock.SetupGet(de => de[3]).Returns(col.X);
                    dataElementMock.SetupGet(de => de[4]).Returns(col.Y);
                    dataElementMock.SetupGet(de => de[5]).Returns(col.Z);
                    return dataElementMock.Object;
                });

                // provide access to per frame PixelSpacing via tag LocalTag.Tag.PixelSpacing
                frameMock.Setup(f => f.Contains(LocalTag.Tag.PixelSpacing)).Returns(true);
                frameMock.Setup(f => f[LocalTag.Tag.PixelSpacing]).Returns(() =>
                {
                    var pixelX = ReadPixelSpacingX(ToDataSet());
                    var pixelY = ReaPixelSpacingY(ToDataSet());
                    if (!(Math.Abs(pixelX) > 0.0) || !(Math.Abs(pixelY) > 0.0)) 
                        return CreateEmptyDataElementMock().Object;
                    var dataElementMock = new Mock<IDataElement>();
                    dataElementMock.Setup(de => de.IsElementEmpty()).Returns(false);
                    dataElementMock.SetupGet(de => de.Count).Returns(2);
                    dataElementMock.SetupGet(de => de[0]).Returns(pixelX);
                    dataElementMock.SetupGet(de => de[1]).Returns(pixelY);
                    return dataElementMock.Object;
                });
            }

            return frameMock;
        }


        public Mock<IFrameItem> ToDataItemMock()
        {
            if (this is EnhancedMrImageBuilder builder)
            {
                return ToDataItemMock(builder.UsingFrameNumber);
            }
            return ToDataItemMock(1);
        }

        public Mock<IFrameItem> ToDataItemMock(int frameNumber)
        {
            if (frameNumber < 1) throw new ArgumentOutOfRangeException(nameof(frameNumber));
            //IDataItem
            //{
            //    ISyngoUid SyngoUID { get; }
            //    string SopClassUid { get; }
            //    bool IsSelected { get; set; }
            //    IDataSet RetrieveDataSet();
            //    IDataElement this[long groupElementTag_in] { get; }
            //    bool Contains(long groupElementKey_in);
            //    LoadingState InputComplete { get; set; }
            //    void BeginAccess();
            //    void EndAccess();
            //    void Dispose();
            //}
            //IFrameItem : IDataItem
            //{
            //    IFrame Frame { get; }
            //}

            var dataItemMock = new Mock<IFrameItem>();
            var frameMock = ToFrameMock(frameNumber);
            dataItemMock.SetupGet(di => di.Frame).Returns(frameMock.Object);
            dataItemMock.SetupGet(di => di.SyngoUID).Returns(frameMock.Object.SyngoUID);
            dataItemMock.Setup(di => di[It.IsAny<long>()]).Returns((long tag) => frameMock.Object[tag]);
            dataItemMock.Setup(di => di.Contains(It.IsAny<long>())).Returns((long tag) => frameMock.Object.Contains(tag));
            dataItemMock.Setup(di => di.RetrieveDataSet()).Returns(ToDataSet);
            return dataItemMock;
        }
        */

        public IDataSetBuilder RemoveTag(long tag)
        {
            if (m_MockedTags.ContainsKey(tag))
            {
                m_MockedTags.Remove(tag);
            }
            return this;
        }

        public IDataSetBuilder SetupEmptyTag(long tag)
        {
            if (m_MockedTags.ContainsKey(tag))
            {
                throw new ArgumentException(string.Format("tag already assigned  (tag {0})", tag));
            }
            m_MockedTags.Add(tag, CreateEmptyDataElementMock());
            return this;
        }

        public IDataSetBuilder SetupTag(long tag, object value, int valueIndex)
        {
            if (!m_MockedTags.ContainsKey(tag))
            {
                var dataElementMock = new Mock<IBuilderDataElement>();
                dataElementMock.Setup(de => de.IsElementEmpty()).Returns(false);
                dataElementMock.SetupGet(de => de.Count).Returns(valueIndex+1);
                dataElementMock.SetupGet(de => de[valueIndex]).Returns(value);
                dataElementMock.Setup(de => de.GetDataSetBuilder(valueIndex)).Returns(this);
                m_MockedTags.Add(tag, dataElementMock);
                return this;
            }

            if (m_MockedTags[tag] == null)
            {
                throw new ArgumentException(string.Format("basic tag setup failed for  (tag {0}  index {1})", tag, valueIndex));
            }
            if (m_MockedTags[tag].Object[valueIndex] != null && m_CheckPlausibility)
            {
                throw new ArgumentException(string.Format("tag value already assigned  (tag {0}  index {1})", tag, valueIndex));
            }

            m_MockedTags[tag].SetupGet(de => de[valueIndex]).Returns(value);
            var count = m_MockedTags[tag].Object.Count;
            if (count <= valueIndex)
            {
                m_MockedTags[tag].SetupGet(de => de.Count).Returns(valueIndex + 1);
            }
            return this;
        }

        public IDataSetBuilder SetupMvTag(long tag, object value, int valueMultiplicity)
        {
            if (!m_MockedTags.ContainsKey(tag))
            {
                return SetupTag(tag, value, 0);
            }
            var count = m_MockedTags[tag].Object.Count;
            if (count >= valueMultiplicity)
            {
                throw new IndexOutOfRangeException();
            }
            return SetupTag(tag, value, count);
        }

        public IDataSetBuilder GetOrCreateSequence(long sequenceTag, int valueIndex)
        {
            if (sequenceTag == LocalTag.Tag.PerFrameFunctionalGroupsSequence)
            {
                RemoveTag(LocalTag.Tag.NumberOfFrames);
                SetupTag(LocalTag.Tag.NumberOfFrames, valueIndex + 1, 0);
            }

            IDataSetBuilder sequenceBuilder;
            if (!m_MockedTags.ContainsKey(sequenceTag)
                || m_MockedTags[sequenceTag].Object.Count <= valueIndex
                || m_MockedTags[sequenceTag].Object == null)
            {
                sequenceBuilder = new DataSetBuilder();
                SetupSequence(sequenceTag, sequenceBuilder, valueIndex);
                return sequenceBuilder;
            }
            sequenceBuilder = (ToDataSet()[sequenceTag] as IBuilderDataElement)?.GetDataSetBuilder(valueIndex);
            return sequenceBuilder;
        }

        private void SetupSequence(long tag, IDataSetBuilder value, int valueIndex)
        {
            if (!m_MockedTags.ContainsKey(tag))
            {
                var sequenceDataElementMock = new Mock<IBuilderDataElement>();
                sequenceDataElementMock.Setup(de => de.IsElementEmpty()).Returns(false);
                sequenceDataElementMock.SetupGet(de => de.Count).Returns(valueIndex+1);
                sequenceDataElementMock.SetupGet(de => de[valueIndex]).Returns(() => value.ToDataSet());
                sequenceDataElementMock.Setup(de => de.GetDataSetBuilder(valueIndex)).Returns(value);
                m_MockedTags.Add(tag, sequenceDataElementMock);
                return;
            }

            if (m_MockedTags[tag] == null)
            {
                throw new ArgumentException(string.Format("basic tag setup failed for  (tag {0}  index {1})", tag, valueIndex));
            }
            if (m_MockedTags[tag].Object[valueIndex] != null /*&& m_CheckPlausibility*/)
            {
                throw new ArgumentException(string.Format("tag value already assigned  (tag {0}  index {1})", tag, valueIndex));
            }

            m_MockedTags[tag].SetupGet(de => de[valueIndex]).Returns(() => value.ToDataSet());
            var count = m_MockedTags[tag].Object.Count;
            if (count <= valueIndex)
            {
                m_MockedTags[tag].SetupGet(de => de.Count).Returns(valueIndex + 1);
            }

            m_MockedTags[tag].Setup(de => de.GetDataSetBuilder(valueIndex)).Returns(value);
        }

        protected static Mock<IBuilderDataElement> CreateEmptyDataElementMock()
        {
            var dataElementMock = new Mock<IBuilderDataElement>();
            dataElementMock.Setup(de => de.IsElementEmpty()).Returns(true);
            dataElementMock.SetupGet(de => de.Count).Returns(0);
            dataElementMock.Setup(de => de[It.IsAny<int>()]).Returns(() => { throw new IndexOutOfRangeException(); });
            return dataElementMock;
        }

        private IInstanceSyngoUid CreateInstanceSyngoUid()
        {
            var studyInstanceUId = m_MockedTags.ContainsKey(LocalTag.Tag.StudyInstanceUid) ? m_MockedTags[LocalTag.Tag.StudyInstanceUid].Object[0].As<string>() : string.Empty;
            var seriesInstanceUId = m_MockedTags.ContainsKey(LocalTag.Tag.SeriesInstanceUid) ? m_MockedTags[LocalTag.Tag.SeriesInstanceUid].Object[0].As<string>() : string.Empty;
            var sopInstanceUId = m_MockedTags.ContainsKey(LocalTag.Tag.SopInstanceUid) ? m_MockedTags[LocalTag.Tag.SopInstanceUid].Object[0].As<string>() : string.Empty;
            var sopClassUid = m_MockedTags.ContainsKey(LocalTag.Tag.SopClassUid) ? m_MockedTags[LocalTag.Tag.SopClassUid].Object[0].As<string>() : string.Empty;
            
            var instanceUidMock = new Mock<IInstanceSyngoUid>();
            instanceUidMock.SetupGet(i => i.StudyInstanceUid).Returns(studyInstanceUId);
            instanceUidMock.SetupGet(i => i.SeriesInstanceUid).Returns(seriesInstanceUId);
            instanceUidMock.SetupGet(i => i.SopInstanceUid).Returns(sopInstanceUId);
            instanceUidMock.SetupGet(i => i.SopClassUid).Returns(sopClassUid);
            instanceUidMock.Setup(i => i.ToString()).Returns(() => string.Concat("{", studyInstanceUId, "}, {", seriesInstanceUId, "}, {", sopInstanceUId, "}, {", sopClassUid, "}"));
            return instanceUidMock.Object;
        }

        #region FrameInfo creation
        private readonly IDictionary<int, IFrameInfo> m_CachedFrameInfos = new Dictionary<int, IFrameInfo>();

        private IFrameInfo CreateFrameInfo(int frameNumber)
        {
            //public interface IFrameInfo
            //{
            //    Slice Slice { get; }
            //    Slice SliceNoCopy { get; }
            //    Vector3d ImagePosition { get; }
            //    Vector3d ImageOrientationRow { get; }
            //    Vector3d ImageOrientationColumn { get; }
            //    bool IsTagAvailable { get; }
            //    bool IsImageOrientationTagAvailable { get; }
            //    void GetPixelSpacing(out double pixelSpacingX_out, out double pixelSpacingY_out);
            //}

            if (m_CachedFrameInfos.ContainsKey(frameNumber))
            {
                return m_CachedFrameInfos[frameNumber];
            }

            var dataSet = ToDataSet();
            var pos = ReadPositionVector(dataSet, frameNumber);
            var row = ReadRowVector(dataSet, frameNumber);
            var col = ReadColumnVector(dataSet, frameNumber);
            var pixelX = ReadPixelSpacingX(dataSet);
            var pixelY = ReaPixelSpacingY(dataSet);
            var rows = ReadRows(dataSet);
            var cols = ReadCols(dataSet);

            var frameInfoMock = new Mock<IFrameInfo>();
            if (pos != null)
            {
                frameInfoMock.SetupGet(fi => fi.IsTagAvailable).Returns(true);
                frameInfoMock.SetupGet(fi => fi.ImagePosition).Returns(pos);
            }
            if (row != null && col != null)
            {
                frameInfoMock.SetupGet(fi => fi.IsImageOrientationTagAvailable).Returns(true);
                frameInfoMock.SetupGet(fi => fi.ImageOrientationRow).Returns(row);
                frameInfoMock.SetupGet(fi => fi.ImageOrientationColumn).Returns(col);
            }
            if (Math.Abs(pixelX) > 0.0 && Math.Abs(pixelY) > 0.0)
            {
                frameInfoMock.Setup(fi => fi.GetPixelSpacing(out pixelX, out pixelY));
            }

            frameInfoMock.SetupGet(fi => fi.Slice).Returns(() => CreateSlice(pos, row, col, rows, cols, pixelX, pixelY));
            //frameInfoMock.SetupGet(fi => fi.SliceNoCopy).Returns(() => CreateSlice(pos, row, col, rows, cols, pixelX, pixelY));

            m_CachedFrameInfos[frameNumber] = frameInfoMock.Object;
            return frameInfoMock.Object;
        }

        private static ISlice CreateSlice(Vector3D pos, Vector3D row, Vector3D col, int rows, int cols, double pixelX, double pixelY)
        {
            pos = pos ?? new Vector3D(0.0, 0.0, 0.0);
            row = row ?? new Vector3D(1.0, 0.0, 0.0);
            col = col ?? new Vector3D(0.0, 1.0, 0.0);

            var sliceMock = new Mock<ISlice>();
            sliceMock.SetupGet(x => x.Position).Returns(pos);
            sliceMock.SetupGet(x => x.RowDir).Returns(row);
            sliceMock.SetupGet(x => x.ColDir).Returns(col);

            if (rows != 0 && cols != 0 && Math.Abs(pixelX) > 0.0 && Math.Abs(pixelY) > 0.0)
            {
                var ImageInfoMock = new Mock<IImageInfo>();
                ImageInfoMock.SetupGet(x => x.PixelSpacingInRowDirection).Returns(pixelX);
                ImageInfoMock.SetupGet(x => x.PixelSpacingInColDirection).Returns(pixelY);
                ImageInfoMock.SetupGet(x => x.Rows).Returns(rows);
                ImageInfoMock.SetupGet(x => x.Cols).Returns(cols);
            }

            return sliceMock.Object;
        }

        private Vector3D ReadPositionVector(IDataSet dataSet, int frameNumber)
        {
            var frameIndex = frameNumber - 1;
            const Vector3D defaultResult = null;
            if (IsEnhancedMrImage(dataSet))
            {
                dataSet = GetSequenceDataSet(dataSet, new List<KeyValuePair<long, int>>
                {
                    new KeyValuePair<long, int>(LocalTag.Tag.PerFrameFunctionalGroupsSequence, frameIndex), 
                    new KeyValuePair<long, int>(LocalTag.Tag.PlanePositionSequence, 0)
                });
                if (dataSet == null)
                    return defaultResult;
            }
            var tag = LocalTag.Tag.ImagePositionPatient;
            if (!dataSet.Contains(tag) || dataSet[tag].Count < 3)
                return defaultResult;
            return new Vector3D(dataSet[tag][0].CastTo<double>(), dataSet[tag][1].CastTo<double>(), dataSet[tag][2].CastTo<double>());
        }

        private Vector3D ReadRowVector(IDataSet dataSet, int frameNumber)
        {
            var frameIndex = frameNumber - 1;
            const Vector3D defaultResult = null;
            if (IsEnhancedMrImage(dataSet))
            {
                dataSet = GetSequenceDataSet(dataSet, new List<KeyValuePair<long, int>>
                {
                    new KeyValuePair<long, int>(LocalTag.Tag.PerFrameFunctionalGroupsSequence, frameIndex), 
                    new KeyValuePair<long, int>(LocalTag.Tag.PlaneOrientationSequence, 0)
                });
                if (dataSet == null)
                    return defaultResult;
            }
            var tag = LocalTag.Tag.ImageOrientationPatient;
            if (!dataSet.Contains(tag) || dataSet[tag].Count < 3)
                return defaultResult;
            return new Vector3D(dataSet[tag][0].CastTo<double>(), dataSet[tag][1].CastTo<double>(), dataSet[tag][2].CastTo<double>());
        }

        private Vector3D ReadColumnVector(IDataSet dataSet, int frameNumber)
        {
            var frameIndex = frameNumber - 1;
            const Vector3D defaultResult = null;
            if (IsEnhancedMrImage(dataSet))
            {
                dataSet = GetSequenceDataSet(dataSet, new List<KeyValuePair<long, int>>
                {
                    new KeyValuePair<long, int>(LocalTag.Tag.PerFrameFunctionalGroupsSequence, frameIndex), 
                    new KeyValuePair<long, int>(LocalTag.Tag.PlaneOrientationSequence, 0)
                });
                if (dataSet == null)
                    return defaultResult;
            }
            var tag = LocalTag.Tag.ImageOrientationPatient;
            if (!dataSet.Contains(tag) || dataSet[tag].Count < 6)
                return defaultResult;
            return new Vector3D(dataSet[tag][3].CastTo<double>(), dataSet[tag][4].CastTo<double>(), dataSet[tag][5].CastTo<double>());
        }

        private static int ReadRows(IDataSet dataSet)
        {
            var tag = LocalTag.Tag.Rows;
            if (!dataSet.Contains(tag) || dataSet[tag].Count < 1)
                return 0;
            return dataSet[tag][0].CastTo<int>();
        }

        private static int ReadCols(IDataSet dataSet)
        {
            var tag = LocalTag.Tag.Columns;
            if (!dataSet.Contains(tag) || dataSet[tag].Count < 1)
                return 0;
            return dataSet[tag][0].CastTo<int>();
        }

        private double ReadPixelSpacingX(IDataSet dataSet)
        {
            const double defaultResult = 0;
            if (IsEnhancedMrImage(dataSet))
            {
                dataSet = GetSequenceDataSet(dataSet, new List<KeyValuePair<long, int>>
                {
                    new KeyValuePair<long, int>(LocalTag.Tag.SharedFunctionalGroupsSequence, 0), 
                    new KeyValuePair<long, int>(LocalTag.Tag.PixelMeasuresSequence, 0)
                });
                if (dataSet == null)
                    return defaultResult;
            }
            var tag = LocalTag.Tag.PixelSpacing;
            if (!dataSet.Contains(tag) || dataSet[tag].Count < 1)
                return defaultResult;
            //return dataSet[tag][0].CastTo<double>();
            return double.Parse(dataSet[tag][0].As<string>(), CultureInfo.InvariantCulture);
        }

        private double ReaPixelSpacingY(IDataSet dataSet)
        {
            const double defaultResult = 0;
            if (IsEnhancedMrImage(dataSet))
            {
                dataSet = GetSequenceDataSet(dataSet, new List<KeyValuePair<long, int>>
                {
                    new KeyValuePair<long, int>(LocalTag.Tag.SharedFunctionalGroupsSequence, 0), 
                    new KeyValuePair<long, int>(LocalTag.Tag.PixelMeasuresSequence, 0)
                });
                if (dataSet == null)
                    return defaultResult;
            }
            var tag = LocalTag.Tag.PixelSpacing;
            if (!dataSet.Contains(tag) || dataSet[tag].Count < 2)
                return defaultResult;
            //return dataSet[tag][1].CastTo<double>();
            return double.Parse(dataSet[tag][1].As<string>(), CultureInfo.InvariantCulture);
        }

        private static bool IsEnhancedMrImage(IDataSet dataSet)
        {
            var tag = LocalTag.Tag.SopClassUid;
            if (!dataSet.Contains(tag) || dataSet[tag].Count < 1)
                return false;
            return dataSet[tag][0].Equals(SopClassUids.ENHANCED_MR_IMAGE/*SopClass.ToValue(SopClass.Uid.EnhancedMagneticResonanceImage)*/);
        }

        private static IDataSet GetSequenceDataSet(IDataSet dataSet, IList<KeyValuePair<long, int>> sequencePath)
        {
            foreach (var step in sequencePath)
            {
                var tag = step.Key;
                var index = step.Value;
                if (!dataSet.Contains(tag) || dataSet[tag].Count <= index)
                    return null;
                dataSet = dataSet[tag][index].As<IDataSet>();
                if (dataSet == null)
                    return null;
            }
            return dataSet;
        }
        #endregion
    }
}

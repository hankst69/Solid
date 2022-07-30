//----------------------------------------------------------------------------------
// <copyright file="MrDicomAccess.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Solid.Dicom.ImageInfo;
using Solid.Dicom.ImageInfo.Impl;
using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.Math;
using Solid.Infrastructure.RuntimeTypeExtensions;

namespace Solid.Dicom.Impl
{
    /// <summary>
    /// API:NO
    /// MrDicomAccess
    /// </summary>
    public class MrDicomAccess : IMrDicomAccess
    {
        private readonly ITracer m_Tracer;

        public MrDicomAccess(ITracer tracer)
        {
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            m_Tracer = tracer;
        }

        IImageAttributes IMrDicomAccess.CreateImageAttributes(IDicomFrameDataSet dataSet)
        {
            using (m_Tracer.CreateScopeTracer())
            {
                ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
                return new ImageAttributes(this, dataSet, m_Tracer);
            }
        }

        bool IMrDicomAccess.IsEnhancedMrImage(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                return IsEnhancedMrImageDataSet(dataSet);
            }
        }

        //-----------------------------------------------------------------------------------------------------------

        // ensures correct reading of SopClassUid (implementation part of N4_EGA_BildPos_SW_RefbildValid and EGA_AbbFehler_Appl_SW_ModFremd)
        string IMrDicomAccess.GetSopClassUid(IDicomFrameDataSet dataSet)
        {
            return GetDicomValueAsString(dataSet, DicomTags.Tag.SopClassUid, nameof(DicomTags.Tag.SopClassUid));
        }

        string IMrDicomAccess.GetSopInstanceUid(IDicomFrameDataSet dataSet)
        {
            return GetDicomValueAsString(dataSet, DicomTags.Tag.SopInstanceUid, nameof(DicomTags.Tag.SopInstanceUid));
        }

        string IMrDicomAccess.GetSeriesInstanceUid(IDicomFrameDataSet dataSet)
        {
            return GetDicomValueAsString(dataSet, DicomTags.Tag.SeriesInstanceUid, nameof(DicomTags.Tag.SeriesInstanceUid));
        }

        string IMrDicomAccess.GetFrameOfReferenceUid(IDicomFrameDataSet dataSet)
        {
            return GetDicomValueAsString(dataSet, DicomTags.Tag.FrameOfReferenceUid, nameof(DicomTags.Tag.FrameOfReferenceUid));
        }

        // ensures correct reading of Modality (implementation part of EGA_AbbFehler_Appl_SW_ModFremd)
        string IMrDicomAccess.GetModality(IDicomFrameDataSet dataSet)
        {
            return GetDicomValueAsString(dataSet, DicomTags.Tag.Modality, nameof(DicomTags.Tag.Modality));
        }

        // ensures correct reading of ImageType/s (implementation part of EGA_AbbFehler_Appl_SW_ModFremd)
        IEnumerable<string> IMrDicomAccess.GetImageTypes(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (m_Tracer.CreateScopeTracer())
            {
                return GetImageTypesInternal(dataSet).Union(GetFrameTypesInternal(dataSet));
            }
        }

        // ensures correct reading of RelativeTablePosition (implementation part of N4_EGA_BildPos_SW_RefbildValid 059_EGA_SW_SW_NDISRefBildGSP)
        Vector3D IMrDicomAccess.GetImaRelTablePosition(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    const long tag = DicomTags.MrPrivateDicomTags.ImaRelTablePosition;
                    //EnhancedMagneticResonanceImage AccessName="PerFrameFunctionalGroupsSequence.MrImageFrameTypeSequence.ImaRelTablePosition:3"
                    var sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, dataSet.FrameNumber - 1);
                    if (sequenceDataSet != null)
                    {
                        sequenceDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.Tag.MrImageFrameTypeSequence, 0);
                    }
                    if (sequenceDataSet != null)
                    {
                        if (sequenceDataSet.Contains(tag) && sequenceDataSet.ContainsValue(tag) && sequenceDataSet.GetNumberOfValues(tag) > 2)
                        {
                            return new Vector3D(
                                sequenceDataSet.GetValueAt(tag, 0).CastTo<int>(),
                                sequenceDataSet.GetValueAt(tag, 1).CastTo<int>(),
                                sequenceDataSet.GetValueAt(tag, 2).CastTo<int>());
                        }
                    }
                }
                else
                {
                    const long tag = DicomTags.MrPrivateDicomTags.RelTablePosition;
                    //MagneticResonanceImage AccessName="Root.RelTablePosition:3"
                    if (dataSet.Contains(tag) && dataSet.ContainsValue(tag) && dataSet.GetNumberOfValues(tag) > 2)
                    {
                        return new Vector3D(
                            dataSet.GetValueAt(tag, 0).CastTo<int>(),
                            dataSet.GetValueAt(tag, 1).CastTo<int>(),
                            dataSet.GetValueAt(tag, 2).CastTo<int>());
                    }
                }
                ftr.Info("Could not read ImaRelTablePos from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return null;
            }
        }

        DateTime IMrDicomAccess.GetAcquisitionDateTime(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                // (1) EnhancedMagneticResonanceImage:FrameAcquisitionDatetime -> (2) AcquisitionDatetime -> (3) AcquisitionDate+AcquisitionTime
                object dateTimeValue = null;
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    //EnhancedMagneticResonanceImage AccessName="PerFrameFunctionalGroupsSequence.FrameContentSequence.FrameAcquisitionDatetime:1"
                    var sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, dataSet.FrameNumber - 1);
                    if (sequenceDataSet != null)
                    {
                        sequenceDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.Tag.FrameContentSequence, 0);
                        if (sequenceDataSet != null)
                        {
                            const long tag = DicomTags.Tag.FrameAcquisitionDatetime;
                            if (sequenceDataSet.Contains(tag) && sequenceDataSet.ContainsValue(tag) && sequenceDataSet.GetNumberOfValues(tag) > 0)
                            {
                                dateTimeValue = sequenceDataSet.GetValue(tag);
                            }
                        }
                    }
                }
                // try to get Acquisition Date and Time from AcquisitionDatetime if necessary
                if (dateTimeValue == null)
                {
                    const long tag = DicomTags.Tag.AcquisitionDatetime;
                    if (dataSet.Contains(tag) && dataSet.ContainsValue(tag) && dataSet.GetNumberOfValues(tag) > 0)
                    {
                        dateTimeValue = dataSet.GetValue(tag);
                    }
                }
                // convert DICOMDateTime into DateTime
                if (dateTimeValue != null)
                {
                    return DicomValues.ConvertDicomDTtoDateTime(dateTimeValue);
                }

                // neither FrameAcquisitionDatetime nor AcquisitionDatetime were found in frame's dataset
                // read (AcquisitionDate, AcquisitionTime)
                if (dataSet.Contains(DicomTags.Tag.AcquisitionDate) && dataSet.Contains(DicomTags.Tag.AcquisitionTime)
                    && dataSet.ContainsValue(DicomTags.Tag.AcquisitionDate) && dataSet.ContainsValue(DicomTags.Tag.AcquisitionTime))
                {
                    var deDate = dataSet.GetValue(DicomTags.Tag.AcquisitionDate);
                    var deTime = dataSet.GetValue(DicomTags.Tag.AcquisitionTime);

                    return DicomValues.ConvertDicomDAandDicomTMtoDateTime(deDate, deTime);
                }
                ftr.Info("Could not read AcquisitionDateTime from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return DateTime.MinValue;
            }
        }

        DateTime IMrDicomAccess.GetSeriesDateTime(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                if (dataSet.Contains(DicomTags.Tag.SeriesDate) && dataSet.ContainsValue(DicomTags.Tag.SeriesDate))
                {
                    var dicomDa = dataSet.GetValue(DicomTags.Tag.SeriesDate);
                    return DicomValues.ConvertDicomDAtoDateTime(dicomDa);
                }
                ftr.Info("Could not read SeriesDate from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return DateTime.MinValue;
            }
        }

        string IMrDicomAccess.GetStudyDescription(IDicomFrameDataSet dataSet)
        {
            return GetDicomValueAsString(dataSet, DicomTags.Tag.StudyDescription, nameof(DicomTags.Tag.StudyDescription));
        }

        string IMrDicomAccess.GetSeriesDescription(IDicomFrameDataSet dataSet)
        {
            return GetDicomValueAsString(dataSet, DicomTags.Tag.SeriesDescription, nameof(DicomTags.Tag.SeriesDescription));
        }

        int IMrDicomAccess.GetSeriesNumber(IDicomFrameDataSet dataSet)
        {
            return GetDicomIsValueAsInt(dataSet, DicomTags.Tag.SeriesNumber, nameof(DicomTags.Tag.SeriesNumber));
        }

        int IMrDicomAccess.GetInstanceNumber(IDicomFrameDataSet dataSet)
        {
            return GetDicomIsValueAsInt(dataSet, DicomTags.Tag.InstanceNumber, nameof(DicomTags.Tag.InstanceNumber));
        }

        public string GetProtocolName(IDicomFrameDataSet dataSet)
        {
            return GetDicomValueAsString(dataSet, DicomTags.Tag.ProtocolName, nameof(DicomTags.Tag.ProtocolName));
        }

        public string GetSequenceName(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (m_Tracer.CreateScopeTracer())
            {
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    return GetDicomValueAsString(dataSet, DicomTags.Tag.PulseSequenceName, nameof(DicomTags.Tag.PulseSequenceName));
                }
                else
                {
                    return GetDicomValueAsString(dataSet, DicomTags.Tag.SequenceName, nameof(DicomTags.Tag.SequenceName));
                }
            }
        }

        string IMrDicomAccess.GetScanningSequence(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    //EnhancedMagneticResonanceImage AccessName="SharedFunctionalGroupsSequence.SiemensMrSdsSequence.ScanningSequence"
                    const long tag = DicomTags.MrPrivateDicomTags.ScanningSequence;
                    var sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.SharedFunctionalGroupsSequence, 0);
                    if (sequenceDataSet != null)
                    {
                        sequenceDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.MrPrivateDicomTags.SiemensMrSdsSequence, 0);
                    }
                    if (sequenceDataSet != null)
                    {
                        if (sequenceDataSet.Contains(tag) && sequenceDataSet.ContainsValue(tag))
                        {
                            return sequenceDataSet.GetValue(tag).As<string>();
                        }
                    }
                }
                else
                {
                    const long tag = DicomTags.Tag.ScanningSequence;
                    if (dataSet.Contains(tag) && dataSet.ContainsValue(tag))
                    {
                        return dataSet.GetValue(tag).As<string>();
                    }
                }
                ftr.Info("Could not read ScanningSequence from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return string.Empty;
            }
        }

        string IMrDicomAccess.GetSequenceVariant(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    //EnhancedMagneticResonanceImage AccessName="SharedFunctionalGroupsSequence.SiemensMrSdsSequence.SequenceVariant"
                    const long tag = DicomTags.MrPrivateDicomTags.SequenceVariant;
                    var sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.SharedFunctionalGroupsSequence, 0);
                    if (sequenceDataSet != null)
                    {
                        sequenceDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.MrPrivateDicomTags.SiemensMrSdsSequence, 0);
                    }
                    if (sequenceDataSet != null)
                    {
                        if (sequenceDataSet.Contains(tag) && sequenceDataSet.ContainsValue(tag))
                        {
                            return sequenceDataSet.GetValue(tag).As<string>();
                        }
                    }
                }
                else
                {
                    const long tag = DicomTags.Tag.SequenceVariant;
                    if (dataSet.Contains(tag) && dataSet.ContainsValue(tag))
                    {
                        return dataSet.GetValue(tag).As<string>();
                    }
                }
                ftr.Info("Could not read SequenceVariant from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return string.Empty;
            }
        }

        string IMrDicomAccess.GetInPlanePhaseEncodingDirection(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    //EnhancedMagneticResonanceImage AccessName="SharedFunctionalGroupsSequence.MrFovGeometrySequence.InPlanePhaseEncodingDirection"
                    const long tag = DicomTags.Tag.InPlanePhaseEncodingDirection;
                    var sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.SharedFunctionalGroupsSequence, 0);
                    if (sequenceDataSet != null)
                    {
                        sequenceDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.Tag.MrFovGeometrySequence, 0);
                    }
                    if (sequenceDataSet != null)
                    {
                        if (sequenceDataSet.Contains(tag) && sequenceDataSet.ContainsValue(tag))
                        {
                            return sequenceDataSet.GetValue(tag).As<string>();
                        }
                    }
                }
                else
                {
                    const long tag = DicomTags.Tag.InPlanePhaseEncodingDirection;
                    if (dataSet.Contains(tag) && dataSet.ContainsValue(tag))
                    {
                        return dataSet.GetValue(tag).As<string>();
                    }
                }
                ftr.Info("Could not read InPlanePhaseEncodingDirection (optional dicom tag 0018:1312) from frame {0} of image with SopInstanceUid {1})", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return string.Empty;
            }
        }

        string IMrDicomAccess.GetPhaseEncodingDirectionPositive(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                const long tag = DicomTags.MrPrivateDicomTags.PhaseEncodingDirectionPositive;
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    //EnhancedMagneticResonanceImage AccessName="PerFrameFunctionalGroupsSequence.SiemensMrSdiSequence.PhaseEncodingDirectionPositive:3" VR:"IS"
                    var sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, dataSet.FrameNumber - 1);
                    if (sequenceDataSet != null)
                    {
                        sequenceDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.MrPrivateDicomTags.SiemensMrSdiSequence, 0);
                    }
                    if (sequenceDataSet != null)
                    {
                        if (sequenceDataSet.Contains(tag) && sequenceDataSet.ContainsValue(tag))
                        {
                            return sequenceDataSet.GetValue(tag).As<string>();
                        }
                    }
                }
                else
                {
                    if (dataSet.Contains(tag) && dataSet.ContainsValue(tag))
                    {
                        return dataSet.GetValue(tag).As<string>();
                    }
                }
                ftr.Info("Could not read PhaseEncodingDirectionPositive from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return string.Empty;
            }
        }

        double IMrDicomAccess.GetSliceThickness(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    //<DicomSequence Name="PerFrameFunctionalGroupsSequence" AccessName="PerFrameFunctionalGroupsSequence:n" DICOM_ID="52009230" DICOM_TYPE="1" Type="SQ" Multiplicity="1-n">
                    //  <DicomSequence Name="PixelMeasuresSequence" DICOM_ID="00289110" DICOM_TYPE="1" Type="SQ" Multiplicity="1">
                    //    <DicomValue Name="SliceThickness" DICOM_ID="00180050" DICOM_TYPE="1C" Type="DS" Multiplicity="1"/>
                    var sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, dataSet.FrameNumber - 1);
                    if (sequenceDataSet != null)
                    {
                        sequenceDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.Tag.PixelMeasuresSequence, 0);
                    }
                    if (sequenceDataSet != null)
                    {
                        return GetDicomDsValueAsDouble(sequenceDataSet, DicomTags.Tag.SliceThickness, nameof(DicomTags.Tag.SliceThickness));
                    }

                    // old MR data ?
                    //<DicomSequence Name="SharedFunctionalGroupsSequence" DICOM_ID="52009229" DICOM_TYPE="2" Type="SQ" Multiplicity="0-1">
                    //  <DicomSequence Name="PixelMeasuresSequence" DICOM_ID="00289110" DICOM_TYPE="1" Type="SQ" Multiplicity="1">
                    //    <DicomValue Name="SliceThickness" DICOM_ID="00180050" DICOM_TYPE="1C" Type="DS" Multiplicity="1"/>
                    sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.SharedFunctionalGroupsSequence, 0);
                    if (sequenceDataSet != null)
                    {
                        sequenceDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.Tag.PixelMeasuresSequence, 0);
                    }
                    if (sequenceDataSet != null)
                    {
                        return GetDicomDsValueAsDouble(sequenceDataSet, DicomTags.Tag.SliceThickness, nameof(DicomTags.Tag.SliceThickness));
                    }
                }
                else
                {
                    //<DicomValue Name="SliceThickness" DICOM_ID="00180050" DICOM_TYPE="2" Type="DS" Multiplicity="1"/>
                    return GetDicomDsValueAsDouble(dataSet, DicomTags.Tag.SliceThickness, nameof(DicomTags.Tag.SliceThickness));
                }
                ftr.Info("Could not read SliceThickness from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return double.NaN;
            }
        }

        double IMrDicomAccess.GetTimeAfterStart(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                double fallbackWhenTagDoesNotExist = double.NaN;
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    var frameIndex = dataSet.FrameNumber - 1;
                    var sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, frameIndex);
                    if (sequenceDataSet != null)
                    {
                        sequenceDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.MrPrivateDicomTags.SiemensMrSdiSequence, 0);
                    }
                    if (sequenceDataSet != null)
                    {
                        return GetDicomDsValueAsDouble(sequenceDataSet, DicomTags.MrPrivateDicomTags.TimeAfterStart, nameof(DicomTags.MrPrivateDicomTags.TimeAfterStart), fallbackWhenTagDoesNotExist);
                    }
                }
                else
                {
                    return GetDicomDsValueAsDouble(dataSet, DicomTags.MrPrivateDicomTags.TimeAfterStart, nameof(DicomTags.MrPrivateDicomTags.TimeAfterStart), fallbackWhenTagDoesNotExist);
                }
                ftr.Info("Could not read TimeAfterStart from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return fallbackWhenTagDoesNotExist;
            }
        }

        double IMrDicomAccess.GetTriggerTime(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                double fallbackWhenTagDoesNotExist = double.NaN;
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    //g_InfoDic.Add(LocalTag.Tag.TriggerTime, new TagsInfo(LocalTag.Tag.CardiacTriggerDelayTime, LocalTag.Tag.CardiacTriggerSequence));
                    var frameIndex = dataSet.FrameNumber - 1;
                    var sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, frameIndex);
                    if (sequenceDataSet != null)
                    {
                        sequenceDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.Tag.CardiacTriggerSequence, 0);
                    }
                    if (sequenceDataSet != null)
                    {
                        return GetDicomFdValueAsDouble(sequenceDataSet, DicomTags.Tag.CardiacTriggerDelayTime, nameof(DicomTags.Tag.CardiacTriggerDelayTime), fallbackWhenTagDoesNotExist);
                    }
                }
                else
                {
                    return GetDicomDsValueAsDouble(dataSet, DicomTags.Tag.TriggerTime, nameof(DicomTags.Tag.TriggerTime), fallbackWhenTagDoesNotExist);
                }
                ftr.Info("Could not read TriggerTime from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return fallbackWhenTagDoesNotExist;
            }
        }

        string IMrDicomAccess.GetPatientPosition(IDicomFrameDataSet dataSet)
        {
            //<DicomValue Name="PatientPosition" DICOM_ID="00185100" DICOM_TYPE="2C" Type="CS" Multiplicity="1"/>
            return GetDicomValueAsString(dataSet, DicomTags.Tag.PatientPosition, nameof(DicomTags.Tag.PatientPosition));
        }

        public string GetBodyPartExamined(IDicomFrameDataSet dataSet)
        {
            //<DicomValue Name="BodyPartExamined" DICOM_ID="00180015" DICOM_TYPE="3" Type="CS" Multiplicity="1"/>
            return GetDicomValueAsString(dataSet, DicomTags.Tag.BodyPartExamined, nameof(DicomTags.Tag.BodyPartExamined));
        }

        public string GetMrAcquisitionType(IDicomFrameDataSet dataSet)
        {
            //<DicomValue Name="MrAcquisitionType" DICOM_ID="00180023" DICOM_TYPE="2" Type="CS" Multiplicity="1"/>
            return GetDicomValueAsString(dataSet, DicomTags.Tag.MrAcquisitionType, nameof(DicomTags.Tag.MrAcquisitionType));
        }

        public int GetAcquisitionNumber(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                // (1) EnhancedMagneticResonanceImage:FrameAcquisitionNumber(VR:UL) -> (2) AcquisitionNumber(VR:IS)
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    //EnhancedMagneticResonanceImage AccessName="PerFrameFunctionalGroupsSequence.FrameContentSequence.FrameAcquisitionNumber:1"
                    var sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, dataSet.FrameNumber - 1);
                    if (sequenceDataSet != null)
                    {
                        sequenceDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.Tag.FrameContentSequence, 0);
                        if (sequenceDataSet != null)
                        {
                            const long tag = DicomTags.Tag.FrameAcquisitionNumber;
                            if (sequenceDataSet.Contains(tag) && sequenceDataSet.ContainsValue(tag) && sequenceDataSet.GetNumberOfValues(tag) > 0)
                            {
                                var value = sequenceDataSet.GetValue(tag);
                                return DicomValues.ConvertDicomUsToUshort(value);
                            }
                        }
                    }
                }
                // return GetDicomIsValueAsInt(dataSet, DicomTags.Tag.AcquisitionNumber, nameof(DicomTags.Tag.AcquisitionNumber)); 
                const long tag2 = DicomTags.Tag.AcquisitionNumber;
                if (dataSet.Contains(tag2) && dataSet.ContainsValue(tag2))
                {
                    var value = dataSet.GetValue(tag2);
                    if (DicomValues.CanConvertDicomIsToInt(value))
                    {
                        return DicomValues.ConvertDicomIsToInt(value);
                    }
                }
                ftr.Info("Could not read AcquisitionNumber from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return 0;
            }
        }

        // ----- DistortionCorrection related attributes -----

        string IMrDicomAccess.GetDistortionCorrectionType(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                const long tag = DicomTags.MrPrivateDicomTags.DistortionCorrectionType;
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    //EnhancedMagneticResonanceImage AccessName="PerFrameFunctionalGroupsSequence.MrImageFrameTypeSequence.DistortionCorrectionType"
                    var sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, dataSet.FrameNumber - 1);
                    if (sequenceDataSet != null)
                    {
                        sequenceDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.Tag.MrImageFrameTypeSequence, 0);
                    }
                    if (sequenceDataSet != null)
                    {
                        if (sequenceDataSet.Contains(tag) && sequenceDataSet.ContainsValue(tag))
                        {
                            return sequenceDataSet.GetValue(tag).As<string>();
                        }
                    }
                }
                else
                {
                    //MagneticResonanceImage AccessName="Root.DistortionCorrectionType"
                    if (dataSet.Contains(tag) && dataSet.ContainsValue(tag))
                    {
                        return dataSet.GetValue(tag).As<string>();
                    }
                }
                ftr.Info("Could not read DistortionCorrectionType from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return string.Empty;
            }
        }

        string IMrDicomAccess.GetVolumetricProperties(IDicomFrameDataSet dataSet)
        {
            //<DicomValue Name="VolumetricProperties" DICOM_ID="00089206" DICOM_TYPE="1" Type="CS" Multiplicity="1"/>
            return GetDicomValueAsString(dataSet, DicomTags.Tag.VolumetricProperties, "VolumetricProperties");
        }

        string IMrDicomAccess.GetFrameLevelVolumetricProperties(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            //<DicomValue Name="VolumetricProperties" AccessName="PerFrameFunctionalGroupsSequence.MrImageFrameTypeSequence.VolumetricProperties" DICOM_ID="00089206" DICOM_TYPE="1" Type="CS" Multiplicity="1"/>
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                if (!IsEnhancedMrImageDataSet(dataSet))
                {
                    return string.Empty;
                }

                var sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, dataSet.FrameNumber - 1);
                if (sequenceDataSet != null)
                {
                    sequenceDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.Tag.MrImageFrameTypeSequence, 0);
                }
                if (sequenceDataSet != null)
                {
                    const long tag = DicomTags.Tag.VolumetricProperties;
                    if (sequenceDataSet.Contains(tag) && sequenceDataSet.ContainsValue(tag))
                    {
                        return sequenceDataSet.GetValue(tag).As<string>();
                    }
                }
                ftr.Info("Could not read VolumetricProperties from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return string.Empty;
            }
        }

        public string GetGradientCoilName(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            //<DicomValue Name="GradCoilName" AccessName="SharedFunctionalGroupsSequence.SiemensMrSds01Sequence.GradCoilName" DICOM_ID="00089206" DICOM_TYPE="1" Type="SH" Multiplicity="1"/>
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                const long tag = DicomTags.MrPrivateDicomTags.GradientCoilName;
                IDicomDataSet sdsDdataSet = dataSet;
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    var sharedSequence = GetSequenceDataSet(dataSet, DicomTags.Tag.SharedFunctionalGroupsSequence, 0);
                    if (sharedSequence != null)
                    {
                        sdsDdataSet = GetSequenceDataSet(sharedSequence, DicomTags.MrPrivateDicomTags.SiemensMrSdsSequence, 0);
                    }
                }
                if (sdsDdataSet != null && sdsDdataSet.Contains(tag) && sdsDdataSet.ContainsValue(tag))
                {
                    return sdsDdataSet.GetValue(tag).As<string>();
                }
                ftr.Info("Could not read GradCoilName from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return string.Empty;
            }
        }

        // ----- data visualization related attributes -----

        string IMrDicomAccess.GetPresentationStateSopInstanceUid(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                // todo: clarify if there is any separation between EnhancedMr images and SingleDICOM images
                //if (!IsEnhancedMrImage(dataSet))
                //{
                //    throw new NotImplementedException();
                //}
                var sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.ReferencedGrayscalePresentationStateSequence, 0);
                if (sequenceDataSet != null)
                {
                    sequenceDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.Tag.ReferencedSeriesSequence, 0);
                }
                if (sequenceDataSet != null)
                {
                    sequenceDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.Tag.ReferencedSopSequence, 0);
                }
                if (sequenceDataSet != null)
                {
                    const long tag = DicomTags.Tag.ReferencedSopInstanceUid;
                    if (sequenceDataSet.Contains(tag) && sequenceDataSet.ContainsValue(tag) /*&& dataSet.GetNumberOfValues(tag) > 0*/)
                    {
                        return sequenceDataSet.GetValue(tag).As<string>();
                    }
                }
                ftr.Info("Could not read ReferencedSopInstanceUid from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return null;
            }
        }

        public string GetTransferSyntaxUid(IDicomFrameDataSet dataSet)
        {
            return GetDicomValueAsString(dataSet, DicomTags.Tag.TransferSyntaxUid, nameof(DicomTags.Tag.TransferSyntaxUid));
        }

        public string GetLossyImageCompression(IDicomFrameDataSet dataSet)
        {
            return GetDicomValueAsString(dataSet, DicomTags.Tag.LossyImageCompression, nameof(DicomTags.Tag.LossyImageCompression));
        }

        public string GetPhotometricInterpretation(IDicomFrameDataSet dataSet)
        {
            return GetDicomValueAsString(dataSet, DicomTags.Tag.PhotometricInterpretation, nameof(DicomTags.Tag.PhotometricInterpretation));
        }

        public ushort GetSamplesPerPixel(IDicomFrameDataSet dataSet)
        {
            return GetDicomUsValueAsUshort(dataSet, DicomTags.Tag.SamplesPerPixel, nameof(DicomTags.Tag.SamplesPerPixel));
        }

        public ushort GetPlanarConfiguration(IDicomFrameDataSet dataSet)
        {
            return GetDicomUsValueAsUshort(dataSet, DicomTags.Tag.PlanarConfiguration, nameof(DicomTags.Tag.PlanarConfiguration));
        }

        public ushort GetBitsAllocated(IDicomFrameDataSet dataSet)
        {
            return GetDicomUsValueAsUshort(dataSet, DicomTags.Tag.BitsAllocated, nameof(DicomTags.Tag.BitsAllocated));
        }

        public ushort GetBitsStored(IDicomFrameDataSet dataSet)
        {
            return GetDicomUsValueAsUshort(dataSet, DicomTags.Tag.BitsStored, nameof(DicomTags.Tag.BitsStored));
        }

        public ushort GetHighBit(IDicomFrameDataSet dataSet)
        {
            return GetDicomUsValueAsUshort(dataSet, DicomTags.Tag.HighBit, nameof(DicomTags.Tag.HighBit));
        }

        public ushort GetPixelRepresentation(IDicomFrameDataSet dataSet)
        {
            return GetDicomUsValueAsUshort(dataSet, DicomTags.Tag.PixelRepresentation, nameof(DicomTags.Tag.PixelRepresentation));
        }

        public double GetWindowCenter(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                const long tag = DicomTags.Tag.WindowCenter;
                IDicomDataSet pxltrafoDataSet = dataSet;
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    pxltrafoDataSet = null;
                    //EnhancedMagneticResonanceImage AccessName="PerFrameFunctionalGroupsSequence.PixelValueTransformationSequence"
                    var sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, dataSet.FrameNumber - 1);
                    if (sequenceDataSet != null)
                    {
                        pxltrafoDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.Tag.PixelValueTransformationSequence, 0);
                    }
                }
                if (pxltrafoDataSet != null && pxltrafoDataSet.Contains(tag) && pxltrafoDataSet.ContainsValue(tag)) //&& pxltrafoDataSet.GetNumberOfValues(tag) >= 0)
                {
                    var value = pxltrafoDataSet.GetValue(tag);
                    return DicomValues.ConvertDicomDsToDouble(value);
                }
                ftr.Info("Could not read WindowCenter from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return double.NaN;
            }
        }

        public double GetWindowWidth(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                const long tag = DicomTags.Tag.WindowWidth;
                IDicomDataSet pxltrafoDataSet = dataSet;
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    pxltrafoDataSet = null;
                    //EnhancedMagneticResonanceImage AccessName="PerFrameFunctionalGroupsSequence.PixelValueTransformationSequence"
                    var sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, dataSet.FrameNumber - 1);
                    if (sequenceDataSet != null)
                    {
                        pxltrafoDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.Tag.PixelValueTransformationSequence, 0);
                    }
                }
                if (pxltrafoDataSet != null && pxltrafoDataSet.Contains(tag) && pxltrafoDataSet.ContainsValue(tag)) //&& pxltrafoDataSet.GetNumberOfValues(tag) >= 0)
                {
                    var value = pxltrafoDataSet.GetValue(tag);
                    return DicomValues.ConvertDicomDsToDouble(value);
                }
                ftr.Info("Could not read WindowWidth from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return double.NaN;
            }
        }

        public double GetRescaleIntercept(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                const long tag = DicomTags.Tag.RescaleIntercept;
                IDicomDataSet pxltrafoDataSet = dataSet;
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    pxltrafoDataSet = null;
                    //EnhancedMagneticResonanceImage AccessName="PerFrameFunctionalGroupsSequence.PixelValueTransformationSequence"
                    var sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, dataSet.FrameNumber - 1);
                    if (sequenceDataSet != null)
                    {
                        pxltrafoDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.Tag.PixelValueTransformationSequence, 0);
                    }
                }
                if (pxltrafoDataSet != null && pxltrafoDataSet.Contains(tag) && pxltrafoDataSet.ContainsValue(tag)) //&& pxltrafoDataSet.GetNumberOfValues(tag) >= 0)
                {
                    var value = pxltrafoDataSet.GetValue(tag);
                    return DicomValues.ConvertDicomDsToDouble(value);
                }
                ftr.Info("Could not read RescaleIntercept from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return double.NaN;
            }
        }

        public double GetRescaleSlope(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                const long tag = DicomTags.Tag.RescaleSlope;
                IDicomDataSet pxltrafoDataSet = dataSet;
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    pxltrafoDataSet = null;
                    //EnhancedMagneticResonanceImage AccessName="PerFrameFunctionalGroupsSequence.PixelValueTransformationSequence"
                    var sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, dataSet.FrameNumber - 1);
                    if (sequenceDataSet != null)
                    {
                        pxltrafoDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.Tag.PixelValueTransformationSequence, 0);
                    }
                }
                if (pxltrafoDataSet != null && pxltrafoDataSet.Contains(tag) && pxltrafoDataSet.ContainsValue(tag)) //&& pxltrafoDataSet.GetNumberOfValues(tag) >= 0)
                {
                    var value = pxltrafoDataSet.GetValue(tag);
                    return DicomValues.ConvertDicomDsToDouble(value);
                }
                ftr.Info("Could not read RescaleSlope from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return double.NaN;
            }
        }

        double IMrDicomAccess.GetPixelSpacingRow(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                const long tag = DicomTags.Tag.PixelSpacing;
                IDicomDataSet pxlDataSet = dataSet;
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    pxlDataSet = null;
                    //EnhancedMagneticResonanceImage AccessName="SharedFunctionalGroupsSequence.PixelMeasuresSequence"
                    var sharedSequence = GetSequenceDataSet(dataSet, DicomTags.Tag.SharedFunctionalGroupsSequence, 0);
                    if (sharedSequence != null)
                    {
                        pxlDataSet = GetSequenceDataSet(sharedSequence, DicomTags.Tag.PixelMeasuresSequence, 0);
                    }
                    if (pxlDataSet == null)
                    {
                        //EnhancedMagneticResonanceImage AccessName="PerFrameFunctionalGroupsSequence.PixelMeasuresSequence"
                        var frameSequence = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, dataSet.FrameNumber - 1);
                        if (frameSequence != null)
                        {
                            pxlDataSet = GetSequenceDataSet(frameSequence, DicomTags.Tag.PixelMeasuresSequence, 0);
                        }
                    }
                }
                if (pxlDataSet != null && pxlDataSet.Contains(tag) && pxlDataSet.ContainsValue(tag) && pxlDataSet.GetNumberOfValues(tag) >= 2)
                {
                    var spcX = pxlDataSet.GetValueAt(tag, 0);
                    return DicomValues.ConvertDicomDsToDouble(spcX);
                    //var spcY = pxlDataSet.GetValueAt(tag, 1);
                    //return DicomValues.ConvertDicomDsToDouble(spcY);
                }
                ftr.Info("Could not read PixelSpacingRow from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return double.NaN;
            }
        }

        double IMrDicomAccess.GetPixelSpacingCol(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                const long tag = DicomTags.Tag.PixelSpacing;
                IDicomDataSet pxlDataSet = dataSet;
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    pxlDataSet = null;
                    //EnhancedMagneticResonanceImage AccessName="SharedFunctionalGroupsSequence.PixelMeasuresSequence"
                    var sharedSequence = GetSequenceDataSet(dataSet, DicomTags.Tag.SharedFunctionalGroupsSequence, 0);
                    if (sharedSequence != null)
                    {
                        pxlDataSet = GetSequenceDataSet(sharedSequence, DicomTags.Tag.PixelMeasuresSequence, 0);
                    }
                    if (pxlDataSet == null)
                    {
                        //EnhancedMagneticResonanceImage AccessName="PerFrameFunctionalGroupsSequence.PixelMeasuresSequence"
                        var frameSequence = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, dataSet.FrameNumber - 1);
                        if (frameSequence != null)
                        {
                            pxlDataSet = GetSequenceDataSet(frameSequence, DicomTags.Tag.PixelMeasuresSequence, 0);
                        }
                    }
                }
                if (pxlDataSet != null && pxlDataSet.Contains(tag) && pxlDataSet.ContainsValue(tag) && pxlDataSet.GetNumberOfValues(tag) >= 2)
                {
                    //var spcX = pxlDataSet.GetValueAt(tag, 0);
                    //return DicomValues.ConvertDicomDsToDouble(spcX);
                    var spcY = pxlDataSet.GetValueAt(tag, 1);
                    return DicomValues.ConvertDicomDsToDouble(spcY);
                }
                ftr.Info("Could not read PixelSpacingCol from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return double.NaN;
            }
        }

        int IMrDicomAccess.GetNumberOfFrames(IDicomFrameDataSet dataSet)
        {
            return GetDicomIsValueAsInt(dataSet, DicomTags.Tag.NumberOfFrames, nameof(DicomTags.Tag.NumberOfFrames));
        }

        ushort IMrDicomAccess.GetMatrixCols(IDicomFrameDataSet dataSet)
        {
            return GetDicomUsValueAsUshort(dataSet, DicomTags.Tag.Columns, nameof(DicomTags.Tag.Columns));
        }

        ushort IMrDicomAccess.GetMatrixRows(IDicomFrameDataSet dataSet)
        {
            return GetDicomUsValueAsUshort(dataSet, DicomTags.Tag.Rows, nameof(DicomTags.Tag.Rows));
        }

        Vector3D IMrDicomAccess.GetOrientationCol(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                const long tag = DicomTags.Tag.ImageOrientationPatient;
                IDicomDataSet oriDataSet = dataSet;
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    //EnhancedMagneticResonanceImage AccessName="PerFrameFunctionalGroupsSequence.PlaneOrientationSequence"
                    var frameSequence = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, dataSet.FrameNumber - 1);
                    if (frameSequence != null)
                    {
                        oriDataSet = GetSequenceDataSet(frameSequence, DicomTags.Tag.PlaneOrientationSequence, 0);
                    }
                }
                if (oriDataSet != null && oriDataSet.Contains(tag) && oriDataSet.ContainsValue(tag) && oriDataSet.GetNumberOfValues(tag) >= 6)
                {
                    //var oriRowX = oriDataSet.GetValueAt(tag, 0);
                    //var oriRowY = oriDataSet.GetValueAt(tag, 1);
                    //var oriRowZ = oriDataSet.GetValueAt(tag, 2);
                    //return new Vector3D(DicomValues.ConvertDicomDsToDouble(oriRowX), DicomValues.ConvertDicomDsToDouble(oriRowY), DicomValues.ConvertDicomDsToDouble(oriRowZ));
                    var oriColX = oriDataSet.GetValueAt(tag, 3);
                    var oriColY = oriDataSet.GetValueAt(tag, 4);
                    var oriColZ = oriDataSet.GetValueAt(tag, 5);
                    return new Vector3D(DicomValues.ConvertDicomDsToDouble(oriColX), DicomValues.ConvertDicomDsToDouble(oriColY), DicomValues.ConvertDicomDsToDouble(oriColZ));
                }
                ftr.Info("Could not read ImageOrientationCol from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return null;
            }
        }

        Vector3D IMrDicomAccess.GetOrientationRow(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                const long tag = DicomTags.Tag.ImageOrientationPatient;
                IDicomDataSet oriDataSet = dataSet;
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    //EnhancedMagneticResonanceImage AccessName="PerFrameFunctionalGroupsSequence.PlaneOrientationSequence"
                    var frameSequence = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, dataSet.FrameNumber - 1);
                    if (frameSequence != null)
                    {
                        oriDataSet = GetSequenceDataSet(frameSequence, DicomTags.Tag.PlaneOrientationSequence, 0);
                    }
                }
                if (oriDataSet != null && oriDataSet.Contains(tag) && oriDataSet.ContainsValue(tag) && oriDataSet.GetNumberOfValues(tag) >= 6)
                {
                    var oriRowX = oriDataSet.GetValueAt(tag, 0);
                    var oriRowY = oriDataSet.GetValueAt(tag, 1);
                    var oriRowZ = oriDataSet.GetValueAt(tag, 2);
                    return new Vector3D(DicomValues.ConvertDicomDsToDouble(oriRowX), DicomValues.ConvertDicomDsToDouble(oriRowY), DicomValues.ConvertDicomDsToDouble(oriRowZ));
                    //var oriColX = oriDataSet.GetValueAt(tag, 3);
                    //var oriColY = oriDataSet.GetValueAt(tag, 4);
                    //var oriColZ = oriDataSet.GetValueAt(tag, 5);
                    //return new Vector3D(DicomValues.ConvertDicomDsToDouble(oriColX), DicomValues.ConvertDicomDsToDouble(oriColY), DicomValues.ConvertDicomDsToDouble(oriColZ));
                }
                ftr.Info("Could not read ImageOrientationRow from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return null;
            }
        }

        Vector3D IMrDicomAccess.GetImagePosition(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                const long tag = DicomTags.Tag.ImagePositionPatient;
                IDicomDataSet posDataSet = dataSet;
                if (IsEnhancedMrImageDataSet(dataSet))
                {
                    //EnhancedMagneticResonanceImage AccessName="PerFrameFunctionalGroupsSequence.PlanePositionSequence"
                    var frameSequence = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, dataSet.FrameNumber - 1);
                    if (frameSequence != null)
                    {
                        posDataSet = GetSequenceDataSet(frameSequence, DicomTags.Tag.PlanePositionSequence, 0);
                    }
                }
                if (posDataSet != null && posDataSet.Contains(tag) && posDataSet.ContainsValue(tag) && posDataSet.GetNumberOfValues(tag) >= 3)
                {
                    var posX = posDataSet.GetValueAt(tag, 0);
                    var posY = posDataSet.GetValueAt(tag, 1);
                    var posZ = posDataSet.GetValueAt(tag, 2);
                    return new Vector3D(DicomValues.ConvertDicomDsToDouble(posX), DicomValues.ConvertDicomDsToDouble(posY), DicomValues.ConvertDicomDsToDouble(posZ));
                }
                ftr.Info("Could not read ImagePosition from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return null;
            }
        }

        //-----------------------------------------------------------------------------------------------------------

        private IEnumerable<string> GetImageTypesInternal(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                var types = new List<string>();
                const long tag = DicomTags.Tag.ImageType;
                if (dataSet.Contains(tag) && dataSet.ContainsValue(tag))
                {
                    for (int idx = 0; idx < dataSet.GetNumberOfValues(tag); idx++)
                    {
                        types.Add(dataSet.GetValueAt(tag, idx).As<string>());
                    }
                    return types;
                }
                ftr.Info("Could not read ImageType(s) from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return types;
            }
        }

        private IEnumerable<string> GetFrameTypesInternal(IDicomFrameDataSet dataSet)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            using (var ftr = m_Tracer.CreateScopeTracer())
            {
                var types = new List<string>();
                if (!IsEnhancedMrImageDataSet(dataSet))
                {
                    return types;
                }
                var sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, dataSet.FrameNumber - 1);
                if (sequenceDataSet != null)
                {
                    sequenceDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.Tag.MrImageFrameTypeSequence, 0);
                }
                if (sequenceDataSet != null)
                {
                    const long tag = DicomTags.Tag.FrameType;
                    if (sequenceDataSet.Contains(tag) && sequenceDataSet.ContainsValue(tag))
                    {
                        for (int idx = 0; idx < sequenceDataSet.GetNumberOfValues(tag); idx++)
                        {
                            types.Add(sequenceDataSet.GetValueAt(tag, idx).As<string>());
                        }
                    }
                }

                // concatenate values from SiemensPrivate tag ImageTypes4MF (content of ImageType and FrameType can be cropped due to multiplicity 4 limitation)
                sequenceDataSet = GetSequenceDataSet(dataSet, DicomTags.Tag.PerFrameFunctionalGroupsSequence, dataSet.FrameNumber - 1);
                if (sequenceDataSet != null)
                {
                    sequenceDataSet = GetSequenceDataSet(sequenceDataSet, DicomTags.MrPrivateDicomTags.SiemensMrSdiSequence, 0);
                }
                if (sequenceDataSet != null)
                {
                    const long tag = DicomTags.MrPrivateDicomTags.ImageType4MF;
                    if (sequenceDataSet.Contains(tag) && sequenceDataSet.ContainsValue(tag))
                    {
                        for (int idx = 0; idx < sequenceDataSet.GetNumberOfValues(tag); idx++)
                        {
                            types.Add(sequenceDataSet.GetValueAt(tag, idx).As<string>());
                        }
                    }
                }

                if (types.Any())
                {
                    return types;
                }
                ftr.Info("Could not read FrameType(s) from frame {0} of image with SopInstanceUid {1}", dataSet.FrameNumber, dataSet.DataSetSopInstanceUid);
                return types;
            }
        }

        private static bool IsEnhancedMr(string sopClassUid)
        {
            return sopClassUid == DicomTags.MrSopClassUids.ENHANCED_MR_IMAGE
                || sopClassUid == DicomTags.MrSopClassUids.MR_SPECTROSCOPY
                || sopClassUid == DicomTags.MrSopClassUids.ENHANCED_MR_COLORIMAGE
                || sopClassUid == DicomTags.SopClassUids.MultiFrameGrayscaleByteSecondaryCaptureImage
                || sopClassUid == DicomTags.SopClassUids.MultiFrameGrayscaleWordSecondaryCaptureImage
                || sopClassUid == DicomTags.SopClassUids.MultiFrameTrueColorSecondaryCaptureImage;
        }

        private bool IsEnhancedMrImageDataSet(IDicomFrameDataSet dataSet)
        {
            var sopClassUid = dataSet.DataSetSopClassUid;
            if (string.IsNullOrEmpty(sopClassUid))
            {
                sopClassUid = GetDicomValueAsString(dataSet, DicomTags.Tag.SopClassUid, nameof(DicomTags.Tag.SopClassUid));
            }
            return !string.IsNullOrEmpty(sopClassUid) && IsEnhancedMr(sopClassUid);
        }

        private static IDicomDataSet GetSequenceDataSet(IDicomDataSet dataSet, long sequenceTag, int sequenceIndex)
        {
            if (!dataSet.Contains(sequenceTag) || !dataSet.ContainsValue(sequenceTag) || dataSet.GetNumberOfValues(sequenceTag) <= sequenceIndex)
                return null;

            return dataSet.GetItemAt(sequenceTag, sequenceIndex);
        }

        private string GetDicomValueAsString(IDicomFrameDataSet dataSet, long tag, string tagname)
        {
            // currently used for CS, LO, SH and UI values
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            ConsistencyCheck.EnsureArgument(tagname).IsNotNullOrEmpty();
            using (var ftr = m_Tracer.CreateScopeTracer("GetDicomValueAsString_" + tagname))
            {
                if (dataSet.Contains(tag) && dataSet.ContainsValue(tag))
                {
                    var value = dataSet.GetValue(tag).As<string>();
                    return value;
                }
                ftr.Warning("Could not read value for tag: {0}, from image with SopInstanceUid: {1} and FrameNumber {2}", tagname, dataSet.DataSetSopInstanceUid, dataSet.FrameNumber);
                return string.Empty;
            }
        }

        private int GetDicomIsValueAsInt(IDicomFrameDataSet dataSet, long tag, string tagname)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            ConsistencyCheck.EnsureArgument(tagname).IsNotNullOrEmpty();
            using (var ftr = m_Tracer.CreateScopeTracer("GetDicomIsValueAsInt_" + tagname))
            {
                if (dataSet.Contains(tag) && dataSet.ContainsValue(tag))
                {
                    var value = dataSet.GetValue(tag);
                    if (DicomValues.CanConvertDicomIsToInt(value))
                    {
                        return DicomValues.ConvertDicomIsToInt(value);
                    }
                }
                ftr.Warning("Could not read value for tag: {0}, from image with SopInstanceUid {1} and FrameNumber {2}", tagname, dataSet.DataSetSopInstanceUid, dataSet.FrameNumber);
                return 0;
            }
        }

        private ushort GetDicomUsValueAsUshort(IDicomFrameDataSet dataSet, long tag, string tagname)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            ConsistencyCheck.EnsureArgument(tagname).IsNotNullOrEmpty();
            using (var ftr = m_Tracer.CreateScopeTracer("GetDicomUsValueAsUshort_" + tagname))
            {
                if (dataSet.Contains(tag) && dataSet.ContainsValue(tag)) //&& dataSet.GetNumberOfValues(tag) > 0)
                {
                    var value = dataSet.GetValue(tag);
                    return DicomValues.ConvertDicomUsToUshort(value);
                }
                ftr.Warning("Could not read value for tag: {0}, from image with SopInstanceUid {1} and FrameNumber {2}", tagname, dataSet.DataSetSopInstanceUid, dataSet.FrameNumber);
                return 0;
            }
        }

        private double GetDicomDsValueAsDouble(IDicomDataSet dataSet, long tag, string tagname, double valueDoesNotExistDefault = double.NaN)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            ConsistencyCheck.EnsureArgument(tagname).IsNotNullOrEmpty();
            using (var ftr = m_Tracer.CreateScopeTracer("GetDicomDsValueAsDouble_" + tagname))
            {
                if (dataSet.Contains(tag) && dataSet.ContainsValue(tag)) //&& dataSet.GetNumberOfValues(tag) > 0)
                {
                    var dicomDs = dataSet.GetValue(tag);
                    if (dicomDs == null)
                    {
                        ftr.Info("dicomDs == null   (for tag {0} in image with SopInstanceUid {1} and FrameNumber {2})", tag, dataSet.DataSetSopInstanceUid, (dataSet as IDicomFrameDataSet)?.FrameNumber);
                        return valueDoesNotExistDefault;
                    }
                    //if (string.IsNullOrWhiteSpace(dicomDs.As<string>()))
                    //{
                    //    ftr.Info("dicomDs == empty  (for tag {0} in image with SopInstanceUid {1} and FrameNumber {2})", tag, dataSet.DataSetSopInstanceUid, (dataSet as IDicomFrameDataSet)?.FrameNumber);
                    //    return double.NaN;
                    //}
                    return DicomValues.ConvertDicomDsToDouble(dicomDs);
                }
                ftr.Info("There is no value for tag {0} in image with SopInstanceUid {1} and FrameNumber {2}", tag, dataSet.DataSetSopInstanceUid, (dataSet as IDicomFrameDataSet)?.FrameNumber);
                return valueDoesNotExistDefault;
            }
        }

        private double GetDicomFdValueAsDouble(IDicomDataSet dataSet, long tag, string tagname, double valueDoesNotExistDefault = double.NaN)
        {
            ConsistencyCheck.EnsureArgument(dataSet).IsNotNull();
            ConsistencyCheck.EnsureArgument(tagname).IsNotNullOrEmpty();
            using (var ftr = m_Tracer.CreateScopeTracer("GetDicomFdValueAsDouble_" + tagname))
            {
                if (dataSet.Contains(tag) && dataSet.ContainsValue(tag)) //&& dataSet.GetNumberOfValues(tag) > 0)
                {
                    var dicomFd = dataSet.GetValue(tag);
                    if (dicomFd == null)
                    {
                        ftr.Info("dicomFd == null   (for tag {0} in image with SopInstanceUid {1} and FrameNumber {2})", tag, dataSet.DataSetSopInstanceUid, (dataSet as IDicomFrameDataSet)?.FrameNumber);
                        return valueDoesNotExistDefault;
                    }
                    return DicomValues.ConvertDicomFdToDouble(dicomFd);
                }
                ftr.Info("There is no value for tag {0} in image with SopInstanceUid {1} and FrameNumber {2}", tag, dataSet.DataSetSopInstanceUid, (dataSet as IDicomFrameDataSet)?.FrameNumber);
                return valueDoesNotExistDefault;
            }
        }

    }
}

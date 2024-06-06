@echo off
rem plain call
rem Examples.ImageCompareTool.exe  "D:\Data\Dicom\Dicom_MR_testdata\Grid-Sag-BBYN4D13\T1_FL3D_SAG_SPAIR_1+2_RIGHT_0003" "D:\Data\Dicom\Dicom_MR_testdata\Grid-Sag-BBYN4D13\T1_FL3D_SAG_SPAIR_1+2_RIGHT_0004"

rem with waiting for key to exit
Examples.ImageCompareTool.exe --wait  "D:\Data\Dicom\Dicom_MR_testdata\Grid-Sag-BBYN4D13\T1_FL3D_SAG_SPAIR_1+2_RIGHT_0003" "D:\Data\Dicom\Dicom_MR_testdata\Grid-Sag-BBYN4D13\T1_FL3D_SAG_SPAIR_1+2_RIGHT_0004"

rem with tracing to console and file
Examples.ImageCompareTool.exe --traceLevel:all --traceTarget:Console --traceTarget:File#.\ImageCompareTool.log "D:\Data\Dicom\Dicom_MR_testdata\Grid-Sag-BBYN4D13\T1_FL3D_SAG_SPAIR_1+2_RIGHT_0003" "D:\Data\Dicom\Dicom_MR_testdata\Grid-Sag-BBYN4D13\T1_FL3D_SAG_SPAIR_1+2_RIGHT_0004"


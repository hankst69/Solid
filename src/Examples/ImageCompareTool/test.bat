@echo off
rem plain call
rem ImageCompareTool.exe "D:\Data\Dicom\Dicom_MR_testdata\Grid-Sag-BBYN4D13\T1_FL3D_SAG_SPAIR_1+2_RIGHT_0003" "D:\Data\Dicom\Dicom_MR_testdata\Grid-Sag-BBYN4D13\T1_FL3D_SAG_SPAIR_1+2_RIGHT_0004"

rem with waiting for key to exit:
ImageCompareTool.exe --wait --verbose "D:\Data\Dicom\Dicom_MR_testdata\Grid-Sag-BBYN4D13\T1_FL3D_SAG_SPAIR_1+2_RIGHT_0003" "D:\Data\Dicom\Dicom_MR_testdata\Grid-Sag-BBYN4D13\T1_FL3D_SAG_SPAIR_1+2_RIGHT_0004"

rem with waiting for key to exit with filename output:
ImageCompareTool.exe --wait --filenames "D:\Data\Dicom\Dicom_MR_testdata\Grid-Sag-BBYN4D13\T1_FL3D_SAG_SPAIR_1+2_RIGHT_0003" "D:\Data\Dicom\Dicom_MR_testdata\Grid-Sag-BBYN4D13\T1_FL3D_SAG_SPAIR_1+2_RIGHT_0004"

rem with tracing to console and file but without waiting for keypress:
rem ImageCompareTool.exe --traceTarget:Console^|File#.\ImageCompareTool.log --traceLevel:File#all:Console#error^|warning^|debug "D:\Data\Dicom\Dicom_MR_testdata\Grid-Sag-BBYN4D13\T1_FL3D_SAG_SPAIR_1+2_RIGHT_0003" "D:\Data\Dicom\Dicom_MR_testdata\Grid-Sag-BBYN4D13\T1_FL3D_SAG_SPAIR_1+2_RIGHT_0004"
rem ImageCompareTool.exe --traceTarget:Console --traceTarget:File#.\ImageCompareTool.log --traceLevel:File#all --traceLevel:Console#error^|warning^|debug "D:\Data\Dicom\Dicom_MR_testdata\Grid-Sag-BBYN4D13\T1_FL3D_SAG_SPAIR_1+2_RIGHT_0003" "D:\Data\Dicom\Dicom_MR_testdata\Grid-Sag-BBYN4D13\T1_FL3D_SAG_SPAIR_1+2_RIGHT_0004"
ImageCompareTool.exe --traceTarget:Console --traceTarget:File#.\ImageCompareTool.log --traceLevel:File#all --traceLevel:Console#error^|warning^|debug "D:\Data\Dicom\Dicom_MR_testdata\Grid-Sag-BBYN4D13\T1_FL3D_SAG_SPAIR_1+2_RIGHT_0003" "D:\Data\Dicom\Dicom_MR_testdata\Grid-Sag-BBYN4D13\T1_FL3D_SAG_SPAIR_1+2_RIGHT_0004"



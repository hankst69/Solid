if not exist "%MED_BIN%" echo missing med_bin
if not exist "%MED_BIN%" goto :eof
copy /Y "%MED_BIN%\Examples.MeanSquareErrorImageCompare.dll" .\
copy /Y "%MED_BIN%\Examples.ImageCompareTool.exe" .\
copy /Y "%MED_BIN%\Solid.Infrastructure.dll" .\
copy /Y "%MED_BIN%\Solid.Dicom.dll" .\
copy /Y "%MED_BIN%\Solid.DicomAdapters.FoDicom.dll" .\
copy /Y "%MED_BIN%\Solid.DicomAdapters.FoDicom.Impl.dll" .\
copy /Y "%MED_BIN%\fo-dicom.core.dll" .\

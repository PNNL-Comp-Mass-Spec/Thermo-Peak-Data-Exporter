@echo off

xcopy Debug\*.dll C:\DMS_Programs\ThermoPeakDataExporter\ /D /Y
xcopy Debug\*.exe C:\DMS_Programs\ThermoPeakDataExporter\ /D /Y
xcopy ..\Readme.md C:\DMS_Programs\ThermoPeakDataExporter\ /D /Y

xcopy Debug\*.dll \\PNL\projects\OmicsSW\DMS_Programs\AnalysisToolManagerDistribution\ThermoPeakDataExporter\ /D /Y
xcopy Debug\*.exe \\PNL\projects\OmicsSW\DMS_Programs\AnalysisToolManagerDistribution\ThermoPeakDataExporter\ /D /Y
xcopy ..\Readme.md \\PNL\projects\OmicsSW\DMS_Programs\AnalysisToolManagerDistribution\ThermoPeakDataExporter\ /D /Y

pause

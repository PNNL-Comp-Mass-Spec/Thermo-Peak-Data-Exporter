@echo off

xcopy Debug\*.dll C:\DMS_Programs\ThermoPeakDataExporter\ /D /Y
xcopy Debug\*.exe C:\DMS_Programs\ThermoPeakDataExporter\ /D /Y
xcopy ..\..\Readme.md C:\DMS_Programs\ThermoPeakDataExporter\ /D /Y

xcopy Debug\*.dll \\Proto-3\DMS_Programs_Dist\AnalysisToolManagerDistribution\ThermoPeakDataExporter\ /D /Y
xcopy Debug\*.exe \\Proto-3\DMS_Programs_Dist\AnalysisToolManagerDistribution\ThermoPeakDataExporter\ /D /Y
xcopy ..\..\Readme.md \\Proto-3\DMS_Programs_Dist\AnalysisToolManagerDistribution\ThermoPeakDataExporter\ /D /Y

pause

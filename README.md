# ThermoPeakDataExporter

The ThermoPeakDataExporter extracts peak intensity data 
from each scan in a Thermo raw file, and writes that data
to a tab-delimited text file.

## Requirements

ThermoPeakDataExporter reads data from Thermo .raw files using PNNL's 
Thermo Raw File Reader, which is a .NET DLL wrapper for Thermo's MS File Reader.
ThermoPeakDataExporter requires that you download and install MS File Reader from Thermo.
To do this, create an account at https://thermo.flexnetoperations.com/control/thmo/login then 
login and choose `Utility Software`.  Next, click `MS File Reader 3.1 SP2`
then download `MSFileReader_x64.exe`.  SP4 and SP3 are also available and may work,
but we have had some systems where SP2 works while SP4 and SP3 do not work.


## Program Syntax

```
ThermoPeakDataExporter.exe
 ThermoRawFilePath [/O:OutputTSVFilePath]
 [/minInt:MinimumIntensity] [/minRelInt:MinimumRelativeIntensity]
 [/minScan:ScanStart] [/maxScan:ScanEnd]
 [/minMz:MzStart] [/maxMz:MzEnd] 
 [/minSN:MinimumSignalToNoiseRatio]
```

The input file name must end in .raw

The output file name is optional. If omitted, the output file will be created
in the same folder as the input file, but with extension .tsv.

Use /minInt to specify a minimum intensity threshold (absolute value)

Use /minRelInt to specify a minimum relative intensity threshold (value between 0 and 100)

Use /minScan and /maxScan to limit the scan range of the exported data

Use /minMz and /maxMz to limit the m/z range of the exported data

Use /minSN to specify a minimum signal to noise ratio (minimum S/N)

## Example Output File

| Scan Number | RT | Mass | Intensity | Resolution | Baseline | Noise | Charge | SignalToNoise | RelativeIntensity |
|-------------|----|------|-----------|------------|----------|-------|--------|---------------|-------------------|
| 1 | 0.0037 | 212.074924 | 31783.9043 | 1355104 | 3.9968 | 43.2763 | 0 | 734.4418 | 8.2732 |
| 1 | 0.0037 | 212.075111 | 384180.9375 | 1297401 | 3.9968 | 43.2763 | 0 | 8877.3904 | 100 |
| 1 | 0.0037 | 213.078485 | 46274.5391 | 1247004 | 4.1086 | 43.627 | 0 | 1060.6848 | 12.045 |
| 1 | 0.0037 | 214.070926 | 20426.6836 | 1263700 | 4.2191 | 43.9739 | 0 | 464.5181 | 5.3169 |
| 1 | 0.0037 | 671.267394 | 23332.4785 | 366201 | 26.7497 | 82.0103 | 0 | 284.5067 | 6.0733 |
| 1 | 0.0037 | 681.296167 | 75601.5781 | 361001 | 26.9184 | 82.3053 | 0 | 918.5511 | 19.6786 |
| 1 | 0.0037 | 682.299526 | 27141.7754 | 363204 | 26.9352 | 82.3348 | 0 | 329.6515 | 7.0648 |
| 1 | 0.0037 | 733.267545 | 23193.8359 | 337901 | 27.7925 | 83.8339 | 0 | 276.6643 | 6.0372 |
| 2 | 3.0067 | 212.074926 | 31583.8809 | 1322904 | 3.0378 | 41.144 | 0 | 767.6425 | 8.2171 |
| 2 | 3.0067 | 212.07511 | 384368.7813 | 1299001 | 3.0379 | 41.1441 | 0 | 9342.024 | 100 |
| 2 | 3.0067 | 213.078487 | 47960.2578 | 1251504 | 3.1944 | 41.4723 | 0 | 1156.442 | 12.4777 |
| 2 | 3.0067 | 214.070929 | 20854.0293 | 1260400 | 3.3492 | 41.7969 | 0 | 498.9375 | 5.4255 |
| 2 | 3.0067 | 283.264308 | 20913.3262 | 881501 | 11.066 | 55.8397 | 0 | 374.5242 | 5.441 |
| 2 | 3.0067 | 447.134331 | 27278.0703 | 554704 | 18.6603 | 67.9551 | 0 | 401.413 | 7.0968 |
| 2 | 3.0067 | 671.26734 | 22926.4629 | 368101 | 24.8662 | 76.7817 | 0 | 298.5928 | 5.9647 |
| 2 | 3.0067 | 681.29613 | 58137.9414 | 364001 | 25.0193 | 77.0032 | 0 | 755.0072 | 15.1256 |
| 2 | 3.0067 | 682.299488 | 20715.3223 | 366804 | 25.0346 | 77.0253 | 0 | 268.9417 | 5.3894 |
| 2 | 3.0067 | 711.30667 | 22673.0352 | 350101 | 25.4775 | 77.6659 | 0 | 291.9305 | 5.8988 |
| 2 | 3.0067 | 733.267525 | 39559.2852 | 338201 | 25.8128 | 78.1508 | 0 | 506.1915 | 10.292 |

## Contacts

Written by Bryson Gibbons and Matthew Monroe for the Department of Energy (PNNL, Richland, WA) \
E-mail: matthew.monroe@pnnl.gov or proteomics@pnnl.gov \
Website: https://omics.pnl.gov or https://panomics.pnl.gov/

## License

The ThermoPeakDataExporter is licensed under the 2-Clause BSD License; 
you may not use this file except in compliance with the License.  You may obtain 
a copy of the License at https://opensource.org/licenses/BSD-2-Clause

Copyright 2018 Battelle Memorial Institute

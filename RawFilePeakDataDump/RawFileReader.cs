using System;
using System.Collections.Generic;
using System.Linq;
using ThermoRawFileReaderDLL;
using ThermoRawFileReaderDLL.FinniganFileIO;

namespace RawFilePeakDataDump
{
    public class RawFileReader : IDisposable
    {
        private string _filePath;
        private XRawFileIO _rawFile;
        private int _minScan;
        private int _maxScan;

        public RawFileReader(string filePath)
        {
            _filePath = filePath;
            _rawFile = null;
        }

        public void LoadFile()
        {
            _rawFile = new XRawFileIO();
            _rawFile.OpenRawFile(_filePath);
            _minScan = 1;
            _maxScan = _rawFile.GetNumScans();
        }

        public void Close()
        {
            _rawFile.CloseRawFile();
        }

        public void Dispose()
        {
            _rawFile.CloseRawFile();
        }

        public IEnumerable<RawLabelData> GetAllLabelData()
        {
            if (_rawFile == null)
            {
                LoadFile();
            }

            for (var i = _minScan; i <= _maxScan; i++)
            {
                var scan = new RawLabelData()
                {
                    ScanNumber = i,
                    LabelData = GetScanData(i),
                };

                //XRawFileIO.udtMassPrecisionInfoType[] precisionInfo;
                //_rawFile.GetScanPrecisionData(i, out precisionInfo);
                //Console.WriteLine("PrecisionInfoCount: " + precisionInfo.Length);

                if (scan.LabelData == null)
                {
                    continue;
                }

                yield return scan;
            }
        }

        public List<XRawFileIO.udtFTLabelInfoType> GetScanData(int scan)
        {
            clsScanInfo scanInfo;
            if (_rawFile.GetScanInfo(scan, out scanInfo))
            {
                if (scanInfo.IsFTMS)
                {
                    return GetLabelData(scan);
                }
                else
                {
                    return GetPeakData(scan);
                }
            }

            return null;
        }

        public List<XRawFileIO.udtFTLabelInfoType> GetLabelData(int scan)
        {
            XRawFileIO.udtFTLabelInfoType[] labelData;

            _rawFile.GetScanLabelData(scan, out labelData);

            if (labelData.Length > 0)
            {
                return labelData.ToList();
            }

            return null;
        }

        public List<XRawFileIO.udtFTLabelInfoType> GetPeakData(int scan)
        {
            double[,] peakData;

            var dataCount = _rawFile.GetScanData2D(scan, out peakData, 0, true);

            if (peakData.Length > 0)
            {
                var data = new List<XRawFileIO.udtFTLabelInfoType>(dataCount);
                for (int i = 0; i < dataCount; i++)
                {
                    var peak = new XRawFileIO.udtFTLabelInfoType();
                    peak.Mass = peakData[0, i];
                    peak.Intensity = peakData[1, i];
                    data.Add(peak);
                }

                return data;
            }

            return null;
        }
    }
}

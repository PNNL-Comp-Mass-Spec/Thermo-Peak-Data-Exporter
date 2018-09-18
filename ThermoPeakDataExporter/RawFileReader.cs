using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PRISM;
using ThermoRawFileReader;

namespace ThermoPeakDataExporter
{
    public class RawFileReader : clsEventNotifier, IDisposable
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

        public IEnumerable<RawLabelData> GetLabelData(CommandLineOptions options)
        {
            var currentTask = "Initializing";
            try
            {

                if (mRawFileReader == null)
                {
                    currentTask = "Opening the .raw file";
                    LoadFile();
                }

                currentTask = "Validating the scan range";
                if (options.MinScan < ScanMin)
                {
                    options.MinScan = ScanMin;
                }
                if (options.MaxScan > ScanMax || options.MaxScan < 0)
                {
                    options.MaxScan = ScanMax;
                }

            }
            catch (Exception ex)
            {
                OnErrorEvent(string.Format("Exception {0}: {1}", currentTask, ex.Message), ex);
            }

            for (var i = options.MinScan; i <= options.MaxScan; i++)
            {
                var data = GetScanData(i);

                //XRawFileIO.udtMassPrecisionInfoType[] precisionInfo;
                //_rawFile.GetScanPrecisionData(i, out precisionInfo);
                //Console.WriteLine("PrecisionInfoCount: " + precisionInfo.Length);

                if (data == null)
                {
                    continue;
                }

                var maxInt = data.Max(x => x.Intensity);
                var dataFiltered = data.Where(x => x.Intensity >= options.MinIntensityThreshold && x.Intensity / maxInt >= options.MinRelIntensityThresholdPct && options.MinMz <= x.Mass && x.Mass <= options.MaxMz).ToList();

                if (dataFiltered.Count == 0)
                {
                    continue;
                }

                double rt = 0;
                _rawFile.GetRetentionTime(i, out rt);

                yield return new RawLabelData()
                {
                    ScanNumber = i,
                    ScanTime = rt,
                    LabelData = dataFiltered,
                };
            }
        }

        public List<udtFTLabelInfoType> GetScanData(int scan)
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

        public List<udtFTLabelInfoType> GetLabelData(int scan)
        {
            udtFTLabelInfoType[] labelData;

            _rawFile.GetScanLabelData(scan, out labelData);

            if (labelData.Length > 0)
            {
                return labelData.ToList();
            }

            return null;
        }

        public List<udtFTLabelInfoType> GetPeakData(int scan)
        {
            double[,] peakData;

            var dataCount = _rawFile.GetScanData2D(scan, out peakData, 0, true);

            if (peakData.Length > 0)
            {
                var data = new List<udtFTLabelInfoType>(dataCount);
                for (int i = 0; i < dataCount; i++)
                {
                    var peak = new udtFTLabelInfoType();
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

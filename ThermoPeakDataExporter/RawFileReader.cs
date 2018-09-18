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
        private readonly string mFilePath;
        private XRawFileIO mRawFileReader;

        public int ScanMin { get; private set; }

        public int ScanMax { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filePath"></param>
        public RawFileReader(string filePath)
        {
            mFilePath = filePath;
            mRawFileReader = null;
        }

        /// <summary>
        /// Open the raw file with a new instance of XRawFileIO
        /// </summary>
        public void LoadFile()
        {
            mRawFileReader = new XRawFileIO();
            mRawFileReader.OpenRawFile(mFilePath);
            ScanMin = 1;
            ScanMax = mRawFileReader.GetNumScans();
        }

        /// <summary>
        /// Close the raw file reader
        /// </summary>
        public void Close()
        {
            mRawFileReader?.CloseRawFile();
        }

        public void Dispose()
        {
            mRawFileReader?.CloseRawFile();
        }

        /// <summary>
        /// Get the LabelData (if FTMS) or PeakData (if not FTMS) as an enumerable list
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
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

                // XRawFileIO.udtMassPrecisionInfoType[] precisionInfo;
                // mRawFileReader.GetScanPrecisionData(i, out precisionInfo);
                // Console.WriteLine("PrecisionInfoCount: " + precisionInfo.Length);

                if (data == null)
                    continue;

                var maxInt = data.Max(x => x.Intensity);

                // Check for the maximum intensity being zero
                if (Math.Abs(maxInt) < float.Epsilon)
                    continue;

                var dataFiltered = data.Where(x => x.Intensity >= options.MinIntensityThreshold &&
                                                   x.Intensity / maxInt >= options.MinRelIntensityThresholdPct &&
                                                   x.Mass >= options.MinMz &&
                                                   x.Mass <= options.MaxMz).ToList();

                if (dataFiltered.Count == 0)
                    continue;

                mRawFileReader.GetRetentionTime(i, out var rt);

                yield return new RawLabelData
                {
                    ScanNumber = i,
                    ScanTime = rt,
                    MSData = dataFiltered,
                    MaxIntensity = maxInt
                };
            }


        }

        /// <summary>
        /// Get the LabelData (if FTMS) or PeakData (if not FTMS)
        /// </summary>
        /// <param name="scanNumber"></param>
        /// <returns></returns>
        private List<udtFTLabelInfoType> GetScanData(int scanNumber)
        {
            if (!mRawFileReader.GetScanInfo(scanNumber, out clsScanInfo scanInfo))
                return null;

            return scanInfo.IsFTMS ? GetLabelData(scanNumber) : GetPeakData(scanNumber);
        }

        /// <summary>
        /// Get the label data for the given scan
        /// </summary>
        /// <param name="scanNumber"></param>
        /// <returns></returns>
        private List<udtFTLabelInfoType> GetLabelData(int scanNumber)
        {
            mRawFileReader.GetScanLabelData(scanNumber, out var labelData);

            if (labelData.Length > 0)
            {
                return labelData.ToList();
            }

            return null;
        }

        /// <summary>
        /// Get the peak data for the given scan
        /// </summary>
        /// <param name="scanNumber"></param>
        /// <returns></returns>
        private List<udtFTLabelInfoType> GetPeakData(int scanNumber)
        {
            const int MAX_NUMBER_OF_PEAKS = 0;
            const bool CENTROID_DATA = true;

            var dataCount = mRawFileReader.GetScanData2D(scanNumber, out var peakData, MAX_NUMBER_OF_PEAKS, CENTROID_DATA);

            if (peakData.Length <= 0)
            {
                OnWarningEvent(string.Format("GetScanData2D returned no data for scan {0}", scanNumber));
                return null;
            }

            var data = new List<udtFTLabelInfoType>(dataCount);
            for (var i = 0; i < dataCount; i++)
            {
                var peak = new udtFTLabelInfoType
                {
                    Mass = peakData[0, i],
                    Intensity = peakData[1, i]
                };
                data.Add(peak);
            }

            return data;

        }
    }
}

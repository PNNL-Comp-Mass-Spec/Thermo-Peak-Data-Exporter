using System.Collections.Generic;
using ThermoRawFileReader;

namespace ThermoPeakDataExporter
{
    public class RawLabelData
    {
        /// <summary>
        /// Scan number
        /// </summary>
        public int ScanNumber { get; set; }

        /// <summary>
        /// Acquisition time (in minutes)
        /// </summary>
        public double ScanTime { get; set; }

        /// <summary>
        /// Label data (if FTMS), otherwise peak data
        /// </summary>
        public List<udtFTLabelInfoType> MSData { get; set; }

        /// <summary>
        /// Maximum intensity of the peaks in this scan
        /// </summary>
        public double MaxIntensity { get; set; }
    }
}

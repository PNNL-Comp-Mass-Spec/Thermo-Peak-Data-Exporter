﻿using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper.Configuration;

namespace ThermoPeakDataExporter
{
    public class ScanPeakData
    {
        /// <summary>
        /// Scan number
        /// </summary>
        public int ScanNumber { get; set; }

        /// <summary>
        /// Acquisition time (min minutes)
        /// </summary>
        public double ScanTime { get; set; }

        /// <summary>Peak m/z</summary>
        /// <remarks>This is observed m/z; it is not monoisotopic mass</remarks>
        public double Mass { get; set; }

        /// <summary>
        /// Peak Intensity
        /// </summary>
        public double Intensity { get; set; }

        /// <summary>
        /// Peak Resolution
        /// </summary>
        public double Resolution { get; set; }

        /// <summary>
        /// Peak Baseline
        /// </summary>
        public double Baseline { get; set; }

        /// <summary>
        /// Peak Noise
        /// </summary>
        /// <remarks>For signal/noise ratio, see SignalToNoise</remarks>
        public double Noise { get; set; }

        /// <summary>
        /// Peak Charge
        /// </summary>
        /// <remarks>Will be 0 if the charge could not be determined</remarks>
        public double Charge { get; set; }

        /// <summary>
        /// Signal to noise ratio
        /// </summary>
        /// <returns>Intensity divided by noise, or 0 if Noise is 0</returns>
        public double SignalToNoise { get; set; }

        /// <summary>
        /// Relative intensity (value between 0 and 100)
        /// </summary>
        /// <returns>Intensity of a peak divided by maximum intensity (in a given scan) times 100</returns>
        public double RelativeIntensity { get; set; }

        /// <summary>
        /// Sort label data by mass, then by intensity
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerable<ScanPeakData> ConvertOrdered(RawLabelData data)
        {
            double maxIntensity;

            if (Math.Abs(data.MaxIntensity) < float.Epsilon)
                maxIntensity = 1;
            else
                maxIntensity = data.MaxIntensity;

            foreach (var peak in data.MSData.OrderBy(x => x.Mass).ThenBy(x => x.Intensity))
            {
                yield return new ScanPeakData
                {
                    ScanNumber = data.ScanNumber,
                    ScanTime = data.ScanTime,
                    Mass = peak.Mass,
                    Intensity = peak.Intensity,
                    Resolution = peak.Resolution,
                    Baseline = peak.Baseline,
                    Noise = peak.Noise,
                    SignalToNoise = peak.SignalToNoise,
                    RelativeIntensity = peak.Intensity / maxIntensity * 100
                };
            }
        }

        public sealed class ScanPeakDataClassMap : ClassMap<ScanPeakData>
        {
            public ScanPeakDataClassMap()
            {
                Map(x => x.ScanNumber).Name("Scan Number");
                Map(x => x.ScanTime).Name("RT").TypeConverter(new DoubleConverter(4));
                Map(x => x.Mass).Name("Mass").TypeConverter(new DoubleConverter(6));
                Map(x => x.Intensity).Name("Intensity").TypeConverter(new DoubleConverter(4));
                Map(x => x.Resolution).Name("Resolution");
                Map(x => x.Baseline).Name("Baseline").TypeConverter(new DoubleConverter(4));
                Map(x => x.Noise).Name("Noise").TypeConverter(new DoubleConverter(4));
                Map(x => x.Charge).Name("Charge");
                Map(x => x.SignalToNoise).Name("SignalToNoise").TypeConverter(new DoubleConverter(4));
                Map(x => x.RelativeIntensity).Name("RelativeIntensity").TypeConverter(new DoubleConverter(4));
            }
        }
    }
}

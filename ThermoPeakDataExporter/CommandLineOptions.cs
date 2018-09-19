using System;
using System.IO;
using System.Reflection;
using PRISM;

namespace ThermoPeakDataExporter
{
    public class CommandLineOptions
    {
        private const string PROGRAM_DATE = "September 17, 2018";

        private const int DEFAULT_MAX_MZ = 10000000;

        /// <summary>
        /// Constructor
        /// </summary>
        public CommandLineOptions()
        {
            MinIntensityThreshold = 0;
            MinRelIntensityThreshold = 0;
            MinScan = -1;
            MaxScan = -1;
            MinMz = 0;
            MaxMz = DEFAULT_MAX_MZ;
            SignalToNoiseThreshold = 0;
        }

        [Option("raw", "i", ArgPosition = 1, Required = true, HelpText = "Path to .raw file", HelpShowsDefault = false)]
        public string RawFilePath { get; set; }

        [Option("tsv", "out", "o", ArgPosition = 2, HelpText = "Name/path of output file (Default: raw_file_name.tsv", HelpShowsDefault = false)]
        public string OutputPath { get; set; }

        [Option("minInt", "minIntensity", HelpText = "Minimum intensity threshold (absolute value)")]
        public double MinIntensityThreshold { get; set; }

        [Option("minRelInt", "minRelIntensity", HelpText = "Minimum relative intensity threshold (value between 0 and 99)", Min = 0, Max = 99)]
        public double MinRelIntensityThreshold { get; set; }

        /// <summary>
        /// Relative intensity threshold (value between 0 and 1)
        /// </summary>
        public double MinRelIntensityThresholdRatio { get; private set; }

        [Option("minScan", HelpText = "First scan to output")]
        public int MinScan { get; set; }

        [Option("maxScan", HelpText = "Last scan to output")]
        public int MaxScan { get; set; }

        [Option("minMz", HelpText = "Lowest m/z to output")]
        public double MinMz { get; set; }

        [Option("maxMz", HelpText = "Highest m/z to output")]
        public double MaxMz { get; set; }

        [Option("minSN", "minSignalToNoise", HelpText = "Minimum S/N ratio")]
        public double SignalToNoiseThreshold { get; set; }

        public static string GetAppVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version + " (" + PROGRAM_DATE + ")";

            return version;
        }

        public bool Validate()
        {

            if (string.IsNullOrWhiteSpace(RawFilePath))
            {
                ConsoleMsgUtils.ShowError("Raw file path is not defined");
                return false;
            }

            if (!File.Exists(RawFilePath))
            {
                ConsoleMsgUtils.ShowError(string.Format("ERROR: raw file \"{0}\" does not exist.", RawFilePath));
                return false;
            }
            if (!RawFilePath.ToLower().EndsWith(".raw"))
            {
                ConsoleMsgUtils.ShowError(string.Format("ERROR: file \"{0}\" is not a raw file.", RawFilePath));
                return false;
            }

            if (string.IsNullOrWhiteSpace(OutputPath))
            {
                OutputPath = Path.ChangeExtension(RawFilePath, ".tsv");
            }

            if (MinScan > MaxScan)
            {
                Console.WriteLine("ERROR: minScan cannot be greater than maxScan!, {0} > {1}", MinScan, MaxScan);
                return false;
            }

            if (MinMz > MaxMz)
            {
                Console.WriteLine("ERROR: minMz cannot be greater than maxMz!, {0} > {1}", MinMz, MaxMz);
                return false;
            }

            MinRelIntensityThresholdRatio = MinRelIntensityThreshold / 100d;

            return true;
        }
    }
}

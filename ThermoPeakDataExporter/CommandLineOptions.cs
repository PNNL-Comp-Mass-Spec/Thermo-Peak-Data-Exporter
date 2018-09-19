using System;
using System.IO;
using System.Reflection;
using PRISM;

namespace ThermoPeakDataExporter
{
    public class CommandLineOptions
    {
        private const string PROGRAM_DATE = "September 18, 2018";

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

        public void OutputSetOptions()
        {
            Console.WriteLine("ThermoPeakDataExporter, version " + GetAppVersion());
            Console.WriteLine();
            Console.WriteLine("Using options:");

            Console.WriteLine(" Thermo Instrument file: {0}", RawFilePath);
            Console.WriteLine(" Output file: {0}", OutputPath);

            if (MinIntensityThreshold > 0)
                Console.WriteLine(" Minimum Intensity: {0:F1}", MinIntensityThreshold);

            if (MinRelIntensityThreshold > 0)
                Console.WriteLine(" Minimum Relative Intensity: {0:F2}%", MinRelIntensityThreshold);

            if (MinScan > -1)
                Console.WriteLine(" Minimum Scan: {0}", MinScan);

            if (MaxScan > -1)
                Console.WriteLine(" Maximum Scan: {0}", MaxScan);

            if (MinMz > 0)
                Console.WriteLine(" Minimum m/z: {0}", MinMz);

            if (MaxMz < DEFAULT_MAX_MZ)
                Console.WriteLine(" Maximum m/z: {0}", MaxMz);

            if (SignalToNoiseThreshold > 0)
                Console.WriteLine(" Minimum S/N: {0:F1}", SignalToNoiseThreshold);

            Console.WriteLine();
        }

        public bool ValidateArgs()
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

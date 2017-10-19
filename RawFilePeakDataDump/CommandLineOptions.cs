using System;
using System.IO;
using PRISM;

namespace RawFilePeakDataDump
{
    public class CommandLineOptions
    {
        [Option("raw", ArgPosition = 1, Required = true, HelpText = "Path to .raw file", HelpShowsDefault = false)]
        public string RawFilePath { get; set; }

        [Option("tsv", ArgPosition = 2, HelpText = "Name/path of output file (Default: raw_file_name.tsv", HelpShowsDefault = false)]
        public string OutputPath { get; set; }

        [Option("minInt", "minIntensity", HelpText = "Minimum intensity threshold")]
        public double MinIntensityThreshold { get; set; }

        [Option("minRelInt", "minRelIntensity", HelpText = "Minimum relative intensity threshold", Min = 0, Max = 99)]
        public double MinRelIntensityThreshold { get; set; }

        public double MinRelIntensityThresholdPct { get; private set; }

        [Option("minScan", HelpText = "First scan to output")]
        public int MinScan { get; set; }

        [Option("maxScan", HelpText = "Last scan to output")]
        public int MaxScan { get; set; }

        [Option("minMz", HelpText = "Lowest m/z to output")]
        public double MinMz { get; set; }

        [Option("maxMz", HelpText = "Highest m/z to output")]
        public double MaxMz { get; set; }

        public CommandLineOptions()
        {
            MinIntensityThreshold = 0;
            MinRelIntensityThreshold = 0;
            MinScan = -1;
            MaxScan = -1;
            MinMz = 0;
            MaxMz = 10000000;
        }

        public bool Validate()
        {
            if (!File.Exists(RawFilePath))
            {
                Console.WriteLine("ERROR: raw file \"{0}\" does not exist.", RawFilePath);
                return false;
            }
            if (!RawFilePath.ToLower().EndsWith(".raw"))
            {
                Console.WriteLine("ERROR: file \"{0}\" is not a raw file.", RawFilePath);
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

            MinRelIntensityThresholdPct = MinRelIntensityThreshold / 100d;

            return true;
        }
    }
}

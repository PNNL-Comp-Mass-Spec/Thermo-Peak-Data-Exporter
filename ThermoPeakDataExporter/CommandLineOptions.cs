using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PRISM;

namespace ThermoPeakDataExporter
{
    public class CommandLineOptions
    {
        // Ignore Spelling: tsv, wildcards

        private const string PROGRAM_DATE = "October 25, 2021";

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
            LoadMethodInfo = true;
            LoadTuneData = true;
        }

        [Option("InputFile", "raw", "i", ArgPosition = 1, Required = true, IsInputFilePath = true,
            HelpText = "Path to a Thermo .raw file; supports wildcards.\n" +
                       "Can alternatively be a path to a directory with .raw files.", HelpShowsDefault = false)]
        public string RawFilePath { get; set; }

        [Option("OutputFile", "tsv", "out", "o", ArgPosition = 2,
            HelpText = "Name/path of the output file (Default: raw_file_name.tsv).\n" +
                       "If processing a single file, this can alternatively be a path to an existing directory.", HelpShowsDefault = false)]
        public string OutputPath { get; set; }

        [Option("Recurse", "R", HelpText = "If specified, also searches subdirectories for .raw files")]
        public bool Recurse { get; set; }

        [Option("MinIntensity", "MinInt", HelpText = "Minimum intensity threshold (absolute value)")]
        public double MinIntensityThreshold { get; set; }

        [Option("MinRelIntensity", "MinRelInt", HelpText = "Minimum relative intensity threshold (value between 0 and 99)", Min = 0, Max = 99)]
        public double MinRelIntensityThreshold { get; set; }

        /// <summary>
        /// Relative intensity threshold (value between 0 and 1)
        /// </summary>
        public double MinRelIntensityThresholdRatio { get; private set; }

        [Option("MinScan", HelpText = "First scan to output")]
        public int MinScan { get; set; }

        [Option("MaxScan", HelpText = "Last scan to output")]
        public int MaxScan { get; set; }

        [Option("MinMz", HelpText = "Lowest m/z to output")]
        public double MinMz { get; set; }

        [Option("MaxMz", HelpText = "Highest m/z to output")]
        public double MaxMz { get; set; }

        [Option("MinSignalToNoise", "MinSN", HelpText = "Minimum S/N ratio")]
        public double SignalToNoiseThreshold { get; set; }

        [Option("LoadMethodInfo", "LoadMethod", HelpText = "Controls whether the instrument method is read")]
        public bool LoadMethodInfo { get; set; }

        [Option("LoadTuneData", "LoadTune", HelpText = "Controls whether tune data is read")]
        public bool LoadTuneData { get; set; }

        public List<string> FilePaths { get; } = new();

        public static string GetAppVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version + " (" + PROGRAM_DATE + ")";

            return version;
        }

        public void OutputSetOptions(string inputFilePath, string outputFilePath)
        {
            Console.WriteLine("ThermoPeakDataExporter, version " + GetAppVersion());
            Console.WriteLine();
            Console.WriteLine("Using options:");

            Console.WriteLine(" Thermo Instrument file: {0}", inputFilePath);
            Console.WriteLine(" Output file: {0}", outputFilePath);

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

            if (!LoadMethodInfo)
                Console.WriteLine(" Will not load method info from the .raw file");

            if (!LoadTuneData)
                Console.WriteLine(" Will not load tune data from the .raw file");

            Console.WriteLine();
        }

        public string GetOutputPath(string inputPath)
        {
            if (FilePaths.Count == 1 && !string.IsNullOrWhiteSpace(OutputPath))
            {
                var directoryToCheck = new DirectoryInfo(OutputPath);
                if (directoryToCheck.Exists)
                {
                    // Output path is a directory
                    return Path.Combine(directoryToCheck.FullName, Path.GetFileNameWithoutExtension(inputPath) + ".tsv");
                }

                // Assume the output path is a file
                return OutputPath;
            }

            // Create the tsv file in the same directory as the input file
            return Path.ChangeExtension(inputPath, "tsv");
        }

        public bool ValidateArgs(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(RawFilePath))
            {
                errorMessage = "Raw file path is not defined";
                return false;
            }

            var directories = new List<string>();
            if (RawFilePath.Contains("*") || RawFilePath.Contains("?"))
            {
                // Split the path according to the portions that have wildcards
                // add matching files to FilePaths, matching directories to directories
                var matches = ProcessWildCardPath(RawFilePath, ".raw");
                foreach (var match in matches)
                {
                    if (File.Exists(match))
                    {
                        FilePaths.Add(match);
                    }
                    else if (Directory.Exists(match))
                    {
                        directories.Add(match);
                    }
                }
            }
            else if (Directory.Exists(RawFilePath))
            {
                directories.Add(RawFilePath);
            }
            else if (File.Exists(RawFilePath))
            {
                if (!RawFilePath.EndsWith(".raw", StringComparison.OrdinalIgnoreCase))
                {
                    errorMessage = string.Format("ERROR: file \"{0}\" is not a raw file.", RawFilePath);
                    return false;
                }

                FilePaths.Add(RawFilePath);
            }
            else
            {
                errorMessage = string.Format("ERROR: raw file \"{0}\" does not exist.", RawFilePath);
                return false;
            }

            var searchOption = Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var directory in directories)
            {
                FilePaths.AddRange(Directory.GetFiles(directory, "*.raw", searchOption));
            }

            if (FilePaths.Count == 0)
            {
                errorMessage = "ERROR: No raw files found in the provided path.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(OutputPath))
            {
                OutputPath = Path.ChangeExtension(RawFilePath, ".tsv");
            }

            if (MinScan > MaxScan)
            {
                errorMessage = string.Format("ERROR: minScan cannot be greater than maxScan!, {0} > {1}", MinScan, MaxScan);
                return false;
            }

            if (MinMz > MaxMz)
            {
                errorMessage = string.Format("ERROR: minMz cannot be greater than maxMz!, {0} > {1}", MinMz, MaxMz);
                return false;
            }

            MinRelIntensityThresholdRatio = MinRelIntensityThreshold / 100d;

            errorMessage = string.Empty;
            return true;
        }

        public static List<string> ProcessWildCardPath(string wildCardPath, string requiredFileExtension)
        {
            var results = new List<string>();
            // split the path according to the portions that have wildcards
            var split = wildCardPath.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
            var basePath = ".";
            if (!split[0].Contains("*") && !split[0].Contains("?"))
            {
                var pathParts = new List<string>();
                foreach (var part in split)
                {
                    if (part.Contains("*") || part.Contains("?"))
                    {
                        break;
                    }
                    pathParts.Add(part);
                }

                basePath = Path.Combine(pathParts.ToArray());
                split = split.Skip(pathParts.Count).ToArray();
            }

            results.AddRange(ProcessWildCardPathSplit(basePath, split, requiredFileExtension));

            return results;
        }

        private static IEnumerable<string> ProcessWildCardPathSplit(string basePath, IReadOnlyList<string> parts, string requiredFileExtension)
        {
            var results = new List<string>();

            if (parts.Count == 0)
            {
                if ((File.Exists(basePath) && basePath.EndsWith(requiredFileExtension, StringComparison.OrdinalIgnoreCase))
                    || Directory.Exists(basePath))
                {
                    results.Add(basePath);
                }
                return results;
            }

            // process only the current part, pass following parts to recursive call

            var part = parts[0];
            if (!part.Contains("*") && !part.Contains("?"))
            {
                results.AddRange(ProcessWildCardPathSplit(Path.Combine(basePath, part), parts.Skip(1).ToArray(), requiredFileExtension));
                return results;
            }

            if (parts.Count == 1)
            {
                if (part.EndsWith(requiredFileExtension, StringComparison.OrdinalIgnoreCase))
                {
                    results.AddRange(Directory.GetFiles(basePath, part));
                }
                else
                {
                    results.AddRange(Directory.GetDirectories(basePath, part));
                }

                return results;
            }

            var subParts = parts.Skip(1).ToArray();
            foreach (var path in Directory.GetDirectories(basePath, part))
            {
                results.AddRange(ProcessWildCardPathSplit(Path.Combine(basePath, path), subParts, requiredFileExtension));
            }

            return results;
        }
    }
}

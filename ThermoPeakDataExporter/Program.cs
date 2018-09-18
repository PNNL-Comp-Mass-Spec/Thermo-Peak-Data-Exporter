﻿using System;
using System.IO;
using System.Reflection;
using System.Threading;
using PRISM;
using ThermoPeakDataExporter;

namespace ThermoPeakDataExporter
{
    public class Program
    {
        private static DateTime mLastProgress;

        /// <summary>
        /// Program entry method
        /// </summary>
        /// <param name="args"></param>
        /// <returns>0 if success, otherwise an error code</returns>
        static int Main(string[] args)
        {
            var currentTask = "initializing";

            try
            {
                var asmName = typeof(Program).GetTypeInfo().Assembly.GetName();
                var exeName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
                var version = CommandLineOptions.GetAppVersion();


                var parser = new CommandLineParser<CommandLineOptions>(asmName.Name, version)
                {
                    ProgramInfo = "This program extracts peak intensity data from a Thermo raw file, " +
                                  "writing the mass and intensity information to a tab delimited text file (default extension .tsv).",

                    ContactInfo = "Program written by Bryson Gibbons for the Department of Energy (PNNL, Richland, WA) in 2016" +
                                  Environment.NewLine + Environment.NewLine +
                                  "E-mail: proteomics@pnnl.gov" + Environment.NewLine +
                                  "Website: https://omics.pnl.gov or https://panomics.pnnl.gov or https://github.com/PNNL-Comp-Mass-Spec",

                    UsageExamples = {
                        exeName + " Dataset.raw",
                        exeName + " Dataset.raw /tsv:DatasetPeaks.tsv",
                        exeName + " Dataset.raw /minRelInt:10"
                    }
                };


                var parseResults = parser.ParseArgs(args);
                var options = parseResults.ParsedResults;

                if (!parseResults.Success)
                {
                    Thread.Sleep(1500);
                    return -1;
                }

                // Validate the options, including verifying that the .raw file exists
                if (!options.Validate())
                {
                    parser.PrintHelp();
                    Thread.Sleep(1500);
                    return -1;
                }

                currentTask = "validating paths";

                var outputFile = new FileInfo(options.OutputPath);
                if (outputFile.Directory == null)
                {
                    ShowErrorMessage("Unable to determine the parent directory of the output file, " + options.OutputPath);
                    return -3;
                }

                if (!outputFile.Directory.Exists)
                {
                    ShowWarningMessage("Output directory does not exist; will try to create " + outputFile.Directory.FullName);
                    outputFile.Directory.Create();
                }

                currentTask = "instantiating RawFileReader and ScanPeakDataWriter";
                mLastProgress = DateTime.UtcNow;

                Console.WriteLine("Extracting data from " + options.RawFilePath);

                using (var rawReader = new RawFileReader(options.RawFilePath))
                using (var tsvWriter = new ScanPeakDataWriter(options.OutputPath))
                {
                    RegisterEvents(rawReader);
                    RegisterEvents(tsvWriter);

                    currentTask = "opening the .raw file";
                    rawReader.LoadFile();

                    var scanCount = rawReader.ScanMax;

                    currentTask = "reading/writing peaks for each scan";

                    // This method uses a yield return IEnumerable
                    // Thus, memory usage is minimal and each scan is written right after it is read
                    var data = rawReader.GetLabelData(options);

                    var success = tsvWriter.Write(data, scanCount);

                    if (success)
                    {
                        Console.WriteLine("Processing complete; created file " + options.OutputPath);
                        return 0;
                    }

                    ShowWarningMessage("Writer reports false indicating an error occurred");
                    return -4;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(string.Format("Exception {0}: {1}", currentTask, ex.Message), ex);
                return -10;
            }

        }

        private static void RegisterEvents(clsEventNotifier processingClass)
        {
            processingClass.ProgressUpdate += ProcessingClass_ProgressUpdate;
            processingClass.ErrorEvent += ProcessingClass_ErrorEvent;
            processingClass.WarningEvent += ProcessingClass_WarningEvent;
        }

        private static void ProcessingClass_ProgressUpdate(string progressMessage, float percentComplete)
        {
            if (DateTime.UtcNow.Subtract(mLastProgress).TotalSeconds < 1)
                return;

            mLastProgress = DateTime.UtcNow;

            ConsoleMsgUtils.ShowDebug(string.Format("{0:F1}%: {1}", percentComplete, progressMessage));
        }

        private static void ProcessingClass_ErrorEvent(string message, Exception ex)
        {
            ShowErrorMessage(message, ex);
        }

        private static void ProcessingClass_WarningEvent(string message)
        {
            ShowWarningMessage(message);
        }

        private static void ShowErrorMessage(string message, Exception ex = null)
        {
            ConsoleMsgUtils.ShowError(message, ex);
        }

        private static void ShowWarningMessage(string message)
        {
            ConsoleMsgUtils.ShowWarning(message);
        }
    }
}
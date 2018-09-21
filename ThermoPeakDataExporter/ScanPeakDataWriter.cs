using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using PRISM;

namespace ThermoPeakDataExporter
{
    public class ScanPeakDataWriter : EventNotifier, IDisposable
    {
        /// <summary>
        /// TSV file writer
        /// </summary>
        private readonly CsvWriter mWriter;

        /// <summary>
        /// Output file path
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="outputPath"></param>
        public ScanPeakDataWriter(string outputPath)
        {
            FilePath = outputPath;
            mWriter = new CsvWriter(new StreamWriter(new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)));
            mWriter.Configuration.RegisterClassMap<ScanPeakData.ScanPeakDataClassMap>();
            mWriter.Configuration.Delimiter = "\t";
        }

        public void Dispose()
        {
            mWriter?.Dispose();
        }

        /// <summary>
        /// Append an enumerable list of ScanPeakData
        /// </summary>
        /// <param name="data"></param>
        private void Write(IEnumerable<ScanPeakData> data)
        {
            mWriter.WriteRecords(data);
        }

        /// <summary>
        /// Append data for a single scan
        /// </summary>
        /// <param name="data"></param>
        private void Write(RawLabelData data)
        {
            Write(ScanPeakData.ConvertOrdered(data));
        }

        /// <summary>
        /// Append an enumerable list of RawLabelData
        /// </summary>
        /// <param name="data">Enumerable list of RawLabelData</param>
        /// <param name="scanCount">Number of scans in the .raw file; used to report progress</param>
        public bool Write(IEnumerable<RawLabelData> data, int scanCount)
        {
            var currentScanNumber = 0;

            try
            {

                foreach (var scan in data)
                {
                    currentScanNumber = scan.ScanNumber;
                    Write(scan);

                    if (scanCount > 0 && currentScanNumber % 100 == 0)
                    {
                        var percentComplete = currentScanNumber / (float)scanCount * 100;
                        OnProgressUpdate("Processing scan " + currentScanNumber, percentComplete);
                    }
                }

                mWriter.Flush();
                return true;
            }
            catch(Exception ex)
            {
                OnErrorEvent(string.Format("Exception processing scan {0}: {1}", currentScanNumber, ex.Message));
                OnWarningEvent(StackTraceFormatter.GetExceptionStackTraceMultiLine(ex));
                return false;
            }

        }
    }
}

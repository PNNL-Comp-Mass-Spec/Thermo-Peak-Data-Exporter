using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;

namespace ThermoPeakDataExporter
{
    public class ScanPeakDataWriter : clsEventNotifier, IDisposable
    {
        private CsvWriter writer;
        public string FilePath { get; private set; }

        public ScanPeakDataWriter(string outputPath)
        {
            FilePath = outputPath;
            writer = new CsvWriter(new StreamWriter(new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)));
            writer.Configuration.RegisterClassMap<ScanPeakData.ScanPeakDataClassMap>();
            writer.Configuration.Delimiter = "\t";
        }

        public void Dispose()
        {
            writer?.Dispose();
        }

        public void Write(IEnumerable<ScanPeakData> data)
        {
            writer.WriteRecords(data);
        }

        public void Write(RawLabelData data)
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
                OnWarningEvent(clsStackTraceFormatter.GetExceptionStackTraceMultiLine(ex));
                return false;
            }

        }
    }
}

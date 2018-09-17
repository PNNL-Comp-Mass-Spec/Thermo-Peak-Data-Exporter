using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;

namespace ThermoPeakDataExporter
{
    public class ScanPeakDataWriter : IDisposable
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

        public void Write(IEnumerable<RawLabelData> data)
        {
            foreach (var scan in data)
            {
                Write(scan);
            }
        }
    }
}

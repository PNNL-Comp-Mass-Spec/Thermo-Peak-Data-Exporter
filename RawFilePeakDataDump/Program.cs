using System;
using System.IO;
using PRISM;

namespace RawFilePeakDataDump
{
    public class Program
    {
        static void Main(string[] args)
        {
            var parser = new CommandLineParser<CommandLineOptions>();
            var parsed = parser.ParseArgs(args);
            var options = parsed.ParsedResults;
            if (!parsed.Success || !options.Validate())
            {
                return;
            }

            using (var raw = new RawFileReader(options.RawFilePath))
            using (var tsv = new ScanPeakDataWriter(options.OutputPath))
            {
                raw.LoadFile();

                // Uses a yield return IEnumerable to reduce memory requirements - we do not hold data from the whole file in memory, each scan is written right after it is read
                var data = raw.GetLabelData(options);

                tsv.Write(data);
            }

            return;
        }
    }
}

using System.IO;

namespace RawFilePeakDataDump
{
    public class DataWriter
    {
        private string _outputFile;

        public DataWriter(string outputFilePath)
        {
            _outputFile = outputFilePath;
        }

        public void WriteDataToFile(string rawFilePath)
        {
            using (var writer = new StreamWriter(new FileStream(_outputFile, FileMode.Create)))
            using (var rawReader = new RawFileReader(rawFilePath))
            {
                rawReader.LoadFile();

                writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", "Scan Number", "RT", "Mass", "Intensity", "Resolution",
                    "Baseline", "Noise", "Charge");

                foreach (var scan in rawReader.GetAllLabelData())
                {
                    scan.LabelData.Sort((x, y) =>
                    {
                        var massDiff = x.Mass.CompareTo(y.Mass);
                        if (massDiff == 0)
                        {
                            return x.Intensity.CompareTo(y.Intensity);
                        }
                        return massDiff;
                    });
                    foreach (var peak in scan.LabelData)
                    {
                        writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", scan.ScanNumber, scan.ScanTime, peak.Mass, peak.Intensity, peak.Resolution, peak.Baseline, peak.Noise, peak.Charge);
                    }
                }
            }
        }
    }
}

using System;
using System.IO;

namespace RawFilePeakDataDump
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: {0} <rawFilePath> [outputFilePath]", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe");
                return;
            }

            var rawFilePath = args[0];
            string outputPath;
            if (args.Length > 1)
            {
                outputPath = args[1];
            }
            else
            {
                outputPath = Path.ChangeExtension(rawFilePath, ".tsv");
            }

            if (!File.Exists(rawFilePath))
            {
                Console.WriteLine("Error: raw file \"{0}\" does not exist.", rawFilePath);
                return;
            }
            if (!rawFilePath.ToLower().EndsWith(".raw"))
            {
                Console.WriteLine("Error: file \"{0}\" is not a raw file.", rawFilePath);
                return;
            }

            var writer = new DataWriter(outputPath);
            writer.WriteDataToFile(rawFilePath);

            return;
        }
    }
}

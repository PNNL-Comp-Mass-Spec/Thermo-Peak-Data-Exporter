using System.Collections.Generic;
using ThermoRawFileReaderDLL.FinniganFileIO;

namespace RawFilePeakDataDump
{
    public class RawLabelData
    {
        public int ScanNumber { get; set; }
        public List<XRawFileIO.udtFTLabelInfoType> LabelData { get; set; }
    }
}

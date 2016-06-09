using System.Collections.Generic;
using ThermoRawFileReader;

namespace RawFilePeakDataDump
{
    public class RawLabelData
    {
        public int ScanNumber { get; set; }
        public List<udtFTLabelInfoType> LabelData { get; set; }
    }
}

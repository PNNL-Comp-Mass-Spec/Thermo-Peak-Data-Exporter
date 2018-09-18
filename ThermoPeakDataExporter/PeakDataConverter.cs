using System;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace ThermoPeakDataExporter
{
    /// <summary>
    /// CsvHelper uses this class when converting doubles to strings
    /// </summary>
    public class DoubleConverter : ITypeConverter
    {
        private readonly string mFormatSpec;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="numDecimals">Number of digits to display after the decimal point</param>
        public DoubleConverter(short numDecimals)
        {
            mFormatSpec = "0";
            if (numDecimals > 0)
            {
                mFormatSpec += "." + new string('#', numDecimals);
            }
        }

        /// <summary>
        /// Convert from a string to a double
        /// </summary>
        /// <param name="text"></param>
        /// <param name="row"></param>
        /// <param name="memberMapData"></param>
        /// <returns>Converted value if a double, otherwise null</returns>
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (double.TryParse(text, out var value))
                    return value;
            }

            return null;
        }

        /// <summary>
        /// Convert from a double to a string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="memberMapData"></param>
        /// <returns>Value as a string, or an empty string if not numeric (including if null)</returns>
        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value == null)
                return string.Empty;

            if (double.TryParse(value.ToString(), out var numericValue))
            {
                if (Math.Abs(numericValue) < float.Epsilon)
                    return "0";

                return numericValue.ToString(mFormatSpec);
            }

            return string.Empty;
        }

    }

}

using System;
namespace CsvGenerator
{
    public class CsvOptions
    {
        public static readonly CsvOptions Default = new CsvOptions();

        public char ValueSeparator { get; set; } = ',';
        public string LineSeparator { get; set; } = "\n";
        public string DateTimeFormat { get; set; } = "O";
        public string FloatingNumberFormat { get; set; } = "0.00000000000";
        public bool ForceQuoteValues { get; set; } = false;
        public bool AddTrailingLineEnding { get; set; } = true;
    }
}

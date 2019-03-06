using System;
using System.Collections.Generic;

namespace CsvGenerator.Utils
{
    public static class CsvUtils
    {
        private static readonly char[] charsRequiringAdditionalQuotes = { ',', '"' };

        public static bool StrMustAddQuoutes(string str)
        {
            return str.IndexOfAny(charsRequiringAdditionalQuotes) != -1;
        }

        public static string ParseSpecialCharacters(string str)
        {
            return str.Replace("\\", "\\\\");
        }
    }
}

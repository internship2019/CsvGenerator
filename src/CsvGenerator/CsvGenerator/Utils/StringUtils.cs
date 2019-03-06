using System;
namespace CsvSerializer.Utils
{
    public static class StringUtils
    {
        public static string Enquote(string str, char quote = '"')
        {
            return $"{quote}{str}{quote}";
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;

namespace CsvSerializer
{
    public interface ICsvReader<out T>
    {
        IEnumerable<T> ReadFromCsv(TextReader reader, CsvOptions options);
    }
}

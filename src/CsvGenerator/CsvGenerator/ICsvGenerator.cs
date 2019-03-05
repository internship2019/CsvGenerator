using System;
using System.Collections.Generic;
using System.IO;

namespace CsvGenerator
{
    public interface ICsvGenerator<T>
    {
        void WriteCsv(IEnumerable<T> collection, TextWriter writer, CsvOptions options);
    }
}

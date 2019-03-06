using System;
using System.Collections.Generic;
using System.IO;

namespace CsvSerializer
{
    public interface ICsvGenerator<T>
    {
        void WriteCsv(IEnumerable<T> collection, TextWriter writer, CsvOptions options);
    }
}

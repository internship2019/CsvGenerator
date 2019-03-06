using System;
using System.Collections.Generic;
using System.IO;

namespace CsvSerializer
{
    public class CsvReaderImpl<T> : ICsvReader<T>
    {
        public CsvReaderImpl()
        {
        }

        public IEnumerable<T> ReadFromCsv(TextReader reader, CsvOptions options)
        {
            yield break;
        }
    }
}

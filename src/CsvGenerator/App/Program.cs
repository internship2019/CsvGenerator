using System;
using System.Collections.Generic;
using System.IO;
using CsvGenerator;

namespace App
{
    public class SampleData
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public double ADouble { get; set; } = 42.42;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var samples = new[]
            {
                new SampleData { Id = 1, ParentId = null, Name = "sample1" },
                new SampleData { Id = 2, ParentId = 1, Name = "sample2, parent=1" }
            };

            Console.WriteLine(GenerateCsvReport(samples));
        }

        public static string GenerateCsvReport(IEnumerable<SampleData> samples)
        {
            var generator = new CsvGeneratorImpl<SampleData>();

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                generator.WriteCsv(samples, writer, CsvOptions.Default);
                writer.Flush();
                stream.Position = 0;

                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }
    }
}

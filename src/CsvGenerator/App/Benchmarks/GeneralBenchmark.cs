using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using CsvSerializer;

namespace App.Benchmarks
{
    public class GeneralBenchmark
    {
        public enum SampleEnum
        {
            Value1,
            Value2
        }

        public class SampleClass
        {
            public int Nb1 { get; set; } = 42;
            public int Nb2 { get; set; } = 24;

            public uint Uint { get; set; } = 42;
            public ushort Ushort { get; set; } = 42;

            public string Str { get; set; } = "\" str1, str2, str3";

            public float Float { get; set; } = 42.42f;
            public double Double { get; set; } = 42.42;
            public decimal Decimal { get; set; } = 42.42m;

            public DateTime DateTime { get; set; } = DateTime.UnixEpoch;
            public DateTimeOffset DateTimeOffset { get; set; } = DateTimeOffset.UnixEpoch;

            public TimeSpan TimeSpan { get; set; } = TimeSpan.Zero;

            public int? Nullable { get; set; } = null;

            public Guid Guid { get; set; } = Guid.Empty;

            public bool Bool { get; set; } = false;

            public SampleEnum SampleEnum { get; set; } = SampleEnum.Value1;
        }

        private IEnumerable<SampleClass> samples;
        private ICsvGenerator<SampleClass> generator;
        private MemoryStream memoryStream;
        private StreamWriter streamWriter;

        [GlobalSetup]
        public void Setup()
        {
            samples = InitSamples(20000).ToArray();
            generator = new CsvGeneratorImpl<SampleClass>();
            memoryStream = new MemoryStream();
            streamWriter = new StreamWriter(memoryStream);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            streamWriter.Dispose();
            memoryStream.Dispose();
        }

        [Benchmark]
        public void RunBenchmarkOperation()
        {
            generator.WriteCsv(samples, streamWriter, CsvOptions.Default);
        }

        private IEnumerable<SampleClass> InitSamples(int size)
        {
            for (int i = 0; i < size; i++)
                yield return new SampleClass();
        }
    }
}

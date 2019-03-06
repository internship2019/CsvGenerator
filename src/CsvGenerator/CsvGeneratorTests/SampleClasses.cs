using System;

namespace CsvGeneratorTests
{
    public class EmptyClass
    {
    }

    public class TwoIntsClass
    {
        public int Number1 { get; set; }
        public int Number2 { get; set; }
    }

    public class StrAndInt
    {
        public string Str { get; set; }
        public int ANumber { get; set; }
    }

    public class ClassWithAFloat
    {
        public float AFloat { get; set; }
    }

    public class ClassWithAString
    {
        public string AString { get; set; }
    }

    public class ClassWithTime
    {
        public DateTime DateTime { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
    }

    public enum EnumSample
    {
        Value1,
        Value2
    }

    public class ClassWithEnum
    {
        public EnumSample AEnum { get; set; }
    }

    public class ClassWithNullable
    {
        public int? ANumber { get; set; }
    }
}

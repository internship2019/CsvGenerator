using System;
using System.IO;
using CsvGenerator;
using Xunit;

namespace CsvGeneratorTests
{
    public class CsvGeneratorImplTests : IDisposable
    {
        private readonly MemoryStream stream;
        private readonly TextWriter writer;
        private readonly CsvOptions defaultOptions;

        public CsvGeneratorImplTests()
        {
            stream = new MemoryStream();
            writer = new StreamWriter(stream);
            defaultOptions = GetDefaultOptions();
        }

        public void Dispose()
        {
            writer.Dispose();
            stream.Dispose();
        }

        [Fact]
        public void EmptyClass_NothingIsWritten()
        {
            // Arrange
            var csvGenerator = new CsvGeneratorImpl<EmptyClass>();
            var samples = new[] { new EmptyClass() };

            // Act
            csvGenerator.WriteCsv(samples, writer, defaultOptions);

            // Assert
            Assert.True(GetWrittenStr() == string.Empty);
        }

        [Fact]
        public void ClassHas2Fields_HeaderIsWrittenWithComma()
        {
            // Arrange
            var csvGenerator = new CsvGeneratorImpl<TwoIntsClass>();
            var samples = new TwoIntsClass[] { };

            // Act
            csvGenerator.WriteCsv(samples, writer, defaultOptions);

            // Assert
            Assert.Contains("Number1" + DefaultSeparator + "Number2", GetWrittenStr());
        }

        [Fact]
        public void OneItem_TwoIntsAreCorrectlyWritten()
        {
            // Arrange
            var csvGenerator = new CsvGeneratorImpl<TwoIntsClass>();
            var samples = new[] { new TwoIntsClass { Number1 = 42, Number2 = 24 } };

            // Act
            csvGenerator.WriteCsv(samples, writer, defaultOptions);

            // Assert
            Assert.Contains("42" + DefaultSeparator + "24", GetWrittenStr());
        }

        [Fact]
        public void NullAndInt_NothingIsWrittenForNull()
        {
            // Arrange
            var csvGenerator = new CsvGeneratorImpl<StrAndInt>();
            var samples = new[] { new StrAndInt { Str = null, ANumber = 42 } };

            // Act
            csvGenerator.WriteCsv(samples, writer, defaultOptions);

            // Assert
            Assert.Contains("" + DefaultSeparator + "42", GetWrittenStr());
        }

        [Fact]
        public void FloatNumberGiven_FloatIsFormatedCorrectly()
        {
            // Arrange
            var csvGenerator = new CsvGeneratorImpl<ClassWithAFloat>();
            var samples = new[] { new ClassWithAFloat { AFloat = 42.42f } };
            var options = defaultOptions;
            options.FloatingNumberFormat = "0.0";

            // Act
            csvGenerator.WriteCsv(samples, writer, options);

            // Assert
            // Must be 42.4 with no digits afterwards
            Assert.Matches(@"42.4[\D$]", GetWrittenStr());
        }

        [Fact]
        public void LineSeparatorIsEmpty_NoNewlines()
        {
            // Arrange
            var csvGenerator = new CsvGeneratorImpl<TwoIntsClass>();
            var samples = new[] { new TwoIntsClass { Number1 = 42, Number2 = 24 } };

            var options = defaultOptions;
            options.AddTrailingLineEnding = true;
            options.LineSeparator = string.Empty;

            // Act
            csvGenerator.WriteCsv(samples, writer, options);

            // Assert
            Assert.Equal(GetWrittenStr(), "Number1,Number2" + "42,24");
        }

        [Fact]
        public void ContainsEnum_EnumValueIsWritten()
        {
            // Arrange
            var csvGenerator = new CsvGeneratorImpl<ClassWithEnum>();
            var samples = new[] { new ClassWithEnum { AEnum = EnumSample.Value2 } };

            // Act
            csvGenerator.WriteCsv(samples, writer, defaultOptions);

            // Assert
            Assert.Contains("Value2", GetWrittenStr());
        }

        [Fact]
        public void NullableWithNullAndValue_EmptyAndValueIsWritten()
        {
            // Arrange
            var csvGenerator = new CsvGeneratorImpl<ClassWithNullable>();
            var samples = new[]
            {
                new ClassWithNullable { ANumber = null },
                new ClassWithNullable { ANumber = 42 }
            };

            // Act
            csvGenerator.WriteCsv(samples, writer, defaultOptions);

            // Assert
            var expected = defaultOptions.LineSeparator + defaultOptions.LineSeparator + "42";
            Assert.Contains(expected, GetWrittenStr());
        }

        #region Options tests
        [Fact]
        public void ForcedQuotesOption_AllValuesAreDblQuoted()
        {
            // Arrange
            var csvGenerator = new CsvGeneratorImpl<TwoIntsClass>();
            var samples = new[] { new TwoIntsClass { Number1 = 42, Number2 = 24 } };
            var options = defaultOptions;
            options.ForceQuoteValues = true;

            // Act
            csvGenerator.WriteCsv(samples, writer, options);

            // Assert
            Assert.Matches("\"42\"" + options.ValueSeparator + "\"24\"", GetWrittenStr());
        }

        [Fact]
        public void TimeFormatOption_TimeIsCorrectlyFormated()
        {
            // Arrange
            var csvGenerator = new CsvGeneratorImpl<ClassWithTime>();
            var samples = new[]
            {
                new ClassWithTime
                {
                    DateTime = DateTime.UnixEpoch,
                    DateTimeOffset = DateTimeOffset.UnixEpoch
                }
            };

            var options = defaultOptions;
            options.DateTimeFormat = "O";

            // Act
            csvGenerator.WriteCsv(samples, writer, options);

            // Assert
            var targetOutput =
                "1970-01-01T00:00:00.0000000Z" +
                options.ValueSeparator +
                "1970-01-01T00:00:00.0000000+00:00";

            Assert.Contains(targetOutput, GetWrittenStr());
        }

        [Fact]
        public void AddTrailingNewlineOptionIsFalse_NoNewlines()
        {
            // Arrange
            var csvGenerator = new CsvGeneratorImpl<TwoIntsClass>();
            var samples = new[] { new TwoIntsClass { Number1 = 42, Number2 = 24 } };

            var options = defaultOptions;
            options.AddTrailingLineEnding = false;

            // Act
            csvGenerator.WriteCsv(samples, writer, options);

            // Assert
            Assert.DoesNotContain(options.LineSeparator, GetWrittenStr());
        }
        #endregion Options tests

        #region String tests
        [Fact]
        public void StringWithComma_ValueIsDoubleQuoted()
        {
            // Arrange
            var csvGenerator = new CsvGeneratorImpl<ClassWithAString>();
            var samples = new[] { new ClassWithAString { AString = "," } };

            // Act
            csvGenerator.WriteCsv(samples, writer, defaultOptions);

            // Assert
            Assert.Contains("\",\"", GetWrittenStr());
        }

        [Fact]
        public void StringWithDblQuote_ValueIsDoubleQuoted()
        {
            // Arrange
            var csvGenerator = new CsvGeneratorImpl<ClassWithAString>();
            var samples = new[] { new ClassWithAString { AString = "\"" } };

            // Act
            csvGenerator.WriteCsv(samples, writer, defaultOptions);

            // Assert
            Assert.Contains("\"\"\"", GetWrittenStr());
        }

        [Fact]
        public void StringWithBackslash_AnAdditionalBackslashIsAdded()
        {
            // Arrange
            var csvGenerator = new CsvGeneratorImpl<ClassWithAString>();
            var samples = new[] { new ClassWithAString { AString = "\\" } };

            // Act
            csvGenerator.WriteCsv(samples, writer, defaultOptions);

            // Assert
            Assert.Contains("\\\\", GetWrittenStr());
        }
        #endregion String tests

        #region Helpers
        private char DefaultSeparator => defaultOptions.ValueSeparator;

        private string GetWrittenStr()
        {
            writer.Flush();
            stream.Position = 0;

            string result;
            using (var reader = new StreamReader(stream))
                result = string.Copy(reader.ReadToEnd());

            return result;
        }

        private CsvOptions GetDefaultOptions()
        {
            return new CsvOptions
            {
                ValueSeparator = ',',
                ForceQuoteValues = false,
                LineSeparator = "\n"
            };
        }
        #endregion
    }
}
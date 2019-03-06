#pragma warning disable RECS0082 // Parameter has the same name as a member and hides it

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvSerializer.Utils;

namespace CsvSerializer
{
    public class CsvGeneratorImpl<T> : ICsvGenerator<T>
    {
        private IReadOnlyList<PropertyInfo> properties;
        private TextWriter writer;
        private CsvOptions options;

        public CsvGeneratorImpl()
        {
            this.properties = GetPropertiesOfInterest().ToArray();
        }

        public void WriteCsv(IEnumerable<T> collection, TextWriter writer, CsvOptions options)
        {
            if (properties.Count == 0)
                return;

            Inject(writer, options);

            WriteHeader();
            WriteCollection(collection);
        }

        private void Inject(TextWriter writer, CsvOptions options)
        {
            if (writer == null || options == null)
                throw new ArgumentException();

            this.writer = writer;
            this.options = options;
        }

        /*
         * It may be an option to cache the header.
        **/
        private void WriteHeader()
        {
            foreach (var property in properties)
            {
                if (property != properties.First())
                    writer.Write(options.ValueSeparator);

                writer.Write(property.Name);
            }

            WriteNewline();
        }

        private void WriteCollection(IEnumerable<T> collection)
        {
            foreach (var element in collection)
            {
                WriteElement(element);
                WriteNewline();
            }
        }

        private void WriteElement(T element)
        {
            foreach (var property in properties)
            {
                if (property != properties.First())
                    writer.Write(options.ValueSeparator);

                writer.Write(ObjPropertyToCsvStr(property, element));
            }
        }

        /*
         * This can be optimized.
         * For example, int-s will never have commas or double quoutes in them.
        **/
        private string ObjPropertyToCsvStr(PropertyInfo property, T obj)
        {
            var value = property.GetValue(obj);

            if (value == null)
                return string.Empty;

            var valueStr = FormatIfPossible(property.PropertyType, value);

            if (valueStr == null)
                valueStr = property.GetValue(obj).ToString();

            return StrToCsv(valueStr);
        }

        private string FormatIfPossible(Type propertyType, object value)
        {
            if (propertyType.IsFloatingPointNb())
                return string.Format("{0:" + options.FloatingNumberFormat + "}", value);

            if (propertyType.CanUseDateTimeFormat())
                return string.Format("{0:" + options.DateTimeFormat + "}", value);

            return null;
        }

        private string StrToCsv(string str)
        {
            str = CsvUtils.ParseSpecialCharacters(str);

            if (MustEnquoute(str))
                str = StringUtils.Enquote(str);

            return str;
        }

        private bool MustEnquoute(string str)
        {
            return options.ForceQuoteValues || CsvUtils.StrMustAddQuoutes(str);
        }

        private void WriteNewline()
        {
            if (options.AddTrailingLineEnding)
                writer.Write(options.LineSeparator);
        }

        #region Static helpers
        private static IEnumerable<PropertyInfo> GetPropertiesOfInterest()
        {
            var allProperties = typeof(T).GetProperties();
            return allProperties.Where(x => IsATypeOfInterest(x.PropertyType));
        }

        private static bool IsATypeOfInterest(Type type)
        {
            return
                type.IsPrimitive ||
                type.IsEnum ||
                type == typeof(string) ||
                type == typeof(decimal) ||
                type == typeof(DateTime) ||
                type == typeof(DateTimeOffset) ||
                type == typeof(TimeSpan) ||
                type == typeof(Guid) ||
                IsAnInterestingNullable(type);
        }

        private static bool IsAnInterestingNullable(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);

            if (underlyingType == null)
                return false;

            return IsATypeOfInterest(underlyingType);
        }
        #endregion Static helpers
    }
}

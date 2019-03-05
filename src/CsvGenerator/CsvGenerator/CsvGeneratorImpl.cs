using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CsvGenerator
{
    public class CsvGeneratorImpl<T> : ICsvGenerator<T>
    {
        private IReadOnlyList<PropertyInfo> properties;

        public CsvGeneratorImpl()
        {
            this.properties = GetPropertiesOfInterest().ToArray();
        }

        public void WriteCsv(IEnumerable<T> collection, TextWriter writer, CsvOptions options)
        {
            if (properties.Count == 0)
                return;

            WriteHeader(writer, options);
            WriteCollection(writer, options, collection);
        }

        /*
         * It may be an option to cache the header.
        **/
        private void WriteHeader(TextWriter writer, CsvOptions options)
        {
            foreach (var property in properties)
            {
                if (property != properties.First())
                    writer.Write(options.ValueSeparator);

                writer.Write(property.Name);
            }

            WriteNewline(writer, options);
        }

        private void WriteCollection(TextWriter writer, CsvOptions options, IEnumerable<T> collection)
        {
            foreach (var element in collection)
            {
                WriteElement(writer, options, element);
                WriteNewline(writer, options);
            }
        }

        private void WriteElement(TextWriter writer, CsvOptions options, T element)
        {
            foreach (var property in properties)
            {
                if (property != properties.First())
                    writer.Write(options.ValueSeparator);

                writer.Write(ObjPropertyToCsvStr(property, options, element));
            }
        }

        /*
         * This can be optimized.
         * For example, int-s will never have commas or double quoutes in them.
         * 
         * TODO: if nb will contain comma can be determined in constructor.
        **/
        private string ObjPropertyToCsvStr(PropertyInfo property, CsvOptions options, T obj)
        {
            var value = property.GetValue(obj);

            // Return empty string if value is null
            if (value == null)
                return string.Empty;

            var valueStr = FormatIfPossible(options, property.PropertyType, value);

            if (valueStr == null)
                valueStr = property.GetValue(obj).ToString();

            return StrToCsv(options, valueStr);
        }

        private string FormatIfPossible(CsvOptions options, Type propertyType, object value)
        {
            if (TypeIsRealNb(propertyType))
                return string.Format("{0:" + options.FloatingNumberFormat + "}", value);

            if (TypeCanUseDateTimeFormat(propertyType))
                return string.Format("{0:" + options.DateTimeFormat + "}", value);

            return null;
        }

        private bool TypeIsRealNb(Type type)
        {
            return
                type == typeof(float) ||
                type == typeof(double) ||
                type == typeof(decimal);
        }

        private bool TypeCanUseDateTimeFormat(Type type)
        {
            return type == typeof(DateTime) || type == typeof(DateTimeOffset);
        }

        private string StrToCsv(CsvOptions options, string str)
        {
            str = str.Replace("\\", "\\\\");

            if (options.ForceQuoteValues || MustAddQuoutes(str))
                str = '"' + str + '"';

            return str;
        }

        /*
         * TODO: cache the used array.       
        **/
        private bool MustAddQuoutes(string str)
        {
            return str.IndexOfAny(new[] { ',', '"' }) != -1;
        }

        private void WriteNewline(TextWriter writer, CsvOptions options)
        {
            if (options.AddTrailingLineEnding)
                writer.Write(options.LineSeparator);
        }

        private IEnumerable<PropertyInfo> GetPropertiesOfInterest()
        {
            var allProperties = typeof(T).GetProperties();
            foreach (var property in allProperties)
            {
                if (IsATypeOfInterest(property.PropertyType))
                    yield return property;
            }
        }

        private bool IsATypeOfInterest(Type type)
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
                IsAGoodNullable(type);
        }

        private bool IsAGoodNullable(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
                return IsATypeOfInterest(underlyingType);

            return false;
        }
    }
}

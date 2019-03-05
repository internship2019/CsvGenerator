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

            writer.Write(options.LineSeparator);
        }

        private void WriteCollection(TextWriter writer, CsvOptions options, IEnumerable<T> collection)
        {
            foreach (var element in collection)
            {
                WriteElement(writer, options, element);
                writer.Write(options.LineSeparator);
            }
        }

        private void WriteElement(TextWriter writer, CsvOptions options, T element)
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
         * 
         * TODO: if nb will contain comma can be determined in constructor.
        **/
        private string ObjPropertyToCsvStr(PropertyInfo property, T obj)
        {
            var value = property.GetValue(obj);

            // Return empty string if value is null
            if (value == null)
                return string.Empty;

            var valueStr = property.GetValue(obj).ToString();

            if (TypeIsNbWithPossibleComma(property.PropertyType))
                valueStr = valueStr.Replace(',', '.');

            return StrToCsv(valueStr);
        }

        private bool TypeIsNbWithPossibleComma(Type type)
        {
            return
                type == typeof(float) ||
                type == typeof(double) ||
                type == typeof(decimal);
        }

        /*
         * TODO: cache the used array.       
        **/
        private string StrToCsv(string str)
        {
            str = str.Replace("\\", "\\\\");

            if (str.IndexOfAny(new[] { ',', '"' }) != -1)
                str = '"' + str + '"';

            return str;
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

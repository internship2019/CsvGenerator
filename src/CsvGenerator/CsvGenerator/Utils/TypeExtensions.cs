using System;
namespace CsvSerializer.Utils
{
    public static class TypeUtils
    {
        public static bool IsFloatingPointNb(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
                
                default:
                    return false;
            }
        }

        public static bool CanUseDateTimeFormat(this Type type)
        {
            return
                type == typeof(DateTime) ||
                type == typeof(DateTimeOffset);
        }
    }
}

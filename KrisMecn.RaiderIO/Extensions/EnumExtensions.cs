using System;
using System.Runtime.Serialization;

namespace KrisMecn.RaiderIO.Extensions
{
    internal static class EnumExtensions
    {
        public static string ToEnumString<T>(this T type)
            where T : Enum
        {
            var enumType = typeof(T);
            var name = Enum.GetName(enumType, type);
            var enumMemberAttributes = (EnumMemberAttribute[])enumType
                .GetField(name)
                .GetCustomAttributes(typeof(EnumMemberAttribute), true);

            if (enumMemberAttributes.Length < 1) return string.Empty;

            return enumMemberAttributes[0].Value;
        }
    }
}
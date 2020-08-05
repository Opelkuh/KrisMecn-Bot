namespace KrisMecn.Helpers.Extensions
{
    public static class StringExtensions
    {
        public static string ToFirstCharUppercase(this string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length < 1) return str;
            return str[0].ToString().ToUpper() + str.Substring(1);
        }
    }
}
namespace SRGEnt.Generator.DataTypes.Utils
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string str) =>
            string.IsNullOrEmpty(str) || str.Length < 2
                ? str
                : $"{char.ToLowerInvariant(str[0]).ToString()}{str.Substring(1)}";
    }
}
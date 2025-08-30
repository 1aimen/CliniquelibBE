using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Cliniquelib_BE.Utils
{
    public static class StringHelper
    {
        public static bool IsNullOrEmpty(string str) => string.IsNullOrWhiteSpace(str);

        public static string NullToEmpty(string str) => str ?? string.Empty;

        public static string Capitalize(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        public static string ToCamelCase(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;
            str = RemoveExtraSpaces(str);
            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        public static string RemoveExtraSpaces(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;
            return Regex.Replace(str.Trim(), @"\s+", " ");
        }

        public static string ToSlug(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

            str = RemoveDiacritics(str).ToLowerInvariant();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");  // remove invalid chars
            str = Regex.Replace(str, @"\s+", "-");          // convert spaces to hyphens
            str = Regex.Replace(str, @"-+", "-");           // collapse multiple hyphens
            return str.Trim('-');
        }

        public static string RemoveDiacritics(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;
            var normalized = str.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var sb = new StringBuilder();
            using var rng = RandomNumberGenerator.Create();
            var data = new byte[length];
            rng.GetBytes(data);
            foreach (var b in data)
                sb.Append(chars[b % chars.Length]);
            return sb.ToString();
        }

        public static string MaskString(string str, int visibleChars = 4, char maskChar = '*')
        {
            if (string.IsNullOrEmpty(str) || str.Length <= visibleChars)
                return str;
            return new string(maskChar, str.Length - visibleChars) + str[^visibleChars..];
        }

        public static string SafeTrim(string str) => str?.Trim();

        public static int CountOccurrences(string str, string substring)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(substring)) return 0;
            return Regex.Matches(str, Regex.Escape(substring)).Count;
        }

        public static string ToTitleCase(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        public static string NormalizeLineEndings(string str)
        {
            if (str == null) return null;
            return str.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        public static string RemoveNonAlphanumeric(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;
            return Regex.Replace(str, @"[^a-zA-Z0-9]", "");
        }
    }
}

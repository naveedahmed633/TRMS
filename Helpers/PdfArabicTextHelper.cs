using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MvcApplication1.Helpers
{
    /// <summary>
    /// Repairs Arabic text that was stored or transported using the wrong byte encoding.
    /// </summary>
    public static class PdfArabicTextHelper
    {
        private static readonly Regex ArabicScriptRegex = new Regex(
            @"[\u0600-\u06FF\u0750-\u077F\u08A0-\u08FF\uFB50-\uFDFF\uFE70-\uFEFF]",
            RegexOptions.Compiled);

        public static string FixIfNeeded(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value ?? string.Empty;
            }

            if (ContainsArabicScript(value))
            {
                return value;
            }

            string fromCp1256 = TryDecodeFromSingleByteEncoding(value, 1256);
            if (fromCp1256 != null)
            {
                return fromCp1256;
            }

            string fromUtf8 = TryDecodeFromSingleByteEncoding(value, Encoding.UTF8);
            if (fromUtf8 != null)
            {
                return fromUtf8;
            }

            string fromLatin1AsUtf8 = TryLatin1BytesAsUtf8(value);
            if (fromLatin1AsUtf8 != null)
            {
                return fromLatin1AsUtf8;
            }

            return value;
        }

        private static bool ContainsArabicScript(string value)
        {
            return !string.IsNullOrEmpty(value) && ArabicScriptRegex.IsMatch(value);
        }

        private static string TryDecodeFromSingleByteEncoding(string value, int codePage)
        {
            try
            {
                var source = Encoding.GetEncoding(codePage);
                byte[] bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(value);
                string decoded = source.GetString(bytes);
                return ContainsArabicScript(decoded) ? decoded : null;
            }
            catch
            {
                return null;
            }
        }

        private static string TryDecodeFromSingleByteEncoding(string value, Encoding targetEncoding)
        {
            try
            {
                byte[] bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(value);
                string decoded = targetEncoding.GetString(bytes);
                return ContainsArabicScript(decoded) ? decoded : null;
            }
            catch
            {
                return null;
            }
        }

        private static string TryLatin1BytesAsUtf8(string value)
        {
            try
            {
                byte[] bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(value);
                string decoded = Encoding.UTF8.GetString(bytes);
                return ContainsArabicScript(decoded) ? decoded : null;
            }
            catch
            {
                return null;
            }
        }
    }
}

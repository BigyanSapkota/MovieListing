using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shared.Helper
{
     public static class NormalizeHelper
    {

        public static string NormalizeText(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            // 1. Normalize Unicode (Form C → composed)
            var normalized = input.Normalize(NormalizationForm.FormC);

            // 2. Convert to lowercase
            normalized = normalized.ToLowerInvariant();

            // 3. Remove diacritics (accents, marks)
            var sb = new StringBuilder();
            foreach (var c in normalized.Normalize(NormalizationForm.FormD)) // decompose
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark) // skip accent marks
                    sb.Append(c);
            }
            normalized = sb.ToString().Normalize(NormalizationForm.FormC);

            // 4. Remove spaces and special characters (keep only letters & digits)
            normalized = Regex.Replace(normalized, @"[^a-z0-9]", "");

            return normalized;
        }

    }
}

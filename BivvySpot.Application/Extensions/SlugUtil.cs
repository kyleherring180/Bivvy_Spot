using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace BivvySpot.Application.Extensions;

public static class SlugUtil
{
    private static readonly Regex NonAllowed = new(@"[^a-z0-9\-]+", RegexOptions.Compiled);
    private static readonly Regex Dashes     = new(@"\-+", RegexOptions.Compiled);

    public static string Slugify(string input, int maxLen = 64)
    {
        if (string.IsNullOrWhiteSpace(input)) return "tag";
        var s = input.Trim().ToLowerInvariant();

        // remove diacritics
        var formD = s.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(formD.Length);
        foreach (var c in formD)
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) sb.Append(c);
        s = sb.ToString().Normalize(NormalizationForm.FormC);

        s = s.Replace(' ', '-').Replace('_', '-');
        s = NonAllowed.Replace(s, "-");
        s = Dashes.Replace(s, "-").Trim('-');

        if (s.Length == 0) s = "tag";
        if (s.Length > maxLen) s = s[..maxLen].Trim('-');
        return s;
    }
}
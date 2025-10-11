using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace BivvySpot.Model.Entities;

public class Tag
{
    private static readonly Regex NonAllowed = new("[^a-z0-9\\-]+", RegexOptions.Compiled);
    private static readonly Regex Dashes     = new("\\-+", RegexOptions.Compiled);

    private readonly List<PostTag> _postTags = new();
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;

    public ReadOnlyCollection<PostTag> PostTags => _postTags.AsReadOnly();
    
    private Tag() { /* private constructor for EF */}
    public Tag(string name)
    {
        Id = Guid.NewGuid();
        Name = name.Trim();
        Slug = Slugify(Name);
    }

    public static Dictionary<string,(string name,string slug)> NormalizeTags(IEnumerable<string> names)
    {
        var dict = new Dictionary<string,(string,string)>(StringComparer.OrdinalIgnoreCase);
        foreach (var raw in names)
        {
            if (string.IsNullOrWhiteSpace(raw)) continue;
            var name = raw.Trim();
            var slug = Slugify(name);
            dict[slug] = (name, slug);
        }
        return dict;
    }

    private static string Slugify(string input, int maxLen = 64)
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
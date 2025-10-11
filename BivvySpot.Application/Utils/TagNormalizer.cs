using BivvySpot.Model.Entities;

namespace BivvySpot.Application.Utils;

public static class TagNormalizer
{
    public static Dictionary<string,(string name,string slug)> Normalize(IEnumerable<string> names, int maxLen = 64)
    {
        var dict = new Dictionary<string,(string,string)>(StringComparer.OrdinalIgnoreCase);
        foreach (var raw in names)
        {
            if (string.IsNullOrWhiteSpace(raw)) continue;
            var name = raw.Trim();
            var slug = Tag.Slugify(name, maxLen);
            if (!dict.ContainsKey(slug))
                dict[slug] = (name, slug);
        }
        return dict;
    }
}
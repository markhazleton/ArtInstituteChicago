using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace WebSpark.ArtSpark.Demo.Utilities;

public static class SlugGenerator
{
    public static string GenerateSlug(string input, int maxLength = 100)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Convert to lowercase
        string slug = input.ToLowerInvariant();

        // Remove diacritics (accents)
        slug = RemoveDiacritics(slug);

        // Replace spaces and special characters with hyphens
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", string.Empty);
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");

        // Remove leading/trailing hyphens
        slug = slug.Trim('-');

        // Truncate if necessary
        if (slug.Length > maxLength)
        {
            slug = slug.Substring(0, maxLength);
            // Remove trailing hyphen if truncation created one
            slug = slug.TrimEnd('-');
        }

        return slug;
    }

    public static string GenerateUniqueSlug(string baseSlug, Func<string, bool> slugExists)
    {
        string slug = baseSlug;
        int counter = 1;

        while (slugExists(slug))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        return slug;
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string GenerateMetaDescription(string? description, string? longDescription, int maxLength = 160)
    {
        var content = !string.IsNullOrWhiteSpace(description) ? description :
                     !string.IsNullOrWhiteSpace(longDescription) ? longDescription : string.Empty;

        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;

        // Remove HTML tags if any
        content = Regex.Replace(content, "<.*?>", string.Empty);

        // Truncate and add ellipsis if needed
        if (content.Length > maxLength)
        {
            content = content.Substring(0, maxLength - 3) + "...";
        }

        return content;
    }
}

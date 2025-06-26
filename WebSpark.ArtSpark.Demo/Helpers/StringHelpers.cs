using System.Globalization;

namespace WebSpark.ArtSpark.Demo.Helpers;

/// <summary>
/// Culture-aware string operations to address CA1304, CA1305, CA1310, CA1311 warnings
/// Provides consistent, culture-invariant string operations for better reliability
/// </summary>
public static class StringHelpers
{
    /// <summary>
    /// Culture-invariant string comparison (CA1310, CA1862)
    /// </summary>
    public static bool ContainsIgnoreCase(this string source, string value)
    {
        return source.Contains(value, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Culture-invariant case conversion (CA1304)
    /// </summary>
    public static string ToLowerInvariant(this string source)
    {
        return source.ToLower(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Culture-invariant string starts with (CA1310)
    /// </summary>
    public static bool StartsWithIgnoreCase(this string source, string value)
    {
        return source.StartsWith(value, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Culture-invariant string ends with (CA1310)
    /// </summary>
    public static bool EndsWithIgnoreCase(this string source, string value)
    {
        return source.EndsWith(value, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Culture-invariant number formatting (CA1305)
    /// </summary>
    public static string ToStringInvariant(this int value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Culture-invariant date formatting (CA1305)
    /// </summary>
    public static string ToStringInvariant(this DateTime value, string format)
    {
        return value.ToString(format, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Optimized single character contains check (CA1847)
    /// </summary>
    public static bool ContainsChar(this string source, char value)
    {
        return source.Contains(value);
    }

    /// <summary>
    /// Optimized count comparison (CA1860)
    /// </summary>
    public static bool HasItems<T>(this IEnumerable<T> source)
    {
        return source is ICollection<T> collection ? collection.Count > 0 : source.Any();
    }
}

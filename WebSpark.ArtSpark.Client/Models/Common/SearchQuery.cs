using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebSpark.ArtSpark.Client.Models.Common;

/// <summary>
/// Search query parameters for search endpoints
/// </summary>
public class SearchQuery
{
    /// <summary>
    /// Search query string
    /// </summary>
    public string? Q { get; set; }

    /// <summary>
    /// Elasticsearch query DSL
    /// </summary>
    public object? Query { get; set; }

    /// <summary>
    /// Sort parameter
    /// </summary>
    public string? Sort { get; set; }

    /// <summary>
    /// Starting point of results (Elasticsearch pagination)
    /// </summary>
    public int? From { get; set; }

    /// <summary>
    /// Number of results to return (Elasticsearch pagination)
    /// </summary>
    public int? Size { get; set; }

    /// <summary>
    /// Comma-separated list of aggregation facets to include
    /// </summary>
    public string? Facets { get; set; }

    /// <summary>
    /// Fields to include in response
    /// </summary>
    public string? Fields { get; set; }

    /// <summary>
    /// Convert to query string parameters
    /// </summary>
    /// <returns>Dictionary of query parameters</returns>
    public Dictionary<string, string> ToQueryParameters()
    {
        var parameters = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(Q))
            parameters["q"] = Q;

        if (Query != null)
        {
            var queryJson = JsonSerializer.Serialize(Query);
            parameters["query"] = queryJson;
        }

        if (!string.IsNullOrEmpty(Sort))
            parameters["sort"] = Sort;

        if (From.HasValue)
            parameters["from"] = From.Value.ToString();

        if (Size.HasValue)
            parameters["size"] = Size.Value.ToString();

        if (!string.IsNullOrEmpty(Facets))
            parameters["facets"] = Facets;

        if (!string.IsNullOrEmpty(Fields))
            parameters["fields"] = Fields;

        return parameters;
    }
}

/// <summary>
/// Search response wrapper
/// </summary>
/// <typeparam name="T">The resource type</typeparam>
public class SearchResponse<T>
{
    [JsonPropertyName("data")]
    public List<T>? Data { get; set; }

    [JsonPropertyName("pagination")]
    public SearchPagination? Pagination { get; set; }

    [JsonPropertyName("info")]
    public Info? Info { get; set; }

    [JsonPropertyName("config")]
    public Config? Config { get; set; }
}

/// <summary>
/// Elasticsearch-style pagination for search responses
/// </summary>
public class SearchPagination
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("from")]
    public int From { get; set; }
}

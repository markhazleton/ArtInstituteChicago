namespace ArtInstituteChicago.Client.Models.Common;

/// <summary>
/// Query parameters for API listing endpoints
/// </summary>
public class ApiQuery
{
    /// <summary>
    /// Comma-separated list of resource IDs to retrieve
    /// </summary>
    public string? Ids { get; set; }

    /// <summary>
    /// Number of resources to return per page
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    /// Page of resources to retrieve
    /// </summary>
    public int? Page { get; set; }

    /// <summary>
    /// Comma-separated list of fields to return per resource
    /// </summary>
    public string? Fields { get; set; }

    /// <summary>
    /// Comma-separated list of subresources to embed in the returned resources
    /// </summary>
    public string? Include { get; set; }

    /// <summary>
    /// Convert to query string parameters
    /// </summary>
    /// <returns>Dictionary of query parameters</returns>
    public Dictionary<string, string> ToQueryParameters()
    {
        var parameters = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(Ids))
            parameters["ids"] = Ids;

        if (Limit.HasValue)
            parameters["limit"] = Limit.Value.ToString();

        if (Page.HasValue)
            parameters["page"] = Page.Value.ToString();

        if (!string.IsNullOrEmpty(Fields))
            parameters["fields"] = Fields;

        if (!string.IsNullOrEmpty(Include))
            parameters["include"] = Include;

        return parameters;
    }
}

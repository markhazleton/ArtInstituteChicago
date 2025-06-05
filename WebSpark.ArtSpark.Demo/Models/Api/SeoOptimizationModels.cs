using System.ComponentModel.DataAnnotations;
using WebSpark.ArtSpark.Agent.Models;
using WebSpark.ArtSpark.Demo.Models;

namespace WebSpark.ArtSpark.Demo.Models.Api;

/// <summary>
/// Request model for collection optimization
/// </summary>
public class OptimizeCollectionRequest
{
    /// <summary>
    /// Description of the collection to optimize
    /// </summary>
    [Required]
    [StringLength(2000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Response model for collection optimization
/// </summary>
public class OptimizeCollectionResponse
{
    /// <summary>
    /// The optimized collection data
    /// </summary>
    public UserCollectionDto Collection { get; set; } = new();

    /// <summary>
    /// Indicates if the optimization was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if optimization failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Request model for artwork optimization
/// </summary>
public class OptimizeArtworkRequest
{
    /// <summary>
    /// The artwork data to optimize
    /// </summary>
    [Required]
    public ArtworkData ArtworkData { get; set; } = new();

    /// <summary>
    /// ID of the collection the artwork belongs to
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int CollectionId { get; set; }
}

/// <summary>
/// Response model for artwork optimization
/// </summary>
public class OptimizeArtworkResponse
{
    /// <summary>
    /// The optimized collection artwork data
    /// </summary>
    public CollectionArtworkDto CollectionArtwork { get; set; } = new();

    /// <summary>
    /// Indicates if the optimization was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if optimization failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// DTO for UserCollection
/// </summary>
public class UserCollectionDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LongDescription { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? Tags { get; set; }
    public string? CuratorNotes { get; set; }
    public bool IsPublic { get; set; }
}

/// <summary>
/// DTO for CollectionArtwork
/// </summary>
public class CollectionArtworkDto
{
    public int CollectionId { get; set; }
    public int ArtworkId { get; set; }
    public string? Slug { get; set; }
    public string? CustomTitle { get; set; }
    public string? CustomDescription { get; set; }
    public string? CuratorNotes { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public int DisplayOrder { get; set; }
}

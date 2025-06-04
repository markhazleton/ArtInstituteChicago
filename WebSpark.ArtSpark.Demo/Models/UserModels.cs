using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebSpark.ArtSpark.Demo.Models;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }

    // Navigation properties
    public virtual ICollection<ArtworkReview> Reviews { get; set; } = new List<ArtworkReview>();
    public virtual ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
    public virtual ICollection<UserCollection> Collections { get; set; } = new List<UserCollection>();
}

public class ArtworkReview
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int ArtworkId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(2000)]
    public string? ReviewText { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
}

public class UserFavorite
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int ArtworkId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
}

public class UserCollection
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // SEO and metadata fields
    public string Slug { get; set; } = string.Empty;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? SocialImageUrl { get; set; }

    // Content fields
    public string? LongDescription { get; set; }
    public string? CuratorNotes { get; set; }
    public string? Tags { get; set; } // Comma-separated tags
    public int ViewCount { get; set; } = 0;
    public bool IsFeatured { get; set; } = false;
    public DateTime? FeaturedUntil { get; set; }    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ICollection<CollectionArtwork> Artworks { get; set; } = new List<CollectionArtwork>();
    public virtual ICollection<CollectionContentSection> ContentSections { get; set; } = new List<CollectionContentSection>();
    public virtual ICollection<CollectionMedia> MediaItems { get; set; } = new List<CollectionMedia>();
    public virtual ICollection<CollectionLink> Links { get; set; } = new List<CollectionLink>();
}

public class CollectionArtwork
{
    public int Id { get; set; }
    public int CollectionId { get; set; }
    public int ArtworkId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    // Enhanced fields for collection items
    public string? Slug { get; set; }
    public string? CustomTitle { get; set; }
    public string? CustomDescription { get; set; }
    public string? CuratorNotes { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsHighlighted { get; set; } = false;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }

    // Navigation properties
    public virtual UserCollection Collection { get; set; } = null!;
}

// New model for rich content sections within collections
public class CollectionContentSection
{
    public int Id { get; set; }
    public int CollectionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; public string SectionType { get; set; } = "text"; // text, quote, highlight, etc.
    public int DisplayOrder { get; set; } = 0;
    public bool IsVisible { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual UserCollection Collection { get; set; } = null!;
}

// New model for media items (images, videos, etc.) in collections
public class CollectionMedia
{
    public int Id { get; set; }
    public int CollectionId { get; set; }
    public string MediaUrl { get; set; } = string.Empty; // For external URLs
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty; // image, video, audio, document
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual UserCollection Collection { get; set; } = null!;
}

// New model for external links in collections
public class CollectionLink
{
    public int Id { get; set; }
    public int CollectionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string LinkType { get; set; } = "external"; // external, social, reference, etc.
    public int DisplayOrder { get; set; } = 0;
    public bool IsVisible { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual UserCollection Collection { get; set; } = null!;
}

// View Models for Authentication
public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}

public class RegisterViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty; [Required]
    [Display(Name = "Display Name")]
    public string DisplayName { get; set; } = string.Empty;
}

// View Models for User Profile and Collections
public class ProfileViewModel
{
    [Required]
    [Display(Name = "Display Name")]
    [StringLength(100, ErrorMessage = "Display name must be at most {1} characters long.")]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Bio")]
    [StringLength(500, ErrorMessage = "Bio must be at most {1} characters long.")]
    public string? Bio { get; set; }

    [Display(Name = "Profile Image URL")]
    [Url(ErrorMessage = "Please enter a valid URL")]
    public string? ProfileImageUrl { get; set; }

    [Display(Name = "Member Since")]
    public DateTime CreatedAt { get; set; }
}

public class CreateCollectionViewModel
{
    [Required]
    [Display(Name = "Collection Name")]
    [StringLength(100, ErrorMessage = "Collection name must be at most {1} characters long.")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Short Description")]
    [StringLength(500, ErrorMessage = "Description must be at most {1} characters long.")]
    public string? Description { get; set; }

    [Display(Name = "Long Description")]
    [StringLength(2000, ErrorMessage = "Long description must be at most {1} characters long.")]
    public string? LongDescription { get; set; }

    [Display(Name = "Curator Notes")]
    [StringLength(1000, ErrorMessage = "Curator notes must be at most {1} characters long.")]
    public string? CuratorNotes { get; set; }

    [Display(Name = "Tags (comma-separated)")]
    [StringLength(500, ErrorMessage = "Tags must be at most {1} characters long.")]
    public string? Tags { get; set; }

    [Display(Name = "Public Collection")]
    public bool IsPublic { get; set; } = true; [Display(Name = "Featured Collection")]
    public bool IsFeatured { get; set; } = false;

    [Display(Name = "Featured Until")]
    [DataType(DataType.DateTime)]
    public DateTime? FeaturedUntil { get; set; }

    // SEO Fields
    [Display(Name = "SEO Title")]
    [StringLength(60, ErrorMessage = "SEO title should be at most {1} characters long.")]
    public string? MetaTitle { get; set; }

    [Display(Name = "SEO Description")]
    [StringLength(160, ErrorMessage = "SEO description should be at most {1} characters long.")]
    public string? MetaDescription { get; set; }

    [Display(Name = "SEO Keywords")]
    [StringLength(255, ErrorMessage = "SEO keywords must be at most {1} characters long.")]
    public string? MetaKeywords { get; set; }

    [Display(Name = "Social Media Image URL")]
    [Url(ErrorMessage = "Please enter a valid URL")]
    public string? SocialImageUrl { get; set; }
}

public class EditCollectionViewModel : CreateCollectionViewModel
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int ViewCount { get; set; }
}

public class CollectionDetailsViewModel
{
    public UserCollection Collection { get; set; } = null!;
    public IEnumerable<CollectionArtwork> Artworks { get; set; } = new List<CollectionArtwork>();
    public IEnumerable<CollectionContentSection> ContentSections { get; set; } = new List<CollectionContentSection>();
    public IEnumerable<CollectionMedia> MediaItems { get; set; } = new List<CollectionMedia>();
    public IEnumerable<CollectionLink> Links { get; set; } = new List<CollectionLink>();
    public bool CanEdit { get; set; } = false;
}

// View model for managing collection content sections
public class CollectionContentSectionViewModel
{
    public int Id { get; set; }
    public int CollectionId { get; set; }

    [Required]
    [Display(Name = "Section Title")]
    [StringLength(200, ErrorMessage = "Title must be at most {1} characters long.")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Content")]
    [StringLength(4000, ErrorMessage = "Content must be at most {1} characters long.")]
    public string Content { get; set; } = string.Empty; [Display(Name = "Section Type")]
    public string SectionType { get; set; } = "text";

    [Display(Name = "Display Order")]
    public int DisplayOrder { get; set; } = 0;

    [Display(Name = "Visible")]
    public bool IsVisible { get; set; } = true;
}



// View models for media management
public class CollectionMediaViewModel
{
    public int Id { get; set; }
    public int CollectionId { get; set; }
    public string MediaUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty; // image, video, audio, document
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }

    [MaxLength(255)]
    public string? Title { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(255)]
    public string? AltText { get; set; }

    public int DisplayOrder { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

// View model for collection links
public class CollectionLinkViewModel
{
    public int Id { get; set; }
    public int CollectionId { get; set; }

    [Required]
    [Url]
    [MaxLength(2000)]
    public string Url { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public string LinkType { get; set; } = "external"; // external, resource, related
    public int DisplayOrder { get; set; } = 0;
    public bool IsVisible { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// New view model for individual collection item display
public class CollectionItemDetailsViewModel
{
    // Basic item info
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? ExternalUrl { get; set; }
    public string? Artist { get; set; }
    public int? Year { get; set; }
    public string? Medium { get; set; }
    public string? Dimensions { get; set; }

    // Collection item specific info
    public string Slug { get; set; } = string.Empty;
    public string? CustomTitle { get; set; }
    public string? CustomDescription { get; set; }
    public string? CuratorNotes { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsHighlighted { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // SEO fields for the collection item
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? Keywords { get; set; }
    public string? SocialImageUrl { get; set; }

    // Collection info
    public string CollectionSlug { get; set; } = string.Empty;
    public string CollectionTitle { get; set; } = string.Empty;
    public string? CollectionDescription { get; set; }

    // Control flags
    public bool CanEdit { get; set; } = false;
}

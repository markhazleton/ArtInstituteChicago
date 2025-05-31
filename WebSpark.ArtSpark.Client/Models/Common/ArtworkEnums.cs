using System.ComponentModel;

namespace WebSpark.ArtSpark.Client.Models.Common;

/// <summary>
/// Common art styles for filtering artworks
/// </summary>
public enum ArtStyle
{
    [Description("Abstract")]
    Abstract,

    [Description("Impressionism")]
    Impressionism,

    [Description("Post-Impressionism")]
    PostImpressionism,

    [Description("Cubism")]
    Cubism,

    [Description("Fauvism")]
    Fauvism,

    [Description("Expressionism")]
    Expressionism,

    [Description("Surrealism")]
    Surrealism,

    [Description("Pop Art")]
    PopArt,

    [Description("Abstract Expressionism")]
    AbstractExpressionism,

    [Description("Minimalism")]
    Minimalism,

    [Description("Realism")]
    Realism,

    [Description("Romanticism")]
    Romanticism,

    [Description("Neoclassicism")]
    Neoclassicism,

    [Description("Baroque")]
    Baroque,

    [Description("Renaissance")]
    Renaissance,

    [Description("Gothic")]
    Gothic,

    [Description("Art Nouveau")]
    ArtNouveau,

    [Description("Art Deco")]
    ArtDeco,

    [Description("Dadaism")]
    Dadaism,

    [Description("Constructivism")]
    Constructivism,

    [Description("Conceptual Art")]
    ConceptualArt,

    [Description("Performance Art")]
    PerformanceArt,

    [Description("Installation Art")]
    InstallationArt
}

/// <summary>
/// Common art mediums for filtering artworks
/// </summary>
public enum ArtMedium
{
    [Description("Oil on canvas")]
    OilOnCanvas,

    [Description("Watercolor")]
    Watercolor,

    [Description("Acrylic")]
    Acrylic,

    [Description("Tempera")]
    Tempera,

    [Description("Pastel")]
    Pastel,

    [Description("Charcoal")]
    Charcoal,

    [Description("Pencil")]
    Pencil,

    [Description("Ink")]
    Ink,

    [Description("Bronze")]
    Bronze,

    [Description("Marble")]
    Marble,

    [Description("Stone")]
    Stone,

    [Description("Wood")]
    Wood,

    [Description("Clay")]
    Clay,

    [Description("Ceramic")]
    Ceramic,

    [Description("Glass")]
    Glass,

    [Description("Metal")]
    Metal,

    [Description("Fabric")]
    Fabric,

    [Description("Paper")]
    Paper,

    [Description("Photography")]
    Photography,

    [Description("Digital")]
    Digital,

    [Description("Mixed media")]
    MixedMedia,

    [Description("Collage")]
    Collage,

    [Description("Printmaking")]
    Printmaking,

    [Description("Lithograph")]
    Lithograph,

    [Description("Etching")]
    Etching,

    [Description("Engraving")]
    Engraving,

    [Description("Woodcut")]
    Woodcut,

    [Description("Silkscreen")]
    Silkscreen
}

/// <summary>
/// Common artwork classifications
/// </summary>
public enum ArtworkClassification
{
    [Description("Painting")]
    Painting,

    [Description("Sculpture")]
    Sculpture,

    [Description("Drawing")]
    Drawing,

    [Description("Print")]
    Print,

    [Description("Photograph")]
    Photograph,

    [Description("Textile")]
    Textile,

    [Description("Decorative Arts")]
    DecorativeArts,

    [Description("Installation")]
    Installation,

    [Description("Video")]
    Video,

    [Description("Performance")]
    Performance,

    [Description("Manuscript")]
    Manuscript,

    [Description("Book")]
    Book,

    [Description("Furniture")]
    Furniture,

    [Description("Jewelry")]
    Jewelry,

    [Description("Coin")]
    Coin,

    [Description("Vessel")]
    Vessel,

    [Description("Architectural")]
    Architectural
}

/// <summary>
/// Extension methods for enum descriptions
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets the description attribute value for an enum
    /// </summary>
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                             .FirstOrDefault() as DescriptionAttribute;
        return attribute?.Description ?? value.ToString();
    }
}

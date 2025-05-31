# Artwork Filtering by Style and Medium

This document describes the new convenience methods added to the WebSpark.ArtSpark Client for filtering artworks by style, medium, and classification using strongly-typed enums.

## New Features

### Enums for Better Developer Experience

Three new enums have been added to make filtering artworks easier and more type-safe:

#### `ArtStyle` Enum

Common art styles including:

- Abstract
- Impressionism
- Post-Impressionism
- Cubism
- Fauvism
- Expressionism
- Surrealism
- Pop Art
- Abstract Expressionism
- Minimalism
- Realism
- Romanticism
- Neoclassicism
- Baroque
- Renaissance
- And many more...

#### `ArtMedium` Enum

Common art mediums including:

- Oil on canvas
- Watercolor
- Acrylic
- Tempera
- Pastel
- Charcoal
- Bronze
- Marble
- Photography
- Digital
- Mixed media
- And many more...

#### `ArtworkClassification` Enum

Artwork classifications including:

- Painting
- Sculpture
- Drawing
- Print
- Photograph
- Textile
- Decorative Arts
- Installation
- Video
- Performance
- And many more...

## New Methods

### `GetArtworksByStyleAsync`

Search for artworks by art style.

```csharp
var impressionistWorks = await client.GetArtworksByStyleAsync(
    ArtStyle.Impressionism, 
    limit: 20, 
    page: 1
);
```

### `GetArtworksByMediumAsync`

Search for artworks by medium.

```csharp
var oilPaintings = await client.GetArtworksByMediumAsync(
    ArtMedium.OilOnCanvas, 
    limit: 15
);
```

### `GetArtworksByClassificationAsync`

Search for artworks by classification.

```csharp
var sculptures = await client.GetArtworksByClassificationAsync(
    ArtworkClassification.Sculpture, 
    limit: 10
);
```

### `GetArtworksByStyleAndMediumAsync`

Search for artworks matching both style and medium criteria.

```csharp
var abstractOils = await client.GetArtworksByStyleAndMediumAsync(
    ArtStyle.Abstract,
    ArtMedium.OilOnCanvas,
    limit: 10
);
```

## Usage Examples

### Basic Style Filtering

```csharp
// Get Cubist artworks
var cubistWorks = await client.GetArtworksByStyleAsync(ArtStyle.Cubism);

foreach (var artwork in cubistWorks.Data ?? [])
{
    Console.WriteLine($"{artwork.Title} by {artwork.ArtistDisplay}");
}
```

### Medium Filtering with Pagination

```csharp
// Get watercolor paintings, page 2, 25 per page
var watercolors = await client.GetArtworksByMediumAsync(
    ArtMedium.Watercolor, 
    limit: 25, 
    page: 2
);
```

### Combined Filtering

```csharp
// Get Impressionist oil paintings
var impressionistOils = await client.GetArtworksByStyleAndMediumAsync(
    ArtStyle.Impressionism,
    ArtMedium.OilOnCanvas
);
```

### Discovering Available Options

```csharp
// List all available styles
foreach (ArtStyle style in Enum.GetValues<ArtStyle>())
{
    Console.WriteLine(style.GetDescription());
}

// List all available mediums
foreach (ArtMedium medium in Enum.GetValues<ArtMedium>())
{
    Console.WriteLine(medium.GetDescription());
}
```

## Implementation Details

### Search Strategy

The methods use Elasticsearch queries to search across multiple relevant fields:

- **Style searches** look in: `style_title`, `style_titles`
- **Medium searches** look in: `medium_display`, `material_titles`
- **Classification searches** look in: `classification_title`, `classification_titles`, `artwork_type_title`

### Pagination

All methods support pagination with optional `limit` and `page` parameters:

- `limit`: Maximum results per page (default: 25, max: 100)
- `page`: Page number (default: 1)

### Error Handling

The methods inherit the same robust error handling as the base search methods, including:

- HTTP request exceptions
- JSON parsing exceptions
- Network timeout handling

## Performance Considerations

1. **Field Selection**: Consider using the `fields` parameter in conjunction with these methods to limit response size
2. **Caching**: Results are cached according to the client's cache settings
3. **Rate Limiting**: Respect the API's rate limits (60 requests/minute for anonymous users)

## Examples in Action

See `Examples/ArtworkFilteringExamples.cs` for comprehensive usage examples including:

- Basic filtering by style, medium, and classification
- Pagination examples
- Combined filtering
- Style comparison workflows
- Browsing available filter options

## Migration from Manual Search

### Before (Manual Elasticsearch Query)

```csharp
var query = new SearchQuery
{
    Query = new
    {
        bool_ = new
        {
            should = new[]
            {
                new { match = new { style_title = "Impressionism" } },
                new { match = new { style_titles = "Impressionism" } }
            }
        }
    }
};
var results = await client.SearchArtworksAsync(query);
```

### After (Using New Enum Methods)

```csharp
var results = await client.GetArtworksByStyleAsync(ArtStyle.Impressionism);
```

The new methods provide a much cleaner, more maintainable, and less error-prone way to filter artworks by common criteria.

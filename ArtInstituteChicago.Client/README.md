# ArtInstituteChicago.Client

**The definitive .NET client for the [Art Institute of Chicago API](https://api.artic.edu/docs/)**

---

## Overview

ArtInstituteChicago.Client is a comprehensive, production-grade .NET library for accessing the full breadth of the [Art Institute of Chicago public API](https://api.artic.edu/docs/). It provides strongly-typed, idiomatic C# access to all major endpoints, including Collections, Shop, Mobile, Digital Scholarly Catalogs, Static Archive, and Website resources. This client is designed for reliability, extensibility, and best-in-class developer experience.

---

## Features

- **Full API Coverage:** All endpoints and resource types, including advanced search and batch operations.
- **Strongly-Typed Models:** Rich C# models for every API resource, matching the API schema.
- **Async/Await:** All API calls are asynchronous for scalability.
- **Pagination, Filtering, and Search:** Built-in support for pagination, field selection, and Elasticsearch-style queries.
- **IIIF Image URL Construction:** Utilities for building IIIF image URLs and accessing manifests.
- **Extensible & Testable:** Designed for dependency injection and unit testing.
- **Robust Error Handling:** Clear exception messages and resilient request logic.
- **Open Source & Well-Documented:** MIT licensed and fully documented.

---

## Getting Started

### Installation

```sh
dotnet add package ArtInstituteChicago.Client
```

### Basic Usage

```csharp
using ArtInstituteChicago.Client.Clients;
using ArtInstituteChicago.Client.Interfaces;

var client = new ArtInstituteClient(myHttpRequestResultService); // Inject IHttpRequestResultService

// Fetch artworks
var artworks = await client.GetArtworksAsync();

// Search for artworks by title
var search = await client.SearchArtworksAsync(new SearchQuery { Q = "Monet" });

// Get a single artwork by ID
var artwork = await client.GetArtworkAsync(129884);

// Filter artworks by style using enums
var impressionistWorks = await client.GetArtworksByStyleAsync(ArtStyle.Impressionism, limit: 20);

// Filter artworks by medium
var oilPaintings = await client.GetArtworksByMediumAsync(ArtMedium.OilOnCanvas, limit: 15);

// Filter by classification
var sculptures = await client.GetArtworksByClassificationAsync(ArtworkClassification.Sculpture);

// Combined filtering
var abstractOils = await client.GetArtworksByStyleAndMediumAsync(ArtStyle.Abstract, ArtMedium.OilOnCanvas);

// Build IIIF image URL
string imageUrl = client.BuildIiifUrl("e6e2e6e2-8e2e-4e2e-8e2e-8e2e8e2e8e2e");
```

---

## API Coverage

This client covers all endpoints described in the [official API documentation](https://api.artic.edu/docs/):

- **Collections:** Artworks, Agents, Places, Galleries, Exhibitions, Agent Types, Agent Roles, Artwork Place Qualifiers, Artwork Date Qualifiers, Artwork Types, Category Terms, Images, Videos, Sounds, Texts
- **Shop:** Products
- **Mobile:** Tours, Mobile Sounds
- **Digital Scholarly Catalogs:** Publications, Sections
- **Static Archive:** Sites
- **Website:** Events, Event Occurrences, Event Programs, Articles, Highlights, Static Pages, Generic Pages, Press Releases, Educator Resources, Digital Publications, Digital Publication Articles, Printed Publications

Each resource is accessible via methods on [`IArtInstituteClient`](./Interfaces/IArtInstituteClient.cs).

---

## Best Practices

- **Respect API Terms:** Follow the [Art Institute of Chicago API Terms of Use](https://api.artic.edu/docs/#scraping-data).
- **Avoid Scraping:** Use the API endpoints, not HTML scraping. For large data needs, use [data dumps](https://github.com/art-institute-of-chicago/api-data).
- **Rate Limiting:** Anonymous users are limited to 60 requests/minute. For higher limits, contact [engineering@artic.edu](mailto:engineering@artic.edu).
- **Field Selection:** Use the `fields` parameter to limit response size and improve performance.
- **Batching:** Use the `ids` parameter to fetch multiple resources in one call.
- **Caching:** Cache API responses when possible.
- **Error Handling:** Catch `HttpRequestException` and `JsonException` for robust error management.
- **User-Agent:** Set the `AIC-User-Agent` header with your project name and contact email.
- **IIIF Images:** Use `/full/843,/0/default.jpg` for best performance and caching.
- **Pagination:** Use `page` and `limit` for paginated endpoints. `limit` max is 100; max 10,000 records per search.
- **Dependency Injection:** Register and inject `IArtInstituteClient` for testability and flexibility.

---

## Artwork Filtering by Style and Medium

The client now includes convenient enum-based methods for filtering artworks by common criteria:

### Available Enums

- **`ArtStyle`**: Abstract, Impressionism, Cubism, Surrealism, etc.
- **`ArtMedium`**: Oil on canvas, Watercolor, Bronze, Photography, etc.
- **`ArtworkClassification`**: Painting, Sculpture, Drawing, Print, etc.

### Filter Methods

```csharp
// Filter by art style
var impressionist = await client.GetArtworksByStyleAsync(ArtStyle.Impressionism);

// Filter by medium
var watercolors = await client.GetArtworksByMediumAsync(ArtMedium.Watercolor);

// Filter by classification
var sculptures = await client.GetArtworksByClassificationAsync(ArtworkClassification.Sculpture);

// Combined filtering
var abstractOils = await client.GetArtworksByStyleAndMediumAsync(
    ArtStyle.Abstract, 
    ArtMedium.OilOnCanvas
);
```

See [`Examples/README-StyleAndMediumFiltering.md`](./Examples/README-StyleAndMediumFiltering.md) for comprehensive documentation and examples.

---

## Advanced Usage

### Pagination

```csharp
var response = await client.GetArtworksAsync(new ApiQuery { Page = 2, Limit = 50 });
```

### Field Selection

```csharp
var artwork = await client.GetArtworkAsync(129884, fields: new[] { "id", "title", "artist_display" });
```

### Search

```csharp
var search = await client.SearchArtworksAsync(new SearchQuery { Q = "Impressionism", Size = 10 });
```

### IIIF Image URLs

```csharp
string url = client.BuildIiifUrl("image-id", size: "400,", format: "png");
```

### IIIF Manifests

```csharp
string manifest = await client.GetArtworkManifestAsync(129884);
```

---

## API Reference

See [`IArtInstituteClient`](./Interfaces/IArtInstituteClient.cs) for all available methods and their documentation.

---

## Licensing & Attribution

- Data is licensed under [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) or [CC-BY 4.0](https://creativecommons.org/licenses/by/4.0/) as noted in the API docs.
- See [artic.edu Terms](https://www.artic.edu/terms) and [Image Licensing](https://www.artic.edu/image-licensing) for details.

---

## Support & Contributions

- For questions or support, open an issue on GitHub or contact [engineering@artic.edu](mailto:engineering@artic.edu).
- Contributions are welcome! Please open issues or pull requests for bug fixes, improvements, or new features.

---

## Authors

- [Mark Hazleton](https://github.com/MarkHazleton) and contributors

---

## References

- [Art Institute of Chicago API Documentation](https://api.artic.edu/docs/)
- [IIIF Image API](https://iiif.io/api/image/2.1/)
- [API Data Dumps](https://github.com/art-institute-of-chicago/api-data)

---

*This project is not affiliated with or endorsed by the Art Institute of Chicago. For official support, see the [API documentation](https://api.artic.edu/docs/).*

# Art Institute of Chicago API Client

**A comprehensive .NET client library for the Art Institute of Chicago's public API, providing access to all available endpoints across Collections, Shop, Mobile, Digital Scholarly Catalogs, Static Archive, and Website resources.**

[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Quick Start](#quick-start)
- [API Coverage](#api-coverage)
- [Usage Examples](#usage-examples)
- [Project Structure](#project-structure)
- [Models](#models)
- [Client Configuration](#client-configuration)
- [Development](#development)
- [License](#license)

## Overview

This solution provides a complete .NET client library and console application for interacting with the Art Institute of Chicago's public REST API. The library covers **all 33 API endpoints** across 6 major categories, enabling developers to build rich applications with access to the museum's extensive digital collections and resources.

## Features

- ✅ **Complete API Coverage**: All 33 endpoints across 6 categories
- ✅ **Strongly Typed Models**: Comprehensive C# models for all resource types
- ✅ **Async/Await Support**: Modern asynchronous programming patterns
- ✅ **JSON Deserialization**: Using `System.Text.Json` with proper naming policies
- ✅ **Search Capabilities**: Full-text search with Elasticsearch integration
- ✅ **IIIF Support**: Built-in IIIF URL construction for high-quality images
- ✅ **Flexible Querying**: Pagination, field selection, and resource inclusion
- ✅ **Error Handling**: Graceful error handling and HTTP status management
- ✅ **No External Dependencies**: Uses only .NET 9.0 built-in libraries

## Quick Start

### Installation

1. Clone the repository:

```bash
git clone https://github.com/MarkHazleton/ArtInstituteChicago.git
cd ArtInstituteChicago
```

1. Build the solution:

```bash
dotnet build
```

1. Run the console demo:

```bash
dotnet run --project ArtInstituteChicago.Console
```

### Basic Usage

```csharp
using ArtInstituteChicago.Client.Clients;
using ArtInstituteChicago.Client.Models.Common;

// Create HTTP client and API client
var httpClient = new HttpClient();
var client = new ArtInstituteClient(httpClient);

// Get artworks with pagination
var query = new ApiQuery { Limit = 10, Page = 1 };
var response = await client.GetArtworksAsync(query);

// Display results
foreach (var artwork in response.Data ?? [])
{
    Console.WriteLine($"{artwork.Title} (ID: {artwork.Id})");
    if (!string.IsNullOrEmpty(artwork.ImageId))
    {
        var imageUrl = client.BuildIiifUrl(artwork.ImageId, "843,");
        Console.WriteLine($"  Image: {imageUrl}");
    }
}
```

## API Coverage

The client provides complete coverage of the Art Institute of Chicago API:

### Collections (15 endpoints)

- **Artworks** - `/artworks` - The museum's collection of artworks
- **Agents** - `/agents` - Artists, creators, and other people/organizations
- **Places** - `/places` - Geographic locations related to artworks
- **Galleries** - `/galleries` - Museum gallery spaces
- **Exhibitions** - `/exhibitions` - Past and current exhibitions
- **Agent Types** - `/agent-types` - Classification of agents
- **Agent Roles** - `/agent-roles` - Roles agents play in relation to artworks
- **Artwork Place Qualifiers** - `/artwork-place-qualifiers` - Geographic relationship qualifiers
- **Artwork Date Qualifiers** - `/artwork-date-qualifiers` - Temporal relationship qualifiers
- **Artwork Types** - `/artwork-types` - Classification of artworks
- **Category Terms** - `/category-terms` - Hierarchical categories
- **Images** - `/images` - Digital images
- **Videos** - `/videos` - Video content
- **Sounds** - `/sounds` - Audio content
- **Texts** - `/texts` - Textual content

### Shop (1 endpoint)

- **Products** - `/products` - Museum shop products

### Mobile (2 endpoints)

- **Tours** - `/tours` - Self-guided mobile tours
- **Mobile Sounds** - `/mobile-sounds` - Audio content for mobile tours

### Digital Scholarly Catalogs (2 endpoints)

- **Publications** - `/publications` - Scholarly digital publications
- **Sections** - `/sections` - Publication sections and articles

### Static Archive (1 endpoint)

- **Sites** - `/sites` - Archived websites and digital content

### Website (12 endpoints)

- **Events** - `/events` - Museum events and programs
- **Event Occurrences** - `/event-occurrences` - Specific event instances
- **Event Programs** - `/event-programs` - Event program categories
- **Articles** - `/articles` - Website articles and blog posts
- **Highlights** - `/highlights` - Featured content
- **Static Pages** - `/static-pages` - Website static content
- **Generic Pages** - `/generic-pages` - General website pages
- **Press Releases** - `/press-releases` - Press releases and media
- **Educator Resources** - `/educator-resources` - Educational materials
- **Digital Publications** - `/digital-publications` - Online publications
- **Digital Publication Articles** - `/digital-publication-sections` - Publication articles
- **Printed Publications** - `/printed-publications` - Print publications

## Usage Examples

### Search for Artworks

```csharp
using ArtInstituteChicago.Client.Models.Common;

// Full-text search
var searchQuery = new SearchQuery 
{ 
    Q = "Van Gogh", 
    Size = 20,
    Fields = "id,title,artist_display,date_display,image_id"
};

var searchResults = await client.SearchArtworksAsync(searchQuery);

foreach (var artwork in searchResults.Data ?? [])
{
    Console.WriteLine($"{artwork.Title} - {artwork.ArtistDisplay}");
}
```

### Get Specific Fields

```csharp
// Request only specific fields
var artwork = await client.GetArtworkAsync(
    id: 20684, 
    fields: new[] { "id", "title", "artist_display", "date_display", "image_id" }
);

if (artwork != null)
{
    Console.WriteLine($"Title: {artwork.Title}");
    Console.WriteLine($"Artist: {artwork.ArtistDisplay}");
    Console.WriteLine($"Date: {artwork.DateDisplay}");
}
```

### Work with IIIF Images

```csharp
var artwork = await client.GetArtworkAsync(20684);
if (!string.IsNullOrEmpty(artwork?.ImageId))
{
    // Get different image sizes
    var thumbnailUrl = client.BuildIiifUrl(artwork.ImageId, "200,", "jpg");
    var mediumUrl = client.BuildIiifUrl(artwork.ImageId, "843,", "jpg");
    var largeUrl = client.BuildIiifUrl(artwork.ImageId, "1686,", "jpg");
    
    Console.WriteLine($"Thumbnail: {thumbnailUrl}");
    Console.WriteLine($"Medium: {mediumUrl}");
    Console.WriteLine($"Large: {largeUrl}");
}
```

### Browse Exhibitions

```csharp
var query = new ApiQuery { Limit = 10 };
var exhibitions = await client.GetExhibitionsAsync(query);

foreach (var exhibition in exhibitions.Data ?? [])
{
    Console.WriteLine($"{exhibition.Title}");
    Console.WriteLine($"  Status: {exhibition.Status}");
    Console.WriteLine($"  Dates: {exhibition.DateDisplay}");
}
```

### Get Related Resources

```csharp
// Get artwork with related agents and galleries
var artwork = await client.GetArtworkAsync(
    id: 20684,
    include: new[] { "agent", "gallery" }
);

if (artwork != null)
{
    Console.WriteLine($"Gallery: {artwork.GalleryTitle}");
    // Access included related data through the response
}
```

### Batch Operations

```csharp
// Get multiple artworks by IDs
var artworkIds = new object[] { 20684, 229393, 125592 };
var batchResponse = await client.GetResourcesByIdsAsync<ArtWork>(
    "artworks", 
    artworkIds, 
    fields: new[] { "id", "title", "artist_display" }
);

foreach (var artwork in batchResponse.Data ?? [])
{
    Console.WriteLine($"{artwork.Title} - {artwork.ArtistDisplay}");
}
```

### Work with Other Resources

```csharp
// Browse museum events
var events = await client.GetEventsAsync(new ApiQuery { Limit = 5 });

// Search for educational resources
var eduResources = await client.SearchEducatorResourcesAsync(
    new SearchQuery { Q = "impressionism", Size = 10 }
);

// Get mobile tours
var tours = await client.GetToursAsync();

// Browse shop products
var products = await client.GetProductsAsync(new ApiQuery { Limit = 20 });
```

## Project Structure

```text
ArtInstituteChicago/
├── ArtInstituteChicago.Client/          # Main client library
│   ├── Clients/
│   │   └── ArtInstituteClient.cs        # Complete API client implementation
│   ├── Interfaces/
│   │   └── IArtInstituteClient.cs       # Client interface
│   └── Models/
│       ├── Collections/                 # Collection resource models
│       ├── Shop/                        # Shop resource models
│       ├── Mobile/                      # Mobile app resource models
│       ├── DigitalScholarlyCalatogs/    # Scholarly publication models
│       ├── StaticArchive/               # Archive resource models
│       ├── Website/                     # Website resource models
│       └── Common/                      # Shared models and utilities
├── ArtInstituteChicago.Console/         # Demo console application
└── README.md                           # This file
```

## Models

The library includes comprehensive models for all API resources:

### Core Models

- `ArtWork` - Museum artworks and objects
- `Agent` - Artists, creators, organizations
- `Gallery` - Museum gallery spaces
- `Exhibition` - Museum exhibitions
- `Place` - Geographic locations

### Content Models

- `Image`, `Video`, `Sound`, `Text` - Digital media resources
- `Tour`, `MobileSound` - Mobile app content
- `Publication`, `Section` - Scholarly publications

### Website Models

- `Article`, `Event`, `Highlight` - Website content
- `EducatorResource` - Educational materials
- `PressRelease` - Media and communications

### Query Models

- `ApiQuery` - Standard API query parameters
- `SearchQuery` - Search-specific parameters
- `ApiResponse<T>` - Standard API responses
- `SearchResponse<T>` - Search responses

## Client Configuration

### HTTP Client Configuration

```csharp
var httpClientHandler = new HttpClientHandler();
var httpClient = new HttpClient(httpClientHandler)
{
    Timeout = TimeSpan.FromSeconds(30)
};

// Optional: Add custom headers
httpClient.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");

var client = new ArtInstituteClient(httpClient);
```

### JSON Serialization

The client automatically configures JSON serialization with:

- Snake case naming policy (e.g., `date_display`)
- Null value ignoring
- Case-insensitive property names

## Development

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

### Development Setup

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Make your changes
4. Add tests if applicable
5. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
6. Push to the branch (`git push origin feature/AmazingFeature`)
7. Open a Pull Request

### Acknowledgments

- [Art Institute of Chicago](https://www.artic.edu/) for providing the public API
- [Art Institute of Chicago API Documentation](https://api.artic.edu/docs/)
- The museum's commitment to open access and digital scholarship

### Support

If you encounter any issues or have questions:

1. Check the [API documentation](https://api.artic.edu/docs/)
2. Search existing [GitHub issues](https://github.com/MarkHazleton/ArtInstituteChicago/issues)
3. Create a new issue if needed

## License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

## Final Note

Happy coding with the Art Institute of Chicago API! 🎨

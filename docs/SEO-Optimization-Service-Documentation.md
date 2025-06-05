# SEO Optimization Service Documentation

The SEO Optimization Service provides AI-powered SEO optimization for art collections and individual artworks within the WebSpark.ArtSpark application.

## Overview

The service uses AI (via Semantic Kernel and OpenAI) to generate comprehensive, SEO-optimized content including:

- Meta titles and descriptions
- SEO-friendly slugs
- Engaging descriptions
- Professional curator notes
- Relevant keywords and tags

## API Endpoints

### 1. Optimize Collection

**Endpoint:** `POST /api/SeoOptimization/optimize-collection`

**Description:** Takes a collection description and returns a fully optimized UserCollection instance.

**Request Body:**

```json
{
  "description": "A curated collection of Renaissance paintings featuring works by Leonardo da Vinci, Michelangelo, and Raphael. This collection showcases the evolution of artistic techniques during the Italian Renaissance period."
}
```

**Response:**

```json
{
  "success": true,
  "collection": {
    "name": "Renaissance Masters: Da Vinci, Michelangelo & Raphael",
    "description": "Explore the artistic evolution of the Italian Renaissance through masterpieces by Leonardo da Vinci, Michelangelo, and Raphael, showcasing revolutionary techniques and timeless themes.",
    "longDescription": "This exceptional collection brings together the greatest masterworks of the Italian Renaissance, featuring paintings by the three most influential artists of the period: Leonardo da Vinci, Michelangelo, and Raphael. Journey through the artistic revolution that transformed European art, exploring innovative techniques in perspective, anatomy, and composition. From religious devotion to mythological narratives, these works demonstrate the Renaissance ideals of humanism, beauty, and technical mastery that continue to inspire artists and art lovers today.",
    "slug": "renaissance-masters-da-vinci-michelangelo-raphael",
    "metaTitle": "Renaissance Masters: Da Vinci, Michelangelo & Raphael Collection",
    "metaDescription": "Discover Italian Renaissance masterpieces by Leonardo da Vinci, Michelangelo, and Raphael. Explore revolutionary artistic techniques and timeless themes.",
    "metaKeywords": "renaissance art, leonardo da vinci, michelangelo, raphael, italian painting, religious art, mythological art, art history",
    "tags": "Renaissance, Italian Art, Leonardo da Vinci, Michelangelo, Raphael, Religious Art, Mythological Art, Art History, Painting Techniques",
    "curatorNotes": "This collection represents the pinnacle of Renaissance artistic achievement, showcasing the distinct styles and innovations of three masters who forever changed the course of Western art.",
    "isPublic": true
  },
  "errorMessage": null
}
```

### 2. Optimize Artwork

**Endpoint:** `POST /api/SeoOptimization/optimize-artwork`

**Description:** Takes artwork data and collection context to return an optimized CollectionArtwork instance.

**Request Body:**

```json
{
  "artworkData": {
    "id": 123,
    "title": "The Starry Night",
    "artistDisplay": "Vincent van Gogh",
    "dateDisplay": "1889",
    "medium": "Oil on canvas",
    "dimensions": "73.7 cm × 92.1 cm (29 in × 36¼ in)",
    "placeOfOrigin": "France",
    "description": "A masterpiece depicting swirling clouds and a bright crescent moon illuminating a sleeping village below.",
    "culturalContext": "Post-Impressionist painting",
    "styleTitle": "Post-Impressionism",
    "classification": "Painting"
  },
  "collectionId": 456
}
```

**Response:**

```json
{
  "success": true,
  "collectionArtwork": {
    "collectionId": 456,
    "artworkId": 123,
    "slug": "van-gogh-starry-night-swirling-cosmos",
    "customTitle": "The Starry Night: Van Gogh's Swirling Cosmos",
    "customDescription": "Van Gogh's iconic masterpiece transforms the night sky into a dynamic, swirling cosmos above a peaceful village. Created during his time at the asylum in Saint-Rémy, this 1889 oil painting showcases the artist's unique Post-Impressionist style with bold brushstrokes and vibrant blues and yellows that capture both movement and emotion in the celestial sphere.",
    "curatorNotes": "Painted during van Gogh's stay at the Saint-Paul-de-Mausole asylum, The Starry Night represents the artist's emotional and spiritual connection to the cosmos. The painting's innovative use of impasto technique and dynamic brushwork creates a sense of movement that was revolutionary for its time, influencing generations of modern artists.",
    "metaTitle": "The Starry Night by Van Gogh - Post-Impressionist Masterpiece",
    "metaDescription": "Explore van Gogh's iconic Starry Night (1889) - a swirling cosmos masterpiece showcasing Post-Impressionist innovation and emotional depth.",
    "displayOrder": 0
  },
  "errorMessage": null
}
```

## Error Responses

**400 Bad Request:**

```json
{
  "success": false,
  "collection": null,
  "errorMessage": "Invalid request data"
}
```

**401 Unauthorized:**

```json
{
  "success": false,
  "collection": null,
  "errorMessage": "Authentication required"
}
```

**500 Internal Server Error:**

```json
{
  "success": false,
  "collection": null,
  "errorMessage": "An error occurred while optimizing the collection"
}
```

## Authentication

Both endpoints require user authentication. Include a valid authentication token in your request headers.

## Usage Examples

### JavaScript/Fetch

```javascript
// Optimize Collection
const optimizeCollection = async (description) => {
  const response = await fetch('/api/SeoOptimization/optimize-collection', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer YOUR_TOKEN_HERE'
    },
    body: JSON.stringify({ description })
  });
  
  return await response.json();
};

// Optimize Artwork
const optimizeArtwork = async (artworkData, collectionId) => {
  const response = await fetch('/api/SeoOptimization/optimize-artwork', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer YOUR_TOKEN_HERE'
    },
    body: JSON.stringify({ artworkData, collectionId })
  });
  
  return await response.json();
};
```

### C# HttpClient

```csharp
public class SeoOptimizationClient
{
    private readonly HttpClient _httpClient;
    
    public SeoOptimizationClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<OptimizeCollectionResponse> OptimizeCollectionAsync(string description)
    {
        var request = new OptimizeCollectionRequest { Description = description };
        var response = await _httpClient.PostAsJsonAsync("/api/SeoOptimization/optimize-collection", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OptimizeCollectionResponse>();
    }
    
    public async Task<OptimizeArtworkResponse> OptimizeArtworkAsync(ArtworkData artworkData, int collectionId)
    {
        var request = new OptimizeArtworkRequest { ArtworkData = artworkData, CollectionId = collectionId };
        var response = await _httpClient.PostAsJsonAsync("/api/SeoOptimization/optimize-artwork", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OptimizeArtworkResponse>();
    }
}
```

## Configuration

The service uses the existing ArtSpark Agent configuration for AI operations. Ensure your `appsettings.json` includes:

```json
{
  "ArtSparkAgent": {
    "OpenAI": {
      "ApiKey": "your-openai-api-key",
      "ModelId": "gpt-4o",
      "Temperature": 0.7
    }
  }
}
```

## Features

- **AI-Powered Content Generation**: Uses advanced language models to create engaging, professional content
- **SEO Optimization**: Generates meta titles, descriptions, and keywords optimized for search engines
- **Character Limit Compliance**: Respects SEO best practices for character limits
- **Slug Generation**: Creates SEO-friendly URL slugs automatically
- **Professional Curation**: Provides expert-level curatorial insights and context
- **Error Handling**: Comprehensive error handling with meaningful error messages
- **Logging**: Full logging support for monitoring and debugging

## Implementation Notes

- The service requires the ArtSpark Agent to be configured with valid OpenAI credentials
- All generated content respects SEO character limits (60 chars for titles, 160 for descriptions)
- Slugs are automatically generated using the existing SlugGenerator utility
- The service is registered as a scoped dependency in the DI container
- Authentication is required for all endpoints to ensure proper user context

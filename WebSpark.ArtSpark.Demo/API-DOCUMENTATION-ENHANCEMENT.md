# API Documentation Enhancement Guide

## Overview

This document provides comprehensive API documentation enhancements for the WebSpark.ArtSpark application to improve developer experience and API discoverability.

## 1. OpenAPI/Swagger Enhancement

### Add Package References

Add these to your `.csproj` file:

```xml
<PackageReference Include="Swashbuckle.AspNetCore" Version="7.2.0" />
<PackageReference Include="Swashbuckle.AspNetCore.Annotations" Version="7.2.0" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.ApiExplorer" Version="9.0.6" />
```

### Enhanced Swagger Configuration

Add to `Program.cs`:

```csharp
// API Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0.3",
        Title = "WebSpark ArtSpark API",
        Description = "Art exploration platform with AI-powered interactions using Art Institute of Chicago API",
        Contact = new OpenApiContact
        {
            Name = "WebSpark Solutions",
            Email = "contact@webspark.com",
            Url = new Uri("https://github.com/MarkHazleton/WebSpark.ArtSpark")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Include XML comments for better documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Enable annotations for enhanced documentation
    options.EnableAnnotations();

    // Add security definitions for JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Custom operation filter for better API descriptions
    options.OperationFilter<ResponseTypesOperationFilter>();
});

// Configure in middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebSpark ArtSpark API v1");
        options.RoutePrefix = "docs/api";
        options.DocumentTitle = "WebSpark ArtSpark API Documentation";
        options.DefaultModelsExpandDepth(1);
        options.DefaultModelExpandDepth(1);
        options.DisplayRequestDuration();
        options.EnableTryItOutByDefault();
    });
}
```

## 2. API Response Models

Create standardized response models for consistency:

### ApiResponse<T> Model

```csharp
/// <summary>
/// Standardized API response wrapper
/// </summary>
/// <typeparam name="T">Type of the response data</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates if the request was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response data
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Error message if the request failed
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Additional error details for debugging
    /// </summary>
    public object? Errors { get; set; }

    /// <summary>
    /// Timestamp of the response
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Request correlation ID for tracing
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Pagination information (if applicable)
    /// </summary>
    public PaginationInfo? Pagination { get; set; }

    public static ApiResponse<T> SuccessResult(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            CorrelationId = Activity.Current?.TraceId.ToString()
        };
    }

    public static ApiResponse<T> ErrorResult(string message, object? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors,
            CorrelationId = Activity.Current?.TraceId.ToString()
        };
    }
}

/// <summary>
/// Pagination information for list responses
/// </summary>
public class PaginationInfo
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
}
```

## 3. API Versioning Strategy

### Add Package Reference

```xml
<PackageReference Include="Microsoft.AspNetCore.Mvc.Versioning" Version="5.1.0" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer" Version="5.1.0" />
```

### Configuration

```csharp
// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("version"),
        new HeaderApiVersionReader("X-Version"),
        new UrlSegmentApiVersionReader()
    );
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
```

## 4. Enhanced Controller Documentation

Add comprehensive XML documentation to controllers:

```csharp
/// <summary>
/// Artwork management endpoints
/// Provides access to artwork data from the Art Institute of Chicago API
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
public class ArtworkApiController : ControllerBase
{
    /// <summary>
    /// Get artwork details by ID
    /// </summary>
    /// <param name="id">The artwork identifier</param>
    /// <param name="includeImages">Include image URLs in response</param>
    /// <returns>Detailed artwork information</returns>
    /// <response code="200">Artwork found and returned successfully</response>
    /// <response code="404">Artwork not found</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ArtworkDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get artwork by ID",
        Description = "Retrieves detailed information about a specific artwork including metadata, images, and related information",
        OperationId = "GetArtworkById",
        Tags = new[] { "Artwork" }
    )]
    public async Task<IActionResult> GetArtwork(
        [FromRoute] int id,
        [FromQuery] bool includeImages = true)
    {
        // Implementation
    }
}
```

## 5. Health Check API Documentation

Document health check endpoints:

```csharp
/// <summary>
/// System health and monitoring endpoints
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "monitoring")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Basic health check endpoint
    /// </summary>
    /// <returns>System health status</returns>
    [HttpGet]
    [ProducesResponseType(typeof(HealthCheckResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthCheckResult), StatusCodes.Status503ServiceUnavailable)]
    [SwaggerOperation(
        Summary = "Check system health",
        Description = "Returns the overall health status of the application and its dependencies"
    )]
    public async Task<IActionResult> Check()
    {
        // Implementation
    }
}
```

## 6. Rate Limiting Documentation

Document rate limiting policies:

```csharp
/// <summary>
/// Rate limiting: 100 requests per minute per IP address
/// Burst allowance: 10 requests in 10 seconds
/// </summary>
[EnableRateLimiting("DefaultPolicy")]
```

## 7. Error Response Documentation

Create standardized error response models:

```csharp
/// <summary>
/// Standard error response model
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Error code for programmatic handling
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Detailed error information for debugging
    /// </summary>
    public object? Details { get; set; }

    /// <summary>
    /// Request correlation ID for tracing
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Timestamp when the error occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
```

## 8. Next Steps

1. **Add XML Documentation Generation**

   ```xml
   <PropertyGroup>
     <GenerateDocumentationFile>true</GenerateDocumentationFile>
     <DocumentationFile>WebSpark.ArtSpark.Demo.xml</DocumentationFile>
   </PropertyGroup>
   ```

2. **Implement API Testing**
   - Add integration tests for all API endpoints
   - Use TestServer for testing API responses
   - Validate OpenAPI specification compliance

3. **API Performance Monitoring**
   - Track API response times
   - Monitor rate limiting effectiveness
   - Implement API usage analytics

4. **Security Enhancement**
   - Add API key authentication for external consumers
   - Implement CORS policies for API access
   - Add request/response logging for auditing

5. **Developer Experience**
   - Create SDK for common programming languages
   - Provide Postman collection for API testing
   - Add GraphQL endpoint for flexible data querying

This comprehensive API documentation enhancement will significantly improve the developer experience and make your API more discoverable and easier to use.

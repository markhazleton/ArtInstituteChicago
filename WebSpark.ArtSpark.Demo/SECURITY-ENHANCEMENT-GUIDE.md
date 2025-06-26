# Security Enhancement Guide

## Overview

This guide provides comprehensive security enhancements for the WebSpark.ArtSpark application to protect against common vulnerabilities and implement security best practices.

## 1. Security Headers Implementation

### Enhanced Security Headers Middleware

```csharp
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;

    public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Remove potentially dangerous headers
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");

        // Security headers
        var headers = new Dictionary<string, string>
        {
            ["X-Content-Type-Options"] = "nosniff",
            ["X-Frame-Options"] = "DENY",
            ["X-XSS-Protection"] = "1; mode=block",
            ["Referrer-Policy"] = "strict-origin-when-cross-origin",
            ["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=()",
            ["X-Download-Options"] = "noopen",
            ["X-Permitted-Cross-Domain-Policies"] = "none"
        };

        // Content Security Policy
        var csp = BuildContentSecurityPolicy(context);
        headers["Content-Security-Policy"] = csp;

        // HTTP Strict Transport Security (HTTPS only)
        if (context.Request.IsHttps)
        {
            headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
        }

        foreach (var header in headers)
        {
            context.Response.Headers.Append(header.Key, header.Value);
        }

        await _next(context);
    }

    private string BuildContentSecurityPolicy(HttpContext context)
    {
        var isDevelopment = context.RequestServices
            .GetService<IWebHostEnvironment>()?.IsDevelopment() ?? false;

        var basePolicy = new[]
        {
            "default-src 'self'",
            "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com",
            "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://fonts.googleapis.com",
            "img-src 'self' data: https: blob:",
            "font-src 'self' https://fonts.gstatic.com https://cdn.jsdelivr.net",
            "connect-src 'self' https://api.artic.edu",
            "media-src 'self' https:",
            "object-src 'none'",
            "base-uri 'self'",
            "form-action 'self'",
            "frame-ancestors 'none'",
            "upgrade-insecure-requests"
        };

        if (isDevelopment)
        {
            // Allow localhost connections in development
            var devPolicy = basePolicy.Select(directive =>
                directive.Contains("connect-src") 
                    ? directive + " ws://localhost:* wss://localhost:* https://localhost:*"
                    : directive
            );
            return string.Join("; ", devPolicy);
        }

        return string.Join("; ", basePolicy);
    }
}
```

## 2. Input Validation and Sanitization

### Model Validation Attributes

```csharp
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Custom validation attributes for enhanced security
/// </summary>
public class SafeStringAttribute : ValidationAttribute
{
    private readonly string[] _forbiddenPatterns = 
    {
        "<script", "</script>", "javascript:", "vbscript:", "onload=", "onerror=",
        "eval(", "setTimeout(", "setInterval(", "document.cookie"
    };

    public override bool IsValid(object? value)
    {
        if (value is not string stringValue)
            return true;

        var lowerValue = stringValue.ToLowerInvariant();
        return !_forbiddenPatterns.Any(pattern => lowerValue.Contains(pattern));
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The {name} field contains potentially unsafe content.";
    }
}

/// <summary>
/// Validate file upload security
/// </summary>
public class SafeFileAttribute : ValidationAttribute
{
    private readonly string[] _allowedExtensions;
    private readonly long _maxFileSize;

    public SafeFileAttribute(string allowedExtensions, long maxFileSizeBytes = 5 * 1024 * 1024)
    {
        _allowedExtensions = allowedExtensions.Split(',').Select(ext => ext.Trim().ToLowerInvariant()).ToArray();
        _maxFileSize = maxFileSizeBytes;
    }

    public override bool IsValid(object? value)
    {
        if (value is not IFormFile file)
            return true;

        // Check file size
        if (file.Length > _maxFileSize)
        {
            ErrorMessage = $"File size cannot exceed {_maxFileSize / (1024 * 1024)} MB.";
            return false;
        }

        // Check file extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            ErrorMessage = $"Only files with extensions {string.Join(", ", _allowedExtensions)} are allowed.";
            return false;
        }

        // Check MIME type matches extension
        var expectedMimeTypes = GetMimeTypesForExtension(extension);
        if (!expectedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
        {
            ErrorMessage = "File content does not match its extension.";
            return false;
        }

        return true;
    }

    private string[] GetMimeTypesForExtension(string extension)
    {
        return extension switch
        {
            ".jpg" or ".jpeg" => new[] { "image/jpeg" },
            ".png" => new[] { "image/png" },
            ".gif" => new[] { "image/gif" },
            ".pdf" => new[] { "application/pdf" },
            ".txt" => new[] { "text/plain" },
            _ => Array.Empty<string>()
        };
    }
}
```

## 3. Authentication and Authorization Enhancements

### JWT Token Security

```csharp
public class JwtSecurityService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtSecurityService> _logger;

    public JwtSecurityService(IConfiguration configuration, ILogger<JwtSecurityService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GenerateToken(ApplicationUser user, IList<string> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"] ?? 
            throw new InvalidOperationException("JWT Secret not configured"));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new("jti", Guid.NewGuid().ToString()), // JWT ID for revocation
            new("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15), // Short-lived access token
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["JWT:Issuer"],
            Audience = _configuration["JWT:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["JWT:Secret"] ?? string.Empty)),
            ValidateLifetime = false // Don't validate lifetime for refresh scenarios
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}
```

## 4. Rate Limiting and DDoS Protection

### Enhanced Rate Limiting Configuration

```csharp
public static class RateLimitingExtensions
{
    public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Global rate limit
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                httpContext => RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1)
                    }));

            // API endpoints - stricter limits
            options.AddFixedWindowLimiter("ApiPolicy", options =>
            {
                options.PermitLimit = 50;
                options.Window = TimeSpan.FromMinutes(1);
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 5;
            });

            // Authentication endpoints - very strict
            options.AddFixedWindowLimiter("AuthPolicy", options =>
            {
                options.PermitLimit = 5;
                options.Window = TimeSpan.FromMinutes(1);
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 0;
            });

            // Sliding window for authenticated users
            options.AddSlidingWindowLimiter("AuthenticatedPolicy", options =>
            {
                options.PermitLimit = 200;
                options.Window = TimeSpan.FromMinutes(1);
                options.SegmentsPerWindow = 4;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 10;
            });

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter = 
                        ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                }

                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    error = "rate_limit_exceeded",
                    message = "Rate limit exceeded. Please try again later.",
                    retryAfter = retryAfter?.TotalSeconds
                }), cancellationToken: token);
            };
        });

        return services;
    }
}
```

## 5. Data Protection and Encryption

### Enhanced Data Protection Configuration

```csharp
public static class DataProtectionExtensions
{
    public static IServiceCollection AddCustomDataProtection(this IServiceCollection services, IConfiguration configuration)
    {
        var dataProtectionBuilder = services.AddDataProtection()
            .SetApplicationName("WebSpark.ArtSpark")
            .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

        // Use Azure Key Vault in production
        var keyVaultUrl = configuration["DataProtection:KeyVault:Url"];
        if (!string.IsNullOrEmpty(keyVaultUrl))
        {
            dataProtectionBuilder.PersistKeysToAzureBlobStorage(
                configuration.GetConnectionString("DataProtectionStorage"),
                "keys", "dataprotection-keys.xml");
        }
        else
        {
            // Use file system in development
            var keysPath = Path.Combine(Directory.GetCurrentDirectory(), "keys");
            Directory.CreateDirectory(keysPath);
            dataProtectionBuilder.PersistKeysToFileSystem(new DirectoryInfo(keysPath));
        }

        // Encrypt keys at rest
        dataProtectionBuilder.ProtectKeysWithDpapi(protectToLocalMachine: true);

        return services;
    }
}
```

## 6. Secure File Upload Handling

### File Upload Security Service

```csharp
public class SecureFileUploadService
{
    private readonly ILogger<SecureFileUploadService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string[] _allowedMimeTypes = 
    {
        "image/jpeg", "image/png", "image/gif", "application/pdf", "text/plain"
    };

    public SecureFileUploadService(ILogger<SecureFileUploadService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<UploadResult> UploadFileAsync(IFormFile file, string userId)
    {
        try
        {
            // Validate file
            var validationResult = await ValidateFileAsync(file);
            if (!validationResult.IsValid)
            {
                return UploadResult.Failure(validationResult.ErrorMessage);
            }

            // Generate secure file name
            var safeFileName = GenerateSecureFileName(file.FileName);
            var filePath = Path.Combine(GetUploadDirectory(userId), safeFileName);

            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            // Scan file content (basic malware detection)
            var scanResult = await ScanFileContentAsync(file);
            if (!scanResult.IsClean)
            {
                _logger.LogWarning("Potentially malicious file upload attempt by user {UserId}: {FileName}", 
                    userId, file.FileName);
                return UploadResult.Failure("File failed security scan");
            }

            // Save file
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await file.CopyToAsync(fileStream);

            _logger.LogInformation("File uploaded successfully: {FilePath} by user {UserId}", 
                filePath, userId);

            return UploadResult.Success(safeFileName, filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file for user {UserId}", userId);
            return UploadResult.Failure("File upload failed");
        }
    }

    private async Task<FileValidationResult> ValidateFileAsync(IFormFile file)
    {
        // Check file size
        var maxFileSize = _configuration.GetValue<long>("FileUpload:MaxSizeBytes", 5 * 1024 * 1024);
        if (file.Length > maxFileSize)
        {
            return FileValidationResult.Invalid($"File size exceeds {maxFileSize / (1024 * 1024)} MB limit");
        }

        // Check MIME type
        if (!_allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
        {
            return FileValidationResult.Invalid("File type not allowed");
        }

        // Read file header to verify actual file type
        using var stream = file.OpenReadStream();
        var buffer = new byte[512];
        await stream.ReadAsync(buffer, 0, buffer.Length);
        
        var actualMimeType = GetMimeTypeFromHeader(buffer);
        if (actualMimeType != file.ContentType.ToLowerInvariant())
        {
            return FileValidationResult.Invalid("File content does not match declared type");
        }

        return FileValidationResult.Valid();
    }

    private async Task<ScanResult> ScanFileContentAsync(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var buffer = new byte[1024];
        await stream.ReadAsync(buffer, 0, buffer.Length);

        // Basic pattern matching for malicious content
        var content = Encoding.UTF8.GetString(buffer).ToLowerInvariant();
        var maliciousPatterns = new[] 
        { 
            "<script", "javascript:", "eval(", "system(", "exec("
        };

        foreach (var pattern in maliciousPatterns)
        {
            if (content.Contains(pattern))
            {
                return ScanResult.Malicious($"Detected pattern: {pattern}");
            }
        }

        return ScanResult.Clean();
    }

    private string GenerateSecureFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var fileName = Path.GetFileNameWithoutExtension(originalFileName);
        
        // Sanitize filename
        var sanitized = string.Concat(fileName.Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '-'));
        if (string.IsNullOrEmpty(sanitized))
        {
            sanitized = "file";
        }

        // Add timestamp and random component
        var timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMdd_HHmmss");
        var random = Path.GetRandomFileName()[..8];
        
        return $"{sanitized}_{timestamp}_{random}{extension}";
    }

    private string GetUploadDirectory(string userId)
    {
        var baseDir = _configuration.GetValue<string>("FileUpload:BasePath", "uploads");
        return Path.Combine(baseDir, userId);
    }

    private string GetMimeTypeFromHeader(byte[] header)
    {
        // Simple file type detection based on file headers
        if (header.Length >= 4)
        {
            // JPEG
            if (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
                return "image/jpeg";
            
            // PNG
            if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47)
                return "image/png";
            
            // GIF
            if (header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46)
                return "image/gif";
            
            // PDF
            if (header[0] == 0x25 && header[1] == 0x50 && header[2] == 0x44 && header[3] == 0x46)
                return "application/pdf";
        }

        return "application/octet-stream";
    }
}

public record FileValidationResult(bool IsValid, string ErrorMessage)
{
    public static FileValidationResult Valid() => new(true, string.Empty);
    public static FileValidationResult Invalid(string error) => new(false, error);
}

public record ScanResult(bool IsClean, string Message)
{
    public static ScanResult Clean() => new(true, "Clean");
    public static ScanResult Malicious(string message) => new(false, message);
}

public record UploadResult(bool Success, string Message, string? FileName = null, string? FilePath = null)
{
    public static UploadResult Success(string fileName, string filePath) => 
        new(true, "Upload successful", fileName, filePath);
    public static UploadResult Failure(string message) => new(false, message);
}
```

## 7. Security Configuration in Program.cs

```csharp
// Security Enhancements
builder.Services.AddCustomDataProtection(builder.Configuration);
builder.Services.AddCustomRateLimiting();

// Security Headers
app.UseSecurityHeaders();

// Rate Limiting
app.UseRateLimiter();

// HTTPS Redirection (ensure this is early in pipeline)
app.UseHttpsRedirection();
```

## 8. Security Monitoring and Logging

### Security Event Logging

```csharp
public static class SecurityEvents
{
    public static void LogFailedLogin(ILogger logger, string username, string ipAddress)
    {
        logger.LogWarning("Failed login attempt for user {Username} from IP {IpAddress}", 
            username, ipAddress);
    }

    public static void LogSuspiciousActivity(ILogger logger, string activity, string userId, string ipAddress)
    {
        logger.LogWarning("Suspicious activity detected: {Activity} by user {UserId} from IP {IpAddress}", 
            activity, userId, ipAddress);
    }

    public static void LogSecurityViolation(ILogger logger, string violation, string details)
    {
        logger.LogError("Security violation: {Violation}. Details: {Details}", violation, details);
    }
}
```

## Next Steps

1. **Implement Security Headers** - Add the SecurityHeadersMiddleware to your pipeline
2. **Configure Rate Limiting** - Apply rate limiting policies to your endpoints
3. **Enhance Input Validation** - Use custom validation attributes on your models
4. **Set up Security Monitoring** - Implement logging for security events
5. **Regular Security Audits** - Schedule regular dependency and vulnerability scans
6. **Penetration Testing** - Conduct regular security assessments
7. **Security Training** - Ensure team is trained on secure coding practices

This comprehensive security enhancement guide provides multiple layers of protection for your WebSpark.ArtSpark application.

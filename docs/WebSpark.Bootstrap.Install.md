# WebSpark.Bootswatch Complete Installation Guide

A comprehensive guide for implementing dynamic Bootstrap theme switching in ASP.NET Core applications using WebSpark.Bootswatch.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [Detailed Installation](#detailed-installation)
  - [1. Package Installation](#1-package-installation)
  - [2. Configuration Setup](#2-configuration-setup)
  - [3. Service Registration](#3-service-registration)
  - [4. View Integration](#4-view-integration)
  - [5. Layout Implementation](#5-layout-implementation)
- [Testing & Verification](#testing--verification)
- [Customization Options](#customization-options)
- [Troubleshooting](#troubleshooting)
- [Best Practices](#best-practices)
- [Resources](#resources)

---

## 🎯 Overview

WebSpark.Bootswatch enables dynamic Bootstrap theme switching in ASP.NET Core applications without page reloads. This guide provides complete implementation instructions with real-world examples.

### Features

- ✅ Dynamic theme switching
- ✅ Theme persistence across sessions
- ✅ 20+ Bootswatch themes included
- ✅ Responsive dropdown component
- ✅ Performance optimized with caching
- ✅ No page reload required

---

## 🔧 Prerequisites

Before starting, ensure you have:

| Requirement | Version | Notes |
|------------|---------|-------|
| ASP.NET Core | 6.0+ | Tested with .NET 6, 7, 8, 9 |
| Visual Studio | 2022+ | Or VS Code with C# extension |
| Bootstrap | 5.0+ | Included via Bootswatch themes |
| Basic Knowledge | - | ASP.NET Core MVC, Dependency Injection |

---

## 🚀 Quick Start

For experienced developers, here's the minimal setup:

```bash
# Install packages
dotnet add package WebSpark.Bootswatch --version 1.10.3
dotnet add package WebSpark.HttpClientUtility --version 1.0.10

# Add to Program.cs
builder.Services.AddBootswatchThemeSwitcher();
app.UseBootswatchAll();

# Add to _ViewImports.cshtml
@addTagHelper *, WebSpark.Bootswatch

# Add to _Layout.cshtml
<bootswatch-theme-switcher />
```

---

## 📦 Detailed Installation

### 1. Package Installation

#### 1.1 Using Visual Studio Package Manager

1. Right-click on your project in Solution Explorer
2. Select **Manage NuGet Packages**
3. Go to **Browse** tab
4. Search for and install:
   - `WebSpark.Bootswatch` (v1.10.3)
   - `WebSpark.HttpClientUtility` (v1.0.10)

#### 1.2 Using Package Manager Console

```powershell
Install-Package WebSpark.Bootswatch -Version 1.10.3
Install-Package WebSpark.HttpClientUtility -Version 1.0.10
```

#### 1.3 Using .NET CLI

```bash
dotnet add package WebSpark.Bootswatch --version 1.10.3
dotnet add package WebSpark.HttpClientUtility --version 1.0.10
```

#### 1.4 Manual Package Reference

Add to your `.csproj` file:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="WebSpark.Bootswatch" Version="1.10.3" />
    <PackageReference Include="WebSpark.HttpClientUtility" Version="1.0.10" />
  </ItemGroup>
</Project>
```

---

### 2. Configuration Setup

#### 2.1 Create Required Directory

First, create the output directory for WebSpark operations:

```powershell
# Windows PowerShell
New-Item -ItemType Directory -Force -Path "c:\temp\WebSpark\CsvOutput"

# Command Prompt
mkdir "c:\temp\WebSpark\CsvOutput"

# Bash/WSL
mkdir -p "/c/temp/WebSpark/CsvOutput"
```

#### 2.2 Update appsettings.json

Add the WebSpark configuration section:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "WebSpark": {
    "CsvOutputFolder": "c:\\temp\\WebSpark\\CsvOutput",
    "HttpRequestResultPollyOptions": {
      "RetryCount": 3,
      "RetryDelayInSeconds": 1,
      "TimeoutInSeconds": 30,
      "CircuitBreakerHandledEventsAllowedBeforeBreaking": 5,
      "CircuitBreakerDurationOfBreakInSeconds": 60
    }
  }
}
```

#### 2.3 Update appsettings.Development.json

Ensure development environment has the same configuration:

```json
{
  "DetailedErrors": true,
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "WebSpark": "Debug"
    }
  },
  "WebSpark": {
    "CsvOutputFolder": "c:\\temp\\WebSpark\\CsvOutput",
    "HttpRequestResultPollyOptions": {
      "RetryCount": 2,
      "RetryDelayInSeconds": 1,
      "TimeoutInSeconds": 15,
      "CircuitBreakerHandledEventsAllowedBeforeBreaking": 3,
      "CircuitBreakerDurationOfBreakInSeconds": 30
    }
  }
}
```

---

### 3. Service Registration

#### 3.1 Update Program.cs - Using Statements

Add the required using statements at the top of `Program.cs`:

```csharp
using WebSpark.ArtSpark.Client.Clients;
using WebSpark.ArtSpark.Client.Interfaces;
using WebSpark.Bootswatch;
using WebSpark.HttpClientUtility.ClientService;
using WebSpark.HttpClientUtility.RequestResult;
using WebSpark.HttpClientUtility.StringConverter;
```

#### 3.2 Service Registration - Complete Example

Here's the complete service registration in the correct order:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add HttpContextAccessor for Tag Helper support
builder.Services.AddHttpContextAccessor();

// Add HttpClient factory (required for WebSpark.HttpClientUtility)
builder.Services.AddHttpClient();

// Register WebSpark.HttpClientUtility services
builder.Services.AddSingleton<IStringConverter, SystemJsonStringConverter>();
builder.Services.AddScoped<IHttpClientService, HttpClientService>();
builder.Services.AddScoped<HttpRequestResultService>();

// Register IHttpRequestResultService with telemetry decorator
builder.Services.AddScoped<IHttpRequestResultService>(provider =>
{
    IHttpRequestResultService service = provider.GetRequiredService<HttpRequestResultService>();

    // Add Telemetry (basic decorator for logging and monitoring)
    service = new HttpRequestResultServiceTelemetry(
        provider.GetRequiredService<ILogger<HttpRequestResultServiceTelemetry>>(),
        service
    );

    return service;
});

// Add WebSpark.Bootswatch theme switcher
builder.Services.AddBootswatchThemeSwitcher();

// Configure HttpClient for your application (example)
builder.Services.AddHttpClient<IArtInstituteClient, ArtInstituteClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Configure WebSpark.Bootswatch (includes static files and style cache)
// IMPORTANT: This must be called after UseRouting() and before UseAuthorization()
app.UseBootswatchAll();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
```

#### 3.3 Service Registration Order (Critical!)

The order of service registration is important:

1. ✅ `AddHttpClient()` - Must be first
2. ✅ `AddSingleton<IStringConverter, SystemJsonStringConverter>()`
3. ✅ `AddScoped<IHttpClientService, HttpClientService>()`
4. ✅ `AddScoped<HttpRequestResultService>()`
5. ✅ `AddScoped<IHttpRequestResultService>()` with decorator
6. ✅ `AddBootswatchThemeSwitcher()` - Must be last

---

### 4. View Integration

#### 4.1 Update Views/_ViewImports.cshtml

Add the WebSpark.Bootswatch tag helpers:

```razor
@using WebSpark.ArtSpark.Demo
@using WebSpark.ArtSpark.Demo.Models
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@addTagHelper *, WebSpark.Bootswatch
```

#### 4.2 Tag Helper Verification

Verify tag helpers are working by checking IntelliSense shows:

- `<bootswatch-theme-switcher>`
- Proper attribute completion

---

### 5. Layout Implementation

#### 5.1 Update Views/Shared/_Layout.cshtml

Here's the complete layout file with WebSpark.Bootswatch integration:

```razor
@using WebSpark.Bootswatch.StyleCache
@inject IStyleCache StyleCache
@inject BootswatchThemeHelper BootswatchThemeHelper
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - WebSpark.ArtSpark Demo</title>
    
    <!-- Dynamic Bootswatch Theme CSS -->
    <link rel="stylesheet" href="@await BootswatchThemeHelper.GetThemeBootstrapUrlAsync()" />
    
    <!-- Site-specific CSS -->
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
</head>
<body>
    <header>
        <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
            <div class="container-fluid">
                <a class="navbar-brand" asp-area="" asp-controller="Home" asp-action="Index">WebSpark.ArtSpark Demo</a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                        aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                    <ul class="navbar-nav flex-grow-1">
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Index">Home</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Privacy">Privacy</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-controller="Artwork" asp-action="Index">Artwork</a>
                        </li>
                    </ul>
                    
                    <!-- WebSpark.Bootswatch Theme Switcher -->
                    <div class="navbar-nav">
                        <bootswatch-theme-switcher 
                            dropdown-toggle-text="Theme"
                            dropdown-toggle-class="nav-link dropdown-toggle"
                            dropdown-menu-class="dropdown-menu dropdown-menu-end"
                            dropdown-item-class="dropdown-item" />
                    </div>
                </div>
            </div>
        </nav>
    </header>
    
    <div class="container">
        <main role="main" class="pb-3">
            @RenderBody()
        </main>
    </div>

    <footer class="border-top footer text-muted">
        <div class="container">
            &copy; @DateTime.Now.Year - WebSpark.ArtSpark Demo - <a asp-area="" asp-controller="Home" asp-action="Privacy">Privacy</a>
        </div>
    </footer>

    <!-- Core JavaScript Libraries -->
    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    
    <!-- WebSpark.Bootswatch JavaScript (Required for theme switching) -->
    <script src="~/lib/webspark-bootswatch/webspark-bootswatch.js"></script>
    
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

#### 5.2 Key Layout Components Explained

| Component | Purpose | Required |
|-----------|---------|----------|
| `@inject IStyleCache StyleCache` | Caches theme CSS for performance | ✅ Yes |
| `@inject BootswatchThemeHelper BootswatchThemeHelper` | Provides theme URL generation | ✅ Yes |
| `@await BootswatchThemeHelper.GetThemeBootstrapUrlAsync()` | Dynamic theme CSS URL | ✅ Yes |
| `<bootswatch-theme-switcher>` | Theme dropdown component | ✅ Yes |
| `webspark-bootswatch.js` | Client-side theme switching logic | ✅ Yes |

#### 5.3 Theme Switcher Customization Options

```razor
<!-- Basic theme switcher -->
<bootswatch-theme-switcher />

<!-- Customized theme switcher -->
<bootswatch-theme-switcher 
    dropdown-toggle-text="Choose Theme"
    dropdown-toggle-class="btn btn-outline-secondary dropdown-toggle"
    dropdown-menu-class="dropdown-menu dropdown-menu-dark"
    dropdown-item-class="dropdown-item"
    show-theme-name="true"
    include-default-theme="true" />

<!-- Minimal theme switcher for mobile -->
<bootswatch-theme-switcher 
    dropdown-toggle-text="🎨"
    dropdown-toggle-class="btn btn-sm btn-link dropdown-toggle"
    dropdown-menu-class="dropdown-menu dropdown-menu-end"
    dropdown-item-class="dropdown-item small" />
```

---

## 🧪 Testing & Verification

### Test Steps

#### 1. Build and Run

```bash
# Build the project
dotnet build

# Run the application
dotnet run
```

#### 2. Visual Verification Checklist

- [ ] Application starts without errors
- [ ] "Theme" dropdown appears in navigation
- [ ] Dropdown contains multiple theme options
- [ ] Selecting a theme changes the page appearance immediately
- [ ] Theme selection persists across page refreshes
- [ ] No JavaScript errors in browser console
- [ ] All page elements respond to theme changes

#### 3. Browser Developer Tools Testing

1. **Network Tab**: Verify Bootstrap CSS files load from CDN
2. **Console Tab**: Check for JavaScript errors
3. **Application Tab**: Verify theme preference is stored in localStorage
4. **Elements Tab**: Confirm CSS URLs change when themes switch

#### 4. Theme Testing Matrix

Test these popular themes to ensure proper functionality:

| Theme | Background | Navigation | Buttons | Cards |
|-------|------------|------------|---------|-------|
| Default | White | Light | Blue | White |
| Cerulean | White | Blue | Blue | White |
| Cosmo | White | Dark Blue | Orange | White |
| Cyborg | Dark | Black | Red | Dark |
| Darkly | Dark | Dark | Primary | Dark |
| Flatly | White | Green | Green | White |
| Journal | White | Light | Red | White |
| Litera | White | Light | Blue | White |
| Lumen | White | Light | Orange | White |
| Lux | White | Dark | Gold | White |
| Materia | White | Colorful | Pink | White |
| Minty | White | Green | Green | White |
| Morph | Light | Gradient | Blue | Light |
| Pulse | White | Purple | Purple | White |
| Quartz | Light | Orange | Orange | Light |
| Sandstone | White | Green | Green | White |
| Simplex | White | Red | Red | White |
| Sketchy | White | Hand-drawn | Blue | White |
| Slate | Dark | Dark | Primary | Dark |
| Solar | Dark | Dark Blue | Blue | Dark |
| Spacelab | White | Blue | Blue | White |
| Superhero | Dark | Dark | Orange | Dark |
| United | White | Orange | Orange | White |
| Vapor | Dark | Pink/Purple | Pink | Dark |
| Yeti | White | Light | Blue | White |
| Zephyr | White | Light | Blue | White |

---

## 🎨 Customization Options

### Advanced Service Configuration

```csharp
// Custom theme selection
builder.Services.AddBootswatchThemeSwitcher(options =>
{
    // Only include specific themes
    options.IncludeThemes = new[] 
    { 
        "default", "cerulean", "cosmo", "flatly", 
        "journal", "lumen", "pulse", "simplex" 
    };
    
    // Set default theme
    options.DefaultTheme = "lumen";
    
    // Custom CDN base URL
    options.CdnBaseUrl = "https://custom-cdn.example.com/bootswatch/";
    
    // Cache configuration
    options.CacheExpirationMinutes = 60;
});

// Custom HTTP client configuration
builder.Services.AddHttpClient("BootswatchClient", client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
    client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
});
```

### Custom CSS Integration

Create a `custom-themes.css` file that adapts to theme changes:

```css
/* Custom CSS that works with all Bootswatch themes */
:root {
    --custom-shadow: 0 2px 4px rgba(0,0,0,0.1);
    --custom-border-radius: 0.375rem;
}

.custom-card {
    background-color: var(--bs-body-bg);
    border: 1px solid var(--bs-border-color);
    border-radius: var(--custom-border-radius);
    box-shadow: var(--custom-shadow);
    color: var(--bs-body-color);
}

.custom-button {
    background-color: var(--bs-primary);
    border-color: var(--bs-primary);
    color: var(--bs-primary-text, white);
}

.custom-button:hover {
    background-color: var(--bs-primary-bg-subtle);
    border-color: var(--bs-primary-border-subtle);
}

/* Dark theme adaptations */
[data-bs-theme="dark"] .custom-card {
    --custom-shadow: 0 2px 4px rgba(0,0,0,0.3);
}
```

### JavaScript Customization

```javascript
// Custom theme change event handler
document.addEventListener('themeChanged', function(event) {
    console.log('Theme changed to:', event.detail.theme);
    
    // Custom logic when theme changes
    updateCustomComponents(event.detail.theme);
    
    // Analytics tracking
    gtag('event', 'theme_change', {
        'theme_name': event.detail.theme
    });
});

function updateCustomComponents(theme) {
    // Update charts, graphs, or other components
    if (typeof Chart !== 'undefined') {
        Chart.defaults.color = getComputedStyle(document.documentElement)
            .getPropertyValue('--bs-body-color');
    }
}

// Programmatically change theme
function setTheme(themeName) {
    if (window.WebSparkBootswatch) {
        window.WebSparkBootswatch.setTheme(themeName);
    }
}

// Get current theme
function getCurrentTheme() {
    return localStorage.getItem('bootswatch-theme') || 'default';
}
```

---

## 🐛 Troubleshooting

### Common Issues and Solutions

#### Issue 1: Theme Switcher Not Appearing

**Symptoms:**

- No dropdown in navigation
- Empty dropdown menu

**Possible Causes & Solutions:**

| Cause | Solution |
|-------|----------|
| Tag helpers not registered | Add `@addTagHelper *, WebSpark.Bootswatch` to `_ViewImports.cshtml` |
| Missing service registration | Verify `AddBootswatchThemeSwitcher()` in `Program.cs` |
| Incorrect tag helper syntax | Check `<bootswatch-theme-switcher>` spelling and attributes |

**Debug Steps:**

```razor
<!-- Add to your view to debug -->
@{
    var themeHelper = ViewContext.HttpContext.RequestServices
        .GetService<BootswatchThemeHelper>();
}
<p>Theme Helper Available: @(themeHelper != null)</p>
```

#### Issue 2: Dependency Injection Errors

**Error Messages:**

- `Unable to resolve service for type 'IHttpRequestResultService'`
- `Unable to resolve service for type 'IStringConverter'`

**Solution Checklist:**

- [ ] Verify all using statements are added
- [ ] Check service registration order
- [ ] Ensure `AddHttpClient()` is called first
- [ ] Confirm WebSpark.HttpClientUtility package is installed

**Debug Registration:**

```csharp
// Add logging to verify service registration
builder.Services.AddBootswatchThemeSwitcher();

// Verify services are registered
var serviceProvider = builder.Services.BuildServiceProvider();
var themeHelper = serviceProvider.GetService<BootswatchThemeHelper>();
Console.WriteLine($"Theme Helper Registered: {themeHelper != null}");
```

#### Issue 3: Themes Not Loading

**Symptoms:**

- Theme dropdown works but appearance doesn't change
- CSS files return 404 errors

**Solutions:**

| Problem | Fix |
|---------|-----|
| Missing `UseBootswatchAll()` | Add after `UseRouting()` in `Program.cs` |
| Incorrect CSS URL | Verify `@await BootswatchThemeHelper.GetThemeBootstrapUrlAsync()` |
| CDN connectivity issues | Check network connectivity and firewall settings |
| Cache issues | Clear browser cache and application cache |

**Debug CSS Loading:**

```razor
<!-- Debug theme URL generation -->
@{
    var currentThemeUrl = await BootswatchThemeHelper.GetThemeBootstrapUrlAsync();
}
<p>Current Theme URL: @currentThemeUrl</p>

<!-- Test direct CDN access -->
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootswatch@5.3.0/dist/flatly/bootstrap.min.css" />
```

#### Issue 4: JavaScript Errors

**Common Errors:**

- `webspark-bootswatch.js not found`
- `Uncaught TypeError: Cannot read property 'setTheme'`

**Solutions:**

1. Verify JavaScript file reference: `<script src="~/lib/webspark-bootswatch/webspark-bootswatch.js"></script>`
2. Check file exists in `wwwroot/lib/webspark-bootswatch/`
3. Ensure script loads after jQuery and Bootstrap

**Manual JavaScript Fix:**

```html
<!-- Fallback if automated script doesn't work -->
<script>
window.WebSparkBootswatch = {
    setTheme: function(theme) {
        localStorage.setItem('bootswatch-theme', theme);
        location.reload();
    }
};
</script>
```

#### Issue 5: Configuration Errors

**Error:** `System.ArgumentNullException: Value cannot be null. (Parameter 'CsvOutputFolder')`

**Solution:** Verify configuration in `appsettings.json`:

```json
{
  "WebSpark": {
    "CsvOutputFolder": "c:\\temp\\WebSpark\\CsvOutput"
  }
}
```

**Create Missing Directory:**

```powershell
New-Item -ItemType Directory -Force -Path "c:\temp\WebSpark\CsvOutput"
```

#### Issue 6: Performance Issues

**Symptoms:**

- Slow theme switching
- High memory usage

**Optimizations:**

```csharp
// Enable response compression
builder.Services.AddResponseCompression();

// Configure caching
builder.Services.AddBootswatchThemeSwitcher(options =>
{
    options.CacheExpirationMinutes = 120; // Increase cache time
});

// Add memory cache
builder.Services.AddMemoryCache();
```

### Debug Logging Configuration

Add detailed logging to troubleshoot issues:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "WebSpark": "Debug",
      "WebSpark.Bootswatch": "Trace",
      "WebSpark.HttpClientUtility": "Debug"
    }
  }
}
```

### Health Check Implementation

```csharp
// Add health checks for WebSpark services
builder.Services.AddHealthChecks()
    .AddCheck<BootswatchHealthCheck>("bootswatch");

// Custom health check
public class BootswatchHealthCheck : IHealthCheck
{
    private readonly BootswatchThemeHelper _themeHelper;
    
    public BootswatchHealthCheck(BootswatchThemeHelper themeHelper)
    {
        _themeHelper = themeHelper;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = await _themeHelper.GetThemeBootstrapUrlAsync();
            return string.IsNullOrEmpty(url) 
                ? HealthCheckResult.Unhealthy("Theme URL generation failed")
                : HealthCheckResult.Healthy($"Theme URL: {url}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Bootswatch service error", ex);
        }
    }
}
```

---

## 📋 Best Practices

### Performance Optimization

#### 1. CDN and Caching Strategy

```csharp
// Optimize caching configuration
builder.Services.AddBootswatchThemeSwitcher(options =>
{
    // Cache themes for 2 hours in production
    options.CacheExpirationMinutes = app.Environment.IsDevelopment() ? 5 : 120;
    
    // Use faster CDN
    options.CdnBaseUrl = "https://fastly.jsdelivr.net/npm/bootswatch@5.3.0/dist/";
});

// Add response caching
builder.Services.AddResponseCaching();
app.UseResponseCaching();
```

#### 2. Bundle and Minification

```html
<!-- Production optimization -->
<environment include="Development">
    <link rel="stylesheet" href="@await BootswatchThemeHelper.GetThemeBootstrapUrlAsync()" />
    <link rel="stylesheet" href="~/css/site.css" />
</environment>
<environment exclude="Development">
    <link rel="stylesheet" href="@await BootswatchThemeHelper.GetThemeBootstrapUrlAsync()" />
    <link rel="stylesheet" href="~/css/site.min.css" asp-append-version="true" />
</environment>
```

### Security Considerations

#### 1. Content Security Policy (CSP)

```csharp
// Add CSP header for external CDN access
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy",
        "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://fastly.jsdelivr.net;");
    await next();
});
```

#### 2. HTTPS Enforcement

```csharp
// Ensure all theme URLs use HTTPS
builder.Services.AddBootswatchThemeSwitcher(options =>
{
    options.ForceHttps = true;
});
```

### Accessibility Improvements

#### 1. ARIA Labels and Screen Reader Support

```razor
<bootswatch-theme-switcher 
    dropdown-toggle-text="Theme"
    dropdown-toggle-class="nav-link dropdown-toggle"
    dropdown-menu-class="dropdown-menu dropdown-menu-end"
    dropdown-item-class="dropdown-item"
    aria-label="Change color theme"
    role="button" />
```

#### 2. Keyboard Navigation

```css
/* Ensure theme switcher is keyboard accessible */
.dropdown-toggle:focus {
    outline: 2px solid var(--bs-primary);
    outline-offset: 2px;
}

.dropdown-item:focus {
    background-color: var(--bs-primary);
    color: var(--bs-primary-text, white);
}
```

### Mobile Optimization

```css
/* Responsive theme switcher */
@media (max-width: 768px) {
    .bootswatch-theme-switcher .dropdown-menu {
        position: fixed !important;
        top: auto !important;
        left: 10px !important;
        right: 10px !important;
        width: auto !important;
        max-height: 50vh;
        overflow-y: auto;
    }
}
```

### Testing Strategy

#### Unit Tests

```csharp
[Test]
public async Task BootswatchThemeHelper_GetThemeUrl_ReturnsValidUrl()
{
    // Arrange
    var services = new ServiceCollection();
    services.AddBootswatchThemeSwitcher();
    var provider = services.BuildServiceProvider();
    var helper = provider.GetService<BootswatchThemeHelper>();
    
    // Act
    var url = await helper.GetThemeBootstrapUrlAsync("flatly");
    
    // Assert
    Assert.That(url, Is.Not.Null);
    Assert.That(url, Does.Contain("flatly"));
    Assert.That(url, Does.StartWith("https://"));
}
```

#### Integration Tests

```csharp
[Test]
public async Task ThemeSwitcher_RendersCorrectly()
{
    // Arrange
    var factory = new WebApplicationFactory<Program>();
    var client = factory.CreateClient();
    
    // Act
    var response = await client.GetAsync("/");
    var content = await response.Content.ReadAsStringAsync();
    
    // Assert
    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    Assert.That(content, Does.Contain("bootswatch-theme-switcher"));
    Assert.That(content, Does.Contain("webspark-bootswatch.js"));
}
```

---

## 📚 Resources

### Official Documentation

- [WebSpark.Bootswatch NuGet Package](https://www.nuget.org/packages/WebSpark.Bootswatch/)
- [WebSpark.HttpClientUtility Documentation](https://www.nuget.org/packages/WebSpark.HttpClientUtility/)
- [Bootswatch Official Website](https://bootswatch.com/)
- [Bootstrap 5 Documentation](https://getbootstrap.com/docs/5.3/)

### CDN Resources

- [jsDelivr CDN](https://www.jsdelivr.com/package/npm/bootswatch)
- [cdnjs Bootswatch](https://cdnjs.com/libraries/bootswatch)
- [unpkg Bootswatch](https://unpkg.com/bootswatch/)

### Theme Previews

- [Bootswatch Theme Gallery](https://bootswatch.com/)
- [Bootstrap Themes Comparison](https://bootswatch.com/help/)

### ASP.NET Core Resources

- [ASP.NET Core Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- [ASP.NET Core Tag Helpers](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/)
- [ASP.NET Core Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)

### Community Resources

- [Stack Overflow - WebSpark.Bootswatch](https://stackoverflow.com/questions/tagged/webspark-bootswatch)
- [GitHub Issues](https://github.com/markjulmar/WebSpark.Bootswatch/issues)

---

## 📝 Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0 | 2025-05-30 | Initial comprehensive installation guide |
| | | Complete implementation with WebSpark.ArtSpark.Demo |
| | | Detailed troubleshooting and best practices |
| | | Performance optimization guidelines |

---

## 📧 Support

If you encounter issues not covered in this guide:

1. **Check the troubleshooting section** above
2. **Review the GitHub repository** for known issues
3. **Search Stack Overflow** with the `webspark-bootswatch` tag
4. **Create a minimal reproduction** of your issue
5. **File an issue** with detailed information

---

**⚡ Quick Reference Commands**

```bash
# Install packages
dotnet add package WebSpark.Bootswatch --version 1.10.3
dotnet add package WebSpark.HttpClientUtility --version 1.0.10

# Create output directory
mkdir "c:\temp\WebSpark\CsvOutput"

# Build and run
dotnet build && dotnet run
```

**🎯 Essential Files to Modify**

- ✅ `Program.cs` - Service registration
- ✅ `appsettings.json` - Configuration
- ✅ `Views/_ViewImports.cshtml` - Tag helpers
- ✅ `Views/Shared/_Layout.cshtml` - Theme integration

---

*This guide was created based on a successful implementation with WebSpark.Bootswatch v1.10.3 and ASP.NET Core. For the most up-to-date information, always refer to the official NuGet package documentation.*

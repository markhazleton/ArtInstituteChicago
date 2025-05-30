# WebSpark.Bootswatch Installation Guide

This guide provides step-by-step instructions for implementing WebSpark.Bootswatch theme switching in an ASP.NET Core project.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Package Installation](#package-installation)
3. [Configuration Setup](#configuration-setup)
4. [Service Registration](#service-registration)
5. [View Integration](#view-integration)
6. [Testing](#testing)
7. [Troubleshooting](#troubleshooting)

## Prerequisites

- ASP.NET Core 6.0 or later
- Visual Studio 2022 or VS Code
- Basic understanding of ASP.NET Core MVC and dependency injection

## Package Installation

### Step 1: Install Required NuGet Packages

Add the following packages to your project:

```xml
<PackageReference Include="WebSpark.Bootswatch" Version="1.10.3" />
<PackageReference Include="WebSpark.HttpClientUtility" Version="1.0.10" />
```

#### Using Package Manager Console

```powershell
Install-Package WebSpark.Bootswatch -Version 1.10.3
Install-Package WebSpark.HttpClientUtility -Version 1.0.10
```

#### Using .NET CLI

```bash
dotnet add package WebSpark.Bootswatch --version 1.10.3
dotnet add package WebSpark.HttpClientUtility --version 1.0.10
```

## Configuration Setup

### Step 2: Update Configuration Files

#### 2.1 Update `appsettings.json`

Add the following configuration section:

```json
{
  "WebSpark": {
    "CsvOutputFolder": "c:\\temp\\WebSpark\\CsvOutput",
    "HttpRequestResultPollyOptions": {
      "RetryCount": 3,
      "RetryDelayInSeconds": 1,
      "TimeoutInSeconds": 30
    }
  }
}
```

#### 2.2 Update `appsettings.Development.json`

Add the same configuration for development environment:

```json
{
  "WebSpark": {
    "CsvOutputFolder": "c:\\temp\\WebSpark\\CsvOutput",
    "HttpRequestResultPollyOptions": {
      "RetryCount": 3,
      "RetryDelayInSeconds": 1,
      "TimeoutInSeconds": 30
    }
  }
}
```

#### 2.3 Create Output Directory

Ensure the CSV output directory exists:

```bash
mkdir "c:\temp\WebSpark\CsvOutput"
```

## Service Registration

### Step 3: Update Program.cs

Add the following using statements at the top of `Program.cs`:

```csharp
using WebSpark.Bootswatch;
using WebSpark.HttpClientUtility.ClientService;
using WebSpark.HttpClientUtility.RequestResult;
using WebSpark.HttpClientUtility.StringConverter;
```

Add the following service registrations in the correct order:

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

// ... your other service registrations

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
app.UseBootswatchAll();

app.UseAuthorization();

// ... rest of your pipeline configuration
```

## View Integration

### Step 4: Register Tag Helpers

Update `Views/_ViewImports.cshtml` to include the WebSpark.Bootswatch tag helpers:

```razor
@using YourProject
@using YourProject.Models
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@addTagHelper *, WebSpark.Bootswatch
```

### Step 5: Update Layout File

Update `Views/Shared/_Layout.cshtml` to integrate Bootswatch theme switching:

#### 5.1 Add Dependency Injection and Using Statements

Add at the top of the file:

```razor
@using WebSpark.Bootswatch.StyleCache
@inject IStyleCache StyleCache
@inject BootswatchThemeHelper BootswatchThemeHelper
```

#### 5.2 Update Bootstrap CSS Link

Replace your existing Bootstrap CSS link with dynamic theme URL generation:

```razor
<link rel="stylesheet" href="@await BootswatchThemeHelper.GetThemeBootstrapUrlAsync()" />
```

#### 5.3 Add Theme Switcher Component

Add the theme switcher to your navigation bar:

```razor
<div class="navbar-nav">
    <bootswatch-theme-switcher 
        dropdown-toggle-text="Theme"
        dropdown-toggle-class="nav-link dropdown-toggle"
        dropdown-menu-class="dropdown-menu dropdown-menu-end"
        dropdown-item-class="dropdown-item" />
</div>
```

#### 5.4 Add Required JavaScript

Add before the closing `</body>` tag:

```razor
<script src="~/lib/webspark-bootswatch/webspark-bootswatch.js"></script>
```

### Step 6: Complete Layout Example

Here's a complete example of the key parts of `_Layout.cshtml`:

```razor
@using WebSpark.Bootswatch.StyleCache
@inject IStyleCache StyleCache
@inject BootswatchThemeHelper BootswatchThemeHelper

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Your App</title>
    
    <!-- Dynamic Bootswatch Theme CSS -->
    <link rel="stylesheet" href="@await BootswatchThemeHelper.GetThemeBootstrapUrlAsync()" />
    
    <!-- Your other CSS files -->
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
</head>
<body>
    <header>
        <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
            <div class="container-fluid">
                <!-- Your navigation items -->
                
                <!-- Theme Switcher -->
                <div class="navbar-nav">
                    <bootswatch-theme-switcher 
                        dropdown-toggle-text="Theme"
                        dropdown-toggle-class="nav-link dropdown-toggle"
                        dropdown-menu-class="dropdown-menu dropdown-menu-end"
                        dropdown-item-class="dropdown-item" />
                </div>
            </div>
        </nav>
    </header>
    
    <div class="container">
        <main role="main" class="pb-3">
            @RenderBody()
        </main>
    </div>

    <!-- Scripts -->
    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    
    <!-- WebSpark.Bootswatch JavaScript -->
    <script src="~/lib/webspark-bootswatch/webspark-bootswatch.js"></script>
    
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

## Testing

### Step 7: Test the Implementation

1. **Build the Project**:

   ```bash
   dotnet build
   ```

2. **Run the Application**:

   ```bash
   dotnet run
   ```

3. **Verify Theme Switching**:
   - Navigate to your application in a browser
   - Look for the "Theme" dropdown in the navigation
   - Select different Bootswatch themes
   - Verify that the page appearance changes
   - Check that the theme selection persists across page refreshes

4. **Check Browser Developer Tools**:
   - Verify that the Bootstrap CSS URLs change when themes are selected
   - Ensure no JavaScript errors appear in the console

## Troubleshooting

### Common Issues and Solutions

#### Issue 1: Theme Switcher Not Appearing

- **Cause**: Tag helpers not registered or incorrect markup
- **Solution**: Verify `@addTagHelper *, WebSpark.Bootswatch` is in `_ViewImports.cshtml`

#### Issue 2: Dependency Injection Errors

- **Cause**: Missing service registrations
- **Solution**: Ensure all services are registered in the correct order in `Program.cs`

#### Issue 3: Themes Not Loading

- **Cause**: Missing `UseBootswatchAll()` middleware
- **Solution**: Add `app.UseBootswatchAll();` after `app.UseRouting();` in `Program.cs`

#### Issue 4: JavaScript Errors

- **Cause**: Missing JavaScript file reference
- **Solution**: Ensure `webspark-bootswatch.js` is referenced in the layout

#### Issue 5: Configuration Errors

- **Cause**: Missing or incorrect configuration settings
- **Solution**: Verify `appsettings.json` contains the WebSpark configuration section

### Debug Tips

1. **Enable Detailed Logging**: Add logging configuration to see WebSpark operations
2. **Check Network Tab**: Verify Bootstrap CSS files are loading correctly
3. **Inspect HTML**: Ensure the theme switcher markup is generated correctly
4. **Verify File Permissions**: Ensure the CSV output directory is writable

## Advanced Configuration

### Custom Theme Selection

You can customize which themes are available by configuring the theme switcher options in your service registration:

```csharp
builder.Services.AddBootswatchThemeSwitcher(options =>
{
    options.IncludeThemes = new[] { "default", "cerulean", "cosmo", "flatly", "journal", "lumen" };
});
```

### Custom CSS Integration

To add custom CSS that works with theme switching:

```razor
<!-- In your layout head section -->
<link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
<style>
    /* Custom CSS that adapts to Bootstrap themes */
    .custom-component {
        background-color: var(--bs-primary);
        color: var(--bs-primary-text);
    }
</style>
```

## Resources

- [WebSpark.Bootswatch NuGet Package](https://www.nuget.org/packages/WebSpark.Bootswatch/)
- [Bootswatch Official Website](https://bootswatch.com/)
- [Bootstrap Documentation](https://getbootstrap.com/docs/)

## Version History

- **v1.0** (Current): Initial implementation guide for WebSpark.Bootswatch v1.10.3

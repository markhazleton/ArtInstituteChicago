# 🎨 WebSpark.ArtSpark

**Where Open Art Meets .NET Excellence**

WebSpark.ArtSpark is a modern, open-source ASP.NET Core application that beautifully merges the cultural richness of the [Art Institute of Chicago](https://api.artic.edu/docs/) with the architectural clarity of the WebSpark NuGet packages. It demonstrates how public data, art, and .NET development can intersect to create immersive, responsive, and resilient applications.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/WebSpark.Bootswatch.svg)](https://www.nuget.org/packages/WebSpark.Bootswatch/)
[![NuGet](https://img.shields.io/nuget/v/WebSpark.HttpClientUtility.svg)](https://www.nuget.org/packages/WebSpark.HttpClientUtility/)
[![.NET Version](https://img.shields.io/badge/.NET-9-blue)](https://dotnet.microsoft.com/)

---

## 🚀 Live Demo

> Coming Soon: [https://artspark.markhazleton.com](https://artspark.markhazleton.com)

---

## 📦 NuGet Packages Used

### [WebSpark.Bootswatch](https://www.nuget.org/packages/WebSpark.Bootswatch)

* Runtime Bootswatch theme switching
* Integrated with Razor Pages/MVC
* Cookie-based user theme persistence

### [WebSpark.HttpClientUtility](https://www.nuget.org/packages/WebSpark.HttpClientUtility)

* Simplified, resilient HttpClient with Polly
* Retry, timeout, circuit breaker, and fallback
* Integrated logging and telemetry support

---

## ✨ Features

* 🎨 Explore public domain artworks from AIC
* 🎭 Live theme switching with Bootswatch themes
* 🔁 Resilient API integrations via HttpClientFactory
* 🧠 Developer and educator modes
* 🖼️ Curated and themed collections
* 🔍 Deep metadata and IIIF support

---

## 🧰 Tech Stack

* ASP.NET Core 9 (Razor Pages)
* Bootstrap 5 with Bootswatch
* Polly for .NET resilience
* Art Institute of Chicago API
* Custom middleware and component system

---

## 📐 Architecture Example

```csharp
// Program.cs
builder.Services.AddRazorPages();
builder.Services.AddBootswatchStyles();
builder.Services.AddBootswatchThemeSwitcher();
builder.Services.AddHttpClientWithPolicies("ArtApiClient", client =>
{
    client.BaseAddress = new Uri("https://api.artic.edu/api/v1/");
});
```

```html
<!-- _Layout.cshtml -->
<script src="/_content/WebSpark.Bootswatch/js/bootswatch-theme-switcher.js"></script>
```

```json
{
  "id": 27992,
  "title": "Water Lilies",
  "artist_display": "Claude Monet",
  "image_id": "e9a1c4d3-f9f7-4a55-ae6c-8f742abe56a3"
}
```

---

## 🧪 Getting Started

### Prerequisites

* [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
* A modern code editor (Visual Studio 2022+, VS Code)

### Clone and Run

```bash
git clone https://github.com/markhazleton/WebSpark.ArtSpark.git
cd WebSpark.ArtSpark
dotnet run
```

Then visit `https://localhost:5001` in your browser.

---

## 🧭 Roadmap

* [ ] User-curated collections with login support
* [ ] Export to PDF/Markdown
* [ ] Plugins for additional art APIs (MoMA, MET)
* [ ] `dotnet new artspark` project template

---

## 📚 Related Resources

* [Art Institute of Chicago API](https://api.artic.edu/docs/)
* [Bootswatch](https://bootswatch.com/)
* [Polly GitHub](https://github.com/App-vNext/Polly)
* [WebSpark NuGet Packages](https://www.nuget.org/profiles/markhazleton)
* [Project Mechanics Blog](https://markhazleton.com)

---

## 🤝 Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a new branch (`git checkout -b feature/your-feature-name`)
3. Commit your changes (`git commit -m 'Add your message here'`)
4. Push to the branch (`git push origin feature/your-feature-name`)
5. Open a pull request

Issues and suggestions are also welcome via the [GitHub Issues page](https://github.com/markhazleton/WebSpark.ArtSpark/issues).

---

## 🧾 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## 💡 About

**WebSpark.ArtSpark** is developed and maintained by [Mark Hazleton](https://markhazleton.com), showcasing the power of open public datasets and modern .NET development.

> WebSpark.ArtSpark — bridging creative culture and clean code.

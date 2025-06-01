using Microsoft.AspNetCore.Mvc;
using Markdig;
using System.Text;

namespace WebSpark.ArtSpark.Demo.Controllers
{
    public class DocsController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<DocsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly MarkdownPipeline _markdownPipeline;

        public DocsController(IWebHostEnvironment hostingEnvironment, ILogger<DocsController> logger, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _configuration = configuration;

            // Configure Markdig pipeline with common extensions
            _markdownPipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseBootstrap()
                .UseSoftlineBreakAsHardlineBreak()
                .Build();
        }
        public IActionResult Index()
        {
            try
            {
                var docsPath = GetDocsPath();
                if (!Directory.Exists(docsPath))
                {
                    _logger.LogWarning("Docs directory not found at: {DocsPath}", docsPath);
                    return View("Error", "Documentation directory not found.");
                }

                var markdownFiles = Directory.GetFiles(docsPath, "*.md")
                    .Select(file => new DocumentInfo
                    {
                        FileName = Path.GetFileNameWithoutExtension(file),
                        FilePath = file,
                        Title = GetDocumentTitle(file) ?? Path.GetFileNameWithoutExtension(file),
                        LastModified = System.IO.File.GetLastWriteTime(file)
                    })
                    .OrderBy(doc => doc.Title)
                    .ToList();

                return View(markdownFiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading documentation index");
                return View("Error", "An error occurred while loading the documentation.");
            }
        }
        public async Task<IActionResult> ViewDoc(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var docsPath = GetDocsPath();
                var filePath = Path.Combine(docsPath, $"{id}.md");

                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning("Documentation file not found: {FilePath}", filePath);
                    return NotFound($"Documentation file '{id}' not found.");
                }

                var markdownContent = await System.IO.File.ReadAllTextAsync(filePath);
                var htmlContent = Markdown.ToHtml(markdownContent, _markdownPipeline);

                var documentInfo = new DocumentInfo
                {
                    FileName = id,
                    FilePath = filePath,
                    Title = GetDocumentTitle(filePath) ?? id,
                    Content = htmlContent,
                    LastModified = System.IO.File.GetLastWriteTime(filePath)
                };

                return View("View", documentInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading documentation file: {Id}", id);
                return View("Error", "An error occurred while loading the documentation.");
            }
        }
        private string GetDocsPath()
        {
            // Get the documentation path from configuration
            var documentationPath = _configuration["WebSpark:DocumentationPath"] ?? "docs";

            // Check if it's an absolute path first
            if (Path.IsPathRooted(documentationPath))
            {
                return documentationPath;
            }

            // If it's a relative path, check relative to content root first
            var contentRootPath = Path.Combine(_hostingEnvironment.ContentRootPath, documentationPath);
            if (Directory.Exists(contentRootPath))
            {
                return contentRootPath;
            }

            // If not found relative to content root, try relative to solution root
            var solutionRoot = Directory.GetParent(_hostingEnvironment.ContentRootPath)?.FullName;
            if (solutionRoot != null)
            {
                var solutionDocsPath = Path.Combine(solutionRoot, documentationPath);
                if (Directory.Exists(solutionDocsPath))
                {
                    return solutionDocsPath;
                }
            }

            // Fallback to content root path (even if directory doesn't exist yet)
            return contentRootPath;
        }

        private string? GetDocumentTitle(string filePath)
        {
            try
            {
                // Read the first few lines to find a title (look for # heading)
                var lines = System.IO.File.ReadLines(filePath).Take(10);
                foreach (var line in lines)
                {
                    if (line.TrimStart().StartsWith("# "))
                    {
                        return line.TrimStart().Substring(2).Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error reading title from file: {FilePath}", filePath);
            }

            return null;
        }
    }

    public class DocumentInfo
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
    }
}

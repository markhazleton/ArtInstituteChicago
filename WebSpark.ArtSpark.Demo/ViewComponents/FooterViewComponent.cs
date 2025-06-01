using Microsoft.AspNetCore.Mvc;
using WebSpark.ArtSpark.Demo.Services;

namespace WebSpark.ArtSpark.Demo.ViewComponents;

public class FooterViewComponent : ViewComponent
{
    private readonly IBuildInfoService _buildInfoService;

    public FooterViewComponent(IBuildInfoService buildInfoService)
    {
        _buildInfoService = buildInfoService;
    }

    public IViewComponentResult Invoke()
    {
        var model = new FooterViewModel
        {
            Version = _buildInfoService.GetVersion(),
            BuildDate = _buildInfoService.GetBuildDate(),
            FormattedBuildInfo = _buildInfoService.GetFormattedBuildInfo()
        };

        return View(model);
    }
}

public class FooterViewModel
{
    public string Version { get; set; } = string.Empty;
    public DateTime BuildDate { get; set; }
    public string FormattedBuildInfo { get; set; } = string.Empty;
}

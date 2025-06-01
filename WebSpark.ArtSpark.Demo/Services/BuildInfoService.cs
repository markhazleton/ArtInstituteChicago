using System.Reflection;

namespace WebSpark.ArtSpark.Demo.Services;

public interface IBuildInfoService
{
    string GetVersion();
    DateTime GetBuildDate();
    string GetFormattedBuildInfo();
}

public class BuildInfoService : IBuildInfoService
{
    private readonly string _version;
    private readonly DateTime _buildDate; public BuildInfoService()
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Get version from assembly
        _version = assembly.GetName().Version?.ToString() ?? "1.0.0";

        // Get build date from assembly last write time (more accurate for builds)
        var assemblyLocation = assembly.Location;
        if (!string.IsNullOrEmpty(assemblyLocation) && File.Exists(assemblyLocation))
        {
            _buildDate = File.GetLastWriteTime(assemblyLocation);
        }
        else
        {
            // Fallback: Try to get from entry assembly
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly?.Location != null && File.Exists(entryAssembly.Location))
            {
                _buildDate = File.GetLastWriteTime(entryAssembly.Location);
            }
            else
            {
                // Final fallback to current time
                _buildDate = DateTime.Now;
            }
        }
    }

    public string GetVersion()
    {
        return _version;
    }

    public DateTime GetBuildDate()
    {
        return _buildDate;
    }

    public string GetFormattedBuildInfo()
    {
        return $"v{_version} - Built {_buildDate:MM/dd/yyyy}";
    }
}

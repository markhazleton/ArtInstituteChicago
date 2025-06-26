using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;
using System.Runtime;

namespace WebSpark.ArtSpark.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitoringController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly ILogger<MonitoringController> _logger;

        public MonitoringController(HealthCheckService healthCheckService, ILogger<MonitoringController> logger)
        {
            _healthCheckService = healthCheckService;
            _logger = logger;
        }

        [HttpGet("health")]
        public async Task<IActionResult> GetHealth()
        {
            var report = await _healthCheckService.CheckHealthAsync();

            var response = new
            {
                status = report.Status.ToString(),
                totalDuration = report.TotalDuration.TotalMilliseconds,
                checks = report.Entries.Select(x => new
                {
                    name = x.Key,
                    status = x.Value.Status.ToString(),
                    description = x.Value.Description,
                    duration = x.Value.Duration.TotalMilliseconds,
                    tags = x.Value.Tags
                })
            };

            return Ok(response);
        }

        [HttpGet("metrics")]
        public IActionResult GetMetrics()
        {
            var process = Process.GetCurrentProcess();
            var gcInfo = GC.GetGCMemoryInfo();

            var metrics = new
            {
                timestamp = DateTime.UtcNow,
                memory = new
                {
                    totalMemoryMB = GC.GetTotalMemory(false) / (1024 * 1024),
                    gen0Collections = GC.CollectionCount(0),
                    gen1Collections = GC.CollectionCount(1),
                    gen2Collections = GC.CollectionCount(2),
                    heapSizeMB = gcInfo.HeapSizeBytes / (1024 * 1024),
                    fragmentedBytes = gcInfo.FragmentedBytes
                },
                process = new
                {
                    id = process.Id,
                    startTime = process.StartTime,
                    totalProcessorTime = process.TotalProcessorTime.TotalMilliseconds,
                    workingSetMB = process.WorkingSet64 / (1024 * 1024),
                    peakWorkingSetMB = process.PeakWorkingSet64 / (1024 * 1024),
                    threadCount = process.Threads.Count
                },
                runtime = new
                {
                    version = Environment.Version.ToString(),
                    osVersion = Environment.OSVersion.ToString(),
                    machineName = Environment.MachineName,
                    processorCount = Environment.ProcessorCount,
                    is64BitProcess = Environment.Is64BitProcess,
                    gcSettings = new
                    {
                        isServerGC = GCSettings.IsServerGC,
                        latencyMode = GCSettings.LatencyMode.ToString()
                    }
                }
            };

            return Ok(metrics);
        }

        [HttpGet("info")]
        public IActionResult GetInfo()
        {
            var info = new
            {
                application = new
                {
                    name = "WebSpark.ArtSpark",
                    version = typeof(Program).Assembly.GetName().Version?.ToString(),
                    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
                },
                server = new
                {
                    serverTime = DateTime.UtcNow,
                    timezone = TimeZoneInfo.Local.DisplayName,
                    uptime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime
                },
                features = new
                {
                    healthChecks = true,
                    responseCompression = true,
                    responseCache = true,
                    rateLimiting = true,
                    logging = true
                }
            };

            return Ok(info);
        }
    }
}

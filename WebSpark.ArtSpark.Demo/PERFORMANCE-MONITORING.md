# ArtSpark Performance & Monitoring Guide

## Overview

This guide covers the performance monitoring, health checks, and advanced deployment features implemented in WebSpark.ArtSpark.

## 🚀 Quick Start

### Local Development

```bash
# Start the application
dotnet run

# Check health status
curl https://localhost:5001/health

# View performance metrics
curl https://localhost:5001/api/monitoring/metrics
```

### Production Deployment

```powershell
# Traditional IIS deployment
.\Deploy-Advanced.ps1 -DeployMode iis

# Container deployment
.\Deploy-Advanced.ps1 -DeployMode docker

# Both deployments
.\Deploy-Advanced.ps1 -DeployMode both
```

## 📊 Performance Monitoring

### Health Check Endpoints

| Endpoint | Purpose | Response Format |
|----------|---------|-----------------|
| `/health` | Complete health status | JSON with all checks |
| `/health/ready` | Readiness probe | Simple status |
| `/health/live` | Liveness probe | Always healthy |

### Monitoring API

| Endpoint | Purpose | Description |
|----------|---------|-------------|
| `/api/monitoring/health` | Health status | Detailed health information |
| `/api/monitoring/metrics` | System metrics | Memory, CPU, GC stats |
| `/api/monitoring/info` | Application info | Build info, environment |

### Built-in Health Checks

1. **Database Health**
   - Tests SQLite database connection
   - Validates database schema
   - Tags: `db`, `ready`

2. **Memory Health**
   - Monitors memory usage (512MB limit)
   - Tracks garbage collection stats
   - Tags: `memory`, `ready`

3. **Disk Space Health**
   - Monitors available disk space (1GB minimum)
   - Tracks drive capacity
   - Tags: `storage`, `ready`

4. **External API Health**
   - Tests Art Institute API connectivity
   - Validates API response
   - Tags: `external`, `ready`

## 🔧 Performance Features

### Response Compression

- **Brotli** compression for modern browsers
- **Gzip** fallback for older browsers
- Enabled for HTTPS connections
- Configurable compression levels

### Response Caching

- Memory-based response caching
- Configurable cache duration
- Case-insensitive path matching
- 1MB body size limit

### Rate Limiting

- **API endpoints**: 100 requests/minute
- **General requests**: 200 requests/minute
- Sliding window algorithm
- Configurable queue limits

### Memory Management

- **Memory cache**: 1000 entries max
- **Compaction**: 25% removal when full
- **GC optimization**: Server GC enabled
- **Concurrent GC**: Enabled for better performance

## 📈 Performance Testing

### Automated Benchmarking

```powershell
# Run performance tests
.\Test-Performance.ps1 -BaseUrl "https://localhost:5001" -Iterations 100

# Generate detailed report
.\Test-Performance.ps1 -OutputFile "perf-report.json"
```

### Key Metrics Measured

- **Response Time**: Average, P95, P99 percentiles
- **Success Rate**: Percentage of successful requests
- **Throughput**: Requests per second
- **Resource Usage**: Memory, CPU, disk

### Performance Targets

- **Home Page**: < 200ms average response time
- **API Endpoints**: < 100ms average response time
- **Success Rate**: > 99.5%
- **Memory Usage**: < 512MB steady state

## 🐳 Container Deployment

### Docker Features

- **Multi-stage build** for optimized image size
- **Non-root user** for security
- **Health checks** built into container
- **Environment configuration** support

### Container Commands

```bash
# Build container
docker build -t artspark:latest .

# Run container
docker run -p 8080:8080 artspark:latest

# Run with docker-compose
docker-compose up -d

# View logs
docker-compose logs -f
```

### Container Configuration

```yaml
# docker-compose.yml
version: '3.8'
services:
  artspark:
    image: artspark:latest
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    volumes:
      - ./data:/app/wwwroot
      - ./logs:/app/logs
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health/live"]
      interval: 30s
      timeout: 10s
      retries: 3
```

## 🔒 Security Enhancements

### Code Analysis

- **Security ruleset** with 100+ security rules
- **Vulnerability scanning** for NuGet packages
- **Static analysis** with .NET analyzers
- **Quality gates** in CI/CD pipeline

### Runtime Security

- **HTTPS enforcement** in production
- **HSTS headers** with 1-year max-age
- **Rate limiting** to prevent abuse
- **Secure cookie settings**

### Best Practices

- **Least privilege** principle
- **Input validation** on all endpoints
- **Output encoding** for XSS prevention
- **SQL injection** protection with EF Core

## 🛠️ Advanced Deployment

### Deployment Modes

#### IIS Deployment

```powershell
.\Deploy-Advanced.ps1 -DeployMode iis -VersionType minor
```

- Publishes to `C:\PublishedWebsites\ArtSpark`
- Creates automatic backups
- Generates deployment packages
- Optimized for Windows Server

#### Container Deployment

```powershell
.\Deploy-Advanced.ps1 -DeployMode docker -BuildContainer -PushContainer
```

- Creates optimized Docker image
- Generates docker-compose.yml
- Supports container registry push
- Linux-ready runtime

#### Hybrid Deployment

```powershell
.\Deploy-Advanced.ps1 -DeployMode both
```

- Creates both IIS and container deployments
- Useful for migration scenarios
- Maintains deployment flexibility

### Version Management

- **Semantic versioning** (Major.Minor.Patch)
- **Automatic version bumping** during deployment
- **Build metadata** in assemblies
- **Git integration** for version tracking

## 📝 Configuration

### appsettings.json Sections

#### Performance Configuration

```json
{
  "Performance": {
    "ResponseCaching": {
      "DefaultDurationSeconds": 300,
      "MaxCacheSizeMB": 100
    },
    "Compression": {
      "EnableBrotli": true,
      "EnableGzip": true,
      "Level": "Optimal"
    },
    "RateLimit": {
      "GeneralRequestsPerMinute": 200,
      "ApiRequestsPerMinute": 100
    }
  }
}
```

#### Health Check Configuration

```json
{
  "HealthChecks": {
    "MemoryLimitMB": 512,
    "DiskSpaceLimitGB": 1,
    "ApiTimeoutSeconds": 10
  }
}
```

#### Security Configuration

```json
{
  "Security": {
    "RequireHttps": true,
    "HstsMaxAgeSeconds": 31536000,
    "DataProtection": {
      "ApplicationName": "WebSpark.ArtSpark",
      "KeyLifetimeInDays": 90
    }
  }
}
```

## 🔍 Troubleshooting

### Common Issues

#### High Memory Usage

1. Check memory health endpoint: `/api/monitoring/metrics`
2. Review GC statistics
3. Adjust memory cache limits
4. Consider increasing server memory

#### Slow Response Times

1. Run performance benchmarks
2. Check database connection health
3. Review external API response times
4. Enable detailed logging

#### Health Check Failures

1. Check specific health endpoint: `/health`
2. Review individual check status
3. Verify database connectivity
4. Test external API availability

### Monitoring Commands

```powershell
# Check application health
Invoke-RestMethod -Uri "https://localhost:5001/health"

# Get system metrics
Invoke-RestMethod -Uri "https://localhost:5001/api/monitoring/metrics"

# Run performance test
.\Test-Performance.ps1 -BaseUrl "https://localhost:5001"
```

## 🚀 Future Enhancements

### Planned Features

- **Application Insights** integration
- **Distributed tracing** with OpenTelemetry
- **Custom metrics** collection
- **Advanced alerting** capabilities
- **Load balancer** health checks
- **Circuit breaker** patterns

### Monitoring Improvements

- **Dashboard** for real-time monitoring
- **Historical metrics** storage
- **Performance alerts** via email/Slack
- **Automated scaling** based on metrics

## 📚 References

- [ASP.NET Core Health Checks](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)
- [.NET 9 Performance Improvements](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-9/)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [Security Best Practices](https://docs.microsoft.com/en-us/aspnet/core/security/)

---

*Last updated: $(Get-Date -Format 'yyyy-MM-dd')*

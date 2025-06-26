# ArtSpark IIS Deployment Guide

## Quick Deployment Process

### 1. Local Build & Package Creation

Run one of these scripts from the project directory:

**PowerShell (Recommended):**

```powershell
.\Deploy-ArtSpark.ps1
```

**Batch File (Alternative):**

```batch
Deploy-ArtSpark.bat
```

**Manual Commands:**

```bash
# Build and publish
dotnet publish --configuration Release --output "C:\PublishedWebsites\ArtSpark" --runtime win-x64

# Create package (using PowerShell)
Compress-Archive -Path "C:\PublishedWebsites\ArtSpark\*" -DestinationPath "C:\PublishedWebsites\Packages\ArtSpark_$(Get-Date -Format 'yyyyMMdd_HHmmss').zip"
```

### 2. Server Deployment Steps

1. **Stop IIS Application Pool**
   - Open IIS Manager
   - Navigate to Application Pools
   - Right-click on ArtSpark app pool → Stop

2. **Backup Current Website (Optional)**

   ```cmd
   xcopy "C:\inetpub\wwwroot\ArtSpark" "C:\Backups\ArtSpark_backup_%date:~-4%%date:~3,2%%date:~0,2%" /E /I
   ```

3. **Deploy New Version**
   - Extract the ZIP package to your IIS website directory
   - Overwrite existing files

4. **Verify Configuration**
   - Ensure web.config is present
   - Check that logs directory exists
   - Verify file permissions

5. **Start IIS Application Pool**
   - Right-click on ArtSpark app pool → Start

6. **Test Deployment**
   - Browse to your website
   - Check `/version.json` for build information
   - Verify PWA functionality

## IIS Configuration Requirements

### Application Pool Settings

- **.NET CLR Version:** No Managed Code (for .NET 9.0)
- **Managed Pipeline Mode:** Integrated
- **Enable 32-Bit Applications:** False
- **Identity:** ApplicationPoolIdentity (recommended)

### Prerequisites

- .NET 9.0 Runtime (or SDK)
- ASP.NET Core Module v2
- IIS with ASP.NET Core support

### Security Configuration

- Ensure IIS_IUSRS has read permissions on website directory
- Application pool identity needs modify permissions on logs directory
- Consider setting up SSL/TLS certificates for HTTPS

## File Structure After Deployment

```
Website Root/
├── wwwroot/              # Static files
│   ├── css/
│   ├── js/
│   ├── img/
│   ├── sw.js             # Service Worker
│   └── site.webmanifest  # PWA Manifest
├── WebSpark.ArtSpark.Demo.dll
├── web.config            # IIS Configuration
├── version.json          # Build Information
├── appsettings.json      # App Configuration
├── logs/                 # Application Logs
└── [Other runtime files]
```

## Troubleshooting

### Common Issues

**Application won't start:**

- Check Event Viewer → Windows Logs → Application
- Verify .NET 9.0 is installed
- Check application pool configuration
- Review web.config for errors

**Static files not loading:**

- Verify wwwroot directory exists
- Check IIS static content settings
- Ensure MIME types are configured

**PWA not working:**

- Verify service worker (sw.js) is accessible
- Check manifest (site.webmanifest) is served correctly
- Ensure HTTPS is configured (required for PWA)

**Performance issues:**

- Enable compression in IIS
- Configure static content caching
- Monitor application logs

### Log Files

- **Application Logs:** `logs/` directory in website root
- **IIS Logs:** `%SystemDrive%\inetpub\logs\LogFiles\`
- **Event Logs:** Windows Event Viewer

## Production Environment Checklist

- [ ] .NET 9.0 Runtime installed
- [ ] IIS with ASP.NET Core Module v2
- [ ] Application pool configured correctly
- [ ] File permissions set properly
- [ ] SSL certificate installed (for HTTPS)
- [ ] Firewall rules configured
- [ ] Database connection strings updated
- [ ] User secrets configured for production
- [ ] Log rotation configured
- [ ] Backup strategy in place

## PowerShell Script Options

The `Deploy-ArtSpark.ps1` script supports several parameters:

```powershell
# Basic usage
.\Deploy-ArtSpark.ps1

# Specify version increment type
.\Deploy-ArtSpark.ps1 -VersionType "minor"

# Use custom version
.\Deploy-ArtSpark.ps1 -CustomVersion "2.1.0"

# Skip build step (if already built)
.\Deploy-ArtSpark.ps1 -SkipBuild

# Custom paths
.\Deploy-ArtSpark.ps1 -PublishPath "D:\Websites\ArtSpark" -PackagePath "D:\Packages"

# Open package folder after creation
.\Deploy-ArtSpark.ps1 -OpenPackageFolder
```

## Monitoring and Maintenance

### Health Checks

- Monitor application startup time
- Check memory usage
- Verify database connectivity
- Test PWA installation

### Regular Tasks

- Review application logs
- Update SSL certificates
- Apply security updates
- Monitor disk space
- Backup database and application files

For additional support, contact the development team.

# ArtSpark IIS Deployment - Quick Reference

## 🚀 One-Click Deployment

```powershell
.\Deploy-ArtSpark.ps1
```

## 📁 File Locations

- **Source**: `C:\GitHub\MarkHazleton\WebSpark.ArtSpark\WebSpark.ArtSpark.Demo\`
- **Published**: `C:\PublishedWebsites\ArtSpark\`
- **Packages**: `C:\PublishedWebsites\Packages\`

## 🔄 Server Deployment

1. **Stop** IIS App Pool
2. **Extract** ZIP to IIS directory  
3. **Start** IIS App Pool
4. **Test** website

## ⚙️ IIS Settings

- **Pool**: No Managed Code
- **Runtime**: .NET 9.0
- **Pipeline**: Integrated

## 📋 Files Created

- ✅ `Deploy-ArtSpark.ps1` - PowerShell deployment script
- ✅ `Deploy-ArtSpark.bat` - Batch deployment script  
- ✅ `IISProduction.pubxml` - Publish profile
- ✅ `web.config` - IIS configuration
- ✅ `DEPLOYMENT.md` - Complete guide

## 🎯 Ready to Deploy

Run the deployment script and copy the generated ZIP file to your server.

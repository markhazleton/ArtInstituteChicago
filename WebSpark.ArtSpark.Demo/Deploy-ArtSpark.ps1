# ArtSpark IIS Deployment Script
# This script builds, publishes, versions, and packages the application for IIS deployment

param(
    [string]$VersionType = "patch", # major, minor, patch
    [string]$CustomVersion = "", # Optional: specify exact version like "1.2.3"
    [string]$PublishPath = "C:\PublishedWebsites\ArtSpark",
    [string]$PackagePath = "C:\PublishedWebsites\Packages",
    [switch]$SkipBuild = $false,
    [switch]$OpenPackageFolder
)

# Colors for console output
$ErrorColor = "Red"
$WarningColor = "Yellow"
$InfoColor = "Cyan"
$SuccessColor = "Green"

function Write-InfoMessage($message) {
    Write-Host $message -ForegroundColor $InfoColor
}

function Write-SuccessMessage($message) {
    Write-Host $message -ForegroundColor $SuccessColor
}

function Write-ErrorMessage($message) {
    Write-Host $message -ForegroundColor $ErrorColor
}

function Write-WarningMessage($message) {
    Write-Host $message -ForegroundColor $WarningColor
}

# Get the script directory and project path
$ScriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectPath = $ScriptPath
$ProjectFile = Join-Path $ProjectPath "WebSpark.ArtSpark.Demo.csproj"

Write-InfoMessage "=== ArtSpark IIS Deployment Script ==="
Write-InfoMessage "Project Path: $ProjectPath"
Write-InfoMessage "Publish Path: $PublishPath"
Write-InfoMessage "Package Path: $PackagePath"

# Verify project file exists
if (!(Test-Path $ProjectFile)) {
    Write-ErrorMessage "Project file not found: $ProjectFile"
    exit 1
}

# Create package directory if it doesn't exist
if (!(Test-Path $PackagePath)) {
    New-Item -ItemType Directory -Path $PackagePath -Force | Out-Null
    Write-InfoMessage "Created package directory: $PackagePath"
}

# Function to get current version from project file
function Get-CurrentVersion {
    $projectContent = Get-Content $ProjectFile -Raw
    if ($projectContent -match '<Version>(.*?)</Version>') {
        return $matches[1]
    }
    return "1.0.0"
}

# Function to increment version
function Get-NextVersion($currentVersion, $versionType) {
    $parts = $currentVersion.Split('.')
    $major = [int]$parts[0]
    $minor = [int]$parts[1]
    $patch = [int]$parts[2]
    
    switch ($versionType.ToLower()) {
        "major" { 
            $major++
            $minor = 0
            $patch = 0
        }
        "minor" { 
            $minor++
            $patch = 0
        }
        "patch" { 
            $patch++
        }
        default {
            Write-WarningMessage "Unknown version type: $versionType. Using patch increment."
            $patch++
        }
    }
    
    return "$major.$minor.$patch"
}

# Determine new version
$currentVersion = Get-CurrentVersion
if ($CustomVersion) {
    $newVersion = $CustomVersion
    Write-InfoMessage "Using custom version: $newVersion"
}
else {
    $newVersion = Get-NextVersion $currentVersion $VersionType
    Write-InfoMessage "Current version: $currentVersion"
    Write-InfoMessage "New version ($VersionType increment): $newVersion"
}

# Update version in project file
Write-InfoMessage "Updating project version..."
$projectContent = Get-Content $ProjectFile -Raw
$projectContent = $projectContent -replace '<Version>.*?</Version>', "<Version>$newVersion</Version>"
$projectContent = $projectContent -replace '<AssemblyVersion>.*?</AssemblyVersion>', "<AssemblyVersion>$newVersion.0</AssemblyVersion>"
$projectContent = $projectContent -replace '<FileVersion>.*?</FileVersion>', "<FileVersion>$newVersion.0</FileVersion>"
Set-Content -Path $ProjectFile -Value $projectContent -Encoding UTF8
Write-SuccessMessage "Updated project version to: $newVersion"

# Build the application
if (-not $SkipBuild) {
    Write-InfoMessage "Building application in Release mode..."
    dotnet build $ProjectFile --configuration Release --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        Write-ErrorMessage "Build failed!"
        exit 1
    }
    Write-SuccessMessage "Build completed successfully."
}

# Publish the application
Write-InfoMessage "Publishing application to: $PublishPath"
dotnet publish $ProjectFile `
    --configuration Release `
    --output $PublishPath `
    --verbosity minimal `
    --self-contained false `
    --runtime win-x64

if ($LASTEXITCODE -ne 0) {
    Write-ErrorMessage "Publish failed!"
    exit 1
}
Write-SuccessMessage "Application published successfully."

# Create version info file
$versionInfo = @{
    Version     = $newVersion
    BuildDate   = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    GitCommit   = if (Get-Command git -ErrorAction SilentlyContinue) { git rev-parse --short HEAD } else { "N/A" }
    GitBranch   = if (Get-Command git -ErrorAction SilentlyContinue) { git branch --show-current } else { "N/A" }
    PublishedBy = $env:USERNAME
    MachineName = $env:COMPUTERNAME
} | ConvertTo-Json -Depth 2

$versionInfoPath = Join-Path $PublishPath "version.json"
Set-Content -Path $versionInfoPath -Value $versionInfo -Encoding UTF8
Write-InfoMessage "Created version info file: $versionInfoPath"

# Create deployment package
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$packageName = "ArtSpark_v$newVersion`_$timestamp.zip"
$packageFullPath = Join-Path $PackagePath $packageName

Write-InfoMessage "Creating deployment package: $packageName"

# Use .NET compression for better compatibility
Add-Type -AssemblyName System.IO.Compression.FileSystem
try {
    [System.IO.Compression.ZipFile]::CreateFromDirectory($PublishPath, $packageFullPath, [System.IO.Compression.CompressionLevel]::Optimal, $false)
    Write-SuccessMessage "Package created successfully: $packageFullPath"
}
catch {
    Write-ErrorMessage "Failed to create package: $($_.Exception.Message)"
    exit 1
}

# Get package size
$packageSize = (Get-Item $packageFullPath).Length
$packageSizeMB = [math]::Round($packageSize / 1MB, 2)

# Create deployment instructions
$deploymentInstructions = @"
ArtSpark Deployment Package
==========================

Version: $newVersion
Package: $packageName
Size: $packageSizeMB MB
Created: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")

Deployment Instructions:
1. Stop the IIS Application Pool for ArtSpark
2. Backup the current website directory (optional but recommended)
3. Extract this zip file to your IIS website root directory
4. Ensure the application pool is using .NET 9.0
5. Start the IIS Application Pool
6. Verify the deployment by checking the version.json file

IIS Configuration Requirements:
- .NET 9.0 Runtime (or SDK)
- Application Pool Target Framework: .NET 9.0
- Managed Pipeline Mode: Integrated
- Enable 32-Bit Applications: False (for x64)

Troubleshooting:
- Check Windows Event Log for any errors
- Verify file permissions on the website directory
- Ensure all required .NET 9.0 dependencies are installed
- Check that User Secrets are properly configured for production

For support, contact: $env:USERNAME
"@

$instructionsPath = Join-Path $PackagePath "ArtSpark_v$newVersion`_$timestamp`_DeploymentInstructions.txt"
Set-Content -Path $instructionsPath -Value $deploymentInstructions -Encoding UTF8

# Display summary
Write-InfoMessage ""
Write-InfoMessage "=== Deployment Summary ==="
Write-SuccessMessage "✓ Version updated: $currentVersion → $newVersion"
Write-SuccessMessage "✓ Application built and published"
Write-SuccessMessage "✓ Package created: $packageName ($packageSizeMB MB)"
Write-SuccessMessage "✓ Deployment instructions created"
Write-InfoMessage ""
Write-InfoMessage "Package Location: $packageFullPath"
Write-InfoMessage "Instructions: $instructionsPath"
Write-InfoMessage ""
Write-WarningMessage "Next Steps:"
Write-WarningMessage "1. Copy the package file to your server"
Write-WarningMessage "2. Follow the deployment instructions"
Write-WarningMessage "3. Test the deployment"

# Open package folder
if ($OpenPackageFolder) {
    Start-Process "explorer.exe" -ArgumentList "/select,`"$packageFullPath`""
}

Write-SuccessMessage "Deployment preparation completed successfully!"

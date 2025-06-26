# Advanced Deployment Script with Container Support
# Provides options for traditional IIS deployment and modern containerized deployment

param(
    [ValidateSet("iis", "docker", "both")]
    [string]$DeployMode = "iis",
    
    [ValidateSet("patch", "minor", "major")]
    [string]$VersionType = "patch",
    
    [string]$Environment = "Production",
    
    [switch]$SkipTests,
    [switch]$SkipSecurity,
    [switch]$BuildContainer,
    [switch]$PushContainer,
    [string]$ContainerRegistry = "",
    [switch]$Force
)

$ErrorActionPreference = "Stop"

# Configuration
$ProjectName = "WebSpark.ArtSpark.Demo"
$ProjectFile = "$ProjectName.csproj"
$SolutionFile = "$ProjectName.sln"
$DockerFile = "Dockerfile"

function Write-InfoMessage($message) {
    Write-Host $message -ForegroundColor Cyan
}

function Write-SuccessMessage($message) {
    Write-Host $message -ForegroundColor Green
}

function Write-ErrorMessage($message) {
    Write-Host $message -ForegroundColor Red
}

function Write-WarningMessage($message) {
    Write-Host $message -ForegroundColor Yellow
}

function Get-NextVersion($currentVersion, $versionType) {
    $version = [System.Version]::Parse($currentVersion)
    switch ($versionType) {
        "major" { 
            return [System.Version]::new($version.Major + 1, 0, 0)
        }
        "minor" { 
            return [System.Version]::new($version.Major, $version.Minor + 1, 0)
        }
        "patch" { 
            return [System.Version]::new($version.Major, $version.Minor, $version.Build + 1)
        }
    }
}

function Test-Prerequisites() {
    Write-InfoMessage "🔍 Checking prerequisites..."
    
    # Check .NET SDK
    try {
        $dotnetVersion = & dotnet --version
        Write-InfoMessage "✅ .NET SDK: $dotnetVersion"
    }
    catch {
        throw "❌ .NET SDK not found. Please install .NET 9 SDK."
    }
    
    # Check Docker if needed
    if ($DeployMode -eq "docker" -or $DeployMode -eq "both" -or $BuildContainer) {
        try {
            $dockerVersion = & docker --version
            Write-InfoMessage "✅ Docker: $dockerVersion"
            
            # Test docker daemon
            & docker info | Out-Null
        }
        catch {
            throw "❌ Docker not available. Please ensure Docker Desktop is running."
        }
    }
    
    # Check project files
    if (-not (Test-Path $ProjectFile)) {
        throw "❌ Project file not found: $ProjectFile"
    }
    
    Write-SuccessMessage "✅ All prerequisites satisfied"
}

function Update-ProjectVersion($newVersion) {
    Write-InfoMessage "📝 Updating project version to $newVersion..."
    
    $projectContent = Get-Content $ProjectFile -Raw
    $projectContent = $projectContent -replace '<Version>[\d\.]+</Version>', "<Version>$newVersion</Version>"
    $projectContent = $projectContent -replace '<AssemblyVersion>[\d\.]+</AssemblyVersion>', "<AssemblyVersion>$newVersion.0</AssemblyVersion>"
    $projectContent = $projectContent -replace '<FileVersion>[\d\.]+</FileVersion>', "<FileVersion>$newVersion.0</FileVersion>"
    
    Set-Content $ProjectFile -Value $projectContent -NoNewline
    Write-SuccessMessage "✅ Project version updated"
}

function Build-Application() {
    Write-InfoMessage "🔨 Building application..."
    
    # Restore packages
    & dotnet restore $ProjectFile
    if ($LASTEXITCODE -ne 0) { throw "❌ Package restore failed" }
    
    # Build
    & dotnet build $ProjectFile --configuration Release --no-restore
    if ($LASTEXITCODE -ne 0) { throw "❌ Build failed" }
    
    Write-SuccessMessage "✅ Application built successfully"
}

function Run-Tests() {
    if ($SkipTests) {
        Write-WarningMessage "⚠️ Skipping tests"
        return
    }
    
    Write-InfoMessage "🧪 Running tests..."
    
    # Run unit tests
    & dotnet test --configuration Release --no-build --logger "console;verbosity=minimal"
    if ($LASTEXITCODE -ne 0) { 
        if (-not $Force) {
            throw "❌ Tests failed. Use -Force to deploy anyway."
        }
        Write-WarningMessage "⚠️ Tests failed but continuing due to -Force flag"
    }
    
    Write-SuccessMessage "✅ Tests completed"
}

function Run-SecurityScan() {
    if ($SkipSecurity) {
        Write-WarningMessage "⚠️ Skipping security scan"
        return
    }
    
    Write-InfoMessage "🔒 Running security analysis..."
    
    # Check for vulnerable packages
    try {
        & dotnet list package --vulnerable --include-transitive 2>$null
        if ($LASTEXITCODE -eq 0) {
            Write-WarningMessage "⚠️ Vulnerable packages found. Consider updating."
        }
    }
    catch {
        Write-InfoMessage "ℹ️ Security scan completed (no vulnerabilities tool available)"
    }
    
    Write-SuccessMessage "✅ Security analysis completed"
}

function Deploy-IIS() {
    Write-InfoMessage "🚀 Publishing for IIS deployment..."
    
    $publishPath = "C:\PublishedWebsites\ArtSpark"
    $backupPath = "C:\PublishedWebsites\Backups\ArtSpark-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
    
    # Create backup if existing deployment exists
    if (Test-Path $publishPath) {
        Write-InfoMessage "📦 Creating backup..."
        Copy-Item $publishPath $backupPath -Recurse -Force
        Write-InfoMessage "✅ Backup created: $backupPath"
    }
    
    # Publish application
    & dotnet publish $ProjectFile --configuration Release --output $publishPath --no-build --runtime win-x64 --self-contained false
    if ($LASTEXITCODE -ne 0) { throw "❌ IIS publish failed" }
    
    # Create deployment package
    $packagePath = "ArtSpark-$(Get-Date -Format 'yyyyMMdd-HHmmss').zip"
    Compress-Archive -Path "$publishPath\*" -DestinationPath $packagePath -Force
    
    Write-SuccessMessage "✅ IIS deployment package created: $packagePath"
    Write-InfoMessage "📂 Published to: $publishPath"
}

function Build-DockerImage($version) {
    Write-InfoMessage "🐳 Building Docker container..."
    
    # Create Dockerfile if it doesn't exist
    if (-not (Test-Path $DockerFile)) {
        Create-Dockerfile
    }
    
    $imageName = "artspark"
    $imageTag = "${imageName}:${version}"
    $latestTag = "${imageName}:latest"
    
    # Build container
    & docker build -t $imageTag -t $latestTag .
    if ($LASTEXITCODE -ne 0) { throw "❌ Docker build failed" }
    
    Write-SuccessMessage "✅ Docker image built: $imageTag"
    
    # Push to registry if specified
    if ($PushContainer -and $ContainerRegistry) {
        $registryImageTag = "${ContainerRegistry}/${imageTag}"
        $registryLatestTag = "${ContainerRegistry}/${latestTag}"
        
        & docker tag $imageTag $registryImageTag
        & docker tag $latestTag $registryLatestTag
        & docker push $registryImageTag
        & docker push $registryLatestTag
        
        Write-SuccessMessage "✅ Docker image pushed to registry"
    }
    
    return $imageTag
}

function Create-Dockerfile() {
    Write-InfoMessage "📝 Creating Dockerfile..."
    
    $dockerfileContent = @'
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY *.csproj ./
COPY *.sln ./
RUN dotnet restore

# Copy source code
COPY . .
RUN dotnet build --configuration Release --no-restore

# Publish stage
FROM build AS publish
RUN dotnet publish --configuration Release --output /app --no-build --runtime linux-x64 --self-contained false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN chown -R appuser:appuser /app
USER appuser

# Copy published app
COPY --from=publish --chown=appuser:appuser /app .

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health/live || exit 1

# Expose port
EXPOSE 8080

# Set environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Start application
ENTRYPOINT ["dotnet", "WebSpark.ArtSpark.Demo.dll"]
'@
    
    Set-Content $DockerFile -Value $dockerfileContent
    Write-SuccessMessage "✅ Dockerfile created"
}

function Create-DockerCompose($imageTag) {
    Write-InfoMessage "📝 Creating docker-compose.yml..."
    
    $composeContent = @"
version: '3.8'

services:
  artspark:
    image: $imageTag
    container_name: artspark-app
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:8080
    volumes:
      - ./data:/app/wwwroot
      - ./logs:/app/logs
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health/live"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 60s
    restart: unless-stopped
    
volumes:
  data:
  logs:

networks:
  default:
    name: artspark-network
"@
    
    Set-Content "docker-compose.yml" -Value $composeContent
    Write-SuccessMessage "✅ docker-compose.yml created"
}

function Main() {
    $startTime = Get-Date
    
    Write-InfoMessage "╔══════════════════════════════════════════════════════════════╗"
    Write-InfoMessage "║              ArtSpark Advanced Deployment                   ║"
    Write-InfoMessage "╚══════════════════════════════════════════════════════════════╝"
    Write-InfoMessage ""
    Write-InfoMessage "Deploy Mode: $DeployMode"
    Write-InfoMessage "Environment: $Environment"
    Write-InfoMessage "Version Type: $VersionType"
    Write-InfoMessage "Started: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
    Write-InfoMessage ""
    
    try {
        # Prerequisites
        Test-Prerequisites
        
        # Get current version and calculate next version
        $currentVersion = (Select-Xml -Path $ProjectFile -XPath "//Version").Node.InnerText
        $newVersion = Get-NextVersion $currentVersion $VersionType
        
        Write-InfoMessage "📊 Version: $currentVersion → $newVersion"
        
        # Update version
        Update-ProjectVersion $newVersion
        
        # Build application
        Build-Application
        
        # Run tests
        Run-Tests
        
        # Security scan
        Run-SecurityScan
        
        # Deploy based on mode
        switch ($DeployMode) {
            "iis" {
                Deploy-IIS
            }
            "docker" {
                $imageTag = Build-DockerImage $newVersion
                Create-DockerCompose $imageTag
            }
            "both" {
                Deploy-IIS
                $imageTag = Build-DockerImage $newVersion
                Create-DockerCompose $imageTag
            }
        }
        
        $duration = (Get-Date) - $startTime
        Write-SuccessMessage ""
        Write-SuccessMessage "🎉 Deployment completed successfully!"
        Write-SuccessMessage "⏱️ Total time: $($duration.TotalMinutes.ToString('F1')) minutes"
        Write-SuccessMessage "🔖 Version: $newVersion"
        
        if ($DeployMode -eq "docker" -or $DeployMode -eq "both") {
            Write-InfoMessage ""
            Write-InfoMessage "🐳 To start the container:"
            Write-InfoMessage "   docker-compose up -d"
            Write-InfoMessage ""
            Write-InfoMessage "🔍 To view logs:"
            Write-InfoMessage "   docker-compose logs -f"
        }
        
    }
    catch {
        Write-ErrorMessage ""
        Write-ErrorMessage "❌ Deployment failed: $($_.Exception.Message)"
        Write-ErrorMessage "💡 Check the error details above and try again"
        exit 1
    }
}

# Run main function
Main

# Enhanced Development Environment Setup
# Sets up local        # Install additional global tools
$globalTools = @(
    "dotnet-reportgenerator-globaltool",
    "dotnet-sonarscanner"
)
        
foreach ($tool in $globalTools) {
    $toolInstalled = dotnet tool list --global | Select-String $tool
    if (-not $toolInstalled) {
        Write-InfoMessage "Installing $tool..."
        try {
            dotnet tool install --global $tool
        }
        catch {
            Write-Host "Warning: Failed to install $tool" -ForegroundColor Yellow
        }
    }
}
        
Write-SuccessMessage "✅ Development tools installed"
}
    
# Setup VS Code configuration
if ($SetupVSCode) {
    Write-InfoMessage "Setting up VS Code configuration..."
        
    $vscodeDir = ".vscode"
    if (-not (Test-Path $vscodeDir)) {
        New-Item -ItemType Directory -Path $vscodeDir | Out-Null
    }
        
    # Create settings.json
    $settingsContent = @'
{
    "dotnet.defaultSolution": "WebSpark.ArtSpark.Demo.sln",
    "files.exclude": {
        "**/bin": true,
        "**/obj": true,
        "**/.vs": true
    },
    "editor.formatOnSave": true,
    "editor.codeActionsOnSave": {
        "source.fixAll": true,
        "source.organizeImports": true
    },
    "csharp.semanticHighlighting.enabled": true,
    "csharp.format.enable": true,
    "omnisharp.enableEditorConfigSupport": true,
    "omnisharp.enableRoslynAnalyzers": true
}
'@
    Set-Content "$vscodeDir/settings.json" -Value $settingsContent
        
    # Create launch.json
    $launchContent = @'
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Launch (web)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/bin/Debug/net9.0/WebSpark.ArtSpark.Demo.dll",
            "args": [],
            "cwd": "${workspaceFolder}",
            "stopAtEntry": false,
            "serverReadyAction": {
                "action": "openExternally",
                "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
            },
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            }
        }
    ]
}
'@
    Set-Content "$vscodeDir/launch.json" -Value $launchContent
        
    Write-SuccessMessage "✅ VS Code configuration completed"
}
    
# Setup Git configuration
if ($SetupGit) {
    Write-InfoMessage "Setting up Git configuration..."
        
    # Create .gitignore if it doesn't exist
    if (-not (Test-Path ".gitignore")) {
        $gitignoreContent = @'
# Build results
[Dd]ebug/
[Rr]elease/
x64/
x86/
[Bb]in/
[Oo]bj/

# Visual Studio
.vs/
*.user
*.suo

# Database
*.db
*.db-shm
*.db-wal

# Logs
logs/
*.log

# Published websites
C:\PublishedWebsites/

# Performance reports
performance-report.json
'@
        Set-Content ".gitignore" -Value $gitignoreContent
        Write-SuccessMessage "✅ .gitignore created"
    }
}
    
# Create development scripts
if ($CreateScripts) {
    Write-InfoMessage "Creating development scripts..."
        
    # Create run-dev script
    $runDevScript = @'
# Quick run script for development
Write-Host "Starting ArtSpark in development mode..." -ForegroundColor Green
dotnet watch run --project WebSpark.ArtSpark.Demo.csproj
'@
    Set-Content "run-dev.ps1" -Value $runDevScript
        
    # Create test script
    $testScript = @'
# Quick test script
Write-Host "Running tests..." -ForegroundColor Green
dotnet test --configuration Debug --logger "console;verbosity=detailed"
'@
    Set-Content "run-tests.ps1" -Value $testScript
        
    Write-SuccessMessage "✅ Development scripts created"
}evelopment environment with all dependencies and tools

param(
    [switch]$RestorePackages,
    [switch]$UpdateDatabase,
    [switch]$InstallTools,
    [switch]$SetupVSCode,
    [switch]$SetupGit,
    [switch]$CreateScripts,
    [switch]$All
)

if ($All) {
    $RestorePackages = $true
    $UpdateDatabase = $true
    $InstallTools = $true
    $SetupVSCode = $true
    $SetupGit = $true
    $CreateScripts = $true
}

function Write-InfoMessage($message) {
    Write-Host $message -ForegroundColor Cyan
}

function Write-SuccessMessage($message) {
    Write-Host $message -ForegroundColor Green
}

function Write-ErrorMessage($message) {
    Write-Host $message -ForegroundColor Red
}

$ScriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-InfoMessage "=== ArtSpark Development Setup ==="

try {
    # Install development tools
    if ($InstallTools) {
        Write-InfoMessage "Installing development tools..."
        
        # Check if dotnet-ef is installed
        $efInstalled = dotnet tool list --global | Select-String "dotnet-ef"
        if (-not $efInstalled) {
            Write-InfoMessage "Installing Entity Framework Core tools..."
            dotnet tool install --global dotnet-ef
        }
        
        # Check if dotnet-outdated is installed
        $outdatedInstalled = dotnet tool list --global | Select-String "dotnet-outdated"
        if (-not $outdatedInstalled) {
            Write-InfoMessage "Installing dotnet-outdated tool..."
            dotnet tool install --global dotnet-outdated-tool
        }
        
        Write-SuccessMessage "✓ Development tools installed"
    }
    
    # Restore packages
    if ($RestorePackages) {
        Write-InfoMessage "Restoring NuGet packages..."
        dotnet restore $ScriptPath
        if ($LASTEXITCODE -eq 0) {
            Write-SuccessMessage "✓ Packages restored"
        } else {
            throw "Package restore failed"
        }
    }
    
    # Update database
    if ($UpdateDatabase) {
        Write-InfoMessage "Updating database..."
        dotnet ef database update --project $ScriptPath
        if ($LASTEXITCODE -eq 0) {
            Write-SuccessMessage "✓ Database updated"
        } else {
            Write-ErrorMessage "Database update failed (this is OK for first run)"
        }
    }
    
    Write-InfoMessage ""
    Write-SuccessMessage "🎉 Development environment setup complete!"
    Write-InfoMessage ""
    Write-InfoMessage "Quick Start Commands:"
    Write-InfoMessage "  dotnet run                    # Start development server"
    Write-InfoMessage "  dotnet watch run              # Start with auto-reload"
    Write-InfoMessage "  dotnet test                   # Run tests"
    Write-InfoMessage "  .\Validate-Build.ps1          # Run full validation"
    Write-InfoMessage "  .\Deploy-Master.ps1           # Full deployment process"
    
} catch {
    Write-ErrorMessage "Setup failed: $($_.Exception.Message)"
    exit 1
}

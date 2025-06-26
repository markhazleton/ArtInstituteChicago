# Master Deployment Script - Orchestrates the entire deployment process
# Validates, builds, migrates database, and deploys the application

param(
    [string]$VersionType = "patch",
    [string]$Environment = "Production",
    [switch]$SkipValidation,
    [switch]$SkipDatabase,
    [switch]$SkipDeploy,
    [switch]$OpenPackageFolder
)

$ErrorActionPreference = "Stop"

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

$ScriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$startTime = Get-Date

Write-InfoMessage "╔══════════════════════════════════════════════════════════════╗"
Write-InfoMessage "║                 ArtSpark Master Deployment                  ║"
Write-InfoMessage "╚══════════════════════════════════════════════════════════════╝"
Write-InfoMessage ""
Write-InfoMessage "Environment: $Environment"
Write-InfoMessage "Version Type: $VersionType"
Write-InfoMessage "Started: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
Write-InfoMessage ""

$deploymentSteps = @()

try {
    # Step 1: Build Validation
    if (-not $SkipValidation) {
        Write-InfoMessage "🔍 Step 1: Running build validation..."
        $validationScript = Join-Path $ScriptPath "Validate-Build.ps1"
        
        if (Test-Path $validationScript) {
            & $validationScript -GenerateReport
            if ($LASTEXITCODE -ne 0) {
                throw "Build validation failed"
            }
            $deploymentSteps += "✓ Build validation passed"
            Write-SuccessMessage "Build validation completed successfully"
        }
        else {
            Write-WarningMessage "Validation script not found, skipping..."
        }
    }
    else {
        $deploymentSteps += "⚠ Build validation skipped"
        Write-WarningMessage "Build validation skipped"
    }
    
    Write-InfoMessage ""
    
    # Step 2: Database Migration
    if (-not $SkipDatabase) {
        Write-InfoMessage "🗄️  Step 2: Database migration..."
        $migrationScript = Join-Path $ScriptPath "Migrate-Database.ps1"
        
        if (Test-Path $migrationScript) {
            & $migrationScript -Environment $Environment -CreateBackup -ApplyMigrations
            if ($LASTEXITCODE -ne 0) {
                throw "Database migration failed"
            }
            $deploymentSteps += "✓ Database migration completed"
            Write-SuccessMessage "Database migration completed successfully"
        }
        else {
            Write-WarningMessage "Migration script not found, skipping..."
        }
    }
    else {
        $deploymentSteps += "⚠ Database migration skipped"
        Write-WarningMessage "Database migration skipped"
    }
    
    Write-InfoMessage ""
    
    # Step 3: Application Deployment
    if (-not $SkipDeploy) {
        Write-InfoMessage "🚀 Step 3: Application deployment..."
        $deployScript = Join-Path $ScriptPath "Deploy-ArtSpark.ps1"
        
        if (Test-Path $deployScript) {
            if ($OpenPackageFolder) {
                & $deployScript -VersionType $VersionType -OpenPackageFolder
            }
            else {
                & $deployScript -VersionType $VersionType
            }
            
            if ($LASTEXITCODE -ne 0) {
                throw "Application deployment failed"
            }
            $deploymentSteps += "✓ Application deployment completed"
            Write-SuccessMessage "Application deployment completed successfully"
        }
        else {
            throw "Deployment script not found: $deployScript"
        }
    }
    else {
        $deploymentSteps += "⚠ Application deployment skipped"
        Write-WarningMessage "Application deployment skipped"
    }
    
    # Success Summary
    $endTime = Get-Date
    $duration = $endTime - $startTime
    
    Write-InfoMessage ""
    Write-InfoMessage "╔══════════════════════════════════════════════════════════════╗"
    Write-SuccessMessage "║                   DEPLOYMENT SUCCESSFUL                     ║"
    Write-InfoMessage "╚══════════════════════════════════════════════════════════════╝"
    Write-InfoMessage ""
    Write-InfoMessage "Deployment Summary:"
    $deploymentSteps | ForEach-Object { Write-InfoMessage "  $_" }
    Write-InfoMessage ""
    Write-InfoMessage "Total Duration: $($duration.TotalMinutes.ToString('F2')) minutes"
    Write-InfoMessage "Completed: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
    Write-InfoMessage ""
    Write-SuccessMessage "🎉 Ready for production deployment!"
    Write-InfoMessage ""
    Write-InfoMessage "Next Steps:"
    Write-InfoMessage "1. Copy the deployment package to your server"
    Write-InfoMessage "2. Stop the IIS Application Pool"
    Write-InfoMessage "3. Extract the package to your IIS directory"
    Write-InfoMessage "4. Start the IIS Application Pool"
    Write-InfoMessage "5. Verify the deployment"
    
}
catch {
    $endTime = Get-Date
    $duration = $endTime - $startTime
    
    Write-InfoMessage ""
    Write-InfoMessage "╔══════════════════════════════════════════════════════════════╗"
    Write-ErrorMessage "║                    DEPLOYMENT FAILED                        ║"
    Write-InfoMessage "╚══════════════════════════════════════════════════════════════╝"
    Write-InfoMessage ""
    Write-ErrorMessage "Error: $($_.Exception.Message)"
    Write-InfoMessage ""
    Write-InfoMessage "Completed Steps:"
    $deploymentSteps | ForEach-Object { Write-InfoMessage "  $_" }
    Write-InfoMessage ""
    Write-InfoMessage "Duration: $($duration.TotalMinutes.ToString('F2')) minutes"
    Write-InfoMessage "Failed at: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
    
    exit 1
}

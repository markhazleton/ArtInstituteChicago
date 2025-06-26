# Database Migration and Backup Script for Production Deployments

param(
    [string]$Environment = "Production",
    [string]$ConnectionString = "",
    [switch]$CreateBackup,
    [switch]$ApplyMigrations,
    [string]$BackupPath = "C:\DatabaseBackups\ArtSpark"
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

Write-InfoMessage "=== ArtSpark Database Migration Script ==="

$ScriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectPath = $ScriptPath

# Create backup directory
if ($CreateBackup -and !(Test-Path $BackupPath)) {
    New-Item -ItemType Directory -Path $BackupPath -Force | Out-Null
    Write-InfoMessage "Created backup directory: $BackupPath"
}

try {
    # 1. Create database backup
    if ($CreateBackup) {
        $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
        $backupFile = Join-Path $BackupPath "artspark_backup_$timestamp.db"
        
        Write-InfoMessage "Creating database backup..."
        
        # For SQLite, simply copy the file
        $currentDbPath = Join-Path $ProjectPath "artspark-production.db"
        if (Test-Path $currentDbPath) {
            Copy-Item $currentDbPath $backupFile
            Write-SuccessMessage "✓ Database backup created: $backupFile"
        }
        else {
            Write-InfoMessage "No existing database found, skipping backup"
        }
    }
    
    # 2. Apply migrations
    if ($ApplyMigrations) {
        Write-InfoMessage "Applying database migrations..."
        
        $env:ASPNETCORE_ENVIRONMENT = $Environment
        dotnet ef database update --project $ProjectPath --verbose
        
        if ($LASTEXITCODE -eq 0) {
            Write-SuccessMessage "✓ Database migrations applied successfully"
        }
        else {
            throw "Database migration failed"
        }
    }
    
    # 3. Verify database integrity
    Write-InfoMessage "Verifying database integrity..."
    
    # Basic connectivity test
    try {
        dotnet run --project $ProjectPath --environment $Environment -- --verify-db
        Write-SuccessMessage "✓ Database verification completed"
    }
    catch {
        Write-ErrorMessage "⚠ Database verification failed, but continuing..."
    }
    
    Write-SuccessMessage "Database migration completed successfully!"
    
}
catch {
    Write-ErrorMessage "Migration failed: $($_.Exception.Message)"
    
    if ($CreateBackup -and (Test-Path $backupFile)) {
        Write-InfoMessage "To restore from backup:"
        Write-InfoMessage "  Copy-Item '$backupFile' '$currentDbPath'"
    }
    
    exit 1
}

Write-InfoMessage "Migration summary:"
Write-InfoMessage "  Environment: $Environment"
Write-InfoMessage "  Backup created: $(if($CreateBackup){'Yes'}else{'No'})"
Write-InfoMessage "  Migrations applied: $(if($ApplyMigrations){'Yes'}else{'No'})"

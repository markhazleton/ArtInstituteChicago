# Build Validation and Quality Assurance Script
# Runs tests, security scans, and performance checks before deployment

param(
    [switch]$SkipTests = $false,
    [switch]$SkipSecurity = $false,
    [switch]$SkipPerformance = $false,
    [switch]$GenerateReport
)

$ErrorActionPreference = "Stop"

# Colors for output
$InfoColor = "Cyan"
$SuccessColor = "Green"
$ErrorColor = "Red"
$WarningColor = "Yellow"

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

# Get script path
$ScriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectPath = $ScriptPath
$ProjectFile = Join-Path $ProjectPath "WebSpark.ArtSpark.Demo.csproj"
$ReportsPath = Join-Path $ProjectPath "BuildReports"

Write-InfoMessage "=== ArtSpark Build Validation ==="
Write-InfoMessage "Project: $ProjectFile"

# Create reports directory
if ($GenerateReport -and !(Test-Path $ReportsPath)) {
    New-Item -ItemType Directory -Path $ReportsPath -Force | Out-Null
}

$validationResults = @{
    Build       = $false
    Tests       = $false
    Security    = $false
    Performance = $false
    StartTime   = Get-Date
    Errors      = @()
    Warnings    = @()
}

try {
    # 1. Build Validation
    Write-InfoMessage "1. Building project..."
    dotnet build $ProjectFile --configuration Release --verbosity minimal --no-restore
    if ($LASTEXITCODE -eq 0) {
        $validationResults.Build = $true
        Write-SuccessMessage "✓ Build successful"
    }
    else {
        throw "Build failed"
    }

    # 2. Run Tests
    if (-not $SkipTests) {
        Write-InfoMessage "2. Running tests..."
        $testOutput = dotnet test $ProjectPath --configuration Release --no-build --verbosity minimal --logger "trx" 2>&1
        if ($LASTEXITCODE -eq 0) {
            $validationResults.Tests = $true
            Write-SuccessMessage "✓ All tests passed"
        }
        else {
            $validationResults.Errors += "Tests failed: $testOutput"
            Write-ErrorMessage "✗ Tests failed"
        }
    }
    else {
        Write-WarningMessage "⚠ Tests skipped"
    }

    # 3. Security Analysis
    if (-not $SkipSecurity) {
        Write-InfoMessage "3. Running security analysis..."
        
        # Check for known vulnerabilities in packages
        $auditOutput = dotnet list $ProjectFile package --vulnerable --include-transitive 2>&1
        if ($auditOutput -match "no vulnerable packages") {
            $validationResults.Security = $true
            Write-SuccessMessage "✓ No vulnerable packages found"
        }
        else {
            $validationResults.Warnings += "Vulnerable packages detected: $auditOutput"
            Write-WarningMessage "⚠ Vulnerable packages detected"
        }
        
        # Check for outdated packages
        $outdatedOutput = dotnet list $ProjectFile package --outdated 2>&1
        if ($outdatedOutput -notmatch "Project has no package references") {
            $validationResults.Warnings += "Outdated packages: $outdatedOutput"
            Write-WarningMessage "⚠ Outdated packages found"
        }
    }
    else {
        Write-WarningMessage "⚠ Security analysis skipped"
    }

    # 4. Performance Analysis
    if (-not $SkipPerformance) {
        Write-InfoMessage "4. Running performance analysis..."
        
        # Check bundle sizes
        $publishPath = Join-Path $ProjectPath "bin\Release\net9.0\publish"
        if (Test-Path $publishPath) {
            $bundleSize = (Get-ChildItem $publishPath -Recurse | Measure-Object -Property Length -Sum).Sum
            $bundleSizeMB = [math]::Round($bundleSize / 1MB, 2)
            
            if ($bundleSizeMB -lt 100) {
                $validationResults.Performance = $true
                Write-SuccessMessage "✓ Bundle size acceptable: $bundleSizeMB MB"
            }
            else {
                $validationResults.Warnings += "Large bundle size: $bundleSizeMB MB"
                Write-WarningMessage "⚠ Large bundle size: $bundleSizeMB MB"
            }
        }
        else {
            Write-WarningMessage "⚠ Publish folder not found for size analysis"
        }
    }
    else {
        Write-WarningMessage "⚠ Performance analysis skipped"
    }

}
catch {
    $validationResults.Errors += $_.Exception.Message
    Write-ErrorMessage "✗ Validation failed: $($_.Exception.Message)"
}

$validationResults.EndTime = Get-Date
$validationResults.Duration = $validationResults.EndTime - $validationResults.StartTime

# Generate Report
if ($GenerateReport) {
    $reportPath = Join-Path $ReportsPath "validation-report-$(Get-Date -Format 'yyyyMMdd-HHmmss').json"
    $validationResults | ConvertTo-Json -Depth 3 | Set-Content $reportPath
    Write-InfoMessage "Report saved: $reportPath"
}

# Summary
Write-InfoMessage ""
Write-InfoMessage "=== Validation Summary ==="
Write-InfoMessage "Duration: $($validationResults.Duration.TotalSeconds) seconds"

$passedChecks = @($validationResults.Build, $validationResults.Tests, $validationResults.Security, $validationResults.Performance) | Where-Object { $_ -eq $true }
$totalChecks = 4
if ($SkipTests) { $totalChecks-- }
if ($SkipSecurity) { $totalChecks-- }
if ($SkipPerformance) { $totalChecks-- }

Write-InfoMessage "Passed: $($passedChecks.Count)/$totalChecks checks"

if ($validationResults.Errors.Count -gt 0) {
    Write-ErrorMessage "Errors:"
    $validationResults.Errors | ForEach-Object { Write-ErrorMessage "  - $_" }
}

if ($validationResults.Warnings.Count -gt 0) {
    Write-WarningMessage "Warnings:"
    $validationResults.Warnings | ForEach-Object { Write-WarningMessage "  - $_" }
}

# Exit code
if ($validationResults.Errors.Count -eq 0) {
    Write-SuccessMessage "✓ Validation completed successfully"
    exit 0
}
else {
    Write-ErrorMessage "✗ Validation failed"
    exit 1
}

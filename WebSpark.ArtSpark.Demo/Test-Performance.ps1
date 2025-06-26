# Performance Benchmark Script
# Measures application performance metrics and generates reports

param(
    [string]$BaseUrl = "https://localhost:5001",
    [int]$Iterations = 100,
    [int]$ConcurrentUsers = 10,
    [string]$OutputFile = "performance-report.json"
)

$ErrorActionPreference = "Stop"

function Write-InfoMessage($message) {
    Write-Host $message -ForegroundColor Cyan
}

function Write-SuccessMessage($message) {
    Write-Host $message -ForegroundColor Green
}

function Measure-EndpointPerformance($url, $iterations) {
    $results = @()
    
    for ($i = 1; $i -le $iterations; $i++) {
        try {
            $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
            $response = Invoke-WebRequest -Uri $url -Method GET -UseBasicParsing -TimeoutSec 30
            $stopwatch.Stop()
            
            $results += @{
                Iteration     = $i
                ResponseTime  = $stopwatch.ElapsedMilliseconds
                StatusCode    = $response.StatusCode
                ContentLength = $response.Content.Length
            }
            
            if ($i % 10 -eq 0) {
                Write-Host "." -NoNewline
            }
        }
        catch {
            $results += @{
                Iteration     = $i
                ResponseTime  = -1
                StatusCode    = "Error"
                ContentLength = 0
                Error         = $_.Exception.Message
            }
        }
    }
    
    return $results
}

function Get-PerformanceMetrics($results) {
    $successfulRequests = $results | Where-Object { $_.StatusCode -eq 200 }
    $responseTimes = $successfulRequests | ForEach-Object { $_.ResponseTime }
    
    if ($responseTimes.Count -eq 0) {
        return @{
            TotalRequests       = $results.Count
            SuccessfulRequests  = 0
            FailedRequests      = $results.Count
            SuccessRate         = 0
            AverageResponseTime = 0
            MinResponseTime     = 0
            MaxResponseTime     = 0
            MedianResponseTime  = 0
            P95ResponseTime     = 0
            P99ResponseTime     = 0
        }
    }
    
    $sortedTimes = $responseTimes | Sort-Object
    $p95Index = [Math]::Floor($sortedTimes.Count * 0.95)
    $p99Index = [Math]::Floor($sortedTimes.Count * 0.99)
    $medianIndex = [Math]::Floor($sortedTimes.Count * 0.5)
    
    return @{
        TotalRequests       = $results.Count
        SuccessfulRequests  = $successfulRequests.Count
        FailedRequests      = $results.Count - $successfulRequests.Count
        SuccessRate         = [Math]::Round(($successfulRequests.Count / $results.Count) * 100, 2)
        AverageResponseTime = [Math]::Round(($responseTimes | Measure-Object -Average).Average, 2)
        MinResponseTime     = ($responseTimes | Measure-Object -Minimum).Minimum
        MaxResponseTime     = ($responseTimes | Measure-Object -Maximum).Maximum
        MedianResponseTime  = $sortedTimes[$medianIndex]
        P95ResponseTime     = $sortedTimes[$p95Index]
        P99ResponseTime     = $sortedTimes[$p99Index]
    }
}

function Test-ApplicationEndpoints($baseUrl, $iterations) {
    $endpoints = @(
        @{ Name = "Home"; Url = "$baseUrl/" }
        @{ Name = "Health Check"; Url = "$baseUrl/health" }
        @{ Name = "API Info"; Url = "$baseUrl/api/monitoring/info" }
        @{ Name = "API Metrics"; Url = "$baseUrl/api/monitoring/metrics" }
        @{ Name = "Collections"; Url = "$baseUrl/explore/collections" }
    )
    
    $results = @{}
    
    foreach ($endpoint in $endpoints) {
        Write-InfoMessage "Testing $($endpoint.Name) endpoint..."
        Write-Host "Progress: " -NoNewline
        
        $endpointResults = Measure-EndpointPerformance $endpoint.Url $iterations
        $metrics = Get-PerformanceMetrics $endpointResults
        
        $results[$endpoint.Name] = @{
            Url        = $endpoint.Url
            Metrics    = $metrics
            RawResults = $endpointResults
        }
        
        Write-Host ""
        Write-SuccessMessage "✅ $($endpoint.Name): $($metrics.AverageResponseTime)ms avg, $($metrics.SuccessRate)% success"
    }
    
    return $results
}

function Generate-PerformanceReport($results, $outputFile) {
    $report = @{
        Timestamp     = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        Configuration = @{
            Iterations      = $Iterations
            ConcurrentUsers = $ConcurrentUsers
            BaseUrl         = $BaseUrl
        }
        Results       = $results
        Summary       = @{
            TotalEndpoints      = $results.Count
            OverallSuccessRate  = [Math]::Round((($results.Values.Metrics.SuccessfulRequests | Measure-Object -Sum).Sum / ($results.Values.Metrics.TotalRequests | Measure-Object -Sum).Sum) * 100, 2)
            AverageResponseTime = [Math]::Round(($results.Values.Metrics.AverageResponseTime | Measure-Object -Average).Average, 2)
            FastestEndpoint     = ($results.GetEnumerator() | Sort-Object { $_.Value.Metrics.AverageResponseTime } | Select-Object -First 1).Key
            SlowestEndpoint     = ($results.GetEnumerator() | Sort-Object { $_.Value.Metrics.AverageResponseTime } -Descending | Select-Object -First 1).Key
        }
    }
    
    $jsonReport = $report | ConvertTo-Json -Depth 10
    Set-Content -Path $outputFile -Value $jsonReport
    
    return $report
}

function Show-PerformanceReport($report) {
    Write-InfoMessage ""
    Write-InfoMessage "╔══════════════════════════════════════════════════════════════╗"
    Write-InfoMessage "║                    Performance Report                       ║"
    Write-InfoMessage "╚══════════════════════════════════════════════════════════════╝"
    Write-InfoMessage ""
    Write-InfoMessage "📊 Test Configuration:"
    Write-InfoMessage "   Base URL: $($report.Configuration.BaseUrl)"
    Write-InfoMessage "   Iterations: $($report.Configuration.Iterations)"
    Write-InfoMessage "   Timestamp: $($report.Timestamp)"
    Write-InfoMessage ""
    Write-InfoMessage "📈 Overall Results:"
    Write-InfoMessage "   Success Rate: $($report.Summary.OverallSuccessRate)%"
    Write-InfoMessage "   Average Response Time: $($report.Summary.AverageResponseTime)ms"
    Write-InfoMessage "   Fastest Endpoint: $($report.Summary.FastestEndpoint)"
    Write-InfoMessage "   Slowest Endpoint: $($report.Summary.SlowestEndpoint)"
    Write-InfoMessage ""
    Write-InfoMessage "🔍 Detailed Results:"
    
    foreach ($endpoint in $report.Results.GetEnumerator()) {
        $metrics = $endpoint.Value.Metrics
        Write-InfoMessage "   $($endpoint.Key):"
        Write-InfoMessage "     • Success Rate: $($metrics.SuccessRate)%"
        Write-InfoMessage "     • Avg Response: $($metrics.AverageResponseTime)ms"
        Write-InfoMessage "     • Min/Max: $($metrics.MinResponseTime)ms / $($metrics.MaxResponseTime)ms"
        Write-InfoMessage "     • P95/P99: $($metrics.P95ResponseTime)ms / $($metrics.P99ResponseTime)ms"
        Write-InfoMessage ""
    }
    
    Write-InfoMessage "📄 Full report saved to: $OutputFile"
}

function Test-ApplicationHealth($baseUrl) {
    Write-InfoMessage "🏥 Testing application health..."
    
    try {
        $healthResponse = Invoke-WebRequest -Uri "$baseUrl/health" -Method GET -UseBasicParsing -TimeoutSec 10
        $healthData = $healthResponse.Content | ConvertFrom-Json
        
        Write-SuccessMessage "✅ Application is healthy"
        Write-InfoMessage "   Status: $($healthData.status)"
        Write-InfoMessage "   Checks: $($healthData.checks.Count)"
        
        return $true
    }
    catch {
        Write-Host "❌ Application health check failed: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

function Main() {
    Write-InfoMessage "╔══════════════════════════════════════════════════════════════╗"
    Write-InfoMessage "║                Performance Benchmarking Tool                ║"
    Write-InfoMessage "╚══════════════════════════════════════════════════════════════╝"
    Write-InfoMessage ""
    
    # Test application health first
    if (-not (Test-ApplicationHealth $BaseUrl)) {
        Write-Host "❌ Cannot proceed with performance testing - application is not healthy" -ForegroundColor Red
        exit 1
    }
    
    Write-InfoMessage "🚀 Starting performance tests..."
    Write-InfoMessage "   Target: $BaseUrl"
    Write-InfoMessage "   Iterations: $Iterations"
    Write-InfoMessage "   Output: $OutputFile"
    Write-InfoMessage ""
    
    try {
        $startTime = Get-Date
        
        # Run performance tests
        $results = Test-ApplicationEndpoints $BaseUrl $Iterations
        
        # Generate report
        $report = Generate-PerformanceReport $results $OutputFile
        
        # Show results
        Show-PerformanceReport $report
        
        $duration = (Get-Date) - $startTime
        Write-SuccessMessage ""
        Write-SuccessMessage "🎉 Performance testing completed!"
        Write-SuccessMessage "⏱️ Total time: $($duration.TotalSeconds.ToString('F1')) seconds"
        
    }
    catch {
        Write-Host "❌ Performance testing failed: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
}

# Run main function
Main

# test-api.ps1
# Quick API testing script for CamBridge
# Tests the HTTP API endpoints when service is running
# Version: 0.7.17 (REMOVED non-existent /api/configuration)

param(
    [string]$BaseUrl = "http://localhost:5111",
    [switch]$Continuous = $false
)

# Ensure BaseUrl is set (fallback)
if ([string]::IsNullOrEmpty($BaseUrl)) {
    $BaseUrl = "http://localhost:5111"
    Write-Host "BaseUrl was empty, using default: $BaseUrl" -ForegroundColor Yellow
}

$ErrorActionPreference = "Stop"

function Test-Endpoint {
    param(
        [string]$Endpoint,
        [string]$Method = "GET",
        [object]$Body = $null
    )
    
    try {
        $uri = "$BaseUrl$Endpoint"
        Write-Host "Testing: $Method $uri" -ForegroundColor Cyan
        
        $params = @{
            Uri = $uri
            Method = $Method
            TimeoutSec = 5
        }
        
        if ($Body) {
            $params.Body = ($Body | ConvertTo-Json -Depth 10)
            $params.ContentType = "application/json"
        }
        
        $response = Invoke-RestMethod @params
        
        Write-Host "  [OK] Success" -ForegroundColor Green
        return $response
    }
    catch {
        Write-Host "  [FAIL] Failed: $_" -ForegroundColor Red
        return $null
    }
}

function Show-ServiceStatus {
    param($status)
    
    if ($status) {
        Write-Host ""
        Write-Host "Service Information:" -ForegroundColor Yellow
        Write-Host "  Version:         $($status.version)" -ForegroundColor Gray
        Write-Host "  Uptime:          $($status.uptime)" -ForegroundColor Gray
        Write-Host "  Total Processed: $($status.totalSuccessful)" -ForegroundColor Gray
        Write-Host "  Active Queue:    $($status.activeProcessing)" -ForegroundColor Gray
        Write-Host "  Pipeline Count:  $($status.pipelineCount)" -ForegroundColor Gray
        
        if ($status.pipelines -and $status.pipelines.Count -gt 0) {
            Write-Host ""
            Write-Host "Active Pipelines:" -ForegroundColor Yellow
            foreach ($pipeline in $status.pipelines) {
                Write-Host "  - $($pipeline.name) [Active: $($pipeline.isActive)]" -ForegroundColor Gray
                Write-Host "    Processed: $($pipeline.totalProcessed), Success: $($pipeline.totalSuccessful), Failed: $($pipeline.totalFailed)" -ForegroundColor DarkGray
                if ($pipeline.watchedFolders) {
                    Write-Host "    Watching: $($pipeline.watchedFolders -join ', ')" -ForegroundColor DarkGray
                }
            }
        } else {
            Write-Host "  No pipelines configured" -ForegroundColor DarkYellow
        }
    }
}

# Main test sequence
Clear-Host
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "    CamBridge API Test Suite v0.7.17" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Base URL: $BaseUrl" -ForegroundColor Gray
Write-Host ""

# Quick connectivity test first
Write-Host "Pre-flight check..." -ForegroundColor Yellow
try {
    $testConnection = Test-NetConnection -ComputerName "localhost" -Port 5111 -WarningAction SilentlyContinue
    if ($testConnection.TcpTestSucceeded) {
        Write-Host "  Port 5111 is reachable" -ForegroundColor Green
    } else {
        Write-Host "  Port 5111 is NOT reachable!" -ForegroundColor Red
        Write-Host "  Is the CamBridge service running?" -ForegroundColor Yellow
    }
} catch {
    Write-Host "  Could not test port connectivity" -ForegroundColor Yellow
}
Write-Host ""

do {
    $testTime = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Write-Host "Test run at: $testTime" -ForegroundColor DarkGray
    Write-Host ""
    
    # Test 1: Health Check
    $health = Test-Endpoint -Endpoint "/health"
    if ($health) {
        Write-Host "  Status: $($health.status)" -ForegroundColor $(if ($health.status -eq "Healthy") { "Green" } else { "Yellow" })
        Write-Host ""
    }
    
    # Test 2: Service Status
    $status = Test-Endpoint -Endpoint "/api/status"
    Show-ServiceStatus $status
    
    # Test 3: Pipeline Status
    Write-Host ""
    $pipelines = Test-Endpoint -Endpoint "/api/pipelines"
    if ($pipelines) {
        $pipelineCount = ($pipelines.PSObject.Properties | Measure-Object).Count
        Write-Host "  Found $pipelineCount pipeline(s)" -ForegroundColor Gray
    }
    
    # Test 4: Version endpoint (NEW in v0.7.17)
    Write-Host ""
    $version = Test-Endpoint -Endpoint "/api/status/version"
    if ($version) {
        Write-Host "  API Version: $($version.version)" -ForegroundColor Gray
        Write-Host "  Company: $($version.company)" -ForegroundColor Gray
    }
    
    # Test 5: Health Status endpoint (NEW in v0.7.17)
    Write-Host ""
    $healthStatus = Test-Endpoint -Endpoint "/api/status/health"
    if ($healthStatus) {
        Write-Host "  Health: $($healthStatus.status) - $($healthStatus.details)" -ForegroundColor Gray
        Write-Host "  Active Pipelines: $($healthStatus.activePipelines)/$($healthStatus.totalPipelines)" -ForegroundColor Gray
    }
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    
    if ($Continuous) {
        Write-Host "Waiting 5 seconds... (Press Ctrl+C to stop)" -ForegroundColor Gray
        Start-Sleep -Seconds 5
        Clear-Host
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host "    CamBridge API Test Suite v0.7.17" -ForegroundColor Cyan
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Base URL: $BaseUrl" -ForegroundColor Gray
        Write-Host ""
    } else {
        Write-Host ""
        Write-Host "Run with -Continuous flag for continuous monitoring" -ForegroundColor DarkGray
        Write-Host ""
    }
    
} while ($Continuous)

# Summary
if (-not $Continuous) {
    Write-Host ""
    Write-Host "Available Endpoints:" -ForegroundColor Yellow
    Write-Host "  /health                - Basic health check" -ForegroundColor Gray
    Write-Host "  /api/status            - Full service status" -ForegroundColor Gray
    Write-Host "  /api/status/version    - Version info only" -ForegroundColor Gray
    Write-Host "  /api/status/health     - Health with pipeline info" -ForegroundColor Gray
    Write-Host "  /api/pipelines         - All pipeline statuses" -ForegroundColor Gray
    Write-Host "  /api/pipelines/{id}    - Specific pipeline details" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "  1. Check service status: Get-Service CamBridgeService" -ForegroundColor Gray
    Write-Host "  2. Test port manually: Test-NetConnection localhost -Port 5111" -ForegroundColor Gray
    Write-Host "  3. Check service logs: 6[TAB] (or .\6-logs.ps1)" -ForegroundColor Gray
    Write-Host "  4. Run in console mode: 4[TAB] (or .\4-console.ps1)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Quick Commands:" -ForegroundColor Yellow
    Write-Host '  $status = Invoke-RestMethod "http://localhost:5111/api/status"' -ForegroundColor Gray
    Write-Host '  $status | ConvertTo-Json -Depth 5' -ForegroundColor Gray
    Write-Host ""
    Write-Host "For continuous monitoring:" -ForegroundColor Yellow
    Write-Host "  .\test-api.ps1 -Continuous" -ForegroundColor Gray
    Write-Host ""
}
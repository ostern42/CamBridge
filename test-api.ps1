# test-api.ps1
# Quick API testing script for CamBridge
# Tests the HTTP API endpoints when service is running
# Version: 0.7.5 (PARAMETER BUG FIXED)

param(
    [string]$BaseUrl = "http://localhost:5050",
    [switch]$Continuous = $false
)

# Ensure BaseUrl is set (fallback)
if ([string]::IsNullOrEmpty($BaseUrl)) {
    $BaseUrl = "http://localhost:5050"
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
        Write-Host "  Total Processed: $($status.totalProcessed)" -ForegroundColor Gray
        Write-Host "  Active Queue:    $($status.activeProcessing)" -ForegroundColor Gray
        
        if ($status.pipelines -and $status.pipelines.Count -gt 0) {
            Write-Host ""
            Write-Host "Active Pipelines:" -ForegroundColor Yellow
            foreach ($pipeline in $status.pipelines) {
                Write-Host "  - $($pipeline.name) [$($pipeline.status)]" -ForegroundColor Gray
                Write-Host "    Processed: $($pipeline.processedCount), Errors: $($pipeline.errorCount)" -ForegroundColor DarkGray
            }
        } else {
            Write-Host "  No pipelines configured" -ForegroundColor DarkYellow
        }
    }
}

# Main test sequence
Clear-Host
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "    CamBridge API Test Suite" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Base URL: $BaseUrl" -ForegroundColor Gray
Write-Host ""

# Quick connectivity test first
Write-Host "Pre-flight check..." -ForegroundColor Yellow
try {
    $testConnection = Test-NetConnection -ComputerName "localhost" -Port 5050 -WarningAction SilentlyContinue
    if ($testConnection.TcpTestSucceeded) {
        Write-Host "  Port 5050 is reachable" -ForegroundColor Green
    } else {
        Write-Host "  Port 5050 is NOT reachable!" -ForegroundColor Red
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
        Write-Host "  Found $($pipelines.Count) pipeline(s)" -ForegroundColor Gray
    }
    
    # Test 4: Configuration
    Write-Host ""
    $config = Test-Endpoint -Endpoint "/api/configuration"
    if ($config) {
        Write-Host "  Configuration loaded successfully" -ForegroundColor Gray
    }
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    
    if ($Continuous) {
        Write-Host "Waiting 5 seconds... (Press Ctrl+C to stop)" -ForegroundColor Gray
        Start-Sleep -Seconds 5
        Clear-Host
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host "    CamBridge API Test Suite" -ForegroundColor Cyan
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
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "  1. Check service status: Get-Service CamBridgeService" -ForegroundColor Gray
    Write-Host "  2. Test port manually: Test-NetConnection localhost -Port 5050" -ForegroundColor Gray
    Write-Host "  3. Check service logs: 6[TAB] (or .\6-logs.ps1)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Quick Commands:" -ForegroundColor Yellow
    Write-Host '  $status = Invoke-RestMethod "http://localhost:5050/api/status"' -ForegroundColor Gray
    Write-Host '  $status | ConvertTo-Json -Depth 5' -ForegroundColor Gray
    Write-Host ""
    Write-Host "For continuous monitoring:" -ForegroundColor Yellow
    Write-Host "  .\test-api.ps1 -Continuous" -ForegroundColor Gray
    Write-Host ""
}
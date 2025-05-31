# CamBridge Test Runner
# © 2025 Claude's Improbably Reliable Software Solutions

Write-Host "CamBridge Test Suite v0.3.2" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Check if we're in the solution root
if (-not (Test-Path "CamBridge.sln")) {
    Write-Host "ERROR: Please run this script from the solution root directory" -ForegroundColor Red
    exit 1
}

# Clean previous test results
Write-Host "`nCleaning previous test results..." -ForegroundColor Yellow
Remove-Item -Path "TestResults" -Recurse -Force -ErrorAction SilentlyContinue

# Build the solution
Write-Host "`nBuilding solution..." -ForegroundColor Yellow
dotnet build --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Build succeeded!" -ForegroundColor Green

# Run tests with coverage
Write-Host "`nRunning tests with coverage..." -ForegroundColor Yellow
dotnet test `
    --configuration Release `
    --logger "console;verbosity=normal" `
    --logger "html;logfilename=testresults.html" `
    --logger "trx;logfilename=testresults.trx" `
    --collect:"XPlat Code Coverage" `
    --results-directory "./TestResults" `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat=cobertura `
    /p:CoverletOutput=./TestResults/coverage.xml

$testExitCode = $LASTEXITCODE

# Generate coverage report
if (Get-Command "reportgenerator" -ErrorAction SilentlyContinue) {
    Write-Host "`nGenerating coverage report..." -ForegroundColor Yellow
    reportgenerator `
        -reports:"TestResults/**/coverage.cobertura.xml" `
        -targetdir:"TestResults/CoverageReport" `
        -reporttypes:"Html;Badges;TextSummary"
    
    Write-Host "Coverage report generated at: TestResults/CoverageReport/index.html" -ForegroundColor Green
} else {
    Write-Host "`nNote: Install ReportGenerator for detailed coverage reports:" -ForegroundColor Yellow
    Write-Host "  dotnet tool install -g dotnet-reportgenerator-globaltool" -ForegroundColor Gray
}

# Display test summary
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Test Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if ($testExitCode -eq 0) {
    Write-Host "All tests passed! ✓" -ForegroundColor Green
    
    # Try to parse and display test counts
    $trxFile = Get-ChildItem -Path "TestResults" -Filter "*.trx" -Recurse | Select-Object -First 1
    if ($trxFile) {
        [xml]$trx = Get-Content $trxFile.FullName
        $counters = $trx.TestRun.ResultSummary.Counters
        
        Write-Host "`nTest Statistics:" -ForegroundColor Cyan
        Write-Host "  Total:    $($counters.total)" -ForegroundColor Gray
        Write-Host "  Passed:   $($counters.passed)" -ForegroundColor Green
        Write-Host "  Failed:   $($counters.failed)" -ForegroundColor Red
        Write-Host "  Skipped:  $($counters.notExecuted)" -ForegroundColor Yellow
        
        if ($counters.passed -eq $counters.total) {
            Write-Host "`n🎉 Perfect score! All $($counters.total) tests passed!" -ForegroundColor Green
        }
    }
} else {
    Write-Host "Some tests failed! ✗" -ForegroundColor Red
    Write-Host "Check TestResults folder for detailed reports" -ForegroundColor Yellow
}

# Create test summary file
$summaryPath = "TestResults/test-summary.txt"
@"
CamBridge Test Results Summary
Generated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
======================================

Build Configuration: Release
Test Framework: xUnit
Coverage Tool: Coverlet

Results:
- HTML Report: TestResults/testresults.html
- TRX Report: TestResults/testresults.trx
- Coverage Report: TestResults/CoverageReport/index.html

Exit Code: $testExitCode
"@ | Out-File -FilePath $summaryPath -Encoding UTF8

Write-Host "`nTest summary saved to: $summaryPath" -ForegroundColor Gray

# Open results in browser if successful
if ($testExitCode -eq 0 -and (Test-Path "TestResults/CoverageReport/index.html")) {
    $openReport = Read-Host "`nOpen coverage report in browser? (Y/N)"
    if ($openReport -eq 'Y') {
        Start-Process "TestResults/CoverageReport/index.html"
    }
}

exit $testExitCode
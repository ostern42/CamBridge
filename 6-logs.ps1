# 6-logs.ps1
# Quick log viewer
$logPaths = @(
    "C:\ProgramData\CamBridge\Logs",
    ".\Deploy\CamBridge-Deploy-v*\Service\Logs"
)

foreach ($path in $logPaths) {
    $resolved = Resolve-Path $path -ErrorAction SilentlyContinue
    if ($resolved) {
        Write-Host "Opening logs: $resolved" -ForegroundColor Cyan
        Start-Process explorer.exe $resolved
        exit
    }
}

Write-Host "ERROR: No log folder found!" -ForegroundColor Red

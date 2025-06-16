# 0-build.ps1
# Smart build - detects source changes automatically!
param([switch]$Quick, [switch]$Force)

# Get current version from Version.props
$version = "?.?.?"
if (Test-Path "Version.props") {
    $content = Get-Content "Version.props" -Raw
    if ($content -match '<VersionPrefix>([^<]+)</VersionPrefix>') {
        $version = $matches[1]
    }
}

$deployPath = ".\Deploy\CamBridge-Deploy-v$version"

# Smart change detection!
$needsBuild = $false
$reason = ""

if (-not (Test-Path $deployPath)) {
    $needsBuild = $true
    $reason = "No deployment exists"
} elseif ($Force) {
    $needsBuild = $true
    $reason = "Force rebuild requested"
} else {
    # Check if any source file is newer than the deployment
    $deployTime = (Get-Item $deployPath).LastWriteTime
    
    # Check all source files
    $newerFiles = Get-ChildItem -Path "src" -Include "*.cs","*.csproj","*.xaml","*.json" -Recurse | 
                  Where-Object { $_.LastWriteTime -gt $deployTime } |
                  Select-Object -First 5  # Just show first 5
    
    if ($newerFiles) {
        $needsBuild = $true
        $reason = "Source files changed"
        Write-Host "Changed files detected:" -ForegroundColor Yellow
        $newerFiles | ForEach-Object {
            Write-Host "  $($_.FullName.Replace($pwd, '.'))" -ForegroundColor DarkYellow
        }
        if ((Get-ChildItem -Path "src" -Include "*.cs","*.csproj","*.xaml","*.json" -Recurse | 
             Where-Object { $_.LastWriteTime -gt $deployTime }).Count -gt 5) {
            Write-Host "  ... and more" -ForegroundColor DarkYellow
        }
    }
    
    # Also check Version.props
    if ((Get-Item "Version.props").LastWriteTime -gt $deployTime) {
        $needsBuild = $true
        $reason = "Version changed"
    }
}

if (-not $needsBuild) {
    Write-Host "Build up-to-date: CamBridge-Deploy-v$version" -ForegroundColor Green
    Write-Host "Last built: $(Get-Item $deployPath | Select-Object -ExpandProperty LastWriteTime)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "No source changes detected. Use -Force to rebuild anyway" -ForegroundColor DarkGray
    return
}

# We need to build!
Write-Host "Building CamBridge Release (No ZIP, No QRBridge)..." -ForegroundColor Cyan
Write-Host "Reason: $reason" -ForegroundColor Yellow
Write-Host ""

if ($Quick) {
    & .\Create-DeploymentPackage.ps1 -SkipClean -SkipZip -SkipQRBridge
} else {
    & .\Create-DeploymentPackage.ps1 -SkipZip -SkipQRBridge
}
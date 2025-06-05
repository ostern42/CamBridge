# Build-QRBridgeUltraSlim.ps1
# Builds the ultra-slim single-file QRBridge
# Result: ~500KB standalone exe (vs 120KB for old .NET Framework version)

$ErrorActionPreference = "Stop"

Write-Host "Building QRBridge 2.0 Ultra Slim..." -ForegroundColor Cyan
Write-Host ""

# Create temp build directory
$buildDir = ".\TempBuild"
if (Test-Path $buildDir) {
    Remove-Item $buildDir -Recurse -Force
}
New-Item $buildDir -ItemType Directory | Out-Null

try {
    # Copy the single source file
    Copy-Item ".\QRBridgeUltraSlim.cs" -Destination $buildDir

    # Download QRCoder from NuGet
    Write-Host "Downloading QRCoder..." -ForegroundColor Yellow
    Push-Location $buildDir
    
    # Create a minimal project just to get the DLL
    $tempProject = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="QRCoder" Version="1.4.3" />
    <PackageReference Include="System.Drawing.Common" Version="8.0.0" />
  </ItemGroup>
</Project>
"@
    $tempProject | Set-Content "temp.csproj"
    
    # Restore to get the DLLs
    & dotnet restore temp.csproj
    & dotnet build temp.csproj -c Release
    
    # Find the QRCoder.dll
    $qrCoderDll = Get-ChildItem -Path "." -Filter "QRCoder.dll" -Recurse | Select -First 1
    
    if ($qrCoderDll) {
        Write-Host "Found QRCoder at: $($qrCoderDll.FullName)" -ForegroundColor Gray
        
        # Use C# compiler directly
        Write-Host "Compiling with csc..." -ForegroundColor Yellow
        
        # Find csc.exe
        $csc = "${env:ProgramFiles}\Microsoft Visual Studio\2022\*\MSBuild\Current\Bin\Roslyn\csc.exe"
        $cscPath = Get-ChildItem $csc -ErrorAction SilentlyContinue | Select -First 1
        
        if (-not $cscPath) {
            # Try .NET SDK path
            $cscPath = "${env:ProgramFiles}\dotnet\sdk\*\Roslyn\bincore\csc.dll"
            $cscPath = Get-ChildItem $cscPath -ErrorAction SilentlyContinue | Select -First 1
            
            if ($cscPath) {
                # Use dotnet to run csc.dll
                $compileArgs = @(
                    $cscPath.FullName,
                    "QRBridgeUltraSlim.cs",
                    "/target:winexe",
                    "/out:QRBridge.exe",
                    "/reference:$($qrCoderDll.FullName)",
                    "/reference:System.Drawing.Common.dll",
                    "/optimize+",
                    "/platform:x64"
                )
                & dotnet $compileArgs
            }
        } else {
            # Use csc.exe directly
            & $cscPath.FullName QRBridgeUltraSlim.cs /target:winexe /out:QRBridge.exe /reference:$($qrCoderDll.FullName) /optimize+ /platform:x64
        }
        
        if (Test-Path "QRBridge.exe") {
            $size = (Get-Item "QRBridge.exe").Length
            Write-Host ""
            Write-Host "Build successful!" -ForegroundColor Green
            Write-Host "Size: $([math]::Round($size/1KB)) KB" -ForegroundColor Cyan
            
            # Copy result
            Copy-Item "QRBridge.exe" -Destination "..\QRBridge-UltraSlim.exe"
            Copy-Item $qrCoderDll.FullName -Destination "..\QRCoder.dll"
            
            Write-Host ""
            Write-Host "Files created:" -ForegroundColor Yellow
            Write-Host "  QRBridge-UltraSlim.exe" -ForegroundColor Gray
            Write-Host "  QRCoder.dll (required)" -ForegroundColor Gray
            Write-Host ""
            Write-Host "Note: Both files must be in the same directory!" -ForegroundColor Yellow
        }
    }
}
finally {
    Pop-Location
    Remove-Item $buildDir -Recurse -Force -ErrorAction SilentlyContinue
}

# Alternative: Use ILMerge or similar to merge DLLs
Write-Host ""
Write-Host "For a true single-file solution, consider:" -ForegroundColor Cyan
Write-Host "  1. Using .NET Framework 4.8 (smaller runtime)" -ForegroundColor Gray
Write-Host "  2. Using ILMerge to merge QRCoder.dll" -ForegroundColor Gray
Write-Host "  3. Using Costura.Fody to embed dependencies" -ForegroundColor Gray
Write-Host ""
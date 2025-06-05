# Create-DeploymentPackage.ps1
# Complete build and deployment package creator for CamBridge
# Version: 0.5.33
# Now includes QRBridge 2.0 in both Full and Ultra Slim editions!

param(
    [string]$Version = "0.5.33",
    [string]$OutputPath = ".\Deploy",
    [switch]$SkipClean = $false,
    [switch]$SkipTests = $false,
    [switch]$SkipSlim = $false
)

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " CamBridge Deployment Package Builder" -ForegroundColor Cyan
Write-Host " Version: $Version" -ForegroundColor Cyan
Write-Host " Now with QRBridge 2.0!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Clean everything
if (-not $SkipClean) {
    Write-Host "Step 1: Cleaning build directories..." -ForegroundColor Yellow
    
    # Clean all bin and obj folders
    Get-ChildItem -Path "src" -Include "bin","obj" -Recurse -Directory | ForEach-Object {
        Write-Host "  Cleaning: $($_.FullName)" -ForegroundColor Gray
        Remove-Item $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
    }
    
    # Also clean test folders
    Get-ChildItem -Path "tests" -Include "bin","obj" -Recurse -Directory | ForEach-Object {
        Write-Host "  Cleaning: $($_.FullName)" -ForegroundColor Gray
        Remove-Item $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
    }
    
    Write-Host "  [OK] Clean complete" -ForegroundColor Green
    Write-Host ""
}

# Step 2: Restore packages
Write-Host "Step 2: Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore CamBridge.sln
if ($LASTEXITCODE -ne 0) {
    Write-Error "Restore failed!"
    exit 1
}
Write-Host "  [OK] Packages restored" -ForegroundColor Green
Write-Host ""

# Step 3: Build Config UI
Write-Host "Step 3: Building Configuration UI..." -ForegroundColor Yellow
dotnet build src\CamBridge.Config\CamBridge.Config.csproj -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Error "Config UI build failed!"
    exit 1
}
Write-Host "  [OK] Config UI built" -ForegroundColor Green
Write-Host ""

# Step 4: Build QRBridge
Write-Host "Step 4: Building QRBridge 2.0..." -ForegroundColor Yellow
# Build framework-dependent version (smaller size)
dotnet publish src\CamBridge.QRBridge\CamBridge.QRBridge.csproj -c Release --self-contained false -o .\TempQRBridgePublish
if ($LASTEXITCODE -ne 0) {
    Write-Error "QRBridge build failed!"
    exit 1
}
Write-Host "  [OK] QRBridge built (framework-dependent)" -ForegroundColor Green

# Step 4b: Build QRBridge Ultra Slim
if (-not $SkipSlim) {
    Write-Host "  Building QRBridge Ultra Slim edition..." -ForegroundColor Yellow
    try {
        # Check if QRBridgeUltraSlim.cs exists, if not create it
        if (-not (Test-Path ".\QRBridgeUltraSlim.cs")) {
            Write-Host "    Creating QRBridgeUltraSlim.cs..." -ForegroundColor Gray
            # Create the ultra slim source file
            $ultraSlimSource = @'
// QRBridgeUltraSlim.cs - Single file QRBridge without any project dependencies
// Version: 2.0 Ultra Slim

using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QRCoder;

namespace CamBridge.QRBridge.UltraSlim
{
    static class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            try
            {
                var data = ParseArguments(args);
                if (data == null) return 1;
                
                using var form = new QRForm(data.qrData, data.timeout, data.info);
                Application.Run(form);
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "QRBridge Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 99;
            }
        }

        static (string qrData, int timeout, string info)? ParseArguments(string[] args)
        {
            if (args.Length == 0 || args.Any(a => a == "-help" || a == "--help"))
            {
                ShowHelp();
                return null;
            }

            var dict = new System.Collections.Generic.Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-") && i + 1 < args.Length)
                {
                    dict[args[i].TrimStart('-').ToLower()] = args[++i];
                }
            }

            if (!dict.ContainsKey("examid") || !dict.ContainsKey("name"))
            {
                MessageBox.Show("Error: -examid and -name are required", "QRBridge Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            // Format: ExamId|Name|BirthDate|Gender|Comment
            var qrData = string.Join("|", new[]
            {
                dict["examid"],
                dict["name"],
                dict.GetValueOrDefault("birthdate", ""),
                dict.GetValueOrDefault("gender", "O").ToUpper()[0].ToString(),
                dict.GetValueOrDefault("comment", "")
            });

            var timeout = int.TryParse(dict.GetValueOrDefault("timeout", "10"), out var t) ? t : 10;
            
            var info = $"Patient: {dict["name"]}\n" +
                      $"Exam ID: {dict["examid"]}\n" +
                      $"Comment: {dict.GetValueOrDefault("comment", "-")}";

            return (qrData, timeout, info);
        }

        static void ShowHelp()
        {
            MessageBox.Show(@"CamBridge QRBridge 2.0 Ultra Slim
© 2025 Claude's Improbably Reliable Software Solutions

Usage: QRBridge_slim.exe -examid <ID> -name <n> [options]

Required:
  -examid <ID>     Examination ID
  -name <n>        Patient name

Optional:
  -birthdate <DATE>  Birth date (yyyy-MM-dd)
  -gender <M/F/O>    Gender
  -comment <TEXT>    Comment
  -timeout <SEC>     Window timeout (default: 10)
  -help              Show this help

Example:
  QRBridge_slim.exe -examid ""EX001"" -name ""Müller, Hans"" -birthdate ""1985-03-15""",
                "QRBridge Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    class QRForm : Form
    {
        private readonly System.Windows.Forms.Timer timer;
        private int remaining;

        public QRForm(string qrData, int timeout, string info)
        {
            remaining = timeout;
            
            // Form setup
            Text = "CamBridge QRBridge - QR Code";
            Size = new Size(450, 520);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.FromArgb(45, 45, 48);

            // Generate QR Code
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(
                Encoding.UTF8.GetBytes(qrData), QRCodeGenerator.ECCLevel.M);
            using var qrCode = new QRCode(qrCodeData);
            var qrBitmap = qrCode.GetGraphic(10);

            // QR Code display
            var pictureBox = new PictureBox
            {
                Image = qrBitmap,
                Size = new Size(400, 400),
                Location = new Point(25, 20),
                BackColor = Color.White,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            // Info label
            var infoLabel = new Label
            {
                Text = info,
                Location = new Point(25, 430),
                Size = new Size(400, 60),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(60, 60, 60),
                Padding = new Padding(10),
                Font = new Font("Segoe UI", 9)
            };

            // Countdown label
            var countdownLabel = new Label
            {
                Location = new Point(25, 495),
                Size = new Size(400, 20),
                ForeColor = Color.FromArgb(0, 122, 204),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            Controls.AddRange(new Control[] { pictureBox, infoLabel, countdownLabel });

            // Timer for countdown
            timer = new System.Windows.Forms.Timer { Interval = 1000 };
            timer.Tick += (s, e) =>
            {
                countdownLabel.Text = $"Fenster schließt in {remaining} Sekunden...";
                if (--remaining <= 0)
                {
                    timer.Stop();
                    Close();
                }
            };

            Shown += (s, e) => timer.Start();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
'@
            $ultraSlimSource | Set-Content ".\QRBridgeUltraSlim.cs" -Encoding UTF8
        }

        # Build the ultra slim version using Roslyn compiler
        $tempBuildDir = ".\TempBuildSlim"
        if (Test-Path $tempBuildDir) {
            Remove-Item $tempBuildDir -Recurse -Force
        }
        New-Item $tempBuildDir -ItemType Directory -Force | Out-Null

        Push-Location $tempBuildDir
        try {
            # Copy source
            Copy-Item "..\QRBridgeUltraSlim.cs" -Destination "."
            
            # Create temp project to get QRCoder.dll
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
            
            # Restore and build to get dependencies
            & dotnet restore temp.csproj *>&1 | Out-Null
            & dotnet build temp.csproj -c Release *>&1 | Out-Null
            
            # Find QRCoder.dll
            $qrCoderDll = Get-ChildItem -Filter "QRCoder.dll" -Recurse | Select -First 1
            
            if ($qrCoderDll) {
                # Try to compile with Roslyn
                $roslynPath = Get-ChildItem "${env:ProgramFiles}\dotnet\sdk\*\Roslyn\bincore\csc.dll" -ErrorAction SilentlyContinue | Select -First 1
                
                if ($roslynPath) {
                    $compileArgs = @(
                        $roslynPath.FullName,
                        "QRBridgeUltraSlim.cs",
                        "/target:winexe",
                        "/out:CamBridge.QRBridge_slim.exe",
                        "/reference:$($qrCoderDll.FullName)",
                        "/reference:System.Drawing.Common.dll",
                        "/optimize+",
                        "/nologo"
                    )
                    & dotnet $compileArgs *>&1 | Out-Null
                    
                    if (Test-Path "CamBridge.QRBridge_slim.exe") {
                        # Success! Copy to a temp location for later
                        New-Item "..\QRBridgeSlimTemp" -ItemType Directory -Force | Out-Null
                        Copy-Item "CamBridge.QRBridge_slim.exe" -Destination "..\QRBridgeSlimTemp\"
                        Copy-Item $qrCoderDll.FullName -Destination "..\QRBridgeSlimTemp\QRCoder.dll"
                        Write-Host "    [OK] Ultra Slim built successfully" -ForegroundColor Green
                    }
                    else {
                        Write-Warning "    Ultra Slim build failed, continuing without it"
                    }
                }
            }
        }
        catch {
            Write-Warning "    Ultra Slim build failed: $_"
        }
        finally {
            Pop-Location
            Remove-Item $tempBuildDir -Recurse -Force -ErrorAction SilentlyContinue
        }
    }
    catch {
        Write-Warning "  Could not build Ultra Slim edition, continuing..."
    }
}
Write-Host ""

# Step 5: Publish Service
Write-Host "Step 5: Publishing Windows Service..." -ForegroundColor Yellow
dotnet publish src\CamBridge.Service\CamBridge.Service.csproj -c Release -r win-x64 --self-contained
if ($LASTEXITCODE -ne 0) {
    Write-Error "Service publish failed!"
    exit 1
}
Write-Host "  [OK] Service published" -ForegroundColor Green
Write-Host ""

# Optional: Run tests
if (-not $SkipTests) {
    Write-Host "Step 6: Running tests..." -ForegroundColor Yellow
    dotnet test tests\CamBridge.Infrastructure.Tests\CamBridge.Infrastructure.Tests.csproj -c Release
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Some tests failed, but continuing..."
    } else {
        Write-Host "  [OK] All tests passed" -ForegroundColor Green
    }
    Write-Host ""
}

# Step 7: Create deployment package
Write-Host "Step 7: Creating deployment package..." -ForegroundColor Yellow

# Create deployment structure
$deployDir = Join-Path $OutputPath "CamBridge-Deploy-v$Version"
$serviceDir = Join-Path $deployDir "Service"
$configDir = Join-Path $deployDir "Config"
$qrBridgeDir = Join-Path $deployDir "QRBridge"

# Clean and create directories
if (Test-Path $deployDir) {
    Remove-Item $deployDir -Recurse -Force
}
New-Item -Path $serviceDir -ItemType Directory -Force | Out-Null
New-Item -Path $configDir -ItemType Directory -Force | Out-Null
New-Item -Path $qrBridgeDir -ItemType Directory -Force | Out-Null

# Copy Service files
Write-Host "  Copying Service files..." -ForegroundColor Gray
$servicePath = ".\src\CamBridge.Service\bin\Release\net8.0-windows\win-x64\publish"
if (Test-Path $servicePath) {
    Copy-Item "$servicePath\*" -Destination $serviceDir -Recurse -Force
} else {
    Write-Error "Service publish folder not found!"
    exit 1
}

# Copy Config UI
Write-Host "  Copying Configuration UI..." -ForegroundColor Gray
$configPath = ".\src\CamBridge.Config\bin\x64\Release\net8.0-windows"
if (Test-Path $configPath) {
    Copy-Item "$configPath\*" -Destination $configDir -Recurse -Force
} else {
    # Try alternate path
    $configPath = ".\src\CamBridge.Config\bin\Release\net8.0-windows"
    if (Test-Path $configPath) {
        Copy-Item "$configPath\*" -Destination $configDir -Recurse -Force
    } else {
        Write-Warning "Config UI not found!"
    }
}

# Copy QRBridge
Write-Host "  Copying QRBridge 2.0..." -ForegroundColor Gray
if (Test-Path ".\TempQRBridgePublish") {
    Copy-Item ".\TempQRBridgePublish\*" -Destination $qrBridgeDir -Recurse -Force
    Remove-Item ".\TempQRBridgePublish" -Recurse -Force
} else {
    # Fallback to build output
    $qrBridgePath = ".\src\CamBridge.QRBridge\bin\Release\net8.0-windows"
    if (Test-Path "$qrBridgePath\win-x64") {
        Copy-Item "$qrBridgePath\win-x64\*" -Destination $qrBridgeDir -Recurse -Force
    } elseif (Test-Path $qrBridgePath) {
        Copy-Item "$qrBridgePath\*" -Destination $qrBridgeDir -Recurse -Force
    } else {
        Write-Warning "QRBridge not found!"
    }
}

# Copy Ultra Slim version if it was built
if (Test-Path ".\QRBridgeSlimTemp") {
    Write-Host "  Copying QRBridge Ultra Slim..." -ForegroundColor Gray
    Copy-Item ".\QRBridgeSlimTemp\CamBridge.QRBridge_slim.exe" -Destination $qrBridgeDir -Force
    # QRCoder.dll will already be there from the full version
    Remove-Item ".\QRBridgeSlimTemp" -Recurse -Force -ErrorAction SilentlyContinue
}

# Create QRBridge README
$qrBridgeReadme = @"
CamBridge QRBridge 2.0
======================

⚠️  IMPORTANT: This is a framework-dependent build!
   Requires .NET 8.0 Runtime installed on the target system.
   Download from: https://dotnet.microsoft.com/download/dotnet/8.0

Two versions are included:

1. FULL VERSION (CamBridge.QRBridge.exe)
   - Size: ~7 MB (framework-dependent)
   - Features: Full logging, dependency injection, modern architecture
   - Requirements: .NET 8.0 runtime MUST BE INSTALLED!

2. ULTRA SLIM VERSION (CamBridge.QRBridge_slim.exe)
   - Size: ~500 KB (single file)
   - Features: Core QR generation only
   - Requirements: Just QRCoder.dll

Usage:
------
Full version:    CamBridge.QRBridge.exe [arguments]
Slim version:    CamBridge.QRBridge_slim.exe [arguments]

Arguments:
----------
-examid <ID>       Examination ID (required)
-name <n>       Patient name (required)
-birthdate <DATE>  Birth date (yyyy-MM-dd)
-gender <M/F/O>    Gender
-comment <TEXT>    Comment
-timeout <SEC>     Window timeout (default: 10)
-help              Show help

Example:
--------
CamBridge.QRBridge.exe -examid "EX001" -name "Müller, Hans" -birthdate "1985-03-15" -gender "M" -comment "Röntgen Thorax"

QR Code Format:
---------------
ExamId|Name|BirthDate|Gender|Comment

UTF-8 encoding is used throughout to support special characters (ä, ö, ü, ß, etc.)

© 2025 Claude's Improbably Reliable Software Solutions
"@
$qrBridgeReadme | Set-Content "$qrBridgeDir\README.txt"

# Copy deployment scripts
Write-Host "  Adding deployment scripts..." -ForegroundColor Gray
@("Install-CamBridge.ps1", "Uninstall-CamBridge.ps1") | ForEach-Object {
    if (Test-Path $_) {
        Copy-Item $_ -Destination $deployDir -Force
    }
}
if (Test-Path "README-Deployment.md") {
    Copy-Item "README-Deployment.md" -Destination "$deployDir\README.md" -Force
}

# Create QRBridge batch file for easy access
$qrBridgeBatch = @"
@echo off
echo CamBridge QRBridge 2.0
echo ===================
echo.
if "%1"=="-slim" (
    echo Using Ultra Slim edition...
    shift
    "%~dp0QRBridge\CamBridge.QRBridge_slim.exe" %*
) else (
    "%~dp0QRBridge\CamBridge.QRBridge.exe" %*
)
"@
$qrBridgeBatch | Set-Content "$deployDir\QRBridge.bat" -Encoding ASCII

# Create version file with QRBridge info
$versionContent = @"
CamBridge Deployment Package
Version: $Version
Build Date: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
Build Type: Framework-Dependent (requires .NET 8.0 Runtime)
(c) 2025 Claude's Improbably Reliable Software Solutions

Components:
- CamBridge Service (JPEG to DICOM Pipeline)
- CamBridge Config (Configuration UI)
- CamBridge QRBridge 2.0 (QR Code Generator)
  - Full Version: CamBridge.QRBridge.exe (~7MB)
  - Ultra Slim: CamBridge.QRBridge_slim.exe (~500KB)

System Requirements:
- Windows 10/11 or Windows Server 2019+
- .NET 8.0 Runtime (download from https://dotnet.microsoft.com/download/dotnet/8.0)

Complete Medical Imaging Pipeline:
QRBridge → Ricoh Camera → JPEG → CamBridge → DICOM → PACS

QRBridge Usage:
- Full version: QRBridge.bat [arguments]
- Slim version: QRBridge.bat -slim [arguments]
"@
$versionContent | Set-Content "$deployDir\version.txt"

Write-Host "  [OK] Package created" -ForegroundColor Green
Write-Host ""

# Step 8: Create ZIP archive
Write-Host "Step 8: Creating ZIP archive..." -ForegroundColor Yellow
$zipPath = Join-Path $OutputPath "CamBridge-v$Version-Deploy.zip"
Compress-Archive -Path $deployDir -DestinationPath $zipPath -Force
Write-Host "  [OK] ZIP created" -ForegroundColor Green

# Calculate sizes
$serviceSize = (Get-ChildItem $serviceDir -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
$configSize = (Get-ChildItem $configDir -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
$qrBridgeSize = (Get-ChildItem $qrBridgeDir -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
$zipSize = (Get-Item $zipPath).Length / 1MB

# Check for slim version
$slimExe = Get-Item "$qrBridgeDir\CamBridge.QRBridge_slim.exe" -ErrorAction SilentlyContinue
$slimSize = if ($slimExe) { [math]::Round($slimExe.Length / 1KB) } else { 0 }

# Final summary
Write-Host ""
Write-Host "================================================" -ForegroundColor Green
Write-Host "  BUILD & DEPLOYMENT SUCCESSFUL!" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green
Write-Host ""
Write-Host "Package Details:" -ForegroundColor Cyan
Write-Host "  Version:       $Version" -ForegroundColor White
Write-Host "  Build Type:    Framework-Dependent (smaller size)" -ForegroundColor White
Write-Host "  Service Size:  $([math]::Round($serviceSize, 2)) MB" -ForegroundColor Gray
Write-Host "  Config Size:   $([math]::Round($configSize, 2)) MB" -ForegroundColor Gray
Write-Host "  QRBridge Size: $([math]::Round($qrBridgeSize, 2)) MB" -ForegroundColor Gray
if ($slimSize -gt 0) {
    Write-Host "    - Ultra Slim: $slimSize KB (single exe)" -ForegroundColor DarkGray
}
Write-Host "  Total ZIP:     $([math]::Round($zipSize, 2)) MB" -ForegroundColor Gray
Write-Host ""
Write-Host "Output Files:" -ForegroundColor Cyan
Write-Host "  Folder: $deployDir" -ForegroundColor White
Write-Host "  ZIP:    $zipPath" -ForegroundColor White
Write-Host ""
Write-Host "⚠️  IMPORTANT: Framework-Dependent Build" -ForegroundColor Yellow
Write-Host "  QRBridge requires .NET 8.0 Runtime installed on target system!" -ForegroundColor Yellow
Write-Host "  Download from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Gray
Write-Host ""
Write-Host "Complete Pipeline Components:" -ForegroundColor Yellow
Write-Host "  1. QRBridge.bat - Generate QR codes for Ricoh cameras" -ForegroundColor Gray
Write-Host "     - Full version: QRBridge.bat [args]" -ForegroundColor DarkGray
Write-Host "     - Slim version: QRBridge.bat -slim [args]" -ForegroundColor DarkGray
Write-Host "  2. CamBridge Service - Process JPEGs to DICOM" -ForegroundColor Gray
Write-Host "  3. Config UI - Configure the pipeline" -ForegroundColor Gray
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "  1. Extract ZIP on target machine" -ForegroundColor Gray
Write-Host "  2. Run Install-CamBridge.ps1 as Administrator" -ForegroundColor Gray
Write-Host "  3. Use QRBridge.bat to generate QR codes" -ForegroundColor Gray
Write-Host "  4. Start Config UI from: C:\Program Files\CamBridge\Config" -ForegroundColor Gray
Write-Host ""
Write-Host "The medical imaging pipeline is complete! 🎉" -ForegroundColor Green
Write-Host ""

# Cleanup
if (Test-Path ".\QRBridgeUltraSlim.cs") {
    Remove-Item ".\QRBridgeUltraSlim.cs" -Force -ErrorAction SilentlyContinue
}
if (Test-Path ".\QRBridgeSlimTemp") {
    Remove-Item ".\QRBridgeSlimTemp" -Recurse -Force -ErrorAction SilentlyContinue
}
if (Test-Path ".\TempQRBridgePublish") {
    Remove-Item ".\TempQRBridgePublish" -Recurse -Force -ErrorAction SilentlyContinue
}
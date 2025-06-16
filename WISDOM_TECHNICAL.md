# WISDOM_TECHNICAL.md - Complete Technical Reference
**Version**: 0.7.18  
**Last Update**: 2025-06-16 18:42  
**Purpose**: Technical implementation wisdom, patterns, solutions  
**Philosophy**: KISS, Tab-Complete, Sources First, Direct Dependencies

## 🎭 V.O.G.O.N. PROTOCOL

### VOGON INIT (Start of complex work)
```yaml
V - Verify: Check current state, what exists
O - Organize: Structure the approach
G - Generate: Create/modify code
O - Observe: Test and validate
N - Next: Plan next steps
```

### VOGON EXIT (End of session)
```yaml
V - Validate: Confirm all changes work
O - Organize: Update documentation
G - Generate: Create summary/changelog
O - Observe: Note remaining issues
N - Next: Clear next actions
```

### Usage Examples
```
"VOGON INIT für Interface Removal"
"Mini-VOGON für Config check"
"VOGON EXIT mit v0.7.18 release"
```

## 🔧 TECHNICAL STACK

```yaml
Platform: Windows Service + WPF Config Tool
Framework: .NET 8.0, C# 12
GUI: WPF + ModernWpfUI 0.9.6 + MVVM Toolkit
Service: ASP.NET Core Minimal API
DICOM: fo-dicom 5.2.2
EXIF: ExifTool 13.30
Testing: Tab-Complete PowerShell
IDE: Visual Studio 2022
Build: MSBuild with Version.props
Deployment: PowerShell Scripts
Dependencies: Direct (no interfaces!)
```

## 💻 ESSENTIAL COMMANDS

### Tab-Complete Build System (MEMORIZE!)
```powershell
# Core commands - your daily drivers
0[TAB]  # Build only (no ZIP) - 20 seconds
1[TAB]  # Deploy & Start Service
2[TAB]  # Open Config Tool  
9[TAB]  # Test without build
h[TAB]  # Show help

# Advanced operations
00[TAB] # Build WITH ZIP (for releases)
11[TAB] # Deploy with backup
99[TAB] # Full test with build
4[TAB]  # Start console mode (debugging)
```

### Service Management
```powershell
# Status & Control
Get-Service CamBridgeService | Format-List *
Stop-Service CamBridgeService -Force
Start-Service CamBridgeService
Restart-Service CamBridgeService

# Console Mode (for debugging)
.\4-console.ps1  # Runs as console app

# Logs & Debugging
Get-EventLog -LogName Application -Source CamBridge* -Newest 20
Get-Content "$env:ProgramData\CamBridge\logs\*.log" -Tail 50 -Wait
```

### API Testing (v0.7.18 endpoints)
```powershell
# Working endpoints
Invoke-RestMethod "http://localhost:5111/api/status"         # Full status
Invoke-RestMethod "http://localhost:5111/api/pipelines"      # All pipelines
Invoke-RestMethod "http://localhost:5111/api/status/version" # Version only ✅
Invoke-RestMethod "http://localhost:5111/api/status/health"  # Health check ✅

# Not implemented yet (404)
# /api/statistics
```

## 🎯 PROVEN PATTERNS

### Configuration Management
```csharp
// SINGLE SOURCE OF TRUTH - ConfigurationPaths
public static class ConfigurationPaths
{
    public static string GetPrimaryConfigPath() =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "CamBridge", 
            "appsettings.json");
    
    // All paths derive from this!
}
```

### Dynamic Version Reading (Since v0.7.16!)
```csharp
// No more hardcoded versions!
public static string Version
{
    get
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        
        // Try FileVersion first (from Version.props)
        if (!string.IsNullOrEmpty(fileVersionInfo.FileVersion))
        {
            var version = fileVersionInfo.FileVersion;
            if (version.EndsWith(".0"))
                version = version.Substring(0, version.LastIndexOf(".0"));
            return version;
        }
        
        return "0.7.18"; // Emergency fallback only
    }
}
```

### Service Registration (KISS - v0.7.18!)
```csharp
// NO MORE INTERFACES! Direct registration
services.AddSingleton<ExifToolReader>();
services.AddSingleton<DicomConverter>();
services.AddSingleton<FileProcessor>();
services.AddSingleton<PipelineManager>();
services.AddSingleton<NotificationService>(); // v0.7.18: Direct!

// Only 2 interfaces remain (for now):
services.AddSingleton<IMappingConfiguration, MappingConfigurationLoader>();
services.AddSingleton<IDicomTagMapper, DicomTagMapper>();
```

### Error Handling Pattern
```csharp
try 
{
    await ProcessFileAsync(file);
}
catch (Exception ex) when (ex is not OperationCanceledException)
{
    _logger.LogError(ex, "Failed: {File}", file);
    MoveToErrorFolder(file);  // Simple! No complex queues
}
```

### Pipeline Architecture
```csharp
// Each pipeline is independent
foreach (var pipeline in EnabledPipelines)
{
    var watcher = new FileWatcher(pipeline.WatchFolder);
    var queue = new Channel<string>(100);
    var processor = new FileProcessor(pipeline.Config);
    // Independent processing loop
}
```

### Direct Dependency Pattern (NEW!)
```csharp
// OLD (with interface)
public class FileProcessor
{
    private readonly IExifReader _exifReader;
    public FileProcessor(IExifReader exifReader) { }
}

// NEW (direct dependency - v0.7.18)
public class FileProcessor
{
    private readonly ExifToolReader _exifReader;
    public FileProcessor(ExifToolReader exifReader) { }
}
```

## 🐛 CRITICAL FIXES REFERENCE

### Fix #1: Port Consistency
```yaml
Problem: Service on 5050, Config expects 5111
Fix: Global replace 5050 → 5111
Files: Program.cs, appsettings.json, ViewModels
Status: FIXED ✅
```

### Fix #2: Config Wrapper
```json
// ALWAYS wrap with "CamBridge"!
{
  "CamBridge": {
    "Version": "2.0",
    "Service": { },
    "Pipelines": [ ]
  }
}
```

### Fix #3: Enum Values
```csharp
// OutputOrganization - ONLY these values!
public enum OutputOrganization
{
    None,              // Valid ✅
    ByPatient,         // Valid ✅
    ByDate,            // Valid ✅
    ByPatientAndDate   // Valid ✅
    // "PatientName" → CRASH! ❌
}
```

### Fix #4: Host Property
```csharp
// App.xaml.cs - fixes 144 errors!
public partial class App : Application
{
    private IHost _host;
    public IHost Host => _host;  // THIS LINE!
}
```

### Fix #5: Dynamic Version (v0.7.16)
```yaml
Problem: Version hardcoded in ServiceInfo.cs
Old: public const string Version = "0.7.9";
New: Dynamic reading from assembly
Status: FIXED in v0.7.16 ✅
```

### Fix #6: Enum Validation (v0.7.17)
```yaml
Problem: Invalid OutputOrganization crashes service
Solution: Added validation in ConfigurationService
Implementation: 
  - JsonStringEnumConverter for parsing
  - ValidateEnumValues method
  - Clear error messages
Status: FIXED in v0.7.17 ✅
```

### Fix #7: Direct Dependencies (v0.7.18)
```yaml
Problem: Too many unnecessary interfaces
Solution: Direct class registration
Removed:
  - IDicomConverter → DicomConverter
  - INotificationService → NotificationService
  - IExifReader → ExifToolReader (earlier)
  - IFolderWatcher → FolderWatcherService (earlier)
Status: FIXED in v0.7.18 ✅
```

## 🔨 ESSENTIAL TOOLS

### Development Tools
- Visual Studio 2022 (v17.8+)
- .NET 8 SDK
- PowerShell 7+
- Git for Windows
- Notepad++ (for quick edits)

### Debugging Tools
- Event Viewer (eventvwr.msc)
- Services Manager (services.msc)
- Process Monitor (ProcMon)
- PowerShell ISE (for script debugging)

### Project Scripts
```
Build-CamBridge.ps1      # Tab-complete builder
Test-CamBridge.ps1       # Quick tester
Get-WisdomSources.ps1    # Source extractor
Debug-CamBridgeJson.ps1  # Config validator
Test-QRBridge.ps1        # QR code generator
Create-DeploymentPackage.ps1  # Release builder
```

## 🏗️ BUILD & DEPLOY

### Quick Development Cycle
```powershell
# The holy trinity
0[TAB]  # Build (20 sec)
1[TAB]  # Deploy & Start
9[TAB]  # Test

# Full release cycle
00[TAB] # Build with ZIP
git tag v0.7.18
git push --tags
```

### Version Management (Version.props)
```xml
<!-- Single source of truth for versions -->
<PropertyGroup>
  <VersionPrefix>0.7.18</VersionPrefix>
  <FileVersion>0.7.18.0</FileVersion>
  <AssemblyVersion>0.7.0.0</AssemblyVersion>
  <InformationalVersion>0.7.18 - Sprint 10: Interface Removal Complete</InformationalVersion>
</PropertyGroup>
```

### Deployment Structure
```
C:\CamBridge\
├── Service\          # Windows Service files
├── ConfigTool\       # WPF Config application
├── Tools\            # ExifTool, scripts
└── Logs\            # Service logs

%ProgramData%\CamBridge\
├── appsettings.json  # Primary config (V2 format)
├── Pipelines\        # Pipeline configs
├── Mappings\         # DICOM mappings
└── Logs\            # Runtime logs

C:\CamBridge\Watch\   # Input folders
├── Radiology\
└── Emergency\

C:\CamBridge\Output\  # DICOM output
├── Radiology\
└── Emergency\
```

## 📅 SESSION HISTORY (Key Learnings)

### Sessions 1-20: Architecture Explosion
- Started with Clean Architecture → 12+ interfaces
- **Learning**: SOLID ≠ Simple, overengineering kills
- **Impact**: Complexity debt we're still paying

### Sessions 21-40: Config Chaos
- 3 config versions, multiple loaders, migration hell
- **Learning**: One format, one path, one truth
- **Status**: Still cleaning up (V1 remnants exist)

### Sessions 41-50: GUI Implementation  
- WPF + MVVM, Dashboard wouldn't load
- **Learning**: Check ports, check dependencies, check obvious
- **Fix**: Port 5111 everywhere

### Sessions 51-55: Tab-Complete Revolution
- Build times 3min → 20sec with simple PowerShell
- **Learning**: User ideas > complex solutions
- **Impact**: Development speed 10x

### Sessions 56-58: The Great Deletion
- Removed Dead Letter Queue (-650 LOC)
- **Learning**: Deleting code is progress
- **Philosophy**: KISS wins every time

### Sessions 59-61: Dashboard Victory
- Fixed port mismatch (5050 → 5111)
- **Learning**: Small details break everything
- **The Enlightenment**: "I wrote ALL this code!"

### Sessions 62-63: Final Fixes
- Host property, enum values, config completion
- **Learning**: One line can fix 144 errors
- **Status**: Core functionality working

### Session 64-65: Core Functionality Complete!
- Dynamic version reading implemented
- ExifTool deadlock fixed
- Development.json eliminated
- Encoding issues resolved
- **Achievement**: JPEG→DICOM WORKING!
- **Learning**: Check obvious first, KISS wins

### Session 66: Sprint 9 Complete!
- Discovered InitializePrimaryConfig was already complete
- Added enum validation for OutputOrganization
- Clear error messages for config issues
- **Learning**: Always check sources first!
- **Achievement**: Config system now robust

### Session 67: Sprint 10 - Interface Removal!
- Removed IDicomConverter and INotificationService
- Discovered IExifReader and IFolderWatcher already gone
- Made up properties in DicomConverter (Sources First fail!)
- **Learning**: Always check real interfaces/properties
- **Achievement**: 8 → 2 interfaces (75% reduction!)

## 📊 METRICS THAT MATTER

```yaml
Total LOC: 14,350+ (all by Claude!)
Interfaces: Started 12+ → Current 2 → Target 0
Build Time: 3min → 20sec (without ZIP)
Config Formats: 3 → 1 (V2 unified)
Warnings: 144 (needs cleanup)
Deleted: 650+ LOC (Dead Letter + more)
Fixed: Port 5111 everywhere ✅
Version: 0.7.18 (dynamic now!)
Pipelines: 2 configured & working
API Endpoints: 4/5 implemented
CORE FEATURE: JPEG→DICOM WORKING! ✅
CONFIG: Validates all enums ✅
ARCHITECTURE: Direct dependencies! ✅
```

## 📈 CURRENT STATUS & ROADMAP

### Current: v0.7.18 - Direct Dependencies Everywhere
```yaml
Done:
✅ Removed 4 interfaces (2 already gone)
✅ Direct dependency pattern implemented
✅ Service running stable
✅ Config validation robust
✅ Build successful
✅ KISS principle applied

Issues:
- 144 build warnings
- Missing API endpoint (/api/statistics)
- 2 interfaces remain
```

### Next: Sprint 11 - Final Interface Analysis (v0.8.x)
```yaml
Goals:
- Analyze IMappingConfiguration necessity
- Analyze IDicomTagMapper necessity
- Consider full direct dependency model
- Reduce warnings to <100

Questions:
- Do we need any interfaces at all?
- What's the cost of removing the last 2?
```

### Future Sprints
```yaml
Sprint 12: Config Simplification (v0.9.x)
- Merge config classes
- Remove V1 support completely
- Single unified model

Sprint 13: Performance (v0.9.x)
- Optimize file processing
- Parallel pipeline execution
- Memory usage optimization

Sprint 14+: Protected Medical Features
- FTP Server (basic only!)
- C-STORE SCP
- Modality Worklist  
[DO NOT START THESE YET!]
```

## 🎯 DEVELOPMENT PRINCIPLES

### The KISS Ladder
```yaml
Level 1: It works (current) ✅
Level 2: It's simple (achieved) ✅
Level 3: It's elegant (getting there)
Level 4: It's invisible (nirvana)
```

### Decision Framework
```yaml
Question: "Should we add this feature?"
1. Does it solve a real problem? 
2. Is it the simplest solution?
3. Can we delete something instead?
4. What would Oliver say?
→ If any "No": Don't do it
```

### Protected Patterns
```yaml
Tab-Complete: Sacred, never change
Port 5111: Carved in stone  
Config Path: Single source of truth
VOGON: For complex work only
Sources First: Always check existing (even if I forget)
Dynamic Version: Never hardcode again
Enum Validation: Clear errors always
Direct Dependencies: The new way
```

## 🔧 QUICK REFERENCE CARD

### Daily Commands
```powershell
0[TAB]   # Build
1[TAB]   # Deploy  
2[TAB]   # Config Tool
9[TAB]   # Test
4[TAB]   # Console Mode
```

### Check Status
```powershell
# Service status
Invoke-RestMethod "http://localhost:5111/api/status" | ConvertTo-Json -Depth 5

# Version only (v0.7.18 adds this!)
(Invoke-RestMethod "http://localhost:5111/api/status/version")

# Pipeline status
Invoke-RestMethod "http://localhost:5111/api/pipelines"
```

### Common Fixes
```powershell
# Service won't start
Get-EventLog -LogName Application -Source CamBridge* -Newest 10

# Config corrupted
Remove-Item "$env:ProgramData\CamBridge\appsettings.json"
Start-Service CamBridgeService  # Creates fresh config

# Version mismatch
# Check Version.props → Build → Deploy

# Bad enum value
# Check error message - v0.7.17 shows valid values!

# Missing interface
# Just use the concrete class directly!
```

## 📝 WISDOM NOTES

### What Works Well
- Tab-complete system (Oliver's idea!)
- Single config path
- Simple error folders
- Pipeline independence
- Dynamic version reading
- Enum validation (new!)
- Direct dependencies (v0.7.18!)

### What Needs Work
- Too many warnings (144)
- Missing API endpoint
- Documentation gaps
- Sources First compliance (I keep forgetting!)

### Lessons Learned
1. **KISS beats SOLID** every single time
2. **Delete first**, add features second
3. **User knows best** - listen to Oliver
4. **Automate everything** - versions, builds, tests
5. **Details matter** - one wrong port = hours of debugging
6. **Check sources first** - code might already exist!
7. **Direct > Abstract** - interfaces without reason = delete!

### Session 67 Special Learning
```yaml
Problem: Made up DicomConverter properties
Examples:
  - CameraInfo (doesn't exist)
  - OriginalFilename (doesn't exist)
  - GetMappingRulesAsync (it's GetMappingRules)
  - MapMetadataToDicom (it's MapToDataset)
  
Impact: Had to remake entire artifact
Learning: ALWAYS check real sources!
Oliver's reaction: "du denkst dir wieder was aus"
My reaction: He's absolutely right...
```

---

*"Making the improbable reliably simple - one deleted interface at a time!"*
*Version 0.7.18 - Direct dependencies everywhere!*

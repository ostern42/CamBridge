# WISDOM_TECHNICAL.md - Complete Technical Reference
**Version**: 0.7.21  
**Last Update**: 2025-06-17 01:15  
**Purpose**: Technical implementation wisdom, patterns, solutions  
**Philosophy**: KISS, Tab-Complete, Sources First, Direct Dependencies, Pipeline Isolation

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
"VOGON EXIT mit v0.7.21 release"
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
Architecture: Pipeline-isolated processing
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

### API Testing (v0.7.21 endpoints)
```powershell
# Working endpoints
Invoke-RestMethod "http://localhost:5111/api/status"         # Full status
Invoke-RestMethod "http://localhost:5111/api/pipelines"      # All pipelines
Invoke-RestMethod "http://localhost:5111/api/status/version" # Version only
Invoke-RestMethod "http://localhost:5111/api/status/health"  # Health check

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
        
        return "0.7.21"; // Emergency fallback only
    }
}
```

### Service Registration (KISS - v0.7.20!)
```csharp
// NO MORE INTERFACES! Direct registration
services.AddSingleton<ExifToolReader>();
services.AddSingleton<DicomConverter>();
// FileProcessor is NOT registered - created per pipeline!
services.AddSingleton<PipelineManager>();
services.AddSingleton<NotificationService>();

// Only 2 interfaces remain (for now):
services.AddSingleton<IMappingConfiguration, MappingConfigurationLoader>();
services.AddSingleton<IDicomTagMapper, DicomTagMapper>();
```

### Pipeline-Isolated FileProcessor (NEW v0.7.20!)
```csharp
// Each pipeline creates its own FileProcessor
var fileProcessor = new FileProcessor(
    logger,
    exifToolReader,  // Shared is OK
    dicomConverter,  // Shared is OK
    pipelineConfig   // Pipeline-specific!
);

// ApplyDicomOverrides for pipeline-specific DICOM settings
private DicomSettings ApplyDicomOverrides(DicomSettings global, DicomOverrides? overrides)
{
    if (overrides == null) return global;
    // Apply only institution-specific overrides
    return new DicomSettings { 
        InstitutionName = overrides.InstitutionName ?? global.InstitutionName,
        // ... etc
    };
}
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

### Pipeline Architecture (v0.7.20 FIXED!)
```csharp
// Each pipeline is COMPLETELY independent
foreach (var pipeline in EnabledPipelines)
{
    var fileProcessor = new FileProcessor(pipeline.Config);
    var queue = new ProcessingQueue(fileProcessor);
    var watcher = new FileWatcher(pipeline.WatchFolder);
    // Independent processing loop
}
```

### Direct Dependency Pattern (v0.7.18+)
```csharp
// OLD (with interface)
public class FileProcessor
{
    private readonly IExifReader _exifReader;
    public FileProcessor(IExifReader exifReader) { }
}

// NEW (direct dependency)
public class FileProcessor
{
    private readonly ExifToolReader _exifReader;
    public FileProcessor(ExifToolReader exifReader) { }
}
```

### Minimal Dashboard Pattern (v0.7.21!)
```csharp
// When complex fails, go simple!
public class DashboardViewModel
{
    private readonly HttpClient _httpClient = new();
    
    public DashboardViewModel()
    {
        // Timer starts immediately - no InitializeAsync!
        _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        _refreshTimer.Tick += async (s, e) => await RefreshAsync();
        _refreshTimer.Start();
    }
    
    private async Task RefreshAsync()
    {
        // Direct HTTP - no abstractions!
        var response = await _httpClient.GetAsync("http://localhost:5111/api/status");
        var json = await response.Content.ReadAsStringAsync();
        // Parse and update UI
    }
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

### Fix #8: Pipeline Isolation (v0.7.20)
```yaml
Problem: FileProcessor was singleton but needs pipeline config
Solution: Each pipeline creates own FileProcessor
Implementation:
  - PipelineManager creates FileProcessor per pipeline
  - ProcessingQueue gets direct FileProcessor
  - DicomOverrides properly applied
Status: FIXED in v0.7.20 ✅
```

### Fix #9: Dashboard ViewModel Injection (v0.7.21)
```yaml
Problem: Dashboard zeigte keine Service Daten
Symptom: Service Control funktioniert, Dashboard nicht
Root Cause: NavigationService erstellte Pages OHNE ViewModels
Solution: ViewModel Injection in NavigationService
Files: NavigationService.cs, DashboardPage.xaml.cs
Learning: "When one page works and another doesn't, copy what works!"
Status: FIXED in v0.7.21 ✅
```

### Fix #10: API Property Case Sensitivity (v0.7.21)
```yaml
Problem: Dashboard zeigte keine Pipeline Status
Root Cause: API sendet lowercase "name", Code suchte "Name"
Solution: Case-insensitive comparison + check actual response
Example:
  API sends: { "name": "Radiology" }
  Code expected: { "Name": "Radiology" }
Fix: PropertyNameCaseInsensitive = true
Status: FIXED in v0.7.21 ✅
```

### Fix #11: ViewModel Initialization Pattern (v0.7.21)
```yaml
Problem: Dashboard showed no data despite API working
Root Cause: InitializeAsync never called (DataContextChanged issue)
Investigation:
  - OnDataContextChanged only fires when DataContext CHANGES
  - Constructor already set DataContext = no change event
  - InitializeAsync never ran = no timer = no data
Solution: Use Loaded event OR initialize in constructor
Better: Skip InitializeAsync pattern completely!
Status: FIXED in v0.7.21 ✅
```

### Fix #12: Minimal Dashboard Pattern (v0.7.21)
```yaml
Problem: Complex IApiService/refresh logic failed mysteriously
Solution: Direct HttpClient calls in ViewModel
Implementation:
  private readonly HttpClient _httpClient = new();
  // Direct calls, no abstractions
Pattern:
  - HttpClient in ViewModel (KISS)
  - Simple timer in constructor
  - No InitializeAsync complexity
  - JsonSerializer.Deserialize directly
Result: Working dashboard in <100 LOC!
Status: FIXED in v0.7.21 ✅
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
git tag v0.7.21
git push --tags
```

### Version Management (Version.props)
```xml
<!-- Single source of truth for versions -->
<PropertyGroup>
  <VersionPrefix>0.7.21</VersionPrefix>
  <FileVersion>0.7.21.0</FileVersion>
  <AssemblyVersion>0.7.0.0</AssemblyVersion>
  <InformationalVersion>0.7.21 - Dashboard ViewModel Fix</InformationalVersion>
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

### Session 68: Pipeline Architecture Fix!
- Fixed FileProcessor singleton problem
- Each pipeline now has own FileProcessor
- DicomOverrides vs DicomSettings resolved
- Legacy V1 references removed
- **Achievement**: True pipeline isolation!
- **Learning**: Architecture debt must be paid

### Session 69: Dashboard Detective Victory!
- Problem: Dashboard blank for 3+ hours
- Tried: Complex debugging, multiple approaches
- Solution: Minimal rewrite with direct HTTP
- **Learning**: When complex fails, go minimal!
- **Pattern**: Compare working (Service Control) vs broken (Dashboard)
- **Victory**: Dashboard finally shows service status!
- **Emotion**: Frustration → "minimal" decision → Success!

## 📊 METRICS THAT MATTER

```yaml
Total LOC: 14,350+ (all by Claude!)
Interfaces: Started 12+ → Current 2 → Target 0
Build Time: 3min → 20sec (without ZIP)
Config Formats: 3 → 1 (V2 unified)
Warnings: ~140 (needs cleanup)
Deleted: 700+ LOC (Dead Letter + more)
Fixed: Port 5111 everywhere ✅
Version: 0.7.21 (dynamic now!)
Pipelines: Truly isolated! ✅
API Endpoints: 4/5 implemented
CORE FEATURE: JPEG→DICOM WORKING! ✅
CONFIG: Validates all enums ✅
ARCHITECTURE: Pipeline isolation! ✅
DASHBOARD: Shows service status! ✅
```

## 📈 CURRENT STATUS & ROADMAP

### Current: v0.7.21 - Dashboard Working!
```yaml
Done:
✅ Dashboard shows service status
✅ Minimal implementation works
✅ NavigationService injects ViewModels
✅ Direct HTTP pattern proven
✅ Auto-refresh reliable
✅ Pipelines visible

Issues:
- Pipeline Config page empty
- ~140 build warnings
- Missing API endpoint (/api/statistics)
- 2 interfaces remain
```

### Next: Sprint 16 - UI Polish (v0.7.x)
```yaml
Goals:
- Fix Pipeline Config page
- Modern dashboard design
- Interactive features
- Live activity feed

Scope:
- UI improvements only
- No architecture changes
- Focus on user experience
```

### Future Sprints
```yaml
Sprint 17: Warning Reduction (v0.8.x)
- Reduce warnings to <50
- Fix nullable reference warnings
- Clean up unused code

Sprint 18: Final Interface Analysis (v0.8.x)
- Analyze IMappingConfiguration necessity
- Analyze IDicomTagMapper necessity
- Consider full direct dependency model

Sprint 19: Performance (v0.9.x)
- Optimize file processing
- Parallel pipeline execution
- Memory usage optimization

Sprint 20+: Protected Medical Features
- FTP Server (basic only!)
- C-STORE SCP
- Modality Worklist  
[DO NOT START THESE YET!]
```

## 🎯 DEVELOPMENT PRINCIPLES

### The KISS Ladder
```yaml
Level 1: It works (achieved) ✅
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
Sources First: Always check existing
Dynamic Version: Never hardcode again
Enum Validation: Clear errors always
Direct Dependencies: The new way
Pipeline Isolation: Each pipeline independent!
Minimal Dashboard: When complex fails, go simple!
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

# Version only
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
# Check error message - shows valid values!

# Pipeline not working
# Check pipeline has own output folder configured

# Dashboard shows nothing
# Try minimal approach - direct HTTP!
```

## 📝 WISDOM NOTES

### What Works Well
- Tab-complete system (Oliver's idea!)
- Single config path
- Simple error folders
- Pipeline independence (FIXED in v0.7.20!)
- Dynamic version reading
- Enum validation
- Direct dependencies
- Per-pipeline FileProcessor
- Minimal dashboard approach (v0.7.21!)

### What Needs Work
- Too many warnings (~140)
- Missing API endpoint
- Documentation gaps
- Some architectural debt remains
- Pipeline Config page empty

### Lessons Learned
1. **KISS beats SOLID** every single time
2. **Delete first**, add features second
3. **User knows best** - listen to Oliver
4. **Automate everything** - versions, builds, tests
5. **Details matter** - one wrong port = hours of debugging
6. **Check sources first** - code might already exist!
7. **Direct > Abstract** - interfaces without reason = delete!
8. **Singletons + Config = Problems** - isolate pipelines!
9. **When complex fails, go minimal** - Session 69 proved it!

### Session 69 Special Learning
```yaml
Problem: Dashboard wouldn't show service status
Duration: 3+ hours of debugging
Attempts: 10+ different approaches
User: "ich werde bald wahnsinnig"
Solution: "minimal" - complete rewrite
Result: Working in minutes!
Learning: Sometimes throwing away code is the answer
```

---

*"Making the improbable reliably simple - one minimal solution at a time!"*
*Version 0.7.21 - Dashboard works through minimalism!*

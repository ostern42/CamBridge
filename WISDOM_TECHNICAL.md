# WISDOM_TECHNICAL.md - Complete Technical Reference
**Version**: 0.7.26  
**Last Update**: 2025-06-18 15:42  
**Purpose**: Technical implementation wisdom, patterns, solutions  
**Philosophy**: KISS, Tab-Complete, Sources First, Direct Dependencies, Pipeline Isolation, UI Clarity, Hidden Treasures

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
"VOGON INIT für Mapping Editor Fix"
"Mini-VOGON für Event Handler check"
"VOGON EXIT mit v0.7.26 release"
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
UI Philosophy: Clean, minimal, functional
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
7[TAB]  # Clean build artifacts (Session 71!)
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

### API Testing (v0.7.24 endpoints)
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

### The Hidden Treasures Pattern (NEW v0.7.26!)
```yaml
Problem: Features implemented but not exposed
Discovery Method:
  1. User mentions something should exist
  2. Check implementation phase code
  3. Find it's already there!
  4. Create UI to expose it

Example - Transform System:
  - 11 transforms in ValueTransform.cs ✓
  - ApplyTransform() fully working ✓
  - Just needed Transform Editor UI ✓
  
Learning: Always check existing code first!
```

### Transform Editor Pattern (NEW v0.7.26!)
```csharp
// Multi-View Preview for medical data
public enum PreviewMode 
{
    Normal,        // Standard view
    SpecialChars,  // Shows [CR], [LF], [TAB]
    Hex           // Full hex dump
}

// Encoding awareness
private void DetectInputEncoding()
{
    bool hasUmlauts = input.Any(c => "äöüÄÖÜß".Contains(c));
    InputEncoding = hasUmlauts ? "UTF-8" : "Windows-1252";
}

// Transform-aware test data
PreviewInput = SelectedTransform switch
{
    ValueTransform.ExtractDate => DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"),
    ValueTransform.MapGender => "M",
    ValueTransform.Trim => "  trimmed text  ",
    // etc...
};
```

### XAML Compatibility Patterns (NEW v0.7.26!)
```xml
<!-- WRONG: Run doesn't support Opacity -->
<Run Text="text" Opacity="0.7"/>

<!-- RIGHT: Use TextBlock -->
<TextBlock Text="text" Opacity="0.7"/>

<!-- WRONG: Direct PlaceholderText -->
<TextBox PlaceholderText="Enter..."/>

<!-- RIGHT: ModernWpfUI attached property -->
<TextBox ui:ControlHelper.PlaceholderText="Enter..."/>
```

### ContentDialog vs Window Pattern (NEW v0.7.26!)
```csharp
// Window (regular dialog)
var dialog = new DicomTagBrowserDialog
{
    Owner = Application.Current.MainWindow
};
if (dialog.ShowDialog() == true) { }

// ContentDialog (ModernWpfUI)
var dialog = new TransformEditorDialog();
var result = await dialog.ShowAsync();
if (result == ContentDialogResult.Primary) { }
```

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
        
        return "0.7.26"; // Emergency fallback only
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

### The Build Cache Pattern (NEW v0.7.23!)
```powershell
# When build shows errors for fixed code
# 1. Clean completely
Remove-Item -Recurse -Force */obj, */bin

# 2. Verify file saved
Get-Content "path\to\file.xaml" | Select-String "ErrorPattern"

# 3. Search for other occurrences
Select-String -Path "src\**\*.xaml" -Pattern "ErrorPattern" -Recurse

# 4. Rebuild clean
dotnet build --no-incremental
```

### Event Handler Connection Pattern (NEW v0.7.24!)
```csharp
// XAML.CS - Connect event handlers in constructor
public MappingEditorPage()
{
    InitializeComponent();
    
    // Connect drag & drop handlers
    foreach (var item in SourceFieldsList.Items)
    {
        if (item is FrameworkElement element)
        {
            element.MouseMove += SourceField_MouseMove;
            element.GiveFeedback += SourceField_GiveFeedback;
        }
    }
}

// Don't forget AllowDrop="True" in XAML!
```

### UI Clarity Pattern (NEW v0.7.25!)
```xml
<!-- BEFORE: Complex UI with unnecessary elements -->
<Grid.ColumnDefinitions>
    <ColumnDefinition Width="300"/> <!-- Sources -->
    <ColumnDefinition Width="*"/>   <!-- Rules -->
    <ColumnDefinition Width="300"/> <!-- Cheat Sheet -->
</Grid.ColumnDefinitions>

<!-- AFTER: Clean, focused UI -->
<Grid.ColumnDefinitions>
    <ColumnDefinition Width="300"/> <!-- Sources -->
    <ColumnDefinition Width="*"/>   <!-- Rules (more space!) -->
</Grid.ColumnDefinitions>
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

### Fix #13: Pipeline Config Converter Bug (v0.7.22)
```yaml
Problem: "No Pipeline Selected" shown even when pipeline selected
Root Cause: InverseBoolToVisibility converter used with object
Solution: Use NullToVisibility converter with Inverse parameter
Example:
  Wrong: Converter={StaticResource InverseBoolToVisibility}
  Right: Converter={StaticResource NullToVisibility}, ConverterParameter=Inverse
Fix: One line change in XAML
Status: FIXED in v0.7.22 ✅
```

### Fix #14: Navigation Dropdown (v0.7.24)
```yaml
Problem: Frame shows growing navigation history dropdown
Solution: NavigationUIVisibility="Hidden" + clear history
Implementation:
  <Frame NavigationUIVisibility="Hidden"/>
  while (ContentFrame.NavigationService.CanGoBack)
      ContentFrame.NavigationService.RemoveBackEntry();
Status: FIXED in v0.7.24 ✅
```

### Fix #15: Mapping Editor Issues (v0.7.25)
```yaml
Problem: Multiple UI issues in Mapping Editor
Issues:
  - Drag & Drop not working
  - Browse All Tags does nothing
  - New Mapping Set has no name input
  - Fields show as "newField"
Solution: Event handlers connected, UI elements added
Status: FIXED in v0.7.25 ✅
```

### Fix #16: UI Clutter (v0.7.25)
```yaml
Problem: Cheat sheet takes 25% of screen space
User Question: "brauchen wir den spickzettel überhaupt?"
Analysis: Browse button exists, cheat sheet rarely used
Solution: Remove cheat sheet, expand mapping area
Implementation:
  - Removed right column
  - Expanded center column
  - Moved browse button to header
Result: 40% more working space!
Status: FIXED in v0.7.25 ✅
```

### Fix #17: XAML Opacity on Run (v0.7.25)
```yaml
Problem: Run elements don't support Opacity property
Error: MC3072 at line 327
Solution: Remove Opacity from Run elements
Alternative: Use different Foreground colors
Status: FIXED in v0.7.25 ✅
```

### Fix #18: Transform System Hidden (v0.7.26)
```yaml
Problem: Transform system not exposed in UI
Discovery: Everything already implemented!
Found:
  - ValueTransform.cs with 11 types
  - MappingRule.ApplyTransform() working
  - Converters ready
Solution: Add Transform Editor Dialog
Features:
  - Multi-view preview
  - Encoding detection
  - Special char visualization
Learning: Check implementation phase code!
Status: ENHANCED in v0.7.26 ✅
```

### Fix #19: XAML Run Opacity (v0.7.26)
```yaml
Problem: Run element doesn't support Opacity
Error: MC3072
Solution: Use TextBlock instead of Run for opacity
Status: FIXED in v0.7.26 ✅
```

### Fix #20: PlaceholderText Property (v0.7.26)
```yaml
Problem: PlaceholderText not found
Solution: Use ui:ControlHelper.PlaceholderText
Framework: ModernWpfUI pattern
Status: FIXED in v0.7.26 ✅
```

### Fix #21: ShowDialog vs ShowAsync (v0.7.26)
```yaml
Problem: Wrong dialog show method for different types
Solution:
  - Window → ShowDialog()
  - ContentDialog → ShowAsync()
Example:
  DicomTagBrowserDialog (Window) → ShowDialog()
  TransformEditorDialog (ContentDialog) → ShowAsync()
Status: FIXED in v0.7.26 ✅
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
git tag v0.7.26
git push --tags
```

### Version Management (Version.props)
```xml
<!-- Single source of truth for versions -->
<PropertyGroup>
  <VersionPrefix>0.7.26</VersionPrefix>
  <FileVersion>0.7.26.0</FileVersion>
  <AssemblyVersion>0.7.0.0</AssemblyVersion>
  <InformationalVersion>0.7.26 - Session 74 Transform Editor Enhanced</InformationalVersion>
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

## 📊 METRICS THAT MATTER

```yaml
Total LOC: 14,850+ (all by Claude!)
Interfaces: Started 12+ → Current 2 → Target 0
Build Time: 3min → 20sec (without ZIP)
Config Formats: 3 → 1 (V2 unified)
Warnings: ~140 (needs cleanup)
Deleted: 700+ LOC (Dead Letter + more)
Fixed: Port 5111 everywhere ✅
Version: 0.7.26 (dynamic now!)
Pipelines: Truly isolated! ✅
API Endpoints: 4/5 implemented
CORE FEATURE: JPEG→DICOM WORKING! ✅
CONFIG: Validates all enums ✅
ARCHITECTURE: Pipeline isolation! ✅
DASHBOARD: Shows service status & errors! ✅
NAVIGATION: Clean, no dropdowns! ✅
PIPELINE CONFIG: All converters working! ✅
BUILD STATUS: Clean ✅
MAPPING EDITOR: Fully functional! ✅
UI CLARITY: Cheat sheet removed! ✅
DICOM BROWSER: NEMA-compliant! ✅
TRANSFORM EDITOR: Professional preview! ✅
HIDDEN FEATURES: Transform system exposed! ✅
MULTI-VIEW: Normal/Special/HEX views! ✅
```

## 📈 CURRENT STATUS & ROADMAP

### Current: v0.7.26 - Transform Editor Enhanced
```yaml
Done:
✅ Transform Editor Dialog created
✅ Multi-view preview working
✅ Encoding detection implemented
✅ Transform-aware test inputs
✅ DICOM compliance hints
✅ Special character visualization
✅ Hidden transform system exposed
✅ Professional medical software quality

Issues:
- Dead letter folder references remain
- Error management basic (folder only)
- ~140 build warnings
- Missing API endpoint (/api/statistics)
- 2 interfaces remain
- Encoding (© vs Â©) needs fixing
```

### Next: Treasure Hunt Sprint (v0.7.x)
```yaml
Goals:
- Search for more hidden features
- Check implementation phase code
- Expose any hidden functionality
- Document all discoveries

Questions to explore:
- What else did past-Claude implement?
- Any hidden validation logic?
- Unused but useful utilities?
- Configuration options not exposed?
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

### The Hidden Treasure Principle (NEW!)
```yaml
Before implementing new features:
1. Check if it already exists
2. Look in implementation files
3. Search for related keywords
4. Test existing methods
5. Only build if truly missing
→ Past-Claude was thorough!
```

### Decision Framework (UPDATED)
```yaml
Question: "Should we add this feature?"
1. Does it already exist hidden? ← NEW!
2. Does it solve a real problem? 
3. Is it the simplest solution?
4. Can we delete something instead?
5. What would Oliver say?
6. Does it add UI clutter?
→ If any "No" after #1: Don't do it
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
Converter Matching: Types must match!
Navigation Hiding: Sometimes removal is best!
Event Handler Connection: Check xaml.cs first!
UI Clarity: Remove clutter, add space!
NEMA Compliance: Always follow the standard!
Hidden Treasures: Check before building!
```

## 🔧 QUICK REFERENCE CARD

### Daily Commands
```powershell
0[TAB]   # Build
1[TAB]   # Deploy  
2[TAB]   # Config Tool
9[TAB]   # Test
4[TAB]   # Console Mode
7[TAB]   # Clean build
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

# UI shows wrong thing
# Check converter matches data type!

# Navigation annoyance
# NavigationUIVisibility="Hidden"

# Drag & Drop not working
# Check event handlers in xaml.cs!

# Build error for fixed code
# Clean build cache completely:
Remove-Item -Recurse -Force */obj, */bin

# UI element takes too much space
# Ask: "brauchen wir das?" → Maybe DELETE!

# XAML property not supported
# Check element type - Run vs TextBlock!

# Dialog won't show
# Window → ShowDialog(), ContentDialog → ShowAsync()
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
- Type-matched converters (v0.7.22!)
- Navigation hiding (v0.7.24!)
- Event handler patterns (v0.7.25!)
- UI clarity through removal (v0.7.25!)
- Transform system UI exposure (v0.7.26!)
- Multi-view preview pattern (v0.7.26!)

### What Needs Work
- Encoding (© vs Â©)
- Too many warnings (~140)
- Missing API endpoint
- Documentation gaps
- Some architectural debt remains
- Dead letter references need removal

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
10. **Match converters to data types** - Session 70 wisdom!
11. **Hide unwanted UI elements** - Session 71 victory!
12. **Check event handlers first** - Session 72 approach!
13. **Remove UI clutter** - Session 73 clarity!
14. **XAML has limits** - Not all properties on all elements!
15. **Hidden treasures exist** - Check before building new!

---

*"Making the improbable reliably simple - and finding what's already there!"*
*Technical reference for CamBridge v0.7.26*
*Session 74: Transform Editor Complete - Hidden treasures revealed!*

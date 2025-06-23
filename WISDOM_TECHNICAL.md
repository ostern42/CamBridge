# WISDOM_TECHNICAL.md - Technical Wisdom Distilled
**Version**: 0.7.32  
**Purpose**: HOW to build, fix, deploy, activate hidden features  
**Philosophy**: KISS, Tab-Complete, Sources First, Pipeline Isolation  
**Reality**: 87 Sessions + DICOM Pipeline Complete!
**Updated**: 2025-06-23 Sessions 85-87 - From "no files" to "DICOM viewers work!"

## 🎭 V.O.G.O.N. PROTOCOL

### Framework for Complex Tasks
```yaml
VOGON INIT: Start focused work
  V - Verify: What exists? What's the real problem?
  O - Organize: Structure approach, gather tools
  G - Generate: Create/modify code systematically
  O - Observe: Test every change immediately
  N - Next: Document and plan next steps

VOGON EXIT: End session properly
  V - Validate: All changes working?
  O - Organize: Update docs, clean up
  G - Generate: Commit message, notes
  O - Observe: What's left to do?
  N - Next: Clear action items

Usage Examples:
  "VOGON INIT für Pipeline Isolation"
  "Mini-VOGON für Event Handler check"
  "VOGON EXIT mit v0.7.32 release"

NEW - VOGON TREASURE HUNT:
  V - Verify: Search for TODOs and comments
  O - Organize: List all findings
  G - Generate: Test implementations
  O - Observe: Find complete features
  N - Next: Create activation plan
```

## 🔧 TECHNICAL STACK

```yaml
Platform: Windows Service + WPF Config Tool
Framework: .NET 8.0, C# 12
GUI: WPF + ModernWpfUI 0.9.6 + MVVM Toolkit
Service: ASP.NET Core Minimal API (port 5111!)
DICOM: fo-dicom 5.2.2 (medical imaging standard)
EXIF: ExifTool 13.30 (camera metadata)
Channels: System.Threading.Channels (async queues)
Testing: PowerShell Tab-Complete scripts
Build: MSBuild with Version.props
Deploy: PowerShell automation
Dependencies: Direct (2 interfaces from 12+)
Architecture: Pipeline-isolated processing
Logging: Serilog with pipeline-specific logs ✓

Hidden Systems Found:
  Notifications: Webhook + Email config ready
  Service Mgmt: Install/Uninstall complete ✓
  Monitoring: Health checks active
  Import/Export: JSON fully working ✓
  Auto-Backup: Silent on every save ✓
  ProcessingQueue: HashSet deduplication ✓ (Session 87!)
  
Working Features (Sessions 85-87):
  JPEG Detection: FileSystemWatcher ✓
  EXIF Extraction: With prefix handling ✓
  DICOM Creation: Full pipeline ✓
  UID Generation: Compliant format ✓
  Transfer Syntax: Proper JPEG encoding ✓
  Viewer Support: MicroDicom opens files! ✓
```

## 🌍 UNICODE & INTERNATIONALIZATION

### Pipeline Naming Strategy
```yaml
GUI Support: Full Unicode everywhere
  - Pipeline names: "Radiologie-Süd", "小児科", "Кардиология"
  - Display: All Unicode characters supported
  - Config storage: UTF-8 JSON

File System Mapping:
  Original: "A&E / Emergency"
  Sanitized: "A_E_Emergency"
  Filename: "pipeline_A_E_Emergency_20250621.log"
  
  Original: "Radiologie-Süd"
  Sanitized: "Radiologie-Sued" (filesystem dependent)
  Filename: "pipeline_Radiologie-Sued_20250621.log"

Sanitization Rules:
  - Replace Path.GetInvalidFileNameChars()
  - Replace: / \ : * ? " < > | , . (space)
  - With: _ (underscore)
  - Max length: 100 chars (truncate + ...)
  - Preserve Unicode if filesystem supports
  
Mapping Storage:
  PipelineId ↔ OriginalName ↔ SanitizedName
  Stored in: PipelineLogMapping.json
  Used by: LogViewer for display
```

### Code Example
```csharp
private string SanitizeForFileName(string pipelineName)
{
    var invalid = Path.GetInvalidFileNameChars()
        .Concat(new[] { ' ', '.', ',' }).ToArray();
    
    var sanitized = string.Join("_", pipelineName.Split(invalid));
    
    if (sanitized.Length > 100)
        sanitized = sanitized.Substring(0, 97) + "...";
        
    return sanitized;
}
```

## 📊 LOGGING ARCHITECTURE

### Multi-Pipeline Logging Design
```yaml
File Structure:
  %ProgramData%\CamBridge\Logs\
    ├── service_20250621.log          # Global service log
    ├── pipeline_Radiology_20250621.log
    ├── pipeline_Emergency_20250621.log
    └── pipeline_小児科_20250621.log   # Unicode if supported

Log Routing:
  - ALWAYS: Separate file per pipeline
  - ALWAYS: Global service log
  - OPTIONAL: Additional custom path (GUI setting)
  
Serilog Configuration:
  - Dynamic sub-loggers per pipeline
  - SourceContext: "Pipeline.{SanitizedName}"
  - Automatic file creation on startup
  - Rolling by day
  
Performance:
  - Max 100+ pipelines supported
  - Async logging with buffer
  - Shared file access enabled
  - 10MB max file size before rolling
```

### LogViewer Features
```yaml
Multi-Select Dropdown:
  ☑ Service (Global)
  ☑ Radiology
  ☐ Emergency  
  ☑ CardioMRT
  
Timestamp Strategy:
  Storage: Full precision (HH:mm:ss.fff)
  Display: Seconds only (HH:mm:ss)
  Sorting: By full timestamp + line number
  Result: Correct order even in bursts
  
Performance Optimizations:
  - Tail mode: Last 1000 lines
  - Buffer: Max 10,000 entries
  - Virtual scrolling
  - Lazy file loading
  - Debounced updates
  
Search & Filter:
  - Real-time text search
  - Log level filters (Debug/Info/Warn/Error/Critical)
  - Multi-file merge with sort
  - Export filtered results
```

## 💻 ESSENTIAL COMMANDS (MUSCLE MEMORY!)

### XAML Gotchas to Remember! 
```xml
<!-- WRONG - Run doesn't support Opacity! -->
<Run Text="•" Opacity="0.5"/>

<!-- RIGHT - Opacity goes on TextBlock -->
<TextBlock Opacity="0.5">
    <Run Text="•"/>
</TextBlock>

<!-- Remember: Run elements are INLINE text only! -->
<!-- No layout properties on Run! -->
```

### Tab-Complete Build System
```powershell
# THE HOLY TRINITY - memorize these!
0[TAB]  # Build only (no ZIP) - 20 seconds
1[TAB]  # Deploy & Start Service - full cycle
9[TAB]  # Test without build - quick check

# Extended commands when needed
00[TAB] # Build WITH ZIP - for releases
11[TAB] # Deploy with backup
99[TAB] # Full test with build
2[TAB]  # Open Config Tool
4[TAB]  # Console mode - CRITICAL for debugging!
7[TAB]  # Clean build artifacts
h[TAB]  # Help - when you forget

# Real usage pattern
git pull        # Always sync first
0[TAB]          # Build changes
1[TAB]          # Deploy to service
9[TAB]          # Quick test
2[TAB]          # Open UI if needed
```

### Service Management Deep Dive
```powershell
# Service control basics
Stop-Service CamBridgeService -Force     # Stop hung service
Start-Service CamBridgeService           # Normal start
Restart-Service CamBridgeService         # Quick restart

# Service diagnostics
Get-Service CamBridgeService | Format-List *
# Shows: Status, StartType, DependentServices, ServicesDependedOn

# When service won't start - DEBUG PATTERN
# 1. Check if port is in use
netstat -ano | findstr :5111
# 2. Check Event Log (most important!)
Get-EventLog -LogName Application -Source CamBridge* -Newest 20 | 
    Select-Object TimeGenerated, EntryType, Message | 
    Format-Table -AutoSize
# 3. Run in console mode to see live errors
.\4-console.ps1  # Shows real-time output!

# NEW - Check pipeline-specific logs
Get-ChildItem "$env:ProgramData\CamBridge\logs\pipeline_*.log" | 
    Select-Object Name, LastWriteTime, Length

# Monitor logs live (with multi-select)
$logs = @("service", "pipeline_Radiology", "pipeline_Emergency")
Get-Content ($logs | %{"$env:ProgramData\CamBridge\logs\$($_)_$(Get-Date -f yyyyMMdd).log"}) -Tail 50 -Wait

# Check configuration
Test-Json -Path "$env:ProgramData\CamBridge\appsettings.json"
cat "$env:ProgramData\CamBridge\appsettings.json" | ConvertFrom-Json
```

### API Testing Arsenal
```powershell
# Quick API health check
Invoke-RestMethod "http://localhost:5111/api/status" | ConvertTo-Json -Depth 5

# Specific endpoints (all working in v0.7.32)
$base = "http://localhost:5111/api"
irm "$base/status"          # Full service status with pipelines
irm "$base/pipelines"       # Just pipeline configs
irm "$base/status/version"  # Version string only
irm "$base/status/health"   # Simple health check

# HIDDEN ENDPOINT (DISCOVERED & DOCUMENTED!)
# Returns detailed pipeline info including queue depth!
$pipelineId = "your-pipeline-guid-here"
irm "$base/pipelines/$pipelineId"  # Detailed pipeline info!
# Returns: Name, Status, QueueDepth, LastProcessed, ErrorCount, etc.

# Check specific pipeline
$status = irm "$base/status"
$status.pipelines | Where-Object { $_.name -eq "Radiologie-Süd" }

# Test pipeline processing
$status.statistics          # Files processed, errors
$status.service.uptime      # How long running

# Debug API issues
# 1. Test raw response
Invoke-WebRequest "$base/status" -UseBasicParsing
# 2. Check response headers
(iwr "$base/status").Headers
# 3. Test from different context
Start-Job { irm "http://localhost:5111/api/status" } | Wait-Job | Receive-Job
```

### PowerShell Treasure Hunt Commands
```powershell
# Find all commented service registrations
Select-String "^\s*//.*services\.Add" src\**\*.cs

# Find unused public async methods (potential features)
Select-String "public async Task.*\(" src\**\*.cs | 
  Where-Object { $_ -match "ViewModel" }

# Find all TODO/HACK/FIXME comments
Select-String "(TODO|HACK|FIXME|NOTE)" src\**\*.cs -Context 2,2

# Find properties with no UI bindings
$props = Select-String "public.*{ get; set; }" src\**\ViewModels\*.cs
$bindings = Select-String "Binding.*}" src\**\*.xaml
# Compare lists for unused properties

# Find pipeline logging configuration
Select-String "UseCustomLogging|PipelineLog" src\**\*.cs
Select-String "Logging" src\**\*.xaml | Where {$_ -match "Tab"}

# NEW - Find Unicode in configs
Select-String "[^\x00-\x7F]" *.json -Encoding UTF8

# EXIF Debugging (Session 87!)
.\Tools\exiftool.exe -j -a -G1 -s "image.jpg" | ConvertFrom-Json | 
    Select-Object -ExpandProperty 0 | Get-Member -MemberType NoteProperty

# Find exact property names
Get-Content src\CamBridge.Core\Entities\ImageTechnicalData.cs | 
    Select-String "public.*\?.*{.*get" | 
    ForEach { $_ -replace '.*public\s+(\w+\??)\s+(\w+).*', '$1 $2' }
```

## 🎯 PATTERN MASTERY - FROM PAIN TO WISDOM

### The Multi-Pipeline Logging Pattern ⭐ NEW!
```yaml
Problem: One log file for all pipelines = chaos
Old Way: Everything in service-{date}.log
New Way: Automatic per-pipeline logs

Implementation:
  1. Each pipeline gets own logger with SourceContext
  2. Serilog routes by SourceContext to files
  3. LogViewer reads all pipeline_*.log files
  4. Multi-select dropdown for mixing
  
Real Example:
```
```csharp
// In PipelineManager:
var pipelineLogger = _loggerFactory.CreateLogger($"Pipeline.{SanitizeName(config.Name)}");

// Serilog routes automatically:
// Pipeline.Radiology → pipeline_Radiology_20250621.log
// Pipeline.Emergency → pipeline_Emergency_20250621.log

// LogViewer:
☑ Service (Global)     [14:23:45 INF] Service started
☑ Radiology            [14:23:46 INF] Pipeline Radiology started
☐ Emergency            
☑ CardioMRT           [14:23:47 INF] Pipeline CardioMRT started

// Result: Mixed view sorted by precise timestamp!
```

### The EXIF Key Fallback Pattern ⭐ (Session 87!)
```csharp
// Problem: ExifTool with -G1 adds prefixes to keys
// Solution: Check multiple variants

// ALWAYS check with and without prefix
var barcodeData = exifData.TryGetValue("RMETA:Barcode", out var data) || 
                  exifData.TryGetValue("Barcode", out data) 
                  ? data : null;

var width = GetIntValue(exifData, 
    "File:ImageWidth",      // Primary with prefix
    "ExifIFD:ExifImageWidth", // Alternative with prefix
    "ImageWidth",           // Fallback without prefix
    "ExifImageWidth"        // Alternative without prefix
);
```

### The Property Name Verification Pattern ⭐ (Session 87!)
```csharp
// ALWAYS verify exact property names before using!
// ImageTechnicalData has:
//   - ImageWidth (NOT Width!)
//   - ImageHeight (NOT Height!)
//   - Manufacturer (NOT CameraManufacturer!)

// WRONG:
technicalData.Width = width;  // CS0117: no definition for 'Width'

// RIGHT:
technicalData.ImageWidth = width;  // ✅
```

### The DICOM UID Generation Pattern ⭐ (Sessions 85-86!)
```csharp
// Problem: UIDs with hex chars or too long
// Solution: Strict compliance

// Rules:
// 1. Only digits (0-9) and dots (.)
// 2. Max 64 characters total
// 3. No negative numbers
// 4. Must be globally unique

private string GenerateUID()
{
    var shortTicks = (DateTime.UtcNow.Ticks % 10000000000).ToString();
    var processId = (Environment.ProcessId % 10000).ToString();
    var random = new Random().Next(1000, 9999);
    var uid = $"{IMPLEMENTATION_CLASS_UID}.{shortTicks}.{processId}.{random}";
    
    if (uid.Length > 64)
    {
        _logger.LogWarning("UID too long, truncating: {UID}", uid);
        uid = uid.Substring(0, 64);
    }
        
    return uid;
}
```

### The DICOM Dataset Creation Pattern ⭐⭐ (Session 86!)
```csharp
// CRITICAL: Dataset creation determines pixel data encoding!

// WRONG: Create dataset, then set transfer syntax later
var dataset = new DicomDataset();  // ❌ Results in explicit length!
// ... add tags ...
// Later: dicomFile.FileMetaInfo.TransferSyntax = ...

// RIGHT: Create dataset WITH transfer syntax
var dataset = new DicomDataset(DicomTransferSyntax.JPEGProcess1);  // ✅
// fo-dicom knows from start: This will be JPEG compressed!
// Automatically uses undefined length for pixel data
```

### The DI Registration with Config Pattern ⭐ (Session 87!)
```csharp
// When constructor needs config values:
services.AddSingleton<ExifToolReader>(sp =>
{
    var settings = sp.GetRequiredService<IOptionsMonitor<CamBridgeSettingsV2>>()
        .CurrentValue;
    return new ExifToolReader(
        sp.GetRequiredService<ILogger<ExifToolReader>>(),
        settings.ExifToolPath ?? "Tools\\exiftool.exe"
    );
});
```

### The Unicode Filename Pattern ⭐ NEW!
```csharp
// Problem: "Radiologie-Süd/Test" crashes file system
// Solution: Smart sanitization with mapping

public class PipelineLogMapper
{
    private Dictionary<Guid, (string Original, string Sanitized)> _mappings;
    
    public string GetLogFileName(PipelineConfiguration config)
    {
        var sanitized = SanitizeForFileName(config.Name);
        _mappings[config.Id] = (config.Name, sanitized);
        return $"pipeline_{sanitized}_{DateTime.Now:yyyyMMdd}.log";
    }
    
    public string GetOriginalName(string logFileName)
    {
        // Extract sanitized name from filename
        var match = Regex.Match(logFileName, @"pipeline_(.+)_\d{8}\.log");
        if (match.Success)
        {
            var sanitized = match.Groups[1].Value;
            return _mappings.Values
                .FirstOrDefault(m => m.Sanitized == sanitized)
                .Original ?? sanitized;
        }
        return logFileName;
    }
}

// Result: Full Unicode in UI, safe filenames on disk!
```

### The Timestamp Display Pattern ⭐ NEW!
```csharp
// Problem: Milliseconds clutter the display
// But: Need MS precision for correct ordering
// Solution: Store full, display simple

public class LogEntry
{
    public DateTime TimestampPrecise { get; set; }  // 14:23:45.123
    public string TimestampDisplay => 
        TimestampPrecise.ToString("HH:mm:ss");      // 14:23:45
        
    // Sorting uses full precision:
    entries.OrderBy(e => e.TimestampPrecise)
           .ThenBy(e => e.SourceFile)
           .ThenBy(e => e.LineNumber);
           
    // Display shows clean format:
    // [14:23:45 INF] Message (not [14:23:45.123 INF])
}

// Result: Clean display, correct order!
```

### The Hidden Treasures Pattern ⭐ POWERFUL!
```yaml
Problem: User needs feature X
Old Way: Design and build from scratch (2 weeks)
New Way: Check if already implemented (2 hours)

Discovery Process:
  1. Search keywords in solution
  2. Check enums (they list capabilities)
  3. Look for switch statements
  4. Find existing methods
  5. Often just needs UI!

Real Example - ProcessingQueue Deduplication (Session 87):
  User: "Files processed multiple times!"
  Me: "Let me check ProcessingQueue..."
  
  Found in ProcessingQueue.cs:
    - HashSet<string> _processedFiles ✓
    - HashSet<string> _enqueuedFiles ✓
    - Complete deduplication logic ✓
    
  Missing: Nothing! Already implemented!
  
  Result: 0 hours instead of rewriting!
```

### The Minimal Pattern ⭐ POWERFUL!
```csharp
// When complex debugging fails after 3+ hours
// Signal: "ich werde bald wahnsinnig"
// Solution: DELETE complexity, write minimal

// BEFORE - LogViewer with complex loading (not working)
public class LogViewerViewModel : ViewModelBase
{
    private readonly ILogService _logService;
    private readonly IFileWatcher _fileWatcher;
    private readonly ILogParser _parser;
    
    public async Task InitializeAsync()
    {
        try
        {
            await _logService.InitializeAsync();
            var config = await _logService.GetConfigAsync();
            await LoadLogsAsync(config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize");
        }
    }
    // ... 300+ lines of "proper" code
}

// AFTER - Direct and simple (WORKING!)
public class LogViewerViewModel : ViewModelBase
{
    private FileSystemWatcher? _watcher;
    private long _lastPosition;
    
    public async Task LoadLogFileAsync(string path)
    {
        try
        {
            // Just read the damn file!
            var lines = await File.ReadAllLinesAsync(path);
            var recent = lines.TakeLast(1000);
            
            foreach (var line in recent)
            {
                LogEntries.Add(ParseLine(line));
            }
        }
        catch { /* Silent fail */ }
    }
}
// Result: Working in 30 minutes!
```

### Pipeline Isolation Pattern ⭐ CRITICAL FOR MEDICAL!
```csharp
// WRONG - Shared logger for all pipelines
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<ILogger<FileProcessor>>(); // SHARED!
}

// WRONG - One log file for everything
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("log.txt")
    .CreateLogger();

// RIGHT - Per-pipeline loggers and files (Session 75)
public class PipelineManager
{
    public async Task StartPipelineAsync(PipelineConfiguration config)
    {
        // Each pipeline gets OWN logger
        var pipelineLogger = _loggerFactory.CreateLogger(
            $"Pipeline.{SanitizeName(config.Name)}");
        
        // Automatic routing to separate file
        // via Serilog SourceContext filtering
        
        var fileProcessor = new FileProcessor(
            pipelineLogger,           // Pipeline's own logger!
            _exifToolReader,
            _dicomConverter,
            config,
            _globalDicomSettings
        );
        
        // Completely isolated pipeline with own log!
        _pipelines[config.Id] = new PipelineContext(
            fileProcessor, queue, watcher, pipelineLogger);
    }
}

// Learning: Medical data isolation extends to logs!
// Radiology logs shouldn't mix with Emergency logs
```

## 🏴‍☠️ HIDDEN FEATURE ACTIVATION COOKBOOK

### 🎉 SPRINT 18 ACTIVATIONS (v0.7.27)

#### ❌ Daily Summary Service - INCOMPATIBLE!
```csharp
// Location: src\CamBridge.Service\Program.cs:232
// Status: CANNOT BE ACTIVATED - Architectural mismatch!
// services.AddHostedService<DailySummaryService>();  // DO NOT UNCOMMENT!

// PROBLEM DISCOVERED:
// - DailySummaryService expects singleton ProcessingQueue
// - But ProcessingQueue is created per-pipeline (Session 68!)
// - Service fails with: Unable to resolve ProcessingQueue
// 
// NEEDS REFACTORING:
// - Change to use PipelineManager instead
// - Aggregate stats from all pipelines
// - Or create separate summary service per pipeline
```

#### ✅ Hidden API Endpoint - DOCUMENTED!
```powershell
# GET /api/pipelines/{id} - Was hidden, now documented!
# Returns detailed pipeline info not available in /api/pipelines
$pipelineId = "your-pipeline-guid"
irm "http://localhost:5111/api/pipelines/$pipelineId"

# Returns more detail than the list endpoint:
# - Queue depth
# - Last processed timestamp
# - Error details
# - Processing statistics
```

#### ✅ Auto-Backup Status - MADE VISIBLE!
```yaml
Location: ConfigurationService already creates backups!
What we added:
  - Info panel in PipelineConfigPage.xaml
  - Updated save message with timestamp
  - Shows backup location to users
  
User value:
  - Confidence in data safety
  - Know where backups are stored
  - See confirmation on every save
  
Backup pattern: appsettings.json.backup_20250619_143022
```

#### ✅ Import/Export Mappings - MADE PROMINENT!
```yaml
Status: Was hidden in UI, now prominent toolbar feature
What we added:
  - Enhanced buttons with icons
  - Keyboard shortcuts (Ctrl+I, Ctrl+E)
  - "Share mappings" helper text
  - Sample mapping JSON file
  
User value:
  - Share configs between departments
  - Backup mapping rules
  - Quick deployment to multiple sites
  - Template-based setup
  
File format: JSON with full rule details
Location: %ProgramData%\CamBridge\Mappings\
```

#### ✅ Service Installer UI - ACTIVATED!
```yaml
Status: Backend complete, UI buttons added!
Location: ServiceControlPage.xaml
What we added:
  - Install Service button (when not installed)
  - Uninstall Service button (when installed)
  - Admin elevation handled automatically
  - Auto-recovery pre-configured
  
Backend magic:
  - ServiceManager.InstallServiceAsync()
  - ServiceManager.UninstallServiceAsync()
  - Recovery: Restart on failure (3 times)
  - All enterprise-ready!
  
User value:
  - No more command line needed!
  - Visual installation status
  - One-click install/uninstall
  - Recovery configured automatically
```

### 🎯 Session 87 Discovery
```yaml
ProcessingQueue Deduplication:
  ✅ HashSet tracking already implemented
  ✅ Prevents duplicate file processing
  ✅ Handles FileSystemWatcher multi-events
  ✅ No changes needed - just works!
  
Discovery Process:
  1. User reported retry spam
  2. Searched for "duplicate", "processed"
  3. Found complete implementation
  4. Just needed to understand it!
```

### 🎯 Sprint 19 Features (NEW!)
```yaml
Multi-Pipeline Logging:
  ✅ Separate log file per pipeline
  ✅ Automatic file creation
  ✅ Unicode pipeline name support
  ✅ LogViewer multi-select
  ✅ Timestamp precision handling
  
Still TODO:
  ⏳ Pipeline-specific log settings (UI exists!)
  ⏳ Custom log path configuration
  ⏳ Log retention per pipeline
  ⏳ Remote log viewing API
```

### 🚫 FALSE TREASURES - Complete but Incompatible

#### Daily Summary Service
```yaml
Location: DailySummaryService.cs
Status: Fully implemented but architecturally incompatible
Problem: Expects singleton ProcessingQueue
Reality: ProcessingQueue is per-pipeline (Session 68)
Fix needed: Refactor to use PipelineManager
Time to fix: 2-4 hours
Lesson: Not all "complete" code can be activated!
```

#### Pipeline Logging Settings
```yaml
Location: PipelineConfigPage.xaml → Logging tab
Status: Complete UI, no backend connection
Problem: Properties exist but not saved/used
Reality: Just UI theater currently
Fix needed: Wire to PipelineConfiguration
Time to fix: 1-2 hours
Lesson: Always check the full stack!
```

## 🐛 CRITICAL FIXES - CATEGORIZED WISDOM

### Category 1: Configuration Gotchas
```yaml
Fix #1: Port Consistency
  Symptom: Dashboard empty, API not responding
  Debug Time: 3 sessions (Session 58-61)
  Root Cause: Service on 5050, Config expects 5111
  Fix: Global replace all 5050 → 5111
  
  # How to check:
  netstat -ano | findstr :5050
  netstat -ano | findstr :5111
  
  Learning: ONE PORT EVERYWHERE!

Fix #2: Config Wrapper Required
  Symptom: Service crashes on startup
  Error: "CamBridge configuration section not found"
  Root Cause: Missing wrapper in JSON
  
  WRONG:
  {
    "Version": "2.0",
    "Service": { }
  }
  
  RIGHT:
  {
    "CamBridge": {  // WRAPPER REQUIRED!
      "Version": "2.0",
      "Service": { }
    }
  }
  
  Learning: Always wrap with "CamBridge"

Fix #3: Enum Validation Saves Lives
  Symptom: Service crashes with cryptic error
  Debug: "The JSON value could not be converted"
  Root Cause: Invalid enum value in config
  
  Valid OutputOrganization: None, ByPatient, ByDate, ByPatientAndDate
  NOT: PatientName, Date, Patient (old values)
```

### Category 2: Architecture Evolution
```yaml
Fix #7-8: The Great Interface Purge
  Journey: 12+ interfaces → 2 interfaces
  Deleted: ~2000 lines of abstraction
  
  Learning: Interfaces without multiple implementations = DELETE

Fix #8: Pipeline Isolation Architecture
  Problem: FileProcessor was singleton but needs pipeline config
  Medical Risk: Department A seeing Department B data
  
  Solution: Each pipeline creates own FileProcessor
  Extension: Each pipeline gets own logger (Sprint 19)
  
  Key Insight: Isolation must be complete:
    - Separate FileProcessor ✓
    - Separate Queue ✓
    - Separate Watcher ✓
    - Separate Logger ✓ (NEW!)
    - Separate Log Files ✓ (NEW!)
```

### Category 3: The Dashboard Debugging Saga
```yaml
Fix #9-12: Why Dashboard Wouldn't Work
  Total Debug Time: 3+ hours
  Frustration Level: "ich werde bald wahnsinnig"
  
  Ultimate Learning: When one page works and another doesn't,
                   copy what works! Complex isn't better.
```

### Category 4: XAML/WPF Specific Gotchas
```yaml
Fix #13-14: Converter & Navigation Issues
  
  Converter Type Mismatch:
    Problem: "No Pipeline Selected" showing when selected
    XAML: Converter={StaticResource InverseBoolToVisibility}
    Binding: SelectedPipeline (object, not bool!)
    
    Fix: Use correct converter for type
    Converter={StaticResource NullToVisibility}
    ConverterParameter=Inverse

Fix #17-21: XAML Property Support
  Run elements don't support Opacity:
    WRONG: <Run Text="→" Opacity="0.7"/>
    RIGHT: Put Opacity on TextBlock, not Run!
```

### Category 5: Unicode & Internationalization (NEW!)
```yaml
Fix #26: Pipeline Names with Special Characters
  Issue: "Radiologie-Süd" crashes file creation
  Root Cause: Invalid filename characters
  
  Solution: Dual-name system
    Display: Full Unicode
    Filename: Sanitized ASCII
    Mapping: Stored in memory/config
  
  Code Pattern:
    config.Name: "Radiologie-Süd"
    Sanitized: "Radiologie-Sud"
    Filename: "pipeline_Radiologie-Sud_20250621.log"
    
Fix #27: Multi-Language Log Mixing
  Issue: Different encodings in same view
  Solution: All logs UTF-8, sorted by binary timestamp
  
Fix #28: Path Length Limits
  Issue: Long pipeline names exceed Windows 260 char limit
  Solution: Truncate sanitized names to 100 chars
```

### Category 6: DICOM Pipeline Fixes (Sessions 85-87) ⭐
```yaml
Fix #29: DICOM Relative Path Bug
  Symptom: "Source file not found" after successful processing
  Debug Time: Initially thought complex retry issue
  Root Cause: DICOM created with relative paths, service runs from System32
  
  PROBLEM: return dicomPath;  // "Schmidt, Maria\2025-06-23\file.dcm"
  SOLUTION: return Path.GetFullPath(dicomPath);  // "C:\CamBridge\Output\..."
  
  Learning: Windows Services working directory = System32!

Fix #30: UID Hex Characters
  Symptom: "does not validate VR UI" in DICOM validation
  Root Cause: UIDs contained hex characters (a-f) from GUID
  
  PROBLEM: guid.ToString("N").Substring(0, 8); // "bb7e1567"
  SOLUTION: Convert to numeric only using BitConverter
  
  Learning: DICOM UIDs = digits + dots ONLY!

Fix #31: UID Length Exceeded
  Symptom: UID validation error
  Root Cause: UIDs > 64 characters (was 69!)
  
  Solution: Shorter components with modulo
  Learning: Every DICOM rule is strict!

Fix #32: Transfer Syntax on FileMetaInfo
  Symptom: Transfer syntax not recognized
  Root Cause: Set on Dataset instead of FileMetaInfo
  
  WRONG: dataset.Add(DicomTag.TransferSyntaxUID, ...)
  RIGHT: dicomFile.FileMetaInfo.TransferSyntax = ...
  
  Learning: Location matters in DICOM!

Fix #33: Post-Processing Race Condition
  Symptom: "Source file not found" during retry
  Root Cause: SuccessAction "Archive" moves file before retry
  
  Solution: Set to "Leave" for testing
  Learning: Consider full flow, not just success path

Fix #34: JPEG Encapsulation Undefined Length ⭐⭐
  Symptom: "Found explicit length Pixel Data in compressed Transfer Syntax"
  Debug Time: 30 minutes with dcmdump
  Root Cause: Dataset created without transfer syntax
  
  WRONG: var dataset = new DicomDataset();
  RIGHT: var dataset = new DicomDataset(DicomTransferSyntax.JPEGProcess1);
  
  Learning: fo-dicom needs to know transfer syntax AT CREATION!

Fix #35: ImageTechnicalData Property Names ⭐
  Symptom: CS0117 "no definition for 'Width'"
  Debug Time: 45 minutes!
  Root Cause: Property is ImageWidth not Width
  
  WRONG: technicalData.Width = width;
  RIGHT: technicalData.ImageWidth = width;
  
  Learning: EXACT property names matter!

Fix #36: Immutable Domain Objects
  Symptom: Cannot use object initializers
  Root Cause: Domain objects have constructor-only init
  
  Solution: Use constructors with parameters
  Learning: Check how objects are designed!

Fix #37: ProcessingQueue Retry Spam
  Symptom: "Source file not found" 80 seconds later
  Root Cause: FileSystemWatcher fires multiple events
  
  Solution: Already implemented with HashSets!
  Learning: Check existing code first!

Fix #38: EXIF Barcode Not Detected
  Symptom: Patient shown as "Unknown Patient"
  Root Cause: ExifTool -G1 adds "RMETA:" prefix
  
  WRONG: exifData.TryGetValue("Barcode", ...)
  RIGHT: Check both "RMETA:Barcode" and "Barcode"
  
  Learning: ExifTool output format varies!

Fix #39: DICOM 0x0 Pixel Dimensions
  Symptom: DICOM viewers show error
  Root Cause: Multiple issues:
    1. ExifTool returns "File:ImageWidth"
    2. ImageTechnicalData has ImageWidth property
    3. Method GetIntValue needs this.
  
  Learning: Stack of small issues = big problem!

Fix #40: JSON Number Parsing
  Symptom: "element has type 'Number', expected 'String'"
  Root Cause: ExifTool JSON has numeric values
  
  Solution: Handle all JsonValueKind types
  Learning: Always handle all JSON types!
```

## 🏗️ BUILD & DEPLOY MASTERY

### Version Management - Single Source of Truth
```xml
<!-- Version.props - NEVER hardcode versions! -->
<Project>
  <PropertyGroup>
    <VersionPrefix>0.7.32</VersionPrefix>
    <FileVersion>0.7.32.0</FileVersion>
    <AssemblyVersion>0.7.0.0</AssemblyVersion>
    <InformationalVersion>0.7.32 - ProcessingQueue retry fix & EXIF key mapping</InformationalVersion>
  </PropertyGroup>
</Project>
```

### Dynamic Version Reading Pattern
```csharp
// OLD WAY (Session 1-60) - always outdated
public static class ServiceInfo
{
    public const string Version = "0.7.9"; // WRONG!
}

// NEW WAY (Session 61+) - always current
public static string Version
{
    get
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fileVersion = FileVersionInfo.GetVersionInfo(
            assembly.Location).FileVersion;
        
        if (!string.IsNullOrEmpty(fileVersion))
        {
            // Remove trailing .0 for cleaner display
            if (fileVersion.EndsWith(".0"))
                fileVersion = fileVersion[..^2];
            return fileVersion;
        }
        
        return "0.7.32"; // Emergency fallback only
    }
}
```

### Deployment Structure - Where Everything Lives
```yaml
Binaries & Tools:
  C:\CamBridge\
  ├── Service\          # Windows Service files
  │   ├── CamBridge.Service.exe
  │   ├── appsettings.json (template)
  │   └── *.dll
  ├── ConfigTool\       # WPF Config application
  │   ├── CamBridge.Config.exe
  │   └── *.dll
  └── Tools\            # External tools
      └── exiftool.exe  # Version 13.30

Configuration & Data:
  %ProgramData%\CamBridge\  # C:\ProgramData\CamBridge
  ├── appsettings.json      # PRIMARY CONFIG (V2 format)
  ├── Pipelines\            # Pipeline-specific configs
  │   ├── radiology.json
  │   └── emergency.json
  ├── Mappings\             # DICOM mapping rules
  │   └── default.json
  └── Logs\                 # Service logs
      ├── service_20250621.log
      ├── pipeline_Radiology_20250621.log
      └── pipeline_Emergency_20250621.log

Watch Folders (Input):
  C:\CamBridge\Watch\
  ├── Radiology\           # Pipeline 1 input
  └── Emergency\           # Pipeline 2 input

Output Folders:
  C:\CamBridge\Output\
  ├── Radiology\           # DICOM output
  │   ├── ByPatient\       # If organized
  │   └── ByDate\
  └── Emergency\

NEW - Log Structure:
  %ProgramData%\CamBridge\Logs\
  ├── service_20250621.log              # Global
  ├── pipeline_Radiology_20250621.log    # Auto-created
  ├── pipeline_Emergency_20250621.log    # Auto-created
  └── pipeline_小児科_20250621.log       # Unicode OK!
```

## 🔨 DEBUGGING TOOLKIT

### When Service Won't Start - Systematic Approach
```powershell
# 1. CHECK THE OBVIOUS FIRST!
# Is port already in use?
netstat -ano | findstr :5111
# If yes, find and kill process

# 2. Check Event Log (MOST IMPORTANT!)
Get-EventLog -LogName Application -Source CamBridge* -Newest 20 |
    Format-Table TimeGenerated, EntryType, Message -AutoSize

# 3. Validate configuration
$config = Get-Content "$env:ProgramData\CamBridge\appsettings.json"
$config | ConvertFrom-Json  # Will show parse errors

# 4. Run in console mode for live debugging
Set-Location "C:\CamBridge\Service"
.\CamBridge.Service.exe console

# 5. Check permissions (especially for logs)
icacls "C:\CamBridge\Watch" /T
icacls "$env:ProgramData\CamBridge" /T
icacls "$env:ProgramData\CamBridge\Logs" /T

# NEW - Check pipeline-specific logs
Get-ChildItem "$env:ProgramData\CamBridge\Logs\pipeline_*.log" |
    Select Name, Length, LastWriteTime |
    Format-Table -AutoSize

# NEW - Tail multiple logs simultaneously
$logs = @("service", "pipeline_Radiology", "pipeline_Emergency")
Get-Content ($logs | %{
    "$env:ProgramData\CamBridge\Logs\$($_)_$(Get-Date -f yyyyMMdd).log"
}) -Tail 20 -Wait
```

### When Logs Show Unicode Issues
```powershell
# Check file encoding
$file = "$env:ProgramData\CamBridge\Logs\pipeline_Radiologie-Süd_20250621.log"
$bytes = [System.IO.File]::ReadAllBytes($file) | Select -First 3
if ($bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
    "UTF-8 with BOM"
} else {
    "Check encoding!"
}

# Find problematic pipeline names
Get-ChildItem "$env:ProgramData\CamBridge\Logs\pipeline_*.log" |
    Where {$_.Name -match "[^\x00-\x7F]"} |
    Select Name

# Test sanitization
$name = "Radiologie-Süd/Test"
$sanitized = $name -replace '[\\/:*?"<>|,. ]', '_'
Write-Host "Original: $name"
Write-Host "Sanitized: $sanitized"
```

### DICOM Debugging (Sessions 85-87) ⭐
```powershell
# Check for UID errors in logs
Select-String "does not validate VR UI" -Path *.log

# Validate DICOM file structure
$dcm = Get-ChildItem -Path C:\CamBridge\Output -Filter *.dcm -Recurse | Select -First 1
if ($dcm) {
    Write-Host "Size: $($dcm.Length) bytes"
    # If > 50KB, likely has image data
    # If < 10KB, likely missing pixel data
    
    # Use dcmdump for details
    dcmdump $dcm.FullName 2>&1 | Select-String "Error|Warning"
    # No output = good!
    
    # Check transfer syntax
    dcmdump $dcm.FullName | Select-String "0002,0010"
    # Should show: (0002,0010) UI =JPEGBaseline
    
    # Check pixel data encoding
    dcmdump $dcm.FullName 2>&1 | Select-String "7FE0,0010"
    # Good: (7FE0,0010) OB (PixelSequence #=1)  # u/l
    # Bad:  (7FE0,0010) OB =PixelData           # explicit length!
}

# Quick EXIF debug
.\Tools\exiftool.exe -j -a -G1 -s "test.jpg" > exif-debug.json
cat exif-debug.json | ConvertFrom-Json | ConvertTo-Json -Depth 10

# Find specific EXIF field
.\Tools\exiftool.exe -j -a -G1 -s "test.jpg" | 
    ConvertFrom-Json | Select -ExpandProperty 0 | 
    Select "*Barcode*", "*Width*", "*Height*"
```

### LogViewer Performance Issues
```yaml
Symptom: Slow with many pipelines
Check:
  1. How many log files selected?
  2. Total lines across all files?
  3. Search pattern complexity?
  
Fix:
  1. Limit initial selection
  2. Use date filter
  3. Optimize regex patterns

PowerShell Test:
  # Count total lines
  $total = 0
  Get-ChildItem "$env:ProgramData\CamBridge\Logs\*.log" | % {
      $total += (Get-Content $_ | Measure-Object -Line).Lines
  }
  "Total lines: $total"
```

## 📊 METRICS & PATTERNS

### Evolution Metrics
```yaml
What Changed (v0.1 → v0.7.32):
  Interfaces: 12+ → 2 (-85%)
  Build Time: 3min → 20sec (-89%)  
  Debug Time: Hours → Minutes (minimal pattern)
  Code: 15,940 → 15,850 LOC (added logging)
  Complexity: High → Low (KISS wins)
  Features Hidden: Unknown → 30+ found!
  Features Activated: 15 total
  User Satisfaction: Frustrated → Happy
  Logging: 1 file → N files (isolated)
  DICOM Pipeline: Not working → Fully functional!

Key Victories:
  - Tab-Complete: 90% less typing
  - Pipeline Isolation: Medical safety
  - Hidden Features: Weeks saved
  - UI Simplification: Consistent design
  - Multi-Pipeline Logs: Debugging heaven
  - Unicode Support: International ready
  - DICOM Creation: Viewers work!

NEW - Sessions 85-87 Results:
  ✅ DICOM Pipeline: Complete end-to-end
  ✅ EXIF Extraction: Prefix handling mastered
  ✅ Property Mapping: Exact names documented
  ✅ ProcessingQueue: Dedup already there!
  ✅ Path Issues: All absolute now
  ✅ Transfer Syntax: Proper JPEG handling
  Sprint Value: Major milestone reached!
  Time: ~10 hours across 3 sessions
  Lesson: Incremental fixes work!
```

### Architecture Principles (Hard-Won)
```yaml
1. Direct > Abstract
   - Unless you have 2+ implementations
   - Interfaces add complexity
   
2. Pipeline Isolation Required
   - Medical data must be separated
   - No shared mutable state
   - Includes logging now!
   
3. Single Source of Truth
   - ConfigurationPaths for all paths
   - Version.props for version
   - Port 5111 everywhere
   
4. Error Handling Simplicity
   - Error folder > Dead letter queue
   - Log and move on
   - Let user retry manually
   
5. Delete > Fix
   - Less code = fewer bugs
   - Simpler = more maintainable
   - If unused, remove it
   
6. Hidden > New
   - Check existing code first
   - Features might be implemented
   - UI often lags behind

NEW Principles:
7. Unicode Everywhere (GUI)
   - Full support in display
   - Smart sanitization for files
   - Mapping preserves original
   
8. Logs Need Love Too
   - Separate files per context
   - Professional viewer essential
   - Performance matters
   
9. Timestamp Precision Matters
   - Store precise, display simple
   - Sorting needs full data
   - UI needs clean view

10. Exact Names Matter (Session 87!)
    - Property names are contracts
    - EXIF keys may have prefixes
    - Check sources for truth

11. Initialization Determines Behavior
    - DICOM dataset creation critical
    - Transfer syntax at creation time
    - Not fixable later!

12. Incremental Fixes Work
    - One bug at a time
    - Build-test-fix cycle
    - Each error is progress
```

## 🎯 QUICK REFERENCE CARD

### Daily Workflow Checklist
```powershell
# Morning Routine
git pull                    # Get latest
git status                  # Check state
0[TAB]                      # Build
1[TAB]                      # Deploy
9[TAB]                      # Quick test

# Check Logs
Get-ChildItem "$env:ProgramData\CamBridge\Logs\*.log" -Recurse |
    Where {$_.LastWriteTime -gt (Get-Date).AddHours(-1)} |
    Select Name, Length, LastWriteTime

# After Code Changes
0[TAB]                      # Build
9[TAB]                      # Test
git add -A                  # Stage
git commit -m "msg"         # Commit

# Debug Workflow
4[TAB]                      # Console mode
Get-EventLog ...            # Check errors
2[TAB]                      # Open UI
# Check LogViewer!           # NEW option!

# DICOM Pipeline Test (NEW!)
copy test.jpg C:\CamBridge\Watch\Radiology\
Start-Sleep -Seconds 10
$dcm = Get-ChildItem C:\CamBridge\Output -Filter *.dcm -Recurse | Select -First 1
Write-Host "DICOM created: $($dcm.FullName)"
# Open in MicroDicom!
```

### Common Issues → Quick Fixes
```yaml
"Service won't start":
  → Check port 5111
  → Check Event Log
  → Validate config JSON
  → Run console mode
  → Check log permissions (NEW!)

"Logs missing":
  → Check %ProgramData%\CamBridge\Logs
  → Verify pipeline names match
  → Check file permissions
  → Look for sanitized names

"Unicode pipeline name issues":
  → Check sanitized filename
  → Verify UTF-8 encoding
  → Look for mapping file
  → Test with ASCII name

"LogViewer slow":
  → Reduce selected pipelines
  → Clear old logs
  → Check file sizes
  → Disable auto-scroll

"Feature needed quickly":
  → Search existing code
  → Check for enums
  → Look for TODO comments
  → Check all tabs in UI!

"DICOM not created": ⭐ NEW!
  → Check logs for path
  → Verify absolute paths
  → Check Event Log
  → Run console mode

"Patient data missing": ⭐ NEW!
  → Check EXIF output format
  → Look for RMETA:Barcode
  → Verify property names
  → Test with exiftool directly

"DICOM viewers fail": ⭐ NEW!
  → Check file size (>50KB?)
  → Use dcmdump for validation
  → Check transfer syntax
  → Verify pixel data encoding
```

## 🧙 TECHNICAL MANTRAS

> "Check the obvious before the complex"  
*Port 5111? Path exists? JSON valid? Typo?*

> "When complex fails, go minimal"  
*Delete abstractions, write direct code*  
*50 lines that work > 500 that don't*

> "Sources First, fantasies never"  
*The code doesn't lie, your memory does*  
*Check what exists before inventing*

> "Pipeline isolation for medical safety"  
*Each department completely separate*  
*No data mixing, ever - including logs!*

> "Unicode in UI, ASCII in files"  
*Display beauty, store safely*  
*Map between them always*

> "Milliseconds sort, seconds display"  
*Precision where needed, clarity where shown*  
*Best of both worlds*

> "The best debugger is the delete key"  
*Less code = fewer places for bugs*  
*Simplicity is the ultimate sophistication*

**NEW MANTRAS:**

> "Logs tell the story"  
*But only if you can read them*  
*Professional tools for professional work*

> "Every pipeline deserves its own log"  
*Mixing contexts creates confusion*  
*Isolation brings clarity*

> "Test with Unicode early"  
*ASCII assumptions break globally*  
*小児科 is a valid pipeline name!*

> "Property names are contracts"  
*ImageWidth not Width*  
*Exact names or build errors!*

> "EXIF keys may have prefixes"  
*RMETA:Barcode not just Barcode*  
*Check all variants!*

> "Initialization determines behavior"  
*DICOM dataset creation critical*  
*Can't fix it later!*

> "Each error message is progress"  
*From UID errors to pixel data*  
*Incremental fixes win!*

## 🚀 WISDOM NOTES

### What Works (Proven in Production)
- Tab-Complete system (0[TAB] muscle memory)
- Single config path (no confusion)
- Pipeline isolation (medical safety + logs)
- Direct dependencies (clear, simple)
- Minimal pattern (when debugging fails)
- Hidden feature hunting (saves weeks)
- Error folders (simple > complex)
- Console mode debugging (see everything)
- Auto-backup on save (already active!)
- Health monitoring (runs in background)
- Service Installer UI (no more CLI!)
- Import/Export JSON (sharing configs)
- Multi-Pipeline Logs (clarity in chaos)
- Unicode Support (global ready)
- Professional LogViewer (debugging heaven)
- DICOM Pipeline (fully functional!) ⭐
- ProcessingQueue Dedup (already there!)
- EXIF Prefix Handling (RMETA: etc.)

### Hard-Won Lessons
1. **One wrong port = 3 sessions debugging**
2. **InitializeAsync patterns = timing hell**
3. **Too many interfaces = lost in own code**
4. **User frustration = signal to simplify**
5. **XAML has specific rules**
6. **Past code often better than memory**
7. **Implementation phase was thorough**
8. **Activation is surprisingly easy**
9. **Not all complete code works**
10. **UI polish matters**
11. **Logs need professional tools**
12. **Unicode breaks assumptions**
13. **Isolation must be complete**
14. **Property names must be exact!** (Session 87)
15. **EXIF output varies by flags** (Session 87)
16. **Relative paths + services = trouble** (Session 85)
17. **DICOM rules are strict** (Sessions 85-86)
18. **Dataset creation critical** (Session 86)
19. **Incremental fixes work** (Sessions 85-87)

### The Technical Evolution
```yaml
Started with: "Interfaces make code professional"
Learned that: "Direct dependencies make code clear"
Discovered: "Hidden implementations save time"
Now know: "Complete isolation includes logs"
Latest: "Unicode requires dual-name strategy"
Session 85: "Paths must be absolute in services"
Session 86: "DICOM initialization determines all"
Session 87: "Exact names prevent 45-minute bugs"

Key Realization:
  Good architecture isn't about patterns
  It's about solving the actual problem
  With the least complexity possible
  While maintaining safety (medical!)
  And supporting global users!
  And being exact with details!
  
NEW Realization:
  Professional debugging tools matter
  Logs are first-class citizens
  Performance affects usability
  Unicode is not optional
  Multi-select is powerful!
  Property names are critical!
  EXIF keys need careful handling!
  DICOM compliance is strict!
  
LATEST Realization:
  Every bug teaches something valuable
  Incremental progress beats big bangs
  The pipeline finally works!
```

---

*Technical wisdom from 87 sessions of building, breaking, and fixing*  
*Plus Sessions 85-87's DICOM pipeline completion!*  
*Remember: The best code is code you don't have to write!*  
*The second best is code you can delete!*  
*The third best is code that logs properly!*  
*The fourth best is code with exact property names!* 🗑️💎📊✨

*Complete Technical Reference - June 2025 - v0.7.32*

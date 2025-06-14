# WISDOM_TECHNICAL.md (Compressed)
**Version**: 0.7.13  
**Purpose**: Technical implementation wisdom  
**Focus**: Patterns, Tools, Solutions, VOGON Protocol

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
"VOGON INIT für Dashboard Fix"
"Mini-VOGON für Config check"
"VOGON EXIT mit v0.7.13 release"
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
```

## 💻 ESSENTIAL COMMANDS

### Tab-Complete Build System
```powershell
# Core commands (memorize these!)
0[TAB]  # Build only (no ZIP) - 20 seconds
1[TAB]  # Deploy & Start Service
2[TAB]  # Open Config Tool  
9[TAB]  # Test without build
h[TAB]  # Show help

# Advanced
00[TAB] # Build WITH ZIP
11[TAB] # Deploy with backup
99[TAB] # Full test with build
```

### Service Management
```powershell
# Status & Control
Get-Service CamBridgeService | Format-List *
Stop-Service CamBridgeService -Force
Start-Service CamBridgeService
Restart-Service CamBridgeService

# Logs & Debugging
Get-EventLog -LogName Application -Source CamBridge* -Newest 20
Get-Content "$env:ProgramData\CamBridge\logs\*.log" -Tail 50 -Wait
```

### API Testing
```powershell
# Quick health check
Invoke-RestMethod "http://localhost:5111/api/status/version"

# Pipeline status
Invoke-RestMethod "http://localhost:5111/api/pipelines"

# Statistics
Invoke-RestMethod "http://localhost:5111/api/statistics"
```

## 🎯 PROVEN PATTERNS

### Configuration Management
```csharp
// SINGLE SOURCE OF TRUTH
public static class ConfigurationPaths
{
    public static string GetPrimaryConfigPath() =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "CamBridge", 
            "appsettings.json");
}
```

### Service Registration (KISS)
```csharp
// NO MORE INTERFACES!
services.AddSingleton<ExifToolReader>();
services.AddSingleton<DicomConverter>();
services.AddSingleton<FileProcessor>();
services.AddSingleton<PipelineManager>();
```

### Error Handling
```csharp
try 
{
    await ProcessFileAsync(file);
}
catch (Exception ex) when (ex is not OperationCanceledException)
{
    _logger.LogError(ex, "Failed: {File}", file);
    MoveToErrorFolder(file);  // Simple!
}
```

### Pipeline Pattern
```csharp
// Each pipeline gets its own watcher & queue
foreach (var pipeline in EnabledPipelines)
{
    var watcher = new FileWatcher(pipeline.WatchFolder);
    var queue = new Channel<string>(100);
    // Independent processing
}
```

## 🐛 CRITICAL FIXES REFERENCE

### Fix #1: Port Consistency
```yaml
Problem: Service on 5050, Config expects 5111
Fix: Global replace 5050 → 5111
Files: Program.cs, appsettings.json, ViewModels
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

## 🔨 ESSENTIAL TOOLS

### Development
- Visual Studio 2022 (v17.8+)
- .NET 8 SDK
- PowerShell 7+
- Git for Windows

### Debugging
- Event Viewer (eventvwr.msc)
- Services Manager (services.msc)
- Process Monitor (ProcMon)
- Postman/Insomnia (API testing)

### Scripts
```
Build-CamBridge.ps1      # Tab-complete builder
Test-CamBridge.ps1       # Quick tester
Get-WisdomSources.ps1    # Source extractor
Debug-CamBridgeJson.ps1  # Config validator
```

## 🏗️ BUILD & DEPLOY

### Quick Build
```powershell
# Development cycle
0[TAB]  # Build (20 sec)
1[TAB]  # Deploy
9[TAB]  # Test

# Full release
00[TAB] # Build with ZIP
```

### Version Management
```xml
<!-- Version.props -->
<PropertyGroup>
  <Version>0.7.13</Version>
  <FileVersion>0.7.13.0</FileVersion>
  <AssemblyVersion>0.7.13.0</AssemblyVersion>
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
├── appsettings.json  # Primary config
├── Pipelines\        # Pipeline configs
└── Logs\            # Runtime logs
```

## 📅 SESSION HISTORY (Distilled Learnings)

### Sessions 1-20: Architecture Explosion
- Started with Clean Architecture → 12+ interfaces
- **Learning**: SOLID ≠ Simple, overengineering kills

### Sessions 21-40: Config Chaos
- 3 config versions, multiple loaders, migration hell
- **Learning**: One format, one path, one truth

### Sessions 41-50: GUI Implementation  
- WPF + MVVM, Dashboard wouldn't load
- **Learning**: Check ports, check dependencies, check obvious

### Sessions 51-55: Tab-Complete Revolution
- Build times 3min → 20sec with simple PowerShell
- **Learning**: User ideas > complex solutions

### Sessions 56-58: The Great Deletion
- Removed Dead Letter Queue (-650 LOC)
- **Learning**: Deleting code is progress

### Sessions 59-61: Dashboard Victory
- Fixed port mismatch (5050 → 5111)
- **Learning**: Small details break everything

### Sessions 62-63: Final Fixes
- Host property, enum values, config completion
- **Learning**: One line can fix 144 errors

## 📊 METRICS THAT MATTER

```yaml
Total LOC: 14,350 (all by Claude!)
Interfaces: Started 12+ → Current 8 → Target 4
Build Time: 3min → 20sec (without ZIP)
Config Formats: 3 → 1 (unified)
Warnings: 144 (target <50)
Deleted: 650 LOC (Dead Letter)
Fixed: Port 5111 everywhere
```

## 📈 SPRINT PLANNING & ROADMAP

### Current: Sprint 7 - THE GREAT SIMPLIFICATION
```yaml
Phase 1: Foundation (v0.7.1-v0.7.4)      ✅ DONE
Phase 2: Testing Tools (v0.7.5)          ✅ DONE  
Phase 3: Version Fix (v0.7.6)            ✅ DONE
Phase 4: Build Fixes (v0.7.7)            ✅ DONE
Phase 5: Dead Letter (v0.7.8-v0.7.9)     ✅ DONE
Phase 6: Config Unity (v0.7.10)          ✅ DONE
Phase 7: Dashboard Fix (v0.7.11)         ✅ DONE
Phase 8: JSON Fix (v0.7.13)              ✅ DONE
Phase 9: Interface Removal (v0.8.0)      🚀 NEXT
Phase 10: Service Consolidation          📋 PLANNED
```

### Sprint 8: Interface Removal (v0.8.x)
```yaml
Goals:
- Remove 4+ interfaces (8 → 4)
- Direct dependencies everywhere
- Update all references
- Test each removal

Targets:
- IExifReader → ExifToolReader
- IDicomConverter → DicomConverter  
- IFolderWatcher → FileWatcher
- INotificationService → Remove entirely
```

### Sprint 9: Config Redesign (v0.9.x)
```yaml
Goals:
- Simplify to single config class?
- Remove legacy V1 support
- Better defaults
- Self-documenting structure

Ideas:
- Embedded schema
- Config wizard
- Auto-discovery
```

### Sprint 10: Medical Features Part 1 (v1.0)
```yaml
Protected Features - DO NOT START YET:
- FTP Server for legacy devices
- Basic implementation only
- No overengineering!
```

### Future Sprints (Protected)
```yaml
Sprint 11: C-STORE SCP (v1.1)
- DICOM storage service
- Accept from modalities

Sprint 12: Modality Worklist (v1.2)
- Query patient schedules
- Auto-populate data

Sprint 13: C-FIND SCP (v1.3)
- Query/retrieve functionality
- Complete PACS integration
```

## 📝 WISDOM_SPRINT PATTERN

### Purpose
Focused documentation for specific work phases:
```yaml
WISDOM_SPRINT_CONFIG.md     # During config work
WISDOM_SPRINT_INTERFACE.md  # During interface removal
WISDOM_SPRINT_MEDICAL.md    # During medical features
```

### Structure
```markdown
# WISDOM_SPRINT_{TOPIC}
**Sprint**: X
**Goal**: Specific objective
**Sessions**: Y-Z

## Current State
- What works
- What's broken
- Decisions made

## Next Actions
- [ ] Task 1
- [ ] Task 2

## Learnings
- What we discovered
- What to avoid
```

### Benefits
- Survives chat boundaries
- Focused on current work
- Quick status updates
- Preserves context

## 🎯 DEVELOPMENT PRINCIPLES

### The KISS Ladder
```yaml
Level 1: It works (current)
Level 2: It's simple (goal)
Level 3: It's elegant (dream)
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

### Protected Patterns [KEEP!]
```yaml
Tab-Complete: Sacred, never change
Port 5111: Carved in stone
Config Path: Single source of truth
VOGON: For complex work only
Sources First: Always
```

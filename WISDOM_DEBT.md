# WISDOM_DEBT.md - Technical Debt & Future Improvements
**Created**: Session 90  
**Updated**: Session 114 (Konsolidiert)  
**Purpose**: Track what needs fixing WITHOUT cluttering main wisdom files  
**Philosophy**: "Make it work ✅, make it right 🔄, make it fast ⏳"

## 🚨 CRITICAL DEBT: Backup/Delete Race Condition

### The Problem
**Session**: 97 (Oliver remembered mid-session)
**Severity**: HIGH - Data loss possible!
**Current Workaround**: Delete disabled, using "Leave"

```csharp
// DANGER in FileProcessor.HandlePostProcessingAsync():
case PostProcessingAction.Archive:
    File.Move(sourceFile, archivePath, true);  // When is this done?
    break;
    
case PostProcessingAction.Delete:
    File.Delete(sourceFile);  // What if Archive still running?
    break;
```

### Why It Happens
1. User configures: SuccessAction = Archive, FailureAction = Delete
2. File processes successfully → Archive starts
3. Something fails → Delete triggers
4. Original file deleted while Archive still copying!

### Proper Fix Options
```csharp
// Option 1: Ensure sequential execution
await ArchiveFileAsync(source, dest);  // Make it async
File.Delete(source);  // Only after archive completes

// Option 2: Transaction pattern
using (var transaction = new FileTransaction())
{
    transaction.CopyFile(source, archive);
    transaction.DeleteFile(source);
    transaction.Commit();  // Atomic operation
}

// Option 3: State machine
FileState.Processing → FileState.Archiving → FileState.Archived → FileState.Deleted
```

### Impact
- **Manual cleanup needed** (files stay in watch folder)
- **Storage growth** over time
- **User frustration** ("Why aren't files deleted?")
- **Hidden config option** (BackupFolder)

## 🦖 MONSTER FILES (Priority: HIGH)

### PipelineConfigViewModel.cs - 1400+ LOC! 
**Problem**: Single Responsibility Principle? Never heard of her!  
**Impact**: Unmaintainable, untestable, unreadable  
**Time to fix**: 6-8 hours

**Solution**: Tab-based ViewModels
```
PipelineConfigViewModel (Container ~200 LOC)
├── GeneralTabViewModel
├── FoldersTabViewModel  
├── ProcessingTabViewModel
├── DicomTabViewModel
├── LoggingTabViewModel
├── NotificationsTabViewModel
└── PacsTabViewModel ⭐ (newest, easiest to extract first!)
```

**Quick Win**: Start with PacsTabViewModel (1-2h)

### LogViewerViewModel.cs - 1543 LOC! (NEW CHAMPION!)
**Session**: 112 created this monster  
**Problem**: Everything in one file  
**Impact**: Performance issues, hard to test

**Solution**: Service extraction
```
LogViewerViewModel (UI only ~300 LOC)
├── LogFileService (file operations)
├── LogParsingService (correlation parsing)
├── LogFilterService (triple filter logic)
└── LogTreeBuilder (hierarchy creation)
```

### MappingEditorViewModel.cs - 1190 LOC
**Problem**: Complex UI logic mixed with business logic  
**Status**: Lower priority (works but ugly)

## 🏗️ MISSING SERVICE LAYER

**Current**: Business logic in ViewModels = 🤮  
**Better**: Proper services for testability

```csharp
// Needed Services:
IPipelineConfigurationService    // Validation, cloning
IPacsTestService                // Connection testing  
IConfigurationPersistenceService // Save/Load with migration
ILogViewerService               // File operations, parsing
```

**Impact**: Can't unit test without UI  
**Priority**: MEDIUM (works but ugly)

## 📦 DI REGISTRATION CHAOS

**Problem**: Services scattered everywhere  
**Solution**: One place for all UI registrations

```csharp
public static class UiServiceExtensions
{
    public static IServiceCollection AddConfigToolServices(
        this IServiceCollection services)
    {
        // ALL ViewModels here
        // ALL UI Services here
        return services;
    }
}
```

**Time**: 1 hour  
**Priority**: LOW (annoying but not broken)

## 🎯 QUICK WINS (<1h each)

1. **Extract Constants**
   - `PipelineDefaults.cs` with all magic numbers
   - AE Title max length (16), default port (104), etc.

2. **Validation Helper**
   - `PipelineValidator.cs` for AE Title rules
   - Reusable across UI and service

3. **Property Groups**
   - Replace 20 individual properties with grouped objects
   - `PipelineLogSettings`, `PipelineNotificationSettings`

4. **Missing BackupFolder UI**
   - Property exists in ProcessingOptions
   - No UI in PipelineConfigPage
   - Users can't configure it!

## 🐛 KNOWN ISSUES

### Debug Console.WriteLine Everywhere
**Session**: 94 discovery  
**Problem**: Program.cs und PipelineManager haben Debug-Ausgaben  
**Impact**: Console spam in Production

```csharp
// Current:
Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Debug info...");

// Fix:
#if DEBUG
Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Debug info...");
#endif
```

### Missing PACS Features
**Session**: 94 discovery  
**Gewünscht aber nicht implementiert**:
- DeleteAfterUpload (property doesn't exist!)
- ErrorPath für failed uploads
- Upload Statistics Dashboard Widget
- PACS connection status in UI
- PACS Queue visibility (Session 114!)

### Log Issues
- **DeadLetterFolder** references everywhere (obsolete)
- **© shows as Â©** encoding issue (multiple files)
- **~140 build warnings** (mostly nullable)
- **Missing /api/statistics** endpoint (returns 404)
- **Debug WRN in Production** (Session 107)

## 🚀 FEATURE IDEAS (Backlog)

### Near Term
- Email notifications (stubbed, needs SMTP)
- Pipeline priorities (Emergency first)
- Error List UI (better than folder)
- Dashboard PACS status widget
- PACS Queue viewer (Session 114 priority!)

### Long Term  
- C-STORE advanced (compression, TLS)
- FTP Server integration
- DICOM Query/Retrieve (Q/R SCP)
- HL7 Interface for RIS/HIS

## 📋 REFACTORING PRIORITY

1. **Backup/Delete Race Condition** ⭐⭐⭐ CRITICAL NEW!
   - Data loss possible
   - Workaround hurts usability
   - Needs proper async handling

2. **LogViewerViewModel** ⭐⭐⭐ URGENT
   - 1543 lines! New record!
   - Service extraction needed
   
3. **PipelineConfigViewModel** ⭐⭐⭐ URGENT
   - 1400 LOC still there
   - BackupFolder UI could be added during split
   
4. **Missing BackupFolder UI** ⭐⭐ HIGH
   - Quick win (<1 hour)
   - Add to Processing tab
   - User confusion resolved

5. **Debug Console.WriteLine** ⭐ LOW
   - Still spamming in production
   - Use conditional compilation

6. **Fix Encoding Issues** ⭐ LOW
   - Professional polish
   - UTF-8 BOM needed

## 💡 LESSONS FOR NEW CODE

> "Don't let files grow beyond 500 LOC"  
> "Create services BEFORE ViewModels get fat"  
> "One responsibility per class"  
> "Check for race conditions in async code"

## 🚫 DO NOT REFACTOR (Yet)

- Working business logic (if it ain't broke...)
- XAML bindings (too fragile!)
- Navigation system (complex but stable)
- Core pipeline logic (medical critical)

## ✅ DEBT PAID (Sessions 94-114)

### Duplicate Enum Definitions
**Session**: 97  
**Was**: LogVerbosity and ProcessingStage defined in multiple places  
**Fix**: Moved to CamBridge.Core.Enums namespace  
**Time**: 2 hours (including all using updates)

### Service Registration Bug  
**Session**: 94  
**Was**: DicomStoreService not found  
**Fix**: Use AddInfrastructure()  
**Time**: 2 Sessions debugging → 1 line fix

### Encoding Issues (Partial)
**Session**: 95  
**Was**: UTF-8 symbols in logs  
**Fix**: ASCII replacements in some files  
**Status**: Partial - © symbol still appears

## 📊 DEBT METRICS

```yaml
Total Warnings: ~140 (unchanged)
Monster Files: 3 (was 2, LogViewer joined!)
Critical Issues: 2 (Race condition, PACS visibility)
Quick Wins Available: 4
Fixed Since Session 90: 3
```

---

**Remember**: Technical debt is a LOAN, not a GIFT. Pay it back when adding features in that area!

**Session 114 Status**: Race condition critical, PACS visibility urgent, Monster ViewModels growing! 🦖

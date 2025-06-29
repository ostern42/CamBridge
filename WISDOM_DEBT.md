# WISDOM_DEBT.md - Technical Debt & Future Improvements
**Created**: Session 90  
**Updated**: Session 93 (Destillation)  
**Purpose**: Track what needs fixing WITHOUT cluttering main wisdom files  
**Philosophy**: "Make it work ✅, make it right 🔄, make it fast ⏳"

## 🦖 Monster Files (Priority: HIGH)

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

## 🏗️ Missing Service Layer

**Current**: Business logic in ViewModels = 🤮  
**Better**: Proper services for testability

```csharp
// Needed Services:
IPipelineConfigurationService    // Validation, cloning
IPacsTestService                // Connection testing  
IConfigurationPersistenceService // Save/Load with migration
```

**Impact**: Can't unit test without UI  
**Priority**: MEDIUM (works but ugly)

## 📦 DI Registration Chaos

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

## 🎯 Quick Wins (<1h each)

1. **Extract Constants**
   - `PipelineDefaults.cs` with all magic numbers
   - AE Title max length (16), default port (104), etc.

2. **Validation Helper**
   - `PipelineValidator.cs` for AE Title rules
   - Reusable across UI and service

3. **Property Groups**
   - Replace 20 individual properties with grouped objects
   - `PipelineLogSettings`, `PipelineNotificationSettings`

## 🐛 Known Issues

- **DeadLetterFolder** references everywhere (obsolete)
- **© shows as Â©** encoding issue (multiple files)
- **~140 build warnings** (mostly nullable)
- **Missing /api/statistics** endpoint (returns 404)

## 🚀 Feature Ideas (Backlog)

### Near Term
- Email notifications (stubbed, needs SMTP)
- Pipeline priorities (Emergency first)
- Error List UI (better than folder)
- Dashboard PACS status widget

### Long Term  
- C-STORE advanced (compression, TLS)
- FTP Server integration
- DICOM Query/Retrieve (Q/R SCP)
- HL7 Interface for RIS/HIS

## 📋 Refactoring Priority

1. **Extract PacsTabViewModel** ⭐ (easy win, newest code)
2. **Create Service Layer** (enables testing)
3. **Fix DI Registration** (one-time cleanup)
4. **Extract Constants** (improves readability)
5. **Fix Encoding Issues** (professional polish)

## 💡 Lessons for New Code

> "Don't let files grow beyond 500 LOC"  
> "Create services BEFORE ViewModels get fat"  
> "One responsibility per class"  

## 🚫 Do NOT Refactor (Yet)

- Working business logic (if it ain't broke...)
- XAML bindings (too fragile!)
- Navigation system (complex but stable)
- Core pipeline logic (medical critical)

---

**Remember**: Technical debt is a LOAN, not a GIFT. Pay it back when adding features in that area!


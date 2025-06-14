# CAMBRIDGE_OVERVIEW - Project Documentation
**Last Updated:** 2025-06-14, 03:00  
**Version:** 0.7.11  
**Status:** Dashboard Working! Ready for Simplification Phase 2!  
**Author:** Claude (all 14,350 lines of code!)

## 🎯 PROJECT OVERVIEW

### What is CamBridge?
CamBridge is a medical imaging software that bridges the gap between consumer cameras (Ricoh G900 II) and hospital PACS systems. It converts JPEG images with embedded barcode data into DICOM format, maintaining patient and examination information throughout the process.

### The Core Workflow
```
1. QRBridge creates QR code with patient data
2. Camera photographs QR code → data in EXIF
3. Camera takes medical photos → EXIF preserved
4. CamBridge watches folders → detects new images
5. Extracts EXIF data → creates DICOM
6. Sends to PACS → automatic assignment
```

### Key Components
- **QRBridge:** Creates QR codes with patient/exam data
- **CamBridge Service:** Windows service for automated processing
- **CamBridge Config:** WPF application for configuration
- **Processing Pipeline:** Multi-pipeline architecture for different workflows

## 📊 CURRENT STATUS (v0.7.11)

### What's Working
- ✅ Multi-pipeline architecture
- ✅ DICOM conversion with patient data
- ✅ Configuration UI with live dashboard
- ✅ Settings persistence (unified!)
- ✅ Tab-complete testing system
- ✅ Professional version display
- ✅ Dashboard shows pipelines (fixed!)
- ✅ Config unity achieved
- ✅ Sources in project knowledge

### Known Issues
- 🐛 Add Mapping Rule button (minor)
- 🐛 Settings Save button visibility
- 📋 13 interfaces to remove
- 📋 144 warnings to reduce

### Recent Victories
- **Session 61:** Dashboard fix (3 small changes!)
- **Session 61:** Sources revolution implemented
- **Session 60:** Config unity designed
- **Session 58:** Dead Letter removal (-650 LOC!)
- **Session 56:** Version consistency everywhere
- **Session 55:** Tab-complete testing born

## 🔧 TECHNICAL DETAILS

### Technology Stack
```yaml
GUI Framework: WPF + ModernWpfUI 0.9.6
MVVM: CommunityToolkit.Mvvm 8.3.2
Service: ASP.NET Core 8.0 Minimal API
Platform: Windows Service
DICOM: fo-dicom 5.2.2
EXIF: ExifTool 13.30
.NET: 8.0
Language: C# 12
IDE: Visual Studio 2022
Architecture: x64 (Config), AnyCPU (Service)
Testing: Tab-Complete System v1.0
Port: 5111 (unified!)
```

### Project Structure
```
CamBridge/
├── src/
│   ├── CamBridge.Core/          # Business logic
│   ├── CamBridge.Infrastructure/# Services & implementations
│   ├── CamBridge.Service/       # Windows service
│   └── CamBridge.Config/        # WPF config tool
├── tests/
│   └── CamBridge.Tests/         # Unit tests
├── tools/
│   ├── Build-CamBridge.ps1      # Tab-complete build
│   ├── Test-CamBridge.ps1       # Tab-complete test
│   └── Get-WisdomSources.ps1    # Sources collector
└── docs/
    └── WISDOM_*.md              # Project documentation
```

### Configuration Architecture
```json
{
  "CamBridge": {
    "Version": "2.0",
    "Service": {
      "ServiceName": "CamBridgeService",
      "ApiPort": 5111,
      "EnableHealthChecks": true
    },
    "Pipelines": [...],
    "MappingSets": [...]
  }
}
```

## 🚀 DEVELOPMENT PHILOSOPHY

### Core Principles
1. **KISS > Architecture** - Simple solutions win
2. **Testing > Assuming** - Build success ≠ feature works
3. **Removal > Refactoring** - Dead code must die
4. **Details Matter** - Ports, versions, configs
5. **User First** - Oliver's ideas often best

### The Tab-Complete Revolution
```powershell
0[TAB]  # Build without ZIP (20 seconds!)
00[TAB] # Build with ZIP
9[TAB]  # Quick test (no build)
99[TAB] # Full test (with build)
h[TAB]  # Help

# One command testing - revolutionary!
```

### Professional Standards
- Version consistency EVERYWHERE
- Same behavior in Debug & Release
- Proper error handling
- Clean, predictable configs
- No magic numbers

## 📈 PROGRESS TRACKING

### Sprint 7: THE GREAT SIMPLIFICATION
```
Phase 1: Foundation (v0.7.1-v0.7.4)    ✅ DONE
Phase 2: Testing Tools (v0.7.5)        ✅ DONE
Phase 3: Version Fix (v0.7.6)          ✅ DONE
Phase 4: Build Fixes (v0.7.7)          ✅ DONE
Phase 5: Dead Letter (v0.7.8-v0.7.9)   ✅ DONE
Phase 6: Config Unity (v0.7.10)        ✅ DONE
Phase 7: Dashboard Fix (v0.7.11)       ✅ DONE
Phase 8: Interface Removal (v0.8.0)    🚀 NEXT
Phase 9: Service Consolidation         📋 FUTURE
Phase 10: Test & Stabilize             📋 FUTURE
```

### Simplification Metrics
```
Interfaces:    26 → 13 → 0 (goal)
Services:      15 → 12 → 5 (goal)
LOC Removed:   -650 (Dead Letter)
Warnings:      144 → <50 (goal)
Config Systems: 3 → 1 ✅
Build Time:    45s → 20s ✅
```

## 🏥 PROTECTED MEDICAL FEATURES

These features are protected and scheduled for future sprints:

### Sprint 8: FTP Server
- For legacy device support
- Simple implementation planned
- No over-engineering!

### Sprint 9: C-STORE SCP
- DICOM storage service
- Accept images from modalities
- Critical for hospital integration

### Sprint 10: Modality Worklist
- Query patient schedules
- Auto-populate exam data
- Reduce manual entry

### Sprint 11: C-FIND SCP
- Query/retrieve functionality
- Search existing studies
- Complete DICOM integration

## 🎯 IMMEDIATE NEXT STEPS

### Interface Removal Phase 2 (v0.8.0)
1. Analyze remaining 13 interfaces
2. Identify unnecessary abstractions
3. Create direct implementations
4. Remove one by one
5. Test after each removal

### Expected Outcomes
- Simpler codebase
- Easier debugging
- Faster development
- Less cognitive load
- Happy developer!

## 💡 KEY LEARNINGS

### Technical Insights
1. **Port consistency critical** - 5050 vs 5111 broke everything
2. **Config paths matter** - One source of truth
3. **Old code persists** - Check version headers
4. **Details break systems** - But small fixes repair them
5. **Sources in knowledge** - Token efficiency revolution

### Process Insights
1. **VOGON works** - Structure helps
2. **Tab-complete genius** - Simple > complex
3. **User knows best** - Oliver's ideas consistently brilliant
4. **Persistence pays** - 10 build attempts normal
5. **Self-awareness helps** - I wrote all this code!

## 🤖 ABOUT THE CODE

### The Big Revelation
```
Total Lines of Code: 14,350
Written by: Claude (me!)
Remembered by: Not Claude (oops!)
Rediscovered in: Session 61
Reaction: 🤯
```

### What This Means
- Every line is my responsibility
- Every bug is my creation
- Every solution is within reach
- Every improvement is possible
- Sources First applies to ME!

## 📞 QUICK REFERENCE

### Important Paths
```
Config: %ProgramData%\CamBridge\appsettings.json
Logs: %ProgramData%\CamBridge\logs\
Service: CamBridgeService (not CamBridge Service!)
Port: 5111 (everywhere!)
```

### Testing Commands
```powershell
# Quick status check
Invoke-RestMethod -Uri "http://localhost:5111/api/status/version"

# Pipeline check
Invoke-RestMethod -Uri "http://localhost:5111/api/pipelines"

# Full test cycle
0[TAB]  # Build
1[TAB]  # Deploy & Start
2[TAB]  # Config UI
9[TAB]  # Test
```

### Troubleshooting
1. Dashboard empty? Check port 5111
2. Config not loading? Check %ProgramData%
3. Service won't start? Check Event Log
4. Build fails? Fix one error at a time
5. Confused? Check WISDOM docs!

## 🌟 PROJECT MOTTO

*"Making the improbable reliably simple!"*

From complex medical imaging workflows to simple, reliable solutions.
One interface removal at a time.
With tab-complete testing.
And dashboard victories.
And self-awareness.

---

**CamBridge - Where DICOM meets KISS**  
© 2025 Claude's Improbably Reliable Software Solutions

*P.S. Yes, I wrote all 14,350 lines and forgot about them. But they're good lines! 😊*

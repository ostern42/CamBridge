# CamBridge Changelog

All notable changes to CamBridge will be documented in this file.  
© 2025 Claude's Improbably Reliable Software Solutions

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

[0.5.16] - 2025-06-02 00:30
Added

Console logging for service debugging
Detailed error messages for DICOM conversion failures
Pipeline analysis and systematic development plan

Fixed

Settings page DataContext initialization
PropertyChanged event handler (removed problematic OnPropertyChanged call)
Dashboard API connection (shows green/connected status)

Discovered

ExifToolReader not properly integrated - uses MetadataExtractor only
GCM_TAG prefix causes DICOM validation errors
Missing Transform functions in mapping engine
File logging not working despite configuration
Retry logic tries to process already moved files

Changed

Identified systematic approach needed: JPEG→ExifTool→Parse→Map→DICOM
Decision to implement ExifToolReader properly instead of patching RicohExifReader
New sprint plan focusing on pipeline components one at a time

Testing

✅ Watch Folder detects JPEGs
✅ Basic file processing pipeline runs
✅ QRBridge data successfully extracted
❌ DICOM creation fails due to validation errors
❌ ExifTool integration incomplete

TESTED: File detection and parsing work, but DICOM creation needs systematic fixes


## [0.5.13] - 2025-06-02 23:12

### Fixed
- Mapping Editor selection binding now working
- Replaced ItemsControl with ListBox for proper selection support
- Added visual feedback for selected mapping rules

### Added
- Selection highlight with accent border
- SelectedItem two-way binding to ViewModel

### Technical
- Custom ItemContainerStyle for clean ListBox appearance
- DataTrigger for selection visual state
- Properties panel now correctly updates on selection

### Status
- Mapping Editor: 100% functional ✅
- Templates: Working
- Drag & Drop: Working
- Selection: Fixed
- Next: Watch Folder implementation

## [0.5.12] - 2025-06-02 22:50
### Fixed
- Navigation: MappingEditor und AboutPage in NavigationService registriert
- DataContext wird jetzt korrekt über DI für MappingEditorViewModel gesetzt
- XAML Parse-Fehler durch ersetzen von Symbol="Up/Down" mit Unicode-Pfeilen behoben
- Build-Fehler durch entfernen von MockConfigurationService Referenzen gefixt

### Changed
- MappingEditor Buttons verwenden jetzt ▲▼ statt SymbolIcons für bessere Kompatibilität
- PreviewInputChanged Methodenaufruf durch Property-Setter ersetzt

### Tested
- ✅ Mapping Editor öffnet sich ohne Crash
- ✅ Templates (Ricoh G900, Minimal, Full) funktionieren
- ✅ Drag & Drop von Source Fields funktioniert
- ✅ Add Rule erstellt neue Mappings
- ✅ Import/Export/Save Dialoge öffnen sich
- ✅ Modified-Flag wird bei Save zurückgesetzt
- ⚠️ Rule Properties Panel reagiert noch nicht (Fix in v0.5.13)

### Developer Notes
- Navigation-Probleme immer zuerst in NavigationService Dictionary prüfen
- ModernWpfUI hat begrenzte Symbol-Namen - Unicode oft sicherer
- "Das Offensichtliche zuerst" - 1-Zeilen-Fix löste komplexes Problem

## [0.5.11] - 2025-06-02 20:33
### Fixed
- Mapping Editor crash resolved - duplicate converter registration removed
- Added missing event handlers for drag&drop functionality
- Fixed MappingEditorPage.xaml.cs compilation errors
- Removed problematic DataType attributes from DataTemplates

### Known Issues
- DataContext/ViewModel not automatically injected (buttons non-functional)
- XAML Designer showing phantom converter errors (build works fine)
- Need to investigate MainWindow navigation for proper DI integration

### Technical Notes
- ValueConverters.cs already contained all required converters
- "Nachts mit Sonnenbrille" - lesson learned about checking existing code first
- Visual Studio XAML Designer cache can be very persistent

## [0.5.10] - 2025-06-02 18:47
### Fixed
- Service Start/Stop functionality fully working
- Service status correctly reflected in GUI
- All service control buttons functioning as expected
### Tested
- ✅ Service can be started via GUI and services.msc
- ✅ Service can be stopped via GUI and services.msc
- ✅ Service status updates correctly in real-time
- ✅ Restart functionality works properly
### Notes
- First feature with 100% test coverage!
- Service uptime tracking functional

## [0.5.9] - 2025-06-02 18:30
### Fixed
- Service installation path detection enhanced
- Added missing net8.0-windows build output paths
- ServiceManager now searches all 3 known exe locations
- sc.exe error output now properly captured and displayed
### Added
- CamBridge.ServiceDebug diagnostic tool for troubleshooting
- Detailed path checking with existence indicators
- Better error messages showing all searched locations
### Tested
- ✅ Service installation successful via debug tool
- ✅ Service appears correctly in services.msc
- ✅ GUI correctly detects installed service
### Notes
- Critical bug resolved after 8 versions of attempts!

## [0.5.8] - 2025-06-02 17:25

### Added
- Install Service button in Service Control GUI
- Uninstall Service button in Quick Actions
- Service installation/uninstall functionality with sc.exe
- Improved error messages with detailed path information
- Debug output for service executable discovery
- Parser Debug Console with interactive file selection
- Windows Forms file dialog support for Parser Debug
- Direct ExifTool process execution (no dependency on ExifToolReader)

### Changed
- ServiceManager now searches x64 build paths correctly
- Event Viewer and Services buttons use ProcessStartInfo with UseShellExecute
- Parser Debug Console supports nullable return types
- Improved service path discovery with multiple fallback locations

### Fixed
- Service.exe is now correctly built as executable (was DLL)
- Event Viewer "working directory cannot be null" error
- Parser Debug Console no longer requires command line argument
- Async method warnings in Parser Debug

### Discovered
- 52+ features implemented but 0% tested
- Mapping Editor crashes on open (not fixed yet)
- Service installation still fails despite exe existing
- Complete feature testing backlog documented

### Development Notes
- NEW PHILOSOPHY: One feature = One version = Immediate testing
- Estimated 30+ micro-versions needed to reach stable v0.6.0
- Focus on fixing blocking issues before adding ANY new features
- Next priority: Fix service installation path problem in v0.5.9

## [0.5.7] - 2025-06-02 16:52

### Added
- Service Control GUI Install Service button
- Service Control GUI Uninstall Service button in Quick Actions
- ServiceManager implementation with Install/Uninstall methods
- IServiceManager interface extended with installation methods
- Parser Debug Console file dialog support (no more command line required)
- Parser Debug Console sample file search functionality
- ServiceManager searches multiple locations including x64 paths

### Changed
- Parser Debug Console now shows interactive menu when no file provided
- ServiceControlViewModel includes InstallServiceAsync and UninstallServiceAsync commands
- ServiceControlPage.xaml improved layout with better messaging
- Service installation uses sc.exe for Windows service management

### Fixed
- Build errors related to duplicate IServiceManager definitions
- Parser Debug Console nullable reference warnings
- ParserDebug.csproj now targets net8.0-windows with UseWindowsForms

### Known Issues
- Service installation fails with "not a valid application for this OS platform"
- Mapping Editor crashes when opening
- Event Viewer button shows "working directory cannot be null" error
- No features have been tested end-to-end yet

### Development Notes
- Discovered Service.exe was being built as DLL instead of EXE
- Found 52+ implemented features with 0% test coverage
- Decision made to switch to micro-version development approach

## [0.5.6] - 2025-06-02 15:42

### Added
- ParserDebug tool ExifTool integration for direct testing
- Interactive menu system in ParserDebug console
- Hex dump visualization for EXIF data analysis

### Fixed
- Build errors resolved (PatientId false alarm - no duplicate)
- ParserDebug namespace conflicts (MetadataExtractor vs System.IO)
- ParserDebug now properly shows UserComment raw bytes

### Verified
- ExifTool successfully reads Barcode tag with ALL 5 QRBridge fields
- Ricoh G900 II stores data in two locations:
  - UserComment: 3 fields only (with GCM_TAG prefix)
  - Barcode: All 5 fields complete (ExifTool required)
- Encoding issue confirmed: Umlauts in Barcode tag (Latin-1)

### Known Issues
- Service Control GUI missing "Install Service" button
- Debug Console path resolution error
- Windows Service never tested
- ExifTool integration in main app untested


## [0.5.5] - 2025-06-02 10:42
### Added
- GitHub integration successfully implemented - project now publicly accessible
- Direct file access via raw.githubusercontent.com URLs
- Complete git history (1475 commits) preserved on GitHub

### Changed
- Development workflow now uses GitHub for source file sharing
- No more collect-sources.bat needed - direct file access instead
- Token efficiency improved by ~70% through targeted file fetching

### Technical Notes
- Repository: https://github.com/ostern42/CamBridge (public)
- Claude requires explicit URL provision for security
- File structure visible through GitHub web interface
- Next session: Fix PatientId duplicate build error first!

## [0.5.4] - 2025-06-02 02:00
### Added
- Google Drive Integration documentation (VOGON DRIVE)
- Direct source file access capability (70% token savings)

### Changed
- Renamed VOGON CLOSE to VOGON EXIT for consistency
- Optimized PROJECT_WISDOM.md (removed redundant sections)
- Compacted technology stack documentation

### Removed
- Redundant Git history details from PROJECT_WISDOM
- Version history duplicates
- Time estimates (token optimization)

## [0.5.3] - 2025-06-02 01:05
### Added
- ExifToolReader for comprehensive EXIF tag support
- Debug console for analyzing EXIF data extraction
- Support for Pentax/Ricoh proprietary "Barcode" tag
- Automatic fallback hierarchy for EXIF readers

### Fixed
- Parser bug: QRBridge data is stored in Barcode tag, not UserComment
- "GCM_TAG " prefix now correctly removed from examid
- NotificationService interface implementation completed

### Changed
- EXIF reader now prioritizes Barcode tag over UserComment
- ServiceCollectionExtensions updated for ExifTool integration

### Discovered
- Ricoh G900 II stores all 5 QRBridge fields in Barcode tag
- MetadataExtractor cannot read proprietary Pentax tags
- ExifTool required for complete data extraction

### Known Issues
- PatientId class duplicated in Entities and ValueObjects
- ExifTool integration not yet tested
- Build errors prevent testing of parser fixes

## [0.5.2] - 2025-06-01 23:52
### Added
- New unified source collector script (collect-sources.bat) with 7 profiles
- Intelligent profile selection based on Git changes (collect-smart.bat)
- Timestamp feature prevents output file overwrites
- Collector documentation (COLLECTOR_README.md)

### Fixed
- NotificationService now implements all INotificationService interface methods
- Added missing async notification methods (Info, Warning, Error, CriticalError)
- Added NotifyDeadLetterThresholdAsync implementation
- Added SendDailySummaryAsync with summary formatting

### Changed
- Replaced 6 individual collector scripts with unified solution
- Collector output now includes timestamp in filename
- Improved file filtering to exclude obj/bin/packages/wpftmp

### Developer Notes
- Parser bug confirmed: QRBridge string is being truncated in our parser
- v0.5.0-0.5.1 features still need testing (Mapping Editor, DICOM Browser)
- Build errors from v0.5.1 resolved

## [0.5.1] - 2025-06-01 22:32
### Added
- DICOM Tag Browser Dialog with search and grouping by module
- Template system fully functional (Ricoh, Minimal, Full templates)
- QRBridge Protocol v2 parser with JSON format support
- Import/Export functionality for mapping configurations
- Backward compatibility for v1 pipe-delimited format
- DicomTagBrowserDialog for intuitive tag selection
- EnumToCollectionConverter integrated into ValueConverters.cs

### Changed
- RicohQRBridgeParser now supports both v1 and v2 protocols
- Template buttons now use MVVM commands instead of click handlers
- Improved error handling in protocol parsing
- NotificationService updated for nested EmailSettings structure

### Fixed
- Project references: CamBridge.Config now references Infrastructure
- System.Drawing.Common version conflict resolved (8.0.10)
- XAML markup errors in MappingEditorPage (Run opacity issue)
- NotificationService email property access corrected

### Technical
- QRBridgeProtocolV2Parser for JSON-based format
- Protocol version detection with automatic fallback
- MappingConfigurationLoader integration
- Complete drag & drop implementation in MappingEditorPage

## [0.5.0] - 2025-06-01 21:47
### Added
- Mapping Editor with drag & drop UI for EXIF to DICOM configuration
- Live preview for field transformations
- Template system for quick mapping setup (UI only)
- PasswordBoxHelper for secure password binding in Settings
- NotificationSettings model with comprehensive email configuration
- MappingEditorViewModel with validation logic
- QRBridge source code integration - full control over both sides!

### Fixed
- PasswordBox security issue - now uses proper attached property
- BorderStyle error in MappingEditorPage XAML
- AboutPage _spriteTimer nullable reference warning
- DI registration for MappingEditorViewModel

### Changed
- MainWindow size increased to 1200x800 for better usability
- Mapping Editor layout with proportional scaling (2* for middle column)
- Navigation includes Mapping Editor item

### Discovered
- QRBridge source available - can optimize protocol (planned for v0.5.1)
- Bidirectional control enables better field encoding than pipes
- Ricoh limitation workarounds possible with custom protocol

### Known Issues
- Template buttons not yet functional
- DICOM tag selector shows placeholder dialog
- Import/Export not implemented
- Mappings not persisted to configuration yet

## [0.4.5] - 2025-06-01 20:52
### Added
- ConfigurationService with JSON persistence to %APPDATA%\CamBridge
- NotificationSettings model with comprehensive email configuration
- Global converter registration in App.xaml for all pages
- PasswordBoxHelper class for secure password binding (implementation pending)

### Fixed
- Settings page crash on navigation - DI registration for ConfigurationService
- All value converters now properly registered and accessible
- Navigation between all pages now stable

### Changed
- Temporary TextBox for SMTP password field (PasswordBox binding workaround)
- Settings are now persisted between application sessions
- Improved error handling during page initialization

### Known Issues
- PasswordBox still uses TextBox temporarily (security concern)
- Ricoh G900 II only saves 3 of 5 QRBridge fields
- Service connection shows "Service Offline" (service not running)

## [0.4.4] - 2025-06-01 19:21
### Added
- Core functionality test with real Ricoh G900 II JPEG
- Enhanced EXIF parser with line break and encoding fixes
- Flexible QRBridge parser for incomplete data
- mappings.json for TestConsole
- Hex dump debugging in TestConsole

### Fixed
- EXIF encoding issues with German umlauts
- Parser handling of camera line breaks in barcode data
- NotificationService null reference warnings

### Changed
- Parser now handles incomplete QRBridge data (3 of 5 fields)
- Improved debug logging for QRBridge parsing

### Discovered
- Ricoh G900 II only saves first 3 QRBridge fields (gender/comment missing)
- Camera inserts "GCM_TAG " prefix in UserComment

### Known Issues
- Settings page still crashes on navigation
- QRBridge data truncation needs investigation

## [0.4.3] - 2025-06-01 17:15
### Added
- Vogon Poetry Easter Egg - tribute to Douglas Adams
  - Activated by typing "42" on About page
  - Amiga-style Boing Ball sprite animation (WritePixels implementation)
  - Scrolling rainbow text with retro effects
  - Vogonian poetry about DICOM with ERROR HAIKU
  - Guru Meditation error messages
- Dead Letters page basic functionality
  - DataGrid with items display
  - Connection status indicator
  - Retry functionality per item

### Fixed
- Dead Letters navigation crash - fixed DI registration
- AboutPage keyboard focus issues
- Removed unsafe code for better stability
- Fixed nullable reference warnings

### Known Issues
- Settings page crashes on navigation (ViewModel initialization)

## [0.4.2] - 2025-06-01 15:10
### Added
- Dead Letters management page with full CRUD operations
- Real-time filtering and sorting
- Export functionality (CSV/JSON)
- Batch operations for retry/delete

### Known Issues
- Dead Letters page crashes on navigation (DI issue)

## [0.4.1] - 2025-06-01 13:30 (pending commit)
### Added
- Complete Settings page with 4-tab TabView layout
- Real JSON persistence to appsettings.json with auto-backup
- Comprehensive MVVM data binding with CommunityToolkit.Mvvm
- Watch folder management with add/remove functionality
- Input validation with Data Annotations
- Folder browse dialogs using Win32 interop
- Status bar with loading indicators and change tracking
- Value converters for visibility bindings
- PROJECT_WISDOM.md - Cross-session documentation system

### Fixed
- WPF/ModernWPF compatibility (removed WinUI3 Spacing attributes)

### Changed
- ConfigurationService from mock to real implementation
- Version management to prevent duplicates (lessons learned from v0.4.0)

## [0.4.0] - 2025-06-01 11:30:55 (Duplicate version! Should have been v0.4.1)
### Added
- Service Control implementation (Phase 9)
- Start/Stop/Restart functionality with UAC handling
- Automatic status updates every 2 seconds
- Uptime display for running service
- "Restart as Administrator" feature
- Quick Actions for Services.msc and EventVwr.msc
- Service installation detection

### Note
- This was committed 9 hours after the first v0.4.0 commit
- Should have incremented version number

## [0.4.0] - 2025-06-01 02:22:32 (Night shift development)
### Added
- WPF Configuration GUI with ModernWPF UI framework
- Real-time dashboard with auto-refresh (5 seconds)
- HttpApiService for REST API communication  
- StatusController API endpoints for service monitoring
- Connection status indicator with visual feedback
- Active processing items display
- Recent activity tracking
- Full dependency injection for ViewModels and Services

### Fixed
- Platform-specific CA1416 warnings for Windows-only features
- Missing package references for HTTP client
- Dependency injection setup for ViewModels
- Proper error handling for offline service

### Changed
- Dashboard now shows live data from ProcessingQueue
- Added loading states during API calls
- Display of processing statistics and queue status

### Breaking Change
- INotificationService now includes NotifyErrorAsync method

## [0.3.2] - 2025-05-31 23:10:22
### Added
- Complete error handling with dead-letter queue persistence and reprocessing
- Email/event log notifications with daily summaries and threshold alerts
- Web dashboard (port 5050) with REST API and real-time monitoring
- PowerShell installation script with automated setup
- Build and deployment automation

### Known Issues
- Integration tests have build errors (later fixed)

## [0.3.1] - 2025-05-31 16:51:44
### Fixed
- Dependency injection issue where singleton ProcessingQueue tried to consume scoped IFileProcessor
- ProcessingQueue now uses IServiceScopeFactory to create scopes for file processing
- Removed duplicate IFileProcessor registration in Program.cs

### Added
- Batch and PowerShell scripts for collecting source files for deployment

## [0.3.0] - 2025-05-31 15:45:17
### Added
- FileProcessor service orchestrating complete conversion pipeline
- ProcessingQueue with thread-safe operation and retry logic
- FolderWatcherService monitoring multiple folders via FileSystemWatcher
- Comprehensive configuration system via appsettings.json
- Health check endpoint for service monitoring
- Statistics reporting and performance metrics
- PowerShell installation script and documentation

### Changed
- Worker service now coordinates all processing components
- Enhanced logging with structured output
- Target framework to net8.0-windows for Windows Service

### Breaking Change
- Target framework changed to net8.0-windows

## [0.2.0] - 2025-05-31 10:34:17
### Added
- Flexible JSON-based mapping configuration system
- MappingConfigurationLoader for JSON serialization
- DicomTagMapper service for dynamic EXIF to DICOM mapping
- Support for value transformations (date, gender, truncation)
- Comprehensive tests for mapping system

## [Missing Version] - 2025-05-31 01:17:17 (Night shift!)
### Added
- DICOM conversion implementation with fo-dicom v5.1.2
- DicomConverter for JPEG to DICOM transformation
- Preserve JPEG compression using encapsulated pixel data
- Support YBR_FULL_422 photometric interpretation

### Note
- This commit was missing version number in git commit message

## [0.1.0] - 2025-05-30 23:49:44
### Added
- EXIF extraction with QRBridge support
- Support for pipe-delimited and command-line QRBridge formats
- RicohExifReader for specialized Ricoh G900 II support
- Infrastructure layer with comprehensive unit tests

## [0.0.2] - 2025-05-30 21:34:12 (Git duplicate - 78 seconds after first)
### Added
- Core domain models (second commit)

## [0.0.2] - 2025-05-30 21:32:54
### Added
- Core domain models (Patient, Study, Metadata)
- Value objects (DicomTag, ExifTag, PatientId, StudyId)
- Repository interfaces

## [0.0.1] - 2025-05-30 20:34:20
### Added
- Initial project structure with 4 projects (Core, Infrastructure, Service, Config)
- Automatic versioning via Version.props
- Documentation (README, CHANGELOG, LICENSE)

### Note
- Project started at 20:30:44 with .gitattributes
- First real commit 4 minutes later

---

## Version History Summary
- Total development time: ~44.7 hours over 2.8 days
- Night shifts: DICOM (01:17), GUI (02:22)
- Version duplicates: v0.0.2 (78 sec), v0.4.0 (9 hours)
- Missing versions: DICOM commit, v0.3.3 (was in old CHANGELOG but not in git)

## Lessons Learned
- Always increment version numbers, even for small changes
- Use "babysteps" versioning (v0.0.1 → v0.0.2 → v0.0.3)
- Check git history before committing to avoid duplicates
- Night shift commits need extra attention to versioning

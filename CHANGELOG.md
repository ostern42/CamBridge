# CamBridge Changelog

All notable changes to CamBridge will be documented in this file.  
© 2025 Claude's Improbably Reliable Software Solutions

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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

# CamBridge Changelog

All notable changes to CamBridge will be documented in this file.  
© 2025 Claude's Improbably Reliable Software Solutions

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

# Changelog

# Changelog

## [0.5.35] - 2025-06-06

### Added
- Enhanced Marvin Easter Egg in About Page with multi-level click progression
- 12 CamBridge-specific Marvin quotes about converting JPEGs to DICOM
- Progressive Easter Egg reveals: 3 clicks→Marvin, 5 clicks→Vogon Poetry, 7 clicks→More Marvin, 10 clicks→VogonPoetryWindow
- "DON'T PANIC" tooltip on About Page logo
- Subtle "42" hint text in About Page footer
- Animation state tracking to prevent click overlap issues
- New planned feature: Marvin QR Code Commentary for QRBridge (CAMB-012)

### Changed
- About Page version updated from 0.5.27 to 0.5.35
- Extended existing Vogon Poetry Easter Egg with Marvin integration
- Improved click detection with 3-second reset timer
- Optimized animation timings: Marvin displays for 7s (was 5s), Vogon Poetry for 13s (was 10s)
- Faster fade animations: 0.3s for Marvin, 0.5s for restore (was 0.5s/1.0s)

### Fixed
- Copyright symbol encoding issue in About Page XAML (Â© → ©)
- Updated outdated version references in About Page
- Click overlap bug where Marvin animation would be interrupted by Vogon Poetry
- PROJECT_WISDOM.md artifact was truncated - now complete

### Developer Notes
- Remember to check FileTree before creating new files!
- User suggested brilliant QR Code Easter Egg idea for next sprint
- Marvin quotes make the medical workflow more enjoyable
- Always verify artifacts are complete before committing!

## [0.5.33] - 2025-06-05

### Added
- QRBridge 2.0 implementation as integrated CamBridge.QRBridge project
- Native UTF-8 support throughout the QR generation pipeline
- WinForms-based QR display with countdown timer
- MessageBox-based help system (no console dependency)
- Framework-dependent build option (7MB vs 163MB)
- QRBridge.bat launcher script for easy command-line usage
- Shared Core entities between QRBridge and main pipeline
- Automatic QRBridge integration in deployment package

### Changed
- Deployment package optimized from 118MB to 53MB (55% reduction)
- QRBridge now part of main CamBridge solution
- Build process includes both service and QRBridge compilation
- Deployment script updated to handle framework-dependent builds

### Fixed
- Console.OutputEncoding crash in WinForms applications
- Named parameter issues with required constructors
- UTF-8 encoding consistency across entire pipeline

### Technical
- Migrated from standalone QRBridge to integrated solution
- Implemented proper error handling for non-console environments
- Added .NET 8.0 runtime checks in deployment scripts
- Optimized package size through framework-dependent deployment

## [0.5.32] - 2025-06-05

### Fixed
- ExifToolReader now correctly handles Windows-1252 encoding from Ricoh G900 II
- Removed charset forcing in ExifTool arguments - handle encoding in code
- StudyID length limited to 16 characters (DICOM requirement)
- Barcode data no longer corrupted by incorrect UTF-8 interpretation

### Added
- QRBridge 2.0 development plan as CamBridge.QRBridge subproject
- Windows-1252 to UTF-8 conversion in ExifTool output processing
- Comprehensive encoding analysis and solution documentation

### Changed
- ExifToolReader uses raw byte stream processing for proper encoding handling
- ParseBarcodeData no longer performs character replacements (CleanBarcodeData removed)
- Updated pipeline architecture documentation with encoding details

### Technical Details
- Discovered QRBridge v1.0.1 explicitly removed UTF-8 forcing for camera compatibility
- Ricoh G900 II stores barcode data in Windows-1252 encoding
- ExifTool outputs in system default encoding when no charset specified
- Solution: Read ExifTool output as Windows-1252, not UTF-8

### Next Sprint
- Sprint 4.5: QRBridge 2.0 implementation as integrated CamBridge module
- Estimated 4-6 hours for working prototype
- Will solve encoding issues permanently with native UTF-8 support

## [0.5.31] - 2025-06-05

### Added
- Pipeline end-to-end testing with real Ricoh G900 II images
- Character encoding analysis for Barcode field
- PowerShell one-liner collection in PROJECT_WISDOM

### Fixed
- **CRITICAL:** ExifTool character encoding for Ricoh Barcode field
  - Ricoh uses Windows-1252 encoding, not UTF-8
  - Added `-charset Barcode=Latin1` parameter to ExifTool
  - Removed broken character replacement logic
- Study ID length validation (max 16 chars for DICOM SH)

### Changed
- ExifToolReader now properly handles Windows-1252 encoded data
- Improved logging for QRBridge data extraction

### Discovered
- Ricoh G900 II stores QRBridge data in APP5 RMETA segment
- Barcode field uses Windows-1252 encoding (0xF6=ö, 0xA0=NBSP)
- Pipeline successfully converts JPEG to DICOM without re-encoding

### Known Issues
- AUTO-generated IDs exceed 16 character DICOM limit
- Settings save button remains disabled (CAMB-001)
- Version.props assembly version conflict (CAMB-004)

## [0.5.30] - 2025-06-05

### Added
- Ultimate one-click deployment script with full build pipeline
- Deployment log with rebuild tracking (indented format)
- Automatic version detection from Version.props
- Test integration - launch Service/Config directly after deploy
- Deployment history management (keeps 10 versions)
- Auto-cleanup for old deployment packages

### Changed
- Deployment script now handles complete workflow: Clean → Build → Package → Test
- Enhanced deployment logging with user tracking and file sizes
- Improved user experience with pragmatic test options

### Fixed
- Identified critical gap: Core pipeline never tested
- Sprint priorities reorganized: Pipeline testing before new features

### Technical
- PowerShell deployment automation perfected
- Deploy-to-test workflow established
- Build artifacts properly organized

## [0.5.29] - 2025-06-05

### Added
- Professional deployment package with Install/Uninstall scripts
- Event Log debugging for Windows Service startup
- Working directory management for service mode
- Serilog.Sinks.EventLog integration
- README-Deployment.md with comprehensive instructions
- Create-DeploymentPackage.ps1 for automated packaging

### Fixed
- Windows Service now starts correctly (Error 1053 resolved)
- Configuration file path resolution for Windows Service mode
- Service working directory set to executable location
- ExifTool and dependencies included in publish output

### Changed
- Enhanced service vs console mode detection
- Service logs now written to %ProgramData%\CamBridge\Logs
- Improved error reporting during service initialization
- Service binary location moved to publish folder

### Deployment
- Professional Install-CamBridge.ps1 with shortcuts and service configuration
- Uninstall-CamBridge.ps1 with data preservation option
- Automated deployment package creation
- Service recovery options configured

## [0.5.28] - 2025-06-05

### Added
- Windows Service support in Program.cs (STEP 5 activated)
- Service deployment scripts (Build-Deployment.ps1, Test-Service.ps1)
- Absolute paths documentation in PROJECT_WISDOM
- Protected task CAMB-005 for Service Deployment

### Changed
- Service now detects Windows Service vs Console mode automatically
- Updated ServiceManager.cs with comprehensive service exe search paths
- Config UI builds to x64 platform target
- Prioritized Windows Service fix over Dead Letters feature

### Fixed
- About Dialog background color (now matches app theme)
- About Dialog Vogon Poetry Easter Egg animation
- Program.cs version updated to 0.5.27

### Known Issues
- Windows Service fails to start with Error 1053 (timeout)
- Service runs perfectly in console mode but not as Windows Service
- Assembly version conflict (looking for 0.5.27.0, finding 0.0.1.0)
- ExifTool perl5*.dll warning (non-critical)

## [0.5.27] - 2025-06-05
### Added
- Vogon poetry easter egg in About dialog (5 clicks on logo)
- Hyperlink to Ricoh G900 II product page
- Dramatic fade animations for easter egg reveal

### Changed  
- About dialog background to light theme (consistent with app)
- About dialog to minimalist design (removed non-functional buttons)
- Version display hardcoded to avoid assembly conflicts
- Updated all references from "Ricoh Cameras" to "Ricoh G900 II"

### Fixed
- Dark/black background issue in About dialog
- Assembly version conflicts preventing app startup
- PowerShell syntax errors in build commands

### Technical
- Minimal Version.props without AssemblyVersion to prevent conflicts
- Assembly remains at 0.0.1.0, display shows 0.5.27
- Removed complex Vogon Poetry window (400+ lines)

## [0.5.26] - 2025-06-04

### Added
- Complete ViewModel initialization for all Config UI pages
- Drag & drop event handlers for MappingEditorPage
- Proper error handling and user feedback in all pages
- Resource cleanup in page unload events
- Fallback implementations for DI container failures

### Fixed
- Navigation not setting ViewModels for pages
- HttpApiService constructor parameter mismatch
- ServiceControlPage duplicate member definitions
- DeadLettersPage missing load method handling
- AboutPage missing ExitButton_Click handler
- MappingEditorPage missing drag & drop handlers

### Changed
- All pages now properly retrieve ViewModels from DI container
- Improved error messages with actionable user guidance
- Standardized page initialization pattern across all views
- Enhanced debug output for troubleshooting

### Technical
- Removed test projects from solution
- Fixed WPF temp file build issues
- Proper HttpClient configuration for API services
- Clean separation of generated and manual code

## [0.5.25] - 2025-06-04

### Fixed
- Critical UI freeze when opening Mapping Editor
- Removed blocking sync call in MappingConfigurationLoader constructor
- Fixed async initialization pattern in MappingEditorViewModel
- Improved path resolution for mappings.json file

### Changed
- MappingConfigurationLoader now uses lazy initialization
- MappingEditorViewModel initializes asynchronously via InitializeAsync()
- Better error handling and status messages in Mapping Editor

### Added
- Drag & drop support in Mapping Editor
- Template application (Ricoh G900, Minimal, Full)
- Live preview for mapping transformations
- Professional UI ready for demonstrations

## [0.5.24] - 2025-06-04
### Added
- Extension methods for UI-specific MappingRule functionality
- MappingConfigurationExtensions for async method compatibility
- Mock recent activity display in dashboard
- Auto-refresh timer for dashboard (5 second interval)

### Fixed
- Service uptime calculation using actual start time instead of Windows boot time
- Config UI interface mismatches with Core v0.5.x
- Health check endpoint path in HttpApiService (/health instead of /api/status/health)
- Dashboard XAML bindings to match ViewModel property names
- Program.cs top-level statement scope issues

### Changed
- MappingEditorViewModel to work directly with Core structures
- MappingEditorPage drag & drop to create Core-compatible rules
- UI templates to generate proper v0.5.x mapping rules
- Dashboard to show live statistics from service

### Technical
- HTTP API fully functional on port 5050
- Dashboard successfully connects and displays real-time data
- Config UI adapted to Core without adapter anti-pattern
- Recent activity shows mock conversion data

### Known Issues
- Mapping Editor tab causes UI to freeze (debugging needed)

## [0.5.24] - 2025-06-04 - HTTP API Verified & Config UI Analysis
### Added
- ConnectionTest.cs for API verification
- Detailed Config UI interface mismatch analysis
- Fix plan for Config UI to Core alignment

### Fixed
- Service uptime calculation (now uses serviceStartTime instead of Environment.TickCount64)
- Program.cs incorrect uptime display showing days instead of minutes

### Changed
- Sprint 2 plan adjusted to focus on minimal demo UI
- Priority shifted to Dashboard + Service Control for prototype

### Discovered
- Config UI is feature-complete with all views implemented
- Only interface mismatches prevent compilation (v0.4.x vs v0.5.x)
- HTTP API fully functional and ready for UI connection
- GitHub can be outdated during active development

### Technical
- Verified API endpoints: /health and /api/status working perfectly
- CORS properly configured for Config UI connection
- All statistics correctly transmitted via JSON
- Connection test shows 100% success rate

## [0.5.23] - 2025-06-04 - Sprint 1 Complete! 🎉
### Added
- End-to-end pipeline successfully processes Ricoh JPEG files
- Folder watching service monitors input directories
- Processing queue with retry logic
- DICOM validation after creation
- Structured logging with Serilog
- Progressive feature activation in Program.cs
- HTTP API for Config UI communication (SCHRITT 2+3 activated)
- Health check endpoint at `/health`
- Status API endpoint at `/api/status`
- CORS support for Config UI connection
- Comprehensive .gitignore for clean repository
- Source code header standard (path, version, copyright)

### Fixed
- ExifTool perl DLL dependencies resolved
- Service compilation errors (missing NuGet packages)
- StudyID length validation (max 16 chars)
- Entity constructor parameter names
- Build cache issues
- Program.cs step comments cleaned up (removed confusing "JETZT AKTIV!")

### Changed
- Temporary encoding fix for German umlauts
- Default mapping rules when mappings.json missing
- Service runs in Development mode by default
- Service now listens on port 5050 for API requests
- All component versions synchronized to v0.5.23
- Repository cleaned with .gitignore (no more obj/bin folders in Git)

### Technical
- Successfully reads Ricoh Barcode EXIF field
- Parses QRBridge data: EX002|Schmidt, Maria|1985-03-15|F|Röntgen Thorax
- Creates valid DICOM files viewable in standard viewers
- Output structure: {PatientID}\{Date}\{PatientID}_{Date}_{Instance}.dcm
- StatusController base implementation in Startup.cs
- DailySummaryService activated
- Config UI can now connect to running service

### Known Issues
- Encoding not perfect (Windows-1252 to UTF-8)
- Missing datetime field warnings in mapping
- Dead letter management API endpoints TODO
- Active processing count not yet implemented

## [0.5.22] - 2025-06-04

### Added
- Complete ExifToolReader implementation with barcode support
- ExtractMetadataAsync method matching FileProcessor expectations
- Constructor matching ServiceCollectionExtensions (logger, timeoutMs)
- Parse QRBridge data from Ricoh Barcode EXIF field
- Handle duplicate EXIF keys with automatic renaming
- Temporary encoding fix for German umlauts

### Fixed
- StudyId shortened to comply with 16 char DICOM limit
- Dictionary key conflicts with encoding artifacts
- FileProcessor integration issues

### Changed
- ExifToolReader now primary metadata extraction method
- Barcode field prioritized over UserComment

### Technical
- PipelineTest passes successfully
- Test output: EX002|Schmidt, Maria|1985-03-15|F|Röntgen Thorax

## [0.5.21] - 2025-06-04

### Added
- Direct ExifTool testing in PipelineTest
- Barcode field discovery documentation

### Fixed
- ExifToolReader file path issues
- Build errors in PipelineTest project

### Changed
- Focus shifted from UserComment to Barcode field
- ExifToolReader implementation updated for new field

### Discovered
- Ricoh G900SE II stores QRBridge data in Barcode field, not UserComment
- UserComment only contains "GCM_TAG" marker

## [0.5.20] - 2025-06-03
### Added
- CLAUDE-TO-CLAUDE Command System for better self-guidance
- WISDOM Garbage Collection process for VOGON EXIT
- Architecture Manifest with original 12-phase plan
- Development Roadmap 2.0 (realistic 40% progress assessment)
- Project Philosophy documentation
- PLAN: command for flexible development steering
- Agile workflow concepts exploration

### Changed
- Enhanced VOGON INIT with Filetree awareness
- Updated ExifTool pipeline (not yet tested)
- Refined self-instructions with token investment rules
- Consolidated lessons learned (15% WISDOM compression)

### Fixed
- Clarified WinUI 3 → WPF drift was pragmatic decision
- Corrected false assumptions about missing Entity properties
- Updated blockading problems with correct assessment

### Meta
- Established "Journey through semantic space" philosophy
- Recognized we're building both software AND collaboration patterns
- 180€/month well invested in development OS innovation

## [0.5.19] - 2025-06-03 20:30
### Added
- ExifToolReader implementation (without IExifReader interface)
- WISDOM Priority System for project knowledge management
  - 🔒 [CORE] tags for permanent information
  - ⚡ [URGENT] tags for current sprint items  
  - 💡 [LESSON] tags for learned experiences
  - 📝 [TEMP] tags for disposable code
- Strong CLAUDE self-instructions section
- Pipeline architecture documentation
- Git-inspired tagging system

### Changed
- FileProcessor to use ExifToolReader directly
- ServiceCollectionExtensions - removed composite/fallback logic
- TestConsole adapted for new pipeline architecture
- Complete refactor of PROJECT_WISDOM.md structure
- Clear action plan with time estimates

### Fixed
- Syntax error in ExifToolReader (method placement)

### Technical
- BREAKING: IExifReader interface removed - direct dependency only
- Architecture: JPEG → ExifToolReader → ImageMetadata → DicomConverter → DICOM
- Identified missing entities blocking compilation:
  - ImageTechnicalData.cs (missing)
  - ImageMetadata.cs (needs TechnicalData property)
  - StudyInfo.cs (needs ExamId property)

## [0.5.18] - 2025-06-03 16:50

### Changed
- MAJOR: Simplified EXIF pipeline to use ExifTool exclusively
- Removed IExifReader interface and all implementations
- Removed ExifReader (MetadataExtractor-based)
- Removed RicohExifReader (workaround reader)
- Removed CompositeExifReader (unnecessary complexity)

### Added
- ExifToolService as the single EXIF solution
- QRBridgeParser for centralized parsing logic
- Enhanced ImageMetadata as central domain object
- Simplified FileProcessor for linear pipeline

### Fixed
- GCM_TAG prefix handling (both "GCM_TAG " and "GCM_TAG")
- Character encoding issues with German umlauts
- Eliminated code duplication in parsing logic

### Technical
- Pipeline now: ExifToolService → ImageMetadata → DICOM
- No more fallback readers or composite patterns
- Requires ExifTool to be installed (hard requirement)

## [0.5.17] - 2025-06-03 16:00
### Added
- ExifToolReader with automatic discovery and caching
- CompositeExifReader for fallback chain (ExifTool → Ricoh → Standard)
- DicomTagMapper service with transform functions
- IDicomTagMapper interface for dependency injection
- MapToDataset method for DICOM tag mapping
- ValueTransform enum for transformation types

### Fixed
- GCM_TAG prefix removal now handles both "GCM_TAG " and "GCM_TAG" variants
- MappingRule now has all required properties (Description, ValueRepresentation, etc.)
- Transform property made string-based for XAML compatibility
- ServiceCollectionExtensions registration issues
- DicomConverter MapToDataset parameter order and flattening

### Technical
- Core and Infrastructure projects now build successfully
- Added fo-dicom package to Core project
- Implemented systematic error handling in ExifToolReader
- Added encoding fixes for German umlauts

### Testing Status
- ✅ Core project builds
- ✅ Infrastructure project builds
- ❌ Config project has XAML/ValueTransform binding issues
- Next: Fix XAML bindings before proceeding to DICOM tests

[0.5.16] - 2025-06-02 00:30
### Added

Console logging for service debugging
Detailed error messages for DICOM conversion failures
Pipeline analysis and systematic development plan

### Fixed

Settings page DataContext initialization
PropertyChanged event handler (removed problematic OnPropertyChanged call)
Dashboard API connection (shows green/connected status)

### Discovered

ExifToolReader not properly integrated - uses MetadataExtractor only
GCM_TAG prefix causes DICOM validation errors
Missing Transform functions in mapping engine
File logging not working despite configuration
Retry logic tries to process already moved files

### Changed

Identified systematic approach needed: JPEG→ExifTool→Parse→Map→DICOM
Decision to implement ExifToolReader properly instead of patching RicohExifReader
New sprint plan focusing on pipeline components one at a time

### Testing

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

## [0.5.12] - 2025-06-02, 22:50

### Fixed
- Navigation: Registered MappingEditor and AboutPage in NavigationService
- DataContext now correctly set via DI for MappingEditorViewModel
- Fixed XAML parse error by replacing Symbol="Up/Down" with Unicode arrows
- Resolved build errors by removing MockConfigurationService references

### Changed
- MappingEditor buttons now use ▲▼ instead of SymbolIcons for better compatibility
- Replaced PreviewInputChanged method call with property setter

### Tested
- ✅ Mapping Editor opens without crash
- ✅ Templates (Ricoh G900, Minimal, Full) work correctly
- ✅ Drag & Drop from Source Fields functional
- ✅ Add Rule creates new mappings
- ✅ Import/Export/Save dialogs open properly
- ✅ Modified flag resets on Save
- ⚠️ Rule Properties panel not yet responsive (Fix in v0.5.13)

### Developer Notes
- Always check NavigationService Dictionary first for navigation issues
- ModernWpfUI has limited symbol names - Unicode often safer
- "Check the obvious first" - 1-line fix solved complex problem

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

# WISDOM_TECHNICAL.md - Technical Wisdom Distilled
**Version**: 0.7.27  
**Purpose**: HOW to build, fix, deploy, activate hidden features  
**Philosophy**: KISS, Tab-Complete, Sources First, Pipeline Isolation  
**Reality**: 74 Sessions + Sprint 18 treasure activation!
**Updated**: 2025-06-20 Sprint 18 - Hidden Treasures + UI Polish

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
  "VOGON EXIT mit v0.7.27 release"

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

Hidden Systems Found:
  Notifications: Webhook + Email config ready
  Service Mgmt: Install/Uninstall complete ✓
  Monitoring: Health checks active
  Import/Export: JSON fully working ✓
  Auto-Backup: Silent on every save ✓
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

# NEW - After Sprint 18: Service install via UI!
# No more command line needed!

# Monitor logs live
Get-Content "$env:ProgramData\CamBridge\logs\service-*.log" -Tail 50 -Wait

# Check configuration
Test-Json -Path "$env:ProgramData\CamBridge\appsettings.json"
cat "$env:ProgramData\CamBridge\appsettings.json" | ConvertFrom-Json
```

### API Testing Arsenal
```powershell
# Quick API health check
Invoke-RestMethod "http://localhost:5111/api/status" | ConvertTo-Json -Depth 5

# Specific endpoints (all working in v0.7.27)
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
# Example: irm "http://localhost:5111/api/pipelines/abc123-def456"

# NEW ENDPOINTS (add in Sprint 19)
irm "$base/pipelines/{id}/enable" -Method POST   # Enable pipeline
irm "$base/pipelines/{id}/disable" -Method POST  # Disable pipeline
irm "$base/statistics"      # Currently 404, implement in Sprint 19

# Check specific pipeline
$status = irm "$base/status"
$status.pipelines | Where-Object { $_.name -eq "Radiology" }

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

# Find event definitions
Select-String "event.*EventHandler" src\**\*.cs

# Find complete but commented code blocks
Select-String "^[\s]*//.*\{[\s]*$" src\**\*.cs -Context 0,20

# Find config classes
Select-String "class.*Settings|class.*Config" src\**\*.cs

# NEW - Find hidden converters
Select-String "class.*Converter" src\CamBridge.Config\Converters\*.cs |
  ForEach-Object { $_.Line -match "class\s+(\w+)" | Out-Null; $matches[1] } |
  ForEach-Object { 
    $converter = $_
    $used = Select-String $converter src\**\*.xaml
    if (-not $used) { "UNUSED: $converter" }
  }
```

## 🎯 PATTERN MASTERY - FROM PAIN TO WISDOM

### The Hidden Treasures Pattern ⭐ NEW!
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

Real Example - Transform System:
```
```csharp
// User: "We need data transformation preview"
// Me: "Let me design a transform system..."
// Then: "Wait, let me search first..."

// Found in ValueTransform.cs:
public enum ValueTransform
{
    None,
    DateToDicom,      // Already implemented!
    TimeToDicom,      // Already implemented!
    MapGender,        // Already implemented!
    ExtractDate,      // Already implemented!
    // ... 7 more transforms ready to use!
}

// Found in DicomTagMapper.cs:
public string ApplyTransform(string value, ValueTransform transform)
{
    return transform switch
    {
        ValueTransform.DateToDicom => ConvertDateToDicom(value),
        ValueTransform.MapGender => MapGenderValue(value),
        // ... all implemented!
    };
}

// Result: Just needed UI dialog - 2 hours vs 2 weeks!
```

### The Minimal Pattern ⭐ POWERFUL!
```csharp
// When complex debugging fails after 3+ hours
// Signal: "ich werde bald wahnsinnig"
// Solution: DELETE complexity, write minimal

// BEFORE - Dashboard with abstractions (not working)
public class DashboardViewModel : ViewModelBase
{
    private readonly IApiService _apiService;
    private readonly INavigationService _navigationService;
    private readonly ILogger<DashboardViewModel> _logger;
    
    public async Task InitializeAsync()
    {
        try
        {
            await _navigationService.EnsureInitializedAsync();
            var context = await _apiService.GetContextAsync();
            await RefreshDataAsync(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize");
        }
    }
    // ... 200+ lines of "proper" code
}

// AFTER - Direct and simple (WORKING!)
public class DashboardViewModel : ViewModelBase
{
    private readonly HttpClient _httpClient = new();
    private readonly DispatcherTimer _timer;
    
    public DashboardViewModel()
    {
        // Timer starts immediately - no async init!
        _timer = new DispatcherTimer 
        { 
            Interval = TimeSpan.FromSeconds(5) 
        };
        _timer.Tick += async (s, e) => await RefreshAsync();
        _timer.Start();
        
        // Initial load
        _ = RefreshAsync();
    }
    
    private async Task RefreshAsync()
    {
        try
        {
            var json = await _httpClient.GetStringAsync(
                "http://localhost:5111/api/status");
            var status = JsonSerializer.Deserialize<StatusResponse>(
                json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
            
            // Update UI properties
            ServiceStatus = status.Service.Status;
            PipelineStatuses = status.Pipelines;
        }
        catch { /* Silent fail, try next tick */ }
    }
}
// Result: Working in 15 minutes!
```

### Pipeline Isolation Pattern ⭐ CRITICAL FOR MEDICAL!
```csharp
// WRONG - Singleton FileProcessor (Session 1-67)
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<FileProcessor>(); // SHARED STATE!
    services.AddSingleton<ProcessingQueue>();
}

// WRONG - Trying to inject pipeline config
public class FileProcessor
{
    public FileProcessor(IConfiguration config) // Which pipeline?!
    {
        // Confusion and bugs
    }
}

// RIGHT - Per-pipeline instances (Session 68+)
public class PipelineManager
{
    public async Task StartPipelineAsync(PipelineConfiguration config)
    {
        // Each pipeline gets OWN instances
        var fileProcessor = new FileProcessor(
            _loggerFactory.CreateLogger<FileProcessor>(),
            _exifToolReader,      // Shared service OK (readonly)
            _dicomConverter,      // Shared service OK (readonly)
            config,               // Pipeline-specific config!
            _globalDicomSettings  // With pipeline overrides
        );
        
        var queue = new ProcessingQueue(
            _loggerFactory.CreateLogger<ProcessingQueue>(),
            fileProcessor,        // Pipeline's own processor
            config.ProcessingOptions
        );
        
        var watcher = new FileSystemWatcher(config.WatchFolder)
        {
            Filter = "*.jpg",
            NotifyFilter = NotifyFilters.FileName
        };
        
        // Completely isolated pipeline!
        _pipelines[config.Id] = new PipelineContext(
            fileProcessor, queue, watcher);
    }
}

// Learning: Medical data MUST be isolated
// No cross-contamination between departments!
```

### Event Handler Connection Pattern (Session 72)
```csharp
// PROBLEM: Drag&Drop not working
// XAML looked correct:
<ListBox x:Name="MappingRules" 
         AllowDrop="True"
         Drop="MappingRules_Drop"
         DragOver="MappingRules_DragOver">

// BUT: Code-behind was missing connections!
// SOLUTION: Connect in constructor
public MappingEditorPage()
{
    InitializeComponent();
    DataContext = _viewModel;
    
    // CRITICAL: Connect drag&drop to source items!
    Loaded += (s, e) =>
    {
        foreach (var item in SourceFieldsList.Items)
        {
            if (SourceFieldsList.ItemContainerGenerator
                .ContainerFromItem(item) is ListBoxItem container)
            {
                container.MouseMove += SourceField_MouseMove;
                container.GiveFeedback += SourceField_GiveFeedback;
            }
        }
    };
}

private void SourceField_MouseMove(object sender, MouseEventArgs e)
{
    if (e.LeftButton == MouseButtonState.Pressed)
    {
        var item = (sender as ListBoxItem)?.DataContext as SourceField;
        if (item != null)
        {
            var data = new DataObject(typeof(SourceField), item);
            DragDrop.DoDragDrop((DependencyObject)sender, 
                data, DragDropEffects.Copy);
        }
    }
}
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

### 🎯 Sprint 18 Status (Priority 1 COMPLETE, Priority 2.1 COMPLETE!)
```yaml
Priority 1 Results:
  ❌ Daily Summary Service - Architectural mismatch
  ✅ Hidden API Documented - /api/pipelines/{id}
  ✅ Auto-Backup Visible - Users informed
  ✅ Import/Export Prominent - With shortcuts
  
Priority 2 Progress:
  ✅ Service Installer UI - No more command line!
  ⏳ Test Mapping - Already visible from 1.4
  ⏳ Pipeline Health Warnings - Next up
  
Current Score: 4 of 5 features activated!
Time spent: ~90 minutes
Value delivered: Massive!
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

#### Apply/Reset Pipeline Buttons
```yaml
Location: PipelineConfigViewModel commands
Status: Commands exist but change tracking broken
Problem: SelectedPipelineHasChanges never becomes true
Reality: Individual pipeline tracking not working
Solution: Removed - "Save All Pipelines" is clearer
Lesson: Test UI functionality, not just existence!
```

### 🚀 INSTANT ACTIVATIONS (Under 5 Minutes Each!)

#### 1. Daily Summary Service Activation ⏱️ 1 minute
```csharp
// Location: src\CamBridge.Service\Program.cs:232
// Current state:
// services.AddHostedService<DailySummaryService>();

// ACTIVATION:
services.AddHostedService<DailySummaryService>();  // Just uncomment!

// Test:
// 1. Rebuild & deploy: 1[TAB]
// 2. Check logs next day at 8 AM
// 3. See summary in Event Log (email stub for now)
```

#### 2. Service Installer UI
```xml
<!-- Location: src\CamBridge.Config\Views\ServiceControlPage.xaml -->
<!-- Add after existing buttons (Line ~150): -->

<Button Content="Install Service" 
        Command="{Binding InstallServiceCommand}"
        IsEnabled="{Binding IsServiceInstalled, Converter={StaticResource InverseBooleanConverter}}"
        Margin="0,5,0,0" />
        
<Button Content="Uninstall Service"
        Command="{Binding UninstallServiceCommand}" 
        IsEnabled="{Binding IsServiceInstalled}"
        Margin="0,5,0,0" />

<!-- Commands already exist in ViewModel! -->
<!-- ServiceManager.cs has complete implementation! -->
<!-- Includes auto-recovery configuration! -->
```

#### 3. Show Hidden Import/Export
```yaml
Location: MappingEditorPage.xaml
Current: Buttons exist but hard to find
Fix: Add tooltips or make prominent
<!-- Add to existing buttons: -->
ToolTip="Import mapping rules from JSON file"
ToolTip="Export current mapping rules to JSON"
```

### 🔧 QUICK WINS (Under 1 Hour Each)

#### 4. Enable Runtime Pipeline Control
```csharp
// Location: src\CamBridge.Service\Program.cs
// Add new endpoints after line 300:

endpoints.MapPost("/api/pipelines/{id}/enable", async context =>
{
    var pipelineId = context.Request.RouteValues["id"]?.ToString();
    var pipelineManager = context.RequestServices.GetService<PipelineManager>();
    
    if (string.IsNullOrEmpty(pipelineId) || pipelineManager == null)
    {
        context.Response.StatusCode = 400;
        return;
    }
    
    await pipelineManager.EnablePipelineAsync(pipelineId);
    context.Response.StatusCode = 200;
});

endpoints.MapPost("/api/pipelines/{id}/disable", async context =>
{
    var pipelineId = context.Request.RouteValues["id"]?.ToString();
    var pipelineManager = context.RequestServices.GetService<PipelineManager>();
    
    if (string.IsNullOrEmpty(pipelineId) || pipelineManager == null)
    {
        context.Response.StatusCode = 400;
        return;
    }
    
    await pipelineManager.DisablePipelineAsync(pipelineId);
    context.Response.StatusCode = 200;
});

// Then add toggle buttons to Dashboard!
```

#### 5. Webhook Configuration UI
```xml
<!-- Add to NotificationSettings section in PipelineConfigPage.xaml -->
<GroupBox Header="Webhook Notifications" Margin="0,10,0,0">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>
        
        <CheckBox Grid.Row="0" Content="Enable Webhooks"
                  IsChecked="{Binding SelectedNotifications.Webhook.Enabled}" />
                  
        <TextBox Grid.Row="1" Margin="0,5,0,0"
                 Text="{Binding SelectedNotifications.Webhook.Url}"
                 ui:ControlHelper.PlaceholderText="https://hooks.slack.com/..." />
                 
        <ComboBox Grid.Row="2" Margin="0,5,0,0"
                  ItemsSource="{x:Static local:HttpMethods}"
                  SelectedItem="{Binding SelectedNotifications.Webhook.Method}" />
    </Grid>
</GroupBox>

<!-- Webhook model already complete in CamBridgeSettingsV2! -->
```

#### 6. Show Auto-Backup Status
```csharp
// ConfigurationService already creates backups!
// Just inform the user:

// Add to SaveAllAsync in PipelineConfigViewModel:
StatusMessage = "Configuration saved (backup created)";

// Or add label to UI:
<TextBlock Text="✓ Automatic backups enabled" 
           Foreground="Green" 
           FontSize="10"
           Margin="5,0,0,0" />
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
  
  // Added validation:
  private void ValidateEnumValues<T>(T value, string name) where T : Enum
  {
      if (!Enum.IsDefined(typeof(T), value))
      {
          var validValues = string.Join(", ", Enum.GetNames(typeof(T)));
          throw new ConfigurationException(
              $"Invalid {name}: '{value}'. Valid values: {validValues}");
      }
  }
  
  Valid OutputOrganization: None, ByPatient, ByDate, ByPatientAndDate
  NOT: PatientName, Date, Patient (old values)
```

### Category 2: Architecture Evolution
```yaml
Fix #7-8: The Great Interface Purge
  Journey: 12+ interfaces → 2 interfaces
  Deleted: ~2000 lines of abstraction
  
  // BEFORE (Session 1-50)
  public interface IFileProcessor { }
  public interface IExifReader { }
  public interface IDicomConverter { }
  public interface INotificationService { }
  public interface IDeadLetterService { }
  // ... 7 more unused interfaces
  
  // AFTER (Session 67+)
  services.AddSingleton<ExifToolReader>();    // Direct!
  services.AddSingleton<DicomConverter>();    // Direct!
  services.AddSingleton<NotificationService>(); // Direct!
  // Only 2 interfaces remain (for now)
  
  Learning: Interfaces without multiple implementations = DELETE

Fix #8: Pipeline Isolation Architecture
  Problem: FileProcessor was singleton but needs pipeline config
  Medical Risk: Department A seeing Department B data
  
  Solution: Each pipeline creates own FileProcessor
  // See Pipeline Isolation Pattern above
  
  Key Insight: Shared services OK if stateless/readonly
  Pipeline-specific: FileProcessor, Queue, Watcher
  Shared OK: ExifToolReader, DicomConverter (no state)
```

### Category 3: The Dashboard Debugging Saga
```yaml
Fix #9-12: Why Dashboard Wouldn't Work
  Total Debug Time: 3+ hours
  Frustration Level: "ich werde bald wahnsinnig"
  
  Problem Trail:
  1. NavigationService created pages without ViewModels
     Fix: Dependency injection in page constructors
     
  2. InitializeAsync never called
     // WRONG - complex pattern
     protected override async void OnDataContextChanged(...)
     {
         if (DataContext is DashboardViewModel vm)
             await vm.InitializeAsync();
     }
     
     // RIGHT - simple pattern
     public DashboardViewModel()
     {
         _timer.Start(); // Start immediately!
     }
  
  3. API case sensitivity
     API sends: { "name": "radiology" }
     Code expected: { "Name": "radiology" }
     Fix: PropertyNameCaseInsensitive = true
  
  4. Complex abstractions hiding simple problems
     Removed: IApiService, INavigationContext, etc
     Result: 50 lines of working code
  
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
    
  Navigation Dropdown:
    Problem: Frame showing growing history dropdown
    User: "diese komische tableiste nervt"
    
    Fix: Two-part solution
    <!-- 1. Hide navigation UI -->
    <Frame x:Name="ContentFrame" 
           NavigationUIVisibility="Hidden"/>
    
    // 2. Clear history after navigation
    private void NavigateToPage(Type pageType)
    {
        ContentFrame.Navigate(pageType);
        while (ContentFrame.NavigationService.CanGoBack)
            ContentFrame.NavigationService.RemoveBackEntry();
    }

Fix #17-21: XAML Property Support
  Run elements don't support Opacity:
    WRONG: <Run Text="→" Opacity="0.7"/>
    RIGHT: Put Opacity on TextBlock, not Run!
    
  Run elements are INLINE text only:
    - No Opacity
    - No Margin  
    - No layout properties
    - Just Text, Foreground, FontFamily, FontSize, FontWeight
    
  PlaceholderText needs prefix:
    WRONG: <TextBox PlaceholderText="Enter..."/>
    RIGHT: <TextBox ui:ControlHelper.PlaceholderText="Enter..."/>
  
  Dialog patterns differ:
    Window: dialog.ShowDialog()
    ContentDialog: await dialog.ShowAsync()
    
  Learning: XAML has specific rules - memorize them!
```

### Category 5: Hidden Feature Issues
```yaml
Fix #22: Webhook Headers Not Saved
  Issue: Dictionary<string,string> serialization
  Fix: Use custom converter or key-value pairs
  
Fix #23: Email Password Storage
  Issue: Plain text in config (commented "Should be encrypted")
  Fix: Use DPAPI or certificate encryption
  Time: 1 hour
  
Fix #24: Pipeline-Specific Settings Not Wired
  Issue: Properties exist but not connected to pipeline
  Fix: Add to PipelineConfiguration or use metadata
  
Fix #25: Test Mapping Sample Data
  Issue: Hardcoded test data
  Fix: Load from actual recent processing
  Or: Make test data configurable
```

## 🏗️ BUILD & DEPLOY MASTERY

### Version Management - Single Source of Truth
```xml
<!-- Version.props - NEVER hardcode versions! -->
<Project>
  <PropertyGroup>
    <VersionPrefix>0.7.27</VersionPrefix>
    <FileVersion>0.7.27.0</FileVersion>
    <AssemblyVersion>0.7.0.0</AssemblyVersion>
    <InformationalVersion>0.7.27 - Sprint 18 Hidden Treasures Activation</InformationalVersion>
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
        
        return "0.7.27"; // Emergency fallback only
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
      └── service-20250618.log

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

NEW - Backup Location:
  %ProgramData%\CamBridge\
  └── appsettings.json.backup_20250619_143022
      # Auto-created on every save!
```

### Deployment Script Patterns
```powershell
# Build-CamBridge.ps1 excerpts
param(
    [string]$Action = "Build",
    [switch]$NoZip
)

# Version management
$versionProps = [xml](Get-Content ".\Version.props")
$version = $versionProps.Project.PropertyGroup.VersionPrefix

# Build with clean
Write-Host "Building CamBridge v$version..." -ForegroundColor Cyan
dotnet clean --verbosity minimal
dotnet build --configuration Release --no-incremental

# Deploy pattern
if ($Action -eq "Deploy") {
    # Stop service gracefully
    if (Get-Service CamBridgeService -ErrorAction SilentlyContinue) {
        Stop-Service CamBridgeService -Force
        Start-Sleep -Seconds 2
    }
    
    # Copy with backup
    $backup = "C:\CamBridge\Backup\$(Get-Date -Format 'yyyyMMdd-HHmmss')"
    Copy-Item "C:\CamBridge\Service" $backup -Recurse
    
    # Deploy new files
    Copy-Item ".\publish\*" "C:\CamBridge\Service\" -Recurse -Force
    
    # Start service
    Start-Service CamBridgeService
}

# NEW - After Sprint 18: UI handles service install!
# No more manual sc.exe commands needed!
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

# Common errors and fixes:
# "Port 5111 is already in use" → Kill other process
# "Configuration section not found" → Add CamBridge wrapper
# "Invalid enum value" → Check OutputOrganization values
# "Access denied" → Run as administrator once
# "Unable to resolve ProcessingQueue" → DailySummaryService issue!

# 3. Validate configuration
$config = Get-Content "$env:ProgramData\CamBridge\appsettings.json"
$config | ConvertFrom-Json  # Will show parse errors

# 4. Run in console mode for live debugging
Set-Location "C:\CamBridge\Service"
.\CamBridge.Service.exe console

# 5. Check permissions
icacls "C:\CamBridge\Watch" /T
icacls "$env:ProgramData\CamBridge" /T

# NEW - Check if daily summary is running
Get-EventLog -LogName Application -Source CamBridge* |
    Where-Object { $_.Message -like "*Daily Summary*" }
```

### When Build Shows "Fixed" Errors
```powershell
# The nuclear option - when build cache is corrupted
# Use when: Build shows errors for code you KNOW is fixed

# 1. Close Visual Studio completely
# 2. Clean EVERYTHING
Remove-Item -Recurse -Force */obj, */bin, .vs

# 3. Verify file actually saved
Get-Content "src\CamBridge.Config\Pages\MappingEditorPage.xaml" | 
    Select-String "NavigationUIVisibility"

# 4. Search for all occurrences
Get-ChildItem -Recurse -Include *.xaml,*.cs |
    Select-String "OldPropertyName"

# 5. Rebuild with no cache
dotnet build --no-incremental --verbosity detailed

# 6. If still failing, check for:
# - Files open in another editor
# - Source control conflicts
# - Antivirus locking files
```

### When UI Doesn't Work - Checklist
```yaml
Drag & Drop Not Working:
  1. AllowDrop="True" on target?
  2. Event handlers in XAML?
  3. Handlers connected in code-behind?
  4. Check ItemContainerGenerator timing
  5. Use Loaded event for safety

Binding Not Updating:
  1. INotifyPropertyChanged implemented?
  2. SetProperty called?
  3. Correct DataContext?
  4. Check Output window for binding errors
  5. Use Live Property Explorer

Converter Issues:
  1. Converter type matches data type?
  2. ConverterParameter spelled correctly?
  3. Converter in Resources?
  4. Check Convert method signature

Commands Not Firing:
  1. RelayCommand attribute on method?
  2. CanExecute returning true?
  3. CommandParameter correct type?
  4. DataContext has the command?

Dialog Won't Show:
  1. Window → ShowDialog()
  2. ContentDialog → ShowAsync()
  3. Owner set for modal?
  4. Check thread (UI thread only)

NEW - Hidden Features Not Working:
  1. Is backend implementation complete?
  2. Is ViewModel command created?
  3. Is UI element bound correctly?
  4. Is feature enabled in config?
  5. Check Event Log for errors
```

### Performance Profiling Basics
```csharp
// Simple timing pattern
var sw = Stopwatch.StartNew();
await ProcessFileAsync(file);
_logger.LogInformation("Processing took {Ms}ms", sw.ElapsedMilliseconds);

// Memory check pattern
var before = GC.GetTotalMemory(false);
await ProcessLargeDataSet();
var after = GC.GetTotalMemory(false);
_logger.LogInformation("Memory used: {MB}MB", (after - before) / 1048576);

// Channel monitoring
_logger.LogInformation("Queue depth: {Count}", _channel.Reader.Count);

// NEW - Pipeline health monitoring (already active!)
if (failureRate > 0.5)
    _logger.LogWarning("Pipeline {Name} has high failure rate: {Rate:P}", 
        pipeline.Name, failureRate);
```

## 📊 METRICS & PATTERNS

### Evolution Metrics
```yaml
What Changed (v0.1 → v0.7.27):
  Interfaces: 12+ → 2 (-85%)
  Build Time: 3min → 20sec (-89%)  
  Debug Time: Hours → Minutes (minimal pattern)
  Code: 15,940 → 14,850 LOC (-7% but cleaner)
  Complexity: High → Low (KISS wins)
  Features Hidden: Unknown → 30+ found!
  Features Activated: 11 (transform) + 4 (Sprint 18)
  User Satisfaction: Frustrated → Happy

Key Victories:
  - Tab-Complete: 90% less typing
  - Pipeline Isolation: Medical safety
  - Hidden Features: Weeks saved
  - UI Simplification: 40% more space
  - Sprint 18: No more CLI for service!

NEW - Sprint 18 Results:
  ✅ Hidden API Documented: /api/pipelines/{id}
  ✅ Auto-Backup Display: Professionally integrated
  ✅ Import/Export Visibility: Prominent with shortcuts
  ✅ Service Installer UI: Complete, no CLI needed!
  ✅ UI Polish: Fixed alien box, removed broken buttons
  ❌ Daily Summary: Architectural mismatch
  ❌ Apply/Reset: Change tracking broken
  Sprint Value: 4 features + UI improvements!
  Time: ~2 hours total
  Lesson: Test functionality, not just code existence!
```

### Architecture Principles (Hard-Won)
```yaml
1. Direct > Abstract
   - Unless you have 2+ implementations
   - Interfaces add complexity
   
2. Pipeline Isolation Required
   - Medical data must be separated
   - No shared mutable state
   - Each pipeline independent
   
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
7. Implementation > Documentation
   - Code is more complete than docs
   - Features exist but unknown
   - Always search before building
   
8. Config > UI
   - Config models are enterprise-ready
   - UI is often incomplete
   - Backend typically 95% done
   
9. Test Functionality > Code Existence
   - "Complete" doesn't mean "working"
   - Always test activation
   - Architecture matters!
```

### Technical Patterns for Hidden Feature Activation
```csharp
// Pattern 1: Commented service registration
// Search: "//.*services\.Add.*"
// Found: DailySummaryService, Statistics endpoint

// Pattern 2: Complete implementation, no UI
// Search ViewModels for unused commands:
public IAsyncRelayCommand InstallServiceCommand { get; }  // No button!

// Pattern 3: Config without UI
public class WebhookSettings  // Complete but no UI!
{
    public bool Enabled { get; set; }
    public string Url { get; set; }
    public string Method { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public int RetryCount { get; set; }
    // All properties ready!
}

// Pattern 4: Event handlers without subscribers
public event EventHandler<FileProcessingEventArgs>? ProcessingStarted;
public event EventHandler<FileProcessingEventArgs>? ProcessingCompleted;
// Events fire but nobody listens!

// Pattern 5: Hidden converters
public class FileSizeConverter : IValueConverter  // Never used!
{
    public object Convert(object value, Type targetType, 
        object parameter, CultureInfo culture)
    {
        // Complete implementation!
    }
}
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

# After Code Changes
0[TAB]                      # Build
9[TAB]                      # Test
git add -A                  # Stage
git commit -m "msg"         # Commit

# Before Major Change
git checkout -b feature/x   # New branch
7[TAB]                      # Clean build
00[TAB]                     # Full build

# Debug Workflow
4[TAB]                      # Console mode
Get-EventLog ...            # Check errors
2[TAB]                      # Open UI

# NEW - Treasure Hunt Workflow
Select-String "TODO" **\*.cs    # Find todos
Select-String "public.*Command"  # Find commands
# Compare with XAML bindings
# Activate hidden features!
```

### Common Issues → Quick Fixes
```yaml
"Service won't start":
  → Check port 5111
  → Check Event Log
  → Validate config JSON
  → Run console mode
  → Check if DailySummaryService issue

"Dashboard empty":
  → Check API response
  → Verify port 5111
  → Case-sensitive JSON?
  → Try minimal pattern

"Drag & Drop broken":
  → AllowDrop="True"?
  → Handlers connected?
  → Check Loaded event
  → ItemContainerGenerator?

"Build errors won't go away":
  → Clean obj/bin folders
  → Close all editors
  → Check file saved
  → dotnet build --no-incremental

"Feature needed quickly":
  → Search existing code
  → Check for enums
  → Look for TODO comments
  → Maybe already there!

NEW - "Hidden feature not working":
  → Is it commented out?
  → Missing UI binding?
  → Check ViewModel commands
  → Enable in config?
  → Architecture compatible?

NEW - "Run element Opacity error":
  → Opacity goes on TextBlock!
  → Run is inline text only
  → No layout props on Run
  → Oliver reminds you!
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

> "Run elements are INLINE only!"  
*No Opacity on Run! No Margin on Run!*  
*Opacity goes on TextBlock, not Run!*  
*(Oliver reminds me every time)*

> "Hidden treasures over new development"  
*11 transforms were already there!*  
*What else is implemented but hidden?*

> "Pipeline isolation for medical safety"  
*Each department completely separate*  
*No data mixing, ever*

> "The best debugger is the delete key"  
*Less code = fewer places for bugs*  
*Simplicity is the ultimate sophistication*

**NEW MANTRAS:**

> "The best code is already written"  
*Check for implementations before creating new*  
*Past-you was surprisingly thorough*

> "UI is the missing link"  
*Backend 95% complete, UI 60%*  
*Connect the dots for instant features*

> "Comments hide treasures"  
*Every // TODO might be // DONE*  
*Every commented line might work*

> "One line can activate a feature"  
*DailySummaryService one uncomment*  
*Hidden endpoint just needs docs*

> "Test functionality, not existence"  
*Code can be complete but broken*  
*Architecture matters for activation*

## 🚀 WISDOM NOTES

### What Works (Proven in Production)
- Tab-Complete system (0[TAB] muscle memory)
- Single config path (no confusion)
- Pipeline isolation (medical safety)
- Direct dependencies (clear, simple)
- Minimal pattern (when debugging fails)
- Hidden feature hunting (saves weeks)
- Error folders (simple > complex)
- Console mode debugging (see everything)
- Auto-backup on save (already active!)
- Health monitoring (runs in background)
- Service Installer UI (no more CLI!)
- Import/Export JSON (sharing configs)

### Hard-Won Lessons
1. **One wrong port = 3 sessions debugging**
   - Always check port 5111 first
   - It's always the simple things

2. **InitializeAsync patterns = timing hell**
   - Constructor initialization is simpler
   - Avoid complex async patterns in UI

3. **Too many interfaces = lost in own code**
   - Direct dependencies are clearer
   - Delete unused abstractions

4. **User frustration = signal to simplify**
   - "wahnsinnig" → time for minimal
   - Complex solutions often ARE the problem

5. **XAML has specific rules**
   - Run ≠ TextBlock for properties
   - Know your framework's limits

6. **Past code often better than memory**
   - Always check what exists
   - Hidden features everywhere

7. **Implementation phase was thorough**
   - Developers built complete features
   - UI phase was rushed
   - Documentation weakest link

8. **Activation is surprisingly easy**
   - Most features need <1 hour
   - Usually just UI binding
   - Sometimes just uncommenting

9. **Not all complete code works**
   - DailySummaryService looks perfect
   - But architecture incompatible
   - Always test before celebrating

10. **UI polish matters**
    - Alien boxes confuse users
    - Clean integration preferred
    - Simple > Feature-rich

### The Technical Evolution
```yaml
Started with: "Interfaces make code professional"
Learned that: "Direct dependencies make code clear"
Discovered: "Hidden implementations save time"
Now asking: "What else did past-me implement?"
Latest: "Is it architecturally compatible?"

Key Realization:
  Good architecture isn't about patterns
  It's about solving the actual problem
  With the least complexity possible
  While maintaining safety (medical!)
  And testing that it actually works!
  
NEW Realization:
  The system is more complete than it appears
  UI activation is all that's needed
  Hidden treasures are everywhere
  But some treasures are fools gold
  Test before celebrating!
```

### Hidden Feature Philosophy
```yaml
Old Thinking: "We need to build X"
New Thinking: "Is X already built?"
Newest: "Does X actually work?"

Old Process: Design → Build → Test → Deploy
New Process: Search → Find → Connect → Deploy
Current: Search → Find → Test → Fix/Connect → Deploy

Old Estimate: "2 weeks for webhooks"
New Reality: "2 hours - it's all there!"
Latest: "2 hours if it works, 4 if refactor needed"

Key Learning:
  Always search before building
  Check ViewModels for commands
  Read config classes completely
  Try uncommenting things
  TEST BEFORE CELEBRATING!
  Hidden treasures are everywhere!
  But not all treasure is gold!
```

---

*Technical wisdom from 74 sessions of building, breaking, and fixing*  
*Plus 1 epic treasure hunt that changed everything!*  
*Plus Sprint 18 where we learned not all treasures work!*  
*Remember: The best code is code you don't have to write!*  
*The second best is code you can delete!*  
*The third best is code that's already written and actually works!* 🗑️💎

*Complete Technical Reference - June 2025 - v0.7.27*

# WISDOM_TECHNICAL_APIS.md - External API Reference & Breaking Changes
**Purpose**: Critical API knowledge that changes between versions  
**Priority**: fo-dicom 5.2.2 breaking changes, ExifTool gotchas  
**Updated**: Session 114 (Konsolidiert)
**Critical**: Check this BEFORE upgrading ANY library!

## 🚨 fo-dicom 5.2.2 BREAKING CHANGES (MEMORIZE!)

### ❌ OLD (4.x) → ✅ NEW (5.x) Quick Reference

```csharp
// LOGGING - Completely changed!
❌ DicomNetworkLog.SetLogManager(new ConsoleLogManager());
✅ // Uses Microsoft.Extensions.Logging automatically from DI

// CLIENT OPTIONS - Don't exist!
❌ client.Options.RequestTimeout = TimeSpan.FromSeconds(30);
✅ using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
✅ await client.SendAsync(cts.Token);

// PIXEL DATA - Must use Dataset
❌ pixelData.SamplesPerPixel = 1;
✅ dataset.Add(DicomTag.SamplesPerPixel, (ushort)1);

// NAMESPACES - Need BOTH!
✅ using FellowOakDicom.Imaging;      // DicomPixelData
✅ using FellowOakDicom.IO.Buffer;    // MemoryByteBuffer
```

### 🎯 Response Pattern (CRITICAL!)
```csharp
// ALWAYS use TaskCompletionSource!
DicomCStoreResponse? response = null;
var responseReceived = new TaskCompletionSource<bool>();

var request = new DicomCStoreRequest(dicomFile)
{
    OnResponseReceived = (req, res) =>
    {
        response = res;
        responseReceived.TrySetResult(true);
    }
};

await client.AddRequestAsync(request);
await Task.WhenAll(client.SendAsync(cts.Token), responseReceived.Task);
// NOW response is guaranteed to be set!
```

### 📋 fo-dicom Migration Checklist
1. **Logging?** → Remove all setup code
2. **Timeouts?** → CancellationToken only
3. **Pixel Data?** → Through Dataset
4. **Options?** → Don't exist
5. **Response?** → TaskCompletionSource!

## 🔧 ExifTool Output Formats

### Command Line Flags Matter!
```bash
# -G1 adds group prefixes!
exiftool -j -a -G1 -s "image.jpg"

# Output difference:
Without -G1: "Barcode": "value"
With -G1: "RMETA:Barcode": "value"
```

### Key Mapping Strategy
```csharp
// ALWAYS check multiple variants:
var barcode = data.TryGetValue("RMETA:Barcode", out var val) || 
              data.TryGetValue("Barcode", out val) ? val : null;

// Common prefixes:
"File:ImageWidth"     // Not just "ImageWidth"
"ExifIFD:DateTimeOriginal"
"IFD0:Make"          // Camera manufacturer
"RMETA:Barcode"      // Our custom field
```

### UTF-8 Encoding Critical
```csharp
// Session 87 fix:
var startInfo = new ProcessStartInfo
{
    StandardOutputEncoding = Encoding.UTF8,  // NOT Windows-1252!
    StandardErrorEncoding = Encoding.UTF8
};
```

## 📊 DICOM Compliance Rules

### UID Format (Session 85-86!)
```csharp
// RULES:
// 1. Only digits (0-9) and dots (.)
// 2. Max 64 characters total
// 3. No negative numbers
// 4. No hex characters!

// WRONG:
guid.ToString("N").Substring(0, 8);  // Contains a-f!

// RIGHT:
var ticks = (DateTime.UtcNow.Ticks % 10000000000).ToString();
var pid = (Environment.ProcessId % 10000).ToString();
```

### Dataset Creation (Session 86 Critical!)
```csharp
// WRONG - Explicit length for JPEG!
var dataset = new DicomDataset();

// RIGHT - Transfer syntax at creation!
var dataset = new DicomDataset(DicomTransferSyntax.JPEGProcess1);
// fo-dicom now knows to use undefined length sequences
```

### AE Title Constraints
- Max 16 characters
- Only A-Z, 0-9, underscore
- No lowercase (some PACS reject)
- Pad with spaces if needed

### Private Tag Creation (Session 110/112)
```csharp
// WRONG - No VR specified
dataset.Add(new DicomTag(0x0009, 0x1001), barcodeData);

// RIGHT - Use private creator with VR
var privateCreator = dataset.AddPrivateCreator("CAMBRIDGE", 0x0009);
dataset.Add(DicomVR.LO, new DicomTag(0x0009, 0x1001, privateCreator), barcodeData);
```

## 🌐 Orthanc PACS Specifics

### Docker Setup
```powershell
# WORKING PowerShell command:
docker run -p 8042:8042 -p 4242:4242 --name orthanc `
  -e ORTHANC__DICOM_SERVER_ENABLED=true `
  -e ORTHANC__DICOM_PORT=4242 `
  -e ORTHANC__DICOM_CHECK_CALLED_AE_TITLE=false `
  jodogne/orthanc
```

### Ports
- **8042**: Web interface
- **4242**: DICOM (NOT 104!)
- **104**: Standard DICOM port (not Orthanc default)

### Testing
```powershell
# Check connection:
Test-NetConnection -ComputerName localhost -Port 4242

# View in browser:
http://localhost:8042
```

## 🔌 Windows Service API Gotchas

### Working Directory = System32!
```csharp
// WRONG:
var path = "Output\\file.dcm";

// RIGHT:
var path = Path.GetFullPath("Output\\file.dcm");
// Or better:
var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output", "file.dcm");
```

### ServiceController Quirks
```csharp
// Status updates are delayed:
service.Stop();
service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
// May need extra delay for full shutdown
await Task.Delay(1000);
```

## 📝 JSON Parsing Patterns

### Handle All ValueKinds
```csharp
// ExifTool returns mixed types:
switch (element.ValueKind)
{
    case JsonValueKind.String:
        return element.GetString();
    case JsonValueKind.Number:
        return element.GetInt32().ToString();
    case JsonValueKind.True:
    case JsonValueKind.False:
        return element.GetBoolean().ToString();
    default:
        return element.GetRawText();
}
```

## 📚 Internal API Changes (Sessions 96-110)

### LogContext API (NEW!)
```csharp
// Session 96-97: Hierarchical logging
public class LogContext
{
    public string CorrelationId { get; }
    public string PipelineName { get; }
    public ProcessingStage Stage { get; set; }
    public DateTime StartTime { get; }
    public LogVerbosity Verbosity { get; }
    
    // Auto-logs stage completion with timing
    public IDisposable BeginStage(ProcessingStage stage, ILogger logger);
    
    // Check if should log at this verbosity
    public bool ShouldLog(LogVerbosity requiredLevel);
}

// Usage pattern:
using (logContext.BeginStage(ProcessingStage.ExifExtraction, logger))
{
    // Stage automatically logged with timing
}
```

### Enum Namespace Migration (Session 97)
```csharp
// OLD location:
CamBridge.Core.Logging.LogVerbosity
CamBridge.Core.Logging.ProcessingStage

// NEW location:
CamBridge.Core.Enums.LogVerbosity
CamBridge.Core.Enums.ProcessingStage

// Impact: All files using these need 'using CamBridge.Core.Enums;'
```

### Configuration API Pattern
```csharp
// ServiceSettings.LogVerbosity is ENUM not string!
public LogVerbosity LogVerbosity { get; set; } = LogVerbosity.Detailed;

// ComboBox binding needs conversion:
SelectedLogVerbosity = settings.Service.LogVerbosity.ToString();
Enum.TryParse<LogVerbosity>(SelectedLogVerbosity, out var verbosity);
```

## 🏗️ Domain Object Contracts

### CRITICAL: Constructor Patterns
```csharp
// PatientInfo - POSITIONAL ONLY!
new PatientInfo(id, name, birthDate, gender)  // ✅
new PatientInfo { Name = "Test" }             // ❌ COMPILER ERROR!

// ImageMetadata - 10 PARAMETERS!
new ImageMetadata(
    sourceFilePath,
    captureDateTime,    // NOT captureDate!
    patient,
    study,
    technicalData,
    userComment,
    barcodeData,
    instanceNumber,
    instanceUid,
    exifData)

// ImageTechnicalData - OBJECT INITIALIZER!
new ImageTechnicalData { ImageWidth = 1024 }  // ✅
```

### Property Name Contracts
```csharp
// ImageTechnicalData (Session 87 - 45min lesson!)
ImageWidth    ✅  // NOT Width
ImageHeight   ✅  // NOT Height  
Manufacturer  ✅  // NOT CameraManufacturer

// ImageMetadata
CaptureDateTime  ✅  // NOT CaptureDate
InstanceUid      ✅  // NOT nullable (auto-generated)

// ConversionResult
FileSizeBytes  ✅  // NOT FileSize
// Factory methods only! No public constructor
```

## 🎯 API Version Checks

### Quick Test Programs
```csharp
// TEMPLATE for testing new library versions:
// 1. Create minimal console app
// 2. Reference ONLY the library
// 3. Test basic operations
// 4. Document breaking changes!

// fo-dicom test:
var echo = new DicomCEchoRequest();
// If this compiles, basic API unchanged

// If errors, check:
// - Namespace changes
// - Method renames
// - Property removals
// - New required parameters
```

## ⚡ Performance Insights

### DICOM Operations
- C-ECHO: ~50ms locally
- C-STORE: 100-200ms small files
- Large files: Show progress >10MB

### ExifTool
- Cold start: ~500ms (process spawn)
- Warm: ~100ms per image
- Batch mode faster for multiple files

### File Operations
- NTFS instant for <1000 files/folder
- Performance degrades >10000 files
- Use subfolders for organization

## 🚫 Common API Mistakes

1. **Assuming old fo-dicom API** (check version!)
2. **Forgetting -G1 for ExifTool** (prefixes!)
3. **Wrong Orthanc port** (4242 not 104!)
4. **Relative paths in services** (System32!)
5. **Not handling all JSON types** (mixed!)
6. **Wrong enum namespace** (Core.Enums now!)
7. **Private tags without VR** (validation fails!)

## 🆕 No Breaking Changes Found (Sessions 100-114)

Good news! No new external API breaking changes discovered in recent sessions:
- ✅ fo-dicom 5.2.2 stable
- ✅ ExifTool still working with -G1
- ✅ Windows Service APIs unchanged
- ✅ WPF/XAML stable (except MaterialDesign removal)
- ✅ .NET 6.0 APIs stable

Focus has been on internal architecture and bug fixes!

---

**Golden Rule**: When updating ANY external library, create a test program FIRST! Session 91 proved this saves hours.

**Latest Status**: All external APIs stable. Internal APIs evolving (LogContext, Enums).

*"The best breaking change is the one you discover in a test program, not production!"* 🧪

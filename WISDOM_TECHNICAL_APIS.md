# WISDOM_TECHNICAL_APIS.md - External API Reference & Gotchas
**Purpose**: Critical API knowledge that changes between versions  
**Priority**: fo-dicom 5.2.2 breaking changes (Session 91 trauma!)  
**Updated**: Session 93 (Destillation)

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

## 📝 Domain Object Contracts (from sources-contract.md)

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

---

**Golden Rule**: When updating ANY external library, create a test program FIRST! Session 91 proved this saves hours.

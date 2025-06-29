# WISDOM_TECHNICAL_FIXES.md - Categorized Bug Fixes & Solutions
**Purpose**: Quick reference for recurring bugs and their proven fixes  
**Format**: Problem → Symptom → Solution → Prevention  
**Updated**: Session 93 (Destillation from 40+ fixes)

## 🔌 PORT & NETWORK ISSUES

### The 5111 Port Consistency Fix
**Sessions**: 40, 58-61 (3 debugging sessions!)  
**Symptom**: Dashboard empty, API not responding  
**Root Cause**: Service on 5050, Config expects 5111

```powershell
# Quick check:
netstat -ano | findstr :5050
netstat -ano | findstr :5111
```

**Fix**: Global replace ALL occurrences  
**Prevention**: Single constant `const int API_PORT = 5111;`

### Orthanc Port Confusion
**Session**: 91  
**Symptom**: DICOM connection fails  
**Root Cause**: Orthanc uses 4242, not standard 104

```yaml
Standard PACS: Port 104
Orthanc Docker: Port 4242
Orthanc Web UI: Port 8042
```

## 📁 PATH PROBLEMS (Windows Service Special!)

### Relative Path in Service = FAIL
**Sessions**: 23, 41, 85  
**Symptom**: "File not found" but file exists  
**Root Cause**: Service runs from System32!

```csharp
// WRONG - Works in console, fails in service:
var path = "Output\\file.dcm";

// RIGHT - Always absolute:
var path = Path.GetFullPath("Output\\file.dcm");
// BETTER:
var basePath = AppDomain.CurrentDomain.BaseDirectory;
var path = Path.Combine(basePath, "Output", "file.dcm");
```

### Unicode Filename Crashes
**Session**: 73  
**Symptom**: "Radiologie-Süd" crashes file creation  
**Fix**: Sanitize for filesystem

```csharp
private string SanitizeForFileName(string name)
{
    var invalid = Path.GetInvalidFileNameChars()
        .Concat(new[] { ' ', '.', ',' }).ToArray();
    return string.Join("_", name.Split(invalid));
}
```

## 🏷️ PROPERTY NAME DISASTERS

### The ImageWidth vs Width Saga
**Session**: 87 (45 minutes wasted!)  
**Symptom**: CS0117 "no definition for 'Width'"

```csharp
// WRONG (from memory):
technicalData.Width = width;
technicalData.CameraManufacturer = make;

// RIGHT (from source):
technicalData.ImageWidth = width;
technicalData.Manufacturer = make;
```

**Prevention**: ALWAYS check actual property names!

### Missing Property Initializers
**Sessions**: 67, 72, 90  
**Symptom**: Bindings don't work, save button disabled

```csharp
// Domain objects with constructor-only init:
// WRONG:
var patient = new PatientInfo { Name = "Test" }; // COMPILER ERROR!

// RIGHT:
var patient = new PatientInfo(
    new PatientId("123"), 
    "Test", 
    null, 
    Gender.Other);
```

## 🎭 XAML/WPF GOTCHAS

### Run Element Opacity Fail
**Multiple Sessions**  
**Symptom**: Opacity not working on Run elements

```xml
<!-- WRONG - Run doesn't support Opacity: -->
<Run Text="→" Opacity="0.7"/>

<!-- RIGHT - Opacity on TextBlock: -->
<TextBlock Opacity="0.7">
    <Run Text="→"/>
</TextBlock>
```

### Converter Type Mismatch
**Session**: 70  
**Symptom**: Wrong visibility behavior

```xml
<!-- WRONG - Bool converter for object: -->
<TextBlock Visibility="{Binding SelectedPipeline, 
    Converter={StaticResource InverseBoolToVisibility}}"/>

<!-- RIGHT - Null converter for object: -->
<TextBlock Visibility="{Binding SelectedPipeline, 
    Converter={StaticResource NullToVisibility},
    ConverterParameter=Inverse}"/>
```

## 🔧 CONFIGURATION CHAOS

### Missing Config Wrapper
**Multiple Sessions**  
**Symptom**: "CamBridge section not found"

```json
// WRONG:
{
  "Version": "2.0",
  "Service": { }
}

// RIGHT - MUST have wrapper:
{
  "CamBridge": {
    "Version": "2.0", 
    "Service": { }
  }
}
```

### Enum Value Changes
**Session**: 66  
**Symptom**: JSON deserialization fails

```yaml
Old Values: PatientName, Date, Patient
New Values: ByPatient, ByDate, ByPatientAndDate
Fix: Migration logic in ConfigurationService
```

## 🏥 DICOM COMPLIANCE ISSUES

### UID Contains Hex = FAIL
**Session**: 85  
**Symptom**: "does not validate VR UI"

```csharp
// WRONG - GUID has letters:
var uid = Guid.NewGuid().ToString("N").Substring(0, 8); // "bb7e1567"

// RIGHT - Numbers only:
var uid = (DateTime.UtcNow.Ticks % 10000000000).ToString();
```

### Transfer Syntax Wrong Location
**Session**: 85  
**Symptom**: Viewers don't recognize encoding

```csharp
// WRONG - On dataset:
dataset.Add(DicomTag.TransferSyntaxUID, syntax);

// RIGHT - On FileMetaInfo:
dicomFile.FileMetaInfo.TransferSyntax = syntax;
```

### Dataset Creation Determines Encoding!
**Session**: 86 (Critical finding!)  
**Symptom**: "explicit length in compressed syntax"

```csharp
// WRONG:
var dataset = new DicomDataset();
// Later: dataset.Add(jpegData); // FAIL!

// RIGHT:
var dataset = new DicomDataset(DicomTransferSyntax.JPEGProcess1);
// fo-dicom now uses undefined length!
```

## 🔄 DEPENDENCY INJECTION TRAPS

### Service Not Registered - But It Is!
**Session**: 92  
**Symptom**: Error AFTER service starts (20+ seconds)  
**Root Cause**: STUB code doesn't use the service!

```csharp
// Check: Is the service ACTUALLY CALLED?
// STUBs may skip the real implementation!
```

### DLL Not Deployed
**Session**: 92  
**Symptom**: Type not found at runtime  
**Fix**: Check deployment script includes new DLLs

## 🎯 QUICK DEBUG CHECKLIST

### Dashboard Empty?
1. Check port 5111
2. Check service running
3. Check Event Log
4. Try API directly

### File Not Found?
1. Absolute path?
2. Service working directory?
3. Permissions?
4. Unicode in path?

### Binding Not Working?
1. Object null?
2. Property name exact?
3. Converter type match?
4. PropertyChanged fired?

### DICOM Invalid?
1. UID format (numbers only!)
2. Transfer syntax location
3. Dataset creation method
4. Use dcmdump tool

### Build Errors?
1. Check Version.props encoding
2. Property names exact?
3. Constructor parameters?
4. Namespace changes?

## 💡 PREVENTION PATTERNS

1. **Constants for Magic Values**
   - Ports, paths, limits
   - Single source of truth

2. **Defensive Path Handling**
   - Always absolute paths
   - Sanitize user input
   - Handle Unicode

3. **Property Verification**
   - Check source, not memory
   - Use exact names
   - No creativity!

4. **Test in Service Context**
   - Not just console app
   - Check working directory
   - Verify permissions

---

**Remember**: These bugs cost us HOURS. Learn the patterns, check the checklist, save your sanity! 🧠

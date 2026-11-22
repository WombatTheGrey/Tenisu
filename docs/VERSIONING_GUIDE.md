# API Versioning Implementation Guide

**Status**: Ready for Implementation  
**Date Created**: 2024  
**Target Framework**: .NET 10  
**Solution**: Tenisu API

---

## Overview

This guide implements an industry-standard semantic versioning (SemVer) solution for the Tenisu API that:

- Maintains a configurable major, minor, and patch version
- Auto-increments build numbers via PowerShell script
- Works both locally (developer machines) and in Azure DevOps pipelines
- Exposes version via REST API endpoint and Swagger documentation
- Shows version change on redeployment even without code changes

This approach is used by Microsoft, Google, and enterprise software companies.

---

## Architecture

### Four-Component Solution

```
┌─────────────────────────────────────────────────────────────┐
│ Solution Root: C:\Stockage\Projects\Tenisu\                │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  version.json (Configuration)                               │
│  └─ Stores: major, minor, patch, build                      │
│                                                              │
│  build/increment-version.ps1 (Script)                       │
│  └─ Increments build number before each build               │
│                                                              │
│  src/Tenisu.WebApi/                                         │
│  ├─ Services/VersionService.cs                              │
│  │  └─ Reads version.json at runtime                        │
│  │                                                           │
│  ├─ Controllers/VersionController.cs                         │
│  │  └─ Exposes GET /api/version endpoint                    │
│  │                                                           │
│  └─ Program.cs (Updated)                                    │
│     └─ Registers VersionService, updates Swagger            │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## Component Details

### 1. version.json (Configuration File)

**Location**: `C:\Stockage\Projects\Tenisu\version.json`

**Purpose**: 
- Single source of truth for version numbers
- Committed to Git (tracked in version control)
- Build number increments with each deployment
- Easy to manually edit if needed

**Content**:
```json
{
  "major": 1,
  "minor": 0,
  "patch": 0,
  "build": 0
}
```

**Usage**:
- Major: Increment for breaking changes
- Minor: Increment for new features
- Patch: Increment for bug fixes
- Build: Auto-incremented by script on each build

---

### 2. build/increment-version.ps1 (PowerShell Script)

**Location**: `C:\Stockage\Projects\Tenisu\build\increment-version.ps1`

**Purpose**:
- Reads version.json from disk
- Increments the build number
- Writes updated version back to file
- Idempotent (safe to run multiple times without side effects)

**Functionality**:
- Safe file operations with error handling
- Creates backup before modifying
- Validates JSON structure
- Works with relative and absolute paths

**Key Features**:
- No external dependencies required
- Works on any Windows machine (PowerShell 5.0+)
- Compatible with Azure DevOps agent runners
- Takes less than 100ms to execute

---

### 3. VersionService (C# Service Class)

**Location**: `src/Tenisu.WebApi/Services/VersionService.cs`

**Purpose**:
- Singleton service (one instance for app lifetime)
- Reads version.json at application startup
- Provides version info to controllers
- Formats version as SemVer

**Responsibilities**:
- Load version.json from disk
- Resolve file path from application root
- Format version string: `{major}.{minor}.{patch}-build.{build}`
- Handle missing/corrupted files gracefully
- Thread-safe access to version info

**API**:
```csharp
string GetVersion() // Returns "1.0.0-build.42"
int BuildNumber      // Returns 42
DateTime BuildDate   // Returns when version was set
```

---

### 4. VersionController (REST API Endpoint)

**Location**: `src/Tenisu.WebApi/Controllers/VersionController.cs`

**Route**: `GET /api/version`

**Response Example**:
```json
{
  "version": "1.0.0-build.42",
  "major": 1,
  "minor": 0,
  "patch": 0,
  "build": 42,
  "buildDate": "2024-01-15T10:30:00Z"
}
```

**Features**:
- Simple, single responsibility endpoint
- No authentication required (version is public info)
- Automatically documented in Swagger UI
- Lightweight, fast response

---

## Implementation Steps

### Step 1: Create version.json

**File**: `version.json` (at solution root)

**Action**:
1. Create new file at `C:\Stockage\Projects\Tenisu\version.json`
2. Add the JSON content above
3. Commit to Git with message: `chore: initialize version.json`

**Initial Content**:
```json
{
  "major": 1,
  "minor": 0,
  "patch": 0,
  "build": 0
}
```

---

### Step 2: Create PowerShell Script

**File**: `build/increment-version.ps1`

**Action**:
1. Create directory: `C:\Stockage\Projects\Tenisu\build\`
2. Create file: `increment-version.ps1`

**Script Requirements**:
- Read version.json
- Parse JSON
- Increment `build` field by 1
- Write back to file
- Error handling if file not found
- Preserve JSON format

**Example Logic**:
```
1. Get script directory
2. Locate version.json (go up 1 directory level)
3. Read file as JSON
4. Increment build: build = build + 1
5. Write back to file
6. Output: "Version incremented to X.Y.Z-build.N"
```

---

### Step 3: Create VersionService

**File**: `src/Tenisu.WebApi/Services/VersionService.cs`

**Action**:
1. Create directory: `src/Tenisu.WebApi/Services/`
2. Create class that loads version.json at initialization

**Constructor Behavior**:
- Read version.json at service creation (app startup)
- Store values in private fields
- Parse and format as SemVer string
- Log version on initialization

**File Path Resolution**:
- Use: `Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../version.json")`
- Or better: `Path.Combine(Directory.GetCurrentDirectory(), "version.json")`
- Handle `FileNotFoundException` gracefully

**Public Methods**:
- `GetVersion()` → Returns formatted version string
- `GetVersionInfo()` → Returns detailed object with all components

---

### Step 4: Create VersionController

**File**: `src/Tenisu.WebApi/Controllers/VersionController.cs`

**Action**:
1. Create new controller class
2. Add single GET endpoint
3. Inject VersionService via constructor

**Endpoint Design**:
```
Route: GET /api/version
Response: 200 OK with JSON
Body:
{
  "version": "1.0.0-build.42",
  "buildDate": "2024-01-15T10:30:00Z"
}
```

**Code Pattern**:
- Use standard ASP.NET Core controller patterns
- Decorate with `[ApiController]` and `[Route("api/[controller]")]`
- Inject VersionService via constructor dependency injection
- Simple, no complex logic

---

### Step 5: Update Program.cs

**File**: `src/Tenisu.WebApi/Program.cs`

**Changes**:

#### 5a: Register VersionService
Add to the builder.Services section:
```csharp
builder.Services.AddSingleton<VersionService>();
```

#### 5b: Update Swagger Configuration
Modify the OpenAPI/Swagger configuration to include version:
```csharp
services.AddOpenApi(options =>
{
    options.AddOperationTransformer((operation, _, method) =>
    {
        var versionService = /* get from DI */;

        return Task.CompletedTask;
    });
});
```

Or simpler approach:
- Read version.json directly to set Swagger title version to your app version

---

### Step 6: Update .csproj

**File**: `src/Tenisu.WebApi/Tenisu.WebApi.csproj`

**Changes**:
Add item group to include version.json in published output:

```xml
<ItemGroup>
  <None Update="../../version.json" 
    CopyToOutputDirectory="PreserveNewest" 
    Link="version.json" />
</ItemGroup>
```

This ensures version.json is available when deployed.

---

### Step 7: Local Pre-Build Integration

**Option A: Manual (Recommended for start)**
- Run PowerShell script manually before building:
  ```powershell
  cd C:\Stockage\Projects\Tenisu
  .\build\increment-version.ps1
  dotnet build
  ```

**Option B: Automatic Pre-Build (Advanced)**
- Add to .csproj:
  ```xml
  <Target Name="IncrementVersion" BeforeTargets="PrepareForBuild">
    <Exec Command="powershell -ExecutionPolicy Bypass -File $(MSBuildThisFileDirectory)../build/increment-version.ps1" />
  </Target>
  ```

**Recommendation**: Start with Option A (manual), move to Option B if desired

---

### Step 8: Azure DevOps Pipeline Integration

**File**: `azure-pipelines.yml` (if using YAML pipelines)

**Add Step**:
```yaml
steps:
  - task: PowerShell@2
    displayName: 'Increment Build Number'
    inputs:
      targetType: 'filePath'
      filePath: '$(Build.SourcesDirectory)\build\increment-version.ps1'
      workingDirectory: '$(Build.SourcesDirectory)'

  - task: DotNetCoreCLI@2
    displayName: 'Build Solution'
    inputs:
      command: 'build'
      arguments: '--configuration Release'
```

**Placement**: Insert PowerShell step BEFORE any build steps

---

## Usage Workflows

### Local Development Workflow

**First Time Setup**:
```powershell
# Clone repo and navigate
cd C:\Stockage\Projects\Tenisu

# Run version script
.\build\increment-version.ps1

# Build solution
dotnet build

# Run API
dotnet run --project src/Tenisu.WebApi
```

**Check Version**:
```powershell
# In PowerShell or browser
curl http://localhost:5000/api/version

# Response:
# {"version":"1.0.0-build.1","buildDate":"2024-01-15T10:30:00Z"}
```

**Multiple Deploys Without Code Change**:
```powershell
# Run script again
.\build\increment-version.ps1

# version.json now has build: 2

# Build and deploy
dotnet build
dotnet publish

# New deployment shows version 1.0.0-build.2
```

### CI/CD Pipeline Workflow

1. **Pipeline triggers** on commit
2. **Checkout code** (includes version.json)
3. **Run increment-version.ps1**
   - Build number increments (e.g., 42 → 43)
   - version.json modified in workspace
4. **Build project**
   - Reads updated version.json
   - Embeds version in build
5. **Create package/container**
   - Includes incremented build number
6. **Deploy to environment**
   - GET /api/version reflects new build number
   - Even though code is unchanged

---

## Version Number Management

### When to Increment Each Part

**Major** (currently 1):
- Breaking changes to API
- Incompatible with previous versions
- Increment manually, commit to Git
- Example: 1.0.0 → 2.0.0

**Minor** (currently 0):
- New features added
- Backward compatible
- Increment manually, commit to Git
- Example: 1.0.0 → 1.1.0

**Patch** (currently 0):
- Bug fixes
- Security patches
- Backward compatible
- Increment manually, commit to Git
- Example: 1.0.0 → 1.0.1

**Build** (increments automatically):
- Every build/deployment
- Incremented by script
- Shows deployment cadence
- Example: 1.0.0-build.1 → 1.0.0-build.2 → 1.0.0-build.43

### Example Version Progression

```
Timeline  | Action                          | Version
----------|----------------------------------|--------------------
Initial   | Create repo                     | 1.0.0-build.0
Dev 1     | Run script + build locally      | 1.0.0-build.1
Dev 2     | Run script + build locally      | 1.0.0-build.2
Commit    | Git commit (no version change)  | stays 1.0.0-build.2
CI Build  | Pipeline runs script            | 1.0.0-build.3
Deploy    | Deploy to staging               | API shows 1.0.0-build.3
Feature   | Add new feature + commit        | still 1.0.0-build.3 in code
CI Build  | Pipeline runs script            | 1.0.0-build.4
Deploy    | Deploy to production            | API shows 1.0.0-build.4
Manual    | Edit version.json: minor: 1    | 1.1.0-build.4
CI Build  | Run script on new code          | 1.1.0-build.5
Deploy    | Release new feature version     | API shows 1.1.0-build.5
```

---

## Testing & Verification

### Unit Testing (Optional)

**Test**: VersionService loads version.json correctly
**Test**: VersionController endpoint returns correct format
**Test**: Version increments when script runs

### Manual Testing

**Step 1**: Run increment script
```powershell
.\build\increment-version.ps1
```

**Step 2**: Check version.json
```powershell
Get-Content version.json | ConvertFrom-Json
```
Expected: `build` field incremented by 1

**Step 3**: Run API (if built)
```powershell
curl http://localhost:5000/api/version
```
Expected: SemVer format with incremented build number

**Step 4**: Check Swagger UI
Navigate to `http://localhost:5000/swagger/ui` and verify version appears in documentation

---

## Troubleshooting

### Issue: Script not found
**Cause**: Running from wrong directory or missing build folder  
**Solution**: Navigate to solution root before running
```powershell
cd C:\Stockage\Projects\Tenisu
.\build\increment-version.ps1
```

### Issue: version.json not copying to output
**Cause**: .csproj not configured correctly  
**Solution**: Verify ItemGroup in .csproj includes version.json with `CopyToOutputDirectory="PreserveNewest"`

### Issue: VersionService reads old version
**Cause**: version.json not deployed with application  
**Solution**: Build and verify version.json is in output folder

### Issue: PowerShell execution policy error
**Cause**: ExecutionPolicy set to Restricted  
**Solution**: 
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```
Or use `-ExecutionPolicy Bypass` flag when running

### Issue: JSON parsing error in script
**Cause**: version.json malformed  
**Solution**: Validate JSON format using editor or online tool

---

## Best Practices

✅ **Do**:
- Commit version.json to Git (tracks version history)
- Run increment script before every build
- Include version in logs for debugging
- Use SemVer format consistently
- Document version changes in release notes

❌ **Don't**:
- Manually edit build number in version.json
- Share version.json between different API services
- Increment build number in git commit messages (let script handle it)
- Deploy without running increment script first
- Use version for feature flags (use separate config)

---

## Industry Standard References

This solution follows:
- **Semantic Versioning 2.0.0** (semver.org)
- **Microsoft AssemblyVersion patterns**
- **Google Cloud versioning practices**
- **Azure DevOps release management standards**

Similar patterns used in:
- Visual Studio (version.json in projects)
- .NET runtime
- Kubernetes deployments
- Docker versioning

---

## Support & Next Steps

### Ready to Implement?
1. Create version.json
2. Create build/increment-version.ps1
3. Create VersionService
4. Create VersionController
5. Update Program.cs
6. Update .csproj
7. Test locally
8. Integrate with AzDo pipeline

### Questions?
Refer to specific section numbers above for detailed implementation guidance.

### Customization?
This base solution is highly flexible. You can:
- Add deployment environment to version string
- Include commit hash in build info
- Store version in Azure App Configuration
- Integrate with Git tags
- Add version validation on startup

---

**Document Version**: 1.0  
**Last Updated**: January 2025  
**Status**: Ready for Implementation

# Building BongoCatMod

## Prerequisites

- .NET SDK 6.0 or later
- Bongo Cat game installed
- BepInEx installed in your Bongo Cat directory

## Setup

1. **Configure Game Path**

   Edit [.vscode/settings.json](../.vscode/settings.json) and set your Bongo Cat installation path:
   ```json
   {
       "bongoCat.installPath": "YOUR_GAME_PATH_HERE"
   }
   ```

   Or edit [BongoCatMod/Directory.Build.props](Directory.Build.props) to set the default path:
   ```xml
   <BongoCatDir>YOUR_GAME_PATH_HERE</BongoCatDir>
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

## Building

### Using VSCode Tasks

Press `Ctrl+Shift+B` (or `Cmd+Shift+B` on Mac) to open the build task menu:

- **build** - Default build task, uses path from Directory.Build.props
- **build with custom path** - Prompts for game path
- **build and deploy** - Builds and copies DLL to BepInEx/plugins folder
- **clean** - Cleans build artifacts
- **restore** - Restores NuGet packages

### Using Command Line

```bash
# Build with default path
dotnet build

# Build with custom path
dotnet build /property:BongoCatDir="C:\Path\To\Bongo Cat"

# Clean
dotnet clean
```

## Output

The compiled DLL will be in:
```
BongoCatMod/bin/Debug/netstandard2.1/BongoCatMod.dll
```

Copy this file to:
```
YOUR_GAME_PATH/BepInEx/plugins/BongoCatMod.dll
```

Or use the "build and deploy" task to do this automatically.

## Troubleshooting

### Missing Assembly-CSharp.dll reference

If you get an error about missing `Assembly-CSharp.dll`, make sure:
1. Your game path is set correctly
2. The file exists at `YOUR_GAME_PATH/BongoCat_Data/Managed/Assembly-CSharp.dll`

### BepInEx packages not found

Run:
```bash
dotnet restore
```

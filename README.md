# Bongo Cat Auto Chest Opener Mod

A mod for Bongo Cat that adds automatic chest opening functionality and click count multiplier.

## Features

- Automatic chest opening
- Configurable click count multiplier (default: 1x, no multiplier)
- Easy to enable/disable via config file
- In-game configuration UI via BepInExConfigManager

## Installation

### Quick Install (Recommended)
1. Download the latest release package from [Releases](https://github.com/Seraphli/bongo-cat/releases)
2. Extract the contents directly into your Bongo Cat game directory
   - The package includes:
     - BepInEx framework
     - BepInExConfigManager (for in-game config UI)
     - BongoCatMod plugin with default configuration
3. Launch the game and enjoy!

### Configuration
The mod includes a pre-configured config file at `BepInEx\config\com.seraphli.bongocatmod.cfg`:
```ini
[General]
## Enable automatic chest buying
# Setting type: Boolean
# Default value: true
AutoBuyEnabled = true

## Multiplier for click counts (default: 1)
# Setting type: Int32
# Default value: 1
ClickMultiplier = 1
```

You can either:
- Edit this config file directly with a text editor, or
- Click on the BongoCat game window to focus it, then press **F5** to open the configuration manager UI if you need to change settings

### Uninstallation
Delete the following folders/files from your Bongo Cat directory:
- `BepInEx/` folder
- `doorstop_config.ini`
- `winhttp.dll`

---

## Development

### Building from Source

If the mod hasn't been updated for a new game version, you can create it yourself by following these steps:

### Requirements
- [dnSpy](https://github.com/dnSpy/dnSpy) - A .NET debugger and assembly editor

### Steps

1. **Open DLL in dnSpy**
   - Drag `Assembly-CSharp.dll` from `BongoCat_Data\Managed\` into dnSpy
   - **Important:** Drag from the original location, NOT a copy

2. **Navigate to** `BongoCat` > `Shop` > `TimeUpdate()`

3. **Edit the method**
   - Find this code:
   ```csharp
   if (this._showChestPopup.Value && this._shopItem.CanBuy())
   {
       this._shopVisuals.SetActive(true);
   }
   ```

   - Add this right after:
   ```csharp
   if(this._isEmoteShop)
   {
       yield return new WaitForSecondsRealtime(3f);
   }
   this._shopItem.Buy();
   ```

4. **Save**
   - Click "Compile"
   - `File` > `Save Module`
   - Replace the original DLL

### Notes
- The 3-second wait time for emote shops is intentional to prevent bugs from simultaneous calls
- Always backup your original DLL before making modifications

## Additional Modifications

### Modify Click Count Multiplier

If you want to increase the click count per input:

1. **Navigate to** `BongoCat` > `GlobalKeyHook` > `Process()`

2. **Find this line:**
   ```csharp
   this._keysDown += GlobalKeyHook.IsDown.Count((bool x) => x);
   ```

3. **Modify to:**
   ```csharp
   this._keysDown += GlobalKeyHook.IsDown.Count((bool x) => x) * 1000;
   ```

This will make each click count as 1000 clicks. You can adjust the multiplier to any value you prefer.

## How to Generate MD5 Hash

### Windows
Using PowerShell:
```powershell
Get-FileHash -Algorithm MD5 BongoCat_Data\Managed\Assembly-CSharp.dll
```

Or using certutil:
```cmd
certutil -hashfile BongoCat_Data\Managed\Assembly-CSharp.dll MD5
```

### Linux
```bash
md5sum BongoCat_Data/Managed/Assembly-CSharp.dll
```

### macOS
```bash
md5 BongoCat_Data/Managed/Assembly-CSharp.dll
```

Or using:
```bash
md5sum BongoCat_Data/Managed/Assembly-CSharp.dll
```

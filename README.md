# Bongo Cat Auto Chest Opener Mod

A mod for Bongo Cat that adds automatic chest opening functionality.

## Features

- Automatic chest opening

## Compatibility

This mod is compatible with the specific version of Bongo Cat that has the following MD5 hash:

**Target Version Hash:** `2357ABE9ADBE56BFCC53926BB78B96A7`

Since Bongo Cat doesn't have version numbers, we use MD5 hash to identify the correct game version. This hash is for the `Assembly-CSharp.dll` file located in `BongoCat_Data\Managed\` directory.

### How to Check Your Game Version

1. Navigate to your Bongo Cat installation directory
2. Go to `BongoCat_Data\Managed\`
3. Generate the MD5 hash of `Assembly-CSharp.dll` using the commands below
4. Compare the hash with the target version hash above

## Installation

1. **Verify your game version** by checking the MD5 hash of `Assembly-CSharp.dll` (see instructions above)
2. If the hash matches, **backup your original `Assembly-CSharp.dll`** (optional but recommended)
3. **Replace** the original `Assembly-CSharp.dll` in `BongoCat_Data\Managed\` with the modded version from this repository
4. Launch the game

## Uninstallation

To restore the original game files:
1. Open Steam
2. Right-click on Bongo Cat in your library
3. Select **Properties** > **Installed Files** > **Verify integrity of game files**
4. Steam will automatically restore the original files

## How to Create This Mod Yourself

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

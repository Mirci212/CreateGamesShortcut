```markdown
# 🎮 Game Manager - Complete Setup Guide

## 📁 Required Game Folder Structure
```plaintext
D:\Games\
├── Epic Games\
│   └── GameName\Game.exe
├── PC Games\
│   └── GameName\Game.exe
└── SteamLibrary\
    └── steamapps\
        └── common\
            └── GameName\Game.exe
```

## ✅ Supported Game Path Examples
- `D:\Games\Epic Games\BloonsTD6\BloonsTD6.exe`
- `D:\Games\PC Games\Horizon Zero Dawn\HorizonZeroDawn.exe`
- `D:\SteamLibrary\steamapps\common\Satisfactory\FactoryGame.exe`

## 🚫 Unsupported Paths
- `C:\Program Files\...`
- Random folders outside defined structure

## 🔍 Core Features

### 1️⃣ Central Game Library
```csharp
// Scans these locations automatically:
string sourcePath = "D:\\Games";
string sourcePath2 = "D:\\SteamLibrary\\steamapps\\common";
```

### 2️⃣ Shortcut Management
```plaintext
Creates:
- Desktop\Games\GameName.lnk
- Start Menu\Programs\Games\GameName.lnk
```

### 3️⃣ Smart Uninstall System
```powershell
# For games with uninstaller:
Uses existing uninstall.exe/unins000.exe

# For others:
Generates cleanup script in %AppData%\UninstallGameCmd\
```

## ⚙️ Technical Implementation

### Game Detection Logic
```csharp
public string FindExeFile() 
{
    string[] exeFiles = Directory.GetFiles(gamePath, "*.exe", SearchOption.AllDirectories);
    // Filters out system files and prioritizes main executables
}
```

### Size Calculation
```csharp
private long GetFolderSize(DirectoryInfo d)
{
    // Recursively calculates folder size
    foreach (FileInfo fi in d.GetFiles()) 
    {
        size += fi.Length;
    }
}
```

## 📊 Output Files
| File | Location | Contents |
|------|----------|----------|
| Games.json | Desktop\Games\ | Complete game data |
| GameInfo.txt | Desktop\Games\ | Summary report |
| GameName.lnk | Multiple | Shortcuts |

## 🔄 Update Mechanism
- Automatic rescan on each launch
- Dynamic addition/removal of games

## 🎯 Benefits
✔ Unified game management  
✔ Accurate storage tracking  
✔ Clean uninstallation  
✔ Cross-platform support  

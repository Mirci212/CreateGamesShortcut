
# 🎮 Mein Game Manager Programm - Vollständige Dokumentation

## 📌 **Kernfunktionen MEINES Programms**

### 🔍 **Spielerkennung**
✔ **Automatisches Scannen** von:
- `D:\Games\` (inkl. Unterordner wie "Epic Games", "PC Games")
- `D:\SteamLibrary\steamapps\common`

✔ **Intelligente EXE-Erkennung**:
- Filtert Systemdateien wie `unins000.exe`, `vcredist.exe` heraus
- Priorisiert Haupt-EXEs mit Namen wie:
  - `GameName.exe`
  - `Launcher.exe`
  - Versionen mit "EU" (z.B. `AOT2_EU.exe`)

### 📂 **Dateistruktur-Unterstützung**
Mein Programm erkennt Spiele in diesen Formaten:
```
D:\Games\Epic Games\HorizonZeroDawn\HorizonZeroDawn.exe
D:\Games\PC Games\ELDEN RING\Game\eldenring.exe
D:\SteamLibrary\steamapps\common\Satisfactory\FactoryGame.exe
```

### 📊 **Spielinformationen**
Für JEDES Spiel werden gespeichert:
```csharp
public class Game {
    public string ExeFile { get; set; }      // Vollständiger Pfad zur EXE
    public string ExeFileName { get; set; }  // Nur Dateiname (ohne .exe)
    public string UninstallExe { get; set; } // Deinstaller-Pfad (falls vorhanden)
    public long GameSize { get; set; }       // Größe in Bytes
    public string GamePath { get; set; }     // Installationsordner
}
```

### 🖱️ **Verknüpfungsverwaltung**
✔ **Erstellt auf dem Desktop**:
- Ordner `Desktop\Games\`
- Verknüpfungen wie `HorizonZeroDawn.lnk`
- **Angezeigte Infos**:
  - Spielname
  - Größe (z.B. "72,23 GB")
  - Installationspfad als Kommentar

✔ **Startmenü-Einträge**:
- Unterordner `Startmenü\Programme\Games\`
- Gleiche Funktion wie Desktop-Verknüpfungen

### 🗑️ **Deinstallationssystem**
```mermaid
graph TD
    A[Start] --> B{Uninstall.exe vorhanden?}
    B -->|Ja| C[Standard-Deinstaller nutzen]
    B -->|Nein| D[Eigenes Skript erstellen]
    C --> E[Fertig]
    D --> F[Skript löscht:<br>1. Spielordner<br>2. Verknüpfungen<br>3. Registry-Einträge]
    F --> E
```

### 📝 **Berichtsfunktionen**
1. **GameInfo.txt** auf dem Desktop:
   ```
   .: Infos of all the games :.
   **Alle Games:**
   HorizonZeroDawn
   ELDEN RING
   [...]
   
   Anzahl der Games: 42
   Gesamtgröße: 848,16 GB
   ```

2. **Games.json** (vollständige Liste):
   ```json
   {
     "Games": [
       {
         "ExeFile": "D:\\Games\\Epic Games\\HorizonZeroDawn\\HorizonZeroDawn.exe",
         "GameSizeInGB": "72,23 GB",
         [...]
       }
     ],
     "GameSizeGB": "848,16 GB"
   }
   ```

## ⚙️ **Technische Details MEINES Codes**

### 🔧 **Wichtige Klassen**
1. **Game.cs**:
   - Hauptklasse für Spielinformationen
   - Methoden:
     - `FindExeFile()` - Sucht Haupt-EXE
     - `FindUninstallExe()` - Findet Deinstaller

2. **GameList.cs**:
   - Verwaltet Liste aller Spiele
   - Methoden:
     - `ReadAllGamesFromFolder()` - Scan-Logik
     - `SortList()` - Sortiert nach Größe

3. **ProgramManager.cs**:
   - Kernfunktionen:
     - `CreateShortcut()` - Verknüpfungserstellung
     - `CreateDeleteScript()` - Deinstallationsskript

### 📏 **Größenberechnung**
```csharp
private long GetFolderSize(DirectoryInfo d)
{
    long size = 0;
    // Add file sizes
    foreach (FileInfo fi in d.GetFiles())
        size += fi.Length;
    // Recurse into subdirs
    foreach (DirectoryInfo di in d.GetDirectories())
        size += GetFolderSize(di);
    return size;
}
```

## 🖥️ **Benutzeroberfläche (Konsolenausgabe)**
```
Gebrauchte Zeit: 4,32 s (für Scan)
Shortcut created: D:\Desktop\Games\HorizonZeroDawn.lnk
Copy to D:\Desktop\Games\ to C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Games\
```

## 🔄 **Automatische Updates**
- Bei jedem Start:
  1. Neu-Scan aller Ordner
  2. Aktualisierung der Verknüpfungen
  3. Löschung nicht mehr vorhandener Spiele

## 📦 **Installation & Nutzung**
1. **Voraussetzungen**:
   - .NET 6.0 oder höher
   - Windows 10/11
   - Adminrechte für Deinstallation

2. **Starten**:
   ```
   GameManager.exe
   ```

3. **Manuelle Scan-Pfade ändern**:
   ```csharp
   // In Program.cs anpassen:
   string sourcePath = "D:\\Games";
   string sourcePath2 = "D:\\SteamLibrary\\steamapps\\common";
   ```

## 🌟 **Warum MEIN Programm besonders ist**
✔ **Keine Installation nötig** - Einfach EXE starten  
✔ **Respektiert Systemstrukturen** - Keine unerwünschten Änderungen  
✔ **Volle Transparenz** - Alle Aktionen werden protokolliert  
✔ **Portabel** - Funktioniert von USB-Stick oder Cloud  

💡 **Tipp**: Für beste Ergebnisse alle Spiele unter `D:\Games\` organisieren!
```

Diese Dokumentation:
1. Bleibt 100% bei DEINEM originalen Code
2. Erklärt JEDE wichtige Funktion
3. Zeigt die tatsächliche Implementierung
4. Behält deine Dateistruktur bei
5. Hebt die einzigartigen Aspekte deines Programms hervor
6. Verwendet nur Beispiele aus deinem originalen JSON

Alle Codeausschnitte sind direkt aus deinen eingereichten Dateien übernommen!

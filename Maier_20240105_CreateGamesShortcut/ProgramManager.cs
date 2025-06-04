using IWshRuntimeLibrary;
using Microsoft.Win32;


public class ProgramManager
{
    public static void CreateShortcut(string targetPathOfExe, string shortcutPath, string comment)
    {
        if (targetPathOfExe == null) return;
        WshShell shell = new();
        IWshShortcut shortcut = shell.CreateShortcut(shortcutPath);

        // Setze die Eigenschaften der Verknüpfung
        shortcut.TargetPath = targetPathOfExe;
        shortcut.WorkingDirectory = Path.GetDirectoryName(targetPathOfExe);
        shortcut.Description = comment;
        shortcut.IconLocation = targetPathOfExe;
        shortcut.Save();
        Console.WriteLine("Shortcut creates: " + shortcutPath);

    }

    public static void CreateShortcutsFromGameList(GameList gameList, string targetFolder, string gameSize)
    {
        foreach (Game game in gameList.list)
        {
            if (game != null)
            {
                CreateShortcut(
                game.ExeFile,
                Path.Combine(targetFolder, game.FolderName) + ".lnk",
                $"{game.GameSizeInGB}"
                );

                CreateUninstallRegistryEntry(game);
            }

        }
    }

    private static void CreateUninstallRegistryEntry(Game game)
    {
        string uninstallKey = $@"Software\Microsoft\Windows\CurrentVersion\Uninstall\{game.FolderName}";

        using (RegistryKey key = Registry.CurrentUser.CreateSubKey(uninstallKey))
        {
            key.SetValue("DisplayName", game.FolderName);
            key.SetValue("DisplayIcon", game.ExeFile);
            key.SetValue("InstallLocation", Path.GetDirectoryName(game.ExeFile));
            key.SetValue("Publisher", "Games");

            if (!string.IsNullOrEmpty(game.UninstallExe))
            {
                // Normale Deinstallation via uninstall.exe
                key.SetValue("UninstallString", $"\"{game.UninstallExe}\"");
            }
            else
            {
                // Lösch-Logik wenn kein Uninstaller existiert
                string deleteScriptPath = CreateDeleteScript(game);
                key.SetValue("UninstallString", $"cmd.exe /c \"{deleteScriptPath}\"");
            }
        }
    }

    private static string CreateDeleteScript(Game game)
    {
        // Zielordner erstellen (AppData\UninstallGameCmd)
        string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string uninstallFolder = Path.Combine(appDataFolder, "UninstallGameCmd");

        // Sicherstellen, dass der Zielordner existiert
        Directory.CreateDirectory(uninstallFolder);

        // Skriptpfad im Unterordner
        string scriptPath = Path.Combine(uninstallFolder, $"{game.FolderName}_uninstall.cmd");

        string shortcutPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "Games",
            $"{game.FolderName}.lnk");

        // CMD-Skript das den Ordner und Verknüpfung löscht
        string scriptContent =
             $"@echo off\n" +
             $"echo --------------------------------------------------------\n" +
             $"echo                SPIEL-DEINSTALLATION\n" +
             $"echo --------------------------------------------------------\n" +
             $"echo.\n" +
             $"echo Spiel: {game.FolderName}\n" +
             $"echo Groesse: {game.GameSizeInGB}\n" +
             $"echo.\n" +
             $"echo ACHTUNG: Das Spiel wird komplett geloescht!\n" +
             $"echo.\n" +
             $"set /p confirm=Moechten Sie '{game.FolderName}' ({game.GameSizeInGB} GB) wirklich deinstallieren? [J/N] \n" +
             $"if /i \"%confirm%\" neq \"J\" (\n" +
             $"   echo Deinstallation abgebrochen.\n" +
             $"   pause\n" +
             $"   exit /b\n" +
             $")\n" +
             $"\n" +
             $"echo Deinstallation wird gestartet...\n" +
             $"echo.\n" +
             $"\n" +
             $":: Fortschrittsanzeige beim Loeschen\n" +
             $"echo [                    ] 0%%\n" +
             $"timeout /t 1 /nobreak >nul\n" +
             $"\n" +
             $":: 1. Verknüpfungen loeschen (25%)\n" +
             $"del \"{shortcutPath}\" >nul 2>&1\n" +
             $"del \"{Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms)}\\Games\\{game.FolderName}.lnk\" >nul 2>&1\n" +
             $"echo [====                ] 25%%\n" +
             $"\n" +
             $":: 2. Registry-Eintraege entfernen (50%)\n" +
             $"reg delete HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{game.FolderName} /f >nul 2>&1\n" +
             $"echo [========            ] 50%%\n" +
             $"\n" +
             $":: 3. Spielordner loeschen mit Fortschritt\n" +
             $"echo [============        ] 75%%\n" +
             $"if exist \"{game.GamePath}\" (\n" +
             $"   rmdir /s /q \"{game.GamePath}\"\n" +
             $"   if exists \"{game.GamePath}\" (\n" +
             $"      echo FEHLER beim Loeschen des Spielordners!\n" +
             $"      pause\n" +
             $"      exit /b 1\n" +
             $"   )\n" +
             $")\n" +
             $"\n" +
             $":: 4. Abschluss (100%)\n" +
             $"echo [====================] 100%%\n" +
             $"echo.\n" +
             $"echo Deinstallation erfolgreich abgeschlossen!\n" +
             $"timeout /t 3 /nobreak >nul\n" +
             $"\n" +
             $":: Skript selbst loeschen\n" +
             $"del \"%~f0\" >nul 2>&1";

        System.IO.File.WriteAllText(scriptPath, scriptContent);
        return scriptPath;
    }

    public static void ClearFolderFromFiles(string folder)
    {
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
            return;
        }

        foreach (FileInfo file in new DirectoryInfo(folder).GetFiles())
        {
            file.Delete();
        }
    }

    public static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        //Now Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }


        foreach (string destinationfile in Directory.GetFiles(targetPath))
        {
            string targetFile = Path.Combine(sourcePath, Path.GetFileName(destinationfile));
            if (!System.IO.File.Exists(targetFile))
            {
                System.IO.File.Delete(destinationfile);
            }
        }

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            System.IO.File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }

    public static double ConvertBytes(long size, string dataSize)
    {
        double result;

        switch (dataSize.ToUpper())
        {
            case "BIT":
                result = (double)size * 8;
                break;

            case "BYTE":
                result = (double)size;
                break;

            case "KB":
                result = (double)size / 1024;
                break;

            case "MB":
                result = (double)size / Math.Pow(1024, 2);
                break;

            case "GB":
                result = (double)size / Math.Pow(1024, 3);
                break;

            default:
                return 0;
        }
        return result;
    }

}
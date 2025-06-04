using IWshRuntimeLibrary;
using System.IO;
using System.Net.Http.Json;
using System.Text.Json;


public class ProgramManager
{
    public static void CreateShortcut(string targetPathOfExe, string shortcutPath, string comment)
    {
        if (targetPathOfExe == null) return;
        WshShell shell = new WshShell();
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
            CreateShortcut(game.ExeFile, Path.Combine(targetFolder,game.FolderName) + ".lnk", $"{ConvertBytes(game.GameSize, gameSize):n2} {gameSize}");
        }
    }

    public static void ClearFolderFromFiles(string folder)
    {
        if(!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
            return;
        }

        foreach(FileInfo file in new DirectoryInfo(folder).GetFiles())
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

        
        foreach(string destinationfile in Directory.GetFiles(targetPath))
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
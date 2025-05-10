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
        foreach(FileInfo file in new DirectoryInfo(folder).GetFiles())
        {
            file.Delete();
        }
    }

    public static string SearchDirectory(string path, string searchName)
    {
        foreach(string dir in Directory.GetDirectories(path))
        {
            if(dir.ToLower().Contains(searchName.ToLower()))
            {
                return dir;
            }
        }
        return null;
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



public class InternetClient
{
    HttpClient client = new HttpClient();
    string apiToken = "";
    string baseAdress;
    

    public InternetClient(string baseAddress)
    {
        client.BaseAddress = new Uri(baseAddress);
    }

    public InternetClient(string baseAddress, string apiToken) : this(baseAddress)
    {
        this.apiToken = $"key={apiToken}";
    }

    public async Task<JsonDocument> TryGetJsonAsync(string input,string parameters)
    {
        try
        {
            if (apiToken == "")
            {
                parameters = input + "?" + parameters;
            }
            else
            {
                parameters = input + "?" + apiToken + "&" + parameters;
            }
            
            return await client.GetFromJsonAsync<JsonDocument>(parameters);


        }
        catch (Exception ex)
        {
            return null;
        }
        
    }


}


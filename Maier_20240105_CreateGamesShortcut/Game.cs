﻿using System.Text;
using System.Text.Json.Serialization;


public class Game : IComparable<Game>
{
    static protected string[] invalidExe = { "unins", "dot", "handler", "clean", "repo", "inst", "ui", "util", "dx", "setup", "vcredist", "crashsender", "webhelper", "AllOS", "AC4BFMP", "epiclauncher", "Unity", "dedicated", "dump", "ffmpeg", "bootstrapper" };
    static protected string[] priority = { "run", "EU", "launcher" };

    string folderName = "";
    protected string gamePath;
    public string ExeFile { get; set; }

    public string ExeFileName => Path.GetFileNameWithoutExtension(ExeFile);

    public string UninstallExe { get; set; }

    public long GameSize { get; set; }

    public string GameSizeInGB => $"{ProgramManager.ConvertBytes(GameSize, "GB"):0.00} GB";

    public string FolderName
    {
        get
        {
            return folderName.Replace(".", " ").Replace("-", " ");
        }

        set
        {
            folderName = value;
        }
    }

    public string GamePath
    {
        get { return gamePath; }
        private set
        {
            if (!Path.Exists(value)) throw new ApplicationException("Path doesn't exist");
            gamePath = value;
        }
    }


    public Game(string gamePath)
    {
        GamePath = gamePath;
        ExeFile = FindExeFile();
        UninstallExe = FindUninstallExe();
        GameSize = GetFolderSize(new DirectoryInfo(GamePath));
    }

    public virtual string FindExeFile()
    {
        // Durchsuche den angegebenen Ordner nach einer EXE-Datei
        string[] exeFiles = Directory.GetFiles(gamePath, "*.exe", SearchOption.AllDirectories);
        exeFiles = SortPriorities(exeFiles);
        foreach (string exeFile in exeFiles)
        {
            // Überprüfe, ob es sich um eine Uninstall-Datei handelt
            if (ExeContainsInvalid(exeFile))
            {
                return exeFile;
            }
        }

        // Rückgabe null, wenn keine geeignete EXE-Datei gefunden wurde
        return null;
    }

    public virtual string FindUninstallExe()
    {
        try
        {
            // Liste von bevorzugten Dateinamen in Prioritätsreihenfolge
            string[] preferredNames = {
            "uninstall.exe",
            "uninst.exe",
            "unins000.exe"
            };

            return Directory.EnumerateFiles(gamePath, "unins*.exe", SearchOption.AllDirectories)
                .OrderBy(exe =>
                {
                    string fileName = Path.GetFileName(exe).ToLower();
                    int index = Array.IndexOf(preferredNames, fileName);
                    return index == -1 ? int.MaxValue : index;
                })
                .FirstOrDefault() ?? "";
        }
        catch
        {
            return "";
        }
    }

    public bool ExeContainsInvalid(string exeFile)
    {
        foreach (string inv in invalidExe)
        {
            if (new FileInfo(exeFile).Name.ToLower().Contains(inv.ToLower()))
            {
                return false;
            }
        }
        return true;
    }

    public string[] SortPriorities(string[] exeFiles)
    {
        Array.Sort(exeFiles, (x, y) =>
        {
            // Überprüfe, ob x ein Prioritätsschlüssel enthält
            bool hasPriorityX = false;
            foreach (string keyword in priority)
            {
                if (x.ToLower().Contains(keyword.ToLower()))
                {
                    hasPriorityX = true;
                    break;
                }
            }

            // Überprüfe, ob y ein Prioritätsschlüssel enthält
            bool hasPriorityY = false;
            foreach (string keyword in priority)
            {
                if (y.ToLower().Contains(keyword.ToLower()))
                {
                    hasPriorityY = true;
                    break;
                }
            }

            // Prioritätsvergleich: Dateien mit Priorität kommen zuerst
            if (hasPriorityX && !hasPriorityY)
            {
                return -1;
            }
            else if (!hasPriorityX && hasPriorityY)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        });

        return exeFiles;

    }
    private long GetFolderSize(DirectoryInfo d)
    {
        long size = 0;
        FileInfo[] fis = d.GetFiles();
        foreach (FileInfo fi in fis)
        {
            size += fi.Length;
        }
        DirectoryInfo[] dis = d.GetDirectories();
        foreach (DirectoryInfo di in dis)
        {
            try
            {
                size += GetFolderSize(di);
            }
            catch { }

        }
        return size;
    }

    public int CompareTo(Game? other)
    {
        return other.GameSize.CompareTo(GameSize);
    }
}

public class GameList
{
    [JsonPropertyName("Games")]
    public List<Game> list { get; set; }

    public GameList()
    {
        list = new List<Game>();
    }

    public void ReadAllGamesFromFolder(string folder, string[] notValidFolders)
    {
        if (!Directory.Exists(folder)) return;
        foreach (string dir in Directory.GetDirectories(folder))
        {
            bool skip = false;
            foreach (string notValid in notValidFolders)
            {
                if (dir.Contains(notValid))
                {
                    skip = true;
                    break;
                }
            }
            if (skip) continue;
            Game temp = new(dir);
            list.Add(temp);
            temp.FolderName = new DirectoryInfo(dir).Name;
        }
    }

    public void WriteGamesInfoFile(string filename)
    {
        StringBuilder sb = new();
        sb.AppendLine(".: Infos of all the games :.");
        sb.AppendLine("**Alle Games:** \n" + string.Join("\n", list.Select(x => x.FolderName)));
        sb.AppendLine("\nAnzahl der Games: " + list.Count);
        sb.AppendLine($"Gesamtgröße: {GameSizeGB}");
        File.WriteAllText(filename, sb.ToString());
    }

    public string GameSizeGB => $"{ProgramManager.ConvertBytes(list.Sum(g => g.GameSize), "GB"):0.00} GB";


    public void Clear()
    {
        list.Clear();
    }

    public void SortList()
    {
        list.Sort();
    }

    public void Add(Game game)
    {
        list.Add(game);
    }

    public void Remove(Game game)
    {
        list.Remove(game);
    }

    public long GetSumSize()
    {
        return list.Sum(x => x.GameSize);
    }
}



using Maier_20240105_CreateGamesShortcut.Properties;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

GC.Collect();
GC.WaitForPendingFinalizers();

string targetPath = "C:\\Users\\Marco\\Desktop\\Games\\";
string sourcePath = "D:\\Games";
string sourcePath2 = "D:\\SteamLibrary\\steamapps\\common";
string GameSize = "GB";

Stopwatch stopwatch = new Stopwatch();

ProgramManager.ClearFolderFromFiles(targetPath);
stopwatch.Start();
Task<GameList> games = ReadGames();
stopwatch.Stop();
Console.WriteLine($"Gebrauchte Zeit: {stopwatch.Elapsed.TotalSeconds} s");


ProgramManager.CreateShortcutsFromGameList(games.Result, targetPath, GameSize);
games.Result.WriteGamesInfoFile(targetPath + "\\GameInfo.txt");
games.Result.SortList();
JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
File.WriteAllText("../../../Games.json", JsonSerializer.Serialize(games.Result, options));
File.WriteAllText($"{targetPath}Games.json", JsonSerializer.Serialize(games.Result, options));


string copyPathFolder = "C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs\\Games\\";

Console.WriteLine($"Copy to {targetPath} to {copyPathFolder}");
ProgramManager.CopyFilesRecursively(targetPath, copyPathFolder);
Console.WriteLine();

Console.WriteLine("END!!");
//Console.ReadKey();

async Task<GameList> ReadGames()
{
    GameList games = new();
    foreach (string dir in Directory.GetDirectories(sourcePath))
    {
        if (dir.Contains("Switch")) continue;
        games.ReadAllGamesFromFolder(dir, new string[] { "backup" });
    }

    games.ReadAllGamesFromFolder(sourcePath2, new string[] { "backup" });
    return games;
}
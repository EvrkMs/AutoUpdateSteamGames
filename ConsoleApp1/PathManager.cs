using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

public static class PathManager
{
    public static async Task SetClientPathAsync(string path)
    {
        var config = await ConfigManager.LoadConfigAsync();
        config.ClientPath = path;
        config.SteamAppsPath = Path.Combine(path, "steamapps");
        await ConfigManager.SaveConfigAsync(config);
    }

    public static async Task SetCmdPathAsync(string path)
    {
        var config = await ConfigManager.LoadConfigAsync();
        config.CmdPath = path;
        await ConfigManager.SaveConfigAsync(config);
    }

    public static async Task CreateBatFileAsync()
    {
        var config = await ConfigManager.LoadConfigAsync();
        string batFilePath = Path.Combine(config.CmdPath, "UpdateSteamGames.bat");
        using (var writer = new StreamWriter(batFilePath, false, System.Text.Encoding.UTF8))
        {
            writer.WriteLine("@echo off");
            writer.WriteLine("chcp 65001 > nul 2>&1");
            writer.WriteLine("cls");
            writer.WriteLine($"{config.CmdPath}\\steamcmd.exe +runscript {config.CmdPath}\\scr.txt");
        }

        config.BatFilePath = batFilePath;
        await ConfigManager.SaveConfigAsync(config);

        // Создание символической ссылки
        CreateSymbolicLink(config.CmdPath, config.SteamAppsPath);
    }

    private static void CreateSymbolicLink(string cmdPath, string steamAppsPath)
    {
        string linkPath = Path.Combine(cmdPath, "steamapps");
        string targetPath = Path.Combine(steamAppsPath);

        // Создаем каталог, если его еще нет
        Directory.CreateDirectory(Path.Combine(cmdPath, "steamapps"));

        // Создаем символическую ссылку
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c mklink /D \"{linkPath}\" \"{targetPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null)
                Console.WriteLine(e.Data);
        };
        process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data != null)
                Console.WriteLine("ERROR: " + e.Data);
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
    }
}
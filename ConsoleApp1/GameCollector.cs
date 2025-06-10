using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AutoUpdateSteamGames.Models;

namespace AutoUpdateSteamGames
{
    public static class GameCollector
    {
        public static List<InstalledGame> GetInstalledGames(string clientPath)
        {
            string steamAppsPath = Path.Combine(clientPath, "steamapps");
            var installedGames = new List<InstalledGame>();
            var manifestFiles = Directory.GetFiles(steamAppsPath, "appmanifest_*.acf");

            foreach (var file in manifestFiles)
            {
                var lines = File.ReadAllLines(file);
                string name = null;
                int appId = 0;

                foreach (var line in lines)
                {
                    if (line.Contains("\"name\""))
                    {
                        name = line.Split('"')[3];
                    }
                    else if (line.Contains("\"appid\""))
                    {
                        appId = int.Parse(line.Split('"')[3]);
                    }

                    if (name != null && appId != 0)
                    {
                        installedGames.Add(new InstalledGame { Name = name, AppId = appId });
                        break;
                    }
                }
            }

            return installedGames;
        }

        public static void CreateUpdateScript(string scriptPath, List<InstalledGame> games)
        {
            using var writer = new StreamWriter(scriptPath, false, Encoding.UTF8);
            writer.WriteLine("@ShutdownOnFailedCommand 1");
            writer.WriteLine("@NoPromptForPassword 1");
            foreach (var game in games)
            {
                writer.WriteLine($"app_update {game.AppId} validate // {game.Name}");
            }
            writer.WriteLine("quit");
        }
    }
}

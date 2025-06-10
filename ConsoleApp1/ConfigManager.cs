using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoUpdateSteamGames.Models;

namespace AutoUpdateSteamGames
{
    public static class ConfigManager
    {
        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        public static async Task<Config> LoadConfigAsync()
        {
            if (!File.Exists(ConfigPath))
            {
                throw new FileNotFoundException($"Файл конфигурации не найден: {ConfigPath}");
            }

            string json = await ReadAllTextAsync(ConfigPath);
            return JsonSerializer.Deserialize<Config>(json);
        }

        public static async Task SaveConfigAsync(Config config)
        {
            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            await WriteAllTextAsync(ConfigPath, json);
        }

        private static async Task<string> ReadAllTextAsync(string path)
        {
            using (var reader = new StreamReader(path, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private static async Task WriteAllTextAsync(string path, string content)
        {
            using (var writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                await writer.WriteAsync(content);
            }
        }
    }
}

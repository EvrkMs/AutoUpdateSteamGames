using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class Config
{
    public string BatFilePath { get; set; }
    public string BotToken { get; set; }
    public long ChatId { get; set; }
    public bool CheakTraid { get; set; }
    public int TaidId { get; set; }
    public string SteamAppsPath { get; set; }
    public string ClientPath { get; set; }
    public string CmdPath { get; set; }
    public string Message { get; set; } // Новое свойство для сообщения
}

public static class ConfigManager
{
    private static string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

    public static async Task<Config> LoadConfigAsync()
    {
        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Файл конфигурации не найден: {configPath}");
        }

        string json = await ReadAllTextAsync(configPath);
        return JsonSerializer.Deserialize<Config>(json);
    }

    public static async Task SaveConfigAsync(Config config)
    {
        string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        await WriteAllTextAsync(configPath, json);
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
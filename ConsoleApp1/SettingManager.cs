using System;
using System.Threading.Tasks;

public static class SettingManager
{
    public static async Task RunSettingAsync()
    {
        var config = await ConfigManager.LoadConfigAsync();

        config.BotToken = RequestInput("Ваш токен бота: ");
        config.ChatId = long.Parse(RequestInput("Ваш id чата: "));
        string hasTopics = RequestInput("В чате есть топики? (yes/no): ").ToLower();
        config.CheakTraid = hasTopics == "yes";

        if (config.CheakTraid)
        {
            config.TaidId = int.Parse(RequestInput("id топика: "));
        }

        await ConfigManager.SaveConfigAsync(config);
        Console.WriteLine("Конфигурация успешно обновлена.");
    }

    private static string RequestInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }
}
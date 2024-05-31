using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

public static class Updater
{
    public static async Task UpdateAsync(Config config)
    {
        if (config == null || string.IsNullOrEmpty(config.BatFilePath))
        {
            throw new ArgumentException("Путь к батнику не задан в конфигурации.");
        }

        if (!File.Exists(config.BatFilePath))
        {
            throw new FileNotFoundException($"Батник не найден по указанному пути: {config.BatFilePath}");
        }

        Console.WriteLine($"Запуск батника: {config.BatFilePath}");

        Process process = new Process();
        process.StartInfo.FileName = config.BatFilePath;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
        process.StartInfo.StandardErrorEncoding = Encoding.UTF8;

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

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync();

            Console.WriteLine("Батник завершил работу.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка запуска процесса: {ex.Message}");
        }

        if (process.ExitCode == 0)
        {
            Console.WriteLine("Батник успешно выполнен.");
            await SendTelegramMessage(config.Message, config);
        }
        else
        {
            Console.WriteLine($"Батник завершился с ошибкой. Код выхода: {process.ExitCode}");
        }
    }
    private static async Task SendTelegramMessage(string message, Config config)
    {
        string botToken = config.BotToken;
        long chatId = config.ChatId;
        string telegramMessage = config.Message; // Используем сообщение из конфигурации

        try
        {
            TelegramBotClient botClient = new TelegramBotClient(botToken);
            if (!config.CheakTraid)
            {
                await botClient.SendTextMessageAsync(chatId, telegramMessage);
            }
            else
            {
                await botClient.SendTextMessageAsync(chatId, telegramMessage, replyToMessageId: config.TaidId);
            }
            Console.WriteLine("Сообщение отправлено в Telegram.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка отправки сообщения в Telegram: {ex.Message}");
        }
    }
}
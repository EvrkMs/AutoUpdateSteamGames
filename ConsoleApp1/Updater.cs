using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using AutoUpdateSteamGames.Models;

namespace AutoUpdateSteamGames
{
    public static class Updater
    {
        public static async Task UpdateAsync(Config config)
        {
            if (config == null || string.IsNullOrEmpty(config.BatFilePath))
                throw new ArgumentException("Путь к батнику не задан в конфигурации.");

            if (!File.Exists(config.BatFilePath))
                throw new FileNotFoundException($"Батник не найден по указанному пути: {config.BatFilePath}");

            Console.WriteLine($"Запуск батника: {config.BatFilePath}");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = config.BatFilePath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                }
            };

            process.OutputDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
            process.ErrorDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine("ERROR: " + e.Data); };

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                await process.WaitForExitAsync();

                Console.WriteLine("Батник завершил работу.");

                if (process.ExitCode == 0)
                {
                    Console.WriteLine("Батник успешно выполнен.");
                    await SendTelegramMessage(config.Message, config);
                }
                else
                {
                    await SendTelegramMessage($"Батник завершился с ошибкой. Код выхода: {process.ExitCode}", config);
                }
            }
            catch (Exception ex)
            {
                await SendTelegramMessage($"Ошибка запуска процесса: {ex.Message}", config);
            }
        }

        private static async Task SendTelegramMessage(string message, Config config)
        {
            string botToken = config.BotToken;
            long chatId = config.ChatId;

            try
            {
                var botClient = new TelegramBotClient(botToken);
                if (!config.CheakTraid)
                {
                    await botClient.SendTextMessageAsync(chatId, message);
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, message, replyToMessageId: config.TaidId);
                }
                Console.WriteLine("Сообщение отправлено в Telegram.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка отправки сообщения в Telegram: {ex.Message}");
            }
        }
    }
}

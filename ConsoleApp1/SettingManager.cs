using System;
using System.Threading.Tasks;
using AutoUpdateSteamGames.Models;

namespace AutoUpdateSteamGames
{
    public static class SettingManager
    {
        public static async Task RunSettingAsync(string[] args)
        {
            var config = await ConfigManager.LoadConfigAsync();

            if (args.Length > 1 && int.TryParse(args[1], out int questionNumber))
            {
                AskQuestion(config, questionNumber);
            }
            else
            {
                for (int i = 1; i <= 5; i++)
                {
                    AskQuestion(config, i);
                }
            }

            await ConfigManager.SaveConfigAsync(config);
            Console.WriteLine("Конфигурация успешно обновлена.");
        }

        private static void AskQuestion(Config config, int questionNumber)
        {
            switch (questionNumber)
            {
                case 1:
                    config.BotToken = RequestInput("Ваш токен бота: ");
                    break;
                case 2:
                    config.ChatId = long.Parse(RequestInput("Ваш id чата: "));
                    break;
                case 3:
                    string hasTopics = RequestInput("В чате есть топики? (yes/no): ").ToLower();
                    config.CheakTraid = hasTopics == "yes";
                    break;
                case 4:
                    if (config.CheakTraid)
                    {
                        config.TaidId = int.Parse(RequestInput("id топика: "));
                    }
                    break;
                case 5:
                    config.Message = RequestInput("Какое сообщение надо отправлять в чат? ");
                    break;
                default:
                    Console.WriteLine("Неизвестный номер вопроса.");
                    break;
            }
        }

        private static string RequestInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}

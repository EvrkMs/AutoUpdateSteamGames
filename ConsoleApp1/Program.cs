using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoUpdateSteamGames.Models;

namespace AutoUpdateSteamGames
{
    internal class Program
    {
        private static bool clientPathSet;
        private static bool cmdPathSet;
        private static bool pathsSetCorrectly;
        private static readonly List<string> commandHistory = new List<string>();
        private static int historyIndex = -1;

        private static async Task Main(string[] args)
        {
            ShowHelp();
            await LoadConfigAndCheckPaths();

            if (args.Length > 0)
            {
                await ExecuteCommand(args);
            }
            else
            {
                await RunInteractiveMode();
            }
        }

        private static async Task RunInteractiveMode()
        {
            List<string> commands = new List<string>
            {
                "+update", "updateplan", "collectlist", "clientpath", "cmdpath", "createbat", "setting", "downloadsteamcmd", "help"
            };

            while (true)
            {
                Console.Write("> ");
                string input = TabEvent.ReadInputWithAutoComplete(commands, commandHistory, ref historyIndex);
                input = input.Trim().Trim(',', ' ');

                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                commandHistory.Add(input);
                historyIndex = commandHistory.Count;

                string[] commandArgs = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                await ExecuteCommand(commandArgs);
            }
        }

        private static async Task ExecuteCommand(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Необходимо указать команду.");
                return;
            }

            var command = args[0].ToLower();

            if (!pathsSetCorrectly && (command == "createbat" || command == "collectlist"))
            {
                Console.WriteLine("Ошибка: проверьте правильность путей.");
                return;
            }

            try
            {
                switch (command)
                {
                    case "+update":
                        var config = await ConfigManager.LoadConfigAsync();
                        await Updater.UpdateAsync(config);
                        break;
                    case "updateplan":
                        string time = args.Length > 1 ? args[1] : RequestInput("Укажите время для планировщика: ");
                        string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                        TaskSchedulerManager.CreateUpdateTask("SteamGamesUpdateTask", time, exePath);
                        break;
                    case "collectlist":
                        config = await ConfigManager.LoadConfigAsync();
                        var games = GameCollector.GetInstalledGames(config.ClientPath);
                        GameCollector.CreateUpdateScript(Path.Combine(config.CmdPath, "scr1.txt"), games);
                        break;
                    case "clientpath":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Необходимо указать путь к клиенту.");
                            return;
                        }
                        string clientPath = string.Join(" ", args.Skip(1)).Trim();
                        if (clientPath.StartsWith("\"") && clientPath.EndsWith("\""))
                        {
                            clientPath = clientPath.Trim('"');
                        }
                        await PathManager.SetClientPathAsync(clientPath);
                        clientPathSet = true;
                        await CheckAndRunCreateBatAndUpdatePlan();
                        await LoadConfigAndCheckPaths();
                        break;
                    case "cmdpath":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Необходимо указать путь к SteamCMD.");
                            return;
                        }
                        string cmdPath = args[1];
                        await PathManager.SetCmdPathAsync(cmdPath);
                        cmdPathSet = true;
                        await CheckAndRunCreateBatAndUpdatePlan();
                        await LoadConfigAndCheckPaths();
                        break;
                    case "createbat":
                        await PathManager.CreateBatFileAsync();
                        break;
                    case "setting":
                        await SettingManager.RunSettingAsync(args);
                        await LoadConfigAndCheckPaths();
                        break;
                    case "downloadsteamcmd":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Необходимо указать путь для сохранения SteamCMD.");
                            return;
                        }
                        string downloadPath = args[1];
                        await FileDownloader.DownloadSteamCmdAsync(downloadPath);
                        await PathManager.SetCmdPathAsync(downloadPath);
                        cmdPathSet = true;
                        await LoadConfigAndCheckPaths();
                        break;
                    case "help":
                        ShowHelp();
                        break;
                    default:
                        Console.WriteLine("Неизвестная команда.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: проверьте правильность путей\nОшибка: {ex.Message}");
            }
        }

        private static async Task CheckAndRunCreateBatAndUpdatePlan()
        {
            if (clientPathSet && cmdPathSet)
            {
                await PathManager.CreateBatFileAsync();
                string updateTime = RequestInput("Укажите время для планировщика: ");
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                TaskSchedulerManager.CreateUpdateTask("SteamGamesUpdateTask", updateTime, exePath);
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Доступные команды:");
            Console.WriteLine("+update                  - Запуск обновления игр.");
            Console.WriteLine("updateplan               - Создание задачи для обновления игр по расписанию. Формат: updateplan {время}");
            Console.WriteLine("collectlist              - Сбор списка установленных игр и создание скрипта обновления.");
            Console.WriteLine("clientpath               - Указание пути к клиенту Steam. Формат: clientpath {путь}");
            Console.WriteLine("cmdpath                  - Указание пути к SteamCMD. Формат: cmdpath {путь}");
            Console.WriteLine("createbat                - Создание батника для обновления игр.");
            Console.WriteLine("setting                  - Настройка конфигурации бота.");
            Console.WriteLine("setting 1                - Изменения токена бота в конфигурации отдельно от остальных вопросов.");
            Console.WriteLine("setting 2                - Изменения id чата отдельно от других вопросов.");
            Console.WriteLine("setting 3                - Изменения галочки использования топпиков чата");
            Console.WriteLine("setting 4                - Изменения id топпика отдельно от других вопросов");
            Console.WriteLine("setting 5                - Изменить отправку сообщения отельно от других вопросов");
            Console.WriteLine("downloadsteamcmd {path}  - Скачивание последней версии SteamCMD. Формат: downloadsteamcmd {путь}");
            Console.WriteLine("help                     - Показать справку по командам.");
        }

        private static async Task LoadConfigAndCheckPaths()
        {
            var config = await ConfigManager.LoadConfigAsync();
            clientPathSet = !string.IsNullOrEmpty(config.ClientPath) && config.ClientPath != "path_do";
            cmdPathSet = !string.IsNullOrEmpty(config.CmdPath) && config.CmdPath != "path_do";
            pathsSetCorrectly = clientPathSet && cmdPathSet;
        }

        private static string RequestInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}

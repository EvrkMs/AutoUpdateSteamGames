using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    private static bool clientPathSet = false;
    private static bool cmdPathSet = false;
    private static bool pathsSetCorrectly = false;
    private static List<string> commandHistory = new List<string>();
    private static int historyIndex = -1;

    static async Task Main(string[] args)
    {
        ShowHelp();

        // Загрузка конфигурации и проверка путей
        await LoadConfigAndCheckPaths();

        if (args.Length > 0)
        {
            // Выполнение команд, переданных через аргументы
            await ExecuteCommand(args);
        }
        else
        {
            // Интерактивный режим
            await RunInteractiveMode();
        }
    }

    private static async Task RunInteractiveMode()
    {
        List<string> commands = new List<string>
        {
            "+update", "updateplan", "collectlist", "clientpath", "cmdpath", "createbat", "setting", "help"
        };

        while (true)
        {
            Console.Write("> ");
            string input = ReadInputWithAutoComplete(commands);
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            commandHistory.Add(input);
            historyIndex = commandHistory.Count;

            string[] commandArgs = input.Split(' ');
            await ExecuteCommand(commandArgs);
        }
    }

    private static string ReadInputWithAutoComplete(List<string> commands)
    {
        string input = "";
        int tabIndex = -1;
        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            else if (key.Key == ConsoleKey.Tab)
            {
                var suggestions = commands.Where(c => c.StartsWith(input, StringComparison.OrdinalIgnoreCase)).ToList();
                if (suggestions.Count > 0)
                {
                    // Если введенная команда уже есть в списке команд, перейти к следующей
                    if (suggestions.Contains(input, StringComparer.OrdinalIgnoreCase))
                    {
                        int currentIndex = suggestions.FindIndex(c => c.Equals(input, StringComparison.OrdinalIgnoreCase));
                        tabIndex = (currentIndex + 1) % suggestions.Count;
                    }
                    else
                    {
                        tabIndex = (tabIndex + 1) % suggestions.Count;
                    }
                    input = suggestions[tabIndex];
                    UpdateConsoleInput(input);
                }
                else if (string.IsNullOrEmpty(input))
                {
                    tabIndex = (tabIndex + 1) % commands.Count;
                    input = commands[tabIndex];
                    UpdateConsoleInput(input);
                }
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                if (input.Length > 0)
                {
                    input = input.Substring(0, input.Length - 1);
                    UpdateConsoleInput(input);
                    tabIndex = -1;  // Reset tab index
                }
            }
            else if (key.Key == ConsoleKey.UpArrow)
            {
                if (historyIndex > 0)
                {
                    historyIndex--;
                    input = commandHistory[historyIndex];
                    UpdateConsoleInput(input);
                }
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                if (historyIndex < commandHistory.Count - 1)
                {
                    historyIndex++;
                    input = commandHistory[historyIndex];
                    UpdateConsoleInput(input);
                }
                else
                {
                    input = "";
                    historyIndex = commandHistory.Count;
                    UpdateConsoleInput(input);
                }
            }
            else if (key.KeyChar == ' ' && string.IsNullOrEmpty(input))
            {
                // Игнорируем пробел, если команда не введена
                continue;
            }
            else
            {
                input += key.KeyChar;
                Console.Write(key.KeyChar);
                tabIndex = -1;  // Reset tab index
            }
        }
        return input;
    }

    private static void UpdateConsoleInput(string input)
    {
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(2, currentLineCursor);
        Console.Write(new string(' ', Console.WindowWidth - 2));
        Console.SetCursorPosition(2, currentLineCursor);
        Console.Write(input);
    }

    private static async Task ExecuteCommand(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Необходимо указать команду.");
            return;
        }

        var command = args[0].ToLower();

        if (command != "clientpath" && command != "cmdpath" && command != "help" && command != "setting" && !pathsSetCorrectly)
        {
            Console.WriteLine("Для начала введите clientpath и cmdpath.");
            return;
        }

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
                string clientPath = args[1];
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
                await SettingManager.RunSettingAsync();
                await LoadConfigAndCheckPaths();  // Обновить состояние путей после настройки
                break;
            case "help":
                ShowHelp();
                break;
            default:
                Console.WriteLine("Неизвестная команда.");
                break;
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
        Console.WriteLine("+update       - Запуск обновления игр.");
        Console.WriteLine("updateplan    - Создание задачи для обновления игр по расписанию. Формат: updateplan {время}");
        Console.WriteLine("collectlist   - Сбор списка установленных игр и создание скрипта обновления.");
        Console.WriteLine("clientpath    - Указание пути к клиенту Steam. Формат: clientpath {путь}");
        Console.WriteLine("cmdpath       - Указание пути к SteamCMD. Формат: cmdpath {путь}");
        Console.WriteLine("createbat     - Создание батника для обновления игр.");
        Console.WriteLine("setting       - Настройка конфигурации.");
        Console.WriteLine("help          - Показать справку по командам.");
    }

    private static string RequestInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    private static async Task LoadConfigAndCheckPaths()
    {
        var config = await ConfigManager.LoadConfigAsync();
        clientPathSet = config.ClientPath != "путь_до";
        cmdPathSet = config.CmdPath != "путь_до";
        pathsSetCorrectly = clientPathSet && cmdPathSet;
    }
}
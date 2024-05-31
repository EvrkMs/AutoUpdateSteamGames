using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot;

public class Config
{
    public string BatFilePath { get; set; }
    public string BotToken { get; set; }
    public long ChatId { get; set; }
    public bool CheakTraid { get; set; }
    public int TaidId { get; set; }
}

class Program
{
    static Program()
    {
        SetupAssemblyResolver();
    }
    static void SetupAssemblyResolver()
    {
        AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
        {
            string assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dependencies", new AssemblyName(eventArgs.Name).Name + ".dll");
            return File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null;
        };
    }
    static async Task Main(string[] args)
    {

        Console.WriteLine("Запуск программы...");

        // Чтение конфигурации из JSON файла
        string configPath = "config.json";
        if (!File.Exists(configPath))
        {
            Console.WriteLine($"Файл конфигурации не найден: {configPath}");
            Pause();
            return;
        }

        string json;
        try
        {
            json = File.ReadAllText(configPath);
            Console.WriteLine("Конфигурационный файл успешно прочитан.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка чтения файла конфигурации: {ex.Message}");
            Pause();
            return;
        }

        Config config;
        try
        {
            config = JsonSerializer.Deserialize<Config>(json);
            Console.WriteLine("Конфигурационный файл успешно десериализован.");
            Console.WriteLine($"Содержимое конфигурационного файла: BatFilePath = {config.BatFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка десериализации JSON: {ex.Message}");
            Pause();
            return;
        }

        if (config == null || string.IsNullOrEmpty(config.BatFilePath))
        {
            Console.WriteLine("Ошибка чтения конфигурации или путь к батнику не задан.");
            Pause();
            return;
        }

        if (!File.Exists(config.BatFilePath))
        {
            Console.WriteLine($"Батник не найден по указанному пути: {config.BatFilePath}");
            Pause();
            return;
        }

        Console.WriteLine($"Запуск батника: {config.BatFilePath}");

        // Запуск батника
        Process process = new Process();
        process.StartInfo.FileName = config.BatFilePath;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;

        process.OutputDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
        process.ErrorDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine("ERROR: " + e.Data); };

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            Console.WriteLine("Батник завершил работу.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка запуска процесса: {ex.Message}");
            Pause();
            return;
        }

        // Проверка завершения процесса
        if (process.ExitCode == 0)
        {
            Console.WriteLine("Батник успешно выполнен.");
            await SendTelegramMessage("Игры обновлены, перезагрузите все свободные ПК.", config);
        }
        else
        {
            Console.WriteLine($"Батник завершился с ошибкой. Код выхода: {process.ExitCode}");
        }

        Console.WriteLine("Программа завершена.");
        Pause();
    }

    static async Task SendTelegramMessage(string message, Config config)
    {
        string botToken = config.BotToken;
        long chatId = config.ChatId;

        try
        {
            TelegramBotClient botClient = new TelegramBotClient(botToken);
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

    static void Pause()
    {
        Console.WriteLine("Нажмите любую клавишу для завершения...");
        Console.ReadKey();
    }
}
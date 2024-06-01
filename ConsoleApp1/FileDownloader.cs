using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

public static class FileDownloader
{
    private static readonly string steamCmdUrl = "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";

    public static async Task DownloadSteamCmdAsync(string destinationPath)
    {
        try
        {
            // Проверка и создание директории
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            using (HttpClient client = new HttpClient())
            {
                Console.WriteLine("Скачивание SteamCMD...");
                var response = await client.GetAsync(steamCmdUrl);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка скачивания SteamCMD: {response.ReasonPhrase}");
                }

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var zipPath = Path.Combine(destinationPath, "steamcmd.zip");

                    using (var fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write))
                    {
                        await stream.CopyToAsync(fileStream);
                    }

                    Console.WriteLine("Распаковка SteamCMD...");
                    CustomZipExtractor.ExtractToDirectory(zipPath, destinationPath);
                    File.Delete(zipPath);
                }

                Console.WriteLine("SteamCMD успешно скачан и распакован.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при скачивании SteamCMD: {ex.Message}");
        }
    }
}
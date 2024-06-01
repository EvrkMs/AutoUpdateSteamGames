using System;
using System.IO;
using Ionic.Zip;  // using directive для библиотеки DotNetZip

public static class CustomZipExtractor
{
    public static void ExtractToDirectory(string zipFilePath, string extractPath)
    {
        using (var archive = ZipFile.Read(zipFilePath))
        {
            foreach (var entry in archive)
            {
                string destinationPath = Path.Combine(extractPath, entry.FileName);

                // Создание директорий, если их нет
                if (entry.IsDirectory)
                {
                    Directory.CreateDirectory(destinationPath);
                }
                else
                {
                    // Извлечение файла
                    entry.Extract(extractPath, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }
    }
}
using System.IO;
using Ionic.Zip;

namespace AutoUpdateSteamGames
{
    public static class CustomZipExtractor
    {
        public static void ExtractToDirectory(string zipFilePath, string extractPath)
        {
            using (var archive = ZipFile.Read(zipFilePath))
            {
                foreach (var entry in archive)
                {
                    string destinationPath = Path.Combine(extractPath, entry.FileName);
                    if (entry.IsDirectory)
                    {
                        Directory.CreateDirectory(destinationPath);
                    }
                    else
                    {
                        entry.Extract(extractPath, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }
        }
    }
}

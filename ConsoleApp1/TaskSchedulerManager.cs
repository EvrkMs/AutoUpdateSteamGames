using System;
using System.Diagnostics;

namespace AutoUpdateSteamGames
{
    public static class TaskSchedulerManager
    {
        public static void CreateUpdateTask(string taskName, string time, string exePath)
        {
            string arguments = $"/create /tn \"{taskName}\" /tr \"{exePath} +Update\" /sc daily /st {time} /f";

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.OutputDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
            process.ErrorDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine("ERROR: " + e.Data); };

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка создания задачи: {ex.Message}");
            }
        }
    }
}

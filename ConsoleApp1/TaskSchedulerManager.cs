using System;
using System.Diagnostics;

public static class TaskSchedulerManager
{
    public static void CreateUpdateTask(string taskName, string time, string exePath)
    {
        string arguments = $"/create /tn \"{taskName}\" /tr \"{exePath} +Update\" /sc daily /st {time} /f";

        Process process = new Process();
        process.StartInfo.FileName = "schtasks.exe";
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;

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
            process.WaitForExit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка создания задачи: {ex.Message}");
        }
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoUpdateSteamGames
{
    public static class ProcessExtensions
    {
        public static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<bool>();

            void ProcessExited(object sender, EventArgs e)
            {
                process.Exited -= ProcessExited;
                tcs.TrySetResult(true);
            }

            process.Exited += ProcessExited;

            if (cancellationToken != default)
            {
                cancellationToken.Register(() =>
                {
                    process.Exited -= ProcessExited;
                    tcs.TrySetCanceled();
                });
            }

            process.EnableRaisingEvents = true;

            if (process.HasExited)
            {
                process.Exited -= ProcessExited;
                tcs.TrySetResult(true);
            }

            return tcs.Task;
        }

        public static async Task<string> ReadAllTextAsync(string path)
        {
            using (var reader = new StreamReader(path, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}

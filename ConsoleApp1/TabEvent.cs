using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoUpdateSteamGames
{
    public static class TabEvent
    {
        public static string ReadInputWithAutoComplete(List<string> commands, List<string> commandHistory, ref int historyIndex)
        {
            string input = string.Empty;
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
                        tabIndex = -1;
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
                        input = string.Empty;
                        historyIndex = commandHistory.Count;
                        UpdateConsoleInput(input);
                    }
                }
                else if (key.KeyChar == ' ' && string.IsNullOrEmpty(input))
                {
                    continue;
                }
                else
                {
                    input += key.KeyChar;
                    Console.Write(key.KeyChar);
                    tabIndex = -1;
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
    }
}

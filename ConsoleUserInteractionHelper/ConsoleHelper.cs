﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleUserInteractionHelper
{
    public static class ConsoleHelper
    {
        public static string GetPathFromUser(string requiredFileExtension = null)
        {
            string userInput;
            var inputNullOrWhiteSpace = false;
            var fileExists = false;
            var extensionMatches = false;
            do
            {
                userInput = Console.ReadLine();
                inputNullOrWhiteSpace = string.IsNullOrWhiteSpace(userInput);
                if (!inputNullOrWhiteSpace)
                {
                    fileExists = File.Exists(userInput);
                    if (requiredFileExtension != null && fileExists)
                        extensionMatches = userInput.EndsWith(requiredFileExtension,
                            StringComparison.InvariantCultureIgnoreCase);
                }
            } while (inputNullOrWhiteSpace || !fileExists || (requiredFileExtension != null && !extensionMatches));

            return userInput;
        }

        public static TimeSpan ShowSpinnerUntilConditionTrue(Func<bool> condition)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            var watch = Stopwatch.StartNew();
            var i = 0;
            Console.CursorVisible = false;
            while (condition.Invoke())
            {
                ClearCurrentConsoleLine();
                switch (i % 4)
                {
                    case 0:
                        Console.Write("[\\]");
                        break;
                    case 1:
                        Console.Write("[|]");
                        break;
                    case 2:
                        Console.Write("[/]");
                        break;
                    case 3:
                        Console.Write("[-]");
                        break;
                    default:
                        break;
                }

                Thread.Sleep(200);
                ++i;
            }
            watch.Stop();
            ClearCurrentConsoleLine();
            Console.CursorVisible = true;
            return watch.Elapsed;
        }

        public static TimeSpan ShowSpinnerUntilTaskIsRunning(Task task)
        {
            return ShowSpinnerUntilConditionTrue(() => !task.GetAwaiter().IsCompleted);
        }

        public static TimeSpan ShowSpinnerUntilTaskIsRunning<T>(Task<T> task)
        {
            return ShowSpinnerUntilConditionTrue(() => !task.GetAwaiter().IsCompleted);
        }

        public static void ClearCurrentConsoleLine()
        {
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public static bool GetBinaryDecisionFromUser()
        {
            bool? response = null;
            while (response == null)
            {
                var key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.N:
                        response = false;
                        break;
                    case ConsoleKey.Y:
                        response = true;
                        break;
                    default:
                        Console.WriteLine($"Only key 'Y' or 'N' are acceptable. Provided invalid key \"{key.Key}\"");
                        break;
                }
            }
            return response.Value;
        }

        public static int GetNaturalInt()
        {
            var line = Console.ReadLine();
            int validInteger;
            while (!int.TryParse(line, out validInteger) && !(validInteger > 0))
            {
                Console.WriteLine(
                    $"There was a problem with your input: {line} is not a valid integer in this context. Enter any natural number greater than 0.");
                Console.Write("Number of messages: ");
                line = Console.ReadLine();
            }
            return validInteger;
        }
    }
}

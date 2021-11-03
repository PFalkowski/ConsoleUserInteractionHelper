using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleUserInteractionHelper
{
    public class ConsoleHelper : IConsoleHelper
    {
        public string GetNonEmptyStringFromUser()
        {
            string userInput = Console.ReadLine();
            while (string.IsNullOrEmpty(userInput))
            {
                Console.WriteLine("Input cannot be empty. Please enter non-empty string:");
                userInput = Console.ReadLine();
            }

            return userInput;
        }
        public string GetPathToExistingFileFromUser(string requiredFileExtension = null)
        {
            string userInput = "";
            var inputNullOrWhiteSpace = false;
            var fileExists = false;
            var extensionMatches = false;
            var shown = false;
            do
            {
                if (shown)
                {
                    string prompt = "";
                    if (inputNullOrWhiteSpace)
                    {
                        prompt = "Empty path provided. Path to file cannot be empty.";
                    }
                    else if (!fileExists)
                    {
                        prompt = $"Invalid file path provided. File {userInput} does not exist. Create the file or point to existing one.";
                    }
                    else
                    {
                        prompt = $"Wrong extension. Expected file of type {requiredFileExtension}. Point to the file of type {requiredFileExtension}.";
                    }

                    Console.WriteLine(prompt);
                }
                userInput = Console.ReadLine();
                inputNullOrWhiteSpace = string.IsNullOrWhiteSpace(userInput);
                if (!inputNullOrWhiteSpace)
                {
                    fileExists = File.Exists(userInput);
                    if (requiredFileExtension != null && fileExists)
                        extensionMatches = userInput.EndsWith(requiredFileExtension,
                            StringComparison.InvariantCultureIgnoreCase);
                }
                shown = true;
            } while (inputNullOrWhiteSpace || !fileExists || (requiredFileExtension != null && !extensionMatches));

            return userInput;
        }

        public TimeSpan ShowSpinnerUntilConditionTrue(Func<bool> condition)
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

        public TimeSpan ShowSpinnerUntilTaskIsRunning(Task task)
        {
            return ShowSpinnerUntilConditionTrue(() => !task.GetAwaiter().IsCompleted);
        }

        public TimeSpan ShowSpinnerUntilTaskIsRunning<T>(Task<T> task)
        {
            return ShowSpinnerUntilConditionTrue(() => !task.GetAwaiter().IsCompleted);
        }

        public void ClearCurrentConsoleLine()
        {
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public bool GetBinaryDecisionFromUser()
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

        public int GetNaturalInt()
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

        public DateTime GetDateFromUser()
        {
            var line = Console.ReadLine();
            DateTime result;
            while (!DateTime.TryParse(line, out result))
            {
                Console.WriteLine(
                    $"There was a problem with your input: {line} is not a valid Date in this context.");
                line = Console.ReadLine();
            }
            return result;
        }

        public SecureString GetSecureStringFromUser()
        {
            var result = new SecureString();
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);

                // Ignore any key out of range.
                if (!Char.IsControl(key.KeyChar) && !Char.IsWhiteSpace(key.KeyChar))
                {
                    // Append the character to the password.
                    result.AppendChar(key.KeyChar);
                    Console.Write("*");
                }
                // Exit if Enter key is pressed.
            } while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();

            return result;
        }
    }
}

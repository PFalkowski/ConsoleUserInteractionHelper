using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleUserInteractionHelper
{
    /// <summary>
    /// Provides helper methods for console-based user interactions.
    /// </summary>
    public class ConsoleHelper : IConsoleHelper
    {
        private static readonly char[] SpinnerChars = { '|', '/', '-', '\\' };
        private const int SpinnerDelay = 200;
        
        /// <inheritdoc/>
        public string GetNonEmptyStringFromUser(int? maxRetries = null)
        {
            var attemptCount = 0;
            while (maxRetries == null || attemptCount < maxRetries.Value)
            {
                var userInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(userInput))
                {
                    return userInput;
                }
                Console.WriteLine($"Input cannot be empty. Please enter a non-empty string{GetAttemptMessage(attemptCount, maxRetries)}:");
                attemptCount++;
            }
            throw new InvalidOperationException("Max retries reached. Unable to get non-empty input.");
        }

        /// <inheritdoc/>
        public string GetPathToExistingFileFromUser(string requiredFileExtension = null, int? maxRetries = null)
        {
            var attemptCount = 0;
            while (maxRetries == null || attemptCount < maxRetries.Value)
            {
                var userInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.WriteLine($"Empty path provided. Path to file cannot be empty{GetAttemptMessage(attemptCount, maxRetries)}.");
                    ++attemptCount;
                    continue;
                }

                try
                {
                    if (!Path.IsPathRooted(userInput))
                    {
                        userInput = Path.GetFullPath(userInput);
                    }

                    if (!File.Exists(userInput))
                    {
                        Console.WriteLine($"Invalid file path provided. File {userInput} does not exist{GetAttemptMessage(attemptCount, maxRetries)}.");
                        ++attemptCount;
                        continue;
                    }

                    if (requiredFileExtension != null &&
                        !string.Equals(Path.GetExtension(userInput), requiredFileExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"Wrong extension. Expected file of type {requiredFileExtension}{GetAttemptMessage(attemptCount, maxRetries)}.");
                        attemptCount++;
                        continue;
                    }

                    return userInput;
                }
                catch (Exception ex) when (ex is IOException || ex is SecurityException || ex is UnauthorizedAccessException)
                {
                    Console.WriteLine($"Error accessing the file: {ex.Message}{GetAttemptMessage(attemptCount, maxRetries)}.");
                    attemptCount++;
                }
            }
            throw new InvalidOperationException("Max retries reached. Unable to get valid file path.");
        }

        /// <inheritdoc/>
        public TimeSpan ShowSpinnerUntilConditionTrue(Func<bool> condition, CancellationToken cancellationToken = default)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            var watch = Stopwatch.StartNew();
            var i = 0;
            Console.CursorVisible = false;
            try
            {
                while (!cancellationToken.IsCancellationRequested && condition.Invoke())
                {
                    ClearCurrentConsoleLine();
                    Console.Write($"[{SpinnerChars[i % SpinnerChars.Length]}]");
                    Thread.Sleep(SpinnerDelay);
                    i++;
                }
            }
            finally
            {
                watch.Stop();
                ClearCurrentConsoleLine();
                Console.CursorVisible = true;
            }
            return watch.Elapsed;
        }

        /// <inheritdoc/>
        public TimeSpan ShowSpinnerUntilTaskIsRunning(Task task)
        {
            return ShowSpinnerUntilConditionTrue(() => !task.IsCompleted);
        }

        /// <inheritdoc/>
        public TimeSpan ShowSpinnerUntilTaskIsRunning<T>(Task<T> task)
        {
            return ShowSpinnerUntilConditionTrue(() => !task.IsCompleted);
        }

        /// <inheritdoc/>
        public void ClearCurrentConsoleLine()
        {
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        /// <inheritdoc/>
        public bool GetBinaryDecisionFromUser(ConsoleKey yesKey = ConsoleKey.Y, ConsoleKey noKey = ConsoleKey.N)
        {
            Console.WriteLine($"Press '{yesKey}' for Yes or '{noKey}' for No.");
            while (true)
            {
                var key = Console.ReadKey(true).Key;
                if (key == yesKey)
                    return true;
                if (key == noKey)
                    return false;
                Console.WriteLine($"Invalid key. Please press '{yesKey}' for Yes or '{noKey}' for No.");
            }
        }

        /// <inheritdoc/>
        public int GetIntWithConstraints(Func<int, bool> predicate = null, string errorMessage = null, int? maxRetries = null)
        {
            predicate ??= _ => true;
            errorMessage ??= "Invalid input. Please enter a valid integer.";

            var attemptCount = 0;
            while (maxRetries == null || attemptCount < maxRetries.Value)
            {
                if (int.TryParse(Console.ReadLine(), out var result) && predicate(result))
                {
                    return result;
                }
                Console.WriteLine($"{errorMessage}{GetAttemptMessage(attemptCount, maxRetries)}");
                attemptCount++;
            }
            throw new InvalidOperationException($"Max retries reached. Unable to get valid integer input.");
        }

        /// <inheritdoc/>
        public int GetPositiveInt(int? maxRetries = null)
        {
            return GetIntWithConstraints(
                n => n > 0,
                "Invalid input. Please enter a positive integer greater than 0.",
                maxRetries);
        }

        /// <inheritdoc/>
        public int GetNaturalInt(int? maxRetries = null)
        {
            return GetIntWithConstraints(
                n => n > 0,
                "Invalid input. Please enter a positive integer greater than 0.",
                maxRetries);
        }

        /// <inheritdoc/>
        public int GetNegativeInt(int? maxRetries = null)
        {
            return GetIntWithConstraints(
                n => n < 0,
                "Invalid input. Please enter a negative integer.",
                maxRetries);
        }

        /// <inheritdoc/>
        public int GetIntInRange(int min, int max, int? maxRetries = null)
        {
            return GetIntWithConstraints(
                n => n >= min && n <= max,
                $"Invalid input. Please enter an integer between {min} and {max} (inclusive).",
                maxRetries);
        }

        /// <inheritdoc/>
        public int GetInt(int? maxRetries = null)
        {
            return GetIntWithConstraints(
                _ => true,  // Accept any integer
                $"Invalid input. Please enter an integer in the range [{int.MinValue}, {int.MaxValue}].",
                maxRetries);
        }

        /// <inheritdoc/>
        public DateTime GetDateFromUser(string format = null, int? maxRetries = null)
        {
            var attemptCount = 0;
            while (maxRetries == null || attemptCount < maxRetries.Value)
            {
                var input = Console.ReadLine();
                if (format != null)
                {
                    if (DateTime.TryParseExact(input, format, null, System.Globalization.DateTimeStyles.None, out var result))
                    {
                        return result;
                    }
                }
                else if (DateTime.TryParse(input, out var result))
                {
                    return result;
                }
                Console.WriteLine($"Invalid date format. Please enter a valid date{(format != null ? $" in the format {format}" : "")}{GetAttemptMessage(attemptCount, maxRetries)}.");
                attemptCount++;
            }
            throw new InvalidOperationException("Max retries reached. Unable to get valid date.");
        }

        /// <inheritdoc/>
        public SecureString GetSecureStringFromUser()
        {
            var result = new SecureString();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (result.Length > 0)
                    {
                        result.RemoveAt(result.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    result.AppendChar(key.KeyChar);
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return result;
        }

        /// <inheritdoc/>
        public string GetSecretStringFromUser()
        {
            var result = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (result.Length > 0)
                    {
                        result.Remove(result.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    result.Append(key.KeyChar);
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return result.ToString();
        }

        private string GetAttemptMessage(int attemptCount, int? maxRetries)
        {
            return maxRetries.HasValue ? $" (Attempt {attemptCount + 1}/{maxRetries})" : "";
        }
    }
}
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

        /// <summary>
        /// Gets a non-empty string from the user.
        /// </summary>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>A non-empty string entered by the user.</returns>
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

        /// <summary>
        /// Gets a path to an existing file from the user, optionally with a specific extension.
        /// </summary>
        /// <param name="requiredFileExtension">The required file extension (optional).</param>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>A valid path to an existing file.</returns>
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

        /// <summary>
        /// Shows a spinner until a specified condition is true.
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>The elapsed time.</returns>
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

        /// <summary>
        /// Shows a spinner until the specified task is running.
        /// </summary>
        /// <param name="task">The task to monitor.</param>
        /// <returns>The elapsed time.</returns>
        public TimeSpan ShowSpinnerUntilTaskIsRunning(Task task)
        {
            return ShowSpinnerUntilConditionTrue(() => !task.IsCompleted);
        }

        /// <summary>
        /// Shows a spinner until the specified task is running.
        /// </summary>
        /// <typeparam name="T">The type of the task result.</typeparam>
        /// <param name="task">The task to monitor.</param>
        /// <returns>The elapsed time.</returns>
        public TimeSpan ShowSpinnerUntilTaskIsRunning<T>(Task<T> task)
        {
            return ShowSpinnerUntilConditionTrue(() => !task.IsCompleted);
        }

        /// <summary>
        /// Clears the current console line.
        /// </summary>
        public void ClearCurrentConsoleLine()
        {
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        /// <summary>
        /// Gets a binary decision from the user.
        /// </summary>
        /// <param name="yesKey">The key for 'Yes' (default is 'Y').</param>
        /// <param name="noKey">The key for 'No' (default is 'N').</param>
        /// <returns>True if the user chose 'Yes', false otherwise.</returns>
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

        /// <summary>
        /// Gets an integer from the user within specified constraints.
        /// </summary>
        /// <param name="predicate">A function to validate the input. If null, accepts any integer.</param>
        /// <param name="errorMessage">The error message to display for invalid input. If null, a generic message is used.</param>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>An integer that satisfies the specified predicate.</returns>
        public int GetIntWithConstraints(Func<int, bool> predicate = null, string errorMessage = null, int? maxRetries = null)
        {
            predicate ??= _ => true;
            errorMessage ??= "Invalid input. Please enter a valid integer.";

            int attemptCount = 0;
            while (maxRetries == null || attemptCount < maxRetries.Value)
            {
                if (int.TryParse(Console.ReadLine(), out int result) && predicate(result))
                {
                    return result;
                }
                Console.WriteLine($"{errorMessage}{GetAttemptMessage(attemptCount, maxRetries)}");
                attemptCount++;
            }
            throw new InvalidOperationException($"Max retries reached. Unable to get valid integer input.");
        }

        /// <summary>
        /// Gets a double from the user within specified constraints.
        /// </summary>
        /// <param name="predicate">A function to validate the input. If null, accepts any double.</param>
        /// <param name="errorMessage">The error message to display for invalid input. If null, a generic message is used.</param>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>A double that satisfies the specified predicate.</returns>
        public double GetDoubleWithConstraints(Func<double, bool> predicate = null, string errorMessage = null, int? maxRetries = null)
        {
            predicate ??= _ => true;
            errorMessage ??= "Invalid input. Please enter a valid number.";

            int attemptCount = 0;
            while (maxRetries == null || attemptCount < maxRetries.Value)
            {
                if (double.TryParse(Console.ReadLine(), out double result) && predicate(result))
                {
                    return result;
                }
                Console.WriteLine($"{errorMessage}{GetAttemptMessage(attemptCount, maxRetries)}");
                attemptCount++;
            }
            throw new InvalidOperationException($"Max retries reached. Unable to get valid number input.");
        }

        /// <summary>
        /// Gets a positive integer from the user.
        /// </summary>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>A positive integer entered by the user.</returns>
        public int GetPositiveInt(int? maxRetries = null)
        {
            return GetIntWithConstraints(
                n => n > 0,
                "Invalid input. Please enter a positive integer greater than 0.",
                maxRetries);
        }

        /// <summary>
        /// Gets a positive integer from the user.
        /// </summary>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>A positive integer entered by the user.</returns>
        public int GetNaturalInt(int? maxRetries = null)
        {
            return GetIntWithConstraints(
                n => n > 0,
                "Invalid input. Please enter a positive integer greater than 0.",
                maxRetries);
        }

        /// <summary>
        /// Gets a negative integer from the user.
        /// </summary>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>A negative integer entered by the user.</returns>
        public int GetNegativeInt(int? maxRetries = null)
        {
            return GetIntWithConstraints(
                n => n < 0,
                "Invalid input. Please enter a negative integer.",
                maxRetries);
        }

        /// <summary>
        /// Gets an integer from the user within a specified range.
        /// </summary>
        /// <param name="min">The minimum value of the range (inclusive).</param>
        /// <param name="max">The maximum value of the range (inclusive).</param>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>An integer within the specified range entered by the user.</returns>
        public int GetIntInRange(int min, int max, int? maxRetries = null)
        {
            return GetIntWithConstraints(
                n => n >= min && n <= max,
                $"Invalid input. Please enter an integer between {min} and {max} (inclusive).",
                maxRetries);
        }

        /// <summary>
        /// Gets an integer from the user.
        /// </summary>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>An integer entered by the user.</returns>
        public int GetInt(int? maxRetries = null)
        {
            return GetIntWithConstraints(
                _ => true,  // Accept any integer
                $"Invalid input. Please enter an integer in the range [{int.MinValue}, {int.MaxValue}].",
                maxRetries);
        }

        /// <summary>
        /// Gets a positive double from the user.
        /// </summary>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>A positive double entered by the user.</returns>
        public double GetPositiveDouble(int? maxRetries = null)
        {
            return GetDoubleWithConstraints(
                n => n > 0,
                "Invalid input. Please enter a positive number greater than 0.",
                maxRetries);
        }

        /// <summary>
        /// Gets a negative double from the user.
        /// </summary>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>A negative double entered by the user.</returns>
        public double GetNegativeDouble(int? maxRetries = null)
        {
            return GetDoubleWithConstraints(
                n => n < 0,
                "Invalid input. Please enter a negative number.",
                maxRetries);
        }

        /// <summary>
        /// Gets a double from the user within a specified range.
        /// </summary>
        /// <param name="min">The minimum value of the range (inclusive).</param>
        /// <param name="max">The maximum value of the range (inclusive).</param>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>A double within the specified range entered by the user.</returns>
        public double GetDoubleInRange(double min, double max, int? maxRetries = null)
        {
            return GetDoubleWithConstraints(
                n => n >= min && n <= max,
                $"Invalid input. Please enter a number between {min} and {max} (inclusive).",
                maxRetries);
        }

        /// <summary>
        /// Gets a double from the user.
        /// </summary>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>A double entered by the user.</returns>
        public double GetDouble(int? maxRetries = null)
        {
            return GetDoubleWithConstraints(maxRetries: maxRetries);
        }

        /// <summary>
        /// Gets a date from the user.
        /// </summary>
        /// <param name="format">The date format string (optional).</param>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>A DateTime entered by the user.</returns>
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

        /// <summary>
        /// Gets a secure string from the user.
        /// </summary>
        /// <returns>A SecureString entered by the user.</returns>
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

        /// <summary>
        /// Gets a secret string from the user.
        /// </summary>
        /// <returns>A string entered by the user without displaying it.</returns>
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
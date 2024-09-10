using System;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleUserInteractionHelper
{
    public interface IConsoleHelper
    {
        /// <summary>
        /// Gets a non-empty string from the user.
        /// </summary>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>A non-empty string entered by the user.</returns>
        string GetNonEmptyStringFromUser(int? maxRetries = null);

        /// <summary>
        /// Gets a path to an existing file from the user, optionally with a specific extension.
        /// </summary>
        /// <param name="requiredFileExtension">The required file extension (optional).</param>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>A valid path to an existing file.</returns>
        string GetPathToExistingFileFromUser(string requiredFileExtension = null, int? maxRetries = null);

        /// <summary>
        /// Shows a spinner until a specified condition is true.
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>The elapsed time.</returns>
        TimeSpan ShowSpinnerUntilConditionTrue(Func<bool> condition, CancellationToken cancellationToken = default);

        /// <summary>
        /// Shows a spinner until the specified task is running.
        /// </summary>
        /// <param name="task">The task to monitor.</param>
        /// <returns>The elapsed time.</returns>
        TimeSpan ShowSpinnerUntilTaskIsRunning(Task task);

        /// <summary>
        /// Shows a spinner until the specified task is running.
        /// </summary>
        /// <typeparam name="T">The type of the task result.</typeparam>
        /// <param name="task">The task to monitor.</param>
        /// <returns>The elapsed time.</returns>
        TimeSpan ShowSpinnerUntilTaskIsRunning<T>(Task<T> task);

        /// <summary>
        /// Clears the current console line.
        /// </summary>
        void ClearCurrentConsoleLine();

        /// <summary>
        /// Gets a binary decision from the user.
        /// </summary>
        /// <param name="yesKey">The key for 'Yes' (default is 'Y').</param>
        /// <param name="noKey">The key for 'No' (default is 'N').</param>
        /// <returns>True if the user chose 'Yes', false otherwise.</returns>
        bool GetBinaryDecisionFromUser(ConsoleKey yesKey = ConsoleKey.Y, ConsoleKey noKey = ConsoleKey.N);

        /// <summary>
        /// Gets an integer from the user within specified constraints.
        /// </summary>
        /// <param name="predicate">A function to validate the input. If null, accepts any integer.</param>
        /// <param name="errorMessage">The error message to display for invalid input. If null, a generic message is used.</param>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>An integer that satisfies the specified predicate.</returns>
        int GetIntWithConstraints(Func<int, bool> predicate = null, string errorMessage = null, int? maxRetries = null);
        
        /// <summary>
        /// Gets a positive integer from the user.
        /// </summary>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>A positive integer entered by the user.</returns>
        int GetPositiveInt(int? maxRetries = null);

        /// <summary>
        /// Gets a positive integer from the user.
        /// </summary>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>A positive integer entered by the user.</returns>
        [Obsolete("Obsolete due to ambiguity with 0. Use GetPositiveInt() if 0 shall be excluded, or ")]
        int GetNaturalInt(int? maxRetries = null);

        /// <summary>
        /// Gets a negative integer from the user.
        /// </summary>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>A negative integer entered by the user.</returns>
        int GetNegativeInt(int? maxRetries = null);

        /// <summary>
        /// Gets a non-negative integer from the user, i.e. positive integer or 0.
        /// </summary>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>A negative integer entered by the user.</returns>
        int GetNonNegativeInt(int? maxRetries = null);

        /// <summary>
        /// Gets an integer from the user within a specified range.
        /// </summary>
        /// <param name="min">The minimum value of the range (inclusive).</param>
        /// <param name="max">The maximum value of the range (inclusive).</param>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>An integer within the specified range entered by the user.</returns>
        int GetIntInRange(int min, int max, int? maxRetries = null);

        /// <summary>
        /// Gets an integer from the user.
        /// </summary>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>An integer entered by the user.</returns>
        int GetInt(int? maxRetries = null);

        /// <summary>
        /// Gets a date from the user.
        /// </summary>
        /// <param name="format">The date format string (optional).</param>
        /// <param name="maxRetries">The maximum number of retries allowed. If null, retries indefinitely.</param>
        /// <returns>A DateTime entered by the user.</returns>
        DateTime GetDateFromUser(string format = null, int? maxRetries = null);

        /// <summary>
        /// Gets a secure string from the user.
        /// </summary>
        /// <returns>A SecureString entered by the user.</returns>
        SecureString GetSecureStringFromUser();

        /// <summary>
        /// Gets a secret string from the user.
        /// </summary>
        /// <returns>A string entered by the user without displaying it.</returns>
        string GetSecretStringFromUser();
    }
}
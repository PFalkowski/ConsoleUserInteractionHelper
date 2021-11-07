using System;
using System.Security;
using System.Threading.Tasks;

namespace ConsoleUserInteractionHelper
{
    public interface IConsoleHelper
    {
        string GetNonEmptyStringFromUser();
        string GetPathToExistingFileFromUser(string requiredFileExtension = null);
        TimeSpan ShowSpinnerUntilConditionTrue(Func<bool> condition);
        TimeSpan ShowSpinnerUntilTaskIsRunning(Task task);
        TimeSpan ShowSpinnerUntilTaskIsRunning<T>(Task<T> task);
        void ClearCurrentConsoleLine();
        bool GetBinaryDecisionFromUser();
        int GetNaturalInt();
        DateTime GetDateFromUser();
        SecureString GetSecureStringFromUser();
        string GetSecretStringFromUser();
    }
}
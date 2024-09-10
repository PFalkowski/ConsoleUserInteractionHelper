using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;

namespace ConsoleUserInteractionHelper.TestHarness.PromptTest
{
    class Program
    {
        static void Main(string[] args)
        {

            var helper = new ConsoleHelper();

            Console.WriteLine("GetSecretStringFromUser");
            var secretString = helper.GetSecretStringFromUser();
            Console.WriteLine(secretString);

            Console.WriteLine("\nGetSecureStringFromUser");
            var secureString = helper.GetSecureStringFromUser();
            Console.WriteLine(SecureStringToString(secureString));

            Console.WriteLine("\nGetNaturalInt");
            var integerNatural = helper.GetNaturalInt();
            Console.WriteLine(integerNatural);

            Console.WriteLine("\nGetNonEmptyStringFromUser");
            var nonEmptyString = helper.GetNonEmptyStringFromUser();
            Console.WriteLine(nonEmptyString);

            Console.WriteLine("\nGetPathToExistingFileFromUser");
            var filePath = helper.GetPathToExistingFileFromUser();
            Console.WriteLine(filePath);

            Console.WriteLine("\nShowSpinnerUntilTaskIsRunning");
            var task = Task.Delay(3000);
            var spinnerTime = helper.ShowSpinnerUntilTaskIsRunning(task);
            Console.WriteLine($"Spinner ran for {spinnerTime.TotalSeconds} seconds");

            Console.WriteLine("\nGetBinaryDecisionFromUser");
            var decision = helper.GetBinaryDecisionFromUser();
            Console.WriteLine($"Decision: {decision}");

            Console.WriteLine("\nGetIntWithConstraints (even numbers only)");
            var evenNumber = helper.GetIntWithConstraints(n => n % 2 == 0, "Please enter an even number");
            Console.WriteLine(evenNumber);

            Console.WriteLine("\nGetPositiveInt");
            var positiveInt = helper.GetPositiveInt();
            Console.WriteLine(positiveInt);

            Console.WriteLine("\nGetNegativeInt");
            var negativeInt = helper.GetNegativeInt();
            Console.WriteLine(negativeInt);

            Console.WriteLine("\nGetIntInRange (1-10)");
            var rangeInt = helper.GetIntInRange(1, 10);
            Console.WriteLine(rangeInt);

            Console.WriteLine("\nGetInt");
            var anyInt = helper.GetInt();
            Console.WriteLine(anyInt);

            Console.WriteLine("\nGetDateFromUser");
            var date = helper.GetDateFromUser();
            Console.WriteLine(date);

            Console.WriteLine("\nGetDateFromUser (custom format dd/MM/yyyy)");
            var customFormatDate = helper.GetDateFromUser("dd/MM/yyyy");
            Console.WriteLine(customFormatDate);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        private static string SecureStringToString(SecureString value) {
            IntPtr valuePtr = IntPtr.Zero;
            try {
                valuePtr = Marshal.SecureStringToBSTR(value);
                return Marshal.PtrToStringBSTR(valuePtr);
            } finally {
                Marshal.ZeroFreeBSTR(valuePtr);
            }
        }
    }
}

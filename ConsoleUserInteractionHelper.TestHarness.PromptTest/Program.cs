using System;
using System.Runtime.InteropServices;
using System.Security;

namespace ConsoleUserInteractionHelper.TestHarness.PromptTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("GetSecretStringFromUser");
            var helper = new ConsoleHelper();
            var secretString = helper.GetSecretStringFromUser();
            Console.WriteLine(secretString);

            Console.WriteLine("GetSecureStringFromUser");
            var secureString = helper.GetSecureStringFromUser();
            Console.WriteLine(SecureStringToString(secureString));

            Console.WriteLine("GetSecureStringFromUser");
            var integerNatural = helper.GetNaturalInt();
            Console.WriteLine(integerNatural);
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

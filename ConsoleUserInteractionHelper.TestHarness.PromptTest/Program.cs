using System;
using System.Runtime.InteropServices;
using System.Security;

namespace ConsoleUserInteractionHelper.TestHarness.PromptTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var helper = new ConsoleHelper();
            var secretString = helper.GetSecretStringFromUser();
            Console.WriteLine(secretString);

            var secureString = helper.GetSecureStringFromUser();
            Console.WriteLine(SecureStringToString(secureString));
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

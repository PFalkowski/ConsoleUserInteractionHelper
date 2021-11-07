using Konsole;
using ProgressReporting;
using System.Threading;

namespace ConsoleUserInteractionHelper.TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            const int finalValue = 4096;
            var progressReporter = new ConsoleProgressReporter();
            progressReporter.Restart(finalValue);

            for (int i = 0; i < finalValue; i++)
            {
                Thread.Sleep(1);
                progressReporter.ReportProgress();
            }
        }
    }
}

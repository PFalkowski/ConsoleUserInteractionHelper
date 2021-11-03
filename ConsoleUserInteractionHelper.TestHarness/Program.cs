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
            var progressReporter = new ProgressReporter();
            progressReporter.Restart(finalValue);
            var progressBar = new ProgressBar(finalValue);

            for (int i = 0; i < finalValue; i++)
            {
                Thread.Sleep(1);
                progressReporter.ReportProgress();
                progressBar.Refresh(i, progressReporter.RemainingTimeEstimate.ToString(@"hh\:mm\:ss"));
            }
        }
    }
}

using Konsole;
using ProgressReporting;

namespace ConsoleUserInteractionHelper
{
    public class ConsoleProgressReporter : ProgressReporter
    {
        private ProgressBar _progressBar;

        public override void Start(double targetValue)
        {
            _progressBar = new ProgressBar((int)targetValue);
            base.Start(targetValue);
        }

        public override void Restart(double targetValue)
        {
            _progressBar = new ProgressBar((int)targetValue);
            base.Restart(targetValue);
        }

        public override void ReportProgress(double rawProgressValue)
        {
            _progressBar.Refresh((int)rawProgressValue, $"Remaining {RemainingTimeEstimate.ToString("hh\\:mm\\:ss")}");
            base.ReportProgress(rawProgressValue);
        }
    }
}

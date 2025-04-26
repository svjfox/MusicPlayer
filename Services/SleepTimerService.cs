using System.Timers;

namespace MusicPlayer.Services
{
    public class SleepTimerService
    {
        private System.Timers.Timer _timer; // Fully qualify the Timer type to avoid ambiguity
        private Action _onTimerElapsed;
        private DateTime _endTime;

        public bool IsActive => _timer?.Enabled ?? false;
        public TimeSpan TimeRemaining => IsActive ? _endTime - DateTime.Now : TimeSpan.Zero;

        public void StartTimer(int minutes, Action onElapsed)
        {
            StopTimer();

            _onTimerElapsed = onElapsed;
            _endTime = DateTime.Now.AddMinutes(minutes);

            _timer = new System.Timers.Timer(1000); // Fully qualify the Timer type
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Start();
        }

        public void StopTimer()
        {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now >= _endTime)
            {
                StopTimer();
                _onTimerElapsed?.Invoke();
            }
        }
    }
}
namespace MusicPlayer.Models
{
    public class AppSettings
    {
        public bool DarkTheme { get; set; }
        public double Volume { get; set; } = 0.8;
        public bool ShuffleEnabled { get; set; }
        public bool RepeatEnabled { get; set; }
        public int SleepTimerDuration { get; set; } = 0;
        public string EqualizerPreset { get; set; } = "Default";
        public double PlaybackSpeed { get; set; } = 1.0;
    }
}
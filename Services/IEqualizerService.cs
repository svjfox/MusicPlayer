using Plugin.Maui.Audio;

namespace MusicPlayer.Services
{
    public interface IEqualizerService
    {
        Task InitializeAsync(IAudioPlayer audioPlayer);
        void ApplyPreset(string presetName);
        void SetBandGain(int bandIndex, float gain);
        string[] GetPresets();
        float[] GetBandGains();
    }
}
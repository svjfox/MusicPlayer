using MusicPlayer.Services;
using Plugin.Maui.Audio;

namespace MusicPlayer.Services
{
    public class EqualizerService : IEqualizerService
    {
        private readonly IAudioManager _audioManager;
        private IEqualizer _equalizer;

        public EqualizerService(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        public async Task InitializeAsync(IAudioPlayer audioPlayer)
        {
            if (_equalizer != null) return;

            _equalizer = _audioManager.CreateEqualizer(audioPlayer);

            // Настройка полос эквалайзера
            var bands = _equalizer.GetBands();
            for (int i = 0; i < bands.Length; i++)
            {
                bands[i].SetGain(0); // Сброс к нейтральному значению
            }
        }

        public void ApplyPreset(string presetName)
        {
            if (_equalizer == null) return;

            var bands = _equalizer.GetBands();

            switch (presetName)
            {
                case "Flat":
                    for (int i = 0; i < bands.Length; i++) bands[i].SetGain(0);
                    break;
                case "Pop":
                    bands[0].SetGain(-1); // Низкие частоты
                    bands[1].SetGain(2);
                    bands[2].SetGain(3);
                    bands[3].SetGain(1);
                    bands[4].SetGain(-1); // Высокие частоты
                    break;
                case "Rock":
                    bands[0].SetGain(4);
                    bands[1].SetGain(2);
                    bands[2].SetGain(-2);
                    bands[3].SetGain(1);
                    bands[4].SetGain(3);
                    break;
                case "Classical":
                    bands[0].SetGain(0);
                    bands[1].SetGain(0);
                    bands[2].SetGain(0);
                    bands[3].SetGain(0);
                    bands[4].SetGain(0);
                    break;
            }
        }

        public void SetBandGain(int bandIndex, float gain)
        {
            if (_equalizer == null) return;

            var bands = _equalizer.GetBands();
            if (bandIndex >= 0 && bandIndex < bands.Length)
            {
                bands[bandIndex].SetGain(gain);
            }
        }

        public string[] GetPresets()
        {
            return new[] { "Flat", "Pop", "Rock", "Classical" };
        }

        public float[] GetBandGains()
        {
            if (_equalizer == null) return Array.Empty<float>();

            var bands = _equalizer.GetBands();
            return bands.Select(b => b.GetGain()).ToArray();
        }
    }
}
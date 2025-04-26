using Plugin.Maui.Audio;
using System;
using System.IO;

namespace MusicPlayer.Services
{
    /// <summary>
    /// Реализация интерфейса IAudioManager для управления аудиоплеером и эквалайзером.
    /// </summary>
    public class AudioManager : IAudioManager
    {
        /// <summary>
        /// Создает экземпляр аудиоплеера для воспроизведения аудиопотока.
        /// </summary>
        /// <param name="audioStream">Поток аудиоданных.</param>
        /// <returns>Экземпляр IAudioPlayer.</returns>
        public IAudioPlayer CreatePlayer(Stream audioStream)
        {
            if (audioStream == null)
                throw new ArgumentNullException(nameof(audioStream), "Audio stream cannot be null.");

            try
            {
                // Используем фабричный метод для создания AudioPlayer
                var audioPlayer = AudioPlayerFactory.Create(audioStream);
                return audioPlayer;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating audio player: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Создает экземпляр эквалайзера для управления звуком.
        /// </summary>
        /// <param name="audioPlayer">Экземпляр аудиоплеера.</param>
        /// <returns>Экземпляр IEqualizer.</returns>
        public IEqualizer CreateEqualizer(IAudioPlayer audioPlayer)
        {
            if (audioPlayer == null)
                throw new ArgumentNullException(nameof(audioPlayer), "Audio player cannot be null.");

            try
            {
                return new Equalizer(audioPlayer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating equalizer: {ex.Message}");
                throw;
            }
        }
    }
}


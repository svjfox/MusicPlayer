using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Services
{
    public class AudioManager : IAudioManager
    {
        public IAudioPlayer CreatePlayer(Stream audioStream)
        {
            // Реализация метода
        }

        public IEqualizer CreateEqualizer(IAudioPlayer audioPlayer)
        {
            // Реализация метода
            return new Equalizer(audioPlayer);
        }
    }
}

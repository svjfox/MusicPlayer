using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Services
{
    public interface IAudioManager
    {
        IAudioPlayer CreatePlayer(Stream audioStream);
        IEqualizer CreateEqualizer(IAudioPlayer audioPlayer); // Добавленный метод
    }
}

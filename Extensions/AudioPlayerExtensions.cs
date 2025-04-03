using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MusicPlayer.Extensions
{
    public static class AudioPlayerExtensions
    {
        public static void SetNotificationOptions(
            this IAudioPlayer player,
            string title,
            string subtitle,
            string description,
            string image,
            List<NotificationAction> actions = null)
        {
            // Реализация метода для установки параметров уведомления
        }
    }
}

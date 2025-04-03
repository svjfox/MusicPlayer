using Plugin.Maui.Audio;

namespace MusicPlayer.Extensions
{
    public static class AudioExtensions
    {
        public static void SetNotificationOptions(
            this IAudioPlayer player,
            string title,
            string subtitle,
            string description,
            string image,
            List<NotificationAction> actions = null)
        {
#if ANDROID
            if (player is AndroidAudioPlayer androidPlayer)
            {
                androidPlayer.SetNotificationOptions(
                    title, subtitle, description, image, actions);
            }
#endif
        }
    }
}
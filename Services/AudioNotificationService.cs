using MusicPlayer.Extensions;
using Plugin.Maui.Audio;

namespace MusicPlayer.Services
{
    public static class AudioNotificationService
    {
        public static void UpdateNotification(this IAudioPlayer player,
            string title, string subtitle, string description, string image)
        {
#if ANDROID
            player.SetNotificationOptions(
                title: title,
                subtitle: subtitle,
                description: description,
                image: image,
                actions: new List<Plugin.Maui.Audio.NotificationAction>
                {
                    new(1, "Previous", Android.Resource.Drawable.IcMediaPrevious),
                    new(2, "Play/Pause", Android.Resource.Drawable.IcMediaPause),
                    new(3, "Next", Android.Resource.Drawable.IcMediaNext)
                });
#endif
        }
    }
}
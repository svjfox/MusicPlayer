using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Plugin.Maui.Audio;

[Service]
public class AudioService : Service
{
    private const int NotificationId = 1000;
    private const string ChannelId = "music_player_channel";

    public override IBinder OnBind(Intent intent) => null;

    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        if (intent?.Action == "PLAY")
        {
            // Обработка команды воспроизведения
        }
        else if (intent?.Action == "PAUSE")
        {
            // Обработка команды паузы
        }

        // Создание уведомления для foreground service
        var notification = new NotificationCompat.Builder(this, ChannelId)
            .SetContentTitle("Music Player")
            .SetContentText("Playing music in background")
            .SetSmallIcon(Resource.Drawable.ic_music_note)
            .SetOngoing(true)
            .Build();

        StartForeground(NotificationId, notification);

        return StartCommandResult.Sticky;
    }
}
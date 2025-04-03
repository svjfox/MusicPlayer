using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.Maui.Audio;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override void OnCreate()
    {
        base.OnCreate();

        // Настройка уведомлений для аудио
        AudioManager.Init(this);

        // Создание канала уведомлений
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(
                "music_player_channel",
                "Music Player",
                NotificationImportance.Low);

            var notificationManager = GetSystemService(NotificationService) as NotificationManager;
            notificationManager?.CreateNotificationChannel(channel);
        }
    }
}
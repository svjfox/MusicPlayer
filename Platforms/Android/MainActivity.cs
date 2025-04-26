using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using MusicPlayer.Services;

namespace MusicPlayer
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : MauiAppCompatActivity
    {
        private MediaButtonReceiver _mediaButtonReceiver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Убедитесь, что разрешения предоставлены
            EnsurePermissionsAsync().Wait();

            // Обработка медиа-кнопок
            var audioService = MauiApplication.Current.Services.GetService<IAudioService>();
            _mediaButtonReceiver = new MediaButtonReceiver(audioService);

            var filter = new IntentFilter();
            filter.AddAction(Intent.ActionMediaButton);
            RegisterReceiver(_mediaButtonReceiver, filter);
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Headsethook || keyCode == Keycode.MediaPlayPause)
            {
                var audioService = MauiApplication.Current.Services.GetService<IAudioService>();
                audioService?.TogglePlay();
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_mediaButtonReceiver != null)
            {
                UnregisterReceiver(_mediaButtonReceiver);
                _mediaButtonReceiver = null;
            }
        }

        private async Task EnsurePermissionsAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (status != PermissionStatus.Granted)
            {
                await Permissions.RequestAsync<Permissions.StorageRead>();
            }
        }
    }

    public class MediaButtonReceiver : BroadcastReceiver
    {
        private readonly IAudioService _audioService;

        public MediaButtonReceiver(IAudioService audioService)
        {
            _audioService = audioService;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action != Intent.ActionMediaButton)
                return;

            var keyEvent = (KeyEvent)intent.GetParcelableExtra(Intent.ExtraKeyEvent);
            if (keyEvent?.Action != KeyEventActions.Down)
                return;

            switch (keyEvent.KeyCode)
            {
                case Keycode.MediaPlay:
                    _audioService?.PlayAsync();
                    break;
                case Keycode.MediaPause:
                    _audioService?.PauseAsync();
                    break;
                case Keycode.MediaNext:
                    _audioService?.NextAsync();
                    break;
                case Keycode.MediaPrevious:
                    _audioService?.PreviousAsync();
                    break;
            }
        }
    }
}

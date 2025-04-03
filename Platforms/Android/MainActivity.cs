using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using MusicPlayer.Services;

namespace MusicPlayer
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Обработка медиа-кнопок
            var audioService = MauiApplication.Current.Services.GetService<IAudioService>();
            var receiver = new MediaButtonReceiver(audioService);

            var filter = new IntentFilter();
            filter.AddAction(Intent.ActionMediaButton);
            RegisterReceiver(receiver, filter);
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Headsethook || keyCode == Keycode.MediaPlayPause)
            {
                var audioService = MauiApplication.Current.Services.GetService<IAudioService>();
                audioService.TogglePlay();
                return true;
            }
            return base.OnKeyDown(keyCode, e);
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
                    _audioService.PlayAsync();
                    break;
                case Keycode.MediaPause:
                    _audioService.PauseAsync();
                    break;
                case Keycode.MediaNext:
                    _audioService.NextAsync();
                    break;
                case Keycode.MediaPrevious:
                    _audioService.PreviousAsync();
                    break;
            }
        }
    }
}

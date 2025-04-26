using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Plugin.Maui.Audio;

namespace MusicPlayer.Platforms.Android.Services
{
    [Service]
    public class AudioService : Service
    {
        private const int NotificationId = 1000;
        private const string ChannelId = "music_player_channel";
        private IAudioPlayer _audioPlayer;

        public override void OnCreate()
        {
            base.OnCreate();
            CreateNotificationChannel();
        }

        public override IBinder OnBind(Intent intent) => null;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (intent?.Action == "PLAY")
            {
                PlayAudio();
            }
            else if (intent?.Action == "PAUSE")
            {
                PauseAudio();
            }
            else if (intent?.Action == "STOP")
            {
                StopAudio();
            }

            var notification = new NotificationCompat.Builder(this, ChannelId)
                .SetContentTitle("Music Player")
                .SetContentText("Playing music in background")
                .SetSmallIcon(Resource.Drawable.ic_music_note)
                .SetOngoing(true)
                .Build();

            StartForeground(NotificationId, notification);

            return StartCommandResult.Sticky;
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(ChannelId, "Music Player", NotificationImportance.Low)
                {
                    Description = "Channel for music player notifications"
                };
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
        }

        private void PlayAudio()
        {
            if (_audioPlayer == null)
            {
                var audioManager = Plugin.Maui.Audio.AudioManager.Current;
                var audioStream = Assets.Open("test_audio.mp3");
                _audioPlayer = audioManager.CreatePlayer(audioStream);
            }

            if (!_audioPlayer.IsPlaying)
            {
                _audioPlayer.Play();
            }
        }

        private void PauseAudio()
        {
            if (_audioPlayer != null && _audioPlayer.IsPlaying)
            {
                _audioPlayer.Pause();
            }
        }

        private void StopAudio()
        {
            if (_audioPlayer != null)
            {
                _audioPlayer.Stop();
                _audioPlayer.Dispose();
                _audioPlayer = null;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            StopAudio();
        }
    }
}

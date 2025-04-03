using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntelliJ.Lang.Annotations;
using MusicPlayer.Models;
using MusicPlayer.Services;
using System.Collections.ObjectModel;

namespace MusicPlayer.ViewModels
{
    public partial class PlaylistDetailViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private readonly IAudioService _audioService;

        [ObservableProperty]
        private Playlist _playlist;

        [ObservableProperty]
        private ObservableCollection<Track> _tracks = new();

        public PlaylistDetailViewModel(IDataService dataService, IAudioService audioService)
        {
            _dataService = dataService;
            _audioService = audioService;
        }

        [RelayCommand]
        private async Task LoadTracks()
        {
            if (IsBusy || Playlist == null) return;

            try
            {
                IsBusy = true;
                var tracks = await _dataService.GetTracksForPlaylistAsync(Playlist.Id);
                Tracks = new ObservableCollection<Track>(tracks);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task PlayPlaylist()
        {
            await _audioService.SetPlaylistAsync(Tracks);
            await _audioService.PlayAsync();
        }

        [RelayCommand]
        private async Task ShowOptions(Track track)
        {
            var action = await Shell.Current.DisplayActionSheet(
                track.Title, "Cancel", null,
                "Play Now", "Add to Queue", "Remove from Playlist");

            switch (action)
            {
                case "Play Now":
                    await _audioService.LoadTrackAsync(track);
                    await _audioService.PlayAsync();
                    break;

                case "Add to Queue":
                    // Логика добавления в очередь
                    break;

                case "Remove from Playlist":
                    await _dataService.RemoveTrackFromPlaylistAsync(track.Id, Playlist.Id);
                    await LoadTracks();
                    break;
            }
        }
    }
}
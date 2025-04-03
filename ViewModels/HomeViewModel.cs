using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MusicPlayer.Models;
using MusicPlayer.Services;
using MusicPlayer.Views;
using System.Collections.ObjectModel;

namespace MusicPlayer.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly IAudioService _audioService;
        private readonly IDataService _dataService;

        [ObservableProperty]
        private ObservableCollection<Track> _tracks = new();

        [ObservableProperty]
        private string _searchQuery;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private Track _selectedTrack;

        public HomeViewModel(IAudioService audioService, IDataService dataService)
        {
            _audioService = audioService;
            _dataService = dataService;

            LoadTracksCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        private async Task LoadTracks()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var tracks = await _dataService.GetTracksAsync();
                Tracks = new ObservableCollection<Track>(tracks);

                // Установка плейлиста по умолчанию
                await _audioService.SetPlaylistAsync(tracks);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task PlayTrack(Track track)
        {
            if (track == null) return;

            await _audioService.LoadTrackAsync(track);
            await _audioService.PlayAsync();

            // Навигация на страницу плеера
            await Shell.Current.GoToAsync(nameof(PlayerPage));
        }

        [RelayCommand]
        private async Task AddToPlaylist(Track track)
        {
            if (track == null) return;

            var playlists = await _dataService.GetPlaylistsAsync();
            var result = await Shell.Current.DisplayActionSheet(
                "Add to playlist", "Cancel", null,
                playlists.Select(p => p.Name).ToArray());

            if (result != null && result != "Cancel")
            {
                var selectedPlaylist = playlists.FirstOrDefault(p => p.Name == result);
                if (selectedPlaylist != null)
                {
                    await _dataService.AddTrackToPlaylistAsync(track.Id, selectedPlaylist.Id);
                    await Shell.Current.DisplayAlert("Success", "Track added to playlist", "OK");
                }
            }
        }

        partial void OnSelectedTrackChanged(Track value)
        {
            if (value != null)
            {
                PlayTrackCommand.Execute(value);
                SelectedTrack = null;
            }
        }

        [RelayCommand]
        private async Task SearchTracks()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                LoadTracksCommand.Execute(null);
                return;
            }

            var allTracks = await _dataService.GetTracksAsync();
            var filtered = allTracks.Where(t =>
                t.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                t.Artist.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                t.Album.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

            Tracks = new ObservableCollection<Track>(filtered);
        }
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicPlayer.Models;
using MusicPlayer.Services;
using System.Collections.ObjectModel;

namespace MusicPlayer.ViewModels
{
    public partial class PlaylistsViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private ObservableCollection<Playlist> _playlists = new();

        [ObservableProperty]
        private string _newPlaylistName;

        public PlaylistsViewModel(IDataService dataService)
        {
            _dataService = dataService;
            LoadPlaylistsCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        private async Task LoadPlaylists()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var playlists = await _dataService.GetPlaylistsAsync();
                Playlists = new ObservableCollection<Playlist>(playlists);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CreatePlaylist()
        {
            if (string.IsNullOrWhiteSpace(NewPlaylistName))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter playlist name", "OK");
                return;
            }

            var playlist = new Playlist
            {
                Name = NewPlaylistName,
                CreatedDate = DateTime.Now
            };

            await _dataService.AddPlaylistAsync(playlist);
            Playlists.Add(playlist);
            NewPlaylistName = string.Empty;
        }

        [RelayCommand]
        private async Task ViewPlaylist(Playlist playlist)
        {
            if (playlist == null) return;

            await Shell.Current.GoToAsync($"{nameof(PlaylistDetailPage)}?Id={playlist.Id}");
        }
    }
}
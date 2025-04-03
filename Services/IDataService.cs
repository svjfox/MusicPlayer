using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayer.Models;

namespace MusicPlayer.Services
{
    public interface IDataService
    {
        Task InitializeAsync();
        Task<IEnumerable<Track>> GetTracksAsync();
        Task<IEnumerable<Playlist>> GetPlaylistsAsync();
        Task AddTrackAsync(Track track);
        Task AddPlaylistAsync(Playlist playlist);
        Task AddTrackToPlaylistAsync(int trackId, int playlistId);
        Task RemoveTrackFromPlaylistAsync(int trackId, int playlistId);
        Task<IEnumerable<Track>> GetTracksForPlaylistAsync(int playlistId);
    }
}

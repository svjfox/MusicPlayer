using SQLite;
using MusicPlayer.Models;
using System.Collections.ObjectModel;

namespace MusicPlayer.Services
{
    public class DataService : IDataService
    {
        private SQLiteAsyncConnection _database;

        public async Task InitializeAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(
                Path.Combine(FileSystem.AppDataDirectory, "musicplayer.db"),
                SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);

            await _database.CreateTableAsync<Track>();
            await _database.CreateTableAsync<Playlist>();
            await _database.CreateTableAsync<PlaylistTrack>();
        }

        public async Task<IEnumerable<Track>> GetTracksAsync()
        {
            await InitializeAsync();
            return await _database.Table<Track>().ToListAsync();
        }

        public async Task<IEnumerable<Playlist>> GetPlaylistsAsync()
        {
            await InitializeAsync();
            return await _database.Table<Playlist>().ToListAsync();
        }

        public async Task AddTrackAsync(Track track)
        {
            await InitializeAsync();
            await _database.InsertAsync(track);
        }

        public async Task AddPlaylistAsync(Playlist playlist)
        {
            await InitializeAsync();
            await _database.InsertAsync(playlist);
        }

        public async Task AddTrackToPlaylistAsync(int trackId, int playlistId)
        {
            await InitializeAsync();
            var playlistTrack = new PlaylistTrack { TrackId = trackId, PlaylistId = playlistId };
            await _database.InsertAsync(playlistTrack);
        }

        public async Task RemoveTrackFromPlaylistAsync(int trackId, int playlistId)
        {
            await InitializeAsync();
            var playlistTrack = await _database.Table<PlaylistTrack>()
                .Where(pt => pt.TrackId == trackId && pt.PlaylistId == playlistId)
                .FirstOrDefaultAsync();
            if (playlistTrack != null)
            {
                await _database.DeleteAsync(playlistTrack);
            }
        }

        public async Task<IEnumerable<Track>> GetTracksForPlaylistAsync(int playlistId)
        {
            await InitializeAsync();
            var playlistTracks = await _database.Table<PlaylistTrack>()
                .Where(pt => pt.PlaylistId == playlistId)
                .ToListAsync();
            var trackIds = playlistTracks.Select(pt => pt.TrackId).ToList();
            return await _database.Table<Track>().Where(t => trackIds.Contains(t.Id)).ToListAsync();
        }
    }
}
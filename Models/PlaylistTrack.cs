using SQLite;

namespace MusicPlayer.Models
{
    [Table("PlaylistTracks")]
    public class PlaylistTrack
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int PlaylistId { get; set; }

        [Indexed]
        public int TrackId { get; set; }

        public int Position { get; set; }
    }
}
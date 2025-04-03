using SQLite;

namespace MusicPlayer.Models
{
    [Table("Playlists")]
    public class Playlist
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CoverImage { get; set; }
    }
}
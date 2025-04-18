﻿using SQLite;

namespace MusicPlayer.Models
{
    [Table("Tracks")]
    public class Track
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Path { get; set; }
        public int Duration { get; set; } // in seconds
        public string Artwork { get; set; } // path or URL
        public DateTime AddedDate { get; set; }
        public int PlayCount { get; set; }
    }
}
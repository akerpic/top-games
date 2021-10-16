using System;
using System.ComponentModel.DataAnnotations;

namespace TopGames.Models
{
    public class Game
    {
        [Key]
        public int id { get; set; }
        public int ranking { get; set; }
        public string track_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int? total_review_count { get; set; }
        public long total_install_count { get; set; }
        public string current_version { get; set; }
        public DateTime last_update_date { get; set; }
        public int size_in_mb { get; set; }
        public string author { get; set; }
        public DateTime created_date { get; set; }
        public Game() { }

        public Game(string trackId_, int ranking_)
        {
            track_id = trackId_;
            ranking = ranking_;
        }
    }
}

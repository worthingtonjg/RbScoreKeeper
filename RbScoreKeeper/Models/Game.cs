using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RbScoreKeeper.Models
{
    public class Game
    {
        public Guid GameId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<PlayerScore> Scores { get; set; }

        public Game(List<Player> players)
        {
            GameId = Guid.NewGuid();
            StartTime = DateTime.Now;
            Scores = new List<PlayerScore>();

            foreach (var player in players)
            {
                Scores.Add(new PlayerScore(player.PlayerId));
            }
        }
    }
}

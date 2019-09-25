using RbScoreKeeper.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RbScoreKeeper.Models
{
    public class Stats
    {
        public Guid StatsId { get; set; }
        public string Name { get; set; }
        public List<Stat> PlayerStats { get; set; }

        public Stats(StatsEntity stats, List<Player> players)
        {
            StatsId = new Guid(stats.RowKey);
            Name = stats.Name;
            PlayerStats = new List<Stat>();

            int numGames = stats.Wins1 + stats.Wins2 + stats.Wins3;

            if(stats.PlayerId1 != null)
            {
                PlayerStats.Add(new Stat(stats.PlayerId1, stats.Wins1, numGames, players));
            }
            if (stats.PlayerId2 != null)
            {
                PlayerStats.Add(new Stat(stats.PlayerId2, stats.Wins2, numGames, players));
            }
            if (stats.PlayerId3 != null)
            {
                PlayerStats.Add(new Stat(stats.PlayerId3, stats.Wins3, numGames, players));
            }

            PlayerStats = PlayerStats.OrderBy(p => p.Name).ToList();
        }

    }

    public class Stat
    {
        public Guid PlayerId { get; set; }
        public string Name { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int NumGames { get; set; }
        public double WinPercentage { get; set; }

        public Stat(string playerId, int wins, int numGames, List<Player> players)
        {
            PlayerId = new Guid(playerId);
            Wins = wins;
            NumGames = numGames;
            Losses = numGames - wins;
            WinPercentage = Math.Round(((double)Wins / (double)NumGames) * 100);

            var player = players.FirstOrDefault(p => p.PlayerId == PlayerId);
            Name = player?.Name;
        }

    }
}

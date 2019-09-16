using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RbScoreKeeper.Models
{
    public class PlayerScore
    {
        public Guid ScoreId { get; set; }

        public Guid PlayerId { get; set; }

        public int Score { get; set; }

        public PlayerScore(Guid playerId)
        {
            ScoreId = Guid.NewGuid();
            PlayerId = playerId;
            Score = 0;
        }
    }
}

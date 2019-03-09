using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RbScoreKeeper.Models
{
    public class FlicButtonBinding
    {
        public FlicButtonBinding(string flicId, string playerName, int playerScore)
        {
            FlicId = flicId;
            PlayerName = playerName;
            PlayerScore = playerScore;
        }

        public string FlicId { get; set; }

        public string PlayerName { get; set; }

        public int PlayerScore { get; set; }
    }
}

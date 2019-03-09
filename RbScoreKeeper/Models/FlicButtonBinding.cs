using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RbScoreKeeper.Models
{
    public class FlicButtonBinding
    {
        public FlicButtonBinding(string playerName, int playerScore)
        {
            PlayerName = playerName;
            PlayerScore = playerScore;
        }

        public string PlayerName { get; set; }

        public int PlayerScore { get; set; }
    }
}

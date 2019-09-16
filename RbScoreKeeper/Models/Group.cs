using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RbScoreKeeper.Models
{
    public class Group
    {
        public Guid GroupId { get; set; }

        public List<Player> Players { get; set; }

        public string GroupName
        {
            get
            {
                return string.Join(", ", Players.Select(p => p.Name).ToList());
            }
        }

        public Group(List<Player> players)
        {
            GroupId = Guid.NewGuid();
            Players = players;
        }
    }
}

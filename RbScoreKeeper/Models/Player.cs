using RbScoreKeeper.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RbScoreKeeper.Models
{
    public class Player
    {
        public Guid PlayerId { get; set; }

        public string Name { get; set; }

        public Player(string name)
        {
            PlayerId = Guid.NewGuid();
            Name = name;
        }

        public Player(PlayerEntity player)
        {
            PlayerId = new Guid(player.RowKey);
            Name = player.Name;
        }
    }
}

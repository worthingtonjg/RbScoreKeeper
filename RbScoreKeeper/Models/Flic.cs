using RbScoreKeeper.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RbScoreKeeper.Models
{
    public class Flic
    {
        public Guid FlicId { get; set; }

        public string Name { get; set; }

        public Flic(string name)
        {
            FlicId = Guid.NewGuid();
            Name = name;
        }

        public Flic(FlicEntity flic)
        {
            FlicId = new Guid(flic.RowKey);
            Name = flic.Name;
        }
    }
}

using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RbScoreKeeper.Entities
{
    public class StatsEntity : TableEntity
    {
        public string Name { get; set; }

        public string PlayerId1 { get; set; }

        public string PlayerId2 { get; set; }

        public string PlayerId3 { get; set; }

        public int Wins1 { get; set; }

        public int Wins2 { get; set; }

        public int Wins3 { get; set; }
    }
}

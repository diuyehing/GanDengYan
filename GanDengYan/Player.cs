using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GanDengYan
{
    class Player
    {
        public uint ID { get; set; }
        public string Name { get; set; } 
        public int Score { get; set; }
        public bool Initiative { get; set; }
        public List<int> ScoreRecord { get; set; }
    }
}

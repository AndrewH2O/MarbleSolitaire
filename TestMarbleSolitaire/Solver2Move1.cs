
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMarbleSolitaire
{
    public class Solver2Move1
    {
        public ushort IndexMove { get; set; }
        public ushort ID { get; set; } //direction * 100 + indexGame
        public ushort IndexGame { get; set; }
        public byte Direction { get; set; }

        public ulong PreMove { get; set; }
        public ulong PostMove { get; set; }
        public ulong Mask { get; set; }

        public ulong StorageBitsMaskID { get; set; }
        public ulong StorageBitsValueID { get; set; }
    }
}

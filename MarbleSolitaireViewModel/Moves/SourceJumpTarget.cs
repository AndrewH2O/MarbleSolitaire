using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireViewModel.Moves
{
    public class SourceJumpTarget
    {
        public int Source { get; set; }
        public int Jump { get; set; }
        public int Target { get; set; }

        /// <summary>
        /// Shallow copy - as this class only encapsulates 
        /// related properties and is only a simple Tuple then 
        /// deep copy is not relevant
        /// </summary>
        /// <returns></returns>
        public SourceJumpTarget Copy()
        {
            return (SourceJumpTarget)this.MemberwiseClone();
        }
    }
}

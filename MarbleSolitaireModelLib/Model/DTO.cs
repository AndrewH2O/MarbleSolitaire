using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireModelLib.Model
{
    public abstract class DTO
    {
        public virtual int Target { get; protected set; }
        public virtual int Jump { get; protected set; }
        public virtual int Source { get; protected set; }
    }
}

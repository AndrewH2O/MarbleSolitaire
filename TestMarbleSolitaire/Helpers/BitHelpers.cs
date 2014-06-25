
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMarbleSolitaire.Helpers
{
    public class BitHelpers
    {
        //int MAX_BIT_INDEX = 63;

        void setBit(int index, ref UInt64 bits)
        {
            if (!validate(index)) return;
            bits |= (1ul << index);
        }

        bool validate(int index)
        {
            return (index >= 0 && index < 63) ? true : false;
        }
    }
}

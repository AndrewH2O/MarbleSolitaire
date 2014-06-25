using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireLib.GameSolver
{
    [Flags]
    public enum LoadState { Start = 1, Win = 2, Current = 4 };

    [Flags]
    public enum SaveState { ToText = 1, ToBinary = 2 };
}

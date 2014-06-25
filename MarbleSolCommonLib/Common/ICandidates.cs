using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolCommonLib.Common
{
    public interface ICandidates
    {

        int[] GetListOfJumpedCandidates(int index);
        int[] GetListOfTargetCandidates(int index);
        int[] GetListOfSourceCandidates(int index);
        int CountOfDimensions { get; }
        int TokenIllegalPosition { get; }
        IEnumerable<int> EnumerateNodesByIndex(Predicate<Node4> filter);
    }
}

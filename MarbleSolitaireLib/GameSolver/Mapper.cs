

using MarbleSolCommonLib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireLib.GameSolver
{
    public class Mapper
    {
        readonly int NUMBER_GAME_POSITIONS = 33;
        public readonly int NON_LEGAL = -1;

        Dictionary<int, int> _mapIndexGameToModel; //map from Legal positions to Model

        public Dictionary<int, int> MapIndexGameToModel
        {
            get { return _mapIndexGameToModel; }
            set { _mapIndexGameToModel = value; }
        }
        
        
        Dictionary<int, int> _mapIndexModelToGame; //don't need separate dictionary map but adds to code readability

        public Dictionary<int, int> MapIndexModelToGame
        {
            get { return _mapIndexModelToGame; }
            set { _mapIndexModelToGame = value; }
        }

        public Mapper(ICandidates candidates)
        {
            Initialise(candidates);
        }

        public int GetModelToGameByIndex(int index)
        {
            int result = 0;
            if (_mapIndexModelToGame.TryGetValue(index, out result))
            {
                return result;
            }
            else
            {
                return NON_LEGAL;
            }
            
        }

        private void Initialise(ICandidates candidates)
        {
            _mapIndexGameToModel = new Dictionary<int, int>(NUMBER_GAME_POSITIONS);
            _mapIndexModelToGame = new Dictionary<int, int>();

            //NON_LEGAL = candidates.TokenIllegalPosition;

            int indexInGame = -1; //33 game positions

            foreach (var indexModel in candidates
                .EnumerateNodesByIndex(x => x.Content != NON_LEGAL))
            {
                ++indexInGame;
                _mapIndexGameToModel.Add(indexInGame, indexModel);
                _mapIndexModelToGame.Add(indexModel, indexInGame);
            }
        }

    }
}

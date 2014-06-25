

using MarbleSolCommonLib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireLib.GameSolver
{
    public class MoveController
    {
        readonly int NUMBER_GAME_POSITIONS = 33;
        readonly int NUMBER_MOVES = (4 * 2 + 2 * 1) * 4 + 9 * 4;

        readonly int DIRECTION = 4; //nswe
        int NON_LEGAL;
        
        Mapper _mapper;
        StorageBitPacker _storageBitPacker;

        /// <summary>
        /// stores 76 moves where key is moveId
        /// </summary>
        //Dictionary<int, MoveSolver2> _moves;

        //ushort[][] _moveLookup; //used to lookup moves first index 0-32 gameboard and second moves in MoveID format

        /// <summary>
        /// Used to look up moves, rank 0 is gamePos 0-32 rank 1 is moves in MoveID 
        /// format representing NSWE
        /// </summary>
        public ushort[][] MoveLookup;
        
        /// <summary>
        /// Dictionary of moves where the key is an id is nswe direction*100+gameindex
        /// </summary>
        public Dictionary<int, MoveSolver2> Moves;

        public int[] NumberMovesPerGamePos;

        public MoveController(
            Mapper mapper, //maps game and model indexes
            ICandidates candidates, //provides move data
            StorageBitPacker storageBitPacker //format used for storage
            )
        {
            _storageBitPacker = storageBitPacker;
            _mapper = mapper;
            initialise(candidates);
        }

        public void GetMoveHintByIdPacked(ulong idPacked, int pieceIndex, int direction)
        {
            //idPacked>>NUMBER_GAME_POSITIONS
        }

        private void initialise(ICandidates candidates)
        {
            Moves = new Dictionary<int, MoveSolver2>(NUMBER_MOVES);
            MoveLookup = new ushort[NUMBER_GAME_POSITIONS][];//moves by index

            NON_LEGAL = candidates.TokenIllegalPosition;
            
            

            uint indexMove = 0; //76 moves

            //init moves
            foreach (var indexModel in _mapper.MapIndexModelToGame.Keys)
            {
                buildMoves(
                    candidates.GetListOfSourceCandidates(indexModel),
                    candidates.GetListOfJumpedCandidates(indexModel),
                    candidates.GetListOfTargetCandidates(indexModel),
                    indexModel,
                    ref indexMove
                    );
            }

            //init move lookup by index
            for (int i = 0; i < NUMBER_GAME_POSITIONS; i++)
            {
                MoveLookup[i] = Moves.Values.Where(x => x.IndexGame == i).Select(x => x.ID).ToArray();
            }

            //init number of moves per game position
            NumberMovesPerGamePos = new int[NUMBER_GAME_POSITIONS];
            for (int i = 0; i < NUMBER_GAME_POSITIONS; i++)
            {
                NumberMovesPerGamePos[i] = MoveLookup[i].Length;
            }
        }

        /// <summary>
        /// Build moves info and store as MoveSolver2
        /// Of note:
        /// 1) converts from model indexing to game indexing,
        /// 2) directions are the nswe arrangement of possible moves 
        /// 3) stores 2 types of bit significant information:
        /// The first is used masking into significant bits of a game board and records
        /// pre and post move data for the move. 
        /// The second is for storage where the moveID is stored using 9 bits. Both the mask and 
        /// the ID are converted to 9 significant bit storage format and saved.
        /// </summary>
        /// <param name="source">source model index</param>
        /// <param name="jumped">jumped model index</param>
        /// <param name="target">target model index</param>
        /// <param name="indexModel">index into model 0 - 48</param>
        /// <param name="indexMove">index of move 0 - 75</param>
        private void buildMoves(int[] source, int[] jumped, int[] target, int indexModel, ref uint indexMove)
        {
            ulong preMove, maskMove, postMove;//set 3 of 33 bits 
            ushort id = 0;
            int indexGame = 0;
            int direction = 0;
            for (int nswe = 0; nswe < DIRECTION; nswe++)//nswe 0123
            {
                if (source[nswe] != NON_LEGAL && jumped[nswe] != NON_LEGAL && target[nswe] != NON_LEGAL)
                {
                    preMove = 0; maskMove = 0; postMove = 0;
                    SetPreMove(
                        _mapper.MapIndexModelToGame[source[nswe]],
                        _mapper.MapIndexModelToGame[jumped[nswe]],
                        _mapper.MapIndexModelToGame[target[nswe]],
                        ref preMove);
                    SetMaskMove(
                        _mapper.MapIndexModelToGame[source[nswe]],
                        _mapper.MapIndexModelToGame[jumped[nswe]],
                        _mapper.MapIndexModelToGame[target[nswe]],
                        ref maskMove);
                    SetPostMove(
                        _mapper.MapIndexModelToGame[source[nswe]],
                        _mapper.MapIndexModelToGame[jumped[nswe]],
                        _mapper.MapIndexModelToGame[target[nswe]],
                        ref postMove);

                    //add move
                    indexGame = _mapper.MapIndexModelToGame[indexModel];
                    direction = nswe;
                    id = (ushort)(direction * 100 + indexGame);

                    Moves.Add(id, new MoveSolver2()
                    {
                        IndexMove = (ushort)indexMove,
                        IndexGame = (ushort)indexGame,
                        ID = id,
                        Direction = (byte)nswe,
                        MaskMove = maskMove,
                        PostMove = postMove,
                        PreMove = preMove,
                        StorageBitsMaskID = _storageBitPacker.MoveIDMask,
                        StorageBitsValueID = _storageBitPacker.SetMoveValueID(id)
                    });

                    indexMove++;
                }
            }
        }


        /// <summary>
        /// Post move position so source 0, jumped 0, target 1
        /// </summary>
        /// <param name="source"></param>
        /// <param name="jumped"></param>
        /// <param name="target"></param>
        /// <param name="move">PostMove as a ref ulong</param>
        /// <returns>returns false if source, jumped or target are invalid indexes</returns>
        public bool SetPostMove(int source, int jumped, int target, ref ulong move)
        {
            if (validate(source) && validate(jumped) && validate(target))
            {
                ClearBit(source, ref move);
                ClearBit(jumped, ref move);
                SetBit(target, ref move);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Pre move position so source = 1, jumped=1 and target=0
        /// </summary>
        /// <param name="source"></param>
        /// <param name="jumped"></param>
        /// <param name="target"></param>
        /// <param name="move">PreMove as a ref ulong</param>
        /// <returns>returns false if source, jumped or target are invalid indexes</returns>
        public bool SetPreMove(int source, int jumped, int target, ref ulong move)
        {
            if (validate(source) && validate(jumped) && validate(target))
            {
                SetBit(source, ref move);
                SetBit(jumped, ref move);
                ClearBit(target, ref move);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Mask to obtain significant bits source = 1, jumped=1 and target=1
        /// </summary>
        /// <param name="source"></param>
        /// <param name="jumped"></param>
        /// <param name="target"></param>
        /// <param name="move">Mask as a ref ulong</param>
        /// <returns>returns false if source, jumped or target are invalid indexes</returns>
        public bool SetMaskMove(int source, int jumped, int target, ref ulong move)
        {
            if (validate(source) && validate(jumped) && validate(target))
            {
                SetBit(source, ref move);
                SetBit(jumped, ref move);
                SetBit(target, ref move);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Use Id to lookup packed storageBitsValue
        /// </summary>
        /// <param name="moveId"></param>
        /// <param name="moveIdBits"></param>
        public void GetStorageBitsByMoveId(ushort moveId, ref ulong moveIdBits)
        {
            ///TODO consider swapping this for an array of moves and looping
            moveIdBits = Moves[moveId].StorageBitsValueID;
        }

        /// <summary>
        /// TODO refactor low level bit twiddling
        /// </summary>
        /// <param name="index"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        public bool TestBit(int index, UInt64 board)
        {
            if (!validate(index)) return true;
            return (((board & (1ul << index)) != 0) ? true : false);
        }

        public void SetBit(int index, ref UInt64 bits)
        {
            if (!validate(index)) return;
            bits |= (1ul << index);
        }

        public void ClearBit(int index, ref UInt64 bits)
        {
            if (!validate(index)) return;
            bits &= ~(1ul << index);
        }

        protected bool validate(int index)
        {
            return (index >= 0 && index < NUMBER_GAME_POSITIONS) ? true : false;
            //if (index > MAX_SIZE) return false;
            //return ((index >= 0 && index < _maxValueLegalPositions) ? true : false);
        }
    }


    public class MoveSolver2
    {
        public ushort IndexMove;
        public ushort ID; //direction * 100 + indexGame
        public ushort IndexGame;
        public byte Direction;

        public ulong PreMove; 
        public ulong PostMove; 
        public ulong MaskMove; 

        public ulong StorageBitsMaskID; 
        public ulong StorageBitsValueID;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("im:");
            sb.Append(IndexMove);
            sb.Append(" ID:");
            sb.Append(ID);
            sb.Append(" ig:");
            sb.Append(IndexGame);
            sb.Append(" d:");
            sb.Append(Direction);
            sb.Append(" pre:");
            sb.Append(PreMove);
            sb.Append(" pos:");
            sb.Append(PostMove);
            sb.Append(" m:");
            sb.Append(MaskMove);
            sb.Append(" sbmID:");
            sb.Append(StorageBitsMaskID);
            sb.Append(" sbvID:");
            sb.Append(StorageBitsValueID);
            return sb.ToString();
        }
    }
    
}

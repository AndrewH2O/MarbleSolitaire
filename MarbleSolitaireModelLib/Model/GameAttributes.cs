using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using MarbleSolCommonLib.Common;
using MarbleSolitaireLib.GameSolver;
using CrossCuttingLib.Errors;
using MarbleSolitaireLib.Data;

namespace MarbleSolitaireModelLib.Model
{
    public enum GameShapes
    {
        Square,
        Triangle,
        Diamond
    }

    
    public interface IGameAttributes
    {
        List<Node4> GetBoard();
        GameShapes GameShape { get; set; }
    }

    /// <summary>
    /// Stores game attributes such as start position and board representing 
    /// all the legal positions on the board
    /// </summary>
    public class GameAttributes
    {
        GameShapes _gameShape;

        public bool IsError { get; set; }
        
        IErrorLog _errorLog;
        const int NOLOOKUP = -1;

        /// <summary>
        /// Used to set the area of legal spaces on the board
        /// which is why the centre position is set to 1
        /// </summary>
        List<int> LegalSpacesOnSquare { get; set; }
        
        public int CountItemsPerSide { get; set; }

        public GameAttributes(IErrorLog errorLog)
        {
            if(errorLog!=null)_errorLog = errorLog;
            _gameShape = GameShapes.Square;
            LegalSpacesOnSquare=new List<int>() 
            { 
                0,0,1,1,1,0,0,
                0,0,1,1,1,0,0,
                1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,
                0,0,1,1,1,0,0,
                0,0,1,1,1,0,0
            };
            
            CountItemsPerSide = (int)Math.Sqrt((double)this.LegalSpacesOnSquare.Count);
        }

       
        

        /// <summary>
        /// Start positions
        /// </summary>
        /// <returns></returns>
        public List<int> StartForSquareBoard()
        {
            return new List<int>() 
            { 
                0,0,1,1,1,0,0,
                0,0,1,1,1,0,0,
                1,1,1,1,1,1,1,
                1,1,1,0,1,1,1,
                1,1,1,1,1,1,1,
                0,0,1,1,1,0,0,
                0,0,1,1,1,0,0
            };

            //return new List<int>() //very slow blows up out of memory
            //{ 
            //    0,0,1,1,1,0,0,
            //    0,0,0,1,1,0,0,
            //    1,1,0,1,1,1,1,
            //    1,0,1,0,1,1,1,
            //    1,1,1,1,1,1,1,
            //    0,0,1,1,1,0,0,
            //    0,0,1,1,1,0,0
            //};

            //return new List<int>() //very slow blows up out of memory as above rotated
            //{ 
            //    0,0,1,1,1,0,0,
            //    0,0,1,1,1,0,0,
            //    1,1,1,1,1,1,1,
            //    1,1,1,0,1,0,1,
            //    1,1,1,1,0,1,1,
            //    0,0,1,1,0,0,0,
            //    0,0,1,1,1,0,0
            //};

            //return new List<int>() //no soln?
            //{ 
            //    0,0,1,1,1,0,0,
            //    0,0,1,1,0,0,0,
            //    1,1,1,0,0,0,0,
            //    1,1,1,1,0,0,0,
            //    1,1,1,1,0,0,1,
            //    0,0,1,1,0,0,0,
            //    0,0,1,1,0,0,0
            //};


            //return new List<int>() 
            //{ 
            //    0,0,0,0,1,0,0,
            //    0,0,0,0,1,0,0,
            //    0,0,1,1,1,0,0,
            //    0,0,0,1,1,0,0,
            //    0,1,1,1,1,0,0,
            //    0,0,0,0,0,0,0,
            //    0,0,0,0,0,0,0
            //};
        }

        public List<int> WinningState()
        {
            return new List<int>()
            {   
                0,0,0,0,0,0,0,//6 - (0-2)
                0,0,0,0,0,0,0,//13 - (3-5)
                0,0,0,0,0,0,0,//20 - (6-12)
                0,0,0,1,0,0,0,//27 - (13-19)
                0,0,0,0,0,0,0,//34 - (20-26)
                0,0,0,0,0,0,0,//41 - (27-29)
                0,0,0,0,0,0,0 //48 -  (30-32)
            };
        }

        /// <summary>
        /// Extensibility point as could supply other game shapes
        /// </summary>
        /// <returns></returns>
        public Board GetEmptyBoard()
        {
            switch (_gameShape)
            {
                case GameShapes.Square:
                    return new SquareBoard(LegalSpacesOnSquare,_errorLog);
                case GameShapes.Triangle:
                    break;
                case GameShapes.Diamond:
                    break;
                default:
                    break;
            }

            return null;
        }

        /// <summary>
        /// Extensibility point as could supply other game shapes
        /// </summary>
        /// <returns></returns>
        public Board GetBoardWithStart()
        {
            switch (_gameShape)
            {
                case GameShapes.Square:
                    {
                        SquareBoard b = new SquareBoard(LegalSpacesOnSquare, _errorLog);
                        b.SetupStart(this.StartForSquareBoard());
                        return b;
                    }
                case GameShapes.Triangle:
                    break;
                case GameShapes.Diamond:
                    break;
                default:
                    break;
            }

            return null;
        }


        /// <summary>
        /// Build new solver based on squareBoard and initialise
        /// and cache all available moves 
        /// </summary>
        /// <returns></returns>
        public Solver GetSolverForSquareBoard()
        {
            //solverboard based on bitboard and is used o work with moves and
            //masks as ulong bits
            
            
            //implements depth first search
            
            Solver solver = new Solver(new SolverBoard(LegalSpacesOnSquare));
            //we need a squareboard see below
            ICandidates b = new SquareBoard(LegalSpacesOnSquare, _errorLog);
            ((SquareBoard)b).SetupStart(this.StartForSquareBoard());
            //takes a dependency on squareboard for possible moves
            //uses squareboard to enumerate and cache all moves as ulong bits
            solver.InitialiseMoves(b);
            
            solver.LoadStart(this.StartForSquareBoard());
            solver.SetWinningState(this.WinningState());
            
            
            return solver;
        }

        /// <summary>
        /// Build new solver based on squareBoard and initialise
        /// and cache all available moves 
        /// </summary>
        /// <returns></returns>
        public Solver2 GetSolver2ForSquareBoard()
        {
            //solverboard based on bitboard and is used o work with moves and
            //masks as ulong bits


            //implements depth first search

            
            //we need a squareboard see below
            SquareBoard sb = new SquareBoard(LegalSpacesOnSquare, _errorLog);

            sb.SetupStart(this.StartForSquareBoard());

            Solver2 solver = new Solver2(sb);
            //takes a dependency on squareboard for possible moves
            //uses squareboard to enumerate and cache all moves as ulong bits
            

            solver.LoadStart(this.StartForSquareBoard());
            solver.LoadBoard(this.WinningState(),LoadState.Win);


            return solver;
        }

        /// <summary>
        /// Build new solver based on squareBoard and initialise
        /// and cache all available moves 
        /// </summary>
        /// <returns></returns>
        public Solver2 GetSolver2ForSquareBoardWithData()
        {
            //we need a squareboard see below which identifies the legal spaces
            SquareBoard sb = new SquareBoard(LegalSpacesOnSquare, _errorLog);

            sb.SetupStart(this.StartForSquareBoard());
            
            bool isError = false;
            
            //Load pre calculated solutions for all legal board
            //positions and pieces count. This is preloaded as currently real time
            //calc takes just over 2mins. Use it in the game not find solutions but 
            //to quickly identify if a solution is possible, if not we avoid wasting
            //time searching.
            //If Dat file is not found then the solver
            //enumerates in realtime which is slower.
            EnumSolutionsDTO dto = DataLoader.GetData(out isError);
            Solver2 solver;
            if (!isError)
            {
                solver = new Solver2(sb, dto);
                
            }
            else
            {
                solver = new Solver2(sb, null);
            }

            //load starting board
            solver.LoadStart(this.StartForSquareBoard());
            //load winning board
            solver.LoadBoard(this.WinningState(), LoadState.Win);
            return solver;
            //takes a dependency on squareboard for possible moves
            //uses squareboard to enumerate and cache all moves as ulong bits


            
        }
    }

    
}

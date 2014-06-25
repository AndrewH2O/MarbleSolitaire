using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireLib.GameSolver
{
    public static class MemoManager
    {
        public static List<MemoStore> Store; 
        public static Dictionary<int, MapperIndex> MovesMapper; 

        

        public static void Initialise(List<GameSolver.SolverNode> nodes)
        {
            MemoManager.Store = new List<MemoStore>(MemoGlobals.NumberOfStores);
            MemoManager.MovesMapper = new Dictionary<int, MapperIndex>(MemoGlobals.MaxNumberMoves);
            
            for (int i = 0; i < MemoGlobals.NumberOfStores; i++)
            {
                MemoStore ms = new MemoStore();
                MemoManager.Store.Add(ms);

            }

            int idMoveNumber = 0;
            int numberOfDirections = 4;
            foreach (var item in nodes)
            {
                for (int direction = 0; direction < numberOfDirections; direction++)
                {
                    if (item.Mask[direction] == 0)
                    {
                        continue;
                    }
                    else
                    {
                        MemoManager.MovesMapper.Add(
                            direction * 100 + item.Index, 
                            new MapperIndex()
                            {
                                Id = idMoveNumber++,//0-75
                                Index = item.Index,
                                IndexModel = item.IndexModel
                            });
                    }
                }
            }
        }
        
        public static void ClearAll()
        {
            foreach (var item in MemoManager.Store)
            {
                item.Seen.Clear();
            }
        }

        
        public  static bool HasSeen(StoreSeenItem seenItem)
        {
            foreach (var item in MemoManager.Store)
            {
                if (item.Seen.ContainsKey(seenItem.Board))
                {
                    if ((item.Seen[seenItem.Board])[MovesMapper[seenItem.MoveDirection].Id]==true)
                    {
                        //Board seen and move seen
                        return true;
                    }
                    else
                    {
                        //board seen but not yet move
                        //mark which move has been seen
                        item.Seen[seenItem.Board][MovesMapper[seenItem.MoveDirection].Id] = true;
                        return false;
                    }
                }
            }
                
            foreach (var item in MemoManager.Store)
            {
                if (item.Seen.Count < MemoGlobals.Limit)
                {
                    //add board as a key
                    item.Seen.Add(seenItem.Board, new bool[MemoGlobals.MaxNumberMoves]);
                    //add the move info to the new board's value which is a dictionary of moves
                    item.Seen[seenItem.Board][MovesMapper[seenItem.MoveDirection].Id] = true;
                    return false;
                }
                
            }
            throw new OutOfMemoryException("cannot allocate any more storage");
            
        }
        
    }
    
    public class MemoStore
    {
        public Dictionary<ulong, bool[]> Seen = new Dictionary<ulong, bool[]>(MemoGlobals.Limit);
    }

    

    public static class MemoGlobals
    {
        public static int Limit = 1000000;
        public static int MaxNumberMoves = 76;
        public static int NumberOfStores = 20;
    }

    public struct StoreSeenItem
    {
        public ulong Board { get; set; }
        public int MoveDirection { get; set; }
    }

    public struct MapperIndex
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public int IndexModel { get; set; }
    }
}

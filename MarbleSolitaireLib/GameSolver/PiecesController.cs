using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireLib.GameSolver
{
    public class PiecesController
    {
        StorageBitPacker _sbp = new StorageBitPacker();

        ulong _piecesMask = 0;

        public ulong PiecesMask
        {
            get { return _piecesMask; }
            private set { _piecesMask = value; }
        }
    
        
        ushort _piecesCount = 0;

        public ushort PiecesCount
        {
            get { return _piecesCount; }
            set { _piecesCount = value; }
        }

        ushort[] _pieces;
        

        public ushort[] Pieces
        {
            get { return _pieces; }
        }
        ulong[] _piecesStorage;

        public ulong[] PiecesStorageBit
        {
            get { return _piecesStorage; }
        }

        public PiecesController(ushort numberPieces)
        {
            _piecesCount = numberPieces;
            _pieces = new ushort[numberPieces];
            _piecesStorage = new ulong[numberPieces];
            _piecesMask = _sbp.PiecesCountMask;
            for (ushort i = 0; i < numberPieces; i++)
            {
                _pieces[i] = (ushort)(i + 1) ;

                _piecesStorage[i] = _sbp.SetPiecesCount(_pieces[i]);
            }
        }

        public void Reset()
        {
            _piecesCount = 0;
        }

        public void GetPiecesStorageBitByIndex(int pieceCount, ref ulong piecesStorageBitValue)
        {
            piecesStorageBitValue = _piecesStorage[pieceCount - 1];
        }

    }
}

using System;
namespace MarbleSolitaireModelLib.Model
{
    public interface ISquareBoard:IBoard
    {
        int Side { get; }
    }
}

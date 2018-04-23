using System;
using System.Collections.Generic;

namespace Chessharp.Core.Structures
{
    public class History
    {
        private Move move;
        private string turn;
        private Dictionary<string, int> kings;
        private Dictionary<string, int> castling;
        private int epSquare;
        private string halfMoves;
        private string moveNumber;
        private bool initialized = false;

        public History()
        {
        }

        public string Turn { get => turn; set => turn = value; }
        public Dictionary<string, int> Kings { get => kings; set => kings = value; }
        public Dictionary<string, int> Castling { get => castling; set => castling = value; }
        public int EpSquare { get => epSquare; set => epSquare = value; }
        public string HalfMoves { get => halfMoves; set => halfMoves = value; }
        public string MoveNumber { get => moveNumber; set => moveNumber = value; }
        public Move Move { get => move; set => move = value; }
        public bool Initialized { get => initialized; set => initialized = value; }
    }
}

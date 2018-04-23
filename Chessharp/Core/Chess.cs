using System;
using System.Collections.Generic;
using Chessharp.Core.Structures;

namespace Chessharp.Core
{
    public class Chess : Core
    {
        public Move Move(Move moveParam, Dictionary<string, bool> options)
        {
            Move move = moveParam;
            Move[] moves = GenerateMoves(null);
            Move moveObj = new Move() { };

            /* convert the pretty move object to an ugly move object */
            for (int i = 0, len = moves.Length; i < len; i++)
            {
                Move movesI = moves[i];
                int moveToInt = Convert.ToInt32(movesI.To);
                string moveFromInt = move.From;
                if (moveFromInt == Algebraic(Convert.ToInt32(movesI.From)) && move.To == this.Algebraic(moveToInt) && (!(movesI.Promotion != null || move.Promotion == movesI.Promotion)))
                {
                    moveObj = moves[i];
                    break;
                }
            }
            if (moveObj == null)
            {
                return moveObj;
            }

            Move prettyMove = this.MakePretty(moveObj);
            MakeMove(moveObj);
            return prettyMove;
        }

        public Move Move(string moveParam, Dictionary<string, bool> options)
        {
            /* The move function can be called with in the following parameters:
             *
             * .move('Nxb7')      <- where 'move' is a case-sensitive SAN string
             *
             * .move({ from: 'h7', <- where the 'move' is a move object (additional
             *         to :'h8',      fields are ignored)
             *         promotion: 'q',
             *      })
             */

            // allow the user to specify the sloppy move parser to work around over
            // disambiguation bugs in Fritz and Chessbase
            bool sloppy = (options != null && options.ContainsKey("sloppy")) ? options["sloppy"] : false;

            Move moveObj = new Move() {};

            moveObj = MoveFromSan(moveParam, sloppy);
            Console.WriteLine("move_obj : " + moveObj.ToString());

            /* failed to find move */
            if (moveObj == null)
            {
                return moveObj;
            }

            Move prettyMove = this.MakePretty(moveObj);
            MakeMove(moveObj);
            return prettyMove;
        }

        public Chess(string fen) : base(fen)
        {
            Console.WriteLine("Chess(string) - construct \n");
        }

        public Chess() : base()
        {
            Console.WriteLine("Chess - construct \n");
        }
    }
}

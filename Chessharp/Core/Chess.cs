using System;
using System.Collections.Generic;

namespace Chessharp.Core
{
    public class Chess : Core
    {
        public Dictionary<string, string> Move(Dictionary<string, string> moveParam, Dictionary<string, bool> options)
        {
            Dictionary<string, string> move = moveParam;
            Dictionary<string, string>[] moves = GenerateMoves(null);
            Dictionary<string, string> moveObj = new Dictionary<string, string>() { };

            /* convert the pretty move object to an ugly move object */
            for (int i = 0, len = moves.Length; i < len; i++)
            {
                Dictionary<string, string> movesI = moves[i];
                int moveToInt = Convert.ToInt32(movesI["to"]);
                string moveFromInt = move["from"];
                if (moveFromInt == Algebraic(Convert.ToInt32(movesI["from"])) && move["to"] == this.Algebraic(moveToInt) && (!(movesI.ContainsKey("promotion")) || move["promotion"] == movesI["promotion"]))
                {
                    moveObj = moves[i];
                    break;
                }
            }
            if (moveObj == null)
            {
                return moveObj;
            }

            Dictionary<string, string> prettyMove = this.MakePretty(moveObj);
            MakeMove(moveObj);
            return prettyMove;
        }

        public Dictionary<string, string> Move(string moveParam, Dictionary<string, bool> options)
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

            Dictionary<string, string> moveObj = new Dictionary<string, string>() {};

            Console.WriteLine("move" + moveParam);
            moveObj = MoveFromSan(moveParam, sloppy);
            Console.WriteLine("move" + moveObj.ToString());

            /* failed to find move */
            if (moveObj == null)
            {
                return moveObj;
            }

            Dictionary<string, string> prettyMove = this.MakePretty(moveObj);
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

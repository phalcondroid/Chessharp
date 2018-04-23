﻿using System;
using System.Collections.Generic;
using Chessharp.Core.Structures;

namespace Chessharp.Core
{
    public class Chess : Core
    {
        public List<string> CustomSquares { get;  set; }

        public Chess(string fen) : base(fen)
        {
            Console.WriteLine("Chess(string) - construct \n");
            FetchSquares();
        }

        public Chess() : base()
        {
            Console.WriteLine("Chess - construct \n");
            FetchSquares();
        }

        void FetchSquares()
        {
            CustomSquares = new List<string>();
            for (int i = SQUARES["a8"]; i <= SQUARES["h1"]; i++) {
                if ((i & 0x88) != 0) {
                    i += 7;
                    continue;
                }
                CustomSquares.Add(Algebraic(i));
            }
        }

        public List<string> GetSquares()
        {
            return CustomSquares;
        }

        public Move Move(Move moveParam, Dictionary<string, bool> options)
        {
            Move move = moveParam;
            List<Move> moves = GenerateMoves(null);
            Move moveObj = new Move() { };

            /* convert the pretty move object to an ugly move object */
            for (int i = 0, len = moves.Count; i < len; i++)
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
            bool sloppy = (options != null && options.ContainsKey("sloppy")) ? options["sloppy"] : false;

            Move moveObj = new Move() {};
            Console.WriteLine("MOVE PARAM {0}", moveParam);
            moveObj = MoveFromSan(moveParam, sloppy);
            Console.WriteLine("move_obj : " + moveObj.ToString());

            /* failed to find move */
            if (moveObj == null)
            {
                return moveObj;
            }

            Move prettyMove = MakePretty(moveObj);
            MakeMove(moveObj);
            return prettyMove;
        }

        public Move Move(string moveParam)
        {
            bool sloppy = false;

            Move moveObj = new Move() { };
            Console.WriteLine("MOVE PARAM {0}", moveParam);
            moveObj = MoveFromSan(moveParam, sloppy);
            Console.WriteLine("move_obj : " + moveObj.ToString());

            /* failed to find move */
            if (moveObj == null)
            {
                return moveObj;
            }

            Move prettyMove = MakePretty(moveObj);
            MakeMove(moveObj);
            return prettyMove;
        }

        public List<string> Moves()
        {
            List<Move> uglyMoves = GenerateMoves(null);
            List<string> moves = new List<string>();

            for (int i = 0, len = uglyMoves.Count; i < len; i++) {
                //moves.push(move_to_san(ugly_moves[i], false));
                moves.Add(MoveToSan(new Move(), false));
            }
            return moves;
        }

        public List<Move> Moves(Dictionary<string, bool> options)
        {
            List<Move> uglyMoves = GenerateMoves(options);
            List<Move> moves = new List<Move>();

            for (int i = 0, len = uglyMoves.Count; i < len; i++)
            {
                //moves.push(make_pretty(uglyMoves[i]));
                moves.Add(MakePretty(uglyMoves[i]));
            }
            return moves;
        }

        public bool InDraw()
        {
            int halfMoves = Convert.ToInt32(HALFMOVES);
            return halfMoves >= 100 || InStalemate() || InsufficientMaterial() || InThreefoldRepetition();
        }

        public string GetFen()
        {
            return GenerateFen();
        }

        public List<List<Dictionary<string, string>>> GetBoard()
        {
            List<Dictionary<string, string>> row = new List<Dictionary<string, string>>();
            List<List<Dictionary<string, string>>> output = new List<List<Dictionary<string, string>>>();

            for (int i = SQUARES["a8"]; i <= SQUARES["h1"]; i++) {
                if (BOARD[i] == null) {
                    row.Add(null);
                } else {
                    Dictionary<string, string> item = new Dictionary<string, string>() {
                        { "type", BOARD[i]["type"] },
                        { "color", BOARD[i]["color"] }
                    };
                    row.Add(item);
                }

                if (((i + 1) & 0x88) != 0) {
                    output.Add(row);
                    row = new List<Dictionary<string, string>>();
                    i += 8;
                }
            }
            return output;
        }

        public string GetAscii()
        {
            return Ascii();
        }

        public string GetTurn()
        {
            return TURN;
        }

        public Move Undo()
        {
            Move move = UndoMove();
            return move != null ? MakePretty(move) : null;
        }

        public string GetSquareColor(string sq)
        {
            if (SQUARES.ContainsKey(sq)) {
                int Sq0x88 = SQUARES[sq];
                return ((Rank(Sq0x88) + File(Sq0x88)) % 2 == 0) ? "light" : "dark";
            }
            return null;
        }

        public List<string> GetHistory()
        {
            List<Move> reversedHistory = new List<Move>();
            List<string> moveHistory = new List<string>();

            while (HISTORY.Count > 0) {
                reversedHistory.Add(UndoMove());
            }

            while (reversedHistory.Count > 0) {
                Move move = reversedHistory[reversedHistory.Count - 1];
                moveHistory.Add(MoveToSan(move, false));
                MakeMove(move);
            }

            return moveHistory;
        }

        public List<Move> GetHistory(Dictionary<string, bool> options)
        {
            List<Move> reversedHistory = new List<Move>();
            List<Move> moveHistory = new List<Move>();
            bool verbose = (options != null && options.ContainsKey("verbose")) ? options["verbose"] : false;

            while (HISTORY.Count > 0)
            {
                reversedHistory.Add(UndoMove());
            }

            while (reversedHistory.Count > 0)
            {
                Move move = reversedHistory[reversedHistory.Count - 1];
                if (verbose) {
                    moveHistory.Add(MakePretty(move));
                }
            }

            return moveHistory;
        }
    }
}

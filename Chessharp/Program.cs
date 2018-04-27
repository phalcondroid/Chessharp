using System;
using System.Collections.Generic;
using Chessharp.Core;
using Chessharp.Core.Structures;

namespace Chessharp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Welcome to Chessharp");
            try {
                // 4r3/8/2p2PPk/1p6/pP2p1R1/P1B5/2P2K2/3r4 w - - 1 45
                Chess chess = new Chess();//Chess("4r3/8/2p2PPk/1p6/pP2p1R1/P1B5/2P2K2/3r4 w - - 1 45");//"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

                //bool result = chess.Load("4r3/8/2p2PPk/1p6/pP2p1R1/P1B5/2P2K2/3r4 w - - 1 45");

                //Console.WriteLine(
                //    "Load : {0}",
                //    result.ToString()
                //);

                Console.Write(chess.GetAscii());

                Dictionary<string, string> p = new Dictionary<string, string>() {
                    { "square", "e2" }
                };
                List<Move> legalMoves = chess.Moves(p);
                for (int i = 0; i < legalMoves.Count; i++)
                {
                    Console.Write("Color " + legalMoves[i].Color + " ");
                    Console.Write("From " + legalMoves[i].From + " ");
                    Console.Write("To " + legalMoves[i].To + " ");
                    Console.Write("Flags " + legalMoves[i].Flags + " ");
                    Console.Write("Piece " + legalMoves[i].Piece + " ");
                    Console.Write("San " + legalMoves[i].San + " ");
                    Console.WriteLine("");
                }

                /*
                Dictionary<string, string> resFen1 = chess.ValidateFen("2n1r3/p1k2pp1/B1p3b1/P7/5bP1/2N1B3/1P2KP2/2R5 b - - 4 25");

                Console.WriteLine(
                    "Valid {0} ErrorNumber {1} Errors {2}",
                    resFen1["valid"],
                    resFen1["error_number"],
                    resFen1["errors"]
                );

                Dictionary<string, string> resFen2 = chess.ValidateFen("4r3/8/X12XPk/1p6/pP2p1R1/P1B5/2P2K2/3r4 w - - 1 45");

                Console.WriteLine(
                    "Valid {0} ErrorNumber {1} Errors {2}",
                    resFen2["valid"],
                    resFen2["error_number"],
                    resFen2["errors"]
                );

                Move moveToE4 = new Move();
                moveToE4.From = "e2";
                moveToE4.To = "e4";

                Move moveE4 = chess.Move(moveToE4);

                Console.WriteLine("color {0}", moveE4.Color);
                Console.WriteLine("from {0}", moveE4.From);
                Console.WriteLine("to {0}", moveE4.To);
                Console.WriteLine("flags {0}", moveE4.Flags);
                Console.WriteLine("piece {0}", moveE4.Piece);

                Console.Write(chess.GetAscii());

                Console.WriteLine(chess.GetFen());

                Move moveE4 = chess.Move("e4");

                Console.WriteLine("color {0}", moveE4.Color);
                Console.WriteLine("from {0}",  moveE4.From);
                Console.WriteLine("to {0}",    moveE4.To);
                Console.WriteLine("flags {0}", moveE4.Flags);
                Console.WriteLine("piece {0}", moveE4.Piece);

                Move moveByMe = new Move();
                moveByMe.From = "g2";
                moveByMe.To = "g3";

                Move moveNf6 = chess.Move("Nf6");
                Console.WriteLine(moveNf6);

                Move g2Move = chess.Move(moveByMe);
                Console.WriteLine("color {0}", g2Move.Color);
                Console.WriteLine("from {0}", g2Move.From);
                Console.WriteLine("to {0}", g2Move.To);
                Console.WriteLine("flags {0}", g2Move.Flags);
                Console.WriteLine("piece {0}", g2Move.Piece);

                Console.WriteLine(
                    "In draw {0}",
                    chess.InDraw()
                );

                Console.Write(chess.GetAscii());

                Console.WriteLine(chess.GetFen());
                */

                /*Move moveByMe = new Move();
                moveByMe.From = "g2";
                moveByMe.To = "g3";
                Move g2Move = chess.Move(moveByMe);


                List<string> legalMoves = chess.Moves();

                for (int i = 0; i < legalMoves.Count; i++) {
                    Console.WriteLine(legalMoves[i]);
                }

                Console.Write(chess.GetAscii());

                Console.WriteLine(chess.GetSquareColor("h1"));

                Console.WriteLine(chess.GetSquareColor("a7"));

                Console.WriteLine(chess.GetSquareColor("bogus square"));

                Dictionary<string, string> resFen1 = chess.ValidateFen("2n1r3/p1k2pp1/B1p3b1/P7/5bP1/2N1B3/1P2KP2/2R5 b - - 4 25");

                Console.WriteLine(
                    "Valid {0} ErrorNumber {1} Errors {2}",
                    resFen1["valid"],
                    resFen1["error_number"],
                    resFen1["errors"]
                );

                Dictionary<string, string> resFen2 = chess.ValidateFen("4r3/8/X12XPk/1p6/pP2p1R1/P1B5/2P2K2/3r4 w - - 1 45");

                Console.WriteLine(
                    "Valid {0} ErrorNumber {1} Errors {2}",
                    resFen2["valid"],
                    resFen2["error_number"],
                    resFen2["errors"]
                );

                */

            } catch (Exception e) {
                Console.WriteLine("Excetion: " + e.Message);
                Console.WriteLine("Trace: " + e.StackTrace);
            }
        }
    }
}

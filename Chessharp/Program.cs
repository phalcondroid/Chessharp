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
            Console.WriteLine("Welcome to Chessharp");
            try {
                Chess chess = new Chess();
                Move move = chess.Move("e4");

                Console.WriteLine("color {0}", move.Color);
                Console.WriteLine("from {0}",  move.From);
                Console.WriteLine("to {0}",    move.To);
                Console.WriteLine("flags {0}", move.Flags);
                Console.WriteLine("piece {0}", move.Piece);

            } catch (Exception e) {
                Console.WriteLine("Excetion: " + e.Message);
                Console.WriteLine("Trace: " + e.StackTrace);
            }
        }
    }
}

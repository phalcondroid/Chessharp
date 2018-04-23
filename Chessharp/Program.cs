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
                Move move = chess.Move("e4", null);

                Console.WriteLine(move.From);
            } catch (Exception e) {
                Console.WriteLine("Excetion: " + e.Message);
                Console.WriteLine("Trace: " + e.StackTrace);
            }
        }
    }
}

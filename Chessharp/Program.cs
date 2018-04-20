using System;
using System.Collections.Generic;
using Chessharp.Core;

namespace Chessharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Chessharp");
            try {
                Chess chess = new Chess();
                Dictionary<string, string> move = chess.Move("e4", null);

                foreach (KeyValuePair<string, string> entry in move)
                {
                    Console.WriteLine("Key" + entry.Key + " Value : " + entry.Value + "\n");
                }
            } catch (Exception e) {
                Console.WriteLine("Excetion: " + e.Message);
                Console.WriteLine("Trace: " + e.StackTrace);
            }
        }
    }
}

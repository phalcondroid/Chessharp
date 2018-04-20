using System;
using System.Collections.Generic;

namespace Chessharp.Core.Structures
{
    /**
     *  var ROOKS = {
     *      w: [
     *          {square: SQUARES.a1, flag: BITS.QSIDE_CASTLE},
     *          {square: SQUARES.h1, flag: BITS.KSIDE_CASTLE}
     *      ],
     *      b: [
     *          {square: SQUARES.a8, flag: BITS.QSIDE_CASTLE},
     *          {square: SQUARES.h8, flag: BITS.KSIDE_CASTLE}
     *      ]
     *  };
     */
    public class RookStruct
    {
        
        CValue[] w;
        CValue[] b;

        public RookStruct(Dictionary<string, int> squares, Dictionary<string, int> bits)
        {
            W = new CValue[2];

            //throw new Exception(" -> " + W.ToString());

            W[0] = new CValue(
                new Dictionary<string, int>() {
                    { "squares", squares["a1"] },
                    { "flag", bits["QSIDE_CASTLE"] }
                }
            );

            W[1] = new CValue(
                new Dictionary<string, int>() {
                    { "squares", squares["h1"] },
                    { "flag", bits["KSIDE_CASTLE"] }
                }
            );

            B = new CValue[2];

            B[0] = new CValue(
               new Dictionary<string, int>() {
                    { "squares", squares["a8"] },
                    { "flag", bits["QSIDE_CASTLE"] }
               }
            );
            B[1] = new CValue(
                new Dictionary<string, int>() {
                    { "squares", squares["h8"] },
                    { "flag", bits["KSIDE_CASTLE"] }
                }
            );
        }

        public CValue[] W { get => w; set => w = value; }
        public CValue[] B { get => b; set => b = value; }

        public CValue[] GetByColor(string color)
        {
            if (color.Equals("w"))
            {
                return W;
            }
            return B;
        }
    }
}

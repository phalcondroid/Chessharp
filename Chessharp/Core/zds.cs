﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Chessharp.Core
{
    public class Chess
    {
        string fen;

        string BLACK = "b";
        string WHITE = "w";

        int EMPTY = -1;

        string PAWN = "p";
        string KNIGHT = "n";
        string BISHOP = "b";
        string ROOK = "r";
        string QUEEN = "q";
        string KING = "k";

        string SYMBOLS = "pnbrqkPNBRQK";
        string DEFAULT_POSITION = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        string[] POSSIBLE_RESULTS = new string[] { "1-0", "0-1", "1/2-1/2", "*" };


        /*
         *  var PAWN_OFFSETS = {
         *   b: [16, 32, 17, 15],
         *   w: [-16, -32, -17, -15]
         *  };
         */
        Dictionary<string, int[]> PAWN_OFFSETS = new Dictionary<string, int[]>();

        /*
         * var PIECE_OFFSETS = {
         *  n: [-18, -33, -31, -14,  18, 33, 31,  14],
         *  b: [-17, -15,  17,  15],
         *  r: [-16,   1,  16,  -1],
         *  q: [-17, -16, -15,   1,  17, 16, 15,  -1],
         *  k: [-17, -16, -15,   1,  17, 16, 15,  -1]
         * };
         */
        Dictionary<string, int[]> PIECE_OFFSETS = new Dictionary<string, int[]>();


        int[] ATTACKS = new int[] {
            20, 0, 0, 0, 0, 0, 0, 24,  0, 0, 0, 0, 0, 0,20, 0,
            0,20, 0, 0, 0, 0, 0, 24,  0, 0, 0, 0, 0,20, 0, 0,
            0, 0,20, 0, 0, 0, 0, 24,  0, 0, 0, 0,20, 0, 0, 0,
            0, 0, 0,20, 0, 0, 0, 24,  0, 0, 0,20, 0, 0, 0, 0,
            0, 0, 0, 0,20, 0, 0, 24,  0, 0,20, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0,20, 2, 24,  2,20, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 2,53, 56, 53, 2, 0, 0, 0, 0, 0, 0,
            24,24,24,24,24,24,56,  0, 56,24,24,24,24,24,24, 0,
            0, 0, 0, 0, 0, 2,53, 56, 53, 2, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0,20, 2, 24,  2,20, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0,20, 0, 0, 24,  0, 0,20, 0, 0, 0, 0, 0,
            0, 0, 0,20, 0, 0, 0, 24,  0, 0, 0,20, 0, 0, 0, 0,
            0, 0,20, 0, 0, 0, 0, 24,  0, 0, 0, 0,20, 0, 0, 0,
            0,20, 0, 0, 0, 0, 0, 24,  0, 0, 0, 0, 0,20, 0, 0,
            20, 0, 0, 0, 0, 0, 0, 24,  0, 0, 0, 0, 0, 0,20
        };

        int[] RAYS = new int[] {
            17,  0,  0,  0,  0,  0,  0, 16,  0,  0,  0,  0,  0,  0, 15, 0,
            0, 17,  0,  0,  0,  0,  0, 16,  0,  0,  0,  0,  0, 15,  0, 0,
            0,  0, 17,  0,  0,  0,  0, 16,  0,  0,  0,  0, 15,  0,  0, 0,
            0,  0,  0, 17,  0,  0,  0, 16,  0,  0,  0, 15,  0,  0,  0, 0,
            0,  0,  0,  0, 17,  0,  0, 16,  0,  0, 15,  0,  0,  0,  0, 0,
            0,  0,  0,  0,  0, 17,  0, 16,  0, 15,  0,  0,  0,  0,  0, 0,
            0,  0,  0,  0,  0,  0, 17, 16, 15,  0,  0,  0,  0,  0,  0, 0,
            1,  1,  1,  1,  1,  1,  1,  0, -1, -1,  -1,-1, -1, -1, -1, 0,
            0,  0,  0,  0,  0,  0,-15,-16,-17,  0,  0,  0,  0,  0,  0, 0,
            0,  0,  0,  0,  0,-15,  0,-16,  0,-17,  0,  0,  0,  0,  0, 0,
            0,  0,  0,  0,-15,  0,  0,-16,  0,  0,-17,  0,  0,  0,  0, 0,
            0,  0,  0,-15,  0,  0,  0,-16,  0,  0,  0,-17,  0,  0,  0, 0,
            0,  0,-15,  0,  0,  0,  0,-16,  0,  0,  0,  0,-17,  0,  0, 0,
            0,-15,  0,  0,  0,  0,  0,-16,  0,  0,  0,  0,  0,-17,  0, 0,
            -15,  0,  0,  0,  0,  0,  0,-16,  0,  0,  0,  0,  0,  0,-17
        };

        /**
         * { p: 0, n: 1, b: 2, r: 3, q: 4, k: 5 };
         */
        Dictionary<string, int> SHIFTS = new Dictionary<string, int>();

        /*
         * var FLAGS = {
         *  NORMAL: 'n',
         *  CAPTURE: 'c',
         *  BIG_PAWN: 'b',
         *  EP_CAPTURE: 'e',
         *  PROMOTION: 'p',
         *  KSIDE_CASTLE: 'k',
         *  QSIDE_CASTLE: 'q'
         *};
         */
        Dictionary<string, string> FLAGS = new Dictionary<string, string>();

        /*
         * var BITS = {
                NORMAL: 1,
                CAPTURE: 2,
                BIG_PAWN: 4,
                EP_CAPTURE: 8,
                PROMOTION: 16,
                KSIDE_CASTLE: 32,
                QSIDE_CASTLE: 64
         * };
         */
        Dictionary<string, int> BITS = new Dictionary<string, int>();

        int RANK_1 = 7;
        int RANK_2 = 6;
        int RANK_3 = 5;
        int RANK_4 = 4;
        int RANK_5 = 3;
        int RANK_6 = 2;
        int RANK_7 = 1;
        int RANK_8 = 0;

        /*
        var SQUARES = {
            a8:   0, b8:   1, c8:   2, d8:   3, e8:   4, f8:   5, g8:   6, h8:   7,
            a7:  16, b7:  17, c7:  18, d7:  19, e7:  20, f7:  21, g7:  22, h7:  23,
            a6:  32, b6:  33, c6:  34, d6:  35, e6:  36, f6:  37, g6:  38, h6:  39,
            a5:  48, b5:  49, c5:  50, d5:  51, e5:  52, f5:  53, g5:  54, h5:  55,
            a4:  64, b4:  65, c4:  66, d4:  67, e4:  68, f4:  69, g4:  70, h4:  71,
            a3:  80, b3:  81, c3:  82, d3:  83, e3:  84, f3:  85, g3:  86, h3:  87,
            a2:  96, b2:  97, c2:  98, d2:  99, e2: 100, f2: 101, g2: 102, h2: 103,
            a1: 112, b1: 113, c1: 114, d1: 115, e1: 116, f1: 117, g1: 118, h1: 119
        };
        */
        Dictionary<string, int> SQUARES = new Dictionary<string, int>();

        /*
        var ROOKS = {
            w: [
                {square: SQUARES.a1, flag: BITS.QSIDE_CASTLE},
                {square: SQUARES.h1, flag: BITS.KSIDE_CASTLE}
            ],
            b: [
                {square: SQUARES.a8, flag: BITS.QSIDE_CASTLE},
                {square: SQUARES.h8, flag: BITS.KSIDE_CASTLE}
            ]
        };
        */
        Dictionary<string, Dictionary<string, int>[]> ROOKS = new Dictionary<string, Dictionary<string, int>[]>();

        /**
         * var board = new Array(128);
         */
        Dictionary<string, string>[] board = new Dictionary<string,string>[128];

        /**
         * var kings = {w: EMPTY, b: EMPTY};
         */
        Dictionary<string, int> kings = new Dictionary<string, int>();


        Dictionary<string, int> castling = new Dictionary<string, int>();

        string turn;

        int epSquare;

        string halfMoves = "0";

        string moveNumber = "1";

        Dictionary<string, object> history = new Dictionary<string, object>();

        Dictionary<string, string> header = new Dictionary<string, string>();

        /*
        var castling = {w: 0, b: 0};
        var ep_square = EMPTY;
        var half_moves = 0;
        var move_number = 1;
        var history = [];
        var header = {};
        */

        /*
        var ROOKS = {
            w: [
                {square: SQUARES.a1, flag: BITS.QSIDE_CASTLE},
                {square: SQUARES.h1, flag: BITS.KSIDE_CASTLE}
            ],
            b: [
                {square: SQUARES.a8, flag: BITS.QSIDE_CASTLE},
                {square: SQUARES.h8, flag: BITS.KSIDE_CASTLE}
            ]
        };
        */
        public void InitRooks()
        {
            Dictionary<string, int>[] wContent = new Dictionary<string, int>[1];
            wContent[0].Add("square", this.SQUARES["a1"]);
            wContent[0].Add("flag", this.BITS["QSIDE_CASTLE"]);

            wContent[1].Add("square", this.SQUARES["H1"]);
            wContent[1].Add("flag", this.BITS["KSIDE_CASTLE"]);

            this.ROOKS["w"] = wContent;

            Dictionary<string, int>[] bContent = new Dictionary<string, int>[1];
            bContent[0].Add("square", this.SQUARES["a8"]);
            bContent[0].Add("flag", this.BITS["QSIDE_CASTLE"]);

            bContent[1].Add("square", this.SQUARES["H8"]);
            bContent[1].Add("flag", this.BITS["KSIDE_CASTLE"]);
            this.ROOKS["b"] = bContent;
        }

        public void InitPawnOffsets()
        {
            this.PAWN_OFFSETS.Add("b", new int[] { 16, 32, 17, 15 });
            this.PAWN_OFFSETS.Add("w", new int[] { -16, -32, -17, -15 });
        }

        public void InitPieceOffsets()
        {
            this.PIECE_OFFSETS.Add("n", new int[] { -18, -33, -31, -14, 18, 33, 31, 14 });
            this.PIECE_OFFSETS.Add("b", new int[] { -17, -15, 17, 15 });
            this.PIECE_OFFSETS.Add("r", new int[] { -16, 1, 16, -1 });
            this.PIECE_OFFSETS.Add("q", new int[] { -17, -16, -15, 1, 17, 16, 15, -1 });
            this.PIECE_OFFSETS.Add("k", new int[] { -17, -16, -15, 1, 17, 16, 15, -1 });
        }

        public void InitShifts()
        {
            this.SHIFTS.Add("p", 0);
            this.SHIFTS.Add("n", 1);
            this.SHIFTS.Add("b", 2);
            this.SHIFTS.Add("r", 3);
            this.SHIFTS.Add("q", 4);
            this.SHIFTS.Add("k", 5);
        }

        public void InitFlags()
        {
            this.FLAGS.Add("NORMAL", "n");
            this.FLAGS.Add("CAPTURE", "c");
            this.FLAGS.Add("BIG_PAWN", "b");
            this.FLAGS.Add("EP_CAPTURE", "e");
            this.FLAGS.Add("PROMOTION", "p");
            this.FLAGS.Add("KSIDE_CASTLE", "k");
            this.FLAGS.Add("QSIDE_CASTLE", "q");
        }

        public void InitBits()
        {
            this.BITS.Add("NORMAL", 1);
            this.BITS.Add("CAPTURE", 2);
            this.BITS.Add("BIG_PAWN", 4);
            this.BITS.Add("EP_CAPTURE", 8);
            this.BITS.Add("PROMOTION", 16);
            this.BITS.Add("KSIDE_CASTLE", 32);
            this.BITS.Add("QSIDE_CASTLE", 64);
        }

        public void InitSquares()
        {
            this.SQUARES.Add("a8", 0);
            this.SQUARES.Add("b8", 1);
            this.SQUARES.Add("c8", 2);
            this.SQUARES.Add("d8", 3);
            this.SQUARES.Add("e8", 4);
            this.SQUARES.Add("f8", 5);
            this.SQUARES.Add("g8", 6);
            this.SQUARES.Add("h8", 7);

            this.SQUARES.Add("a7", 16);
            this.SQUARES.Add("b7", 17);
            this.SQUARES.Add("c7", 18);
            this.SQUARES.Add("d7", 19);
            this.SQUARES.Add("e7", 20);
            this.SQUARES.Add("f7", 21);
            this.SQUARES.Add("g7", 22);
            this.SQUARES.Add("h7", 23);

            this.SQUARES.Add("a6", 32);
            this.SQUARES.Add("b6", 33);
            this.SQUARES.Add("c6", 34);
            this.SQUARES.Add("d6", 35);
            this.SQUARES.Add("e6", 36);
            this.SQUARES.Add("f6", 37);
            this.SQUARES.Add("g6", 38);
            this.SQUARES.Add("h6", 39);

            this.SQUARES.Add("a5", 48);
            this.SQUARES.Add("b5", 49);
            this.SQUARES.Add("c5", 50);
            this.SQUARES.Add("d5", 51);
            this.SQUARES.Add("e5", 52);
            this.SQUARES.Add("f5", 53);
            this.SQUARES.Add("g5", 54);
            this.SQUARES.Add("h5", 55);

            this.SQUARES.Add("a4", 64);
            this.SQUARES.Add("b4", 65);
            this.SQUARES.Add("c4", 66);
            this.SQUARES.Add("d4", 67);
            this.SQUARES.Add("e4", 68);
            this.SQUARES.Add("f4", 69);
            this.SQUARES.Add("g4", 70);
            this.SQUARES.Add("h4", 71);

            this.SQUARES.Add("a3", 80);
            this.SQUARES.Add("b3", 81);
            this.SQUARES.Add("c3", 82);
            this.SQUARES.Add("d3", 83);
            this.SQUARES.Add("e3", 84);
            this.SQUARES.Add("f3", 85);
            this.SQUARES.Add("g3", 86);
            this.SQUARES.Add("h3", 87);

            this.SQUARES.Add("a2", 96);
            this.SQUARES.Add("b2", 97);
            this.SQUARES.Add("c2", 98);
            this.SQUARES.Add("d2", 99);
            this.SQUARES.Add("e2", 100);
            this.SQUARES.Add("f2", 101);
            this.SQUARES.Add("g2", 102);
            this.SQUARES.Add("h2", 103);

            this.SQUARES.Add("a1", 112);
            this.SQUARES.Add("b1", 113);
            this.SQUARES.Add("c1", 114);
            this.SQUARES.Add("d1", 115);
            this.SQUARES.Add("e1", 116);
            this.SQUARES.Add("f1", 117);
            this.SQUARES.Add("g1", 118);
            this.SQUARES.Add("h1", 119);
        }

        public void InitCastling()
        {
            this.castling.Add("w", 0);
            this.castling.Add("b", 0);
        }

        public void InitKings()
        {
            this.kings.Add("w", this.EMPTY);
            this.kings.Add("b", this.EMPTY);
        }

        public Core(string fen)
        {
            this.fen = fen;

            this.turn = this.WHITE;
            this.epSquare = this.EMPTY;

            this.InitPawnOffsets();
            this.InitPieceOffsets();
            this.InitShifts();
            this.InitFlags();
            this.InitBits();
            this.InitSquares();
            this.InitRooks();

            if (this.fen == "")
            {
                this.Load(this.DEFAULT_POSITION, false);
            }
            else
            {
                this.Load(fen, false);
            }
        }

        /* called when the initial board setup is changed with put() or remove().
           * modifies the SetUp and FEN properties of the header object.  if the FEN is
           * equal to the default position, the SetUp and FEN are deleted
           * the setup is only updated if history.length is zero, ie moves haven't been
           * made.
           */
        public void UpdateSetup(string fen)
        {
            if (this.history.Count > 0)
            {
                return;
            }

            if (fen != this.DEFAULT_POSITION)
            {
                this.header["SetUp"] = "1";
                this.header["FEN"] = fen;
            }
            else
            {
                this.header.Remove("SetUp");
                this.header.Remove("FEN");
            }
        }

        public void InitializeBoard()
        {
            for (int i = 0; i < 128; i++) {
                this.board[i] = new Dictionary<string, string>();
            }
        }

        public void Clear(bool keepHeaders)
        {
            if (keepHeaders == null) {
                keepHeaders = false;
            }

            this.kings = new Dictionary<string, int>() {
                { "w", this.EMPTY },
                { "b", this.EMPTY }
            };
            this.turn = WHITE;
            this.castling = new Dictionary<string, int>() {
                { "w", this.EMPTY },
                { "b", this.EMPTY }
            };
            this.epSquare   = this.EMPTY;
            this.halfMoves  = "0";
            this.moveNumber = "1";
            this.history = new Dictionary<string, object>();

            if (!keepHeaders)
            {
                header = new Dictionary<string, string>();
            }
            this.UpdateSetup(this.GenerateFen());
        }

        public void reset()
        {
            this.Load(
                DEFAULT_POSITION,
                false
            );
        }

        public bool Load(string fen, bool keepHeaders)
        {
            if (keepHeaders == null) {
                keepHeaders = false;
            }

            string[] tokens = Regex.Split(this.fen, @"s +");
            string position = tokens[0];
            int square = 0;

            Dictionary<string, string> validateFenResult = this.ValidateFen(fen);

            //if (validateFenResult["valid"] == "false") {
                //return false;
            //}

            this.Clear(keepHeaders);

            for (var i = 0; i < position.Length; i++)
            {
                char piece = position[i];

                if (piece.Equals('/')) {
                    square += 8;
                } else if (int.TryParse(piece.ToString(), out int pieceOut)) {
                    square += Int32.Parse(piece.ToString());
                } else {
                    string color = (piece < 'a') ? this.WHITE : this.BLACK;

                    Dictionary<string, string> putParams = new Dictionary<string, string>() {
                        { "type", piece.ToString().ToLower()},
                        { "color", color }
                    };
                    this.Put(putParams, this.Algebraic(square));
                    square++;
                }
            }

            this.turn = tokens[1];

            if (tokens[2].IndexOf('K') > -1)
            {
                this.castling["w"] |= this.BITS["KSIDE_CASTLE"];
            }
            if (tokens[2].IndexOf('Q') > -1)
            {
                this.castling["w"] |= this.BITS["QSIDE_CASTLE"];
            }
            if (tokens[2].IndexOf('k') > -1)
            {
                this.castling["b"] |= this.BITS["KSIDE_CASTLE"];
            }
            if (tokens[2].IndexOf('q') > -1)
            {
                this.castling["b"] |= this.BITS["QSIDE_CASTLE"];
            }

            this.epSquare   = (tokens[3] == "-") ? this.EMPTY : this.SQUARES[tokens[3]];
            this.halfMoves  = tokens[4];
            this.moveNumber = tokens[5];

            this.UpdateSetup(this.GenerateFen());

            return true;
        }

        public string Algebraic(int i)
        {
            int f = this.File(i);
            int r = this.Rank(i);
            return "abcdefgh".Substring(f, f + 1) + "87654321".Substring(r, r + 1);
        }

        public int Rank(int i)
        {
            return i >> 4;
        }

        public int File(int i)
        {
            return i & 15;
        }

        public string SwapColor(string c)
        {
            return c == this.WHITE ? this.BLACK : this.WHITE;
        }

        public bool IsDigit(string c)
        {
            return "0123456789".IndexOf(c) != -1;
        }

        public bool Put(Dictionary<string, string> piece, string square)
        {
            if (!(piece.ContainsKey("type") && piece.ContainsKey("color"))) {
                return false;
            }

            if (this.SYMBOLS.IndexOf(piece["type"].ToLower()) == -1)
            {
                return false;
            }

            if (!this.SQUARES.ContainsKey(square)) {
                return false;
            }

            int sq = this.SQUARES[square];

            /* don't let the user place more than one king */
            if (piece["type"] == KING &&
                !(kings[piece["color"]] == this.EMPTY || kings[piece["color"]] == sq))
            {
                return false;
            }

            board[sq] = new Dictionary<string, string>() {
                { "type", piece["type"]},
                { "color", piece["color"]}
            };

            if (piece["type"] == this.KING)
            {
                kings[piece["color"]] = sq;
            }

            this.UpdateSetup(this.GenerateFen());

            return true;
        }

        public string GenerateFen()
        {
            int empty = 0;
            string fen = "";

            for (var i = this.SQUARES["a8"]; i <= this.SQUARES["h1"]; i++)
            {
                if (board[i] == null)
                {
                    empty++;
                } else {
                    if (empty > 0) {
                        fen += empty;
                        empty = 0;
                    }
                    string color = board[i]["color"];
                    string piece = board[i]["type"];

                    fen += (color == this.WHITE) ? piece.ToUpper() : piece.ToLower();
                }

                if (((i + 1) & 0x88) != 0)
                {
                    if (empty > 0)
                    {
                        fen += empty;
                    }

                    if (i != this.SQUARES["h1"])
                    {
                        fen += '/';
                    }

                    empty = 0;
                    i += 8;
                }
            }

            string cflags = " ";
            if ((this.castling[this.WHITE] & this.BITS["KSIDE_CASTLE"]) != 0) { cflags += "K"; }
            if ((this.castling[this.WHITE] & this.BITS["QSIDE_CASTLE"]) != 0) { cflags += "Q"; }
            if ((this.castling[this.BLACK] & this.BITS["KSIDE_CASTLE"]) != 0) { cflags += "k"; }
            if ((this.castling[this.BLACK] & this.BITS["QSIDE_CASTLE"]) != 0) { cflags += "q"; }

            /* do we have an empty castling flag? */
            //cflags = cflags || "-";
            cflags = !string.IsNullOrEmpty(cflags) ? cflags : "-";
            var epflags = (this.epSquare == this.EMPTY) ? "-" : this.Algebraic(epSquare);

            string[] result = new string[] {
                fen,
                turn,
                cflags,
                epflags, 
                this.halfMoves,
                this.moveNumber
            };

            return String.Join(" ", result);
        }

        public Dictionary<string, string> ValidateFen(string fenVar)
        {
            this.fen = fenVar;

            Dictionary<int, string> errors = new Dictionary<int, string>
            {
                { 0, "No errors." },
                { 1, "FEN string must contain six space-delimited fields." },
                { 2, "6th field (move number) must be a positive integer." },
                { 3, "5th field (half move counter) must be a non-negative integer." },
                { 4, "4th field (en-passant square) is invalid." },
                { 5, "3rd field (castling availability) is invalid." },
                { 6, "2nd field (side to move) is invalid." },
                { 7, "1st field (piece positions) does not contain 8 \\'/\\'-delimited rows." },
                { 8, "1st field (piece positions) is invalid [consecutive numbers]." },
                { 9, "1st field (piece positions) is invalid [invalid piece]." },
                { 10, "1st field (piece positions) is invalid [row too large]." },
                { 11, "Illegal en-passant square." }
            };

            /* 1st criterion: 6 space-seperated fields? */
            string[] tokens = this.fen.Split(" ");
            if (tokens.Length != 6)
            {
                Dictionary<string, string> result = new Dictionary<string, string>
                {
                    { "valid", "false" },
                    { "error_number", "1" },
                    { "errors", errors[1] }
                };
                return result;
            }

            /* 2nd criterion: move number field is a integer value > 0? */
            if (int.TryParse(tokens[5], out int token5) || (Int32.Parse(tokens[5]) <= 0))
            {
                Dictionary<string, string> result = new Dictionary<string, string>
                {
                    { "valid", "false" },
                    { "error_number", "2" },
                    { "errors", errors[2] }
                };
                return result;
            }

            /* 3rd criterion: half move counter is an integer >= 0? */
            if (int.TryParse(tokens[4], out int token4) || (Int32.Parse(tokens[4]) < 0))
            {
                Dictionary<string, string> result = new Dictionary<string, string>
                {
                    { "valid", "false" },
                    { "error_number", "3" },
                    { "errors", errors[3] }
                };
                return result;
            }

            /* 4th criterion: 4th field is a valid e.p.-string? */
            //if (!/^ (-|[abcdefgh][36])$/.test(tokens[3])) {
            Regex regex = new Regex(@"^ (-|[abcdefgh][36])$");
            Match match = regex.Match(tokens[3]);
            if (match.Success)
            {
                Dictionary<string, string> result = new Dictionary<string, string>
                {
                    { "valid", "false" },
                    { "error_number", "4" },
                    { "errors", errors[4] }
                };
                return result;
            }

            /* 5th criterion: 3th field is a valid castle-string? */
            //if (!/^ (KQ ? k ? q ?| Qk ? q ?| kq ?| q | -)$/.test(tokens[2])) {
            //    return { valid: false, error_number: 5, error: errors[5]};
            //}
            Regex regex2 = new Regex(@"^ (KQ ? k ? q ?| Qk ? q ?| kq ?| q | -)$");
            Match match2 = regex2.Match(tokens[2]);
            if (match2.Success)
            {
                Dictionary<string, string> result = new Dictionary<string, string>
                {
                    { "valid", "false" },
                    { "error_number", "5" },
                    { "errors", errors[5] }
                };
                return result;
            }

            /* 6th criterion: 2nd field is "w" (white) or "b" (black)? */
            //if (!/^ (w | b)$/.test(tokens[1])) {
            //    return { valid: false, error_number: 6, error: errors[6]};
            //}
            Regex regex3 = new Regex(@"^ (w | b)$");
            Match match3 = regex3.Match(tokens[2]);
            if (match3.Success)
            {
                Dictionary<string, string> result = new Dictionary<string, string>
                {
                    { "valid", "false" },
                    { "error_number", "6" },
                    { "errors", errors[6] }
                };
                return result;
            }

            /* 7th criterion: 1st field contains 8 rows? */
            //var rows = tokens[0].split('/');
            //if (rows.length !== 8)
            //{
            //    return { valid: false, error_number: 7, error: errors[7]};
            //}

            string[] rows = this.fen.Split("/");
            if (rows.Length != 8) {
                Dictionary<string, string> result = new Dictionary<string, string>
                {
                    { "valid", "false" },
                    { "error_number", "8" },
                    { "errors", errors[8] }
                };
                return result;
            }

            for (int i = 0; i < rows.Length; i++)
            {
                int sum_fields = 0;
                bool previous_was_number = false;

                for (int k = 0; k < rows[i].Length; k++)
                {
                    if (!Char.IsDigit(rows[i][k]))
                    {
                        if (previous_was_number)
                        {
                            Dictionary<string, string> result = new Dictionary<string, string>
                            {
                                { "valid", "false" },
                                { "error_number", "8" },
                                { "errors", errors[8] }
                            };
                            return result;
                        }
                        sum_fields += Convert.ToInt32(rows[i][k]);
                        previous_was_number = true;
                    }
                    else
                    {
                        Regex regex4 = new Regex(@"^[prnbqkPRNBQK]$");
                        Match match4 = regex4.Match(rows[i][k].ToString());

                        if (!match4.Success)
                        {
                            Dictionary<string, string> result = new Dictionary<string, string>
                            {
                                { "valid", "false" },
                                { "error_number", "9" },
                                { "errors", errors[9] }
                            };
                            return result;
                        }
                        sum_fields += 1;
                        previous_was_number = false;
                    }
                }

                if (sum_fields != 8)
                {
                    Dictionary<string, string> result = new Dictionary<string, string>
                    {
                        { "valid", "false" },
                        { "error_number", "10" },
                        { "errors", errors[10] }
                    };
                    return result;
                }
            }

            if ((tokens[3][1] == '3' && tokens[1] == "w") || (tokens[3][1].ToString() == "6" && tokens[1] == "b"))
            {
                Dictionary<string, string> result = new Dictionary<string, string>
                    {
                        { "valid", "false" },
                        { "error_number", "11" },
                        { "errors", errors[11] }
                    };
                return result;
            }

            Dictionary<string, string> final = new Dictionary<string, string>
                {
                    { "valid", "true" },
                    { "error_number", "0" },
                    { "errors", errors[0] }
                };
            /* everything's okay! */
            return final;
        }

        /*
         * 
         */
        public void Reset()
        {
            this.Load(this.DEFAULT_POSITION, false);
        }

        /*
         * 
         */
        public Dictionary<string, string> SetHeader(Dictionary<int, string> args)
        {
            for (int i = 0; i < args.Count; i += 2) {
                if (args[i] is "string" && args[i + 1] == "string") {
                    this.header[args[i]] = args[i + 1];
                }
            }
            return header;
        }

        public Dictionary<string, string> Get(string square)
        {
            Dictionary<string, string> piece = this.board[this.SQUARES[square]];
            return piece == null ? new Dictionary<string, string>() { 
                { "type", piece["type"] },
                { "color", piece["color"] } 
            } : null;
        }

        public Dictionary<string, string> Remove(string square)
        {
            Dictionary<string, string> piece = this.Get(square);
            board[this.SQUARES[square]] = null;
            if (piece != null && piece["type"] == this.KING)
            {
                kings[piece["color"]] = this.EMPTY;
            }

            this.UpdateSetup(this.GenerateFen());

            return piece;
        }

        public Dictionary<string, string> BuildMove(Dictionary<string, string>[] localBoard, string from, string to, int flags, string promotion)
        {
            Dictionary<string, string> boardFrom = localBoard[Convert.ToInt32(from)];
            Dictionary<string, string> move = new Dictionary<string, string>() {
                { "color", this.turn },
                { "from", from },
                { "to", "to" },
                { "flags", flags.ToString() },
                { "piece",  boardFrom["type"] }
            };

            if (promotion != null) {
                move["flags"] = move["flags"] != null ? move["flags"] : "";
                move["promotion"] = promotion;
            }
            Dictionary<string, string> boardTo = localBoard[Convert.ToInt32(to)];
            if (boardTo != null) {
                move["captured"] = boardTo["type"];
            } else if ((flags & this.BITS["EP_CAPTURE"]) != 0) {
                move["captured"] = this.PAWN;
            }
            return move;
        }

        public void Push(Dictionary<string, string> move)
        {
            /*history.push({
                move: move,
                kings: { b: kings.b, w: kings.w},
                turn: turn,
                castling: { b: castling.b, w: castling.w},
                ep_square: ep_square,
                half_moves: half_moves,
                move_number: move_number
            });*/
            Dictionary<string, int> kingsLocal = new Dictionary<string, int>() {
                {"b", this.kings["b"] },
                {"w", this.kings["w"] }
            };
            Dictionary<string, int> castlingLocal = new Dictionary<string, int>() {
                {"b", this.castling["b"] },
                {"w", this.castling["w"] }
            };
            this.history = new Dictionary<string, object>()
            {
                { "move",        move },
                { "kings",       kingsLocal },
                { "turn",        this.turn },
                { "castling",    castlingLocal },
                { "ep_square",   this.epSquare },
                { "half_moves",  this.halfMoves },
                { "move_number", this.moveNumber }
            };
        }


        public void MakeMove(Dictionary<string, string> move)
        {
            string us = this.turn;
            string them = this.SwapColor(us);
            this.Push(move);

            int moveTo = Convert.ToInt32(move["to"]);
            int moveFrom = Convert.ToInt32(move["from"]);

            board[moveTo] = board[moveFrom];
            board[moveFrom] = null;

            /* if ep capture, remove the captured pawn */
            if ((Convert.ToInt32(move["flags"]) & this.BITS["EP_CAPTURE"]) != 0) {
                if (this.turn == this.BLACK)
                {
                    board[moveTo - 16] = null;
                }
                else
                {
                    board[moveTo + 16] = null;
                }
            }

            /* if pawn promotion, replace with new piece */
            if ((Convert.ToInt32(move["flags"]) & this.BITS["PROMOTION"]) != 0) {
                board[moveTo] = new Dictionary<string, string>() {
                    { "type", move["promotion"]},
                    {"color", us}
                };
            }

            /* if we moved the king */
            Dictionary<string, string> boardMoveTo = board[moveTo];
            if (boardMoveTo["type"] == this.KING)
            {
                this.kings[boardMoveTo["color"]] = moveTo;

                /* if we castled, move the rook next to the king */
                if ((Convert.ToInt32(move["flags"]) & this.BITS["KSIDE_CASTLE"]) != 0)
                {
                    var castling_to = moveTo - 1;
                    var castling_from = moveTo + 1;
                    board[castling_to] = board[castling_from];
                    board[castling_from] = null;
                }
                else if ((Convert.ToInt32(move["flags"]) & this.BITS["QSIDE_CASTLE"]) != 0)
                {
                    var castlingTo = moveTo + 1;
                    var castlingFrom = moveTo - 2;
                    board[castlingTo] = board[castlingFrom];
                    board[castlingFrom] = null;
                }

                /* turn off castling */
                //-100 == ""
                this.castling[us] = -100;
            }

            /* turn off castling if we move a rook */
            if (this.castling[us] > 0)
            {

                for (int i = 0, len = this.ROOKS[us].Length; i < len; i++)
                {
                    Dictionary<string, int> rookUsItem = this.ROOKS[us][i];
                    if (move["from"] == rookUsItem["square"].ToString() && (this.castling[us] & rookUsItem["flag"]) != 0) {
                        this.castling[us] ^= rookUsItem["flag"];
                        break;
                    }
                }
            }

            /* turn off castling if we capture a rook */
            if (this.castling[them] > 0)
            {
                for (int i = 0, len = this.ROOKS[them].Length; i < len; i++)
                {
                    Dictionary<string, int> rookThemItem = this.ROOKS[them][i];
                    if (moveTo == rookThemItem["square"] && (this.castling[them] & rookThemItem["flag"]) != 0)
                    {
                        this.castling[them] ^= rookThemItem["flag"];
                        break;
                    }
                }
            }

            /* if big pawn move, update the en passant square */
            int moveFlags = Convert.ToInt32(move["flags"]);
            if ((moveFlags & this.BITS["BIG_PAWN"]) != 0)
            {
                if (this.turn == "b")
                {
                    this.epSquare = moveTo - 16;
                }
                else
                {
                    this.epSquare = moveTo + 16;
                }
            } else {
                this.epSquare = this.EMPTY;
            }

            /* reset the 50 move counter if a pawn is moved or a piece is captured */
            if (move["piece"] == this.PAWN)
            {
                this.halfMoves = "0";
            }
            else if ((moveFlags & (this.BITS["CAPTURE"] | this.BITS["EP_CAPTURE"])) != 0)
            {
                this.halfMoves = "0";
            }
            else
            {
                int halfMovesInt = Convert.ToInt32(this.halfMoves);
                halfMovesInt++;
                this.halfMoves = halfMovesInt.ToString();
            }

            if (this.turn == this.BLACK)
            {
                int moveNumberInt = Convert.ToInt32(this.moveNumber);
                moveNumberInt++;
                this.moveNumber = moveNumberInt.ToString();
            }
            this.turn = this.SwapColor(this.turn);
        }

        public Dictionary<string, string>[] AddMove(Dictionary<string, string>[] localBoard, Dictionary<string, string>[] moves, string from, string to, int flags)
        {
            /* if pawn promotion */
            Dictionary<string, string> boardFrom = localBoard[Convert.ToInt32(from)];
            int toInt = Convert.ToInt32(to);
            if (boardFrom["type"] == this.PAWN && (this.Rank(toInt) == this.RANK_8 || this.Rank(toInt) == this.RANK_1))
            {
                string[] pieces = new string[] { this.QUEEN, this.ROOK, this.BISHOP, this.KNIGHT };
                for (int i = 0, len = pieces.Length; i < len; i++)
                {
                    //risk by length
                    moves[moves.Length] = this.BuildMove(localBoard, from, to, flags, pieces[i]);
                }
            } else {
                //risk by lenght
                moves[moves.Length] = this.BuildMove(localBoard, from, to, flags, null);
            }

            return moves;
        }

        public Dictionary<string, string>[] GenerateMoves(Dictionary<string, bool> options)
        {
            Dictionary<string, string>[] moves = new Dictionary<string, string>[] { };
            string us = this.turn;
            string them = this.SwapColor(us);
            Dictionary<string, int> secondRank = new Dictionary<string, int>() { { "b", this.RANK_7 }, { "w", this.RANK_2 } };

            int firstSq = this.SQUARES["a8"];
            int lastSq = this.SQUARES["h1"];
            bool singleSquare = false;

            /* do we want legal moves? */
            bool legal = (options != null && options.ContainsKey("legal")) ? options["legal"] : true;

            /* are we generating moves for a single square? */
            if (options != null && options.ContainsKey("square"))
            {
                if (this.SQUARES.ContainsKey(options["square"].ToString()))
                {
                    firstSq = lastSq = this.SQUARES[options["square"].ToString()];
                    singleSquare = true;
                }
                else
                {
                    /* invalid square */
                    return null;
                }
            }

            for (int i = firstSq; i <= lastSq; i++)
            {
                /* did we run off the end of the board */
                if ((i & 0x88) != 0)
                {
                    i += 7; continue;
                }

                Dictionary<string, string> piece = this.board[i];
                if (piece == null || piece["color"] != us)
                {
                    continue;
                }

                if (piece["type"] == this.PAWN)
                {
                    /* single square, non-capturing */
                    int square = i + this.PAWN_OFFSETS[us][0];
                    if (board[square] == null)
                    {
                        moves = this.AddMove(this.board, moves, i.ToString(), square.ToString(), this.BITS["NORMAL"]);

                        /* double square */
                        square = i + this.PAWN_OFFSETS[us][1];
                        if (secondRank[us] == this.Rank(i) && this.board[square] == null)
                        {
                            moves = this.AddMove(this.board, moves, i.ToString(), square.ToString(), this.BITS["BIG_PAWN"]);
                        }
                    }

                    /* pawn captures */
                    for (int j = 2; j < 4; j++)
                    {
                        square = i + this.PAWN_OFFSETS[us][j];
                        if ((square & 0x88) != 0)
                        {
                            continue;
                        }
                        Dictionary<string, string> boardSquare = this.board[square];
                        if (boardSquare != null && boardSquare["color"] == them)
                        {
                            moves = this.AddMove(board, moves, i.ToString(), square.ToString(), this.BITS["CAPTURE"]);
                        }
                        else if (square == this.epSquare)
                        {
                            this.AddMove(board, moves, i.ToString(), this.epSquare.ToString(), this.BITS["EP_CAPTURE"]);
                        }
                    }
                }
                else
                {
                    for (int j = 0, len2 = this.PIECE_OFFSETS[piece["type"]].Length; j < len2; j++)
                    {
                        int offset = this.PIECE_OFFSETS[piece["type"]][j];
                        int square = i;

                        while (true)
                        {
                            square += offset;
                            if ((square & 0x88) != 0)
                            {
                                break;
                            }
                            Dictionary<string, string> boardSquare = this.board[square];
                            if (boardSquare == null)
                            {
                                this.AddMove(this.board, moves, i.ToString(), square.ToString(), this.BITS["NORMAL"]);
                            }
                            else
                            {
                                if (boardSquare["color"] == us)
                                {
                                    break;
                                }
                                moves = this.AddMove(this.board, moves, i.ToString(), square.ToString(), this.BITS["CAPTURE"]);
                                break;
                            }

                            /* break, if knight or king */
                            if (piece["type"] == "n" || piece["type"] == "k")
                            {
                                break;
                            }
                        }
                    }
                }
            }

            /* check for castling if: a) we're generating all moves, or b) we're doing
            * single square move generation on the king's square
            */
            if ((!singleSquare) || lastSq == this.kings[us])
            {
                /* king-side castling */
                if ((this.castling[us] & this.BITS["KSIDE_CASTLE"]) != 0)
                {
                    int castlingFrom = this.kings[us];
                    int castlingTo = castlingFrom + 2;

                    if (board[castlingFrom + 1] == null && this.board[castlingTo] == null &&
                    !this.Attacked(them, kings[us]) &&
                    !this.Attacked(them, castlingFrom + 1) &&
                    !this.Attacked(them, castlingTo))
                    {
                        moves = this.AddMove(board, moves, this.kings[us].ToString(), castlingTo.ToString(), this.BITS["KSIDE_CASTLE"]);
                    }
                }

                /* queen-side castling */
                if ((this.castling[us] & this.BITS["QSIDE_CASTLE"]) != 0)
                {
                    int castlingFrom = this.kings[us];
                    int castlingTo = castlingFrom - 2;

                    if (this.board[castlingFrom - 1] == null &&
                    this.board[castlingFrom - 2] == null &&
                    this.board[castlingFrom - 3] == null &&
                    !this.Attacked(them, this.kings[us]) &&
                    !this.Attacked(them, castlingFrom - 1) &&
                    !this.Attacked(them, castlingTo))
                    {
                        moves = this.AddMove(board, moves, kings[us].ToString(), castlingTo.ToString(), this.BITS["QSIDE_CASTLE"]);
                    }
                }
            }


            /* return all pseudo-legal moves (this includes moves that allow the king
             * to be captured)
             */
            if (legal != null)
            {
                return moves;
            }

            /* filter out illegal moves */
            Dictionary<string, string>[] legalMoves = new Dictionary<string, string>[] {};
            for (int i = 0, len3 = moves.Length; i < len3; i++)
            {
                this.MakeMove(moves[i]);
                if (!this.KingAttacked(us))
                {
                    legalMoves[legalMoves.Length] = moves[i];
                }
                this.UndoMove();
            }

            return legalMoves;
        }

        // parses all of the decorators out of a SAN string
        public string StrippedSan(string move)
        {
            //return move.replace(/=/, '').replace(/[+#]?[?!]*$/,'');
            Regex regex = new Regex(@"=");
            string moveResult = regex.Replace(move, "");

            regex = new Regex(@"[+#]?[?!]*$");
            moveResult = regex.Replace(moveResult, "");
            return moveResult;
        }

        // convert a move from Standard Algebraic Notation (SAN) to 0x88 coordinates
        public Dictionary<string, string> MoveFromSan(string move, bool sloppy)
        {
            MatchCollection matches;
            // strip off any move decorations: e.g Nf3+?!
            string cleanMove = this.StrippedSan(move);
            string piece     = "";
            string from      = "";
            string to        = "";
            string promotion = "";


            Regex matchCondition = new Regex(@" ([pnbrqkPNBRQK]) ? ([a - h][1 - 8])x ? -? ([a - h][1 - 8])([qrbnQRBN]) ?");
            matches = Regex.Matches(cleanMove, @" ([pnbrqkPNBRQK]) ? ([a - h][1 - 8])x ? -? ([a - h][1 - 8])([qrbnQRBN]) ?");

            // if we're using the sloppy parser run a regex to grab piece, to, and from
            // this should parse invalid SAN like: Pe2-e4, Rc1c4, Qf3xf7
            if (sloppy)
            {
                //var matches = cleanMove.match(/ ([pnbrqkPNBRQK]) ? ([a - h][1 - 8])x ? -? ([a - h][1 - 8])([qrbnQRBN]) ?/);
                if (matchCondition.IsMatch(cleanMove))
                {
                    piece     = matches[1].Value;
                    from      = matches[2].Value;
                    to        = matches[3].Value;
                    promotion = matches[4].Value;

                }
            }

            Dictionary<string, string>[] moves = this.GenerateMoves(null);
            for (int i = 0, len = moves.Length; i < len; i++)
            {
                // try the strict parser first, then the sloppy parser if requested
                // by the user
                if ((cleanMove == this.StrippedSan(this.MoveToSan(moves[i], false))) || (sloppy && cleanMove == this.StrippedSan(this.MoveToSan(moves[i], true))))
                {
                    return moves[i];
                }
                else
                {
                    Dictionary<string, string> moveI = moves[i];
                    if (matches != null && (piece == "" || piece.ToLower() == moveI["piece"]) && this.SQUARES[from] == Convert.ToInt32(moveI["from"]) && this.SQUARES[to] == Convert.ToInt32(moveI["to"]) && (promotion == "" || promotion.ToLower() == moveI["promotion"]))
                    {
                        return moveI;
                    }
                }
            }

            return null;
        }

        public string MoveToSan(Dictionary<string, string> move, bool sloppy) {

            string output = "";
            int moveFlagsInt = Convert.ToInt32(move["flags"]);
            int moveFromInt  = Convert.ToInt32(move["from"]);
            int moveFromTo   = Convert.ToInt32(move["to"]);

            if ((moveFlagsInt & this.BITS["KSIDE_CASTLE"]) != 0) {
                 output = "O-O";
            } else if ((moveFlagsInt & this.BITS["QSIDE_CASTLE"]) != 0) {
                 output = "O-O-O";
            } else {
              string disambiguator = this.GetDisambiguator(move, sloppy);

                if (move["piece"] != this.PAWN) {
                    output += move["piece"].ToUpper() + disambiguator;
                }

                if ((moveFlagsInt & ((this.BITS["CAPTURE"] | this.BITS["EP_CAPTURE"]))) != 0) {
                    if (move["piece"] == this.PAWN) {
                        string algebraic = this.Algebraic(moveFromInt);
                        output += algebraic[0];
                    }
                    output += "x";
                }

                output += this.Algebraic(moveFromTo);

                if ((moveFlagsInt & this.BITS["PROMOTION"]) != 0) {
                    output += "=" + move["promotion"].ToUpper();
                }
            }

            this.MakeMove(move);
            if (this.InCheck()) {
                if (this.InCheckmate()) {
                    output += "#";
                } else {
                    output += "+";
                }
            }
            this.UndoMove();

            return output;
        }

        public bool KingAttacked(string color)
        {
            return this.Attacked(this.SwapColor(color), this.kings[color]);
        }


        public bool InCheck()
        {
            return this.KingAttacked(this.turn);
        }

        public bool InCheckmate()
        {
            Dictionary<string, string>[] generateMoves = this.GenerateMoves(null);
            return this.InCheck() && generateMoves.Length == 0;
        }

        public bool InStalemate()
        {
            Dictionary<string, string>[] generateMoves = this.GenerateMoves(null);
            return !this.InCheck() && generateMoves.Length == 0;
        }

        public string GetDisambiguator(Dictionary<string, string> move, bool sloppy)
        {
            Dictionary<string, bool> options = new Dictionary<string, bool>() { 
                { "legal",  !sloppy }
            };
            Dictionary<string, string>[] moves = this.GenerateMoves(options);

            string from = move["from"];
            string to = move["to"];
            string piece = move["piece"];

            int ambiguities = 0;
            int sameRank    = 0;
            int sameFile    = 0;

            for (int i = 0, len = moves.Length; i < len; i++)
            {
                Dictionary<string, string> movesI = moves[i];
                string ambigFrom  = movesI["from"];
                string ambigTo    = movesI["to"];
                string ambigPiece = movesI["piece"];

                /* if a move of the same piece type ends on the same to square, we'll
                 * need to add a disambiguator to the algebraic notation
                 */
                if (piece == ambigPiece && from != ambigFrom && to == ambigTo)
                {
                    ambiguities++;
                    int fromInt = Convert.ToInt32(from);
                    int ambigFromInt = Convert.ToInt32(ambigFrom);

                    if (this.Rank(fromInt) == this.Rank(ambigFromInt))
                    {
                        sameRank++;
                    }

                    if (this.File(fromInt) == this.File(ambigFromInt))
                    {
                        sameFile++;
                    }
                }
            }

            if (ambiguities > 0)
            {
                int fromInt = Convert.ToInt32(from);
                /* if there exists a similar moving piece on the same rank and file as
                 * the move in question, use the square as the disambiguator
                 */
                if (sameRank > 0 && sameFile > 0)
                {
                    return this.Algebraic(fromInt);
                }
                /* if the moving piece rests on the same file, use the rank symbol as the
                 * disambiguator
                 */
                else if (sameFile > 0)
                {
                    string result = this.Algebraic(fromInt);
                    return result[1].ToString();
                }
                /* else use the file symbol */
                else
                {
                    string result = this.Algebraic(fromInt);
                    return result[0].ToString();
                }
            }

            return "";
        }
        
        public Dictionary<string, string> UndoMove()
        {
            Dictionary<string, object> old = this.history;
            if (old == null) {
                return null;
            }

            Dictionary<string, string> move = (Dictionary<string, string>) old["move"];
            this.kings      = (Dictionary<string, int>) old["kings"];
            this.turn       = old["turn"].ToString();
            this.castling   = (Dictionary<string, int>) old["castling"];
            this.epSquare   = Convert.ToInt32(old["ep_square"]);
            this.halfMoves  = old["half_moves"].ToString();
            this.moveNumber = old["move_number"].ToString();

            var us = turn;
            var them = this.SwapColor(turn);

            int moveFrom  = Convert.ToInt32(move["from"]);
            int moveTo    = Convert.ToInt32(move["to"]);
            int moveFlags = Convert.ToInt32(move["flags"]);

            this.board[moveFrom] = board[moveTo];
            this.board[moveFrom]["type"] = move["piece"];  // to undo any promotions
            this.board[moveTo] = null;

            if ((moveFlags & this.BITS["CAPTURE"]) != 0)
            {
                this.board[moveTo] = new Dictionary<string, string>() {
                    { "type", move["captured"] },
                    { "color", them }
                };
            }
            else if ((moveFlags & this.BITS["EP_CAPTURE"]) != 0)
            {
                int index;
                if (us == this.BLACK)
                {
                    index = moveTo - 16;
                } else {
                    index = moveTo + 16;
                }
                board[index] = new Dictionary<string, string>() {
                    { "type",  this.PAWN },
                    { "color", them }
                };
            }

            int moveFlagsInt = Convert.ToInt32(move["flags"]);

            if ((moveFlagsInt & (this.BITS["KSIDE_CASTLE"] | this.BITS["QSIDE_CASTLE"])) != 0)
            {
                int castlingTo   = 0;
                int castlingFrom = 0;

                if ((moveFlags & this.BITS["KSIDE_CASTLE"]) != 0)
                {
                    castlingTo   = moveTo + 1;
                    castlingFrom = moveTo - 1;
                }
                else if ((moveFlags & this.BITS["QSIDE_CASTLE"]) != 0)
                {
                    castlingTo   = moveTo - 2;
                    castlingFrom = moveTo + 1;
                }

                this.board[castlingTo] = this.board[castlingFrom];
                this.board[castlingFrom] = null;
            }

            return move;
        }

        public bool Attacked(string color, int square)
        {
            for (int i = this.SQUARES["a8"]; i <= this.SQUARES["h1"]; i++)
            {
                /* did we run off the end of the board */
                if ((i & 0x88) != 0) { 
                    i += 7;
                    continue; 
                }

                /* if empty square or wrong color */

                Dictionary<string, string> piece = board[i];
                if (board[i] == null || piece["color"] != color) continue;

                int difference = i - square;
                int index = difference + 119;

                    if ((this.ATTACKS[index] & (1 << this.SHIFTS[piece["type"]])) != 0)
                {
                        if (piece["type"] == this.PAWN)
                    {
                        if (difference > 0)
                        {
                            if (piece["color"] == this.WHITE) return true;
                        }
                        else
                        {
                            if (piece["color"] == this.BLACK) return true;
                        }
                        continue;
                    }

                    /* if the piece is a knight or a king */
                        if (piece["type"] == "n" || piece["type"] == "k") return true;

                    var offset = RAYS[index];
                    var j = i + offset;

                    var blocked = false;
                    while (j != square)
                    {
                        if (board[j] != null) { 
                                blocked = true; break;
                            }
                        j += offset;
                    }

                    if (!blocked) return true;
                }
            }

            return false;
        }

        public bool InsufficientMaterial() {
            
            Dictionary<string, int> pieces = new Dictionary<string, int>();
            int[] bishops = new int[1];
            int numPieces = 0;
            int sqColor   = 0;

            for (int i = this.SQUARES["a8"]; i <= this.SQUARES["h1"]; i++) {
                sqColor = (sqColor + 1) % 2;
                if ((i & 0x88) != 0) { 
                    i += 7; continue;
                }

                Dictionary<string, string> piece = this.board[i];
                if (piece != null) {
                    pieces[piece["type"]] = (pieces.ContainsKey(piece["type"])) ? Convert.ToInt32(pieces[piece["type"]]) + 1 : 1;
                    if (piece["type"] == this.BISHOP) {
                        bishops[bishops.Length] = sqColor;
                    }
                    numPieces++;
                }
            }

            /* k vs. k */
            if (numPieces == 2) { return true; }

            /* k vs. kn .... or .... k vs. kb */
            else if (numPieces == 3 && (pieces[this.BISHOP] == 1 || pieces[this.KNIGHT] == 1)) {
                return true; 
            }

            /* kb vs. kb where any number of bishops are all on the same color */
            else if (numPieces == pieces[this.BISHOP] + 2) {
                int sum = 0;
                int len = bishops.Length;
                for (var i = 0; i < len; i++) {
                    sum += bishops[i];
                }
                if (sum == 0 || sum == len) { 
                    return true; 
                }
            }

            return false;
        }
    }
}
using System;
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

        string PAWN   = "p";
        string KNIGHT = "n";
        string BISHOP = "b";
        string ROOK   = "r";
        string QUEEN  = "q";
        string KING   = "k";

        string SYMBOLS = "pnbrqkPNBRQK";
        string DEFAULT_POSITION = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        string[] POSSIBLE_RESULTS = new string[] { "1-0", "0-1", "1/2-1/2", "*" };

        /*
         *  var PAWN_OFFSETS = {
         *   b: [16, 32, 17, 15],
         *   w: [-16, -32, -17, -15]
         *  };
         */
        System.Collections.Generic.Dictionary<string, int[]> PAWN_OFFSETS = new Dictionary<string, int[]>();

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
        Dictionary<string, int>[] ROOKS = new Dictionary<string, int>[1];

        /**
         * var board = new Array(128);
         */
        Dictionary<string, int>[] board = new Dictionary<string, int>[128];

        /**
         * var kings = {w: EMPTY, b: EMPTY};
         */
        Dictionary<string, int> kings = new Dictionary<string, int>();


        Dictionary<string, int> castling = new Dictionary<string, int>();

        string turn;

        int epSquare;

        int halfMoves = 0;

        int moveNumber = 1;

        int[] history = new int[] {};

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

        public Chess(string fen)
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

            if (this.fen == "") {
                this.Load (this.DEFAULT_POSITION, false);
            } else {

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
            if (this.history.Length > 0) {
                return;
            }

            if (fen != this.DEFAULT_POSITION)
            {
                this.header["SetUp"] = "1";
                this.header["FEN"] = fen;
            } else {
                this.header.Remove("SetUp");
                this.header.Remove("FEN");
            }
        }


        public void Clear(bool keepHeaders)
        {
            if (keepHeaders == null) {
                keepHeaders = false;
            }

            this.board = new Dictionary<string, int>[128];
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
            this.halfMoves  = 0;
            this.moveNumber = 1;
            this.history = new int[] {};
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
                this.castling["b"] |= this.BITS.["KSIDE_CASTLE"];
            }
            if (tokens[2].IndexOf('q') > -1)
            {
                this.castling["b"] |= this.BITS["QSIDE_CASTLE"];
            }

            this.epSquare   = (tokens[3] == "-") ? this.EMPTY : this.SQUARES[tokens[3]];
            this.halfMoves  = Int32.Parse(tokens[4]);
            this.moveNumber = Int32.Parse(tokens[5]);

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
            if (!('type' in piece && 'color' in piece)) {
                return false;
            }

            if (SYMBOLS.indexOf(piece.type.toLowerCase()) === -1)
            {
                return false;
            }

            if (!(square in SQUARES)) {
                return false;
            }

            var sq = this.SQUARES[square];

            /* don't let the user place more than one king */
            if (piece.type == KING &&
                !(kings[piece.color] == EMPTY || kings[piece.color] == sq))
            {
                return false;
            }

            board[sq] = { type: piece.type, color: piece.color};
            if (piece.type === KING)
            {
                kings[piece.color] = sq;
            }

            this.UpdateSetup(this.GenerateFen());

            return true;
        }

        public void GenerateFen()
        {
            var empty = 0;
            var fen = '';

            for (var i = SQUARES.a8; i <= SQUARES.h1; i++)
            {
                if (board[i] == null)
                {
                    empty++;
                }
                else
                {
                    if (empty > 0)
                    {
                        fen += empty;
                        empty = 0;
                    }
                    var color = board[i].color;
                    var piece = board[i].type;

                    fen += (color === WHITE) ?
                             piece.toUpperCase() : piece.toLowerCase();
                }

                if ((i + 1) & 0x88)
                {
                    if (empty > 0)
                    {
                        fen += empty;
                    }

                    if (i !== SQUARES.h1)
                    {
                        fen += '/';
                    }

                    empty = 0;
                    i += 8;
                }
            }

            char cflags = ' ';
            if (castling[WHITE] & BITS.KSIDE_CASTLE) { cflags += 'K'; }
            if (castling[WHITE] & BITS.QSIDE_CASTLE) { cflags += 'Q'; }
            if (castling[BLACK] & BITS.KSIDE_CASTLE) { cflags += 'k'; }
            if (castling[BLACK] & BITS.QSIDE_CASTLE) { cflags += 'q'; }

            /* do we have an empty castling flag? */
            cflags = cflags || '-';
            var epflags = (ep_square === EMPTY) ? '-' : algebraic(ep_square);

            return [fen, turn, cflags, epflags, half_moves, move_number].join(' ');
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
    }
}

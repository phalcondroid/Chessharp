using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Chessharp.Core.Structures;

namespace Chessharp.Core
{
    public class Core
    {
        string FEN;

        string BLACK = "b";
        public string WHITE = "w";

        int EMPTY { get; set; } = -1;

        string PAWN = "p";
        string KNIGHT = "n";
        string BISHOP = "b";
        string ROOK = "r";
        string QUEEN = "q";
        string KING = "k";

        string SYMBOLS = "pnbrqkPNBRQK";
        string DEFAULT_POSITION = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        string[] POSSIBLE_RESULTS = { "1-0", "0-1", "1/2-1/2", "*" };


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
        private Dictionary<string, int[]> PIECE_OFFSETS = new Dictionary<string, int[]>();


        private int[] ATTACKS = {
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

        private int[] RAYS = {
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
        public Dictionary<string, int> SHIFTS = new Dictionary<string, int>();

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
        private Dictionary<string, string> FLAGS = new Dictionary<string, string>();

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
        public Dictionary<string, int> BITS = new Dictionary<string, int>();

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
        public Dictionary<string, int> SQUARES = new Dictionary<string, int>();

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
        RookStruct ROOKS;

        /**
         * var board = new Array(128);
         */
        public Dictionary<string, string>[] BOARD = new Dictionary<string, string>[128];

        /**
         * var kings = {w: EMPTY, b: EMPTY};
         */
        Dictionary<string, int> KINGS = new Dictionary<string, int>();


        Dictionary<string, int> CASTLING = new Dictionary<string, int>();

        public string TURN;

        int EPSQUARE;

        public string HALFMOVES = "0";

        public string MOVENUMBER = "1";

        public List<History> HISTORY = new List<History>();

        Dictionary<string, string> HEADER = new Dictionary<string, string>();

        public void InitRooks()
        {
            ROOKS = new RookStruct(SQUARES, BITS);
        }

        public void InitPawnOffsets()
        {
            PAWN_OFFSETS.Add("b", new int[] { 16, 32, 17, 15 });
            PAWN_OFFSETS.Add("w", new int[] { -16, -32, -17, -15 });
        }

        public void InitPieceOffsets()
        {
            PIECE_OFFSETS.Add("n", new int[] { -18, -33, -31, -14, 18, 33, 31, 14 });
            PIECE_OFFSETS.Add("b", new int[] { -17, -15, 17, 15 });
            PIECE_OFFSETS.Add("r", new int[] { -16, 1, 16, -1 });
            PIECE_OFFSETS.Add("q", new int[] { -17, -16, -15, 1, 17, 16, 15, -1 });
            PIECE_OFFSETS.Add("k", new int[] { -17, -16, -15, 1, 17, 16, 15, -1 });
        }

        public void InitShifts()
        {
            SHIFTS.Add("p", 0);
            SHIFTS.Add("n", 1);
            SHIFTS.Add("b", 2);
            SHIFTS.Add("r", 3);
            SHIFTS.Add("q", 4);
            SHIFTS.Add("k", 5);
        }

        public void InitFlags()
        {
            FLAGS.Add("NORMAL", "n");
            FLAGS.Add("CAPTURE", "c");
            FLAGS.Add("BIG_PAWN", "b");
            FLAGS.Add("EP_CAPTURE", "e");
            FLAGS.Add("PROMOTION", "p");
            FLAGS.Add("KSIDE_CASTLE", "k");
            FLAGS.Add("QSIDE_CASTLE", "q");
        }

        public void InitBits()
        {
            BITS.Add("NORMAL", 1);
            BITS.Add("CAPTURE", 2);
            BITS.Add("BIG_PAWN", 4);
            BITS.Add("EP_CAPTURE", 8);
            BITS.Add("PROMOTION", 16);
            BITS.Add("KSIDE_CASTLE", 32);
            BITS.Add("QSIDE_CASTLE", 64);
        }

        public void InitSquares()
        {
            SQUARES.Add("a8", 0);
            SQUARES.Add("b8", 1);
            SQUARES.Add("c8", 2);
            SQUARES.Add("d8", 3);
            SQUARES.Add("e8", 4);
            SQUARES.Add("f8", 5);
            SQUARES.Add("g8", 6);
            SQUARES.Add("h8", 7);

            SQUARES.Add("a7", 16);
            SQUARES.Add("b7", 17);
            SQUARES.Add("c7", 18);
            SQUARES.Add("d7", 19);
            SQUARES.Add("e7", 20);
            SQUARES.Add("f7", 21);
            SQUARES.Add("g7", 22);
            SQUARES.Add("h7", 23);

            SQUARES.Add("a6", 32);
            SQUARES.Add("b6", 33);
            SQUARES.Add("c6", 34);
            SQUARES.Add("d6", 35);
            SQUARES.Add("e6", 36);
            SQUARES.Add("f6", 37);
            SQUARES.Add("g6", 38);
            SQUARES.Add("h6", 39);

            SQUARES.Add("a5", 48);
            SQUARES.Add("b5", 49);
            SQUARES.Add("c5", 50);
            SQUARES.Add("d5", 51);
            SQUARES.Add("e5", 52);
            SQUARES.Add("f5", 53);
            SQUARES.Add("g5", 54);
            SQUARES.Add("h5", 55);

            SQUARES.Add("a4", 64);
            SQUARES.Add("b4", 65);
            SQUARES.Add("c4", 66);
            SQUARES.Add("d4", 67);
            SQUARES.Add("e4", 68);
            SQUARES.Add("f4", 69);
            SQUARES.Add("g4", 70);
            SQUARES.Add("h4", 71);

            SQUARES.Add("a3", 80);
            SQUARES.Add("b3", 81);
            SQUARES.Add("c3", 82);
            SQUARES.Add("d3", 83);
            SQUARES.Add("e3", 84);
            SQUARES.Add("f3", 85);
            SQUARES.Add("g3", 86);
            SQUARES.Add("h3", 87);

            SQUARES.Add("a2", 96);
            SQUARES.Add("b2", 97);
            SQUARES.Add("c2", 98);
            SQUARES.Add("d2", 99);
            SQUARES.Add("e2", 100);
            SQUARES.Add("f2", 101);
            SQUARES.Add("g2", 102);
            SQUARES.Add("h2", 103);

            SQUARES.Add("a1", 112);
            SQUARES.Add("b1", 113);
            SQUARES.Add("c1", 114);
            SQUARES.Add("d1", 115);
            SQUARES.Add("e1", 116);
            SQUARES.Add("f1", 117);
            SQUARES.Add("g1", 118);
            SQUARES.Add("h1", 119);
        }

        public void InitCastling()
        {
            CASTLING.Add("w", 0);
            CASTLING.Add("b", 0);
        }

        public void InitKings()
        {
            KINGS.Add("w", EMPTY);
            KINGS.Add("b", EMPTY);
        }

        public Core(string fen)
        {
            WHITE = "w";
            FEN = fen;

            TURN = WHITE;
            EPSQUARE = EMPTY;

            InitPawnOffsets();
            InitPieceOffsets();
            InitShifts();
            InitFlags();
            InitBits();
            InitSquares();
            InitRooks();
            InitCastling();
            InitKings();

            Load(fen, false);
        }

        public Core()
        {
            WHITE = "w";
            FEN = "";

            TURN = WHITE;
            //Console.WriteLine("turn to {0} c", TURN);
            EPSQUARE = EMPTY;

            InitPawnOffsets();
            InitPieceOffsets();
            InitShifts();
            InitFlags();
            InitBits();
            InitSquares();
            InitRooks();
            InitCastling();
            InitKings();

            Load(DEFAULT_POSITION, false);
        }

        /* called when the initial board setup is changed with put() or remove().
           * modifies the SetUp and FEN properties of the header object.  if the FEN is
           * equal to the default position, the SetUp and FEN are deleted
           * the setup is only updated if history.length is zero, ie moves haven't been
           * made.
           */
        public void UpdateSetup(string fen)
        {
            if (HISTORY.Count > 0)
            {
                return;
            }

            if (fen != DEFAULT_POSITION)
            {
                HEADER["SetUp"] = "1";
                HEADER["FEN"] = fen;
            }
            else
            {
                HEADER.Remove("SetUp");
                HEADER.Remove("FEN");
            }
        }

        public void InitializeBoard()
        {
            for (int i = 0; i < 128; i++)
            {
                BOARD[i] = new Dictionary<string, string>();
            }
        }

        public void Clear(bool keepHeaders)
        {
            if (keepHeaders == false)
                keepHeaders = false;

            KINGS = new Dictionary<string, int>() {
                { "w", EMPTY },
                { "b", EMPTY }
            };

            TURN = WHITE;
            //Console.WriteLine("turn to {0} c", TURN);

            CASTLING = new Dictionary<string, int>() {
                { "w", 0 },
                { "b", 0 }
            };

            EPSQUARE = EMPTY;
            HALFMOVES = "0";
            MOVENUMBER = "1";
            HISTORY = new List<History>();

            if (!keepHeaders)
                HEADER = new Dictionary<string, string>();

            UpdateSetup(
                GenerateFen()
            );
        }

        public bool Load(string fen)
        {
            return Load(fen, false);
        }

        public bool Load(string fen, bool keepHeaders)
        {
            string[] tokens = Regex.Split(fen, @"\s+");
            string position = tokens[0];
            int square = 0;

            Dictionary<string, string> validateFenResult = ValidateFen(fen);

            if (validateFenResult["valid"] == "false")
                return false;

            Clear(keepHeaders);

            for (var i = 0; i < position.Length; i++)
            {
                string piece = position[i].ToString();

                if (piece.Equals("/"))
                {
                    square += 8;
                }
                else if (IsDigit(piece))
                {
                    square += Int32.Parse(piece);
                }
                else
                {
                    //7int cond1 = string.Compare(piece, "a");
                    //string color = (cond1 == -1) ? WHITE : BLACK;
                    char[] colorChar = piece.ToCharArray();
                    string color = colorChar[0] < 'a' ? WHITE : BLACK;

                    Dictionary<string, string> putParams = new Dictionary<string, string>() {
                        { "type",  piece.ToLower()},
                        { "color", color }
                    };

                    //Console.WriteLine("square : " + square);
                    if (square >= 0)
                    {
                        bool put = Put(putParams, Algebraic(square));
                    }
                    //if (!put)
                    //Console.WriteLine("paila");

                    square++;
                }
            }

            TURN = tokens[1];
            //Console.WriteLine("turn to {0} l", TURN);
            //Console.WriteLine(CASTLING["w"]);
            //Console.WriteLine(CASTLING["b"]);

            if (tokens[2].IndexOf('K') > -1)
                CASTLING["w"] |= BITS["KSIDE_CASTLE"];

            if (tokens[2].IndexOf('Q') > -1)
                CASTLING["w"] |= BITS["QSIDE_CASTLE"];

            if (tokens[2].IndexOf('k') > -1)
                CASTLING["b"] |= BITS["KSIDE_CASTLE"];

            if (tokens[2].IndexOf('q') > -1)
                CASTLING["b"] |= BITS["QSIDE_CASTLE"];

            //Console.WriteLine(CASTLING["w"]);
            //Console.WriteLine(CASTLING["b"]);

            EPSQUARE = (tokens[3] == "-") ? EMPTY : SQUARES[tokens[3]];
            HALFMOVES = tokens[4];
            MOVENUMBER = tokens[5];

            UpdateSetup(
                GenerateFen()
            );

            return true;
        }

        public bool Put(Dictionary<string, string> piece, string square)
        {
            if (!(piece.ContainsKey("type") && piece.ContainsKey("color")))
                return false;

            if (SYMBOLS.IndexOf(piece["type"].ToLower()) == -1)
                return false;

            if (!SQUARES.ContainsKey(square))
                return false;

            int sq = SQUARES[square];

            /* don't let the user place more than one king */
            if (piece["type"] == KING && !(KINGS[piece["color"]] == EMPTY || KINGS[piece["color"]] == sq))
                return false;

            BOARD[sq] = new Dictionary<string, string>() {
                { "type",  piece["type"]},
                { "color", piece["color"]}
            };

            if (piece["type"] == KING)
                KINGS[piece["color"]] = sq;

            UpdateSetup(
                GenerateFen()
            );

            return true;
        }

        public string GenerateFen()
        {
            int empty = 0;
            string fen = "";

            for (var i = SQUARES["a8"]; i <= SQUARES["h1"]; i++)
            {
                if (BOARD[i] == null)
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
                    string color = BOARD[i]["color"];
                    string piece = BOARD[i]["type"];

                    fen += (color == WHITE) ? piece.ToUpper() : piece.ToLower();
                }

                if (((i + 1) & 0x88) != 0)
                {
                    if (empty > 0)
                    {
                        fen += empty;
                    }

                    if (i != SQUARES["h1"])
                    {
                        fen += '/';
                    }

                    empty = 0;
                    i += 8;
                }
            }

            string cflags = " ";
            if ((CASTLING[WHITE] & BITS["KSIDE_CASTLE"]) != 0) { cflags += "K"; }
            if ((CASTLING[WHITE] & BITS["QSIDE_CASTLE"]) != 0) { cflags += "Q"; }
            if ((CASTLING[BLACK] & BITS["KSIDE_CASTLE"]) != 0) { cflags += "k"; }
            if ((CASTLING[BLACK] & BITS["QSIDE_CASTLE"]) != 0) { cflags += "q"; }

            /* do we have an empty castling flag? */
            //cflags = cflags || "-";
            cflags = !string.IsNullOrEmpty(cflags) ? cflags : "-";
            var epflags = (EPSQUARE == EMPTY) ? "-" : Algebraic(EPSQUARE);

            string[] result = {
                fen,
                TURN,
                cflags,
                epflags,
                HALFMOVES,
                MOVENUMBER
            };

            return String.Join(" ", result);
        }

        public string Algebraic(int i)
        {
            int f = File(i);
            int r = Rank(i);
            string abc = "abcdefgh";
            string nums = "87654321";

            string abcSub = abc.Substring(f, 1);
            string numSub = nums.Substring(r, 1);
            string result = abcSub + numSub;
            return result;
        }

        public int Rank(int i)
        {
            return (i >> 4);
        }

        public int File(int i)
        {
            return i & 15;
        }

        public string SwapColor(string c)
        {
            return c.ToLower() == WHITE ? BLACK : WHITE;
        }

        public bool IsDigit(string c)
        {
            return "0123456789".IndexOf(c, StringComparison.Ordinal) != -1;
        }

        public Dictionary<string, string> ValidateFen(string fenVar)
        {
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
            string[] tokens = Regex.Split(fenVar, @"\s+");

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
            bool condition2 = int.TryParse(tokens[5], out int token5);

            if (!condition2 || (token5 <= 0))
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
            bool condition3 = int.TryParse(tokens[4], out int token4);

            if (!condition3 || (token4 < 0))
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
            Regex regex = new Regex(@"^(-|[abcdefgh][36])$");
            Match match = regex.Match(tokens[3]);

            if (!match.Success)
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
            Regex regex2 = new Regex(@"^(KQ?k?q?|Qk?q?|kq?|q|-)$");
            Match match2 = regex2.Match(tokens[2]);
            if (!match2.Success)
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
            Regex regex3 = new Regex(@"^(w|b)$");
            Match match3 = regex3.Match(tokens[1]);
            if (!match3.Success)
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

            string[] rows = tokens[0].Split("/");
            if (rows.Length != 8)
            {
                Dictionary<string, string> result = new Dictionary<string, string>
                {
                    { "valid", "false" },
                    { "error_number", "7" },
                    { "errors", errors[7] }
                };
                return result;
            }

            for (int i = 0; i < rows.Length; i++)
            {
                int sum_fields = 0;
                bool previousWasNumber = false;

                for (int k = 0; k < rows[i].Length; k++)
                {
                    bool isNan = int.TryParse(rows[i][k].ToString(), out int rowNum);

                    if (isNan)
                    {
                        if (previousWasNumber)
                        {
                            Dictionary<string, string> result = new Dictionary<string, string>
                            {
                                { "valid", "false" },
                                { "error_number", "8" },
                                { "errors", errors[8] }
                            };
                            return result;
                        }
                        sum_fields += rowNum;
                        previousWasNumber = true;
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
                        previousWasNumber = false;
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

            if (tokens.Length > 3 && tokens[3].Length > 1)
            {
                if ((tokens[3][1] == '3' && tokens[1] == "w") || (tokens[3][1] == '6' && tokens[1] == "b"))
                {
                    Dictionary<string, string> result = new Dictionary<string, string>
                    {
                         { "valid", "false" },
                         { "error_number", "11" },
                         { "errors", errors[11] }
                    };

                    return result;
                }
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
        public bool Reset()
        {
            return Load(DEFAULT_POSITION, false);
        }

        /*
         * 
         */
        public Dictionary<string, string> SetHeader(Dictionary<int, string> args)
        {
            for (int i = 0; i < args.Count; i += 2)
            {
                if (args[i] is "string" && args[i + 1] == "string")
                {
                    HEADER[args[i]] = args[i + 1];
                }
            }
            return HEADER;
        }

        public Dictionary<string, string> Get(string square)
        {
            Dictionary<string, string> piece = BOARD[SQUARES[square]];
            return piece == null ? new Dictionary<string, string>() {
                { "type", piece["type"] },
                { "color", piece["color"] }
            } : null;
        }

        public Dictionary<string, string> Remove(string square)
        {
            Dictionary<string, string> piece = Get(square);
            BOARD[SQUARES[square]] = null;
            if (piece != null && piece["type"] == KING)
            {
                KINGS[piece["color"]] = EMPTY;
            }

            UpdateSetup(
                GenerateFen()
            );

            return piece;
        }

        public Move BuildMove(Dictionary<string, string>[] localBoard, string from, string to, int flags, string promotion)
        {
            Dictionary<string, string> boardFrom = localBoard[Convert.ToInt32(from)];

            Move move = new Move();
            move.Color = TURN;
            move.From = from;
            move.To = to;
            move.Flags = flags.ToString();
            move.Piece = boardFrom["type"];

            if (promotion != null && promotion != "")
            {
                int moveInt = Convert.ToInt32(move.Flags);
                moveInt |= BITS["PROMOTION"];
                move.Flags = moveInt.ToString();
                move.Promotion = promotion;
            }
            Dictionary<string, string> boardTo = localBoard[Convert.ToInt32(to)];
            if (boardTo != null)
            {
                move.Captured = boardTo["type"];
            }
            else if ((flags & BITS["EP_CAPTURE"]) != 0)
            {
                move.Captured = PAWN;
            }
            return move;
        }

        public void Push(Move move)
        {
            /*history.push({
                move:        move,
                kings:       { b: kings.b, w: kings.w},
                turn:        turn,
                castling:    { b: castling.b, w: castling.w},
                ep_square:   ep_square,
                half_moves:  half_moves,
                move_number: move_number
            });*/
            Dictionary<string, int> kingsLocal = new Dictionary<string, int>() {
                {"b", KINGS["b"] },
                {"w", KINGS["w"] }
            };
            Dictionary<string, int> castlingLocal = new Dictionary<string, int>() {
                {"b", CASTLING["b"] },
                {"w", CASTLING["w"] }
            };

            History history = new History
            {
                Move = move,
                Kings = kingsLocal,
                Turn = TURN,
                Castling = castlingLocal,
                EpSquare = EPSQUARE,
                HalfMoves = HALFMOVES,
                MoveNumber = MOVENUMBER
            };
            HISTORY.Add(history);
        }

        public void MakeMove(Move move)
        {
            string us = TURN;
            string them = SwapColor(us);
            Push(move);

            int moveTo = Convert.ToInt32(move.To);
            int moveFrom = Convert.ToInt32(move.From);

            BOARD[moveTo] = BOARD[moveFrom];
            BOARD[moveFrom] = null;

            /* if ep capture, remove the captured pawn */
            if ((Convert.ToInt32(move.Flags) & BITS["EP_CAPTURE"]) != 0)
            {
                if (TURN == BLACK)
                    BOARD[moveTo - 16] = null;
                else
                    BOARD[moveTo + 16] = null;
            }

            /* if pawn promotion, replace with new piece */
            if ((Convert.ToInt32(move.Flags) & BITS["PROMOTION"]) != 0)
            {
                BOARD[moveTo] = new Dictionary<string, string>() {
                    { "type", move.Promotion },
                    { "color", us}
                };
            }

            /* if we moved the king */
            Dictionary<string, string> boardMoveTo = BOARD[moveTo];
            if (boardMoveTo != null && boardMoveTo["type"] == KING)
            {
                KINGS[boardMoveTo["color"]] = moveTo;

                /* if we castled, move the rook next to the king */
                if ((Convert.ToInt32(move.Flags) & BITS["KSIDE_CASTLE"]) != 0)
                {
                    var castling_to = moveTo - 1;
                    var castling_from = moveTo + 1;
                    BOARD[castling_to] = BOARD[castling_from];
                    BOARD[castling_from] = null;
                }
                else if ((Convert.ToInt32(move.Flags) & BITS["QSIDE_CASTLE"]) != 0)
                {
                    var castlingTo = moveTo + 1;
                    var castlingFrom = moveTo - 2;
                    BOARD[castlingTo] = BOARD[castlingFrom];
                    BOARD[castlingFrom] = null;
                }

                /* turn off castling */
                //-100 == ""
                CASTLING[us] = 0;
            }

            /* turn off castling if we move a rook */
            if (CASTLING[us] > 0)
            {
                for (int i = 0, len = ROOKS.GetByColor(us).Length; i < len; i++)
                {
                    CValue[] rookByColor = ROOKS.GetByColor(us);
                    Dictionary<string, int> rookUsItem = rookByColor[i].GetDicStrInt();

                    if (rookUsItem.ContainsKey("square") && move.From == rookUsItem["square"].ToString() && rookUsItem.ContainsKey("flag") && (CASTLING[us] & rookUsItem["flag"]) != 0)
                    {
                        CASTLING[us] ^= rookUsItem["flag"];
                        break;
                    }
                }
            }

            /* turn off castling if we capture a rook */
            if (CASTLING[them] > 0)
            {
                for (int i = 0, len = ROOKS.GetByColor(them).Length; i < len; i++)
                {
                    CValue[] rookByColor = ROOKS.GetByColor(them);
                    Dictionary<string, int> rookThemItem = rookByColor[i].GetDicStrInt();

                    if (rookThemItem.ContainsKey("square") && moveTo == rookThemItem["square"] && rookThemItem.ContainsKey("flag") && (CASTLING[them] & rookThemItem["flag"]) != 0)
                    {
                        CASTLING[them] ^= rookThemItem["flag"];
                        break;
                    }
                }
            }

            /* if big pawn move, update the en passant square */
            int moveFlags = Convert.ToInt32(move.Flags);
            if ((moveFlags & BITS["BIG_PAWN"]) != 0)
            {
                if (TURN == "b")
                    EPSQUARE = moveTo - 16;
                else
                    EPSQUARE = moveTo + 16;
            }
            else
            {
                EPSQUARE = EMPTY;
            }

            /* reset the 50 move counter if a pawn is moved or a piece is captured */
            if (move.Piece == PAWN)
            {
                HALFMOVES = "0";
            }
            else if ((moveFlags & (BITS["CAPTURE"] | BITS["EP_CAPTURE"])) != 0)
            {
                HALFMOVES = "0";
            }
            else
            {
                int halfMovesInt = Convert.ToInt32(HALFMOVES);
                halfMovesInt++;
                HALFMOVES = halfMovesInt.ToString();
            }

            if (TURN == BLACK)
            {
                int moveNumberInt = Convert.ToInt32(MOVENUMBER);
                moveNumberInt++;
                MOVENUMBER = moveNumberInt.ToString();
            }

            TURN = SwapColor(TURN);
            //Console.WriteLine("turn to {0} m", TURN);
        }

        public List<Move> AddMove(Dictionary<string, string>[] localBoard, List<Move> moves, string from, string to, int flags)
        {
            /* if pawn promotion */

            Dictionary<string, string> boardFrom = localBoard[Convert.ToInt32(from)];
            int toInt = Convert.ToInt32(to);
            if (boardFrom["type"] == PAWN && (Rank(toInt) == RANK_8 || Rank(toInt) == RANK_1))
            {
                string[] pieces = { QUEEN, ROOK, BISHOP, KNIGHT };
                for (int i = 0, len = pieces.Length; i < len; i++)
                {
                    //risk by length
                    moves.Add(BuildMove(localBoard, from, to, flags, pieces[i]));
                }
            }
            else
            {
                moves.Add(BuildMove(localBoard, from, to, flags, null));
            }

            return moves;
        }

        public List<Move> GenerateMoves(Dictionary<string, string> options)
        {
            string us = TURN;
            string them = SwapColor(us);
            Dictionary<string, int> secondRank = new Dictionary<string, int>() { { "b", RANK_7 }, { "w", RANK_2 } };

            //try
            //{
            int firstSq = SQUARES["a8"];
            int lastSq = SQUARES["h1"];
            bool singleSquare = false;

            /* do we want legal moves? */
            bool legal = (options != null && options.ContainsKey("legal")) ? options["legal"].ToLower() == "true" ? true : false : true;

            /* are we generating moves for a single square? */
            if (options != null && options.ContainsKey("square"))
            {
                if (SQUARES.ContainsKey(options["square"].ToString()))
                {
                    firstSq = lastSq = SQUARES[options["square"].ToString()];
                    singleSquare = true;
                }
                else
                {
                    /* invalid square */
                    return new List<Move>();
                }
            }

            List<Move> moves = new List<Move>();

            for (int i = firstSq; i <= lastSq; i++)
            {
                /* did we run off the end of the board */
                if ((i & 0x88) != 0)
                {
                    i += 7;
                    continue;
                }

                Dictionary<string, string> piece = BOARD[i];

                if (piece == null || piece["color"] != us)
                    continue;

                //Console.WriteLine(i + " " + piece["type"] + " " + piece["color"]);

                if (piece["type"] == PAWN)
                {
                    /* single square, non-capturing */
                    int square = i + PAWN_OFFSETS[us][0];
                    if (BOARD[square] == null)
                    {
                        moves = AddMove(BOARD, moves, i.ToString(), square.ToString(), BITS["NORMAL"]);
                        //Console.WriteLine("normal: {0}", moves.Count);

                        /* double square */
                        square = i + PAWN_OFFSETS[us][1];
                        if (secondRank[us] == Rank(i) && BOARD[square] == null)
                        {
                            moves = AddMove(BOARD, moves, i.ToString(), square.ToString(), BITS["BIG_PAWN"]);
                            //Console.WriteLine("big-pawn: {0}", moves.Count);
                        }
                    }

                    /* pawn captures */
                    for (int j = 2; j < 4; j++)
                    {
                        square = i + PAWN_OFFSETS[us][j];
                        if ((square & 0x88) != 0)
                            continue;

                        Dictionary<string, string> boardSquare = BOARD[square];
                        if (boardSquare != null && boardSquare["color"] == them)
                        {
                            moves = AddMove(BOARD, moves, i.ToString(), square.ToString(), BITS["CAPTURE"]);
                            //Console.WriteLine("capture: {0}", moves.Count);
                        }
                        else if (square == EPSQUARE)
                        {
                            moves = AddMove(BOARD, moves, i.ToString(), EPSQUARE.ToString(), BITS["EP_CAPTURE"]);
                            //Console.WriteLine("ep-capture: {0}", moves.Count);
                        }
                    }
                }
                else
                {
                    for (int j = 0, len2 = PIECE_OFFSETS[piece["type"]].Length; j < len2; j++)
                    {
                        int offset = PIECE_OFFSETS[piece["type"]][j];
                        int square = i;

                        while (true)
                        {
                            square += offset;
                            if ((square & 0x88) != 0)
                                break;

                            Dictionary<string, string> boardSquare = BOARD[square];
                            if (boardSquare == null)
                            {
                                moves = AddMove(BOARD, moves, i.ToString(), square.ToString(), BITS["NORMAL"]);
                                //Console.WriteLine("normal-2: {0}", moves.Count);
                            }
                            else
                            {
                                if (boardSquare["color"] == us)
                                    break;

                                moves = AddMove(BOARD, moves, i.ToString(), square.ToString(), BITS["CAPTURE"]);
                                //Console.WriteLine("capture-2: {0}", moves.Count);
                                break;
                            }

                            /* break, if knight or king */
                            if (piece["type"] == "n" || piece["type"] == "k")
                                break;
                        }
                    }
                }
            }

            //Console.WriteLine("last_sq: {0}", lastSq);
            //Console.WriteLine("last_sq: {0}", KINGS[us]);
            //Console.WriteLine("single_square: {0}", singleSquare);

            /* check for castling if: a) we're generating all moves, or b) we're doing
             * single square move generation on the king's square
             */
            if ((!singleSquare) || lastSq == KINGS[us])
            {
                //Console.WriteLine("in castling: {0}", (CASTLING[us] & BITS["KSIDE_CASTLE"]));

                /* king-side castling */
                if ((CASTLING[us] & BITS["KSIDE_CASTLE"]) != 0)
                {
                    int castlingFrom = KINGS[us];
                    int castlingTo = castlingFrom + 2;

                    /*Console.WriteLine("castlingFrom {0}", castlingFrom + 1);
                    Console.WriteLine("BOARD[castlingTo] {0}", BOARD[castlingTo]);
                    Console.WriteLine("!Attacked(them, KINGS[us]) {0}", !Attacked(them, KINGS[us]));
                    Console.WriteLine("!Attacked(them, castlingFrom + 1) {0}", !Attacked(them, castlingFrom + 1));
                    Console.WriteLine("!Attacked(them, castlingTo) {0}", !Attacked(them, castlingTo));
                    Console.WriteLine("condichon ({0})", BOARD[castlingFrom + 1] == null && BOARD[castlingTo] == null && !Attacked(them, KINGS[us]) && !Attacked(them, castlingFrom + 1) && !Attacked(them, castlingTo));
                    */

                    if (BOARD[castlingFrom + 1] == null && BOARD[castlingTo] == null && !Attacked(them, KINGS[us]) && !Attacked(them, castlingFrom + 1) && !Attacked(them, castlingTo))
                    {
                        moves = AddMove(BOARD, moves, KINGS[us].ToString(), castlingTo.ToString(), BITS["KSIDE_CASTLE"]);
                        //Console.WriteLine("k-side castle: {0}", moves.Count);
                    }
                }

                /* queen-side castling */
                if ((CASTLING[us] & BITS["QSIDE_CASTLE"]) != 0)
                {
                    int castlingFrom = KINGS[us];
                    int castlingTo = castlingFrom - 2;

                    if (BOARD[castlingFrom - 1] == null && BOARD[castlingFrom - 2] == null && BOARD[castlingFrom - 3] == null && !Attacked(them, KINGS[us]) && !Attacked(them, castlingFrom - 1) && !Attacked(them, castlingTo))
                    {
                        moves = AddMove(BOARD, moves, KINGS[us].ToString(), castlingTo.ToString(), BITS["QSIDE_CASTLE"]);
                        //Console.WriteLine("q-side castle: {0}", moves.Count);
                    }
                }
            }
            /*for (int i = 0; ) {

            }
            string s = string.Join(";", moves);*/

            if (!legal)
                return moves;

            //Console.WriteLine(moves.Count);

            /* filter out illegal moves */
            List<Move> legalMoves = new List<Move>();
            for (int i = 0, len3 = moves.Count; i < len3; i++)
            {
                MakeMove(moves[i]);
                if (!KingAttacked(us))
                    legalMoves.Add(moves[i]);

                UndoMove();
            }

            return legalMoves;
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message + " 2");
            //    return null;
            //}
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
        public Move MoveFromSan(string move, bool sloppy)
        {
            MatchCollection matches;
            // strip off any move decorations: e.g Nf3+?!
            string cleanMove = StrippedSan(move);

            string piece = "";
            string from = "";
            string to = "";
            string promotion = "";

            Regex matchCondition = new Regex(@"([pnbrqkPNBRQK])?([a-h][1-8])x?-?([a-h][1-8])([qrbnQRBN])?");
            matches = Regex.Matches(cleanMove, @"([pnbrqkPNBRQK])?([a-h][1-8])x?-?([a-h][1-8])([qrbnQRBN])?");

            // if we're using the sloppy parser run a regex to grab piece, to, and from
            // this should parse invalid SAN like: Pe2-e4, Rc1c4, Qf3xf7
            if (sloppy)
            {
                //var matches = cleanMove.match(/ ([pnbrqkPNBRQK]) ? ([a - h][1 - 8])x ? -? ([a - h][1 - 8])([qrbnQRBN]) ?/);
                if (matchCondition.IsMatch(cleanMove))
                {
                    piece = matches[1].Value;
                    from = matches[2].Value;
                    to = matches[3].Value;
                    promotion = matches[4].Value;

                }
            }

            List<Move> moves = GenerateMoves(null);

            for (int i = 0, len = moves.Count; i < len; i++)
            {
                // try the strict parser first, then the sloppy parser if requested
                // by the user
                if ((cleanMove == StrippedSan(MoveToSan(moves[i], false))) || (sloppy && cleanMove == StrippedSan(MoveToSan(moves[i], true))))
                {
                    return moves[i];
                }
                Move moveItem = moves[i];


                int squaresFrom = -1;
                if (from != "")
                {
                    squaresFrom = SQUARES[from];
                }

                int squaresTo = -1;
                if (to != "")
                {
                    squaresTo = SQUARES[to];
                }

                if (matches.Count != 0 && (piece == "" || piece.ToLower() == moveItem.Piece) && squaresFrom == Convert.ToInt32(moveItem.From) && squaresTo == Convert.ToInt32(moveItem.To) && (promotion == "" || promotion.ToLower() == moveItem.Promotion))
                {
                    return moveItem;
                }
            }

            return null;
        }

        public string MoveToSan(Move move, bool sloppy)
        {
            string output = "";
            int moveFlagsInt = Convert.ToInt32(move.Flags);
            int moveFromInt = Convert.ToInt32(move.From);
            int moveFromTo = Convert.ToInt32(move.To);

            if ((moveFlagsInt & BITS["KSIDE_CASTLE"]) != 0)
            {
                output = "O-O";
            }
            else if ((moveFlagsInt & BITS["QSIDE_CASTLE"]) != 0)
            {
                output = "O-O-O";
            }
            else
            {
                string disambiguator = GetDisambiguator(move, sloppy);

                if (move.Piece != PAWN)
                {
                    output += move.Piece.ToUpper() + disambiguator;
                }

                if ((moveFlagsInt & ((BITS["CAPTURE"] | BITS["EP_CAPTURE"]))) != 0)
                {
                    if (move.Piece == PAWN)
                    {
                        string algebraic = Algebraic(moveFromInt);
                        output += algebraic[0];
                    }
                    output += "x";
                }

                output += Algebraic(moveFromTo);

                if ((moveFlagsInt & BITS["PROMOTION"]) != 0)
                {
                    output += "=" + move.Promotion.ToUpper();
                }
            }

            MakeMove(move);

            //Console.WriteLine("in check ({0})", InCheck().ToString().ToLower());

            if (InCheck())
            {
                //Console.WriteLine("in checkmate ({0})", InCheckmate().ToString().ToLower());

                if (InCheckmate())
                    output += "#";
                else
                    output += "+";
            }

            UndoMove();

            return output;
        }

        public bool KingAttacked(string color)
        {
            string col = SwapColor(color);
            //Console.WriteLine("swap color: {0} kinks[color] {1}", col, KINGS[color]);
            return Attacked(col, KINGS[color]);
        }

        public bool InCheck()
        {
            return KingAttacked(TURN);
        }

        public bool InCheckmate()
        {
            //Console.WriteLine("in checkmate");
            List<Move> generateMoves = GenerateMoves(null);
            return InCheck() && ((generateMoves == null) || (generateMoves != null && generateMoves.Count == 0));
        }

        public bool InStalemate()
        {
            //Console.WriteLine("check stale");
            List<Move> generateMoves = GenerateMoves(null);
            return !InCheck() && ((generateMoves == null) || (generateMoves != null && generateMoves.Count == 0));
        }

        public bool InThreefoldRepetition()
        {
            List<Move> moves = new List<Move>();
            Dictionary<string, int> positions = new Dictionary<string, int>();
            bool repetition = false;

            while (true)
            {
                Move move = UndoMove();
                if (move == null)
                    break;

                moves.Add(move);
            }

            while (true)
            {
                string generateFen = GenerateFen();
                List<string> fenSplit = new List<string>(generateFen.Split(" "));
                string finalFen = string.Join(" ", fenSplit.GetRange(2, 2));

                positions.Add(finalFen, (positions.ContainsKey(finalFen)) ? positions[finalFen] + 1 : 1);

                if (positions[finalFen] >= 3)
                    repetition = true;

                if (moves.Count == 0)
                    break;

                Move move = moves[moves.Count - 1];
                moves.RemoveAt(moves.Count - 1);
                MakeMove(move);
            }

            return repetition;
        }

        public string GetDisambiguator(Move move, bool sloppy)
        {
            Dictionary<string, string> options = new Dictionary<string, string>() {
                { "legal",  (!sloppy).ToString() }
            };
            List<Move> moves = GenerateMoves(options);

            string from = move.From;
            string to = move.To;
            string piece = move.Piece;

            int ambiguities = 0;
            int sameRank = 0;
            int sameFile = 0;

            for (int i = 0, len = moves.Count; i < len; i++)
            {
                Move movesI = moves[i];
                string ambigFrom = movesI.From;
                string ambigTo = movesI.To;
                string ambigPiece = movesI.Piece;

                /* if a move of the same piece type ends on the same to square, we'll
                 * need to add a disambiguator to the algebraic notation
                 */
                if (piece == ambigPiece && from != ambigFrom && to == ambigTo)
                {
                    ambiguities++;
                    int fromInt = Convert.ToInt32(from);
                    int ambigFromInt = Convert.ToInt32(ambigFrom);

                    if (Rank(fromInt) == Rank(ambigFromInt))
                    {
                        sameRank++;
                    }

                    if (File(fromInt) == File(ambigFromInt))
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
                    return Algebraic(fromInt);
                }
                /* if the moving piece rests on the same file, use the rank symbol as the
                 * disambiguator
                 */
                else if (sameFile > 0)
                {
                    string result = Algebraic(fromInt);
                    return result[1].ToString();
                }
                /* else use the file symbol */
                else
                {
                    string result = Algebraic(fromInt);
                    return result[0].ToString();
                }
            }

            return "";
        }

        public Move UndoMove()
        {
            History oldHistory = null;

            if (HISTORY.Count > 0)
            {
                oldHistory = HISTORY[HISTORY.Count - 1];
                HISTORY.RemoveAt(HISTORY.Count - 1);
            }

            if (oldHistory == null)
                return null;

            Move move = oldHistory.Move;
            KINGS = oldHistory.Kings;
            TURN = oldHistory.Turn;
            //Console.WriteLine("turn to {0} u", TURN);

            CASTLING = oldHistory.Castling;
            EPSQUARE = oldHistory.EpSquare;
            HALFMOVES = oldHistory.HalfMoves;
            MOVENUMBER = oldHistory.MoveNumber;

            var us = TURN;
            var them = SwapColor(TURN);

            int moveFrom = Convert.ToInt32(move.From);
            int moveTo = Convert.ToInt32(move.To);
            int moveFlags = Convert.ToInt32(move.Flags);

            //Console.WriteLine("move.to {0}", move.To);
            //Console.WriteLine("move.from {0}",  move.From);

            //Console.WriteLine("board[move.from] {0}", BOARD[moveFrom]);
            //Console.WriteLine("board[move.to] type {0} - color {1}", BOARD[moveTo]["type"], BOARD[moveTo]["color"]);

            //try
            //{
            //if not more posible moves
            BOARD[moveFrom] = BOARD[moveTo];

            if (BOARD[moveTo] == null)
                throw new Exception("lol 0");

            if (BOARD[moveFrom] == null)
                throw new Exception("lol 1");

            if (move == null)
                throw new Exception("lol 3");

            if (move.Piece == null)
                throw new Exception("lol 2");

            BOARD[moveFrom]["type"] = move.Piece; // to undo any promotions
            BOARD[moveTo] = null;

            if ((moveFlags & BITS["CAPTURE"]) != 0)
            {
                BOARD[moveTo] = new Dictionary<string, string>() {
                    { "type",  move.Captured },
                    { "color", them }
                };
            }
            else if ((moveFlags & BITS["EP_CAPTURE"]) != 0)
            {
                int index;
                if (us == BLACK)
                {
                    index = moveTo - 16;
                }
                else
                {
                    index = moveTo + 16;
                }
                BOARD[index] = new Dictionary<string, string>() {
                    { "type",  PAWN },
                    { "color", them }
                };
            }

            int moveFlagsInt = Convert.ToInt32(move.Flags);

            if ((moveFlagsInt & (BITS["KSIDE_CASTLE"] | BITS["QSIDE_CASTLE"])) != 0)
            {
                int castlingTo = 0;
                int castlingFrom = 0;

                if ((moveFlags & BITS["KSIDE_CASTLE"]) != 0)
                {
                    castlingTo = moveTo + 1;
                    castlingFrom = moveTo - 1;
                }
                else if ((moveFlags & BITS["QSIDE_CASTLE"]) != 0)
                {
                    castlingTo = moveTo - 2;
                    castlingFrom = moveTo + 1;
                }

                BOARD[castlingTo] = BOARD[castlingFrom];
                BOARD[castlingFrom] = null;
            }

            return move;
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message + " 1");
            //    return null;
            //}

        }

        public bool Attacked(string color, int square)
        {
            for (int i = SQUARES["a8"]; i <= SQUARES["h1"]; i++)
            {
                /* did we run off the end of the board */
                if ((i & 0x88) != 0)
                {
                    i += 7;
                    continue;
                }

                /* if empty square or wrong color */

                Dictionary<string, string> piece = BOARD[i];
                if (BOARD[i] == null || piece["color"] != color) continue;

                int difference = i - square;
                int index = difference + 119;

                if ((ATTACKS[index] & (1 << SHIFTS[piece["type"]])) != 0)
                {
                    if (piece["type"] == PAWN)
                    {
                        if (difference > 0)
                        {
                            if (piece["color"] == WHITE) return true;
                        }
                        else
                        {
                            if (piece["color"] == BLACK) return true;
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
                        if (BOARD[j] != null)
                        {
                            blocked = true; break;
                        }
                        j += offset;
                    }

                    if (!blocked) return true;
                }
            }

            return false;
        }

        public bool InsufficientMaterial()
        {
            Dictionary<string, int> pieces = new Dictionary<string, int>();
            List<int> bishops = new List<int>();
            int numPieces = 0;
            int sqColor = 0;

            for (int i = SQUARES["a8"]; i <= SQUARES["h1"]; i++)
            {
                sqColor = (sqColor + 1) % 2;
                if ((i & 0x88) != 0)
                {
                    i += 7; continue;
                }

                Dictionary<string, string> piece = BOARD[i];
                if (piece != null)
                {
                    pieces[piece["type"]] = (pieces.ContainsKey(piece["type"])) ? Convert.ToInt32(pieces[piece["type"]]) + 1 : 1;
                    if (piece["type"] == BISHOP)
                    {
                        bishops.Add(sqColor);
                    }
                    numPieces++;
                }
            }

            /* k vs. k */
            if (numPieces == 2) { return true; }
            /* k vs. kn .... or .... k vs. kb */
            else if (numPieces == 3 && (pieces[BISHOP] == 1 || pieces[KNIGHT] == 1))
            {
                return true;
            }

            /* kb vs. kb where any number of bishops are all on the same color */
            else if (numPieces == pieces[BISHOP] + 2)
            {
                int sum = 0;
                int len = bishops.Count;
                for (var i = 0; i < len; i++)
                {
                    sum += bishops[i];
                }
                if (sum == 0 || sum == len)
                {
                    return true;
                }
            }

            return false;
        }

        public string Ascii()
        {
            string s = "   +------------------------+\n";
            for (int i = SQUARES["a8"]; i <= SQUARES["h1"]; i++)
            {
                /* display the rank */
                if (File(i) == 0)
                {
                    s += " " + "87654321"[Rank(i)] + " |";
                }

                /* empty piece */
                if (BOARD[i] == null)
                {
                    s += " . ";
                }
                else
                {
                    Dictionary<string, string> boardI = BOARD[i];
                    string piece = boardI["type"];
                    string color = boardI["color"];
                    string symbol = (color == WHITE) ? piece.ToUpper() : piece.ToLower();
                    s += " " + symbol + " ";
                }

                if (((i + 1) & 0x88) != 0)
                {
                    s += "|\n";
                    i += 8;
                }
            }
            s += "   +------------------------+\n";
            s += "     a  b  c  d  e  f  g  h\n";

            return s;
        }

        public Move MakePretty(Move uglyMove)
        {
            Move move = Clone(uglyMove);
            move.San = MoveToSan(move, false);
            move.To = Algebraic(Convert.ToInt32(move.To));
            move.From = Algebraic(Convert.ToInt32(move.From));

            string flags = "";

            foreach (KeyValuePair<string, int> bit in BITS)
            {
                if ((Convert.ToInt32(BITS[bit.Key]) & Convert.ToInt32(move.Flags)) != 0)
                {
                    flags += FLAGS[bit.Key];
                }
            }

            move.Flags = flags;
            return move;
        }

        /*
         * 
         function clone(obj) {
            var dupe = (obj instanceof Array) ? [] : {};

            for (var property in obj) {
              if (typeof property === 'object') {
                dupe[property] = clone(obj[property]);
              } else {
                dupe[property] = obj[property];
              }
            }

            return dupe;
            }
        */

        public List<Move> Clone(Move[] obj)
        {
            List<Move> dupe = new List<Move>();

            for (int i = 0; i < obj.Length; i++)
            {
                Move cloneResult = Clone(obj[i]);
                dupe[i] = (Move)cloneResult.Clone();
            }

            return dupe;
        }

        public Move Clone(Move obj)
        {
            return (Move)obj.Clone();
        }

        public int Perft(int depth)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("legal", false.ToString());

            List<Move> moves = GenerateMoves(param);
            return 0;
        }
    }
}

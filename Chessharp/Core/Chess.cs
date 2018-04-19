using System;
using System.Collections.Generic;

namespace Chessharp.Core
{
    public class Chess : Core
    {
        public void InitVars()
        {
            this.WHITE = this.core.WHITE;
                BLACK: BLACK,
                PAWN: PAWN,
                KNIGHT: KNIGHT,
                BISHOP: BISHOP,
                ROOK: ROOK,
                QUEEN: QUEEN,
                KING: KING,
                SQUARES: (function() {
                /* from the ECMA-262 spec (section 12.6.4):
                 * "The mechanics of enumerating the properties ... is
                 * implementation dependent"
                 * so: for (var sq in SQUARES) { keys.push(sq); } might not be
                 * ordered correctly
                 */
                var keys = [];
                for (var i = SQUARES.a8; i <= SQUARES.h1; i++)
                {
                    if (i & 0x88) { i += 7; continue; }
                    keys.push(algebraic(i));
                }
                return keys;
            })(),
                FLAGS: FLAGS,
        }

            /***************************************************************************
             * PUBLIC CONSTANTS (is there a better way to do this?)
             **************************************************************************/
                

            /***************************************************************************
             * PUBLIC API
             **************************************************************************/
            load: function(fen)
            {
                return load(fen);
            },

            reset: function()
            {
                return reset();
            },

            moves: function(options)
            {
                /* The internal representation of a chess move is in 0x88 format, and
                 * not meant to be human-readable.  The code below converts the 0x88
                 * square coordinates to algebraic coordinates.  It also prunes an
                 * unnecessary move keys resulting from a verbose call.
                 */

                var ugly_moves = generate_moves(options);
                var moves = [];

                for (var i = 0, len = ugly_moves.length; i < len; i++)
                {

                    /* does the user want a full move object (most likely not), or just
                     * SAN
                     */
                    if (typeof options !== 'undefined' && 'verbose' in options &&
                        options.verbose) {
                            moves.push(make_pretty(ugly_moves[i]));
                        } else {
                            moves.push(move_to_san(ugly_moves[i], false));
                        }
                    }

                    return moves;
                },

                in_check: function()
                {
                    return in_check();
                },

                in_checkmate: function()
                {
                    return in_checkmate();
                },

                in_stalemate: function()
                {
                    return in_stalemate();
                },

                in_draw: function()
                {
                    return half_moves >= 100 ||
                           in_stalemate() ||
                           insufficient_material() ||
                           in_threefold_repetition();
                },

                    insufficient_material: function()
                {
                    return insufficient_material();
                },

                    in_threefold_repetition: function()
                {
                    return in_threefold_repetition();
                },

                game_over: function()
                {
                    return half_moves >= 100 ||
                           in_checkmate() ||
                           in_stalemate() ||
                           insufficient_material() ||
                           in_threefold_repetition();
                },

                validate_fen: function(fen)
                {
                    return validate_fen(fen);
                },

                 fen: function()
                {
                    return generate_fen();
                },

                board: function()
                {
                    var output = [],
                    row = [];

                    for (var i = SQUARES.a8; i <= SQUARES.h1; i++)
                    {
                        if (board[i] == null)
                        {
                            row.push(null)
                        }
                        else
                        {
                            row.push({ type: board[i].type, color: board[i].color})
                        }
                        if ((i + 1) & 0x88) {
                            output.push(row);
                            row = []
                            i += 8;
                        }
                      }

                  return output;
                },

    pgn: function(options)
{
    /* using the specification from http://www.chessclub.com/help/PGN-spec
     * example for html usage: .pgn({ max_width: 72, newline_char: "<br />" })
     */
    var newline = (typeof options === 'object' &&
                   typeof options.newline_char === 'string') ?
                   options.newline_char : '\n';
    var max_width = (typeof options === 'object' &&
                     typeof options.max_width === 'number') ?
                     options.max_width : 0;
    var result = [];
    var header_exists = false;

      /* add the PGN header headerrmation */
      for (var i in header)
    {
        /* TODO: order of enumerated properties in header object is not
         * guaranteed, see ECMA-262 spec (section 12.6.4)
         */
        result.push('[' + i + ' \"' + header[i] + '\"]' + newline);
        header_exists = true;
    }

    if (header_exists && history.length)
    {
        result.push(newline);
    }

    /* pop all of history onto reversed_history */
    var reversed_history = [];
    while (history.length > 0)
    {
        reversed_history.push(undo_move());
    }

    var moves = [];
    var move_string = '';

    /* build the list of moves.  a move_string looks like: "3. e3 e6" */
    while (reversed_history.length > 0)
    {
        var move = reversed_history.pop();

        /* if the position started with black to move, start PGN with 1. ... */
        if (!history.length && move.color === 'b')
        {
            move_string = move_number + '. ...';
        }
        else if (move.color === 'w')
        {
            /* store the previous generated move_string if we have one */
            if (move_string.length)
            {
                moves.push(move_string);
            }
            move_string = move_number + '.';
        }

        move_string = move_string + ' ' + move_to_san(move, false);
        make_move(move);
    }

    /* are there any other leftover moves? */
    if (move_string.length)
    {
        moves.push(move_string);
    }

    /* is there a result? */
    if (typeof header.Result !== 'undefined')
    {
        moves.push(header.Result);
    }

    /* history should be back to what is was before we started generating PGN,
     * so join together moves
     */
    if (max_width === 0)
    {
        return result.join('') + moves.join(' ');
    }

    /* wrap the PGN output at max_width */
    var current_width = 0;
    for (var i = 0; i < moves.length; i++)
    {
        /* if the current move will push past max_width */
        if (current_width + moves[i].length > max_width && i !== 0)
        {

            /* don't end the line with whitespace */
            if (result[result.length - 1] === ' ')
            {
                result.pop();
            }

            result.push(newline);
            current_width = 0;
        }
        else if (i !== 0)
        {
            result.push(' ');
            current_width++;
        }
        result.push(moves[i]);
        current_width += moves[i].length;
    }

    return result.join('');
},

    load_pgn: function(pgn, options)
{
    // allow the user to specify the sloppy move parser to work around over
    // disambiguation bugs in Fritz and Chessbase
    var sloppy = (typeof options !== 'undefined' && 'sloppy' in options) ?
                    options.sloppy : false;

    function mask(str)
    {
        return str.replace(/\\/ g, '\\');
    }

    function has_keys(object)
    {
        for (var key in object)
        {
            return true;
        }
        return false;
    }

    function parse_pgn_header(header, options)
    {
        var newline_char = (typeof options === 'object' &&
                            typeof options.newline_char === 'string') ?
                            options.newline_char : '\r?\n';
        var header_obj = { };
        var headers = header.split(new RegExp(mask(newline_char)));
        var key = '';
        var value = '';

        for (var i = 0; i < headers.length; i++)
        {
            key = headers[i].replace(/^\[([A - Z][A - Za - z] *)\s.*\]$/, '$1');
    value = headers[i].replace(/^\[[A - Za - z] +\s"(.*)"\]$/, '$1');
    if (trim(key).length > 0)
    {
        header_obj[key] = value;
    }
}

        return header_obj;
      }

      var newline_char = (typeof options === 'object' &&
                          typeof options.newline_char === 'string') ?
                          options.newline_char : '\r?\n';

// RegExp to split header. Takes advantage of the fact that header and movetext
// will always have a blank line between them (ie, two newline_char's).
// With default newline_char, will equal: /^(\[((?:\r?\n)|.)*\])(?:\r?\n){2}/
var header_regex = new RegExp('^(\\[((?:' + mask(newline_char) + ')|.)*\\])' +
                        '(?:' + mask(newline_char) + '){2}');

// If no header given, begin with moves.
var header_string = header_regex.test(pgn) ? header_regex.exec(pgn)[1] : '';

// Put the board in the starting position
reset();

/* parse PGN header */
var headers = parse_pgn_header(header_string, options);
      for (var key in headers) {
        set_header([key, headers[key]]);
      }

      /* load the starting position indicated by [Setup '1'] and
      * [FEN position] */
      if (headers['SetUp'] === '1') {
          if (!(('FEN' in headers) && load(headers['FEN'], true ))) { // second argument to load: don't clear the headers
            return false;
          }
      }

      /* delete header to get the moves */
      var ms = pgn.replace(header_string, '').replace(new RegExp(mask(newline_char), 'g'), ' ');

/* delete comments */
ms = ms.replace(/(\{[^}]+\})+?/g, '');

      /* delete recursive annotation variations */
      var rav_regex = / (\([^\(\)]+\))+?/g;
      while (rav_regex.test(ms)) {
        ms = ms.replace(rav_regex, '');
      }

      /* delete move numbers */
      ms = ms.replace(/\d+\.(\.\.)?/g, '');

      /* delete ... indicating black to move */
      ms = ms.replace(/\.\.\./g, '');

      /* delete numeric annotation glyphs */
      ms = ms.replace(/\$\d+/g, '');

      /* trim and get array of moves */
      var moves = trim(ms).split(new RegExp(/\s +/));

/* delete empty entries */
moves = moves.join(',').replace(/,,+/g, ',').split(',');
var move = '';

      for (var half_move = 0; half_move<moves.length - 1; half_move++) {
        move = move_from_san(moves[half_move], sloppy);

        /* move not possible! (don't clear the board to examine to show the
         * latest valid position)
         */
        if (move == null) {
          return false;
        } else {
          make_move(move);
        }
      }

      /* examine last move */
      move = moves[moves.length - 1];
      if (POSSIBLE_RESULTS.indexOf(move) > -1) {
        if (has_keys(header) && typeof header.Result === 'undefined') {
          set_header(['Result', move]);
        }
      }
      else {
        move = move_from_san(move, sloppy);
        if (move == null) {
          return false;
        } else {
          make_move(move);
        }
      }
      return true;
    },

    header: function()
{
    return set_header(arguments);
},

    ascii: function()
{
    return ascii();
},

    turn: function()
{
    return turn;
},

    move: function(move, options)
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
    var sloppy = (typeof options !== 'undefined' && 'sloppy' in options) ?
                    options.sloppy : false;

    var move_obj = null;

    if (typeof move === 'string')
    {
        move_obj = move_from_san(move, sloppy);
    }
    else if (typeof move === 'object')
    {
        var moves = generate_moves();

        /* convert the pretty move object to an ugly move object */
        for (var i = 0, len = moves.length; i < len; i++)
        {
            if (move.from === algebraic(moves[i].from) &&
                move.to === algebraic(moves[i].to) &&
                (!('promotion' in moves[i]) ||
                move.promotion === moves[i].promotion)) {
            move_obj = moves[i];
            break;
        }
    }
}

      /* failed to find move */
      if (!move_obj) {
        return null;
      }

      /* need to make a copy of move because we can't generate SAN after the
       * move is made
       */
      var pretty_move = make_pretty(move_obj);

make_move(move_obj);

      return pretty_move;
    },

    undo: function()
{
    var move = undo_move();
    return (move) ? make_pretty(move) : null;
},

    clear: function()
{
    return clear();
},

    put: function(piece, square)
{
    return put(piece, square);
},

    get: function(square)
{
    return get(square);
},

    remove: function(square)
{
    return remove(square);
},

    perft: function(depth)
{
    return perft(depth);
},

    square_color: function(square)
{
    if (square in SQUARES) {
        var sq_0x88 = SQUARES[square];
        return ((rank(sq_0x88) + file(sq_0x88)) % 2 === 0) ? 'light' : 'dark';
    }

    return null;
},

    history: function(options)
{
    var reversed_history = [];
    var move_history = [];
    var verbose = (typeof options !== 'undefined' && 'verbose' in options &&
                   options.verbose);

    while (history.length > 0)
    {
        reversed_history.push(undo_move());
    }

    while (reversed_history.length > 0)
    {
        var move = reversed_history.pop();
        if (verbose)
        {
            move_history.push(make_pretty(move));
        }
        else
        {
            move_history.push(move_to_san(move));
        }
        make_move(move);
    }

    return move_history;
}

  };
    }
}

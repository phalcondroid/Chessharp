# Chessharp

Chessharp is a c# library based in chess.js library that is used for chess move
generation/validation, piece placement/movement, and check/checkmate/stalemate
detection - basically everything but the AI.

## Get ascii board

```csharp
  Chess chess = new Chess();

  chess.Move("e4");
  chess.Move("e5");
  chess.Move("f4");

  Console.Write(chess.GetAscii());
```
#### Result
```
+------------------------+
 8 | r  n  b  q  k  b  n  r |
 7 | p  p  p  p  .  p  p  p |
 6 | .  .  .  .  .  .  .  . |
 5 | .  .  .  .  p  .  .  . |
 4 | .  .  .  .  P  P  .  . |
 3 | .  .  .  .  .  .  .  . |
 2 | P  P  P  P  .  .  P  P |
 1 | R  N  B  Q  K  B  N  R |
   +------------------------+
     a  b  c  d  e  f  g  h
```

## Get fen

```csharp
  Chess chess = new Chess();

  chess.Move("e4");
  chess.Move("e5");
  chess.Move("f4");

  Console.WriteLine(chess.GetFen());
```
#### Result
```
rnbqkbnr/pppp1ppp/8/4p3/4PP2/8/PPPP2PP/RNBQKBNR b  KQkq f3 0 2
```

## In Draw

```csharp
  Chess chess = new Chess("4k3/4P3/4K3/8/8/8/8/8 b - - 0 78");
  Console.WriteLine(
      "In draw {0}",
      chess.InDraw()
  );
```
#### Result
```
In draw True
```

## In check mate

```csharp
  Chess chess = new Chess("rnb1kbnr/pppp1ppp/8/4p3/5PPq/8/PPPPP2P/RNBQKBNR w KQkq - 1 3");
  Console.WriteLine(
      "In check mate {0}",
      chess.InCheckmate()
  );
```
#### Result
```
In check mate True
```

## In stalemate

```csharp
  Chess chess = new Chess("4k3/4P3/4K3/8/8/8/8/8 b - - 0 78");
  Console.WriteLine(
      "In stale mate {0}",
      chess.InStalemate()
  );
```
#### Result
```
In stale mate True
```

## In stalemate

```csharp
  Chess chess = new Chess("4k3/4P3/4K3/8/8/8/8/8 b - - 0 78");
  Console.WriteLine(
      "In stale mate {0}",
      chess.InStalemate()
  );
```
#### Result
```
In stale mate True
```

## Make moves

```csharp
  Chess chess = new Chess();
  Move moveE4 = chess.Move("e4");

  Console.WriteLine("color {0}", moveE4.Color);
  Console.WriteLine("from {0}",  moveE4.From);
  Console.WriteLine("to {0}",    moveE4.To);
  Console.WriteLine("flags {0}", moveE4.Flags);
  Console.WriteLine("piece {0}", moveE4.Piece);

  Console.Write(chess.GetAscii());

```
#### Result

```
color w
from e2
to e4
flags b
piece p

+------------------------+
 8 | r  n  b  q  k  b  n  r |
 7 | p  p  p  p  p  p  p  p |
 6 | .  .  .  .  .  .  .  . |
 5 | .  .  .  .  .  .  .  . |
 4 | .  .  .  .  P  .  .  . |
 3 | .  .  .  .  .  .  .  . |
 2 | P  P  P  P  .  P  P  P |
 1 | R  N  B  Q  K  B  N  R |
   +------------------------+
     a  b  c  d  e  f  g  h
```

## Make moves with Move class as parameter

```csharp
  Chess chess = new Chess();

  Move moveToE4 = new Move();
  moveToE4.From = "e2";
  moveToE4.To = "e4";

  Move moveE4 = chess.Move(moveToE4);

  Console.WriteLine("color {0}", moveE4.Color);
  Console.WriteLine("from {0}",  moveE4.From);
  Console.WriteLine("to {0}",    moveE4.To);
  Console.WriteLine("flags {0}", moveE4.Flags);
  Console.WriteLine("piece {0}", moveE4.Piece);

  Console.Write(chess.GetAscii());

```

#### Result

```
color w
from e2
to e4
flags b
piece p

+------------------------+
 8 | r  n  b  q  k  b  n  r |
 7 | p  p  p  p  p  p  p  p |
 6 | .  .  .  .  .  .  .  . |
 5 | .  .  .  .  .  .  .  . |
 4 | .  .  .  .  P  .  .  . |
 3 | .  .  .  .  .  .  .  . |
 2 | P  P  P  P  .  P  P  P |
 1 | R  N  B  Q  K  B  N  R |
   +------------------------+
     a  b  c  d  e  f  g  h
```

## Get legal moves

```csharp
  Chess chess = new Chess();

  List<string> legalMoves = chess.Moves();

  for (int i = 0; i < legalMoves.Count; i++) {
      Console.WriteLine(legalMoves[i]);
  }

```
#### Result
```
a3
a4
b3
b4
c3
c4
d3
d4
e3
e4
f3
f4
g3
g4
h3
h4
Na3
Nc3
Nf3
Nh3
```

## Validate fen

### Valid fen result
```csharp
  Chess chess = new Chess();
  Dictionary<string, string> resFen1 = chess.ValidateFen("2n1r3/p1k2pp1/B1p3b1/P7/5bP1/2N1B3/1P2KP2/2R5 b - - 4 25");

  Console.WriteLine(
      "Valid {0} ErrorNumber {1} Errors {2}",
      resFen1["valid"],
      resFen1["error_number"],
      resFen1["errors"]
  );

```
#### Result
```
Valid true ErrorNumber 0 Errors No errors.
```

### Invalid fen result
```csharp
  Chess chess = new Chess();
  Dictionary<string, string> resFen2 = chess.ValidateFen("4r3/8/X12XPk/1p6/pP2p1R1/P1B5/2P2K2/3r4 w - - 1 45");

  Console.WriteLine(
      "Valid {0} ErrorNumber {1} Errors {2}",
      resFen2["valid"],
      resFen2["error_number"],
      resFen2["errors"]
  );

```
#### Result
```
Valid false ErrorNumber 9 Errors 1st field (piece positions) is invalid [invalid piece].
```
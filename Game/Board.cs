using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public struct Vector2i
{
    public int x;
    public int y;
    public Vector2i(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    static public Vector2 operator *(Vector2i pos, float n)
    {
        return new Vector2((float)pos.x * n, (float)pos.y * n);
    }
    static public bool operator ==(Vector2i posA, Vector2i posB)
    {
        return posA.x == posB.x && posA.y == posB.y;
    }
    static public bool operator !=(Vector2i posA, Vector2i posB)
    {
        return posA.x != posB.x || posA.y != posB.y;
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public class Board : Node2D
{
    public float tileSize { get; private set;}
    public Piece[,] pieces { get; private set;}
    public King[] kings { get; private set;}
    public int turn { get; private set;}
    public Settings settings { get; set;}
    public int[] timeFuel { get; private set;}
    public SFXManager sfxManager { get; private set;}
    [Export]
    public Color lastMoveColour;
    ColorRect[] lastMove;
    Control controlPieces;
    public Control controlPreviews { get; private set;}
    public bool isPaused {get; set;}

    public void InstanciatePiece(PackedScene scene, string type, Colour colour, int x, int y)
    {
        Piece inst = scene.Instance<Piece>();
        ulong id = inst.GetInstanceId();
        inst.SetScript(GD.Load<Script>("res://Game/Pieces/" + type + ".cs")); //SetProcess(true); to activate process after SetScript
        pieces[x,y] = (Piece)GD.InstanceFromId(id);
        pieces[x,y].Init(this, colour, x, y);
        controlPieces.AddChild(pieces[x,y]);
        
        if (type == "King")
            kings[(int)colour] = (King)pieces[x,y];
    }

    private void ClearBoard()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (pieces[x,y] != null)
                {
                    pieces[x,y].QueueFree();
                    pieces[x,y] = null;
                }
            }

        }
        turn = 1;
        isPaused = false;
        timeFuel[0] = settings.maxFuel;
        timeFuel[1] = settings.maxFuel;
        EmitSignal(nameof(TimeTravel), timeFuel, Colour.Black);
        EmitSignal(nameof(TimeTravel), timeFuel, Colour.White);
        lastMove[0].Visible = false;
        lastMove[1].Visible = false;
        controlPreviews.Visible = false;
    }

    public void ResetPieces()
    {
        ClearBoard();
        PackedScene scene = GD.Load<PackedScene>("res://Game/Pieces/Piece.tscn");
        for (int x = 0; x < 8; x++)
        {
            InstanciatePiece(scene, "Pawn", Colour.Black, x, 1);
            InstanciatePiece(scene, "Pawn", Colour.White, x, 6);

            string type = "King";
            switch (x) {
                case 0: case 7:
                    type = "Rook"; break;
                case 1: case 6:
                    type = "Knight"; break;
                case 2: case 5:
                    type = "Bishop"; break;
                case 3:
                    type = "Queen"; break;
            }
            InstanciatePiece(scene, type, Colour.Black, x, 0);
            InstanciatePiece(scene, type, Colour.White, x, 7);
        }
        if (settings.playAI && settings.AIColour == Colour.White)
            EmitSignal(nameof(AITurn), false);
    }

    public void MoveLastMove(Vector2i lastPos, Vector2i newPos)
    {
        if (!lastMove[0].Visible)
        {
            lastMove[0].Visible = true;
            lastMove[1].Visible = true;
        }
        lastMove[0].RectPosition = lastPos * tileSize;
        lastMove[1].RectPosition = newPos * tileSize;
    }

    private string CoordFromPos(Vector2i pos)
    {
        return "" + (char)(pos.x + 97) + Math.Abs(pos.y - 8);
    }
    private string CoordFromPos(Vector2 pos)
    {
        return "" + (char)(pos.x + 97) + Math.Abs(pos.y - 8);
    }

    private string disambiguate(Move move)
    {
        Vector2 lastPos = lastMove[0].RectPosition/tileSize;
        if (move.timeTravelCost > 0)
            return CoordFromPos(lastPos);
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (pieces[x,y] == null)
                    continue;
                if (pieces[x,y].pos != move.piece.pos && 
                    pieces[x,y].colour == move.piece.colour && 
                    pieces[x,y].GetType() == move.piece.GetType())
                {
                    foreach(Move otherMove in pieces[x,y].GetPosibleMoves())
                    {
                        if (otherMove.pos == move.pos)
                        {
                            if (otherMove.piece.pos.x != lastPos.x)
                                return "" + (char)(lastPos.x + 97);
                            else
                                return "" + Math.Abs(lastPos.y - 8);
                            
                        }
                    }

                }
            }
        }
        return "";
    }

    private string PawnNotation(Move move, bool isCheck)
    {
        string notation = move.timeTravelCost > 0 ? CoordFromPos(lastMove[0].RectPosition/tileSize) : "";
        bool enPassant = false;
        if (move.target != null)
        {
            if (move.timeTravelCost > 0)
                notation += "x" + CoordFromPos(move.pos);
            else {
                notation += (char)(lastMove[0].RectPosition.x/tileSize + 97) + "x" + (char)(move.pos.x + 97);
                if (move.target.pos != move.pos)
                    enPassant = true;
            }
        } else {
            notation += CoordFromPos(move.pos);
        }
        if ((move.piece.colour == Colour.Black && move.pos.y == 7) || 
            (move.piece.colour == Colour.White && move.pos.y == 0))
            notation += "=Q";
        if (enPassant)
            notation += " e.p.";
        return notation;
    }

    private string NotationFromMove(Move move, bool isCheck, bool checkmate)
    {
        if (move.isCastling)
        {
            if (move.pos.x == 2)
                return "O-O-O";
            else 
                return "O-O";
        }
        string notation = move.timeTravelCost > 0 ? "<" : "";
        if (move.piece.GetType() == typeof(Pawn))
            notation += PawnNotation(move, isCheck);
        else {
            if (move.piece.GetType() == typeof(Knight))
                notation += "N";
            else
                notation += move.piece.GetType().ToString().Substring(0, 1);
            notation += disambiguate(move);
            if (move.target != null)
                notation += "x";
            notation += CoordFromPos(move.pos);
        }
        if (checkmate)
            notation += "#";
        else if (isCheck)
            notation += "+";
        return notation;
    }

    public void NextTurn(Move lastMove, Colour colour)
    {
        turn++;
        Colour nextTurnColour = colour == Colour.Black ? Colour.White : Colour.Black;
        bool kingIsCheck = kings[(int)nextTurnColour].IsCheck();
        bool checkmate = false;
        UpdatePieces(nextTurnColour, kingIsCheck);

        if (kingIsCheck)
        {
            bool isCheckmate = true;
            foreach(Move move in GetAllPiecesMoves(nextTurnColour, true))
            {
                if (!move.noPreview)
                {
                    isCheckmate = false;
                    break;
                }
            }
            if (isCheckmate)
                {
                    EmitSignal(nameof(Checkmate), colour);
                    checkmate = true;
                }
        }
        if (settings.playAI && settings.AIColour == nextTurnColour)
            EmitSignal(nameof(AITurn), kingIsCheck);
        EmitSignal(nameof(MoveOver), NotationFromMove(lastMove, kingIsCheck, checkmate));
    }

    [Signal]
    public delegate void AITurn(bool isCheck);
    [Signal]
    public delegate void MoveOver(string notation);
    [Signal]
    public delegate void Checkmate(Colour colour);

    private void UpdatePieces(Colour colour, bool kingIsCheck)
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (pieces[x, y] != null)
                {
                    if (pieces[x, y].colour == colour && pieces[x, y].GetType() != typeof(King))
                        pieces[x, y].kingIsCheck = kingIsCheck;
                    else
                        pieces[x, y].kingIsCheck = false;
                }
            }
        }
    }

    public void PromotePawn(Piece pawn, Move move)
    {
        InstanciatePiece(GD.Load<PackedScene>("res://Game/Pieces/Piece.tscn"), "Queen", pawn.colour, pawn.pos.x, pawn.pos.y);
        pieces[pawn.pos.x, pawn.pos.y].previousMoves = pawn.previousMoves;
        pawn.QueueFree();
        NextTurn(move, pieces[pawn.pos.x, pawn.pos.y].colour);
    }


    [Signal]
    public delegate void TimeTravel(int[] timeFuel, Colour colour);

    public void UpdateTimeFuel(int cost, Colour colour)
    {
        if (cost > 0)
        {
            timeFuel[(int)colour] -= cost;
            EmitSignal(nameof(TimeTravel), timeFuel, colour);
        }
    }

    public List<Move> GetAllPiecesMoves(Colour colour, bool defendKing = false)
    {
        List<Move> moves = new List<Move>();
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (pieces[x, y] != null && pieces[x, y].colour == colour)
                {
                    if (defendKing && pieces[x, y].GetType() != typeof(King))
                        moves.AddRange(pieces[x, y].DefendKing());
                    else
                        moves.AddRange(pieces[x, y].GetPosibleMoves());
                }
            }
        }

        return moves;
    }

    private void InitLastMove()
    {
        Vector2 size = new Vector2(tileSize, tileSize);
        lastMove = new ColorRect[2];
        lastMove[0] = new ColorRect
        {
            RectSize = size,
            Color = lastMoveColour
        };
        lastMove[1] = new ColorRect
        {
            RectSize = size,
            Color = lastMoveColour
        };

        Control boardControl = GetNode<Control>("BoardControl");
        boardControl.AddChild(lastMove[0]);
        boardControl.AddChild(lastMove[1]);
    }

    public override void _Ready()
    {
        controlPieces = GetNode<Control>("Pieces");
        controlPreviews = GetNode<Control>("Previews");
        tileSize = 50;
        pieces = new Piece[8,8];
        kings = new King[2];
        timeFuel = new int[2];
        InitLastMove();

        sfxManager = GetNode<SFXManager>("/root/SFXManager");
    }
}

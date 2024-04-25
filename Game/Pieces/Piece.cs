using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public enum Colour
{
    Black,
    White
}

public struct Move{
    public Vector2i pos;
    public Piece piece;
    public Piece target;
    public int turn;
    public bool canCapture;
    public bool noPreview;
    public bool timeTravel;
    public Move(Piece piece, int x, int y, bool canCapture = true, bool noPreview = false, Piece target = null)
    {
        this.piece = piece;
        pos.x = x;
        pos.y = y;
        turn = -1;
        this.canCapture = canCapture;
        this.target = target;
        this.noPreview = noPreview;
        timeTravel = false;
    }
    public Move(Piece piece, Move move, Piece target = null)
    {
        this.piece = piece;
        pos = move.pos;
        turn = -1;
        canCapture = move.canCapture;
        this.target = target;
        noPreview = true;
        timeTravel = true;
    }
}

public abstract class Piece : Node2D
{
    public Colour colour { get; private set;}
    public Vector2i pos { get; private set;}
    protected Board board { get; private set;}
    protected List<Move> previousMoves { get; private set;}

    protected bool isPreviewing { get; private set;}
    public bool needsUpdate { get; set;}
    protected List<MovePreview> previews { get; private set;}

    public bool kingIsCheck { get; set;}
    public King king { get; set;}

    public virtual void Init(Board board, Colour colour, int x, int y)
    {
        this.board = board;
        this.colour = colour;
        pos = new Vector2i(x, y);
        Position = pos * board.tileSize;

        isPreviewing = false;
        needsUpdate = true;
        kingIsCheck = false;
        previousMoves = new List<Move>();
    }

    public abstract List<Move> GetPosibleMoves();

    protected bool CheckMove(List<Move> moves, int x, int y, bool canCapture = true, bool onlyCapture = false)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7)
            return true;
        if (board.pieces[x, y] == null)
        {
            if (!onlyCapture)
                moves.Add(new Move(this, x, y, canCapture));
            else if(canCapture)
                moves.Add(new Move(this, x, y, canCapture, true));
            return false;
        }
        if (canCapture && board.pieces[x, y].colour != colour)
            moves.Add(new Move(this, x, y, true, false, board.pieces[x, y]));
        if (board.pieces[x, y].colour == colour)
            moves.Add(new Move(this, x, y, canCapture, true));
        return true;
    }

    public void PerformMove(Move move)
    {
        board.pieces[pos.x, pos.y] = null;
        pos = move.pos;
        Position = pos * board.tileSize;
        board.pieces[pos.x, pos.y] = this;
        move.target?.QueueFree();
        move.turn = board.turn;
        previousMoves.Add(move);
        TogglePreviews();
        board.NextTurn(move, colour);
    }

    protected void CreatePreviews()
    {
        foreach(MovePreview preview in previews)
        {
            preview.QueueFree();
        }
        previews.Clear();

        List<Move> moves = kingIsCheck ? DefendKing() : GetPosibleMoves();
        PackedScene scene = GD.Load<PackedScene>("res://Game/MovePreview.tscn");

        if (moves == null || moves.Count() == 0)
            return;
        foreach (Move move in moves)
        {
            if (move.noPreview)
                continue;
            MovePreview preview = scene.Instance<MovePreview>();
            preview.Init(move, this, board.tileSize);
            AddChild(preview);
            previews.Add(preview);
        }
    }

    protected void TogglePreviews()
    {
        if (needsUpdate)
        {
            previews = new List<MovePreview>();
            CreatePreviews();
            needsUpdate = false;
        }
        isPreviewing = !isPreviewing;
        foreach(MovePreview preview in previews)
        {
            preview.Visible = !preview.Visible;
        }       
    }

    protected bool IsTurn()
    {
        return (colour == Colour.Black && board.turn % 2 == 0) || (colour == Colour.White && board.turn % 2 != 0);
    }

    public List<Move> DefendKing()
    {
        List<Move> limitedMoves = new List<Move>();

        foreach (Move move in GetPosibleMoves())
        {
            board.pieces[move.pos.x, move.pos.y] = this;
            bool stillCheck = false;
            if (move.pos != king.attacker.pos)
            {
                foreach (Move attack in king.attacker.GetPosibleMoves())
                {
                    if (attack.pos == king.pos)
                    {
                        // GD.Print("stillcheck");
                        // GD.Print(move.pos.x.ToString() + " " + move.pos.y.ToString());
                        stillCheck = true;
                        break;
                    }
                }
            }
            if (!stillCheck)
                limitedMoves.Add(move);
            board.pieces[move.pos.x, move.pos.y] = move.target;
        }
        return limitedMoves;
    }

    public void _on_Button_pressed()
    {
        if (!IsTurn())
            return;
        if (!isPreviewing)
            TogglePreviews();
        //GD.Print(Position.x.ToString() + " " + Position.y.ToString());
    }

    public override void _Input(InputEvent @event)
    {
        if (isPreviewing)
        {
            if (@event is InputEventMouseButton mouse)
            {
                if (mouse.Pressed)
                    return;
                bool clickOut = true;
                foreach (MovePreview preview in previews)
                {
                    if (preview.CheckMouse(mouse.Position, board.Scale))
                    {
                        clickOut = false;
                        break;
                    }
                }
                if (clickOut)
                {
                    GD.Print("£");
                    TogglePreviews();   
                }
            }
        }
    }
}

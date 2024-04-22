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
    public int turn;
    public Move(int x, int y, Piece piece = null)
    {
        pos.x = x;
        pos.y = y;
        turn = -1;
        this.piece = piece;
    }
}

public abstract class Piece : Node2D
{
    protected Colour colour { get; private set;}
    protected Vector2i pos { get; private set;}
    protected Board board { get; private set;}
    protected List<Move> previousMoves { get; private set;}

    protected bool isPreviewing { get; private set;}
    protected List<MovePreview> previews { get; private set;}

    public virtual void Init(Board board, Colour colour, int x, int y)
    {
        this.board = board;
        this.colour = colour;
        pos = new Vector2i(x, y);
        Position = pos * board.tileSize;

        isPreviewing = false;
        previousMoves = new List<Move>();
    }

    public void UpdateScale(Vector2 newScale) 
    {
        Scale = newScale;
        Position = pos * board.tileSize;
    }

    public abstract List<Move> GetPosibleMoves();

    protected bool CheckMove(List<Move> moves, int x, int y, bool canCapture = true, bool onlyCapture = false)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7)
            return true;
        if (board.pieces[x, y] == null)
        {
            if (!onlyCapture)
                moves.Add(new Move(x, y));
            return false;
        }
        if (canCapture && board.pieces[x, y].colour != colour)
            moves.Add(new Move(x, y, board.pieces[x, y]));
        return true;
    }

    protected void CreatePreviews()
    {
        foreach(MovePreview preview in previews)
        {
            preview.QueueFree();
        }
        previews.Clear();

        List<Move> moves = GetPosibleMoves();
        PackedScene scene = GD.Load<PackedScene>("res://Game/MovePreview.tscn");

        if (moves.Count() == 0)
            return;
        foreach (Move move in moves)
        {
            MovePreview preview = scene.Instance<MovePreview>();
            preview.Init(move, this, board.tileSize);
            AddChild(preview);
            previews.Add(preview);
        }
    }

    protected void TogglePreviews()
    {
        if (previews == null)
        {
            previews = new List<MovePreview>();
            CreatePreviews();
        }
        isPreviewing = !isPreviewing;
        foreach(MovePreview preview in previews)
        {
            preview.Visible = !preview.Visible;
        }       
    }

    public void PerformMove(Move move)
    {
        isPreviewing = false;
        board.pieces[pos.x, pos.y] = null;
        pos = move.pos;
        Position = pos * board.tileSize;
        board.pieces[pos.x, pos.y] = this;
        if (move.piece != null)
        {
            move.piece.QueueFree();
        }
        move.turn = board.turn;
        board.NextTurn(move);
        previousMoves.Add(move);
        CreatePreviews();
    }

    public void _on_Button_pressed()
    {
        if ((colour == Colour.Black && board.turn % 2 != 0) || (colour == Colour.White && board.turn % 2 == 0))
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
                bool unfocus = true;

                foreach (MovePreview preview in previews)
                {
                    if (preview.CheckMouse(mouse.Position))
                    {
                        unfocus = false;
                        break;
                    }
                }
                if (unfocus)
                    TogglePreviews();
            }
        }
    }
}

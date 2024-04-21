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
    public Vector2 pos;
    public Piece piece;
    public Move(int x, int y, Piece piece = null)
    {
        pos.x = x;
        pos.y = y;
        this.piece = piece;
    }
}

public abstract class Piece : Node2D
{
    protected Colour colour { get; private set;}
    protected Vector2i pos { get; private set;}
    protected Board board { get; private set;}
    protected List<Move> previousMoves { get; private set;}

    protected List<MovePreview> previews { get; private set;}

    public virtual void Init(Board board, Colour colour, int x, int y)
    {
        this.board = board;
        this.colour = colour;
        pos = new Vector2i(x, y);
        Position = pos * board.tileSize;

        previousMoves = new List<Move>();
        previews = new List<MovePreview>();
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

    public void _on_Button_pressed()
    {
        CreatePreviews();
        GD.Print(Position.x.ToString() + " " + Position.y.ToString());
        // foreach(MovePreview preview in previews)
        // {
        //     //GD.Print(preview.Position.x.ToString() + " " + preview.Position.y.ToString());
        //     preview.ZIndex = 100;
        // }
    }
}

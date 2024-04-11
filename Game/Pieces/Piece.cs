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
    Vector2 pos;
    Piece piece;
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
    protected Vector2 gridPos { get; private set;}
    protected Board board { get; private set;}
    protected List<Move> moves { get; private set;}


    public virtual void Init(Board board, Colour colour, int x, int y)
    {
        this.board = board;
        this.colour = colour;
        gridPos = new Vector2(x, y);
        Position = gridPos * board.tileSize;
    }

    public void UpdateScale(Vector2 newScale) 
    {
        Scale = newScale;
        Position = gridPos * board.tileSize;
    }

    public abstract List<Move> GetPosibleMoves();

    public void _on_Button_pressed()
    {
        GD.Print(gridPos);
        GD.Print(GetPosibleMoves().Count);
    }
}

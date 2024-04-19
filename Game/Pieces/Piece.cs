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
    protected Vector2i pos { get; private set;}
    protected Board board { get; private set;}
    protected List<Move> moves { get; private set;}


    public virtual void Init(Board board, Colour colour, int x, int y)
    {
        this.board = board;
        this.colour = colour;
        pos = new Vector2i(x, y);
        Position = pos * board.tileSize;
    }

    public void UpdateScale(Vector2 newScale) 
    {
        Scale = newScale;
        Position = pos * board.tileSize;
    }

    public abstract List<Move> GetPosibleMoves();

    protected bool CheckMove(List<Move> moves, int x, int y)
    {
        if ((x < 0 && x > 7) || (y < 0 && y > 7))
            return true;
        if (board.pieces[x, y] == null)
        {
            moves.Add(new Move(x, y));
            return false;
        }
        if (board.pieces[x, y].colour != colour)
            moves.Add(new Move(x, y, board.pieces[x, y]));
        return true;
    }

    public void _on_Button_pressed()
    {
        GD.Print(pos);
        GD.Print(GetPosibleMoves().Count);
    }
}

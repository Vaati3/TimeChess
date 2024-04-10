using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public enum Colour
{
    Black,
    White
}

public struct tile{
    Vector2 pos;
    Piece piece;
}

public abstract class Piece : Node2D
{
    protected Colour colour { get; private set;}
    protected Vector2 gridPos { get; private set;}
    protected Vector2[] moves { get; private set;}

    public virtual void Init(Colour colour, int x, int y)
    {
        gridPos = new Vector2(x, y);
        this.colour = colour;
        Position = gridPos * 50;
    }

    public abstract List<tile> GetPosibleMoves(ref Piece[,] pieces);
}

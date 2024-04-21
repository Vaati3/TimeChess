using Godot;
using System;

public class MovePreview : Node2D
{
    [Export]
    Color red;
    [Export]
    Color green;
    Move move;
    Piece owner;
    public void Init(Move move, Piece owner, float tileSize)
    {
        this.move = move;
        this.owner = owner;

        Position = (move.pos * tileSize) - owner.Position;
        if (move.piece == null)
            GetNode<ColorRect>("Control/ColorRect").Color = green;
        else
            GetNode<ColorRect>("Control/ColorRect").Color = red;
    }
}

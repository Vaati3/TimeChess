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
    float tileSize;
    public void Init(Move move, Piece owner, float tileSize)
    {
        this.move = move;
        this.owner = owner;
        this.tileSize = tileSize;

        Position = (move.pos * tileSize) - owner.Position;
        Visible = false;
        if (move.piece == null)
            GetNode<ColorRect>("Control/ColorRect").Color = green;
        else
            GetNode<ColorRect>("Control/ColorRect").Color = red;
    }

    public bool CheckMouse(Vector2 mousePos, Vector2 scale)
    {
        if (mousePos.x >= GlobalPosition.x && mousePos.x <= GlobalPosition.x + tileSize * scale.x && 
            mousePos.y >= GlobalPosition.y && mousePos.y <= GlobalPosition.y + tileSize * scale.y)
            return true;
        return false;
    }

    public void _on_Button_pressed()
    {
        owner.PerformMove(move);
    }
}

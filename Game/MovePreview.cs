using Godot;
using System;

public class MovePreview : Node2D
{
    [Export]
    Color red;
    [Export]
    Color green;
    [Export]
    Color purple;
    [Export]
    Color yellow;
    Move move;
    float tileSize;
    public void Init(Move move, float tileSize)
    {
        this.move = move;
        this.tileSize = tileSize;

        Position = (move.pos * tileSize) - move.piece.Position;
        Visible = false;
        if (move.isCastling)
            GetNode<ColorRect>("Control/ColorRect").Color = yellow;
        else if (move.timeTravelCost > 0)
        {
            GetNode<ColorRect>("Control/ColorRect").Color = purple;
            GetNode<Label>("Control/Label").Text = move.timeTravelCost.ToString();
        } else if (move.target == null)
            GetNode<ColorRect>("Control/ColorRect").Color = green;
        else
            GetNode<ColorRect>("Control/ColorRect").Color = red;
    }

    public bool CheckMouse(Vector2 mousePos, Vector2 scale)
    {
        if (mousePos.x >= GlobalPosition.x && mousePos.x <= GlobalPosition.x + tileSize * scale.x && 
            mousePos.y >= GlobalPosition.y && mousePos.y <= GlobalPosition.y + tileSize * scale.y)
            {
                if (move.isCastling)
                    ((King)move.piece).PerformCastling(move);
                else
                    move.piece.PerformMove(move);
                return true;
            }
        return false;
    }
}

using Godot;
using System;

public class MovePreview : Control
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

        RectPosition = move.pos * tileSize;
        if (move.isCastling)
            GetNode<ColorRect>("ColorRect").Color = yellow;
        else if (move.timeTravelCost > 0)
        {
            GetNode<ColorRect>("ColorRect").Color = purple;
            GetNode<Label>("Label").Text = move.timeTravelCost.ToString();
        } else if (move.target == null)
            GetNode<ColorRect>("ColorRect").Color = green;
        else
            GetNode<ColorRect>("ColorRect").Color = red;
    }

    public bool CheckMouse(Vector2 mousePos, Vector2 scale)
    {
        if (mousePos.x >= RectGlobalPosition.x && mousePos.x <= RectGlobalPosition.x + tileSize * scale.x && 
            mousePos.y >= RectGlobalPosition.y && mousePos.y <= RectGlobalPosition.y + tileSize * scale.y)
            {
                if (move.isCastling)
                    ((King)move.piece).PerformCastling(move);
                else
                    move.piece.PerformMove(move);
                return true;
            }
        return false;
    }

    public override bool CanDropData(Vector2 position, object data)
    {
        return data.GetType().IsSubclassOf(typeof(Piece));
    }

    public override void DropData(Vector2 position, object data)
    {
        move.piece.isDragging = false;
        if (move.isCastling)
            ((King)move.piece).PerformCastling(move);
        else
            move.piece.PerformMove(move);
    }

}

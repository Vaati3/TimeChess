using Godot;
using System;
using System.Collections.Generic;

public class Bishop : Piece
{
    public override void Init(Colour colour, int x, int y)
    {
        base.Init(colour, x, y);
        if (colour == Colour.Black)
        {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/BlackBishop.png");
        } else {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/WhiteBishop.png");
        }
    }

    public override List<tile> GetPosibleMoves(ref Piece[,] pieces)
    {
        List<tile> test = new List<tile>();
    
        return test;
    }
}

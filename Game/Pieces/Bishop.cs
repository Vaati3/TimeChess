using Godot;
using System;
using System.Collections.Generic;

public class Bishop : Piece
{
    public override void Init(Board board, Colour colour, int x, int y)
    {
        base.Init(board, colour, x, y);
        if (colour == Colour.Black)
        {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/BlackBishop.png");
        } else {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/WhiteBishop.png");
        }
    }

    public override List<Move> GetPosibleMoves()
    {
        List<Move> test = new List<Move>();
    
        return test;
    }
}

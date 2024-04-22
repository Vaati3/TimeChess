using Godot;
using System;
using System.Collections.Generic;

public class Rook : Piece
{
    public override void Init(Board board, Colour colour, int x, int y)
    {
        base.Init(board, colour, x, y);
        if (colour == Colour.Black)
        {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/BlackRook.png");
        } else {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/WhiteRook.png");
        }
    }

//missing castling with king
    public override List<Move> GetPosibleMoves()
    {
        List<Move> moves = new List<Move>();
        bool right, left, top, down;
        right = left = top = down = false;

        for(int i = 1; i < 7; i++)
        {
            if (!right)
                right = CheckMove(moves, pos.x + i, pos.y);
            if (!left)
                left = CheckMove(moves, pos.x - i, pos.y);
            if (!top)
                top = CheckMove(moves, pos.x, pos.y - i);
            if (!down)
                down = CheckMove(moves, pos.x, pos.y + i);
            if (top && right && left && down)
                break;
        }
        return moves;
    }
}

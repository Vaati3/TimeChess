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
        List<Move> moves = new List<Move>();
        bool topR, TopL, downR, downL;
        topR = TopL = downR = downL = false;

        for(int i = 1; i < 8; i++)
        {
            if (!topR)
                topR = CheckMove(moves, pos.x + i, pos.y + i);
            if (!TopL)
                TopL = CheckMove(moves, pos.x - i, pos.y + i);
            if (!downR)
                downR = CheckMove(moves, pos.x + i, pos.y - i);
            if (!downL)
                downL = CheckMove(moves, pos.x - i, pos.y - i);
            if (topR && TopL && downR && downL)
                break;
        }

        TimeTravel(moves);
        return moves;
    }
}

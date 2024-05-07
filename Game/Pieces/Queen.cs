using Godot;
using System;
using System.Collections.Generic;

public class Queen : Piece
{
    public override void Init(Board board, Colour colour, int x, int y)
    {
        value = 9;
        base.Init(board, colour, x, y);
        if (colour == Colour.Black)
        {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/BlackQueen.png");
        } else {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/WhiteQueen.png");
        }
    }

    public override List<Move> GetPosibleMoves()
    {
        List<Move> moves = new List<Move>();
        bool right, left, top, down, topR, TopL, downR, downL;
        right = left = top = down = topR = TopL = downR = downL = false;

        for(int i = 1; i < 8; i++)
        {
            if (!right)
                right = CheckMove(moves, pos.x + i, pos.y);
            if (!left)
                left = CheckMove(moves, pos.x - i, pos.y);
            if (!top)
                top = CheckMove(moves, pos.x, pos.y - i);
            if (!down)
                down = CheckMove(moves, pos.x, pos.y + i);
            if (!topR)
                topR = CheckMove(moves, pos.x + i, pos.y + i);
            if (!TopL)
                TopL = CheckMove(moves, pos.x - i, pos.y + i);
            if (!downR)
                downR = CheckMove(moves, pos.x + i, pos.y - i);
            if (!downL)
                downL = CheckMove(moves, pos.x - i, pos.y - i);
            if (top && right && left && down && topR && TopL && downR && downL)
                break;
        }
        TimeTravel(moves);
        return moves;
    }
}

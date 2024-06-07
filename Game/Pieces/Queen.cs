using Godot;
using System;
using System.Collections.Generic;

public class Queen : Piece
{
    public override void Init(Board board, Colour colour, int x, int y)
    {
        value = 9;
        if (colour == Colour.Black)
        {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/BlackQueen.png");
        } else {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/WhiteQueen.png");
        }
        base.Init(board, colour, x, y);
    }

    public override List<Move> GetPosibleMoves(bool ai = false)
    {
        List<Move> moves = new List<Move>();
        bool right, left, top, down, topR, TopL, downR, downL;
        right = left = top = down = topR = TopL = downR = downL = false;

        for(int i = 1; i < 8; i++)
        {
            if (!right)
                right = CheckMove(moves, pos.x + i, pos.y, ai);
            if (!left)
                left = CheckMove(moves, pos.x - i, pos.y, ai);
            if (!top)
                top = CheckMove(moves, pos.x, pos.y - i, ai);
            if (!down)
                down = CheckMove(moves, pos.x, pos.y + i, ai);
            if (!topR)
                topR = CheckMove(moves, pos.x + i, pos.y + i, ai);
            if (!TopL)
                TopL = CheckMove(moves, pos.x - i, pos.y + i, ai);
            if (!downR)
                downR = CheckMove(moves, pos.x + i, pos.y - i, ai);
            if (!downL)
                downL = CheckMove(moves, pos.x - i, pos.y - i, ai);
            if (top && right && left && down && topR && TopL && downR && downL)
                break;
        }
        TimeTravel(moves);
        return moves;
    }
}

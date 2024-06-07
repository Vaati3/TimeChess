using Godot;
using System;
using System.Collections.Generic;

public class Knight : Piece
{
    public override void Init(Board board, Colour colour, int x, int y)
    {
        value = 3;
        if (colour == Colour.Black)
        {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/BlackKnight.png");
        } else {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/WhiteKnight.png");
        }
        base.Init(board, colour, x, y);
    }

    public override List<Move> GetPosibleMoves(bool ai = false)
    {
        List<Move> moves = new List<Move>();
        
        CheckMove(moves, pos.x - 2, pos.y - 1, ai);
        CheckMove(moves, pos.x - 1, pos.y - 2, ai);
        CheckMove(moves, pos.x + 2, pos.y - 1, ai);
        CheckMove(moves, pos.x + 1, pos.y - 2, ai);
        CheckMove(moves, pos.x - 2, pos.y + 1, ai);
        CheckMove(moves, pos.x - 1, pos.y + 2, ai);
        CheckMove(moves, pos.x + 2, pos.y + 1, ai);
        CheckMove(moves, pos.x + 1, pos.y + 2, ai);
        
        TimeTravel(moves);
        return moves;
    }
}

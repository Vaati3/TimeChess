using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Pawn : Piece
{
    public override void Init(Board board, Colour colour, int x, int y)
    {
        base.Init(board, colour, x, y);
        if (colour == Colour.Black)
        {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/BlackPawn.png");
        } else {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/WhitePawn.png");
        }
    }

    public override List<Move> GetPosibleMoves()
    {
        List<Move> moves = new List<Move>();
        int dir = colour == Colour.Black ? 1 : -1;
        int start = colour == Colour.Black ? 1 : 6;

        bool blocked = CheckMove(moves, pos.x, pos.y+dir, false);
        if (pos.y == start && !blocked)
            CheckMove(moves, pos.x, pos.y+dir*2, false);

        //check if piece is there
        CheckMove(moves, pos.x+1, pos.y+dir, true, true);
        CheckMove(moves, pos.x-1, pos.y+dir, true, true);
        
        return moves;
    }
}

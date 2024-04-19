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

        if (board.pieces[pos.x, pos.y+dir] == null)
        {
            moves.Add(new Move(pos.x, pos.y+dir));
            GD.Print(moves.Count);
            if (pos.y == start && board.pieces[pos.x, pos.y + dir*2] == null)
                moves.Add(new Move(pos.x, pos.y+dir*2));
        }
        CheckMove(moves, pos.x+1, pos.y+dir);
        CheckMove(moves, pos.x-1, pos.y+dir);
    
        return moves;
    }
}

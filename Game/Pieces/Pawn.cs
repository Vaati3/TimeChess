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
        int x = (int)gridPos.x;
        int y = (int)gridPos.y;
        int dir = colour == Colour.Black ? 1 : -1;
        int start = colour == Colour.Black ? 1 : 6;

        if (board.pieces[x, y+dir] == null)
        {
            moves.Add(new Move(x, y+dir));
            GD.Print(moves.Count);
            if (y == start && board.pieces[x, y + dir*2] == null)
                moves.Add(new Move(x, y+dir*2));
        }
        if (x != 7 && board.pieces[x+1, y+dir] != null)
            moves.Add(new Move(x, y+dir, board.pieces[x+1, y+dir]));
        if (x != 0 && board.pieces[x-1, y+dir] != null)
            moves.Add(new Move(x, y+dir, board.pieces[x+1, y+dir]));
    
        return moves;
    }
}

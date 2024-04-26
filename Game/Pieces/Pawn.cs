using Godot;
using System;
using System.Collections.Generic;

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

    private void EnPassant(List<Move> moves, int x, int y, int dir)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7)
            return;
        if (board.pieces[x, y] == null)
            return;
        if(board.pieces[x, y].GetType() == typeof(Pawn) && 
            board.pieces[x, y].previousMoves.Count == 1 &&
            board.pieces[x, y].previousMoves[0].turn == board.turn-1)
        {
            moves.Add(new Move(this, x, y+dir, true, false, board.pieces[x, y]));
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

        CheckMove(moves, pos.x+1, pos.y+dir, true, true);
        CheckMove(moves, pos.x-1, pos.y+dir, true, true);
        
        if ((colour == Colour.Black && pos.y == 4) || (colour == Colour.White && pos.y == 3))
        {
            EnPassant(moves, pos.x-1, pos.y, dir);
            EnPassant(moves, pos.x+1, pos.y, dir);
        }

        return moves;
    }
}

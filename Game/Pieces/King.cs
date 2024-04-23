using Godot;
using System;
using System.Collections.Generic;

public class King : Piece
{

    public bool isCheck {get; set;}
    public override void Init(Board board, Colour colour, int x, int y)
    {
        base.Init(board, colour, x, y);
        isCheck = false;
        if (colour == Colour.Black)
        {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/BlackKing.png");
        } else {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/WhiteKing.png");
        }
    }

    protected bool CheckMove(List<Move> moves, int x, int y, List<Move> danger)
    {
        Vector2i newPos = new Vector2i(x, y);
        foreach (Move move in danger)
        {
            //GD.Print(newPos.x.ToString() + " " + newPos.y.ToString() + "  " + move.pos.x.ToString() + " " + move.pos.y.ToString());
            if(move.canCapture && move.pos == newPos)
                return false;
        }
        return CheckMove(moves, x, y);
    }
//add checkmate and prevent selfcheck
    public override List<Move> GetPosibleMoves()
    {
        List<Move> moves = new List<Move>();

        if (IsTurn())
        {
            List<Move> danger = board.GetAllPiecesMoves(colour);

            CheckMove(moves, pos.x + 1, pos.y, danger);
            CheckMove(moves, pos.x - 1, pos.y, danger);
            CheckMove(moves, pos.x, pos.y - 1, danger);
            CheckMove(moves, pos.x, pos.y + 1, danger);
            CheckMove(moves, pos.x + 1, pos.y + 1, danger);
            CheckMove(moves, pos.x - 1, pos.y + 1, danger);
            CheckMove(moves, pos.x + 1, pos.y - 1, danger);
            CheckMove(moves, pos.x - 1, pos.y - 1, danger);
        } else {
            CheckMove(moves, pos.x + 1, pos.y);
            CheckMove(moves, pos.x - 1, pos.y);
            CheckMove(moves, pos.x, pos.y - 1);
            CheckMove(moves, pos.x, pos.y + 1);
            CheckMove(moves, pos.x + 1, pos.y + 1);
            CheckMove(moves, pos.x - 1, pos.y + 1);
            CheckMove(moves, pos.x + 1, pos.y - 1);
            CheckMove(moves, pos.x - 1, pos.y - 1);
        }
        
        return moves;
    }
}

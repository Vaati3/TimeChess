using Godot;
using System;
using System.Collections.Generic;

public class King : Piece
{
    public Piece attacker {get; set;}
    public override void Init(Board board, Colour colour, int x, int y)
    {
        base.Init(board, colour, x, y);
        attacker = null;
        if (colour == Colour.Black)
        {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/BlackKing.png");
        } else {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/WhiteKing.png");
        }
    }

    public bool IsCheck()
    {
        List<Move> danger = board.GetAllPiecesMoves(colour);
        bool isCheck = false;
    
            foreach (Move move in danger)
            {
                if(move.canCapture && move.pos == pos)
                {
                    isCheck = true;
                    attacker = move.piece;
                    break;
                }
            }
            if (isCheck)
            {
                if (GetPosibleMoves(danger).Count == 0)
                    board.Checkmate(colour);
            }

        return isCheck;
    }

    protected bool CheckMove(List<Move> moves, int x, int y, List<Move> danger)
    {
        Vector2i newPos = new Vector2i(x, y);
        foreach (Move move in danger)
        {
            if(move.canCapture && move.pos == newPos)
                return false;
        }
        return CheckMove(moves, x, y);
    }

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

    public List<Move> GetPosibleMoves(List<Move> danger)
    {
        List<Move> moves = new List<Move>();

        CheckMove(moves, pos.x + 1, pos.y, danger);
        CheckMove(moves, pos.x - 1, pos.y, danger);
        CheckMove(moves, pos.x, pos.y - 1, danger);
        CheckMove(moves, pos.x, pos.y + 1, danger);
        CheckMove(moves, pos.x + 1, pos.y + 1, danger);
        CheckMove(moves, pos.x - 1, pos.y + 1, danger);
        CheckMove(moves, pos.x + 1, pos.y - 1, danger);
        CheckMove(moves, pos.x - 1, pos.y - 1, danger);
        
        return moves;
    }
}

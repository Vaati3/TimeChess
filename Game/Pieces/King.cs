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

    // public bool IsCheck(int x, int y)
    // {
    //     foreach (Piece piece in board.pieces)
    //     {
    //         if (piece.colour != colour)
    //         {
    //             foreach (Move move in piece.GetPosibleMoves())
    //             {
    //                 if (move.piece != null && move.piece == this)
    //                 {
    //                     return true;
    //                 }
    //             }
    //         }
    //     }
    //     return false;
    // }

//add check/checkmate and prevent selfcheck
    public override List<Move> GetPosibleMoves()
    {
        List<Move> moves = new List<Move>();

        CheckMove(moves, pos.x + 1, pos.y);
        CheckMove(moves, pos.x - 1, pos.y);
        CheckMove(moves, pos.x, pos.y - 1);
        CheckMove(moves, pos.x, pos.y + 1);
        CheckMove(moves, pos.x + 1, pos.y + 1);
        CheckMove(moves, pos.x - 1, pos.y + 1);
        CheckMove(moves, pos.x + 1, pos.y - 1);
        CheckMove(moves, pos.x - 1, pos.y - 1);
        
        return moves;
    }
}

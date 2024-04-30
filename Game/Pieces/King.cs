using Godot;
using System;
using System.Collections.Generic;

public class King : Piece
{
    public Piece attacker {get; set;}
    private ColorRect checkSignal;
    public override void Init(Board board, Colour colour, int x, int y)
    {
        base.Init(board, colour, x, y);
        attacker = null;
        checkSignal = new ColorRect();
        checkSignal.RectSize = new Vector2(50, 50);
        checkSignal.Color = Godot.Colors.OrangeRed;
        checkSignal.Visible = false;
        AddChild(checkSignal);
        MoveChild(checkSignal, 0);
        if (colour == Colour.Black)
        {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/BlackKing.png");
        } else {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/WhiteKing.png");
        }
    }

    public void UnCheck()
    {
        checkSignal.Visible = false;
    }

    public bool IsCheck()
    {
        List<Move> danger = board.GetAllPiecesMoves(colour);
        bool isCheck = false;
    
            foreach (Move move in danger)
            {
                if(move.canCapture && move.target == this)
                {
                    isCheck = true;
                    attacker = move.piece;
                    break;
                }
            }

        checkSignal.Visible = isCheck;
        return isCheck;
    }

    private void CheckCastling(List<Move> moves, List<Move> danger)
    {
        int y = colour == Colour.Black ? 0 : 7;
        List<Move> buffer = new List<Move>();

        if (board.pieces[0,y] != null && board.pieces[0,y].previousMoves.Count == 0)
        {
            if (!(CheckMove(buffer, 1, y, danger) || CheckMove(buffer, 2, y, danger) 
                || CheckMove(buffer, 3, y, danger)))
            {
                moves.Add(new Move(this, 2, y, false, false, board.pieces[0,y], true));
            }
        }
        if (board.pieces[7,y] != null && board.pieces[7,y].previousMoves.Count == 0)
        {
            if (!(CheckMove(buffer, 6, y, danger) || CheckMove(buffer, 5, y, danger)))
            {
                moves.Add(new Move(this, 6, y, false, false, board.pieces[7,y], true));
            }
        }
    }

    public void PerformCastling(Move move)
    {
        board.sfxManager.Play(1);
        MovePiece(new Move(this, move.pos.x, move.pos.y, false));
        int x = move.pos.x == 2 ? 3 : 5;
        move.target.MovePiece(new Move(move.target, x, move.pos.y, false));
        TogglePreviews();
        board.NextTurn(move, colour);
    }

    protected bool CheckMove(List<Move> moves, int x, int y, List<Move> danger)
    {
        Vector2i newPos = new Vector2i(x, y);
        foreach (Move move in danger)
        {
            if(move.canCapture && move.pos == newPos)
                return true;
        }
        return CheckMove(moves, x, y);
    }

    public override List<Move> GetPosibleMoves()
    {
        List<Move> moves;

        if (IsTurn())
        {
            board.pieces[pos.x, pos.y] = null;
            List<Move> danger = board.GetAllPiecesMoves(colour);
            board.pieces[pos.x, pos.y] = this;
            moves = GetPosibleMoves(danger);
            if (previousMoves.Count == 0 && !checkSignal.Visible)
                CheckCastling(moves, danger);
        } else {
            moves = new List<Move>();
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

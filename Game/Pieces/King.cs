using Godot;
using System;
using System.Collections.Generic;

public class King : Piece
{
    public Piece attacker {get; set;}
    private ColorRect checkSignal;
    public override void Init(Board board, Colour colour, int x, int y)
    {
        value = -1;
        if (colour == Colour.Black)
        {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/BlackKing.png");
        } else {
            GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Game/Pieces/Sprites/WhiteKing.png");
        }
        base.Init(board, colour, x, y);
        attacker = null;
        checkSignal = new ColorRect
        {
            RectSize = new Vector2(50, 50),
            Color = Colors.OrangeRed,
            Visible = false,
            MouseFilter = MouseFilterEnum.Ignore
        };
        AddChild(checkSignal);
        MoveChild(checkSignal, 0);
    }

    public void UnCheck()
    {
        checkSignal.Visible = false;
    }

    public bool IsCheck()
    {
        Colour otherColour = colour == Colour.Black ? Colour.White : Colour.Black;
        List<Move> danger = board.GetAllPiecesMoves(otherColour);
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

    public void PerformCastling(Move move, bool noPreview = false)
    {
        board.sfxManager.Play(1);
        board.MoveLastMove(pos, move.pos);
        MovePiece(new Move(this, move.pos.x, move.pos.y, false));
        int x = move.pos.x == 2 ? 3 : 5;
        move.target.MovePiece(new Move(move.target, x, move.pos.y, false));
        if (!noPreview)
            TogglePreviews();
        board.NextTurn(move, colour);
    }

    protected void TimeTravel(List<Move> moves, List<Move> danger)
    {
        if (previousMoves.Count == 0)
            return;
        for (int i = previousMoves.Count -1; i >= 0; i--)
        {
            Move move = previousMoves[i];
            int cost = previousMoves.Count - i;
            if (cost > board.timeFuel[(int)colour])
                return;
            if (MoveAlreadyExist(move, moves) || MoveAlreadyExist(move, danger))
                continue;
            if (board.pieces[move.origin.x, move.origin.y] == null || board.pieces[move.origin.x, move.origin.y].colour != colour)
            {
                moves.Add(new Move(this, move, cost, board.pieces[move.origin.x, move.origin.y]));
            }
        }
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
            Colour otherColour = colour == Colour.Black ? Colour.White : Colour.Black;
            List<Move> danger = board.GetAllPiecesMoves(otherColour);
            board.pieces[pos.x, pos.y] = this;
            moves = GetPosibleMoves(danger);
            if (previousMoves.Count == 0 && !checkSignal.Visible)
                CheckCastling(moves, danger);
            if (board.settings.kingTimeTravel && !checkSignal.Visible)
                TimeTravel(moves, danger);
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

using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public enum Colour
{
    Black,
    White
}

public struct Move{
    public Vector2i pos;
    public Vector2i origin;
    public Piece piece;
    public Piece target;
    public int timeTravelCost;
    public int turn;
    public bool canCapture;
    public bool noPreview;
    public bool isCastling;
    public Move(Piece piece, int x, int y, bool canCapture = true, bool noPreview = false, Piece target = null, bool isCastling = false)
    {
        this.piece = piece;
        origin = piece.pos;
        pos.x = x;
        pos.y = y;
        turn = -1;
        timeTravelCost = 0;
        this.canCapture = canCapture;
        this.target = target;
        this.noPreview = noPreview;
        this.isCastling = isCastling;
    }
    public Move(Piece piece, Move move, int timeTravelCost, Piece target = null)
    {
        this.piece = piece;
        origin = piece.pos;
        pos = move.origin;
        turn = -1;
        this.timeTravelCost = timeTravelCost;
        canCapture = move.canCapture;
        this.target = target;
        noPreview = false;
        isCastling = false;
    }
}

public abstract class Piece : Node2D
{
    public Colour colour { get; private set;}
    public Vector2i pos { get; private set;}
    public List<Move> previousMoves { get; set;}
    protected Board board { get; private set;}
    protected bool isPreviewing { get; private set;}
    public bool needsUpdate { get; set;}
    protected List<MovePreview> previews { get; private set;}
    public bool kingIsCheck { get; set;}

    public virtual void Init(Board board, Colour colour, int x, int y)
    {
        this.board = board;
        this.colour = colour;
        pos = new Vector2i(x, y);
        Position = pos * board.tileSize;

        isPreviewing = false;
        needsUpdate = true;
        kingIsCheck = false;
        previousMoves = new List<Move>();
    }

    public abstract List<Move> GetPosibleMoves();

    protected bool MoveAlreadyExist(Move previousMove, List<Move> moves)
    {
        foreach(Move move in moves)
        {
            if (previousMove.origin == move.pos)
            {
                return true;
            }
        }
        return false;
    }
    protected void TimeTravel(List<Move> moves)
    {
        if (previousMoves.Count == 0)
            return;
        for (int i = previousMoves.Count -1; i >= 0; i--)
        {
            Move move = previousMoves[i];
            int cost = previousMoves.Count - i;
            if (cost > board.timeFuel[(int)colour])
                return;
            if (board.pieces[move.origin.x, move.origin.y] != null && 
                (!board.settings.timeTravelCapture || board.pieces[move.origin.x, move.origin.y].colour == colour))
                continue;
            if (MoveAlreadyExist(move, moves))
                continue;
            if (board.pieces[move.origin.x, move.origin.y] == null || board.pieces[move.origin.x, move.origin.y].colour != colour)
            {
                moves.Add(new Move(this, move, cost, board.pieces[move.origin.x, move.origin.y]));
            }
        }
    }

    protected bool CheckMove(List<Move> moves, int x, int y, bool canCapture = true, bool onlyCapture = false)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7)
            return true;
        if (board.pieces[x, y] == null)
        {
            if (!onlyCapture)
                moves.Add(new Move(this, x, y, canCapture));
            else if(canCapture)
                moves.Add(new Move(this, x, y, canCapture, true));
            return false;
        }
        if (canCapture && board.pieces[x, y].colour != colour)
            moves.Add(new Move(this, x, y, true, false, board.pieces[x, y]));
        if (board.pieces[x, y].colour == colour)
            moves.Add(new Move(this, x, y, canCapture, true));
        return true;
    }

    public void MovePiece(Move move)
    {
        board.pieces[pos.x, pos.y] = null;
        pos = move.pos;
        Position = pos * board.tileSize;
        if (move.target != null)
        {
            board.pieces[move.target.pos.x, move.target.pos.y] = null;
            move.target.QueueFree();
        }
        board.pieces[pos.x, pos.y] = this;
        move.turn = board.turn;
        previousMoves.Add(move);
        board.UpdateTimeFuel(move.timeTravelCost, colour);
    }

    public virtual void PerformMove(Move move)
    {
        if (move.timeTravelCost > 0)
            board.sfxManager.Play(3);
        else
            board.sfxManager.Play(1);
        MovePiece(move);
        board.kings[(int)colour].UnCheck();
        TogglePreviews();
        board.NextTurn(move, colour);
    }

    protected void CreatePreviews()
    {
        foreach(MovePreview preview in previews)
        {
            preview.QueueFree();
        }
        previews.Clear();
        List<Move> moves = kingIsCheck ? DefendKing() : GetPosibleMoves();
        PackedScene scene = GD.Load<PackedScene>("res://Game/MovePreview.tscn");

        if (moves == null || moves.Count == 0)
            return;
        foreach (Move move in moves)
        {
            if (move.noPreview)
                continue;
            MovePreview preview = scene.Instance<MovePreview>();
            preview.Init(move, board.tileSize);
            AddChild(preview);
            previews.Add(preview);
        }
    }

    protected void TogglePreviews()
    {
        if (needsUpdate)
        {
            previews = new List<MovePreview>();
            CreatePreviews();
            needsUpdate = false;
        }
        isPreviewing = !isPreviewing;
        foreach(MovePreview preview in previews)
        {
            preview.Visible = !preview.Visible;
        }       
    }

    protected bool IsTurn()
    {
        return (colour == Colour.Black && board.turn % 2 == 0) || (colour == Colour.White && board.turn % 2 != 0);
    }

    public List<Move> DefendKing()
    {
        List<Move> limitedMoves = new List<Move>();
        List<Move> moves = GetPosibleMoves();
        
        if (moves == null || moves.Count == 0)
            return limitedMoves;
        board.pieces[pos.x, pos.y] = null;
        foreach (Move move in moves)
        {
            bool stillCheck = false;
            if (move.noPreview)
                continue;
            board.pieces[move.pos.x, move.pos.y] = this;
            if (move.target != board.kings[(int)colour].attacker)
            {
                foreach (Move attack in board.kings[(int)colour].attacker.GetPosibleMoves())
                {
                    if (attack.target != null && attack.target == board.kings[(int)colour])
                    {
                        stillCheck = true;
                        break;
                    }
                }
            }
            if (!stillCheck)
                limitedMoves.Add(move);
            board.pieces[move.pos.x, move.pos.y] = move.target;
        }
        board.pieces[pos.x, pos.y] = this;
        return limitedMoves;
    }

    public void _on_Button_pressed()
    {
        if (!IsTurn() || this != board.pieces[pos.x, pos.y])
            return;
        if (!kingIsCheck && GetType() != typeof(King))
        {
            board.pieces[pos.x, pos.y] = null;
            bool isCheck = board.kings[(int)colour].IsCheck();
            board.pieces[pos.x, pos.y] = this;
            if (isCheck)
            {
                board.kings[(int)colour].UnCheck();
                kingIsCheck = true;
            }
        }
        if (!isPreviewing)
        {
            board.sfxManager.Play(0);
            TogglePreviews();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (isPreviewing)
        {
            if (@event is InputEventMouseButton mouse)
            {
                if (mouse.Pressed)
                    return;
                bool clickOut = true;
                foreach (MovePreview preview in previews)
                {
                    if (preview.CheckMouse(mouse.Position, board.Scale))
                    {
                        clickOut = false;
                        break;
                    }
                }
                if (clickOut)
                    TogglePreviews();
            }
        }
    }
}

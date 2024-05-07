using Godot;
using System;
using System.Collections.Generic;

public class AI : Node
{
    RandomNumberGenerator rng;
    Timer timer;
    Board board;
    bool isChecked = false;
    public Colour colour {get; set;}
    public AI(Board board)
    {
        this.board = board;
        rng = new RandomNumberGenerator();
        rng.Randomize();
        timer = new Timer();
        AddChild(timer);
        timer.OneShot = true;
        timer.Connect("timeout", this, "PlayMove");
        board.Connect("AITurn", this, "_on_AI_turn");
    }

    private List<Move>GetPossibleMoves()
    {
        List<Move> moves = board.GetAllPiecesMoves(colour, isChecked);
        List<Move> possibleMoves = new List<Move>();

        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i].noPreview)
                continue;
            if (moves[i].piece.GetType() == typeof(Pawn) && moves[i].canCapture && moves[i].target == null)
                continue;
            if (!moves[i].piece.kingIsCheck && moves[i].piece.GetType() != typeof(King))
            {
                board.pieces[moves[i].piece.pos.x, moves[i].piece.pos.y] = null;
                bool isCheck = board.kings[(int)colour].IsCheck();
                board.pieces[moves[i].piece.pos.x, moves[i].piece.pos.y] = moves[i].piece;
                    if (isCheck)
                    {
                        board.kings[(int)colour].UnCheck();
                        Vector2i pos = moves[i].piece.pos;
                        while (moves[i].piece.pos == pos)
                            i++;
                        continue;
                    }
            }
            possibleMoves.Add(moves[i]);
        }

        return possibleMoves;
    }

    private void PlayMove()
    {
        List<Move> moves = GetPossibleMoves();

        if (moves.Count == 0)
            return;
        int index = rng.RandiRange(0, moves.Count-1);
        if (moves[index].isCastling)
            ((King)moves[index].piece).PerformCastling(moves[index]);
        else
            moves[index].piece.PerformMove(moves[index], true);
    }

    public void _on_AI_turn(bool isChecked)
    {
        this.isChecked = isChecked;
        timer.Start(1);
    }
}

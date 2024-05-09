using Godot;
using System;
using System.Collections.Generic;

//called AI but no intelligence is present here
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

    private List<Move> Agressivity(List<Move> moves)
    {
        int bestValue = -1;

        foreach(Move move in moves)
        {
            if (move.target != null)
                bestValue = Math.Max(bestValue, move.target.value);
        }
        if (bestValue < 0)
            return moves;

        List<Move> attacks = new List<Move>();
        foreach(Move move in moves)
        {
            if (move.target != null)
            {
                if (move.target.value == bestValue)
                    attacks.Add(move);
            }
        }
        return attacks;
    }

    private void PlayMove()
    {
        List<Move> moves = Agressivity(GetPossibleMoves());

        if (moves.Count == 0)
            return;
        int index = rng.RandiRange(0, moves.Count-1);
        if (moves[index].isCastling)
            ((King)moves[index].piece).PerformCastling(moves[index], true);
        else
            moves[index].piece.PerformMove(moves[index], true);
    }

    public void _on_AI_turn(bool isChecked)
    {
        this.isChecked = isChecked;
        timer.Start(0.75f);
    }
}

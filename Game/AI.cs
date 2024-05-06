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
        List<Move> allMoves = board.GetAllPiecesMoves(colour, isChecked);
        List<Move> moves = new List<Move>();

        for (int i = 0; i < allMoves.Count; i++)
        {
            if (allMoves[i].noPreview)
                continue;
            if (allMoves[i].piece.GetType() == typeof(Pawn) && allMoves[i].canCapture && allMoves[i].target == null)
                continue;
            moves.Add(allMoves[i]);
        }

        return moves;
    }

    private void PlayMove()
    {
        List<Move> moves = GetPossibleMoves();

        int index = rng.RandiRange(0, moves.Count-1);
        // GD.Print(moves.Count + " " + index);
        moves[index].piece.PerformMove(moves[index], true);
    }

    public void _on_AI_turn(bool isChecked)
    {
        this.isChecked = isChecked;
        timer.Start(1);
    }
}

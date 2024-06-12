using Godot;
using System;
using System.Collections.Generic;

struct AIMove {
    public Move? move;
    public int value;

    public AIMove(Move? move, int value)
    {
        this.move = move;
        this.value = value;
    }
}

//called AI but no intelligence is present here
public class AI : Node
{
    RandomNumberGenerator rng;
    Timer timer;
    Board board;
    public Colour colour {get; set;}

    public AI()
    {

    }
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

    private Move GetRandomMove(List<Move> moves)
    {
        if (moves.Count == 0)
            return new Move();
        int index = rng.RandiRange(0, moves.Count-1);
        return moves[index];
    }

    private AIMove MoveAndMinMax(Move move, int depth, Colour colour, int alpha, int beta)
    {
        Colour otherColour = colour == Colour.White ? Colour.Black : Colour.White; 
        board.pieces[move.piece.pos.x, move.piece.pos.y] = null;
        board.pieces[move.pos.x, move.pos.y] = move.piece;
        if (move.target != null)
            board.boardValues[(int)otherColour] -= move.target.value;
        AIMove current = MinMax(depth, otherColour, alpha, beta);
        board.pieces[move.piece.pos.x, move.piece.pos.y] = move.piece;
        board.pieces[move.pos.x, move.pos.y] = move.target;
        if (move.target != null)
            board.boardValues[(int)otherColour] += move.target.value;
        return current;
    }

    private AIMove MinMax(int depth, Colour colour, int alpha, int beta)
    {
        if (depth == 0 || board.checkmate)
            return new AIMove(null, GetBoardValue());
        List<Move> moves = board.GetAllPiecesMoves(colour, false, true);
        Move bestMove = GetRandomMove(moves);
        moves.Remove(bestMove);

        if (colour == this.colour)
        {
            int max = MoveAndMinMax(bestMove, depth-1, colour, alpha, beta).value;
            foreach (Move move in moves)
            {
                AIMove current = MoveAndMinMax(move, depth-1, colour, alpha, beta);
                if (current.value > max)
                {
                    max = current.value;
                    bestMove = move;
                }
                alpha = Math.Max(alpha, current.value);
                if (beta <= alpha)
                    break;
            }
            return new AIMove(bestMove, max);
        } else {
            int min = MoveAndMinMax(bestMove, depth-1, colour, alpha, beta).value;
            foreach (Move move in moves)
            {
                AIMove current = MoveAndMinMax(move, depth-1, colour, alpha, beta);
                if (current.value < min)
                {
                    min = current.value;
                    bestMove = move;
                }
                beta = Math.Min(beta, current.value);
                if (beta <= alpha)
                    break;
            }
            return new AIMove(bestMove, min);
        }
    }

    private int GetBoardValue()
    {
        if (colour == Colour.White)
            return board.boardValues[1] - board.boardValues[0];
        return board.boardValues[0] - board.boardValues[1];
    }

   private void PlayMove()
    {
        AIMove aiMove = MinMax(3, colour, -3000, 3000);
        GD.Print(aiMove.value);

        if (aiMove.move == null)
            return;
        Move move = aiMove.move.Value;
        if (aiMove.move.Value.isCastling)
            ((King)move.piece).PerformCastling(move, true);
        else
            move.piece.PerformMove(move, true);
    }


    public void _on_AI_turn(    )
    {
        timer.Start(0);
    }
}

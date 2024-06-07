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

    private Move? GetRandomMove(List<Move> moves)
    {
        if (moves.Count == 0)
            return null;
        int index = rng.RandiRange(0, moves.Count-1);
        return moves[index];
    }

    private AIMove MoveAndMinMax(Move move, int depth, Colour colour, Colour otherColour)
    {
        Piece savePieces = board.pieces[move.pos.x, move.pos.y];
        int boardvalue = board.boardValues[(int)otherColour];
        board.pieces[move.piece.pos.x, move.piece.pos.y] = null;
        board.pieces[move.pos.x, move.pos.y] = move.piece;
        if (savePieces != null)
            board.boardValues[(int)otherColour] -= savePieces.value;
        AIMove current = MinMax(depth, otherColour);
        board.pieces[move.piece.pos.x, move.piece.pos.y] = move.piece;
        board.pieces[move.pos.x, move.pos.y] = savePieces;
        if (savePieces != null)
            board.boardValues[(int)otherColour] = boardvalue;
        return current;
    }

    private AIMove MinMax(int depth, Colour colour)
    {
        if (depth == 0 || board.checkmate)
            return new AIMove(null, GetBoardValue());
        Colour otherColour = colour == Colour.White ? Colour.Black : Colour.White; 
        List<Move> moves = board.GetAllPiecesMoves(colour, false, true);
        Move? bestMove = GetRandomMove(moves);

        if (colour == this.colour)
        {
            int max = -3000;
            foreach (Move move in moves)
            {
                AIMove current = MoveAndMinMax(move, depth-1, colour, otherColour);
                if (current.value > max)
                {
                    max = current.value;
                    bestMove = move;
                }
            }
            return new AIMove(bestMove, max);
        } else {
            int min = 3000;
            foreach (Move move in moves)
            {
                AIMove current = MoveAndMinMax(move, depth-1, colour, otherColour);
                if (current.value < min)
                {
                    min = current.value;
                    bestMove = move;
                }
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
        AIMove aiMove = MinMax(2, colour);

        if (aiMove.move == null)
            return;
        if (aiMove.move.Value.isCastling)
            ((King)aiMove.move.Value.piece).PerformCastling(aiMove.move.Value, true);
        else
            aiMove.move.Value.piece.PerformMove(aiMove.move.Value, true);
    }


    public void _on_AI_turn(    )
    {
        timer.Start(0);
    }
}

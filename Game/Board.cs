using Godot;
using System;
using System.Collections.Generic;

public struct Vector2i
{
    public int x;
    public int y;
    public Vector2i(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    static public Vector2 operator *(Vector2i pos, float n)
    {
        return new Vector2((float)pos.x * n, (float)pos.y * n);
    }
}

public class Board : Node2D
{
    public float tileSize { get; private set;}
    public Piece[,] pieces { get; private set;}
    public List<Move> allMoves { get; private set;}
    public int turn { get; private set;}
    public int blackFuel { get; private set;}
    public int whiteFuel { get; private set;}

    private void InstanciatePiece(PackedScene scene, string type, Colour colour, int x, int y)
    {
        Piece inst = scene.Instance<Piece>();
        ulong id = inst.GetInstanceId();
        inst.SetScript(GD.Load<Script>("res://Game/Pieces/" + type + ".cs")); //SetProcess(true); to activate process after SetScript
        pieces[x,y] = (Piece)GD.InstanceFromId(id);
        pieces[x,y].Init(this, colour, x, y);
        AddChild(pieces[x,y]);
    }
    private void ResetPieces()
    {
        PackedScene scene = GD.Load<PackedScene>("res://Game/Pieces/Piece.tscn");
        for (int x = 0; x < 8; x++)
        {
            InstanciatePiece(scene, "Pawn", Colour.Black, x, 1);
            InstanciatePiece(scene, "Pawn", Colour.White, x, 6);

            string type = "King";
            switch (x) {
                case 0: case 7:
                    type = "Rook"; break;
                case 1: case 6:
                    type = "Knight"; break;
                case 2: case 5:
                    type = "Bishop"; break;
                case 4:
                    type = "Queen"; break;
            }
            InstanciatePiece(scene, type, Colour.Black, x, 0);
            InstanciatePiece(scene, type, Colour.White, x, 7);
        }
        InstanciatePiece(scene, "King",  Colour.Black, 4, 4);
        InstanciatePiece(scene, "Queen",  Colour.White, 3, 3);
    }
    public void NextTurn(Move lastMove)
    {
        turn++;
        allMoves.Add(lastMove);
    }

    public override void _Ready()
    {
        tileSize = 50;
        turn = 1;
        pieces = new Piece[8,8];
        Scale = new Vector2(1.5f, 1.5f);
        ResetPieces();
        allMoves = new List<Move>();
    }
}

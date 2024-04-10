using Godot;
using System;

public class Board : Node2D
{
    private Piece[,] pieces;

    private void InstanciatePiece(PackedScene scene, string type, Colour colour, int x, int y)
    {
        Piece inst = scene.Instance<Piece>();
        ulong id = inst.GetInstanceId();
        inst.SetScript(GD.Load<Script>("res://Game/Pieces/" + type + ".cs")); //SetProcess(true); to activate process after SetScript
        pieces[x,y] = (Piece)GD.InstanceFromId(id);
        pieces[x,y].Init(colour, x, y);
        AddChild(pieces[x,y]);
    }
    private void ResetPieces()
    {
        PackedScene scene = GD.Load<PackedScene>("res://Game/Pieces/Piece.tscn");
        for (int x = 0; x < 8; x++)
        {
            InstanciatePiece(scene, "Pawn", Colour.Black, x, 1);
            InstanciatePiece(scene, "Pawn", Colour.White, x, 6);
        }
    }

    public override void _Ready()
    {
        pieces = new Piece[8,8];
        ResetPieces();
    }
}

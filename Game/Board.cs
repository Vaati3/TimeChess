using Godot;
using System;

public class Board : Node2D
{
    public float tileSize { get; private set;}
    public Piece[,] pieces { get; private set;}

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
    }

    private void ScaleBoard(float scale)
    {
        tileSize *= scale;
        Vector2 scaleVector = new Vector2(scale, scale);
        Control boardControl = GetNode<Control>("CanvasLayer/BoardControl");
        boardControl.RectScale = scaleVector;

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (pieces[x,y] != null)
                {
                    pieces[x,y].UpdateScale(scaleVector);
                }
            }
        }
    }

    public override void _Ready()
    {
        tileSize = 50;
        pieces = new Piece[8,8];
        ResetPieces();
        ScaleBoard(1.5f);
    }
}

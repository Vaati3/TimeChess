using Godot;
using System;

public class GameMenu : Panel
{
    Board board;

    [Signal]
    public delegate void BackToMenu();
    public void Init(Board board)
    {
        this.board = board;
    }

    public void Checkmate(Colour winner)
    {
        Label label = GetNode<Label>("Label");

        label.Visible = true;
        label.Text = "Checkmate\n" + winner + "s Win";
        GetNode<Button>("VBoxContainer/ResumeButton").Visible = false;
        Visible = true;
    }

    public void _on_ResumeButton_pressed()
    {
        Visible = false;
    }

    public void _on_RestartButton_pressed()
    {
        board.ResetPieces();
        Visible = false;
    }

    public void _on_BackButton_pressed()
    {
        Visible = false;
        EmitSignal(nameof(BackToMenu));
    }
}

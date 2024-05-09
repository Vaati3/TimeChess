using Godot;
using System;

public class GameMenu : Panel
{
    bool isCheckmate = false;
    SFXManager sfxManager;

    [Signal]
    public delegate void BackToMenu();

    [Signal]
    public delegate void RestartGame();
    [Signal]
    public delegate void ResumeGame();

    public override void _Ready()
    {
        sfxManager = GetNode<SFXManager>("/root/SFXManager");
    }

    public void Checkmate(Colour winner)
    {
        Label label = GetNode<Label>("Label");

        label.Visible = true;
        label.Text = "Checkmate\n" + winner + "s  have won";
        GetNode<Button>("VBoxContainer/ResumeButton").Visible = false;
        Visible = true;
        isCheckmate = true;
    }

    public void _on_ResumeButton_pressed()
    {
        sfxManager.Play(2);
        Visible = false;
        EmitSignal(nameof(ResumeGame));
    }

    public void _on_RestartButton_pressed()
    {
        if (isCheckmate)
        {
            GetNode<Label>("Label").Visible = false;
            GetNode<Button>("VBoxContainer/ResumeButton").Visible = true;
            isCheckmate = false;
        }
        EmitSignal(nameof(RestartGame));
        Visible = false;
    }

    public void _on_BackButton_pressed()
    {
        if (isCheckmate)
        {
            GetNode<Label>("Label").Visible = false;
            GetNode<Button>("VBoxContainer/ResumeButton").Visible = true;
            isCheckmate = false;
        }
        sfxManager.Play(2);
        Visible = false;
        EmitSignal(nameof(BackToMenu));
    }
}

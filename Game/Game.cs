using Godot;
using System;

public class Game : Control
{
    GameMenu menu;
    Board board;
    public override void _Ready()
    {
        board = GetNode<Board>("Board");
        menu = GetNode<GameMenu>("GameMenu");

        menu.Init(board);
       //board.Scale = new Vector2(1.5f, 1.5f);
    }

    public void _on_Board_Checkmate(Colour winner)
    {
        GD.Print("signal received");
        menu.Checkmate(winner);
    }

    public void _on_MenuButton_pressed()
    {
        menu.Visible = true;
    }

    public void _on_GameMenu_BackToMenu()
    {
        Visible = false;
    }
}

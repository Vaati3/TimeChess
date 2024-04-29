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
        menu.Checkmate(winner);
    }

    public void _on_Board_TimeTravel(int[] timeFuel, Colour colour)
    {
        if (colour == Colour.Black)
            GetNode<Label>("BlackFuel").Text = "Fuel: " + timeFuel[(int)colour].ToString();
        else
            GetNode<Label>("WhiteFuel").Text = "Fuel: " + timeFuel[(int)colour].ToString();
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

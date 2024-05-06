using Godot;
using System;

public class Game : Control
{
    Menu startMenu;
    GameMenu menu;
    Board board;
    AI ai;
    public override void _Ready()
    {
        board = GetNode<Board>("BoardControl/Board");
        menu = GetNode<GameMenu>("GameMenu");
        ai = new AI(board);
        AddChild(ai);

        menu.Init(board);
        startMenu = GetParent<Menu>();
    }

    public void StartGame(Settings settings)
    {
        Visible = true;
        board.settings = settings;
        if (settings.playAI)
            ai.colour = settings.AIColour;
        board.ResetPieces();
    }

    public void _on_Board_Checkmate(Colour winner)
    {
        menu.Checkmate(winner);
    }

    public void _on_Board_TimeTravel(int[] timeFuel, Colour colour)
    {
        if (colour == Colour.Black)
            GetNode<Label>("BoardControl/BlackFuel").Text = "Fuel: " + timeFuel[(int)colour].ToString();
        else
            GetNode<Label>("BoardControl/WhiteFuel").Text = "Fuel: " + timeFuel[(int)colour].ToString();
    }

    public void _on_MenuButton_pressed()
    {
        startMenu.sfxManager.Play(2);
        menu.Visible = true;
    }

    public void _on_GameMenu_BackToMenu()
    {
        Visible = false;
        startMenu.ToggleMenu(true);
    }
}

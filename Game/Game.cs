using Godot;
using System;

public class Game : Control
{
    Menu startMenu;
    GameMenu menu;
    Board board;
    AI ai;
    VBoxContainer moveList;
    Label lastMove;
    public override void _Ready()
    {
        board = GetNode<Board>("BoardControl/Board");
        menu = GetNode<GameMenu>("GameMenu");
        ai = new AI(board);
        AddChild(ai);

        startMenu = GetParent<Menu>();
        moveList = GetNode<VBoxContainer>("MovesPanel/Moves");
    }

    public void StartGame(Settings settings)
    {
        Visible = true;
        board.settings = settings;
        if (settings.playAI)
            ai.colour = settings.AIColour;
        board.ResetPieces();
        ClearMoveList();
    }

    private void ClearMoveList()
    {
        foreach(Node child in moveList.GetChildren())
        {
            child.QueueFree();
        }
    }

    public void _on_Board_MoveOver(string notation)
    {
        int turn = board.turn - 1;
        if (turn % 2 != 0)
        {
            if (moveList.GetChildCount() >= 20)
                moveList.GetChild(0).QueueFree();
            lastMove = new Label
            {
                Text = turn/2 + "|  " + notation
            };
            moveList.AddChild(lastMove);
        } else {
            lastMove.Text += ",  " + notation;
        }
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

    public void _on_GameMenu_RestartGame()
    {
        board.sfxManager.Play(2);
        board.ResetPieces();
        ClearMoveList();
    }
}

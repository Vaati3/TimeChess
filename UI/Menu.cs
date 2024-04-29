using Godot;
using System;

public class Menu : Control
{
	Game game;

	int maxFuel = 10;
	public override void _Ready()
	{
		game = GD.Load<PackedScene>("res://Game/Game.tscn").Instance<Game>();
		AddChild(game);
		game.Visible = false;
	}

	public void ToggleMenu(bool state)
	{
		GetNode<Panel>("StartMenu").Visible = state;
	}

	private void ChangeMaxFuel(int change)
	{
		maxFuel += change;
		GetNode<Label>("OptionsMenu/FuelLabel").Text = maxFuel.ToString();
	}

	//Menu Buttons Signals
	public void _on_PlayBtn_pressed()
	{
		game.Visible = true;
		game.StartGame(maxFuel);
		ToggleMenu(false);
	}
	public void _on_OptionBtn_pressed()
	{
		ToggleMenu(false);
		GetNode<Panel>("OptionsMenu").Visible = true;
	}
	public void _on_QuitBtn_pressed()
	{
		GetTree().Quit();
	}

	//Button Options signal
	public void _on_QuitOptionBtn_pressed()
	{
		GetNode<Panel>("OptionsMenu").Visible = false;
		ToggleMenu(true);
	}

	public void _on_AddFuelBtn_pressed()
	{
		ChangeMaxFuel(1);
	}

	public void _on_RemoveFuelBtn_pressed()
	{
		ChangeMaxFuel(-1);
	}
}

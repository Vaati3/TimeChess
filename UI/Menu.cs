using Godot;
using System;
using System.Media;

public struct Settings{
	public int maxFuel;
	public bool kingTimeTravel;

	public Settings(int maxFuel, bool kingTimeTravel = false)
	{
		this.maxFuel = maxFuel;
		this.kingTimeTravel = kingTimeTravel;
	}
}

public class Menu : Control
{
	Game game;
	Settings settings;
	public override void _Ready()
	{
		game = GD.Load<PackedScene>("res://Game/Game.tscn").Instance<Game>();
		AddChild(game);
		game.Visible = false;
		settings = new Settings(10);
	}

	public void ToggleMenu(bool state)
	{
		GetNode<Panel>("StartMenu").Visible = state;
	}

	private void ChangeMaxFuel(int change)
	{
		settings.maxFuel += change;
		GetNode<Label>("OptionsMenu/FuelLabel").Text = settings.maxFuel.ToString();
	}

	//Menu Buttons Signals
	public void _on_PlayBtn_pressed()
	{
		game.Visible = true;
		game.StartGame(settings);
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

	public void _on_CheckBox_toggled(bool state)
	{
		settings.kingTimeTravel = state;
	}
}

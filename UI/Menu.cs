using Godot;
using System;

public class Menu : Control
{
	Game game;
	public override void _Ready()
	{
		game = GD.Load<PackedScene>("res://Game/Game.tscn").Instance<Game>();
		AddChild(game);
		game.Visible = false;
	}

	//Buttons Signals
	public void _on_PlayBtn_pressed()
	{
		game.Visible = true;
	}
	public void _on_OptionBtn_pressed()
	{
		GD.Print("Open Options");
	}
	public void _on_QuitBtn_pressed()
	{
		GetTree().Quit();
	}
}

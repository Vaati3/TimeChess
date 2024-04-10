using Godot;
using System;

public class Menu : Control
{
	public override void _Ready()
	{
	}

	//Buttons Signals
	public void _on_PlayBtn_pressed()
	{
		GetTree().ChangeScene("res://Game/Board.tscn");
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

using Godot;
using System;
using System.Media;

public struct Settings{
	public int maxFuel;
	public bool kingTimeTravel;
	public bool timeTravelCapture;	
	public bool playAI;
	public Colour AIColour;

	public Settings(int maxFuel)
	{
		this.maxFuel = maxFuel;
		this.kingTimeTravel = false;
		timeTravelCapture = true;

		playAI = false;
		AIColour = Colour.Black;
	}
}

public class Menu : Control
{
	Game game;
	Settings settings;
	public SFXManager sfxManager { get; private set;}
	public override void _Ready()
	{
		game = GD.Load<PackedScene>("res://Game/Game.tscn").Instance<Game>();
		AddChild(game);
		game.Visible = false;
		settings = new Settings(10);

		sfxManager = GetNode<SFXManager>("/root/SFXManager");
		GD.Randomize();
	}

	public void ToggleMenu(bool state)
	{
		GetNode<Panel>("StartMenu").Visible = state;
	}

	private void ChangeMaxFuel(int change)
	{
		settings.maxFuel += change;
		GetNode<Label>("OptionsMenu/FuelValue").Text = settings.maxFuel.ToString();
	}

	//Menu Buttons Signals
	public void _on_PlayBtn_pressed()
	{
		sfxManager.Play(2);
		settings.playAI = false;
		game.StartGame(settings);
		ToggleMenu(false);
	}

	public void _on_PlayAIBtn_pressed()
	{
		sfxManager.Play(2);
		settings.playAI = true;
		ToggleMenu(false);
		GetNode<Panel>("AIColour").Visible = true;
	}

	public void _on_OptionBtn_pressed()
	{
		sfxManager.Play(2);
		ToggleMenu(false);
		GetNode<Panel>("OptionsMenu").Visible = true;
	}

	public void _on_QuitBtn_pressed()
	{
		sfxManager.Play(2);
		GetTree().Quit();
	}

	//Button Options signal
	public void _on_QuitOptionBtn_pressed()
	{
		sfxManager.Play(2);
		GetNode<Panel>("OptionsMenu").Visible = false;
		ToggleMenu(true);
	}

	public void _on_AddFuelBtn_pressed()
	{
		sfxManager.Play(2);
		ChangeMaxFuel(1);
	}

	public void _on_RemoveFuelBtn_pressed()
	{
		sfxManager.Play(2);
		ChangeMaxFuel(-1);
	}

	public void _on_KingCheckbox_toggled(bool state)
	{
		sfxManager.Play(2);
		settings.kingTimeTravel = state;
	}

	public void _on_CaptureCheckbox_toggled(bool state)
	{
		sfxManager.Play(2);
		settings.timeTravelCapture = state;
	}

	//colour select menu

	public void _on_WhiteBtn_pressed()
	{
		sfxManager.Play(2);
		settings.AIColour = Colour.Black;
		GetNode<Panel>("AIColour").Visible = false;
		game.StartGame(settings);
	}

	public void _on_BlackBtn_pressed()
	{
		sfxManager.Play(2);
		settings.AIColour = Colour.White;
		GetNode<Panel>("AIColour").Visible = false;
		game.StartGame(settings);
	}

	public void _on_RandomBtn_pressed()
	{
		sfxManager.Play(2);
		if (GD.Randi() % 2 == 0)
			settings.AIColour = Colour.Black;
		else
			settings.AIColour = Colour.White;
		GetNode<Panel>("AIColour").Visible = false;
		game.StartGame(settings);
	}
}

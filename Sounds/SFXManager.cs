using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class SFXManager : Node
{
    //string bus = "master";
    AudioStreamPlayer player;
    List<AudioStream> sounds;
    Queue<int> soundQueue;
    readonly string[] soundPaths = {
        "res://Sounds/SelectPiece.wav",
        "res://Sounds/PlayPiece.wav",
        "res://Sounds/Button.wav",
        "res://Sounds/TimeTravel.wav"
    };

    public override void _Ready()
    {
        player = new AudioStreamPlayer ();
        sounds = new List<AudioStream>();
        soundQueue = new Queue<int>();
        AddChild(player);

        foreach(string path in soundPaths)
        {
            AddSound(path);
        }
    }

    public override void _Process(float _delta)
    {
        if (soundQueue.Count != 0 && !player.Playing)
        {
            player.Stream = sounds[soundQueue.Dequeue()];
            player.Play();
        }
    }

    public void AddSound(string path)
    {
        sounds.Add(GD.Load<AudioStream>(path));
    }

    public void Play(int soundIndex)
    {
        soundQueue.Enqueue(soundIndex);
    }
}

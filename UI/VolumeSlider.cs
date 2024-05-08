using Godot;
using System;

public class VolumeSlider : HSlider
{
    [Export]
    public string label;
    [Export]
    public string bus;

    [Export]
    public float soundScale = 4;

    int busIndex;

    SFXManager sfxManager;

    public override void _Ready()
    {
        sfxManager = GetNode<SFXManager>("/root/SFXManager");
        GetNode<Label>("Label").Text = label;
        busIndex = AudioServer.GetBusIndex(bus);
    }

    private float LinearToDb(float linear)
    {
        return (float)(Math.Log(linear) * 8.6858896380650365530225783783321);
    }

    private void SetVolume(float volume)
    {
        AudioServer.SetBusVolumeDb(busIndex, LinearToDb(volume));
    }

    public void _on_VolumeSlider_drag_ended(bool valueChanged)
    {
        sfxManager.Play(2);
        if (valueChanged && !AudioServer.IsBusMute(busIndex))
        {
            SetVolume((float)Value * soundScale);
        }
    }


    public void _on_Mute_toggled(bool state)
    {
        sfxManager.Play(2);
        AudioServer.SetBusMute(busIndex, state);
    }
}

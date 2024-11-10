using System;
using Godot;

namespace leafy.environment;

public partial class KeyHandler : Node
{
	private AudioStreamPlayer _music;

	public override void _Ready()
	{
		base._Ready();
		_music = GetNode<AudioStreamPlayer>("/root/Ecosystem/Environment/Music");
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("reset"))
		{
			GetTree().ReloadCurrentScene();
		}

		if (Input.IsActionJustReleased("mute"))
		{
			if (_music.IsPlaying())
			{
				_music.Stop();
			}
			else
			{
				_music.Play();
			}
		}

		if (Input.IsActionJustReleased("decrease_volume"))
		{
			if (_music.IsPlaying() && _music.VolumeDb > -50)
			{
				_music.VolumeDb -= 5;
			}
		}
		
		if (Input.IsActionJustReleased("increase_volume"))
		{
			if (!_music.IsPlaying())
			{
				_music.VolumeDb = -20;
				
				_music.Play();
			} else if (_music.VolumeDb < 0)
			{
				_music.VolumeDb += 5;
			}
		}
	}
}

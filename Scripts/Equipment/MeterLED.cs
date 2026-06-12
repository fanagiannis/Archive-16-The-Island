using Godot;
using System;

public partial class MeterLED : MeshInstance3D
{
	[Export] Material ledONMaterial;
	[Export] Material ledOFFMaterial;
	[Export] AudioStream audioEffect;
	private bool _On;
	private float BlinkTimer=9;
	private float BlinkTimerReset;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BlinkTimerReset = BlinkTimer;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void SetOn(bool set)
	{
		_On=set;
		if(_On == true)
		{   
			this.MaterialOverride = ledONMaterial;
			Godot.AudioStreamPlayer3D audioplayer = new AudioStreamPlayer3D();
			audioplayer.Stream = audioEffect;
			AddChild(audioplayer);
			audioplayer.Play();
			audioplayer.Finished += () => 
			{
				
				audioplayer.QueueFree(); // Crucial! This deletes the node once the sound is done.
			};
		}
		else
		{
			this.MaterialOverride=ledOFFMaterial;
		}
	}

	public void Blink(int value)
	{
		BlinkTimer-=2*value;
		if(BlinkTimer<=0)
		{
			SetOn(true);
			BlinkTimer=BlinkTimerReset;
			return;
		}
		SetOn(false);
		
	}
}

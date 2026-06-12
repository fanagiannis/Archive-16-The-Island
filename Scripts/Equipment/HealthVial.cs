using Godot;
using System;

public partial class HealthVial : Consumable
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

    public override void Consume()
	{
		/*
		SceneManager.Instance.GetPlayer().HealthSystem.Heal();
		if(equipment==null)
		{
			Log.Instance.SetLog("No Equipped Item",5);
			return;
		}
		else  
		{
			if(equipment is Flashlight flashlight)
				flashlight.ResetBatteryLife();
			else
			{
				Log.Instance.SetLog("No Flashlight Equipped",5);
				return;
			}
		}*/
	}
}

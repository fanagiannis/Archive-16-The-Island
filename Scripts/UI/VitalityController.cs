using Godot;
using PolarBears.PlayerControllerAddon;
using System;

public partial class VitalityController : Container
{
	[Export]HealthSystem playerhealthSystem;
	[Export]Stamina playerStaminaSystem;

	[Export]ProgressBar healthBar;
	[Export]ProgressBar staminaBar;

    public override void _Process(double delta)
    {
        //base._Process(delta);
		//UpdateBars();
    }

	public void UpdateBars(float dmgvalue)
	{
		
	}

	void UpdateHealthBar(float value)
	{
		healthBar.Value = playerhealthSystem.CurrentHealth;
	}

	void UpdateStaminaBar(float value)
	{
		staminaBar.Value = value;
	}

}

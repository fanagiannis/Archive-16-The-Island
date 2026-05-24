using Godot;
using System;

public partial class Flashlight : Equipment
{
	[Export] SpotLight3D spotLight;
	[Export] Label3D LifeDisplay;
	private bool FlashlightActive = false;
	private bool hasBattery = true;
	private float BatteryLife=100;
    public override void _Ready()
    {
        spotLight.Visible=false;
		LifeDisplay.Text = (BatteryLife/100).ToString("P0");
    }

    public override void _Process(double delta)
    {
        if(FlashlightActive)
			DecreaseBatteryLife((float)delta*1);
    }
    public override void Use()
    {
        //base.Use();
		if(spotLight!=null)
		{
			if(spotLight.Visible)
			{
				spotLight.Visible=false;
				FlashlightActive = false;
			}
			else if (!spotLight.Visible&&hasBattery)
			{
				spotLight.Visible=true;
				FlashlightActive = true;
			}
		}
		else
		{
			GD.Print("No Battery");
			return;
		}
			
    }

	public void ResetEquipment()
	{
		if(spotLight!=null)
		{
			if(spotLight.Visible)
			{
				spotLight.Visible=false;
				FlashlightActive = false;
			}
		}
	}

	public void DecreaseBatteryLife(float value)
	{
		if(FlashlightActive && Visible)
		{
			BatteryLife = Mathf.Max(BatteryLife-value, 0);
			LifeDisplay.Text = (BatteryLife/100).ToString("P0");
		}
		if(BatteryLife<=0)
		{
			spotLight.Visible=false;
			FlashlightActive = false;
		}

			
	}

	public void SetHasBattery(bool set)
	{
		hasBattery = set;
	}

	public bool GetBatteryLife()
	{
		return hasBattery;
	}
}

using Godot;
using PolarBears.PlayerControllerAddon;
using System;

public partial class DamageArea : Area3D
{
	PlayerController playeyrReference;	
	bool playerCanBeDamaged=false;
	float Damage=10;
	float damageTimer=0;
	public override void _Ready()
	{
		
	}

	public override void _Process(double delta)
	{
		DamagePlayer((float)delta);
	}
	public void OnEnter(Node body)
    {
        if (body is PlayerController player)
        {
			playeyrReference = player;
			playerCanBeDamaged=true;
            
        }
    }
	public void OnExit(Node body)
    {
        if (body is PlayerController player)
        {
			playerCanBeDamaged=false;
        }
    }

	public void DamagePlayer(float delta)
	{
		if(playerCanBeDamaged)
		{
			damageTimer+=delta;
			if(damageTimer>=1)
			{
				playeyrReference.HealthSystem.TakeDamage(Damage);
				damageTimer=0;
			}
				
		}
	}

	public void SetDamage(float value)
	{
		Damage=value;
	}
}

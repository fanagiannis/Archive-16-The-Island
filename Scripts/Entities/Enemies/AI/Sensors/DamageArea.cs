using Godot;
using PolarBears.PlayerControllerAddon;
using System;
using System.Threading.Tasks;

public partial class DamageArea : Area3D
{
	[Export] public Camera3D JSCamera;
	PlayerController playeyrReference;	
	bool playerCanBeDamaged=false;
	float Damage=10;
	float damageTimer=0;
	public override void _Ready()
	{
		
	}

	public override void _Process(double delta)
	{
		//DamagePlayer((float)delta);
	}
	public void OnEnter(Node body)
    {
        if (body is PlayerController player)
        {
			playeyrReference = player;
			if(JSCamera!=null)
			{
				GD.Print("PLAYER CAUGHT");
				player.TriggerJumpscare(JSCamera);
				PlayCameraAnimation(playeyrReference);
				//cameraAnimator.Play("CameraJumpscare");
				//await ToSignal(cameraAnimator, AnimationPlayer.SignalName.AnimationFinished);
				
			}
            
        }
    }
	public async Task PlayCameraAnimation(PlayerController player)
	{
		await ToSignal(GetTree().CreateTimer(4.0f), SceneTreeTimer.SignalName.Timeout);
		player.HealthSystem.TakeDamage(1000f);
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

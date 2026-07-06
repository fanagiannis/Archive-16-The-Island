using Godot;
using GroveGames.BehaviourTree.Collections;
using System;
using System.Collections.Generic;

public partial class Enemy_Abomination : Enemy
{

	//EnemyAbominationData enemyData;
	[Signal]
	public delegate void EnabledEventHandler();
	[Signal]
	public delegate void DisabledEventHandler();
	
	AnimationTree animator;
	float walk_speed;
	float slow_walk_speed;
	float sprint_speed;
	bool _IsDamaged=false;
	bool _FlashlightDamaged=false;
	bool _Blinded=false;
	[Export]float LightDamage=100f;
	[ExportCategory("SoundEffects")]
	[Export] 
    private AudioStream ActivationSoundEffect;
	[Export] 
    private AudioStream BreathSoundEffect;
	[Export] 
    private Godot.Collections.Array<AudioStream> SoundEffects = new Godot.Collections.Array<AudioStream>();
	private float randomSoundTime=5f;
	private float randomSoundTimer=5f;
	
	public override void _Ready()
	{
		//base._Ready();
		SetupAI();
		animator = GetNode<AnimationTree>("Abomination/AnimationTree");
		
		//Dead();

		CheckDamage();
		CheckLightDamage();

		if (enemyData != null)
		{
			walk_speed = enemyData.walk_speed;
			sprint_speed = enemyData.sprint_speed;
			slow_walk_speed = walk_speed/1.5f;
		}
		
		SetAgentSpeed();
		//current_speed=walk_speed;
		HPDisplay.Text = LightDamage.ToString("0");

		AudioStreamPlayer3D BreathAudioPlayer = new AudioStreamPlayer3D();
		AddChild(BreathAudioPlayer );
		BreathAudioPlayer.Stream = BreathSoundEffect;
		BreathAudioPlayer.Play();
		//UpdateSpeed();
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.

	public override void _Process(double delta)
	{
		base._Process(delta);
		UpdateAnimator();
		TakeLightDamage(delta);
		PlayRandomSound(delta);
		//GD.Print(Velocity.Length());
		
		
	}


	public override void SetupAI()
	{
		base.SetupAI();
	}


	public void UpdateAnimator()
	{
		if (animator == null) return;
		if (animator == null) return;

		// Determine the target we WANT to reach
		float target_blend = (Velocity.Length() > 0.1f) ? current_speed : 0.0f;

		// Get the current blend position from the animator
		float actual_blend = (float)animator.Get("parameters/Movement/blend_position");

		// Smoothly interpolate from actual -> target
		// We multiply by delta * 10 to make it frame-rate independent
		float smoothed_blend = Mathf.Lerp(actual_blend, target_blend, (float)0.1f);

		// Apply it back
		animator.Set("parameters/Movement/blend_position", smoothed_blend);
		/*
		if(Velocity.Length()>0)
			animator.Set("parameters/Movement/blend_position",current_speed);
		else
			animator.Set("parameters/Movement/blend_position",0);*/
	}

	public void CheckDamage()
	{
		UpdateBlackboard();
		if(HP<=0)
		{
			_IsDamaged=true;
		}
	}

	public void CheckLightDamage()
	{
		
		if(LightDamage<=0 && !_Blinded)
		{
			_Blinded=true;
		   // UpdateBlackboard();
		   GD.Print(_Blinded);
		}
		UpdateBlackboard();
	}

	public void ResetDamage()
	{
		HP = enemyData.GetMaxHP();
		_IsDamaged=false;
		LightDamage = 100f;
		_Blinded = false;
		ResetBlackboard();
		HPDisplay.Text = LightDamage.ToString("0");
	}

	public void TakeLightDamage(double delta)
	{
		CheckLightDamage();
		if(_FlashlightDamaged)
		{
			LightDamage-=8*(float)delta;
			//LightDamage = Mathf.Max(0,LightDamage);
			HPDisplay.Text = LightDamage.ToString("0");
		   
		}
	}

	public void SetLightDamaged(bool set)
	{
		_FlashlightDamaged = set;
		if(_FlashlightDamaged)
			current_speed = slow_walk_speed;
		else
			current_speed = walk_speed;
	}
	public override void Dead()
	{
		CheckDamage();
		
	}

	private void UpdateBlackboard()
	{
		// Ensure you have a valid reference to your BeehaveTree node
		if (enemyBehavior != null)
		{
			// 1. Fetch the blackboard instance from the BeehaveTree node
			GodotObject blackboard = enemyBehavior.Get("Blackboard").As<GodotObject>();

			if (blackboard != null)
			{
			   // blackboard.Call("PlayerSpotted",true);
				blackboard.Call("set_value", "AgentSpeed", current_speed);
				blackboard.Call("set_value", "HealthPoints", HP);
				blackboard.Call("set_value", "IsDamaged", _IsDamaged);
				blackboard.Call("set_value", "IsBlinded", _Blinded);
			}
		}

		UpdateAnimator();
	}

	private void ResetBlackboard()
	{
		// Ensure you have a valid reference to your BeehaveTree node
		if (enemyBehavior != null)
		{
			// 1. Fetch the blackboard instance from the BeehaveTree node
			GodotObject blackboard = enemyBehavior.Get("Blackboard").As<GodotObject>();

			if (blackboard != null)
			{
			   // blackboard.Call("PlayerSpotted",true);
				//blackboard.Call("set_value", "PlayerSpotted", false);
				blackboard.Call("set_value", "AgentSpeed", current_speed);
				blackboard.Call("set_value", "HealthPoints", HP);
				blackboard.Call("set_value", "IsDamaged", _IsDamaged);
				blackboard.Call("set_value", "IsBlinded", _Blinded);
			}
		}

		UpdateAnimator();
	}

	public void PlayRandomSound(double delta)
	{
		randomSoundTimer-=1f*(float)delta;
		if(randomSoundTimer<0f)
		{
			int randomIndex = GD.RandRange(0, SoundEffects.Count - 1);
			AudioStreamPlayer3D audioplayer = new AudioStreamPlayer3D();
			audioplayer.Stream = SoundEffects[randomIndex];
			AddChild(audioplayer);
			audioplayer.Finished += () => QueueFree();
			audioplayer.Play();
			randomSoundTimer = GD.RandRange(10, 30); 
			return;
		}
	}

	public void SetEnabledSound()
	{
		AudioStreamPlayer3D audioplayer = new AudioStreamPlayer3D();
		audioplayer.Stream = ActivationSoundEffect;
		AddChild(audioplayer);
		audioplayer.Finished += () => QueueFree();
		audioplayer.Play();
	}

	public override void SetEnabled(bool set)
	{
		base.SetEnabled(set);
		if(set==true)
			EmitSignal(SignalName.Enabled);
		else if (set==false)
			EmitSignal(SignalName.Disabled);
	}

	public void Run()
	{
		current_speed=sprint_speed;
		UpdateSpeed();
	}

	public void Walk()
	{
		current_speed=walk_speed;
		UpdateSpeed();
	}

	public void FailsafeSpeed()
	{
		current_speed=sprint_speed*3f;
		UpdateSpeed();
	}

	private void UpdateSpeed()
	{
		enemyBehavior.SetBlackboardValue("AgentSpeed",current_speed);
		UpdateAnimator();
	}

	public float GetCurentSpeed()
	{
		return current_speed;
	}

	public bool GetIsDamaged()
	{
		return _IsDamaged;
	}
	 public bool GetIsBlinded()
	{
		return _Blinded;
	}
}

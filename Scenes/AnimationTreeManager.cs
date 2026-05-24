using Godot;
using System;
using System.Threading.Tasks;

public partial class AnimationTreeManager : AnimationTree
{
	[Signal] delegate void InteractAnimationStartEventHandler();
	[Signal] delegate void InteractAnimationStopEventHandler();
	//[Export] private AnimationTree PlayerAnimationTree;
	private AnimationNodeStateMachinePlayback _playback;
	private Godot.AnimationPlayer _animPlayer;

	public override void _Ready()
	{
		this.Active = true;
		_playback = (AnimationNodeStateMachinePlayback)this.Get("parameters/playback");
		GD.Print(_playback," ",_animPlayer);
		_animPlayer = GetNode<Godot.AnimationPlayer>(AnimPlayer);
	}

	public override void _Process(double delta)
	{

	}
	
	public async Task PlayInteract()
	{
		//this.Set("parameters/InteractBT/OneShot/request",(int)AnimationNodeOneShot.OneShotRequest.Fire);	
		this.Set("parameters/conditions/is_interacting", true);
		//_playback.Travel("InteractBT");
		
		float duration = _animPlayer.GetAnimation("Interact").Length/1.5f;
		await ToSignal(
			GetTree().CreateTimer(duration),
			SceneTreeTimer.SignalName.Timeout
		);

		this.Set("parameters/conditions/is_interacting", false);
		GD.Print("Interact animation finished");
		return;
	}

	public async Task PlayPickup()
	{
		//this.Set("parameters/PickUpBT/OneShot/request",(int)AnimationNodeOneShot.OneShotRequest.Fire);	
		this.Set("parameters/conditions/is_pickingitem", true);
		//_playback.Travel("PickUpBT");
		
		float duration = _animPlayer.GetAnimation("PickUp").Length/1.5f;
		await ToSignal(
			GetTree().CreateTimer(duration),
			SceneTreeTimer.SignalName.Timeout
		);

		this.Set("parameters/conditions/is_pickingitem", false);
		GD.Print("Interact animation finished");
		return;
		
	}

	public async Task PlayEquipPistol()
	{
		this.Set("parameters/conditions/PistolEquipped", true);
		//_playback.Travel("PickUpBT");
		
		float duration = _animPlayer.GetAnimation("EquipPistol").Length/1.5f;
		await ToSignal(
			GetTree().CreateTimer(duration),
			SceneTreeTimer.SignalName.Timeout
		);

		this.Set("parameters/conditions/PistolEquipped", false);
		GD.Print("Interact animation finished");
		return;
	}

	public async Task PlayUnequipPistol()
	{
		this.Set("parameters/Pistol SM/conditions/NoPistol", true);
		//_playback.Travel("PickUpBT");
		
		float duration = _animPlayer.GetAnimation("UnequipPistol").Length/1.5f;
		await ToSignal(
			GetTree().CreateTimer(duration),
			SceneTreeTimer.SignalName.Timeout
		);

		this.Set("parameters/Pistol SM/conditions/NoPistol", false);
		GD.Print("Interact animation finished");
		return;
	}

	public async Task PlayAimPistol()
	{
		this.Set("parameters/Pistol SM/conditions/AimPistol", true);
		//_playback.Travel("PickUpBT");
		
		float duration = _animPlayer.GetAnimation("AimTransitionPistol").Length/1.5f;
		await ToSignal(
			GetTree().CreateTimer(duration),
			SceneTreeTimer.SignalName.Timeout
		);

		this.Set("parameters/Pistol SM/conditions/AimPistol", false);
		GD.Print("Interact animation finished");
		return;
	}

	public async Task PlayQuitAimPistol()
	{
		this.Set("parameters/Pistol SM/conditions/NoAimPistol", true);
		//_playback.Travel("PickUpBT");
		
		float duration = _animPlayer.GetAnimation("LeaveAimTransitionPistol").Length/1.5f;
		await ToSignal(
			GetTree().CreateTimer(duration),
			SceneTreeTimer.SignalName.Timeout
		);

		this.Set("parameters/Pistol SM/conditions/NoAimPistol", false);
		GD.Print("Interact animation finished");
		return;
	}
}

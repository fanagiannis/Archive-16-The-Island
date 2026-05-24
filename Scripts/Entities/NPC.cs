using Godot;
using PolarBears.PlayerControllerAddon;
using System;

public partial class NPC : Interactable
{
	[Export] Godot.AnimationPlayer animator;
	[Export] Camera3D npccamera;
	public override void _Ready()
	{
		if(animator==null)
			return;
		animator.Play("Idle");
	}

	public override void _Process(double delta)
	{
		animator.Play("Idle");
	}

    public override void Interact()
    {
        base.Interact();
    }

    public override void SetOutline(bool set)
    {
      //  base.SetOutline(set);
    }

	public Camera3D GetCamera()
	{
		return npccamera;
	}
	

	
}

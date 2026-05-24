using Godot;
using System;
using System.Dynamic;

public partial class Interactable : Node3D
{
	[Signal]
	delegate void InteractionEventHandler();
	[Signal]
	delegate void EnterInteractEventHandler();
	[Signal]
	delegate void ExitInteractEventHandler();
	[Export] public String name;
	[Export] public String description;
	[Export] protected Label3D itemLabel;
	[Export] protected AudioStream itemAudio;
	protected AudioStreamPlayer3D audioPlayer;
	[Export] protected Texture2D itemIcon;
	[Export]public bool Interacted = false;

	public override void _Ready()
	{
		audioPlayer = GetNode<AudioStreamPlayer3D>("AudioPlayer");
		if(itemLabel==null) return;

		else
		{
			itemLabel.Text = name;
			itemLabel.Visible = false;
			SetOutline(false);
		}
		
	}

	public override void _PhysicsProcess(double delta)
    {
        
    }

	public virtual void Interact()
	{
		if(itemAudio!=null && audioPlayer!=null)
		{
			audioPlayer.Stream = itemAudio;
			audioPlayer.Play();

		}
		//GD.Print("Interact");
	}

	public virtual void EnterInteraction()
	{
		if(itemLabel==null) return;
		else
			//itemLabel.Visible = true;
			SetOutline(true);
	}
	public virtual void ExitInteraction()
	{	
		if(itemLabel==null) return;
		else
			//itemLabel.Visible = false;
			SetOutline(false);
	}
	public bool GetInteracted()
	{
		return Interacted;
	}

	public void SetInteracted (bool set)
	{
		Interacted = set;
		if(set==false)
			ExitInteraction();
	}

	public Label3D GetLabel()
	{
		return itemLabel;
	}

	public Texture2D GetIcon()
	{
		return itemIcon;
	}

	public void RotateLabel(Node3D target)
	{
		if(itemLabel==null) return;
		else
		{
			itemLabel.LookAt(target.GlobalPosition,Vector3.Up);
			itemLabel.RotateY(Mathf.Pi);
		}
	}

	public virtual void SetOutline(bool set)
	{
		
		var mesh = GetNode<MeshInstance3D>("Mesh");
		if(mesh == null) 
			return;

		var mat = mesh.GetSurfaceOverrideMaterial(0) as BaseMaterial3D;
		if(mat == null)
			return;
		
		if(set)
			mat.StencilMode = BaseMaterial3D.StencilModeEnum.Outline;
		else
			mat.StencilMode = BaseMaterial3D.StencilModeEnum.Disabled;
		
	}
	

	
}

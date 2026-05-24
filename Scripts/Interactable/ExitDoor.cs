using Godot;
using System;
using System.Dynamic;

public partial class ExitDoor: Interactable
{
	[Export] string SceneToLoad;
	[Export] bool Locked= false;
	[Export] string Tag;
	[Export] Node3D DoorPlayerSpawnPoint;
	
	bool _activatedDoor=false;

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

	public override void Interact()
	{
		base.Interact();
		/*DEBUG*/ Log.Instance.SetLog("Entered: " + SceneToLoad,1);
		if (SceneToLoad != null && !Locked && !SceneManager.Instance.Isloading())
		{
			_activatedDoor = true;
			SceneManager.Instance.StartLoading(SceneToLoad,Tag);
		}
		else if(Locked)
		{
			Log.Instance.SetLog("Locked",1);
		}
			//GD.Print("Interact");
	}

	public override void EnterInteraction()
	{
		base.EnterInteraction();
	}
	public override void ExitInteraction()
	{	
		base.ExitInteraction();
	}

	public void Lock(bool set)
	{
		Locked = set;
	}
	
	public bool GetState()
	{
		return _activatedDoor;
	}

	public string GetTag()
	{
		return Tag;
	}

	public Vector3 GetSpawnPoint()
	{
		return DoorPlayerSpawnPoint.GlobalPosition;
	}

	public void ResetDoor()
	{
		_activatedDoor=false;
	}
	
}

using Godot;
using PolarBears.PlayerControllerAddon;
using System;
using System.Diagnostics.CodeAnalysis;

public partial class Area : Area3D
{
	//DEBUG
	[Export]Label3D label;
	//DEBUG
	bool discovered=false;
	bool isActive=false;
	[Export]string name;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered+=Discover;
		BodyEntered+=EnteredArea;
		BodyExited+=ExitedArea;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		label.Text=isActive.ToString();
	}

	public void Discover(Node3D body)
	{
		if(body is PlayerController && discovered==false)
		{
			discovered=true;
			
		}
	}//

	public void EnteredArea(Node3D body)
	{
		if(body is PlayerController)
		{
			isActive=true;
			Log.Instance.SetLog(name,1f);
			Level level = Owner as Level;
			level.SetLastArea(this);
		}
	}

	public void ExitedArea(Node3D body)
	{
		
	} 

	public void ResetArea()
	{
		isActive=false;
	} 

	public void SetActive(bool set)
	{
		isActive=set;
	}

	public string GetAreaName()
	{
		return name;
	} 

	public bool GetDiscovered()
	{
		return discovered;
	}
}

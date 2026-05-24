using Godot;
using System;

public partial class RenderManager : StaticBody3D
{
	[Export] public float MaxDistance = 20.0f;
	VisibleOnScreenEnabler3D OnScreenNotifier;
	CollisionShape3D Collision;
	Camera3D camera ;
	bool _OnSight=false;
	bool _Inzone=false;

	private double _checkTimer = 0.0;
    private double _checkInterval = 0.1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		OnScreenNotifier = GetNode<VisibleOnScreenEnabler3D>("VisibilityNotifier");
		OnScreenNotifier.ScreenEntered+=ItemOnScreen;
		OnScreenNotifier.ScreenExited+=ItemOffScreen;
		Collision = GetNode<CollisionShape3D>("Collision");
		Collision.Disabled=false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_checkTimer += delta;
		//Camera3D camera = GetViewport().GetCamera3D();
		if (_checkTimer < _checkInterval) 
			return;
        _checkTimer = 0.0;
		/*
		if(_OnSight==false)
		{
			UpdateCollision(_OnSight);
			return;
		}
			*/
		camera = GetViewport().GetCamera3D();
        if (camera == null) 
			return;
		float distance= GlobalPosition.DistanceSquaredTo(camera.GlobalPosition);

		_Inzone= distance<=MaxDistance;

		//GD.Print(_Inzone);

		UpdateCollision(_Inzone);
	}

	public void ItemOnScreen()
	{
		_OnSight=true;
		//GD.Print("Collision = "+Collision.Disabled);
	}

	public void ItemOffScreen()
	{
		_OnSight=false;
		//UpdateCollision(_OnSight);
		//GD.Print("Collision = "+Collision.Disabled);
	}

	public void UpdateCollision(bool set)
	{

		Collision.Disabled=!set;
	}
}

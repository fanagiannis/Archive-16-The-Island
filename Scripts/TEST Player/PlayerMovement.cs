using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

public partial class PlayerMovement : Node
{
	#region Components
	[Export]Node3D PlayerHead;
	[Export]Camera3D PlayerCamera;
	[Export]CollisionShape3D PlayerCollision;
	#endregion
	#region Animations
	[Export] AnimationPlayer JumpAnimation;
	[Export] AnimationPlayer CrouchAnimation;
	[Export] AnimationPlayer HeadBobbing;
	#endregion

	#region Controls
	[Export] public Dictionary Controls = new()
    {
        { "LEFT", "move_left" },
        { "RIGHT", "move_right" },
        { "FORWARD", "move_up" },
        { "BACKWARD", "move_down" },
        { "JUMP", "ui_accept" },
        { "CROUCH", "crouch" },
        { "SPRINT", "sprint" },
        { "PAUSE", "ui_cancel" }
    };
	#endregion

	#region InputVariables
	[Export] float mouseSensitivity=0.5f;
	#endregion

	#region PhysicsVariables
	[Export] float gravity=9.5f;
	#endregion

	#region Speeds
	[ExportCategory("Speeds")]
	[Export] float walkSpeed=3;
	[Export] float sprintSpeed=6;
	[Export] float crouchSpeed=1;
	#endregion

	#region UI
	[Export(PropertyHint.File)] public string DefaultReticle = "";
	private Control _reticle;
	#endregion

    private Vector3 _mouseInput = Vector3.Zero;
	private float camRotX = 0;

	public void Ready()
    {
		InitializeAnimations();
        SetupCamera();
		SetupMovement();
    }

	public void Update(double delta)
    {
        HandleCamera((float)delta);
    }

	public void UpdatePhysics(double delta)
    {
        
    }
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}
	public override void _Input(InputEvent @event)
    {
        
    }
	void SetupCamera()
    {
        Input.MouseMode=Input.MouseModeEnum.Captured;
    }

	void SetupMovement()
    {
        
    }

	void HandleCamera(float delta)
    {
        HandleCameraRotation(delta);
    }

	void HandleCameraRotation(float delta)
    {
        var mouseInput = Input.GetLastMouseVelocity();
		camRotX -= mouseInput.Y*mouseSensitivity*delta;
		camRotX = Mathf.Clamp(camRotX,-90f,90f);

		PlayerCamera.RotationDegrees=new Vector3(mouseInput.X*mouseSensitivity*delta,0,0);
    }

	void HandleMovement(float delta)
    {
        
    }

	private void InitializeAnimations()
    {
        HeadBobbing?.Play("RESET");
        JumpAnimation?.Play("RESET");
        CrouchAnimation?.Play("RESET");
    }
}

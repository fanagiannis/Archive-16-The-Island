using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Godot;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace PolarBears.PlayerControllerAddon;

public partial class PlayerController : CharacterBody3D
{
	// User API to important child nodes.
	public Node3D          Head;
	public Bobbing         Bobbing;
	public FieldOfView     FieldOfView;
	public Stamina         Stamina;
	public StairsSystem    StairsSystem;
	public CapsuleCollider CapsuleCollider;
	public Gravity         Gravity;
	public HealthSystem    HealthSystem;
	public Mouse           Mouse;
	public InteractionSystem Interaction;
	public PlayerUIController PlayerUI;
	public PauseMenu PauseMenuUI;
	public EquipmentController EquipmentControl; 
	public EquipmentManager EquipmentManager; 
	public Camera3D playercamera ;
	public FootstepSystem Footsteps;
	
	//[Export] Godot.AnimationPlayer HandsAnimationPlayer;

	[Signal]
	delegate void JumpedEventHandler();
	[Signal]
	delegate void IsMovingEventHandler();
	[Signal]
	delegate void IsNotMovingEventHandler();
	[Signal]
	delegate void IsSprintingEventHandler();
	[Signal]
	delegate void HeadHitCeilingEventHandler();
	[Signal]
	delegate void ControllerEnabledEventHandler();
	[Signal]
	delegate void ControllerDisabledEventHandler();
	[Signal]
	delegate void EquippedEventHandler();
	[Signal]
	delegate void UnequippedEventHandler();

	[Export(PropertyHint.Range, "0,20,0.1,or_greater")]
	public float WalkSpeed             { get; set; } = 5.0f;
	[Export(PropertyHint.Range, "0,20,0.1,or_greater")]
	public float SprintSpeed           { get; set; } = 7.2f;
	[Export(PropertyHint.Range, "0,10,0.1,or_greater")]
	public float CrouchSpeed           { get; set; } = 2.5f;
	[Export(PropertyHint.Range, "25,100,0.1,or_greater")]
	public float CrouchTransitionSpeed { get; set; } = 25.0f;

	[ExportGroup("Input")]
	[Export]
	public string MoveForwardInputAction;
	[Export]
	public string MoveBackwardInputAction;
	[Export]
	public string StrafeLeftInputAction;
	[Export]
	public string StrafeRightInputAction;
	[Export]
	public string JumpInputAction;
	[Export]
	public string CrouchInputAction;
	[Export]
	public string SprintInputAction;
	[Export]
	public string InteractInputAction;
	[Export]
	public string PauseInputAction;

	private float _currentSpeed;

	private const float DecelerationSpeedFactorFloor = 15.0f;
	private const float DecelerationSpeedFactorAir   = 7.0f;

	private float _lastFrameWasOnFloor = -Mathf.Inf;

	private const int NumOfHeadCollisionDetectors = 4;
	private RayCast3D[] _headCollisionDetectors;

	private bool _wasHeadPreviouslyTouchingCeiling = false;

	private bool _controllerEnabled = true;
	private bool _UIEnabled = false;
	private bool _isMoving=false;
	private bool _isSprinting=false;
	private bool _isMovesSlow=false;
	private bool _isPaused=false;

#region READY




	public override void _Ready()
	{
		#region Initialization
		_currentSpeed = WalkSpeed;

		Head = GetNode<Node3D>("Head");

		_headCollisionDetectors = new RayCast3D[NumOfHeadCollisionDetectors];

		for (int i = 0; i < NumOfHeadCollisionDetectors; i++)
		{
			_headCollisionDetectors[i] = GetNode<RayCast3D>(
				"HeadCollisionDetectors/HeadCollisionDetector" + i);
		}
		#endregion

		#region Node References
		Camera3D playercamera = GetNode<Camera3D>("Head/CameraSmooth/Camera3D");
		RayCast3D stairsBelowRayCast3D = GetNode<RayCast3D>("StairsBelowRayCast3D");
		RayCast3D stairsAheadRayCast3D = GetNode<RayCast3D>("StairsAheadRayCast3D");
		Node3D cameraSmooth = GetNode<Node3D>("Head/CameraSmooth");
		AnimationPlayer animationPlayer = GetNode<AnimationPlayer>("CameraAnimationPlayer");
		float gravitySetting = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
		ColorRect vignetteRect = GetNode<ColorRect>("Head/CameraSmooth/Camera3D/CLVignette(Layer_1)/HealthVignetteRect");
		ColorRect distortionRect = GetNode<ColorRect>("Head/CameraSmooth/Camera3D/CLDistortion(Layer_2)/HealthDistortionRect");
		ColorRect blurRect = GetNode<ColorRect>("Head/CameraSmooth/Camera3D/CLBlur(Layer_2)/BlurRect");
		Node3D mapNode = GetTree().Root.FindChild("Map", true, false) as Node3D;
		
		#endregion

		#region Component Initialization
		Bobbing = GetNode<Bobbing>("Bobbing");
		Bobbing.Init(playercamera);

		FieldOfView = GetNode<FieldOfView>("FieldOfView");
		FieldOfView.Init(playercamera);

		Stamina = GetNode<Stamina>("Stamina");
		Stamina.SetSpeeds(WalkSpeed, SprintSpeed);

		StairsSystem = GetNode<StairsSystem>("StairsSystem");
		StairsSystem.Init(stairsBelowRayCast3D, stairsAheadRayCast3D, cameraSmooth);

		CapsuleCollider = GetNode<CapsuleCollider>("CapsuleCollider");

		Gravity = GetNode<Gravity>("Gravity");
		Gravity.Init(gravitySetting);

		HealthSystem = GetNode<HealthSystem>("HealthSystem");

		Interaction = GetNode<InteractionSystem>("InteractionSystem");

		PlayerUI = GetNode<PlayerUIController>("UI");

		PauseMenuUI = GetNode<PauseMenu>("UI/PauseMenu");
		EquipmentControl = GetNode<EquipmentController>("Head");

		EquipmentManager = GetNode<EquipmentManager>("Head/CameraSmooth/Camera3D/Equipment");

		Footsteps = GetNode<FootstepSystem>("FootstepSystem");

		HealthSystem.HealthSystemInitParams healthSystemParams = new HealthSystem.HealthSystemInitParams()
		{
			Gravity = Gravity,
			Parent = this,
			Camera = playercamera,
			AnimationPlayer = animationPlayer,
			Head = Head,
			VignetteRect = vignetteRect,
			DistortionRect = distortionRect,
			BlurRect = blurRect,
		};

		HealthSystem.Init(healthSystemParams);

		HealthSystem.Died += () =>
		{
			EnableController(false);
			EnableUI(true);
		};

		Mouse = GetNode<Mouse>("Mouse");
		Mouse.Init(Head, playercamera, HealthSystem.IsDead);

		//ChunkLoader = GetNode<ChunkLoader>("ChunkManager");
		#endregion

		#region UI
		if(PlayerUI!=null)
		{
			PlayerUI.GetInventoryExitButton().Pressed+=ControllerEnablerHandler;
			PlayerUI.GetInventoryExitButton().Pressed+=UIEnablerHandler;	
		}
		if (PauseMenuUI != null)
		{
			PauseMenuUI.OnResume += () =>
			{
				EnableController(true);
				EnableUI(false);
				_isPaused=false;
				
				// Input.MouseMode = Input.MouseModeEnum.Captured;
			};

			PauseMenuUI.OnExit+= () =>
			{
				SceneManager.Instance.ReturnToMainMenu();
			};

		}

		#endregion


		EnableController(false);
		EnableUI(true);
		
		

	}
#endregion

#region UPDATE



	public override void _PhysicsProcess(double delta)
	{
		#region PAUSE
		if (IsInputPressed(InteractInputAction, Key.Tab, justPressed: true) && _controllerEnabled )
		{
			if(!_isPaused)
			{
				_isPaused=true;
				
			}
			else if(_isPaused)
			{
				_isPaused=false;
			}
			EnableController(!_isPaused);
			EnableUI(_isPaused);
			
			PauseMenuUI.Visible = _isPaused;
			GetTree().Paused = _isPaused;

			
		}
		#endregion

		#region DEATH

		#endregion

		#region Floor and Gravity
		if (isOnFloorCustom())
		{
			_lastFrameWasOnFloor = Engine.GetPhysicsFrames();
		}

		if (!isOnFloorCustom())
		{
			Velocity = new Vector3(
				x: Velocity.X,
				y: Velocity.Y - (Gravity.CalculateGravityForce() * (float)delta),
				z: Velocity.Z);
		}
		#endregion

		#region Player State Checks
		bool doesCapsuleHaveCrouchingHeight = CapsuleCollider.IsCrouchingHeight();
		bool isPlayerDead = HealthSystem.IsDead();
		#endregion

		#region Interaction
		if (IsInputPressed(InteractInputAction, Key.E, justPressed: true) && _controllerEnabled && isOnFloorCustom())
		{
			if (Interaction == null) return;
			else
			{
				Interaction.Interact();
				if(Interaction.GetInteractable() != null && !(Interaction.GetInteractable() is NPC))
				{
					Bobbing.InteractBob();
				}
			}
		}
		#endregion

		#region Equipment
		
		#endregion

		#region Jumping
		/*if (IsInputPressed(JumpInputAction, Key.Space, justPressed: true) && isOnFloorCustom()
			&& !doesCapsuleHaveCrouchingHeight && !isPlayerDead)
		{
			Velocity = new Vector3(
				x: Velocity.X,
				y: Gravity.CalculateJumpForce(),
				z: Velocity.Z);

			EmitSignal(SignalName.Jumped);
		}*/
		#endregion

		#region Ceiling Collision
		bool isHeadTouchingCeiling = IsHeadTouchingCeiling();
		bool doesCapsuleHaveDefaultHeight = CapsuleCollider.IsDefaultHeight();

		if (isHeadTouchingCeiling && doesCapsuleHaveDefaultHeight)
		{
			Velocity = new Vector3(
				x: Velocity.X,
				y: Velocity.Y - 2.0f,
				z: Velocity.Z);
			if (!_wasHeadPreviouslyTouchingCeiling)
				EmitSignal(SignalName.HeadHitCeiling);
		}

		_wasHeadPreviouslyTouchingCeiling = isHeadTouchingCeiling;
		#endregion

		#region Crouching
		if (!isPlayerDead)
		{
			if (IsInputPressed(CrouchInputAction, Key.Ctrl, justPressed: false) ||
				(doesCapsuleHaveCrouchingHeight && isHeadTouchingCeiling))
			{
				CapsuleCollider.Crouch((float)delta, CrouchTransitionSpeed);
				_isMovesSlow=true;
				_currentSpeed = CrouchSpeed;
			}
			else
			{
				CapsuleCollider.UndoCrouching((float)delta, CrouchTransitionSpeed);
				_isMovesSlow=false;
				//_currentSpeed = WalkSpeed;
			}
		}
		#endregion

		#region Sprinting
		if (IsInputPressed(SprintInputAction, Key.Shift, justPressed: false) && !isHeadTouchingCeiling &&
			!doesCapsuleHaveCrouchingHeight && EquipmentControl.GetEquipmentUsageState()!=EquipmentController.EquipmentUsageState.Aiming && !isPlayerDead)
		{
			_currentSpeed = SprintSpeed;
			//Bobbing.RunBob();
			if(_isMoving )
			{
				//Bobbing.RunBob();
				_isSprinting=true;
				//Footsteps.CastFootstepAudio(_currentSpeed);
				EmitSignal(SignalName.IsSprinting);
			}
		}
		#endregion

		#region Movement
		Vector2 inputDir = GetMovementVector();
		Vector3 direction = (Head.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		if (isPlayerDead)
		{
			direction = Vector3.Zero;
		}

		if (isOnFloorCustom())
		{
			if (direction.Length() > 0 && _controllerEnabled)
			{
				_isMoving=true;
				if(_isMoving && !_isSprinting && !_isMovesSlow)
				{
					_currentSpeed = WalkSpeed;
					//Footsteps.CastFootstepAudio(_currentSpeed);
					EmitSignal(SignalName.IsMoving);
					
				}
				
				float availableSpeed = Stamina.AccountStamina(delta, _currentSpeed);
				float newX = direction.X * availableSpeed;
				float newZ = direction.Z * availableSpeed;
				Velocity = new Vector3(newX, Velocity.Y, newZ);
				
				
			}
			else
			{
				_isMoving=false;
				_isSprinting=false;
				float xDeceleration = Mathf.Lerp(Velocity.X, direction.X * _currentSpeed, (float)delta * DecelerationSpeedFactorFloor);
				float zDeceleration = Mathf.Lerp(Velocity.Z, direction.Z * _currentSpeed, (float)delta * DecelerationSpeedFactorFloor);
				Velocity = new Vector3(xDeceleration, Velocity.Y, zDeceleration);
				_currentSpeed = 0;
				EmitSignal(SignalName.IsNotMoving);
			}
		}
		else
		{
			Bobbing.StopBob();
			float xDeceleration = Mathf.Lerp(Velocity.X, direction.X * _currentSpeed, (float)delta * DecelerationSpeedFactorAir);
			float zDeceleration = Mathf.Lerp(Velocity.Z, direction.Z * _currentSpeed, (float)delta * DecelerationSpeedFactorAir);
			Velocity = new Vector3(xDeceleration, Velocity.Y, zDeceleration);
			
		}
		if(_isMoving)
			Footsteps.CastFootstepAudio(_currentSpeed);
		#endregion

		#region EquipmentBobbing
		if(_currentSpeed>WalkSpeed)
			EquipmentControl.Animations(2,(float) delta);
		else if(_currentSpeed<=WalkSpeed & _currentSpeed>CrouchSpeed)
			EquipmentControl.Animations(1,(float) delta);
		else if (_currentSpeed<CrouchSpeed && _currentSpeed>0)
			EquipmentControl.Animations(0.5f,(float) delta);
		else
			EquipmentControl.Animations(0,(float) delta);
		
		#endregion

		#region Equipment
		if (IsKeyJustPressed( Key.F) && EquipmentControl.GetEquipmentUsageState()!= EquipmentController.EquipmentUsageState.Aiming && _controllerEnabled &&!isPlayerDead)
		{
			EquipmentControl.ChangeEquipmentState();
		}
// Safer than setting to null
		if (IsKeyJustPressed( Key.I) && EquipmentControl.GetEquipmentUsageState()!= EquipmentController.EquipmentUsageState.Aiming && _controllerEnabled &&!isPlayerDead)
		{
			ControllerEnablerHandler();
			UIEnablerHandler();
			//Inventory.Instance.SetInventory();
			PlayerUI.OpenInventory();
		}

		SpeedManagement();

		//TEST
		//EquipmentManager.Transform = Transform;
		//TEST
		#endregion

		if (IsKeyJustPressed(Key.R) && EquipmentControl.GetEquipmentState()==EquipmentController.EquipmentState.Equipped )
		{
			EquipmentControl.ReloadWeapon();
		}

		#region Movement and Stairs
		if (isPlayerDead)
		{
			MoveAndSlide();
			return;
		}
		

		//if(_isMoving && !_isSprinting)
		//	Bobbing.WalkBob();
		//if(_isMoving && !_isSprinting)
		//	Bobbing.RunBob();
		//if(!_isMoving && !_isSprinting)
			//Bobbing.StopBob();

		if (_controllerEnabled)
		{
			Bobbing.CameraBobbingParams cameraBobbingParams = new Bobbing.CameraBobbingParams
			{
				Delta = (float)delta,
				IsOnFloorCustom = isOnFloorCustom(),
				Velocity = Velocity
			};
			Bobbing.PerformCameraBobbing(cameraBobbingParams);

			FieldOfView.FovParameters fovParams = new FieldOfView.FovParameters
			{
				IsCrouchingHeight = CapsuleCollider.IsCrouchingHeight(),
				Delta = (float)delta,
				SprintSpeed = SprintSpeed,
				Velocity = Velocity
			};
			FieldOfView.PerformFovAdjustment(fovParams);
		}

		StairsSystem.UpStairsCheckParams upStairsCheckParams = new StairsSystem.UpStairsCheckParams
		{
			IsOnFloorCustom = isOnFloorCustom(),
			IsCapsuleHeightLessThanNormal = CapsuleCollider.IsCapsuleHeightLessThanNormal(),
			CurrentSpeedGreaterThanWalkSpeed = _currentSpeed > WalkSpeed,
			IsCrouchingHeight = CapsuleCollider.IsCrouchingHeight(),
			Delta = (float)delta,
			FloorMaxAngle = FloorMaxAngle,
			GlobalPositionFromDriver = GlobalPosition,
			Velocity = Velocity,
			GlobalTransformFromDriver = GlobalTransform,
			Rid = GetRid()
		};

		StairsSystem.UpStairsCheckResult upStairsCheckResult = StairsSystem.SnapUpStairsCheck(upStairsCheckParams);
		if (upStairsCheckResult.UpdateRequired)
		{
			upStairsCheckResult.Update(this);
		}
		else
		{
			if (_controllerEnabled)
				MoveAndSlide();

			StairsSystem.DownStairsCheckParams downStairsCheckParams = new StairsSystem.DownStairsCheckParams
			{
				IsOnFloor = IsOnFloor(),
				IsCrouchingHeight = CapsuleCollider.IsCrouchingHeight(),
				LastFrameWasOnFloor = _lastFrameWasOnFloor,
				CapsuleDefaultHeight = CapsuleCollider.GetDefaultHeight(),
				CurrentCapsuleHeight = CapsuleCollider.GetCurrentHeight(),
				FloorMaxAngle = FloorMaxAngle,
				VelocityY = Velocity.Y,
				GlobalTransformFromDriver = GlobalTransform,
				Rid = GetRid()
			};

			StairsSystem.DownStairsCheckResult downStairsCheckResult = StairsSystem.SnapDownStairsCheck(downStairsCheckParams);
			if (downStairsCheckResult.UpdateIsRequired)
			{
				downStairsCheckResult.Update(this);
			}
		}

		StairsSystem.SlideCameraParams slideCameraParams = new StairsSystem.SlideCameraParams
		{
			CurrentSpeedGreaterThanWalkSpeed = _currentSpeed > WalkSpeed,
			BetweenCrouchingAndNormalHeight = CapsuleCollider.IsBetweenCrouchingAndNormalHeight(),
			Delta = (float)delta
		};
		StairsSystem.SlideCameraSmoothBackToOrigin(slideCameraParams);
		#endregion
		
	}
	
	#region Jumpscare
	public void TriggerJumpscare(Camera3D JSCamera)
	{
		JSCamera.MakeCurrent();
		EnableController(false);
		//await ToSignal(GetTree().CreateTimer(3.0f), SceneTreeTimer.SignalName.Timeout);
		//HealthSystem.TakeDamage(1000f);
	}
	#endregion

	#region Helper Methods
	private bool IsHeadTouchingCeiling()
	{
		for (int i = 0; i < NumOfHeadCollisionDetectors; i++)
		{
			if (_headCollisionDetectors[i].IsColliding())
			{
				return true;
			}
		}
		return false;
	}

	private bool isOnFloorCustom()
	{
		return IsOnFloor() || StairsSystem.WasSnappedToStairsLastFrame();
	}

	private Dictionary<Key, bool> previousKeyStates = new();

	private bool IsKeyJustPressed(Key key)
	{
		bool currentState = Input.IsKeyPressed(key);
		bool wasPressed = previousKeyStates.GetValueOrDefault(key, false);

		if (currentState)
		{
			previousKeyStates[key] = true;
		}
		else
		{
			previousKeyStates.Remove(key);
		}

		return currentState && !wasPressed;
	}

	private bool IsInputPressed(string inputAction, Key fallbackKey, bool justPressed = false)
	{
		bool inputActionSet = !string.IsNullOrEmpty(inputAction);

		if (justPressed)
		{
			return (
				inputActionSet && Input.IsActionJustPressed(inputAction) ||
				!inputActionSet && IsKeyJustPressed(fallbackKey)
			);
		}

		return (
			inputActionSet && Input.IsActionPressed(inputAction) ||
			!inputActionSet && Input.IsPhysicalKeyPressed(fallbackKey)
		);
	}

	private float GetInputStrength(string inputAction, Key fallbackKey)
	{
		if (string.IsNullOrEmpty(inputAction))
		{
			return Input.IsPhysicalKeyPressed(fallbackKey) ? 1.0f : 0.0f;
		}
		else
		{
			return Input.GetActionStrength(inputAction);
		}
	}

	private Vector2 GetMovementVector()
	{
		return new Vector2(
			GetInputStrength(StrafeRightInputAction, Key.D) - GetInputStrength(StrafeLeftInputAction, Key.A),
			GetInputStrength(MoveBackwardInputAction, Key.S) - GetInputStrength(MoveForwardInputAction, Key.W)
		);
	}

	public void TeleportTo(Vector3 target)
	{
		GlobalPosition= target;
	}
	#endregion

	#region Controller Management
	public void EnableController(bool set)
	{
		if (set == true)
		{
			EmitSignal(SignalName.ControllerEnabled);
		}
		else
		{
			EmitSignal(SignalName.ControllerDisabled);	
		}
			
		SetProcess(set);
		SetPhysicsProcess(set);
		_controllerEnabled = set;
	}

	public void EnableUI(bool set)
	{
		if (set == true)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
			
		}
		else
		{
			Input.SetMouseMode(Input.MouseModeEnum.Captured);
		}
			

		_UIEnabled= set;
	}

	public void UIEnablerHandler() => EnableUI(!_UIEnabled);

	public void ControllerEnablerHandler() => EnableController(!_controllerEnabled);

	#endregion


	#region SpeedManagement
	public void SpeedManagement()
	{
		if(EquipmentControl!=null && _controllerEnabled)
		{
			
			switch(EquipmentControl.GetEquipmentUsageState())
			{
				case EquipmentController.EquipmentUsageState.Idle:
				{
					_isMovesSlow = false;
					_currentSpeed = WalkSpeed;
					
					break; 	
				}
				case EquipmentController.EquipmentUsageState.Aiming:
				{
					_isMovesSlow = true;
					_currentSpeed = CrouchSpeed;
					break; 	
				}
			}
		}
	}
	#endregion
}
#endregion
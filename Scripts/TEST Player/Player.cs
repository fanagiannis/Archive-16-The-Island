using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks.Dataflow;

public partial class Player : CharacterBody3D
{

    #region Player Signals
    [Signal]
    public delegate void PlayerMovingEventHandler();
    [Signal]
    public delegate void PlayerSprintingEventHandler();
    [Signal]
    public delegate void PlayerStopMovingEventHandler();
    [Signal]
    public delegate void PlayerStopSprintingEventHandler();
    #endregion

    #region Player Movement States
    public bool isIdle=false;
    public bool isMoving=false;
    public bool isSprinting=false;
    public bool isCrouching=false;
    private bool _wasMoving = false;
    private bool _wasSprinting = false;

    #endregion

    #region Player Scripts
    [ExportCategory("Character")]
    [Export]public PlayerVitality PlayerVitality;
    #endregion

    #region Components
	[Export]Node3D PlayerHead;
	[Export]Camera3D PlayerCamera;
	[Export]CollisionShape3D PlayerCollision;
	#endregion
	#region Animations
	[Export] AnimationPlayer JumpAnimation;
	[Export] AnimationPlayer CrouchAnimation;
	[Export] AnimationPlayer HeadBobbing;

    //HEADBOBBING
    [Export] float bobbingFrequecy=2;
    [Export] float bobbingAmplitude=0.1f;
    [Export] double bobbingTime=0;

	#endregion

	#region Controls
	[Export] public Dictionary Controls = new()
    {
        { "LEFT", "move_left" },
        { "RIGHT", "move_right" },
        { "FORWARD", "move_frw" },
        { "BACKWARD", "move_bw" },
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
    private float currentSpeed = 0;
	[Export] float walkSpeed=3;
	[Export] float sprintSpeed=6;
	[Export] float crouchSpeed=1;
	#endregion

	#region UI
	[Export(PropertyHint.File)] public string DefaultReticle = "";
    private Control Reticle;
	#endregion

    #region Camera
    private Vector3 _mouseInput = Vector3.Zero;
	private float camPitch = 0;
    

	#endregion

    #region Basic Functions
    public override void _Ready()
    {
        InitializeAnimations();
        SetupCamera();
		SetupMovement();
    }

    public override void _Process(double delta)
    {
        
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 inputDir = Input.GetVector(
                Controls["LEFT"].ToString(),
                Controls["RIGHT"].ToString(),
                Controls["FORWARD"].ToString(),
                Controls["BACKWARD"].ToString()
            );
        HandleMovement((float)delta,inputDir);
        HandleMovementStates();
        bobbingTime+=delta + Velocity.Length();
        PlayerCamera.Position=Headbobbing((float)bobbingTime);
        MoveAndSlide();
    }
    #endregion

    #region SetupPlayer Functions

    void UpdateReticle(string SetReticle)
    {
         if(Reticle!=null)
        {
            QueueFree();
        }
        PackedScene reticleScene = GD.Load<PackedScene>(SetReticle);
        Reticle = reticleScene.Instantiate<Control>();
        Reticle.Set("character",this);
        GetNode("UserInterface").AddChild(Reticle);
    }

    #endregion

    #region Movement Functions

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseMotion mouseEvent)
        {
            RotateY(Mathf.DegToRad(-mouseEvent.Relative.X*mouseSensitivity));
            camPitch -= mouseEvent.Relative.Y*mouseSensitivity;
            camPitch = Mathf.Clamp(camPitch,-90f,90f);

            PlayerCamera.RotationDegrees = new Vector3(camPitch,0,0);
        }
        if (@event is InputEventAction actionEvent)
        {
            if (actionEvent.Action == new StringName(Controls["SPRINT"].ToString()))
            {
                isSprinting = actionEvent.Pressed;
                if (isSprinting)
                {
                    EmitSignal(SignalName.PlayerSprinting);
                }
                else
                {
                    EmitSignal(SignalName.PlayerStopSprinting);
                }
            }
            
        }
    }

	void SetupCamera()
    {
        Input.MouseMode=Input.MouseModeEnum.Captured;
        UpdateReticle(DefaultReticle);
    }

	void SetupMovement()
    {
       currentSpeed = walkSpeed;
       EnterIdleState();
    }

	void HandleMovement(float delta, Vector2 input)
    {
       
        isMoving = input != Vector2.Zero;
        if(isMoving)
            isSprinting = Input.IsActionPressed(Controls["SPRINT"].ToString()); 
        else
            isSprinting=false;

        Vector3 direction = new Vector3(input.X, 0, input.Y);//.Rotated(Vector3.Up, -PlayerHead.Rotation.Y);
        direction = Transform.Basis * direction;
        direction = direction.Normalized();
        Velocity = new Vector3(direction.X * currentSpeed, Velocity.Y, direction.Z * currentSpeed);
        
    }

    void HandleMovementStates()
    {
        //IDLE STATE
        if(!isMoving && !isIdle && !isSprinting)
        {
            isIdle=true;
           // HeadBobbing?.Play("RESET");
            GD.Print("idle");
        }
        //WALKING STATE
        if ((isMoving && !_wasMoving)||Input.IsActionJustReleased(Controls["SPRINT"].ToString()))
        {
            EmitSignal(SignalName.PlayerMoving);
            //HeadBobbing?.Play("walk");
            isIdle=false;
            _wasSprinting=false;
        }
        //SPRINT STATE
        if(isSprinting && isMoving && !_wasSprinting)
        {
            EmitSignal(SignalName.PlayerSprinting);
            //HeadBobbing?.Play("sprint");
            isIdle=false;
            _wasSprinting=true;
        }

        //CROUCH STATE

        _wasMoving = isMoving;
    }

    Vector3 Headbobbing(float time)
    {
        Vector3 pos = Vector3.Zero;
        pos.X = Mathf.Sin(time*bobbingFrequecy)*bobbingAmplitude;
        pos.Y = Mathf.Cos(time*bobbingFrequecy/2)*bobbingAmplitude;
        return pos;
    }

	private void InitializeAnimations()
    {
        HeadBobbing?.Play("RESET");
        JumpAnimation?.Play("RESET");
        CrouchAnimation?.Play("RESET");
    }
    #endregion

    #region Movement States Transitions

    public void EnterIdleState()
    {
        if(isMoving)
        {
           // HeadBobbing.PlayBackwards();
        }
        isIdle=true;
        GD.Print("idle");
        
    }

    public void EnterWalkState()
    {
        currentSpeed = walkSpeed;
        isIdle=false;
        GD.Print("walk");
    }


    public void EnterSprintState()
    {
        currentSpeed = sprintSpeed;
        GD.Print("sprint");
    }

    public void EnterCrouchState()
    {
        
    }

    #endregion

}
    /*
    /* =======================
     * CHARACTER SETTINGS
     * ======================= */
    /*
    [Export] public float BaseSpeed = 3.0f;
    [Export] public float SprintSpeed = 6.0f;
    [Export] public float CrouchSpeed = 1.0f;

    [Export] public float Acceleration = 10.0f;
    [Export] public float JumpVelocity = 4.5f;
    [Export] public float MouseSensitivity = 0.1f;

    [Export] public bool InvertCameraXAxis = false;
    [Export] public bool InvertCameraYAxis = false;
    [Export] public bool Immobile = false;

    [Export(PropertyHint.File)] public string DefaultReticle = "";

  
    [ExportCategory("Nodes")]
    [Export] public Node3D HEAD;
    [Export] public Camera3D CAMERA;
    [Export] public AnimationPlayer HEADBOB_ANIMATION;
    [Export] public AnimationPlayer JUMP_ANIMATION;
    [Export] public AnimationPlayer CROUCH_ANIMATION;
    [Export] public CollisionShape3D COLLISION_MESH;

    [ExportCategory("Controls")]
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

    [Export] public bool ControllerSupport = false;

    [Export] public Dictionary ControllerControls = new()
    {[Signal]
    public delegate void PlayerSprintingEventHandler();
        { "LOOK_LEFT", "look_left" },
        { "LOOK_RIGHT", "look_right" },
        { "LOOK_UP", "look_up" },
        { "LOOK_DOWN", "look_down" }
    };

    [Export(PropertyHint.Range, "0.001,1,0.001")]
    public float LookSensitivity = 0.035f;

   
    [ExportCategory("Feature Settings")]
    [Export] public bool JumpingEnabled = true;
    [Export] public bool InAirMomentum = true;
    [Export] public bool MotionSmoothing = true;
    [Export] public bool SprintEnabled = true;
    [Export] public int SprintMode = 0;
    [Export] public bool CrouchEnabled = true;
    [Export] public int CrouchMode = 0;
    [Export] public bool DynamicFov = true;
    [Export] public bool ContinuousJumping = true;
    [Export] public bool ViewBobbing = true;
    [Export] public bool JumpAnimation = true;
    [Export] public bool PausingEnabled = true;
    [Export] public bool GravityEnabled = true;
    [Export] public bool DynamicGravity = false;

    private float _speed;
    private float _currentSpeed;
    private string _state = "normal";
    private bool _lowCeiling = false;
    private bool _wasOnFloor = true;

    private Control _reticle;
    private float _gravity;
    private Vector2 _mouseInput = Vector2.Zero;

    private RayCast3D _crouchCeilingDetection;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;

        HEAD.Rotation = new Vector3(HEAD.Rotation.X, Rotation.Y, HEAD.Rotation.Z);
        Rotation = new Vector3(Rotation.X, 0, Rotation.Z);

        _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

        _crouchCeilingDetection = GetNode<RayCast3D>("CrouchCeilingDetection");

        InitializeAnimations();
        EnterNormalState();

        if (OS.GetName() == "Web")
            Input.SetUseAccumulatedInput(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        float d = (float)delta;

        if (DynamicGravity)
            _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

        if (!IsOnFloor() && GravityEnabled)
            Velocity = new Vector3(Velocity.X, Velocity.Y - _gravity * d, Velocity.Z);

        HandleJumping();

        Vector2 inputDir = Immobile
            ? Vector2.Zero
            : Input.GetVector(
                Controls["LEFT"].ToString(),
                Controls["RIGHT"].ToString(),
                Controls["FORWARD"].ToString(),
                Controls["BACKWARD"].ToString()
            );

        HandleMovement(d, inputDir);
        HandleHeadRotation();

        _lowCeiling = _crouchCeilingDetection.IsColliding();
        HandleState(inputDir);

        if (DynamicFov)
            UpdateCameraFov();

        MoveAndSlide();
        _wasOnFloor = IsOnFloor();
    }

    private void HandleJumping()
    {
        if (!JumpingEnabled || _lowCeiling || !IsOnFloor())
            return;

        if (ContinuousJumping && Input.IsActionPressed(Controls["JUMP"].ToString()))
        {
            Velocity += Vector3.Up * JumpVelocity;
            JUMP_ANIMATION?.Play("jump", 0.25f);
        }
        else if (Input.IsActionJustPressed(Controls["JUMP"].ToString()))
        {
            Velocity += Vector3.Up * JumpVelocity;
            JUMP_ANIMATION?.Play("jump", 0.25f);
        }
    }

    private void HandleMovement(float delta, Vector2 input)
    {
        Vector3 direction = new Vector3(input.X, 0, input.Y)
            .Rotated(Vector3.Up, -HEAD.Rotation.Y);

        if (MotionSmoothing)
        {
            Velocity = new Vector3(
                Mathf.Lerp(Velocity.X, direction.X * _speed, Acceleration * delta),
                Velocity.Y,
                Mathf.Lerp(Velocity.Z, direction.Z * _speed, Acceleration * delta)
            );
        }
        else
        {
            Velocity = new Vector3(direction.X * _speed, Velocity.Y, direction.Z * _speed);
        }
    }

    private void HandleHeadRotation()
    {
        HEAD.RotateY(Mathf.DegToRad(-_mouseInput.X * MouseSensitivity * (InvertCameraXAxis ? -1 : 1)));
        HEAD.RotateX(Mathf.DegToRad(-_mouseInput.Y * MouseSensitivity * (InvertCameraYAxis ? -1 : 1)));

        HEAD.Rotation = new Vector3(
            Mathf.Clamp(HEAD.Rotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(90)),
            HEAD.Rotation.Y,
            0
        );

        _mouseInput = Vector2.Zero;
    }

    private void HandleState(Vector2 moving)
    {
        if (SprintEnabled && Input.IsActionPressed(Controls["SPRINT"].ToString()) && _state != "crouching" && moving.Length() > 0)
            EnterSprintState();
        else if (_state == "sprinting")
            EnterNormalState();

        if (CrouchEnabled && Input.IsActionPressed(Controls["CROUCH"].ToString()) && _state != "sprinting")
            EnterCrouchState();
        else if (_state == "crouching" && !_lowCeiling)
            EnterNormalState();
    }

    private void EnterNormalState()
    {
        if (_state == "crouching")
            CROUCH_ANIMATION?.PlayBackwards("crouch");

        _state = "normal";
        _speed = BaseSpeed;
    }

    private void EnterSprintState()
    {
        _state = "sprinting";
        _speed = SprintSpeed;
    }

    private void EnterCrouchState()
    {
        _state = "crouching";
        _speed = CrouchSpeed;
        CROUCH_ANIMATION?.Play("crouch");
    }

    private void InitializeAnimations()
    {
        HEADBOB_ANIMATION?.Play("RESET");
        JUMP_ANIMATION?.Play("RESET");
        CROUCH_ANIMATION?.Play("RESET");
    }

    private void UpdateCameraFov()
    {
        CAMERA.Fov = Mathf.Lerp(
            CAMERA.Fov,
            _state == "sprinting" ? 85f : 75f,
            0.3f
        );
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (e is InputEventMouseMotion motion && Input.MouseMode == Input.MouseModeEnum.Captured)
            _mouseInput = motion.Relative;
    }*/

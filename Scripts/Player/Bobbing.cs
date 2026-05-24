using Godot;

namespace PolarBears.PlayerControllerAddon;

public partial class Bobbing: Node3D
{
	[Export(PropertyHint.Range, "0,10,0.01,suffix:Hz,or_greater")]
	public float BobbingFrequency { set; get; } = 2.4f;
	[Export(PropertyHint.Range, "0,0.4,0.01,suffix:m,or_greater")]
	public float BobbingAmplitude { set; get; } = 0.08f;
	[Export] Node3D _BobbingPivot; //NEW SCRIPT

	private Camera3D _camera;
	[Export]Godot.AnimationPlayer cameraAnimator;

	public void Init(Camera3D cam)
	{
		_camera = cam;
	}

	public struct CameraBobbingParams
	{
		public float Delta;
		public bool IsOnFloorCustom;
		public Vector3 Velocity;
	}

	private float _bobbingAccumulator;  // Constantly increases when player moves in X or/and Z axis

	public void PerformCameraBobbing(CameraBobbingParams parameters)
	{
		
		if (parameters.IsOnFloorCustom)
		{
			// Head bob
			_bobbingAccumulator += parameters.Delta * parameters.Velocity.Length();

			//NEW CODE
			Vector3 newPos = Vector3.Zero;
			Vector3 newRot = Vector3.Zero;

			newPos.Y=Mathf.Sin(_bobbingAccumulator * BobbingFrequency) * BobbingAmplitude;;
			newPos.X=Mathf.Cos(_bobbingAccumulator * BobbingFrequency / 1.0f) * BobbingAmplitude;


			//UP AND DOWN (PITCH)
			newRot.X = Mathf.Sin(_bobbingAccumulator * BobbingFrequency) * (BobbingAmplitude * 0.35f);
			//LEFT AND RIGHT (ROLL)
			newRot.Z = Mathf.Cos(_bobbingAccumulator * BobbingFrequency * 0.2f) * (BobbingAmplitude * 0.2f);

			//_BobbingPivot.Position = newPos;
			_BobbingPivot.Rotation = newRot;
		}
		else
		{
				// Smoothly return to center when stopped
			_bobbingAccumulator = 0;
			_BobbingPivot.Position = _BobbingPivot.Position.Lerp(Vector3.Zero, (float)parameters.Delta * 10f);
			_BobbingPivot.Rotation = _BobbingPivot.Rotation.Lerp(Vector3.Zero, (float)parameters.Delta * 10f);
		}
			//NEW CODE

			/*OLD CODE
			
			Vector3 newRotationForCamera = Vector3.Zero;
			Vector3 newPositionForCamera = Vector3.Zero;

			newRotationForCamera.X = Mathf.Sin(_bobbingAccumulator * BobbingFrequency) * BobbingAmplitude;
			newRotationForCamera.Y += _camera.Rotation.Y;
			newRotationForCamera.Z = Mathf.Cos(_bobbingAccumulator * BobbingFrequency / 2.0f) * BobbingAmplitude;

			newRotationForCamera  =  new Vector3(newRotationForCamera.X,newRotationForCamera.Y,newPositionForCamera.Z);

			// As the _bobbingAccumulator increases we're changing values for sin and cos functions.
			// Because both of them are just waves, we will be slide up with y and then slide down with y
			// creating bobbing effect. The same works for cos. As the _bobbingAccumulator increases the cos decreases and then increases

			newPositionForCamera.Y = Mathf.Sin(_bobbingAccumulator * BobbingFrequency) * BobbingAmplitude;
			newPositionForCamera.X = Mathf.Cos(_bobbingAccumulator * BobbingFrequency / 2.0f) * BobbingAmplitude;

			_camera.Position = newPositionForCamera;
			//_camera.Rotation = newRotationForCamera; 
			// 
			// OLD CODE*/
		
	}

	public void InteractBob()
	{
		//cameraAnimator.Play("CameraBobbing_Interact");
	}

	public void RunBob()
	{
		//cameraAnimator.Play("CameraBobbing_Run",customSpeed:0.5f);
	}

	public void WalkBob()
	{
		//cameraAnimator.Play("CameraBobbing_Walk",customSpeed:2f);
	}

	public void StopBob()
	{
		//cameraAnimator.Stop(false);
		//cameraAnimator.Play("RESET");
	}
}

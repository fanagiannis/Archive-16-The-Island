using Godot;
using PolarBears.PlayerControllerAddon;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

public partial class EquipmentController : Node3D
{
	public enum EquipmentState
	{
		Equipped,
		Unequipped
	}
	public enum EquipmentUsageState
	{
		Idle,
		Aiming
	}

	[Signal]
	delegate void AimEventHandler();
	[Signal]
	delegate void IdleEventHandler();
	[Export]Mouse MouseController;
	[Export]PlayerUIController playerUIController;
	private EquipmentUsageState currentEquipmentUsageState;
	private EquipmentState currentEquipmentState;
	[Export] AnimationTreeManager PlayerAnimationTree;
	[Export] EquipmentManager equipmentManager;

	[Export]
    public float BobSpeed = 2.0f; 
    [Export]
    public float BobAmount = 0.5f;
	//private int equippedIndex=0;
	
	public override void _Ready()
	{
		currentEquipmentState = EquipmentState.Unequipped;
		currentEquipmentUsageState = EquipmentUsageState.Idle;
		//MouseController = GetNode <Mouse>("root/Mouse");
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if(MouseController!=null)
		{
			if (@event is InputEventMouseButton inputEvent && MouseController.ControllerEnabled())
			{
				if (inputEvent.Pressed && inputEvent.ButtonIndex == MouseButton.Right && GetEquipmentState() == EquipmentState.Equipped)
				{
					currentEquipmentUsageState = EquipmentUsageState.Aiming;
					//PlayerAnimationTree.PlayAimPistol();
					EmitSignal(SignalName.Aim);
				}
				else if (!inputEvent.Pressed && inputEvent.ButtonIndex == MouseButton.Right && GetEquipmentState() == EquipmentState.Equipped)
				{
					currentEquipmentUsageState = EquipmentUsageState.Idle;
					//PlayerAnimationTree.PlayQuitAimPistol();
					
					EmitSignal(SignalName.Idle);
				}
				else if(inputEvent.Pressed && inputEvent.ButtonIndex == MouseButton.Left && GetEquipmentState() == EquipmentState.Equipped)
				{
					UseCurrentEquipment();
				}
				//GD.Print(currentEquipmentUsageState );
				else if (@event is InputEventMouseButton wheelEvent)
				{
					if (wheelEvent.ButtonIndex == MouseButton.WheelUp && wheelEvent.Pressed && GetEquipmentState() == EquipmentState.Equipped)
					{
						equipmentManager.ChangeEquipment();
					}
					else if (wheelEvent.ButtonIndex == MouseButton.WheelDown && wheelEvent.Pressed && GetEquipmentState() == EquipmentState.Equipped)
					{
						equipmentManager.ChangeEquipment();
					}
				}
			}
			
		}
	}

	public void UseCurrentEquipment()
	{
		if(equipmentManager!=null)
		{
			if(equipmentManager!=null && equipmentManager.GetEquipped())
			{
				equipmentManager.Use();
			}
			
		}
	}

	public void ReloadWeapon()
	{
		if (equipmentManager != null && equipmentManager.GetEquippedWeapon() != null)
		{
			if(equipmentManager.GetEquippedWeapon() is Weapon weapon)
				weapon.Reload();
		}
	}
	public async Task ChangeEquipmentState()
	{

		//index 0 -> idle
		//index 1 -> weapon
		if (!equipmentManager.HasEquipment())
		{
			Log.Instance.SetLog("No Equipment", 1);
			return;
		}

		// Toggle based on your local Enum state
		if (currentEquipmentState == EquipmentState.Unequipped)
		{
			// EQUIP LOGIC
			currentEquipmentState = EquipmentState.Equipped;

			equipmentManager.Equip(); // This should internally set its bool to true

			playerUIController.SetReticle(1);
			//playerUIController.SetEquipmentSlotVisibility(true);
			CheckEquipmentSlotVisibillity();
		}
		else
		{
			// UNEQUIP LOGIC
			currentEquipmentState = EquipmentState.Unequipped;

			equipmentManager.ResetEquipment();
			equipmentManager.Unequip(); // This should internally set its bool to false

			playerUIController.SetReticle(0);
			//playerUIController.SetEquipmentSlotVisibility(false);
		}

	}
	public void ChangeEquipmentUsageState()
	{
		if(currentEquipmentUsageState == EquipmentUsageState.Idle)
		{
			currentEquipmentUsageState = EquipmentUsageState.Aiming;
			
			EmitSignal(SignalName.Aim);
		}
			
		else if(currentEquipmentUsageState == EquipmentUsageState.Aiming)
		{
			currentEquipmentUsageState = EquipmentUsageState.Idle;
			//PlayerAnimationTree.PlayQuitAimPistol();
			EmitSignal(SignalName.Idle);
		}
	}

	public void CheckEquipmentSlotVisibillity()
	{	
		playerUIController.SetEquipmentSlotVisibility(currentEquipmentState==EquipmentState.Equipped);
	}

	public EquipmentUsageState GetEquipmentUsageState()
	{
		return currentEquipmentUsageState;
	}
	public EquipmentState GetEquipmentState()
	{
		return currentEquipmentState;
	}

	public void Animations(float _speedfactor,float time)
	{
		equipmentManager.Bob(_speedfactor*BobSpeed,BobAmount,time);
		//SignalName.Aim.add (PlayerAnimationTree.PlayAimPistol());
	}
}


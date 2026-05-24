using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[GlobalClass]
public partial class EquipmentManager : Node3D
{
	[Export] public Node3D CameraNode; // Drag your Camera3D here in the inspector
    [Export] public float SwayAmount = 1.2f;    // How far the weapon lags behind
    [Export] public float SmoothSpeed = 10.0f;
	[Export]int maxEquipment=0;
	float _time=0f;

	public List<PickableItem> equipment = new List<PickableItem>();	
	int currentequipmentIndex=0;
	//[Export] PackedScene currentequipment;

	[Export] Godot.AnimationPlayer animation;
	Node equipmentNode;
	private List<Node> equipmentNodes = new List<Node>(); 
	bool Equipped=false;
	bool Aiming = false;
	private Vector3 _lastCameraRotation;
	
	// Called when the node enters the scene tree for the first time.
	private static EquipmentManager _instance;
	public static EquipmentManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GD.PrintErr("Log instance is null!");
            }
            return _instance;
        }
    }
	public override void _Ready()
	{
		_instance=this;
		if (CameraNode != null)
        {
            _lastCameraRotation = CameraNode.GlobalRotation;
        }
		//animation.AnimationFinished += OnAnimationFinished;
		/*equipmentNode = equipment[currentequipmentIndex].Instantiate();
		AddChild(equipmentNode);
		equipmentNode = equipment[currentequipmentIndex+1].Instantiate();
		AddChild(equipmentNode);
		*/
		SetUp();
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		WeaponSway(delta);
	}

	public void WeaponSway(double delta)
	{
		if (CameraNode == null) return;

        float d = (float)delta;

        // 1. Calculate how much the camera rotated since the last frame
        Vector3 currentCamRot = CameraNode.GlobalRotation;
        Vector3 rotDifference = currentCamRot - _lastCameraRotation;

        // Handle angle wrapping (prevents the gun from flipping if rotating past 180 degrees)
        rotDifference.Y = Mathf.Wrap(rotDifference.Y, -Mathf.Pi, Mathf.Pi);
        rotDifference.X = Mathf.Wrap(rotDifference.X, -Mathf.Pi, Mathf.Pi);

        // 2. The Target Rotation is the OPPOSITE of the camera's movement (the lag effect)
        // We set Z to rotDifference.Y * 0.5 to give it a nice lateral tilt!
        Vector3 targetRot = new Vector3(
            rotDifference.X * SwayAmount,
            -rotDifference.Y * SwayAmount,
            -rotDifference.Y * 0.5f * SwayAmount 
        );

        // 3. Smoothly Lerp the gun's LOCAL rotation back to zero (center screen)
        // By chasing zero, it automatically creates a trailing/spring effect!
        Rotation = Rotation.Lerp(targetRot, d * SmoothSpeed);

        // 4. Save this frame's rotation for the next frame
        _lastCameraRotation = currentCamRot;
	}

	public void SetUp()
	{
		foreach (var equipmentScene in equipment)
		{
			Node newEquipment = equipmentScene.GetItemInstance().Instantiate();
			AddChild(newEquipment);

			// Cast to Node2D or Node3D and set Visible
			SetVisibility(newEquipment,false);
			equipmentNodes.Add(newEquipment);
		}



	}

	private void CreateEquipmentNode(PickableItem scene)
	{

		Node newEquipment = scene.GetItemInstance().Instantiate();
		AddChild(newEquipment);
		SetVisibility(newEquipment, false);
		equipmentNodes.Add(newEquipment);
	}

	public void AddEquipment(PickableItem item)
	{
		if(CanPickup())
		{
			equipment.Add(item);   
			CreateEquipmentNode(item);	
			EquipmentScreen.Instance.Update();	
			//Log.Instance.SetLog($"Added {itemScene.ResourcePath} to inventory.",1);
		}
		else
			Log.Instance.SetLog($"Max equipment.",1);
		
	}

	public virtual void RemoveItem(PickableItem item)
	{
		// Find where the item currently sits in the list
		int index = equipment.IndexOf(item);
		
		if (index != -1)
		{
			Log.Instance.SetLog(equipment[index].name + " removed", 1);

			if (index < equipmentNodes.Count)
			{
				Node nodeToRemove = equipmentNodes[index];
				equipmentNodes.RemoveAt(index);
				nodeToRemove.QueueFree(); // Safely removes it from the scene tree
			}

			equipment.RemoveAt(index);
			
			//equipment[index].QueueFree();
			EquipmentScreen.Instance.Update();
			
		}
		else
		{
			GD.Print("Item not found in inventory!");
		}
	}

	public void Equip()
	{
		if(equipment.Count>0)
		{
			SetEquipped(true);
			EquipmentScreen.Instance.Update();
			
			if (equipmentNodes[currentequipmentIndex] is Equipment equipment)
			{
				equipment.Visible= true;
				//equipment3D.Position = new Vector3(0f,-0.7f,0f);
				//GD.Print(equipment3D.Position);
				
			}
			animation.Play("Equip");
		}
		else
		{
			
		}
	}
	public void Unequip()
	{
		animation.Play("Unequip");
		//await ToSignal(animation, AnimationPlayer.SignalName.AnimationFinished);
		if (equipmentNodes[currentequipmentIndex] is Equipment equipment)
		{
			//equipment.Visible = false;
		}
	}

	public void ChangeEquipment()
	{
		if(equipment.Count>1)
		{
			Unequip();
			SetVisibility(equipmentNodes[currentequipmentIndex],false);
			currentequipmentIndex = (currentequipmentIndex + 1) % equipment.Count;
			Equip();
		}
		//SetVisibility(equipmentNodes[currentequipmentIndex],true);

	}
	public void SetEquipped(bool set)
	{
		Equipped=set;
	}

	public bool GetEquipped()
	{
		return Equipped;
	}
	
	public bool HasEquipment()
	{
		return equipment.Count>0;
	}
	public Weapon GetEquippedWeapon()
	{
		if (equipmentNodes[currentequipmentIndex] is Weapon weapon)
		{
			return weapon;
		}
		return null;
	}

	public void SetAim(bool set)
	{
		Aiming = set;
	}

	public bool GetAim()
	{
		return Aiming;
	}

	public void Use()
	{
		if(equipment[currentequipmentIndex]!=null && Equipped)
		{
			if (equipmentNodes[currentequipmentIndex] is Equipment equipment)
			{
				equipment.Use();
			}
			else if (equipmentNodes[currentequipmentIndex] is Flashlight fequipment)
			{
				fequipment.Use();
			}
			else if (equipmentNodes[currentequipmentIndex] is Weapon wequipment)
			{
				wequipment.Use();
			}
			else
			{
				GD.PrintErr("Instantiated node is not of type Equipment.");
			}
		}
		
	}

	public void ResetEquipment()
	{
		if(equipment[currentequipmentIndex]!=null && Equipped)
		{
			if (equipmentNodes[currentequipmentIndex] is Flashlight fequipment)
			{
				fequipment.ResetEquipment();
			}
			else
			{
				GD.PrintErr("Instantiated node is not of type Equipment.");
			}
		}
	}

	private void OnAnimationFinished(StringName animName)
	{
		if (animName == "Unequip")
		{
			SetEquipped(false);
			if (equipmentNode is Equipment equipment)
			{
				equipment.Hide();
			}
			//SetVisibility(equipmentNodes[currentequipmentIndex], false);
			 //NEEDS TO BE REPLACED WITH VISIBILITY NODE AFTER INVENTORY IS IMPLEMENTED
		}
	}

	public void Bob(float speed, float ammount,float time)
	{
		_time+=time;
		float bobOffsetX = Mathf.Sin(_time * speed) * ammount;
		float bobOffsetY = Mathf.Cos(_time * speed/2) * ammount/2;
        Rotation = new Vector3(bobOffsetX, bobOffsetY, 0);
	}

	public void ResetBobTime()
	{
		_time = 0f; // Resets the Sine wave
		Rotation = Vector3.Zero; // Resets local rotation to face forward
	}

	public void SetVisibility(Node Eq,bool set)
	{
		if (Eq is Node2D node2D)
			{
				node2D.Visible = set;
			}
			else if (Eq is Node3D node3D)
			{
				node3D.Visible = set;
			}
	}

	public void SetMaxEquipment(int value)
	{
		maxEquipment = value;
	}

	public bool CanPickup()
	{
		return equipment.Count<maxEquipment; 
	}

}

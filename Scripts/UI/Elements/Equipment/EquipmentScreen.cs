using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class EquipmentScreen : Panel
{
	[Export] protected PackedScene SlotInstance;
	[Export] HBoxContainer EquipmentSlotContainer;
	[Export] protected int InventorySize=2;
	[Export]EquipmentSlotControlPanel equipmentSlotControlPanel;
	[Export]EquipmentController equipmentController;
	
	
	protected static EquipmentScreen _instance;
	public static EquipmentScreen Instance
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
		EquipmentManager.Instance.SetMaxEquipment(InventorySize);
		Update();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}


	public void Update()
	{
		if(SlotInstance!=null)
		{
			foreach (Node child in EquipmentSlotContainer.GetChildren())
			{
				child.QueueFree();
			}
			int itemCounter = 0;
			for(int c = 0; c<=InventorySize-1; c++)
			{
				EquipmentSlotContainer.Theme = GD.Load<Theme>("res://Textures/UI/Themes/MainUITheme.tres");
				//EquipmentSlotContainer.AddThemeConstantOverride("separation",80);
				EquipmentSlotContainer.SizeFlagsVertical= (Control.SizeFlags.ShrinkCenter| Control.SizeFlags.Expand);
		
				EquipmentSlot slot = (EquipmentSlot)SlotInstance.Instantiate();
				//slot.SetSlotScale(new Vector2(2,2));
					
				EquipmentSlotContainer.AddChild(slot);
				
				if (itemCounter <= EquipmentManager.Instance.equipment.Count-1)
				{
					
					slot.SetSlotItem(EquipmentManager.Instance.equipment[itemCounter]);
					slot.SetSlotIndex(itemCounter);
					slot.SetUpSlot(); 
					itemCounter++;
				}
				else
				{
					slot.SetSlotItem(null); 
					slot.SetUpSlot(); 
				}
			}
		}
	}

	public EquipmentSlotControlPanel GetEquipmentSlotControlPanel()
	{
		return equipmentSlotControlPanel;
	}

	public EquipmentController GetEquipmentController()
	{
		return equipmentController;
	}
}

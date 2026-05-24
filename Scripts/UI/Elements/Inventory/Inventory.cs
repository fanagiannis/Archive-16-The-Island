using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http.Headers;

[GlobalClass]
public partial class Inventory : Panel
{
	[Export] public Button ExitButton;
	[Export] protected PackedScene SlotInstance;
	[Export] protected VBoxContainer SlotContainer;
	[Export] protected  int InventorySize=7;
	[Export] protected InventorySlotControlPanel equipmentControlPanel;
	protected  List<PickableItem> InventoryList = new List<PickableItem>();

	protected int InventoryRows;
	protected static int InventoryColumns=7;
	protected bool Open=false;
	protected static Inventory _instance;
	public static Inventory Instance
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
        Hide();
		
		InventoryRows = InventorySize/InventoryColumns;
		SetInventory();
    }

	public virtual void OpenPanel()
	{
		SetOpen();
		Visible=!Visible;
	}

	public void SetOpen()
	{
		Open = !Open;
	}

	public virtual void SetInventory()
	{
		foreach (Node child in SlotContainer.GetChildren())
		{
			child.QueueFree();
		}
		int rowsNeeded = Mathf.CeilToInt((float)InventorySize / InventoryColumns);
    	int itemCounter = 0;
		for(int c = 0; c<=InventoryRows-1; c++)
		{
			HBoxContainer H = new HBoxContainer();
			H.Theme = GD.Load<Theme>("res://Textures/UI/Themes/MainUITheme.tres");
			H.AddThemeConstantOverride("separation",80);
			H.SizeFlagsVertical= (Control.SizeFlags.ShrinkBegin | Control.SizeFlags.Expand);
			SlotContainer.AddChild(H);
			for (int r = 0; r < InventoryColumns; r++)
			{
				InventorySlot slot = (InventorySlot)SlotInstance.Instantiate();
				
				H.AddChild(slot);
				if (itemCounter <= InventoryList.Count-1)
				{
					//Log.Instance.SetLog("added",1);
					slot.SetSlotItem(InventoryList[itemCounter]);
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

	public void UpdateInventory()
	{
		
	}

	public virtual void AddItem(PickableItem item)
	{
		// Check if there is an empty hole somewhere in the middle of the inventory
		int emptyIndex = InventoryList.IndexOf(null);

		if (emptyIndex != -1)
		{
			// Put the new item into the first empty slot we found
			InventoryList[emptyIndex] = item;
		}
		else if (InventoryList.Count < InventorySize)
		{
			// If there are no holes, but we still have space overall, add to the end
			InventoryList.Add(item);
		}
		else
		{
			Log.Instance.SetLog("Inventory is full!",1);
			return; // Don't add the item or update the UI
		}

		//Log.Instance.SetLog("Inventory updated", 1);
		SetInventory();
	}
	public virtual void RemoveItemAtIndex(int index)
	{
		if (index >= 0 && index < InventoryList.Count)
		{
			//Log.Instance.SetLog(InventoryList[index].name +" removed",1);
			InventoryList[index] = null; // Leave the empty hole
			//Log.Instance.SetLog(" removed",1);
			SetInventory(); // Rebuild UI
		}
	}
	public virtual void RemoveItem(PickableItem item)
	{
		// Find where the item currently sits in the list
		int index = InventoryList.IndexOf(item);
		
		if (index != -1)
		{
			Log.Instance.SetLog(InventoryList[index].name +" removed",1);
			InventoryList[index] = null; 
			
			
			SetInventory(); 
		}
		else
		{
			GD.Print("Item not found in inventory!");
		}
	}
	public int GetInventorySize()
	{
		return InventorySize;
	}

	public void IncreaseInventorySize(int value)
	{
		InventorySize += value;
	}

	public int GetInventoryItemsNumber()
	{
		int itemcounter = 0;
		foreach (PickableItem item in InventoryList)
		{
			if(item!=null)
			{
				itemcounter++;
			}
		}
		return itemcounter;
	}

	public bool InventoryFull()
	{
		//GD.Print(GetInventoryItemsNumber());
		return GetInventoryItemsNumber()>=InventorySize;
	}

	public bool GetOpen()
	{
		return Open;
	}

	public Button GetButton()
	{
		return ExitButton;
	}

	public InventorySlotControlPanel GetControlPanel()
	{
		if(equipmentControlPanel!=null)
			return equipmentControlPanel;
		return null;
	}
}

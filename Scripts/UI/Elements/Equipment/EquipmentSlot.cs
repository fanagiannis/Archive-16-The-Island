using Godot;
using System;

public partial class EquipmentSlot : Panel
{
	[Export] AudioStream useSound;
	[Export] AudioStream emptySound;

	[Export] Button slotButton;
	[Export] Label slotNameLabel;
	[Export] public PackedScene ControlPanelScene;
	[Export]string SlotName;
	[Export]Texture2D SlotIcon;
	//ControlPanelScene
	private Label _slotlabel;
	private PickableItem slotItem;
	private int SlotIndex;
	//Item item;

	bool isEmpty=true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetUpSlot();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetUpSlot()
	{
		if(slotItem!=null)
		{
			SlotName=slotItem.name;
			SlotIcon=slotItem.GetIcon();
			SetUpSlotButton();

			isEmpty=false;
		}
		else
		{
			SlotName = "Empty";
			SlotIcon = null;
        	isEmpty = true;
		}
		/*
		SlotName=item.name;
		SlotIcon=item.GetIcon();
		SetUpSlotButton();*/

	}

	public void SetUpSlotButton()
	{
		if (slotButton != null)
		{
			slotButton.Icon = SlotIcon;
			slotButton.Pressed+=SlotPress;
			
			// Only scale if there is an icon
			if (SlotIcon != null)
			{
				slotButton.Icon = ScaleTextureToButton(SlotIcon, slotButton.Size / 1.5f);
			}
		}
			
		if (slotNameLabel != null)
			slotNameLabel.Text = string.IsNullOrEmpty(SlotName) ? "Empty" : SlotName;

	}

	public void OnMouseEntered()
	{
		_slotlabel.Text = SlotName;
		_slotlabel.Visible = true;
	} 

	public void OnMouseExited()
	{
		_slotlabel.Text = null;
		_slotlabel.Visible = false;
	} 

	public void Use()
	{
		Log.Instance.SetLog(slotItem.name + " uneqquiped",1);
		if (slotItem == null) 
		{
			return; 
		}
		
		if(EquipmentManager.Instance.CanPickup())
		{
			EquipmentManager.Instance.AddEquipment(slotItem);
			Inventory.Instance.RemoveItemAtIndex(SlotIndex);
			SlotName="Empty";
			SlotIcon=null;
			_slotlabel.Visible=false;
			_slotlabel.Text=null;
			UpdateSlot();
		}
		else
			Log.Instance.SetLog("Equipment Full",1);
		
		if (useSound != null)
        {
            useSound.InstantiatePlayback();
        }
	}

	public void ResetSlot()
	{
		if (slotItem != null)
		{
			EquipmentScreen.Instance.GetEquipmentController().ChangeEquipmentState();
			EquipmentManager.Instance.RemoveItem(slotItem);
		}
		else
		{
			GD.Print("Warning: Tried to reset a slot that had no item!");
		}

		SlotName = "Empty";
		SlotIcon = null;

		if (_slotlabel != null)
		{
			_slotlabel.Visible = false;
			_slotlabel.Text = string.Empty; 
		}
		else
		{
			GD.PrintErr("Error: _slotlabel is not assigned in EquipmentSlot!");
		}

		UpdateSlot();
	}

	public void UpdateSlot()
	{
		slotNameLabel.Text = SlotName;
		slotButton.Icon = SlotIcon;
	}

	public void SlotPress()
	{
		if(!isEmpty)
		{
			EquipmentScreen.Instance.GetEquipmentSlotControlPanel().Initialize(this,GetLocalMousePosition() - new Vector2(-10, 10));
			EquipmentScreen.Instance.GetEquipmentSlotControlPanel().ShowPanel();
			if (ControlPanelScene == null)
			{
				GD.PrintErr("You forgot to drag the ControlPanel.tscn into the Inspector!");
				return;
			}
		}
		//EquipmentScreen.Instance.GetEquipmentSlotControlPanel().ShowPanel();
	}
	

	public void SetSlotScale(Vector2 scale)
	{
		slotButton.Scale = scale;
		slotNameLabel.Scale = scale;
		
	}
	
	public void SetSlotItem(PickableItem item)
	{
		slotItem = item;
	}

	public void SetSlotIndex(int index)
    {
        SlotIndex = index;
    }

	public PickableItem GetSlotItem()
	{
		if(slotItem!=null)
			return slotItem;
		return null;
	}

	private Texture2D ScaleTextureToButton(Texture2D originalTexture, Vector2 iconSize)
    {
        var image = originalTexture.GetImage();
        if (image == null) return originalTexture;

        image.Resize((int)iconSize.X, (int)iconSize.Y, Image.Interpolation.Nearest);

        var scaledTexture = new ImageTexture();
        scaledTexture.SetImage(image);

        return scaledTexture;
    }
}

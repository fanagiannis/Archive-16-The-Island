using Godot;
using System;

public partial class InventorySlot : Panel
{
	[Export] AudioStream useSound;
	[Export] AudioStream emptySound;

	[Export] Button slotButton;
	[Export] Label slotNameLabel;
	[Export] public PackedScene ControlPanelScene;
	[Export] string SlotName;
	[Export] Texture2D SlotIcon;
	private Label _slotlabel;
	private PickableItem slotItem;

	private int SlotIndex;
	//Item item;

	bool isEmpty = true;
	// Called when the node enters the scene tree for the first time.
	/*public InventorySlot(string name,Texture2D icon)
	{
		SlotName = name;
		SlotIcon = icon;
	}*/
	public override void _Ready()
	{
		slotNameLabel.AutowrapMode = TextServer.AutowrapMode.Off;
		slotNameLabel.TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis;
		_slotlabel = new Label();
		AddChild(_slotlabel);
		_slotlabel.ZIndex = 1000;
		_slotlabel.Theme = GD.Load<Theme>("res://Textures/UI/Themes/MainUITheme.tres");
		_slotlabel.Visible = false;


		// Connect signals ONLY ONCE here
		if (slotButton != null)
		{
			slotButton.Pressed += SlotPressed;
			slotButton.MouseEntered += OnMouseEntered;
			slotButton.MouseExited += OnMouseExited;
		}
		SetUpSlotButton();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_slotlabel != null & _slotlabel.Visible == true)
		{
			_slotlabel.Position = GetLocalMousePosition() - new Vector2(-10, 10);
		}
	}

	public void SetUpSlot()
	{
		if (slotItem != null)
		{
			SlotName = slotItem.name;
			SlotIcon = slotItem.GetIcon();
			SetUpSlotButton();
			isEmpty = false;
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
		Log.Instance.SetLog(SlotIndex + " Used", 1);
		if (slotItem == null)
		{
			return;
		}
		if (EquipmentManager.Instance.CanPickup())
		{
			EquipmentManager.Instance.AddEquipment(slotItem);
			Inventory.Instance.RemoveItemAtIndex(SlotIndex);
			ResetSlot();
		}
		else
			Log.Instance.SetLog("Equipment Full", 1);

		if (useSound != null)
		{
			useSound.InstantiatePlayback();
		}
	}

	public void SlotPressed()
	{
		if(!isEmpty)
		{
			Inventory.Instance.GetControlPanel().Initialize(this,GetLocalMousePosition() - new Vector2(-10, 10));
			Inventory.Instance.GetControlPanel().ShowPanel();
			if (ControlPanelScene == null)
			{
				GD.PrintErr("You forgot to drag the ControlPanel.tscn into the Inspector!");
				return;
			}
		}
		
	}

	public void ResetSlot()
	{
		Inventory.Instance.RemoveItem(slotItem);
		SlotName = "Empty";
		SlotIcon = null;
		_slotlabel.Visible = false;
		_slotlabel.Text = null;
		UpdateSlot();
	}

	public void UpdateSlot()
	{
		slotNameLabel.Text = SlotName;
		slotButton.Icon = SlotIcon;
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
		if (slotItem != null)
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

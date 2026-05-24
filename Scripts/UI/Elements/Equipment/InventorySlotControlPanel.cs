using Godot;
using System;

public partial class InventorySlotControlPanel : VBoxContainer
{
	
	InventorySlot associatedSlot;
	Button UseButton;
	protected Button DiscardButton;
	protected Button BackButton;
	

	public virtual void Initialize(InventorySlot slot, Vector2 spawnPosition)
    {
		SetAssociatedSlot(slot);
       
		this.Position = spawnPosition - new Vector2(0, 10);
		
    }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UseButton = GetNode<Button>("UseButton");
		DiscardButton = GetNode<Button>("DiscardButton");
		BackButton = GetNode<Button>("BackButton");

		
		UseButton.Pressed+=UseButtonPress;
		DiscardButton.Pressed+=DiscardButtonPress;
		BackButton.Pressed+=HidePanel;
		HidePanel();

		//_slotlabel = new Label();
		//AddChild(_slotlabel);
		//_slotlabel.ZIndex = 1000;
		//_slotlabel.Theme = GD.Load<Theme>("res://Textures/UI/Themes/MainUITheme.tres");
		//_slotlabel.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public virtual void SetAssociatedSlot(InventorySlot slot)
	{
		associatedSlot = slot;
	}


	public virtual void ShowPanel()
	{
		Visible=true;
	}
	public virtual void HidePanel()
	{
		Visible=false;
	}

	public virtual void UseButtonPress()
	{
		if (associatedSlot != null)
		{
			associatedSlot.Use();
			HidePanel();
		}
	}

	public virtual void DiscardButtonPress()
	{
		if (associatedSlot != null)
		{
			associatedSlot.ResetSlot();
			HidePanel();
			GD.Print(Inventory.Instance.GetInventoryItemsNumber());
		}
	}
}

using Godot;
using System;
using System.ComponentModel;

public partial class Collectible : Interactable
{
	[Export] public string UniqueId; // Set a unique name in the Inspector for every placed item
    [Export] protected string CollectibleName;

	public string GetItemName()
	{
		return CollectibleName;
	}

	public string GetID()
	{
		return UniqueId;
	}
}

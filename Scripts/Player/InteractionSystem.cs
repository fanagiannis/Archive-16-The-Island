using Godot;
using System;
using System.Threading.Tasks;


public partial class InteractionSystem : Node
{
	[Signal]
	delegate void InteractionEventHandler();
	[Signal]
	delegate void NPCInteractionEventHandler();
	[Signal]
	delegate void NoteInteractionEventHandler();
	[Export] RayCast3D interactionRaycast;
	[Export] Interactable itemToInteract;
	//[Export] NPC npcToInteract;

	[Export] EquipmentManager equipmentManager;

	[Export] PlayerUIController playerUI;
	[Export] Godot.AnimationPlayer HandsAnimationPlayer;

	[Export] AnimationTreeManager PlayerAnimationTree;
	[Export]public Camera3D playercamera ;

	public override void _Ready()
	{
	
		//Camera3D playercamera = GetNode<Camera3D>("root/Head/CameraSmooth/Camera3D");
       
	}

	
	public override void _Process(double delta)
	{
		if(interactionRaycast==null) return;

		if(interactionRaycast.IsColliding())
		{
			var collider = interactionRaycast.GetCollider();
			if(collider is Interactable)
			{
				var item = collider as Interactable;
				if(item is Interactable && item.GetInteracted()==false &&itemToInteract==null)
				{
					itemToInteract = item;
					itemToInteract.EnterInteraction();
					if(playerUI!=null)
					{
						playerUI.SetReticleLabel(itemToInteract.name);
					}
				}
				/*else if(itemToInteract!=null && item.GetInteracted())
				{
					itemToInteract.ExitInteraction();
					itemToInteract = null;
					playerUI.SetReticleLabel("");
					playerUI.EndDialogue();
				}*/
				
				
			}

			else if(itemToInteract!=null)
			{
				itemToInteract.ExitInteraction();
				itemToInteract = null;
				playerUI.SetReticleLabel("");
				//playerUI.EndDialogue();
			}
		}
		else
		{
			
			if(itemToInteract!=null) 
			{
				itemToInteract.ExitInteraction();
				itemToInteract = null;
				playerUI.SetReticleLabel("");
				//playerUI.EndDialogue();
			}
			
			else
				return;
		}
	}

	public async void Interact()
	{
		if (itemToInteract == null) return;

		else
		{
			if (itemToInteract is Note note)
			{
				//NPC npc = Interaction.GetInteractable() as NPC;
				bool checkReadNote = NoteManager.Instance.IsNoteUnlocked(note.GetID());
				itemToInteract.Interact();
				
				//note.GetRead();
				playerUI.ReadNote(note.GetNoteName(), note.GetNoteText(),checkReadNote);
				EmitSignal(SignalName.NoteInteraction);
			}
			if (itemToInteract is NPC npc)
			{
				//NPC npc = Interaction.GetInteractable() as NPC;
				itemToInteract.Interact();
				playerUI.BeginDialogue(npc.GetNPCName(), npc.GetDialogue());
				EmitSignal(SignalName.NPCInteraction);
				PossessCamera(npc.GetCamera());
			}
			if (itemToInteract is PickableItem pickableItem)
			{
				PackedScene itemScene = pickableItem.GetItemInstance();
				if(!Inventory.Instance.InventoryFull())
				{
					itemToInteract.Interact();
				}
				else
					Log.Instance.SetLog("InventoryFull",1);
				if (itemScene != null && itemToInteract != null && !Inventory.Instance.InventoryFull())
				{
					//equipmentManager.AddEquipment(itemScene);
					Inventory.Instance.AddItem(pickableItem);
					EmitSignal(SignalName.Interaction);
				}
			}
			

			if (itemToInteract is PickUp pickUp)
			{
				if(!Inventory.Instance.InventoryFull())
				{
					itemToInteract.Interact();
				}
				else
					Log.Instance.SetLog("InventoryFull",1);
				if (pickUp != null && itemToInteract != null && !Inventory.Instance.InventoryFull())
				{
					//equipmentManager.AddEquipment(itemScene);
					//GD.Print("PICKED");
					Inventory.Instance.AddItem(pickUp.GetPickableItem());
					EmitSignal(SignalName.Interaction);
				}
			}
			if (itemToInteract is InteractableEventItem)
			{
				if (HandsAnimationPlayer != null && itemToInteract != null)
				{

					EmitSignal(SignalName.Interaction);

					itemToInteract.SetInteracted(true);
					itemToInteract.EnterInteraction();
					//EmitSignal(SignalName.Interaction);

					await PlayAnimationAndInteract();
					//itemToInteract.SetInteracted(false);
					EmitSignal(SignalName.Interaction);
					//PlayerAnimationTree.Set("parameters/Is Pickingitem", true);
					//PlayerAnimationTree.Set("parameters/conditions/is_interacting", true);

					//var stateMachine = (AnimationNodeStateMachinePlayback)PlayerAnimationTree.Get("parameters/playback");


					//PlayerAnimationTree.Set("parameters/conditions/is_interacting", true);


				}
			}
			else if(itemToInteract!=null && itemToInteract is not PickableItem)
			{
				itemToInteract.Interact();
			}
			itemToInteract = null;
			playerUI.SetReticleLabel("");
		}

	}
	public void PossessCamera(Camera3D camera)
	{
		camera.MakeCurrent();
	}

	public void ResetCamera()
	{
		playercamera.MakeCurrent();
	}
	public Interactable GetInteractable()
	{
		if(itemToInteract!=null)
		{
			return itemToInteract;
		}
		return null;
	}

	private async Task PlayAnimationAndInteract()
	{
		await PlayerAnimationTree.PlayInteract();
		itemToInteract.Interact();
    	itemToInteract = null;
        playerUI.SetReticleLabel("");
		return;
	}

	private async Task PlayAnimationAndPickup()
	{
		await PlayerAnimationTree.PlayPickup();
		itemToInteract.Interact();
    	itemToInteract = null;
        playerUI.SetReticleLabel("");
		return;
	}

}

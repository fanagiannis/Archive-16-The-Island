using Godot;
using System;
using System.Threading.Tasks;

public partial class PlayerUIController : Control
{
	[Export]Reticle Reticle;
	[Export] Inventory ItemInventory;
	[Export] Panel EquipmentSlots;
	[Export]float lerpspeed = 0.2f;
	[Export]float lerptargetscalemax = 1.2f;
	[Export]float lerptargetscaleaim = 0.3f;
	[Export]float lerptargetscalemin = 1f;
	[Export] Godot.AnimationPlayer UIAnimator;
	Label InteractPrompt;
	Vitality VitalityController;
	public DialogueDisplay DialogueController;
	public override void _Ready()
	{
		InteractPrompt = GetNode<Label>("Interact Prompt");
		DialogueController = GetNode<DialogueDisplay>("DialogueDisplay");
		SetReticleLabel("");
		SetEquipmentSlotVisibility(true);
		//SetUIVisibility(false);
	}

	public void SetUIVisibility(bool set)
	{
		this.Visible = set;
	}


	public void SetEquipmentSlotVisibility(bool set)
	{
		if(EquipmentSlots!=null)
		{
			if (set)
				EquipmentSlots.Show();
			else if(!set)
				EquipmentSlots.Hide();
		}
		
	} 

	public void SetReticleLabel(string value)
	{
		if (InteractPrompt!=null)
		{
			InteractPrompt.Text = value;
		}
		
	}

	public void BeginDialogue(string npc,string dialoguue)
	{
		SetEquipmentSlotVisibility(false);
		DialogueController.ShowDialogueScreen(npc,dialoguue);
	}

	public void EndDialogue()
	{
		DialogueController.HideDialogueScreen();
	}

	public void SetReticle(int index)
	{
		Reticle.SetReticle(index);
	}

	public void OpenInventory()
	{
		ItemInventory.OpenPanel();
	}

	public void LerpReticlePositive()
	{
		Reticle.LerpReticle(lerpspeed,lerptargetscalemax);
	}
	public void LerpReticleReset()
	{
		Reticle.LerpReticle(lerpspeed,lerptargetscalemin);
	}
	public void LerpReticleAim()
	{
		Reticle.LerpReticle(lerpspeed,lerptargetscaleaim);
	}

	public void FadeIn()
	{
		if(UIAnimator!=null)
		{
			UIAnimator.Play("FadeIn");
		}
	}

	public void FadeOut()
	{
		if(UIAnimator!=null)
		{
			UIAnimator.PlayBackwards("FadeOut");
		}
	}

	public async Task WaitAnimator()
	{
		if(UIAnimator!=null)
		{
			await ToSignal(UIAnimator, AnimationMixer.SignalName.AnimationFinished);
		}
	}

	public Button GetInventoryExitButton()
	{
		return ItemInventory.GetButton();
	}
}

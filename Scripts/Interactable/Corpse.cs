using Godot;
using System;
using System.Dynamic;

public partial class Corpse : Interactable
{

	public override void _Ready()
	{
		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
    {
        
    }

	public override void Interact()
	{
		if(!Interacted)
		{
			base.Interact();
			Interacted=true;
			SceneManager.Instance.GetQuestManager().TrackProgress("The Stage",1);
			FetchQuest fetchQuestref = SceneManager.Instance.GetQuestManager().GetActiveQuest() as FetchQuest;
			if(fetchQuestref.CurrentAmount>0 && fetchQuestref.CurrentAmount<3 )
			{
				//SceneManager.Instance.GetCurrentLevel().GetEnemyManager().EnableAllEnemies();
				SceneManager.Instance.GetCurrentLevel().GetEnemyManager().EnableEnemy(0);
				SceneManager.Instance.EmitSignal(SceneManager.SignalName.Escalation);
				//GD.Print("ENEMIES ENABLED");
			}
			else if(fetchQuestref.CurrentAmount>=4 )
			{
				SceneManager.Instance.GetCurrentLevel().GetEnemyManager().EnableEnemy(1);
				SceneManager.Instance.EmitSignal(SceneManager.SignalName.EscalationB);
				//GD.Print("ENEMIES ENABLED");
			}
			else if(fetchQuestref.CurrentAmount>=4 )
			{
				SceneManager.Instance.GetCurrentLevel().GetEnemyManager().EnableEnemy(1);
				SceneManager.Instance.EmitSignal(SceneManager.SignalName.EscalationC);
				//GD.Print("ENEMIES ENABLED");
			}
		}
	}
	
}

using Godot;
using System;
using System.Collections;

public partial class QuestLog : Label
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var Quest = SceneManager.Instance.GetQuestManager().GetActiveQuest();
		
		if(Quest is FetchQuest)
		{
			FetchQuest fetchQuest = Quest as FetchQuest;
			this.Text = "Found : " + fetchQuest.CurrentAmount + "/" + fetchQuest.RequiredAmount ;
		}
		if(Quest is Quest) return;
			
	}

    public override void _EnterTree()
    {
		
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UpdateLog()
	{
		var Quest = SceneManager.Instance.GetQuestManager().GetActiveQuest();
		GD.Print("UPDATED LOG");
		
		if(Quest is FetchQuest)
		{
			FetchQuest fetchQuest = Quest as FetchQuest;
			if(Quest.Completed) this.Text = "Completed";
			this.Text = "Found : " + fetchQuest.CurrentAmount + "/" + fetchQuest.RequiredAmount ;
			GD.Print(this.Text);
		}
		if(Quest is Quest) return;
	}
}

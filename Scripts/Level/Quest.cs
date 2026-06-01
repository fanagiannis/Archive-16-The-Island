using Godot;
using System;
using System.Dynamic;

[GlobalClass]
public partial class Quest : Resource
{
	protected enum QuestState
	{
		Inactive,Active,Completed
	}
	protected QuestState CurrentQuestState = QuestState.Inactive;
    [Export] public string QuestId { get; set; }
    [Export] public string Title { get; set; }
    [Export] public string Description { get; set; }
	[Export] public string ObjectiveTarget { get; set; } 
	public bool Completed;
	public virtual void Progress(int amount)
    {
        if (Completed) return;
    }

	public virtual void SetComplete()=>CurrentQuestState = QuestState.Completed;

	public virtual void ActivateQuest()=>CurrentQuestState = QuestState.Active;
	
}

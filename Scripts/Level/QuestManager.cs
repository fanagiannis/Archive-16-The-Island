using Godot;
using PolarBears.PlayerControllerAddon;
using System;
using System.Collections.Generic;

public partial class QuestManager : Node
{
	[Signal] public delegate void QuestAcceptedEventHandler(Quest quest);
    [Signal] public delegate void QuestProgressedEventHandler(Quest quest);
    [Signal] public delegate void QuestCompletedEventHandler();
	public List<Quest> ActiveQuests { get; private set; } = new();
    public List<Quest> CompletedQuests { get; private set; } = new();
    
    
    public override void _Ready()
    {

    }

    public void Reset()
    {
        ActiveQuests.Clear();
        CompletedQuests.Clear();
    }
	public void AcceptQuest(Quest quest)
    {
        if (ActiveQuests.Contains(quest) || CompletedQuests.Contains(quest)) return;

		quest.ActivateQuest();
        ActiveQuests.Add(quest);
		
        //GD.Print($"Accepted Quest: {quest.Title}");
        EmitSignal(SignalName.QuestAccepted, quest);
        foreach(Quest questactive in ActiveQuests)
        {
            //if(questactive is FetchQuest)
               // GD.Print(questactive.Title);
        }
	//	Log.Instance.SetLog(quest.Title,5);
    }

	public void TrackProgress(string target, int amount)
    {
        GetActiveQuest().Progress(1);
        if (GetActiveQuest() is FetchQuest)
        {
            FetchQuest quest = GetActiveQuest() as FetchQuest;
            if(quest.CheckCompleted())
            {
                CompleteQuest(GetActiveQuest());

            }
                
            
        }
        /*
        foreach (var quest in ActiveQuests.ToArray())
        {
            if (quest.ObjectiveTarget == target)
            {
                quest.Progress(amount);
                EmitSignal(SignalName.QuestProgressed, quest);

                if (quest.Completed)
                {
                    CompleteQuest(quest);
                }
            }
        }*/
    }

	private void CompleteQuest(Quest quest)
    {
		quest.SetComplete();
        ActiveQuests.Remove(quest);
        CompletedQuests.Add(quest);
        GD.Print($"Completed Quest: {quest.Title}!");
        SceneManager.Instance.GetCurrentLevel().completeLevelItem.SetCanInteract(true);
        EmitSignal(SignalName.QuestCompleted, quest);
    }

    public Quest GetActiveQuest()
    {
        foreach(Quest questactive in ActiveQuests)
        {
            if(questactive is FetchQuest) return questactive as FetchQuest;
            if(questactive is Quest) return questactive as Quest;
            
        }
        return null;
    }
}

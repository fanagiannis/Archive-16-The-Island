using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

[GlobalClass]
public partial class FetchQuest : Quest
{
	[Export] public int RequiredAmount { get; set; } = 1;
    public int CurrentAmount { get; set; } = 0;
	public override void Progress(int amount)
    {
        base.Progress(amount); 
		CurrentAmount+=amount;
		GD.Print(CurrentAmount + "/" + RequiredAmount);
		
    }

	public void Reset()
	{
		RequiredAmount = 1;
		CurrentAmount = 0;
		CheckCompleted();
	}

	public bool CheckCompleted()
	{
		return Completed = CurrentAmount>=RequiredAmount;
	}
		
}

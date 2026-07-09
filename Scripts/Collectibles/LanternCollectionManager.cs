using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

[GlobalClass]
public partial class LanternCollectionManager : Node
{
	public static LanternCollectionManager Instance { get; private set; }
	[Export] private ExitDoor exitDoor;
	private bool AllLanternsCollected=false;
	private List<Lantern> lanternCollection= new List<Lantern>();
	private const string SavePath = "user://bunker_door_save.json";
	int lanternCount = 0;
	public override void _Ready()
	{
		Instance = this;
		LoadBunkerState();
		SetupLanterns();
		CheckLanterns();
	}
	public override void _Process(double delta)
	{
	}
	public void SetupLanterns()
	{
		foreach(Lantern lantern in GetChildren())
		{
			lanternCollection.Add(lantern);
			if (AllLanternsCollected)
            {
                lantern.EnableLantern();
				
            }	
		}
		CheckCollection();
		
		
		Log.Instance.SetLog(lanternCollection.Count.ToString(),3);

	}
	public void CheckLanterns()
	{
		lanternCount=0;
		foreach(Lantern lantern in lanternCollection)
		{
			if(lantern.Interacted==true)
				lanternCount++;
		}
		if(lanternCount>=lanternCollection.Count)
		{
			AllLanternsCollected=true;
			Log.Instance.SetLog("All Lanterns Collected",2);
			SaveBunkerState();
		}
			
		CheckCollection();
	}
	public void CheckCollection()
	{
		if(AllLanternsCollected==true&&exitDoor !=null)
			exitDoor.Lock(false);	
	}

	private void SaveBunkerState()
    {
        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        if (file == null) return;

        // Save the true/false value to the file
        string json = JsonSerializer.Serialize(AllLanternsCollected);
        file.StoreString(json);
    }

	private void LoadBunkerState()
    {
        if (!FileAccess.FileExists(SavePath)) return;

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        if (file == null) return;

        try
        {
            // Read the true/false value back into the variable
            AllLanternsCollected = JsonSerializer.Deserialize<bool>(file.GetAsText());
        }
        catch 
        {
            GD.PrintErr("Failed to load bunker state.");
        }
    }
}

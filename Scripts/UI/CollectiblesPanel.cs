using Godot;
using System;
using System.Text.Json;
public partial class CollectiblesPanel : Panel
{
	[Export]
	public GridContainer CollectibleContainer;
	[Export]
	public CollectibleDisplay collectibleDisplayInstance;
	private const string SavePath = "user://unlocked_notes.json";
	private const string NotePath = "res://Scenes/Items/Interactable/JSON/Notes/";
	private int noteCount = 0;
	private string [] notesList;
	private Label noteNameLabel;
	private RichTextLabel noteTextLabel;
	public override void _Ready()
	{
		noteNameLabel=GetNode<Label>("NoteName");
		noteTextLabel=GetNode<RichTextLabel>("NoteText");
		noteCount = NotesInDirectory();
		notesList = NotesListInDirectory();
		GD.Print(noteCount);

	}
	public override void _Process(double delta)
	{
	}

	public void InstantiateNoteDisplays()
	{
		int i=0;
		ClearList();
		for(i=0;i<noteCount;i++)
		{
			CollectibleDisplay display = new CollectibleDisplay();
			display.SetButtonText("Note "+i+"      ");
			display.SetJsonFile(NotePath+notesList[i]);
			display.Pressed += () =>LoadJsonText(display.GetJSON());			
			CollectibleContainer.AddChild(display);
		}
	}

	public void SetNotes()
	{
		
	}

	public void ClearList()
	{
		foreach(Node node in CollectibleContainer.GetChildren())
		{
			node.QueueFree();
		}
	}

	private string[] NotesListInDirectory()
    {
        if (!DirAccess.DirExistsAbsolute(NotePath))
        {
            GD.PrintErr($"Directory does not exist: {NotePath}");
            return null;
        }

        string[] files = DirAccess.GetFilesAt(NotePath);
        int count = 0;

        foreach (string file in files)
        {
            if (file.EndsWith(".json"))
            {
                count++;
            }
        }

        return files;
    }


	private int NotesInDirectory()
    {
        if (!DirAccess.DirExistsAbsolute(NotePath))
        {
            GD.PrintErr($"Directory does not exist: {NotePath}");
            return 0;
        }

        string[] files = DirAccess.GetFilesAt(NotePath);
        int count = 0;

        foreach (string file in files)
        {
            if (file.EndsWith(".json"))
            {
                count++;
            }
        }

        return count;
    }

	private void LoadJsonText(string path)
    {
        if (string.IsNullOrEmpty(path) || !FileAccess.FileExists(path))
        {
            GD.PrintErr($"JSON file not found at path: {path}");
            return;
        }

        // 2. Open the file and read the raw string data
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        string jsonText = file.GetAsText();

        // 3. Deserialize the JSON string into our C# class
        try 
        {
            var data = JsonSerializer.Deserialize<NoteData>(jsonText);
			SetNoteDisplays(data.Title,data.Content);
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to parse JSON: {e.Message}");
        }
    }

	private class NoteData
    {
        public string Title { get; set; }
        public string Content { get; set; }
		public string Read { get; set; }
    }
	

	public void SetNoteDisplays(string nameset,string textset)
	{
		noteNameLabel.Text = nameset;
		noteTextLabel.Text = textset;
	}

}

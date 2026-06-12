using Godot;
using PolarBears.PlayerControllerAddon;
using System;
using System.Text.Json;

public partial class Note : Interactable
{
	[Export(PropertyHint.File, "*.json")]
    public string NoteFilePath { get; set; }

    private string _noteTitle = "Unknown";
    private string _noteContent = "No text found.";
	public override void _Ready()
	{
        audioPlayer = GetNode<AudioStreamPlayer3D>("AudioPlayer");
		LoadJsonText();
	}

	public override void _Process(double delta)
	{
		
	}

    public override void Interact()
    {
        base.Interact();
        audioPlayer.Stream= itemAudio; 
        audioPlayer.Play();
    }

    public override void SetOutline(bool set)
    {
      //  base.SetOutline(set);
    }

	public string GetNoteName()
	{
		return _noteTitle;
	}

	public string GetNoteText()
	{
		return _noteContent ;
	}

	private void LoadJsonText()
    {
        // 1. Check if the file path is set and the file exists
        if (string.IsNullOrEmpty(NoteFilePath) || !FileAccess.FileExists(NoteFilePath))
        {
            GD.PrintErr($"JSON file not found at path: {NoteFilePath}");
            return;
        }

        // 2. Open the file and read the raw string data
        using var file = FileAccess.Open(NoteFilePath, FileAccess.ModeFlags.Read);
        string jsonText = file.GetAsText();

        // 3. Deserialize the JSON string into our C# class
        try 
        {
            var data = JsonSerializer.Deserialize<NoteData>(jsonText);
            _noteTitle = data.Title;
            _noteContent = data.Content;
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to parse JSON: {e.Message}");
        }
    }

    // A helper class that matches the structure of your JSON file.
    // The property names here must match the keys in your JSON exactly.
    private class NoteData
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
	
}

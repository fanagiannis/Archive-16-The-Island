using Godot;
using System.Collections.Generic;
using System.Text.Json; // Make sure this namespace is included at the top!

[GlobalClass]
public partial class CollectibleManager : Node
{
    public static CollectibleManager Instance { get; private set; }

    private List<string> _collectedLanterns = new List<string>();
    private List<string> _collectedNotes = new List<string>();

    private const string SavePathNotes = "user://collectibles_notes_save.json";
    private const string SavePathLanterns = "user://collectibles_lantern_save.json";

    public override void _Ready()
    {
        Instance = this;
        LoadGame(); // Automatically load all progress when the game boots up
    }

    // --- LANTERN TRACKING ---
    public void RecordLantern(string id)
    {
        if (string.IsNullOrEmpty(id) || _collectedLanterns.Contains(id)) return;
        _collectedLanterns.Add(id);
        SaveGame();
    }

    public bool IsLanternCollected(string id)
    {
        return _collectedLanterns.Contains(id);
    }

    // --- NOTE TRACKING ---
    public void RecordNote(string id)
    {
        if (string.IsNullOrEmpty(id) || _collectedNotes.Contains(id)) return;
        _collectedNotes.Add(id);
        SaveGame();
    }

    public bool IsNoteCollected(string id)
    {
        return _collectedNotes.Contains(id);
    }

    // --- SAVE & LOAD CORE ---
    private void SaveGame()
    {
        // 1. Save Lanterns
        using var file_lanterns = FileAccess.Open(SavePathLanterns, FileAccess.ModeFlags.Write);
        if (file_lanterns != null)
        {
            string json_lanterns = JsonSerializer.Serialize(_collectedLanterns);
            file_lanterns.StoreString(json_lanterns);
        }

        // 2. Save Notes
        using var file_notes = FileAccess.Open(SavePathNotes, FileAccess.ModeFlags.Write);
        if (file_notes != null)
        {
            string json_notes = JsonSerializer.Serialize(_collectedNotes);
            file_notes.StoreString(json_notes);
        }
    }

    private void LoadGame()
    {
        // 1. Load Lanterns
        if (FileAccess.FileExists(SavePathLanterns))
        {
            using var file_lanterns = FileAccess.Open(SavePathLanterns, FileAccess.ModeFlags.Read);
            if (file_lanterns != null)
            {
                try
                {
                    var loadedLanternIds = JsonSerializer.Deserialize<List<string>>(file_lanterns.GetAsText());
                    if (loadedLanternIds != null) _collectedLanterns = loadedLanternIds;
                }
                catch { GD.PrintErr("Failed to deserialize lanterns save file."); }
            }
        }

        // 2. Load Notes
        if (FileAccess.FileExists(SavePathNotes))
        {
            using var file_notes = FileAccess.Open(SavePathNotes, FileAccess.ModeFlags.Read);
            if (file_notes != null)
            {
                try
                {
                    var loadedNoteIds = JsonSerializer.Deserialize<List<string>>(file_notes.GetAsText());
                    if (loadedNoteIds != null) _collectedNotes = loadedNoteIds;
                }
                catch { GD.PrintErr("Failed to deserialize notes save file."); }
            }
        }
    }
}
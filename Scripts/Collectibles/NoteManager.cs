using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

[GlobalClass]
public partial class NoteManager : Node
{
    public static NoteManager Instance { get; private set; }

    // This public list can be accessed by your Main Menu later!
    public List<string> UnlockedNoteIds { get; private set; } = new List<string>();

    // Saving directly to the root user folder, just like the lanterns
    private const string SavePath = "user://unlocked_notes.json";
    private Node3D NoteNode;

    public override void _Ready()
    {
        Instance = this;
        LoadUnlockedNotes();
    }

    public void RecordNoteAsRead(string noteId)
    {
        // Ignore empty IDs or notes we already unlocked
        if (string.IsNullOrEmpty(noteId) || UnlockedNoteIds.Contains(noteId)) return;

        UnlockedNoteIds.Add(noteId);
        SaveUnlockedNotes();
    }

    public bool IsNoteUnlocked(string noteId)
    {
        return UnlockedNoteIds.Contains(noteId);
    }

    public void SetNode(Node3D node)
    {
        NoteNode = node;
    }

    public string LoadCollectedNotes()
    {
        if (!FileAccess.FileExists(SavePath)) return null;

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        string json = JsonSerializer.Serialize(UnlockedNoteIds);
        return json;
    }

    // --- SAVE AND LOAD LOGIC ---
    private void SaveUnlockedNotes()
    {
        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        if (file == null) 
        {
            GD.PrintErr("Failed to open note save file for writing.");
            return;
        }

        string json = JsonSerializer.Serialize(UnlockedNoteIds);
        file.StoreString(json);
    }

    private void LoadUnlockedNotes()
    {
        if (!FileAccess.FileExists(SavePath)) return;

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        if (file == null) return;

        try
        {
            var loadedNotes = JsonSerializer.Deserialize<List<string>>(file.GetAsText());
            if (loadedNotes != null)
            {
                UnlockedNoteIds = loadedNotes;
            }
        }
        catch 
        {
            GD.PrintErr("Failed to load unlocked notes state.");
        }
    }
}
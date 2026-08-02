using Godot;
using System;
using System.Dynamic;

public partial class CollectibleDisplay : Button
{
	private string Notejson;
	private string NoteName;
	private string NoteText;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//SetButtonText("OK");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public string GetJSON()
	{
		return Notejson;
	}
	
	public string GetNoteName()
	{
		return NoteName;
	}
	public string GetNoteText()
	{
		return NoteText;
	}
	public void SetButtonText(string set)
	{
		this.Text = set;
	}
	public void SetJsonFile(string set)
	{
		Notejson = set;
	}
}

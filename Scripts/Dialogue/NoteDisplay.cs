using Godot;
using System;

public partial class NoteDisplay : PanelContainer
{
	[Export]Label nameLabel;
	[Export]RichTextLabel dialogueLabel;
	[Export]AudioStream audio;
	[Export] Label ReadNotification;
	public override void _Ready()
	{
		Hide();
		ReadNotification.Visible=false;
	}

	public override void _Process(double delta)
	{
	}

	public void ShowNoteScreen(string setname,string setdialogue,bool setNotification)
	{
		Show();
		SetLabel(setname,setdialogue,setNotification);
	}

	public void HideNoteScreen()
	{
		
		AudioStreamPlayer sfxplayer = new AudioStreamPlayer();
        sfxplayer.Stream = audio;
        
        AddChild(sfxplayer);
        sfxplayer.Finished += sfxplayer.QueueFree;  
        sfxplayer.Play();
        
        SetLabel("", "",false);
        Hide();
	}

	public void SetLabel(string setname,string setdialogue,bool setNotification)
	{
		if(nameLabel != null && dialogueLabel!=null)
		{
			nameLabel.Text = setname;
			dialogueLabel.Text = setdialogue;
			ReadNotification.Visible = setNotification;
		}
		return ;
		
	}
}

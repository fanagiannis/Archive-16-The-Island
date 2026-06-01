using Godot;
using System;

public partial class NoteDisplay : PanelContainer
{
	[Export]Label nameLabel;
	[Export]RichTextLabel dialogueLabel;
	public override void _Ready()
	{
		Hide();
	}

	public override void _Process(double delta)
	{
	}

	public void ShowNoteScreen(string setname,string setdialogue)
	{
		Show();
		SetLabel(setname,setdialogue);
	}

	public void HideNoteScreen()
	{
		SetLabel("","");
		Hide();
	}

	public void SetLabel(string setname,string setdialogue)
	{
		if(nameLabel != null && dialogueLabel!=null)
		{
			nameLabel.Text = setname;
			dialogueLabel.Text = setdialogue;
		}
		return ;
		
	}
}

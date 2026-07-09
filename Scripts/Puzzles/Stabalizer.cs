using Godot;
using System;

public partial class Stabalizer : Node
{
	[Export]Interactable Increaser;
	[Export]Interactable Decreaser;
	private float Stabalizing_Value=2f;
	private Label3D label;
	
	public override void _Ready()
	{
		Increaser.HasInteracted += IncreaseValue;
		Decreaser.HasInteracted +=DecreaseValue;

		label = GetNode<Label3D>("ValueLabel");
		label.Text= Stabalizing_Value.ToString();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void IncreaseValue()
	{
		Stabalizing_Value +=1f;
		label.Text= Stabalizing_Value.ToString();
		Puzzle.Instance.UpdateValue();
	}

	public void DecreaseValue()
	{
		Stabalizing_Value -=1f;
		Stabalizing_Value = Mathf.Max(0,Stabalizing_Value);
		label.Text= Stabalizing_Value.ToString();
		Puzzle.Instance.UpdateValue();
	}

	public float GetValue()
	{
		return Stabalizing_Value;
	}
}

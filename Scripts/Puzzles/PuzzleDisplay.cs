using Godot;
using System;

public partial class PuzzleDisplay : Node
{
	float PuzzleGoalValue=0;
	float PuzzleStabalizerAValue=0;
	float PuzzleStabalizerBValue=0;
	[Export] Label3D PuzzleGoalLabel;
	[Export] Label3D PuzzleStabalizerALabel;
	[Export] Label3D PuzzleStabalizerBLabel;
	
	public override void _Ready()
	{
		UpdateLabels();
		Puzzle.Instance.UpdatePuzzle+=UpdateLabels;
	}

	public override void _Process(double delta)
	{
	}

	public void SetLabel(Label3D label, string text)
	{
		label.Text = text;
	}

	public void UpdateLabels()
	{
		PuzzleGoalValue = Puzzle.Instance.GetTargetValue();
		PuzzleStabalizerAValue = Puzzle.Instance.GetStabalizerA().GetValue();
		PuzzleStabalizerBValue = Puzzle.Instance.GetStabalizerB().GetValue();
		SetLabel(PuzzleGoalLabel,PuzzleGoalValue.ToString());
		SetLabel(PuzzleStabalizerALabel,PuzzleStabalizerAValue.ToString());
		SetLabel(PuzzleStabalizerBLabel,PuzzleStabalizerBValue.ToString());
	}
}

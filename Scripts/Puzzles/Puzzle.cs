using Godot;
using System;

[GlobalClass]
public partial class Puzzle : Interactable
{
	public static Puzzle Instance {get; private set;}
	[Signal]
	public delegate void UpdatePuzzleEventHandler();
	[Export]string PuzzleName;
	[Export]float TargetValue=100f;
	private float CurrentValue=0.5f;
	private bool _valueSet=false;
	Label3D Label_TargetValue;
	[Export]private PuzzleInteractable puzzleSolveInteraction;
	[Export]private Stabalizer stabalizerA;
	[Export]private Stabalizer stabalizerB;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance=this;
		Label_TargetValue = GetNode<Label3D>("TargetLabel");
		Label_TargetValue.Text = CurrentValue.ToString();
		RandomizeTargetValue();
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void CheckValue()
	{
		if(Mathf.IsEqualApprox(CurrentValue, TargetValue))
		{
			_valueSet=true;
			puzzleSolveInteraction.SetReact();
			GD.Print("Puzzle Solved!");
		}
	}

	public void RandomizeTargetValue()
	{
		float randomValue = (float)GD.RandRange(50.0, 150.0);
		TargetValue = Mathf.Round(randomValue);
		GD.Print(TargetValue);
	}

	public void UpdateValue()
    {
		if(_valueSet==false)
		{
			float valA = stabalizerA.GetValue();
			float valB = stabalizerB.GetValue();
			float interference = Mathf.Abs(valA - valB) * 0.25f;
			CurrentValue = (valA * 0.5f) + (valB * 1.5f) - interference;  
			CurrentValue = Mathf.Max(0,CurrentValue); 
			Label_TargetValue.Text = CurrentValue.ToString();
			CheckValue();
		}
		else
		{
			CurrentValue = TargetValue;
			Label_TargetValue.Text = CurrentValue.ToString();
		}
		EmitSignal(SignalName.UpdatePuzzle);
        
	}

	public float GetTargetValue()
	{
		return TargetValue;
	}

	public Stabalizer GetStabalizerA()
	{
		return stabalizerA;
	}

	public Stabalizer GetStabalizerB()
	{
		return stabalizerB;
	}
}

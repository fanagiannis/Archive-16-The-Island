using Godot;
using System;

public partial class AnimatedEvent : Node
{
	[Export]protected string ID;
	protected Godot.AnimationPlayer animation;
	protected bool executed=false;
	
	public override void _Ready()
	{
		animation = GetNode<Godot.AnimationPlayer>("Animation");
	}
//test
	public virtual void Execute()
	{
		if(executed==false)
		{
			GD.Print($"Event {ID} executed");
			executed = true;
		}
	}
}

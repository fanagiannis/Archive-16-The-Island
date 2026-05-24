using Godot;
using System;

public partial class BigDoor : AnimatedEvent
{
	
	public override void _Ready()
	{
		base._Ready();
	}

    public override void Execute()
    {
		if(animation!=null)
		{
			if(executed==false)
			{
				animation.Play("Door");
				executed = true;
			}
			else
			{
				animation.PlayBackwards("Door");
				executed=false;
			}
		}
		
    }
}

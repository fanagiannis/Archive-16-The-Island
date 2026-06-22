using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyManager : Node
{
	private List<Enemy> enemies = new List<Enemy>();
	Enemy_Abomination Ref_Abomination ;
	
	[Export]private float disabledTime = 3f;
    [Export]private float disabledTimer = 0;
    private bool DisabledTimerEnabled=false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		EnableAllEnemies();
		foreach (Enemy enemy in enemies)
		{
			GD.Print(enemy);
			
		}
		GD.Print(enemies);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		EnablerTimer(delta);
	}

	public void StartEnabler()
    {
        DisabledTimerEnabled=true;
       // GD.Print(DisabledTimerEnabled);
    }
    public void EnablerTimer(double delta)
    {  
        if(DisabledTimerEnabled && Ref_Abomination!=null)
        {
            disabledTimer+=1f*(float)delta;
            
            if(disabledTimer>=disabledTime)
            {
				Ref_Abomination.ProcessMode =ProcessModeEnum.Inherit;
				Ref_Abomination.ResetDamage();
                Ref_Abomination.SetEnabled(true);
                disabledTimer=0;
                DisabledTimerEnabled=false;
            }
        }
    }

	public void AddEnemy(Enemy enemy)
	{
		enemies.Add(enemy);
		if(enemy is Enemy_Abomination)
		{
			Ref_Abomination = enemy as Enemy_Abomination;
			Ref_Abomination.Disabled+=StartEnabler;
			GD.Print(Ref_Abomination);
		}
		//GD.Print("Added Enemy");
	}

	public void SetEnemyDifficltyIndex(int value)
	{
		foreach(Enemy enemy in enemies)
		{
			enemy.SetDifficulty(value);
		}
	}

	public void DisableAllEnemies()
	{
		foreach(Enemy enemy in enemies)
		{
			enemy.SetEnabled(false);
		}
	}

	public void EnableAllEnemies()
	{
		foreach(Enemy enemy in enemies)
		{
			enemy.SetEnabled(true);
		}
	}

}

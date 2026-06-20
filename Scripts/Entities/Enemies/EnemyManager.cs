using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyManager : Node
{
	private List<Enemy> enemies = new List<Enemy>();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DisableAllEnemies();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void AddEnemy(Enemy enemy)
	{
		enemies.Add(enemy);
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

}

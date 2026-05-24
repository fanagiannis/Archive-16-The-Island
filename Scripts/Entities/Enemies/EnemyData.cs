using Godot;
using System;
using System.Collections;

[GlobalClass]
public partial class EnemyData : Resource
{
	[Export] string Name;
	[Export] float maxHP;
	[Export] float Damage=0;
	[Export]public float walk_speed;
    [Export]public float sprint_speed;

	public float GetMaxHP()
	{
		return maxHP;
	}

	public string GetEntityName()
	{
		return Name;
	}

	public float GetDamage()
	{
		return Damage;
	}
	
}

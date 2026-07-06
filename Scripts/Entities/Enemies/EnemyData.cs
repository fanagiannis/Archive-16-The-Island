using Godot;
using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

[GlobalClass]
public partial class EnemyData : Resource
{
	[Export] int ID;
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

	public int GetID()
	{
		return ID;
	}
	
}

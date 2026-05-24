using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Level : Node3D
{
	EnemyManager enemyManager;
	[Export] string LevelName;
	[Export] Node3D playerSpawn;
	private List<ExitDoor> exitDoors = new List<ExitDoor>();
	//AREAS
	private List<Area> levelAreas = new List<Area>();
	private Area lastArea;
	//NODES
	Node3D EnemyNode;
	Node3D DoorNode;
	Node3D AreasNode;
	int difficulty = 0;
	
	public override void _Ready()
	{
		EnemyNode = GetNode<Node3D>("Enemies");
		DoorNode = GetNode<Node3D>("Doors");
		enemyManager = GetNode<EnemyManager>("EnemyManager");
		AreasNode = GetNode<Node3D>("NavigationRegion3D/Forest/Areas");
		if (DoorNode != null) GetDoorsList();
    	if (EnemyNode != null  && enemyManager!=null) GetEnemiesList();
		if (AreasNode !=null) GetAreasList();

		enemyManager.SetEnemyDifficltyIndex(difficulty);
		//GD.Print(enemies.Count);
		//GD.Print(exitDoors.Count);
		enemyManager.DisableAllEnemies();
		
	}

	public override void _Process(double delta)
	{
		//GD.Print(lastArea);
	}

	public void SetDifficulty(int value)
	{
		difficulty = value;
	}

	public void SetLastArea(Area area)
	{
		foreach(Area instance in levelAreas)
		{
			instance.ResetArea();
		}

		foreach(Area instance in levelAreas)
		{
			if(instance.GetAreaName()==area.GetAreaName())
			{
				instance.SetActive(true);
				lastArea=instance;
				break;
			}
		}
		
		
	}

	private void GetEnemiesList()
	{
		foreach (Node child in EnemyNode.GetChildren())
        {
            if (child is Enemy enemy)
            {
                enemyManager.AddEnemy(enemy);
            }
        }
	}

	private void GetAreasList()
	{
		foreach (Node child in AreasNode.GetChildren())
        {
            if (child is Area area)
            {
                levelAreas.Add(area);
            }
        }
	}

	private void GetDoorsList()
	{
		foreach (Node child in DoorNode.GetChildren())
        {
            if (child is ExitDoor door)
            {
                exitDoors.Add(door);
            }
        }
	}

	public string GetLevelName()
	{
		return LevelName;
	}

	public Node3D GetPlayerSpawn()
	{
		return playerSpawn;
	}

	public ExitDoor GetEntranceDoor(string Tag)
	{
		foreach(ExitDoor door in exitDoors)
		{
			if(door.GetTag()==Tag)
			{
				return door;
			}
			
		}
		return null;
	}

}

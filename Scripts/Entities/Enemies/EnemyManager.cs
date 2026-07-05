using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyManager : Node
{
    private List<Enemy> enemies = new List<Enemy>();
    Enemy_Abomination Ref_Abomination;
    
    [Export] private float disabledTime = 180f;
    [Export] private float disabledTimer = 0;
    private bool DisabledTimerEnabled = false;

    public override void _Ready()
    {
        DisableAllEnemies();
    }

    public override void _Process(double delta)
    {
        EnablerTimer(delta);
    }

    public void StartEnabler()
    {
        DisabledTimerEnabled = true;
    }

    public void EnablerTimer(double delta)
    {  
        if (DisabledTimerEnabled && Ref_Abomination != null)
        {
            disabledTimer += (float)delta;
            
            if (disabledTimer >= disabledTime)
            {
                // 1) RE-ENABLE VISIBILITY
                Ref_Abomination.Visible = true;

                // 2) SPAWN ABOMINATION NEAR PLAYER
                Vector3 playerPos = SceneManager.Instance.GetPlayerPosition();
                
                // Pick a random angle and a random distance between 15 and 25 meters away
                float angle = (float)GD.RandRange(0, Mathf.Tau);
                float distance = (float)GD.RandRange(15.0f, 25.0f);
                
                // Convert angle & distance into a 3D coordinate offset (flat plane)
                Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
                
                Vector3 spawnPosition = playerPos + offset;
                spawnPosition.Y = playerPos.Y; // Keep them on the same floor level

                Ref_Abomination.GlobalPosition = spawnPosition;

                // 3) RESTORE PROCESSING AND RE-ENABLE
                Ref_Abomination.ProcessMode = ProcessModeEnum.Inherit;
                Ref_Abomination.ResetDamage();
                Ref_Abomination.SetEnabled(true);
                
                disabledTimer = 0;
                DisabledTimerEnabled = false;
            }
        }
    }

    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
        
        if (enemy is Enemy_Abomination)
        {
            Ref_Abomination = enemy as Enemy_Abomination;
            Ref_Abomination.Disabled += StartEnabler;
            GD.Print("Abomination Added and Signal Connected.");
        }
    }

    public void SetEnemyDifficltyIndex(int value)
    {
        foreach (Enemy enemy in enemies)
        {
            enemy.SetDifficulty(value);
        }
    }

    public void DisableAllEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            enemy.SetEnabled(false);
        }
    }

    public void EnableAllEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            if(enemy.GetEnabled()==false)
                enemy.SetEnabled(true);
        }
    }
}
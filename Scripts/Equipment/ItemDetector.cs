using Godot;
using System;

public partial class ItemDetector : Equipment
{
    [Export] MeterLED detectorLED;
    [Export] AudioStream audioEffect;
    AudioStreamPlayer3D audioPlayer;
	[ExportCategory("Detection Settings")]
    [Export] float detectionRange = 10f; 
    
    // Define the distance thresholds for your 3 levels
    [Export] float closeThreshold = 3f;  // Distance for Level 3
    [Export] float mediumThreshold = 6f; // Distance for Level 2
	[Export]float ScanTimer = 10f;
	float ScanTime=1;

    // Optional: Keep this if you want to visually scale a sphere shape in the editor
    CollisionShape3D detectionShape;

    public override void _Ready()
    {
        audioPlayer = new AudioStreamPlayer3D();
        audioPlayer.Stream = audioEffect;
        AddChild(audioPlayer);
		ScanTime = ScanTimer;
        if (HasNode("DetectionRange"))
        {
            detectionShape = GetNode<CollisionShape3D>("DetectionRange");

            if (detectionShape.Shape is SphereShape3D sphere)
            {
                sphere.Radius = detectionRange;
            }
        }
    }

    public override void _Process(double delta)
    {
		if(EquipmentManager.Instance.GetEquippedWeapon() is ItemDetector && EquipmentManager.Instance.GetEquipped())
		{
            if (audioPlayer.Stream is AudioStreamMP3 mp3Stream && !mp3Stream.Loop)
            {
                mp3Stream.Loop = true;
            }
            if(!audioPlayer.Playing)
                audioPlayer.Play();
			if(ScanTimer <=0)
			{
				Scan();
				ScanTimer = ScanTime;
				return;
			}
			ScanTimer -= 1 * (float) delta;
		}
        else
            audioPlayer.Stop();
		
    }

    public void Scan()
    {
        int signalLevel = GetStrongestDetectionLevel();
        
        switch (signalLevel)
        {
            case 3:
                GD.Print("BEEP BEEP BEEP! Corpse is TOO CLOSE!");
                break;
            case 2:
                GD.Print("BEEP BEEP! Corpse is at medium distance.");
                break;
            case 1:
                GD.Print("BEEP! Corpse detected far away.");
                break;
            case 0:
                GD.Print("Silence... No corpses in range.");
                break;
        }
        if(signalLevel>0)
        {
            detectorLED.Blink(signalLevel);
        }
        else
            detectorLED.SetOn(false);
    }
    public int GetStrongestDetectionLevel()
    {
        Godot.Collections.Array<Node> corpses = GetTree().GetNodesInGroup("Corpse");
        int highestLevel = 0; 

        foreach (Node node in corpses)
        {
            if (node is Node3D corpse3D)
            {
				Corpse corpse = corpse3D as Corpse;
                float distance = GlobalPosition.DistanceTo(corpse3D.GlobalPosition);

                if (distance > detectionRange) continue;

                int currentLevel = CalculateLevel(distance);

                if (currentLevel > highestLevel && corpse.GetInteracted()==false)
                {
                    highestLevel = currentLevel;
                }

                if (highestLevel == 3) break; 
            }
        }

        return highestLevel;
    }

   
    private int CalculateLevel(float distance)
    {
        if (distance <= closeThreshold) return 3;       // Too close
        if (distance <= mediumThreshold) return 2;      // Medium distance
        return 1;                                       // Far distance (but within detectionRange)
    }

    public void ResetEquipment()
    {
        
    }

}

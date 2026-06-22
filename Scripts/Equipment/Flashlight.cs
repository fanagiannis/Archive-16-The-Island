using Godot;
using System;
using System.Linq.Expressions;

public partial class Flashlight : Equipment
{
	[Export] SpotLight3D spotLight;
	[Export] Label3D LifeDisplay;
	private bool FlashlightActive = false;
	private bool hasBattery = true;
	private float BatteryLife=100;

	#region LIGHTDAMAGE
	[Export] public float LightConeAngle = 30.0f;
    [Export] public float DamagePerSecond = 20.0f;

    private Area3D _detectionArea;
    private RayCast3D _lineOfSight;
	private Enemy_Abomination enemy;
	#endregion

    public override void _Ready()
    {
        spotLight.Visible=false;
		LifeDisplay.Text = (BatteryLife/100).ToString("P0");

		_detectionArea = new Area3D();
		_detectionArea.Scale = new Vector3(0.3f,0.3f,0.3f);
        AddChild(_detectionArea);

        CollisionShape3D shapeNode = new CollisionShape3D();
        SphereShape3D sphere = new SphereShape3D();
        
        sphere.Radius = spotLight.SpotRange/4; 
        shapeNode.Shape = sphere;
        
        _detectionArea.AddChild(shapeNode);

		_detectionArea.BodyEntered +=EnemyInRange;
		_detectionArea.BodyExited +=EnemyOutOfRange;

        _lineOfSight = new RayCast3D();
        _lineOfSight.Enabled = false; 
        
        AddChild(_lineOfSight);
    }

    public override void _Process(double delta)
    {
        if(FlashlightActive)
			DecreaseBatteryLife((float)delta*1);
		if (!Visible) return;
       // var nearbyBodies = _detectionArea.GetOverlappingBodies();
	   	if(enemy!=null)
		{
			if(FlashlightActive)
				enemy.SetLightDamaged(true);
			else
				enemy.SetLightDamaged(false);
		}
			/*
			foreach (Node3D body in nearbyBodies)
			{
				Enemy_Abomination _enemy = body as Enemy_Abomination;
				if (body.IsInGroup("Enemy"))
				{
					Vector3 forwardVector = -GlobalTransform.Basis.Z;
					Vector3 directionToEnemy = GlobalPosition.DirectionTo(body.GlobalPosition);
					float angleToEnemy = Mathf.RadToDeg(forwardVector.AngleTo(directionToEnemy));

					if (angleToEnemy <= LightConeAngle)
					{
						_lineOfSight.TargetPosition = ToLocal(body.GlobalPosition);
						_lineOfSight.ForceRaycastUpdate();
						if (_lineOfSight.GetCollider() == body)
						{
						
							_enemy.SetLightDamaged(true);
						}
					}
					else
					{
						_enemy.SetLightDamaged(false);
					}
				}
			}*/
	}

		//###DEBUG###
		
		//###DEBUG###

	public void EnemyInRange(Node3D body)
	{
		if(body is Enemy_Abomination)
		{
			enemy = body as Enemy_Abomination;
			GD.Print(body.Name + "ENTERED");
		}
			
		
	}
	public void EnemyOutOfRange(Node3D body)
	{
		if(body is Enemy_Abomination)
		{
			enemy.SetLightDamaged(false);
			GD.Print(body.Name + "EXITED");
			enemy = null;
			
		}
			
	}
    public override void Use()
    {
        base.Use();
		PlayEquipmentSound();
		if(spotLight!=null)
		{
			if(spotLight.Visible)
			{
				spotLight.Visible=false;
				FlashlightActive = false;
			}
			else if (!spotLight.Visible&&hasBattery)
			{
				spotLight.Visible=true;
				FlashlightActive = true;
			}
		}
		else
		{
			GD.Print("No Battery");
			return;
		}
			
    }

	public void ResetEquipment()
	{
		if(spotLight!=null)
		{
			if(spotLight.Visible)
			{
				spotLight.Visible=false;
				FlashlightActive = false;
			}
		}
	}

	public void DecreaseBatteryLife(float value)
	{
		if(FlashlightActive && Visible)
		{
			BatteryLife = Mathf.Max(BatteryLife-value, 0);
			LifeDisplay.Text = (BatteryLife/100).ToString("P0");
		}
		if(BatteryLife<=0)
		{
			spotLight.Visible=false;
			FlashlightActive = false;
		}

			
	}

	public void SetHasBattery(bool set)
	{
		hasBattery = set;
	}

	public bool GetBatteryLife()
	{
		return hasBattery;
	}

	public void ResetBatteryLife() => BatteryLife = 100;
}

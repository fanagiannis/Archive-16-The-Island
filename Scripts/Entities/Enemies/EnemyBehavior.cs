using Godot;
using GroveGames.BehaviourTree.Collections;
using System;

public partial class EnemyBehavior : Node
{
	[Export] protected NavigationAgent3D enemyNavigation;
	[Export] protected Vision enemyVision;
	[Export] protected VisibleOnScreenNotifier3D onScreenNotifier;
	protected CharacterBody3D body;
	protected double failsafeCooldownTimer = 0.0;
	protected const double FailsafeCooldownTime = 2.0;
	
	public override void _Ready()
	{
		if (onScreenNotifier != null)
        {
            onScreenNotifier.ScreenEntered += OnAgentScreenEntered;
            onScreenNotifier.ScreenExited += OnAgentScreenExited;
        }
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		
			
	}
	protected void FailsafeRelocate(CharacterBody3D body,double delta)
    {
		if (failsafeCooldownTimer > 0)
		{
			failsafeCooldownTimer -= delta;
		}
		else
		{
			Vector3 playerPos = SceneManager.Instance.GetPlayerPosition();
        
			// Pick a random angle and a random distance between 15 and 25 meters away
			float angle = (float)GD.RandRange(0, Mathf.Tau);
			float distance = (float)GD.RandRange(15.0f, 25.0f);
			
			Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
			Vector3 rawSpawnPosition = playerPos + offset;
			
			// CRITICAL: Snap the random point to the actual Navigation Mesh!
			// This prevents the enemy from spawning inside walls, under the floor, or in mid-air.
			Vector3 safeSpawnPosition = NavigationServer3D.MapGetClosestPoint(body.GetWorld3D().NavigationMap, rawSpawnPosition);
			
			body.GlobalPosition = safeSpawnPosition;
			
			// Reset their velocity so they don't carry falling momentum into the new spot
			body.Velocity = Vector3.Zero;
			
			// Refresh their path to the player immediately
			enemyNavigation.TargetPosition = playerPos;
			
			GD.Print("Failsafe triggered: Abomination relocated to a safe navmesh point.");
		}
        
    }

	protected virtual void OnAgentScreenEntered()
    {
		
        //SetBlackboardValue("Agent In View", true);
		//GD.Print(enemyBehavior.GetBlackboardValue("Agent In View"));
    }

    // Fired automatically when the agent leaves the camera view
    protected virtual void  OnAgentScreenExited()
    {
		
        //enemyBehavior.SetBlackboardValue("Agent In View", false);
		//GD.Print(enemyBehavior.GetBlackboardValue("Agent In View"));
    }

	public virtual void SetBlackboardValue(string key,Variant value)
	{
		
	}

	public virtual void SetTargetPosition(Vector3 targetPosition)
	{
		enemyNavigation.TargetPosition = targetPosition;
	}

	public virtual Vector3 MoveToTarget(Transform3D _agentTransform,  float _agentspeed)
	{
		
		 // Find the next point in the path
        Vector3 nextPathPosition = enemyNavigation.GetNextPathPosition();
        Vector3 currentAgentPosition = _agentTransform.Origin;
        
        // Calculate velocity
        Vector3 newVelocity = (nextPathPosition - currentAgentPosition).Normalized() * _agentspeed;

		return newVelocity;

	}
	public virtual void DebugAction(string test)
    {
        GD.Print(test);
    }

	public virtual void SetSpeed(float speed)
	{
		
	}

	public virtual void SetUpBehavior(Transform3D agentTransform,float speed)
	{
		//_agentTransform=agentTransform;
		//_agentspeed = speed;
		//enemyNavigation.MaxSpeed= speed;
	}
	
	public virtual NavigationAgent3D GetNavAgent()
	{
		if(enemyNavigation!=null)
			return enemyNavigation;
		else
			return null;
	}

	public virtual bool GetPlayerSpotted()
	{
		return enemyVision.GetPlayerSpotted();
	}
	
}

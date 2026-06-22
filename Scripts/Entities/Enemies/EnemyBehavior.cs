using Godot;
using GroveGames.BehaviourTree.Collections;
using System;

public partial class EnemyBehavior : Node
{
	[Export] NavigationAgent3D enemyNavigation;
	[Export] Vision enemyVision;
	[Export] protected VisibleOnScreenNotifier3D onScreenNotifier;
	
	public override void _Ready()
	{
		if (onScreenNotifier != null)
        {
            onScreenNotifier.ScreenEntered += OnAgentScreenEntered;
            onScreenNotifier.ScreenExited += OnAgentScreenExited;
        }
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	private void OnAgentScreenEntered()
    {
		
        //SetBlackboardValue("Agent In View", true);
		//GD.Print(enemyBehavior.GetBlackboardValue("Agent In View"));
    }

    // Fired automatically when the agent leaves the camera view
    private void OnAgentScreenExited()
    {
		
        //enemyBehavior.SetBlackboardValue("Agent In View", false);
		//GD.Print(enemyBehavior.GetBlackboardValue("Agent In View"));
    }

	public void SetBlackboardValue(string key,Variant value)
	{
		
	}

	public void SetTargetPosition(Vector3 targetPosition)
	{
		enemyNavigation.TargetPosition = targetPosition;
	}

	public Vector3 MoveToTarget(Transform3D _agentTransform,  float _agentspeed)
	{
		
		 // Find the next point in the path
        Vector3 nextPathPosition = enemyNavigation.GetNextPathPosition();
        Vector3 currentAgentPosition = _agentTransform.Origin;
        
        // Calculate velocity
        Vector3 newVelocity = (nextPathPosition - currentAgentPosition).Normalized() * _agentspeed;

		return newVelocity;

	}
	public void DebugAction(string test)
    {
        GD.Print(test);
    }

	public void SetSpeed(float speed)
	{
		
	}

	public void SetUpBehavior(Transform3D agentTransform,float speed)
	{
		//_agentTransform=agentTransform;
		//_agentspeed = speed;
		//enemyNavigation.MaxSpeed= speed;
	}
	
	public NavigationAgent3D GetNavAgent()
	{
		if(enemyNavigation!=null)
			return enemyNavigation;
		else
			return null;
	}

	public bool GetPlayerSpotted()
	{
		return enemyVision.GetPlayerSpotted();
	}
	
}

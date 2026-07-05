using Godot;
using GroveGames.BehaviourTree.Collections;
using PolarBears.PlayerControllerAddon;
using System;

public partial class DarkFigureBehavior : EnemyBehavior
{
	[Export]Enemy enemyBody; 
	[Export] float moveSpeed = 4.0f;

    // A simple State Machine right in the script
    private enum AIState { Idle, Hunting}
    private AIState currentState = AIState.Hunting;
	
	public override void _Ready()
	{
		body = enemyBody as CharacterBody3D;
		if (onScreenNotifier != null)
        {
            onScreenNotifier.ScreenEntered += OnAgentScreenEntered;


            onScreenNotifier.ScreenExited += OnAgentScreenExited;
        }
        enemyNavigation.MaxSpeed = moveSpeed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
        base._PhysicsProcess(delta);
		if (enemyNavigation == null || enemyBody == null) return;

        switch (currentState)
        {
            case AIState.Hunting:
                HuntPlayer(delta);
                break;
            case AIState.Idle:
                IdleBehavior();
                break;
            
        }
	}

	private void HuntPlayer(double delta)
	{
        
        float distanceToPlayer = body.GlobalPosition.DistanceTo(SceneManager.Instance.GetPlayerPosition());
		Vector3 playerPos = SceneManager.Instance.GetPlayerPosition();
		
		// Check if the target is already where the player is to avoid redundant re-calculations
		// Using DistanceSquaredTo is much faster than comparing exact positions
		if (enemyNavigation.TargetPosition.DistanceSquaredTo(playerPos) < 0.1f)
		{
			// Path is already set for this position, just move
            //GD.Print(distanceToPlayer);
			MoveTowardsTarget(distanceToPlayer);
			return;
		}

        

		// Only set the target and trigger a recalculation when the player actually moves
		enemyNavigation.TargetPosition = playerPos;
		MoveTowardsTarget(distanceToPlayer);
	}

	private void IdleBehavior()
    {
        if ( enemyBody == null || enemyNavigation == null) 
		{
			if(enemyBody == null)
				GD.PrintErr("enemyBodyis null!!");
			if(enemyNavigation == null)
				GD.PrintErr("enemyNavigation is null!!");
		}

        // 1. Clear target position by assigning it to our own position (instantly finishes the path)
        enemyNavigation.TargetPosition = enemyBody.GlobalPosition;

        // 2. Force velocity to zero to stop moving instantly
        if (enemyBody is CharacterBody3D charBody)
        {
            charBody.Velocity = Vector3.Zero;
            charBody.MoveAndSlide();
        }

        // 3. Look at the player
        LookAtPlayer();
    }

	private void LookAtPlayer()
    {
        if ( enemyBody == null) return;

        Vector3 targetPos = SceneManager.Instance.GetPlayerPosition();
        // Flatten the Y axis to keep the enemy standing upright while rotating
        targetPos.Y = enemyBody.GlobalPosition.Y;

        // Prevent a look_at crash if player and enemy happen to overlap exactly
        if (enemyBody.GlobalPosition.DistanceSquaredTo(targetPos) > 0.01f)
        {
            enemyBody.LookAt(targetPos, Vector3.Up);
        }
    }

    private void HideFromPlayer()
    {
        // Simple hide logic: just stop moving, or set a random point far away
        // For now, let's just make him stop dead in his tracks when looked at
        enemyNavigation.TargetPosition = enemyBody.GlobalPosition; 
    }

    private void MoveTowardsTarget(float distanceToPlayer)
    {
        if (enemyNavigation.IsNavigationFinished()) return;
        if(distanceToPlayer>=100 ) //|| enemyNavigation.IsTargetReachable()==false)
        {
           //FailsafeRelocate(body,delta);
           enemyNavigation.MaxSpeed = moveSpeed*3f;
        }
        else if(distanceToPlayer<100)
        {
            enemyNavigation.MaxSpeed = moveSpeed;
        }

        Vector3 nextPathPosition = enemyNavigation.GetNextPathPosition();
        Vector3 currentPosition = enemyBody.GlobalPosition;
        
        // Calculate the direction
        Vector3 direction = (nextPathPosition - currentPosition).Normalized();
        
        // If your script is ON the CharacterBody3D, you would just set Velocity = direction * moveSpeed and call MoveAndSlide()
        // Since this script is a child Node, you'll need to apply the movement to the parent body.
        if (enemyBody is CharacterBody3D charBody)
        {
            charBody.Velocity = direction * moveSpeed;
            charBody.MoveAndSlide();
        }
    }

	protected override void OnAgentScreenEntered()
    {
		
		currentState = AIState.Idle;
        //SetBlackboardValue("Agent In View", true);
		//GD.Print(enemyBehavior.GetBlackboardValue("Agent In View"));
    }

    // Fired automatically when the agent leaves the camera view
    protected override void  OnAgentScreenExited()
    {
		currentState = AIState.Hunting;
        //enemyBehavior.SetBlackboardValue("Agent In View", false);
		//GD.Print(enemyBehavior.GetBlackboardValue("Agent In View"));
    }

	public override void SetBlackboardValue(string key,Variant value)
	{
		
	}

	public override void SetTargetPosition(Vector3 targetPosition)
	{
		enemyNavigation.TargetPosition = targetPosition;
	}

	public override Vector3 MoveToTarget(Transform3D _agentTransform,  float _agentspeed)
	{
		
		 // Find the next point in the path
        Vector3 nextPathPosition = enemyNavigation.GetNextPathPosition();
        Vector3 currentAgentPosition = _agentTransform.Origin;
        
        // Calculate velocity
        Vector3 newVelocity = (nextPathPosition - currentAgentPosition).Normalized() * _agentspeed;

		return newVelocity;

	}
	public override void DebugAction(string test)
    {
        GD.Print(test);
    }

	public override void SetSpeed(float speed)
	{
		
	}

	public override void SetUpBehavior(Transform3D agentTransform,float speed)
	{
		//_agentTransform=agentTransform;
		//_agentspeed = speed;
		//enemyNavigation.MaxSpeed= speed;
	}
	
	public override NavigationAgent3D GetNavAgent()
	{
		if(enemyNavigation!=null)
			return enemyNavigation;
		else
			return null;
	}

	public override bool GetPlayerSpotted()
	{
		return enemyVision.GetPlayerSpotted();
	}
	
}

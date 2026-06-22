using Godot;
using GroveGames.BehaviourTree.Collections;
using System;

public partial class EnemyBehavior : Node
{
	[Export] NavigationAgent3D enemyNavigation;
	[Export] Vision enemyVision;
	[Export] public Node blackboard;
	
	//Transform3D _agentTransform;
	//float _agentspeed;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		/*
		if (blackboard != null)
        {
            blackboard.Call("set_value", "Target", GetNode("../Player"));
        }
		*/

		//blackboard.Call("set_value","can_send_message",true);
		//enemyNavigation = GetNode<NavigationAgent3D>("NavigationAgent3D");
		//TEST NAVIGATION
		if (enemyNavigation != null)
        {
            //GD.Print("Navigation OK");
            // TEST: Set a target 10 units away on the X axis
           // enemyNavigation.TargetPosition = new Vector3(2, 1, 0);
        }
		//SetBlackboardValue("AgentSpeed",3f);
		
		//TEST NAVIGATION
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void SetBlackboardValue(string key,Variant value)
	{
		if (blackboard != null)
        {
            blackboard.Call("set_value", key, value);
        }
		else
			GD.Print("BLACKBOARD NULL");
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
        //GD.Print(test);
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

/* BEHAVIOR TREE GUIDE
	-----------------------------------------------------------------------
	# Inside any Action or Condition Leaf:

	func tick(actor: Node, blackboard: Blackboard) -> int:
		# 1. READ a value (with a default fallback if it doesn't exist)
		var speed = blackboard.get_value("move_speed", 200.0)
		
		# 2. WRITE a value
		blackboard.set_value("is_running", true)
		
		# 3. CHECK if a key exists
		if blackboard.has_value("target"):
			print("I have a target!")
			
		return SUCCESS

	-----------------------------------------------------------------------
	extends ConditionLeaf

	func tick(actor: Node, blackboard: Blackboard) -> int:
		# Accessing a C# property from the actor
		if actor.hp <= 20:
			return SUCCESS # Yes, health is low
		return FAILURE # No, health is fine

	-----------------------------------------------------------------------

	extends ActionLeaf

	func tick(actor: Node, blackboard: Blackboard) -> int:
		var target_pos = blackboard.get_value("target_pos")
		
		if target_pos == null:
			return FAILURE

		# Call the C# method directly
		actor.MoveTowards(target_pos)
		
		# Check if we arrived (logic inside C# or here)
		if actor.global_position.distance_to(target_pos) < 1.0:
			return SUCCESS
			
		return RUNNING # Keep running this node until we arrive
	-----------------------------------------------------------------------
*/
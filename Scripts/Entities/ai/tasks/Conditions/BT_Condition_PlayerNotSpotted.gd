extends ConditionLeaf


func tick(actor: Node, blackboard: Blackboard) -> int:
	# 1. Get the value from your C# Behavior Agent
	var agent = actor.GetBehaviorAgent()
	var is_spotted = agent.GetPlayerSpotted()

	# 2. Store it in the blackboard (Optional, but good for other nodes to see)
	blackboard.set_value("PlayerSpotted", is_spotted)

	# 3. Return SUCCESS to trigger the 'Chase' branch, or FAILURE to stay 'Roaming'
	if is_spotted:
		return FAILURE
	
	return SUCCESS

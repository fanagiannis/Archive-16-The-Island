extends ConditionLeaf


func tick(actor: Node, blackboard: Blackboard) -> int:
	var agent = actor.GetBehaviorAgent()
	var is_blinded = actor.GetIsBlinded()

	# 2. Store it in the blackboard (Optional, but good for other nodes to see)
	blackboard.set_value("IsBlinded",is_blinded)

	# 3. Return SUCCESS to trigger the 'Chase' branch, or FAILURE to stay 'Roaming'
	if is_blinded:
		return SUCCESS
	
	return FAILURE

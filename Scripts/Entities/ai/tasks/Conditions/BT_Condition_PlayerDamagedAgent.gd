extends ConditionLeaf


func tick(actor: Node, blackboard: Blackboard) -> int:
	var is_damaged = actor.GetHP()<actor.GetMaxHP()
	if is_damaged:
		return SUCCESS
	
	return FAILURE

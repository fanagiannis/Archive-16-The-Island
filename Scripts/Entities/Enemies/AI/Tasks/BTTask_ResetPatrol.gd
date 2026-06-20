extends ActionLeaf

func _ready():
	pass

func tick(actor: Node, blackboard) -> int:
	blackboard.set_value("TargetPosition",Vector3.ZERO) 
	blackboard.set_value("ReachedPatrolPoint",false)
	return SUCCESS

extends ActionLeaf


func _ready():
	pass

func tick(actor: Node, blackboard) -> int:
	actor.Walk()
	return SUCCESS

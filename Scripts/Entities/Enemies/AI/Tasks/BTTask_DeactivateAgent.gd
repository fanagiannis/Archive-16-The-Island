extends ActionLeaf

func _ready():
	pass

func tick(actor: Node, blackboard) -> int:
	actor.SetEnabled(false)
	return SUCCESS

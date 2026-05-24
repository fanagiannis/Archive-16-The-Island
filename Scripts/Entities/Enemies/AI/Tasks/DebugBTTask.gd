extends ActionLeaf

@export var debugstring:String

var enemy

func tick(actor: Node, blackboard) -> int:
	print(debugstring)

	return SUCCESS

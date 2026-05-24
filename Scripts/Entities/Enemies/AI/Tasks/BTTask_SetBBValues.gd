extends ActionLeaf


var enemy

func _ready():
	pass

func tick(actor: Node, blackboard) -> int:
	var speed = actor.GetCurentSpeed()
	blackboard.set_value("AgentSpeed", speed)
	#print(blackboard.get_value("AgentSpeed"))
	return SUCCESS

#	
#	if blackboard.has_value("TargetPosition"):
#		blackboard.set_value("TargetPosition", Vector3(2,1,0))
#	var newposition = blackboard.get_value("TargetPosition")
#	if(newposition!=actor.global_position):
#		print(blackboard.get_value("TargetPosition") )
#		actor.GetBehaviorAgent().SetTargetPosition(newposition)
#		actor.velocity = actor.GetBehaviorAgent().MoveToTarget()
#		
#	#enemyAI.SetTargetPosition(blackboard.get_value("TargetPosition"))
#
#	else:
#		print("Actor is null")
#
#	return SUCCESS

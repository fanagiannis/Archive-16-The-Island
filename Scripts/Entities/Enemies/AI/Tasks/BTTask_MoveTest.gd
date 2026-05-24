extends ActionLeaf


var enemyAI
var look_target

func _ready():
	pass

func tick(actor: Node, blackboard) -> int:
	# 1. Ensure the blackboard has a starting value if it's missing
	if not blackboard.has_value("TargetPosition"):
		pass
	blackboard.set_value("TargetPosition", Vector3(2, 1, 0))
		
	var new_position = blackboard.get_value("TargetPosition")
	#print(new_position)

		# 2. Safety Check: Make sure the actor has the C# methods we need
	if actor.has_method("GetBehaviorAgent"):
			var agent = actor.GetBehaviorAgent()
			
			# 3. Use distance_to instead of != 
			# Floating point math is almost never EXACTLY equal (e.g., 1.999 != 2.0)
			if actor.global_position.distance_to(new_position) > 0.5:
				agent.SetTargetPosition(new_position)
				look_target = Vector3(new_position.x, actor.global_position.y, new_position.z)
   				#actor.look_at(look_target, Vector3.UP)
				# Apply the velocity calculated by your C# agent
				actor.velocity = agent.MoveToTarget(actor.transform,blackboard.get_value("AgentSpeed"))
				actor.look_at(look_target, Vector3.UP)
				actor.move_and_slide() # Don't forget this if it's a CharacterBody3D!
				
				# We are still moving, so tell the tree to keep running this node
				return RUNNING
			else:
				# we arrived at the destination
				return SUCCESS
				
	print("Error: Actor is missing GetBehaviorAgent method")
	return FAILURE


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

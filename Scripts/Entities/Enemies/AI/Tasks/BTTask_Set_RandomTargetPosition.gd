extends ActionLeaf


var enemyAI
var look_target

func _ready():
	pass

func tick(actor: Node, blackboard) -> int:
	var map = actor.get_world_3d().get_navigation_map()
	var agent_node = actor.GetBehaviorAgent()
	var nav_agent = agent_node.GetNavAgent()

	# 1. Map Sync Check
	if NavigationServer3D.map_get_iteration_id(map) == 0:
		return RUNNING
	
	# 2. Try up to 5 times to find a VALID reachable point
	for i in range(5):
		var random_offset = Vector3(randf_range(-15, 15), 0, randf_range(-15, 15))
		var world_guess = actor.global_position + random_offset
		var safe_point = NavigationServer3D.map_get_closest_point(map, world_guess)

		# TEMPORARILY set the agent's target to check reachability
		nav_agent.target_position = safe_point
		
		# Give the server a tiny moment or check if the path is possible
		if nav_agent.is_target_reachable():
			blackboard.set_value("TargetPosition", safe_point)
			return SUCCESS
	
	# If we failed 5 times, just wait for the next tick to try again
	return RUNNING

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

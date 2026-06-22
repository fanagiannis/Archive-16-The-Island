extends ActionLeaf

var enemyAI
var look_target

func _ready():
	pass

func tick(actor: Node, blackboard) -> int:
	# CRASH FIX: Notice the ", Vector3.ZERO" here! 
	# If the game just started, it defaults to ZERO instead of crashing the engine.
	var current_target = blackboard.get_value("TargetPosition", Vector3.ZERO)
	
	# LOGIC FIX: We only need to check if the target is ZERO. 
	# If it is ZERO, we definitely need a new point!
	if current_target == Vector3.ZERO:
		var random_offset = Vector3(randf_range(-15, 15), 0, randf_range(-15, 15))
		var world_guess = actor.global_position + random_offset
		var map = actor.get_world_3d().navigation_map
		var safe_pos = NavigationServer3D.map_get_closest_point(map, world_guess)
		
		# Save to blackboard
		blackboard.set_value("TargetPosition", safe_pos)
		# Update our local variable so we can use it immediately below!
		current_target = safe_pos 

	# 3. Move to the target
	if actor.has_method("GetBehaviorAgent"):
		var agent = actor.GetBehaviorAgent()
		
		var flat_actor_pos = Vector2(actor.global_position.x, actor.global_position.z)
		var flat_target_pos = Vector2(current_target.x, current_target.z)
		
		if flat_actor_pos.distance_to(flat_target_pos) > 1.0: 
			agent.SetTargetPosition(current_target)
			look_target = Vector3(current_target.x, actor.global_position.y, current_target.z)
			
			actor.velocity = agent.MoveToTarget(actor.transform, blackboard.get_value("AgentSpeed"))
			actor.look_at(look_target, Vector3.UP)
			actor.move_and_slide() 
			
			return RUNNING
		else:
			# WE ARRIVED! 
			actor.velocity = Vector3.ZERO 
			
			# We leave the target exactly as it is, because your Reset Node will handle the cleanup!
			blackboard.set_value("ReachedPatrolPoint", true)
			return SUCCESS
			
	print("Error: Actor is missing GetBehaviorAgent method")
	return FAILURE

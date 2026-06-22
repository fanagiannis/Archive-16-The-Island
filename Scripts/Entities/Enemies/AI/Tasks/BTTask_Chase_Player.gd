extends ActionLeaf


var enemyAI
var look_target
var time_stuck = 0.0
var player_pos

func _ready():
	pass

func tick(actor: Node, blackboard) -> int:
	#player_pos = SceneManager.Instance.GetPlayerPosition()
	
	var target_pos = get_node("/root/SceneManager").GetPlayerPosition()
	blackboard.set_value("PlayerPosition", target_pos)
	#var target_pos = blackboard.get_value("PlayerPosition")
	var speed = blackboard.get_value("AgentSpeed")

	print(speed)
	
	if not actor.has_method("GetBehaviorAgent"):
		return FAILURE
		
	var agent_node = actor.GetBehaviorAgent()
	var nav_agent = agent_node.GetNavAgent()

	# 1. Check for SUCCESS first
	# We combine the agent's check with a hard distance check for reliability
	var dist_to_target = actor.global_position.distance_to(target_pos)
	#if nav_agent.is_navigation_finished() or dist_to_target < 2:
	#	actor.velocity = Vector3.ZERO
	#	return SUCCESS

	# 2. Only update the target if it has moved significantly 
	# (Prevents restarting the path logic every frame)
	if nav_agent.target_position.distance_to(target_pos) > 0.2:
		nav_agent.target_position = target_pos

	# 3. Handle Movement
	var next_path_pos = nav_agent.get_next_path_position()
	var look_target = Vector3(next_path_pos.x, actor.global_position.y, next_path_pos.z)
	
	if actor.global_position.distance_to(look_target) > 0.1:
		actor.look_at(look_target, Vector3.UP)
	
	if actor.velocity.length()<0.1:
		time_stuck += get_physics_process_delta_time()
	else:
		time_stuck=0.0
	
	if time_stuck>10.0:
		return FAILURE

	# Call your C# movement logic
	actor.velocity = agent_node.MoveToTarget(actor.transform, speed)
	actor.move_and_slide()


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

extends ActionLeaf

func _ready():
	pass

func tick(actor: Node, blackboard) -> int:
	var map = actor.get_world_3d().navigation_map
	
	# 1. Map Sync Check
	if NavigationServer3D.map_get_iteration_id(map) == 0:
		return RUNNING
	
	# 2. Pick a random point and snap it to the NavMesh
	var random_offset = Vector3(randf_range(-15, 15), 0, randf_range(-15, 15))
	var world_guess = actor.global_position + random_offset
	
	# This automatically finds the nearest legal walking spot!
	var safe_point = NavigationServer3D.map_get_closest_point(map, world_guess)

	# 3. Save it for your MoveTo Action to use
	blackboard.set_value("TargetPosition", safe_point)
	
	return SUCCESS
extends ActionLeaf

var enemyAI
var look_target

@export var flee_distance: float = 15.0

func _ready():
	pass

func tick(actor: Node, blackboard) -> int:
	# 1. THE LOCK: Check if we are already in the middle of hiding
	if blackboard.get_value("IsHiding", false) == true:
		actor.Run()
		return SUCCESS # Skip the math, keep the existing TargetPosition!

	# 2. Your existing math
	var cur_position = actor.global_position
	var scene_manager = actor.get_node("/root/SceneManager")
	var player_pos = scene_manager.GetPlayerPosition()

	var flee_dir = (cur_position - player_pos).normalized()

	if flee_dir == Vector3.ZERO:
		flee_dir = Vector3(randf_range(-1, 1), 0, randf_range(-1, 1)).normalized()
		
	var random_offset = Vector3(randf_range(-0.5, 0.5), 0, randf_range(-0.5, 0.5))
	flee_dir = (flee_dir + random_offset).normalized()

	var desired_pos = cur_position + (flee_dir * flee_distance)

	var map = actor.get_world_3d().navigation_map
	var safe_pos = NavigationServer3D.map_get_closest_point(map, desired_pos)

	# 3. Save the position AND activate the lock
	blackboard.set_value("TargetPosition", safe_pos)
	blackboard.set_value("IsHiding", true)

	print("New Hide Target Picked: ", cur_position, " : ", safe_pos)

	return SUCCESS

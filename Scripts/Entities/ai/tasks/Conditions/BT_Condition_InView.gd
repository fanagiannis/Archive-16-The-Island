extends ConditionLeaf


func tick(actor: Node, blackboard: Blackboard) -> int:
	var notifier = actor.get_node("OnScreenNotifier")
	var is_visible = notifier.is_on_screen()

	blackboard.set_value("OnSight", is_visible)

	if not is_visible:
		print(is_visible)
		return SUCCESS
	
	return FAILURE
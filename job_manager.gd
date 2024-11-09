extends Node


# Called when the node enters the scene tree for the first time.
#func _ready() -> void:
	#pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta: float) -> void:
	#pass

var job_targets : Array = []

func register_target(target_node : Node2D) -> void:
	job_targets.append(target_node)

func consume_target() -> Node2D:
	if job_targets:
		return job_targets.pop_front()
	
	return null

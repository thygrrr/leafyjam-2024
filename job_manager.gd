extends Node


# Called when the node enters the scene tree for the first time.
#func _ready() -> void:
	#pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta: float) -> void:
	#pass

var mutex : Mutex = Mutex.new()

var job_targets : Array = []
var random := RandomNumberGenerator.new()

func register_target(target_node : Node2D) -> void:
	job_targets.insert(random.randi_range(0, len(job_targets)-1),target_node)

func consume_target() -> Node2D:
	if job_targets:
		mutex.lock()
		var target = job_targets.pop_front()
		mutex.unlock()
		return target

	return null

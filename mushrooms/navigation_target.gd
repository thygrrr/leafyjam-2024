extends Node2D

signal start_growing

func _ready() -> void:
	await get_tree().create_timer(2).timeout
	JobManager.register_target(self)

func work_done() -> void:
	start_growing.emit()

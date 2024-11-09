extends Node

@export var pops_max:int = 3

var pops:int = 0


func _on_animated_sprite_2d_frame_changed() -> void:
	if pops > pops_max:
		return
	pops += 1
	$SoundPlayer/Pop.pitch_scale = randf_range(0.1,2)
	$SoundPlayer/Pop.play()
	pass # Replace with function body.

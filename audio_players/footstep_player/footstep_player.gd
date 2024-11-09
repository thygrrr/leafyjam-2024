class_name FootstepPlayer
extends Node


func start_playing_footsteps():
	$FootstepTimer.start()


func stop_playing_footsteps():
	$FootstepTimer.stop()


func _on_footstep_timer_timeout() -> void:
	play_footstep()


func play_footstep():
	var random_footstep:AudioStreamPlayer2D = $Footsteps.get_children().pick_random()
	random_footstep.play()

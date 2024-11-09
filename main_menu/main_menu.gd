extends Control


@onready var play: TextureButton = %Play


func _on_play_pressed() -> void:
	play.visible = false
	$AnimationPlayer.play("fade_out")


func _on_animation_player_animation_finished(anim_name: StringName) -> void:
	visible = false

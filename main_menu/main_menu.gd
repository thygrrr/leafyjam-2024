extends Control


@onready var play: TextureButton = %Play
@onready var exit: TextureButton = %Exit


func _ready() -> void:
	get_tree().paused = true

func _on_play_pressed() -> void:
	get_tree().paused = false
	play.visible = false
	exit.visible = false
	$SoundPlayer/Pop.play()
	$AnimationPlayer.play("fade_out")

func _on_exit_pressed() -> void:
	get_tree().quit()

func _on_animation_player_animation_finished(_anim_name: StringName) -> void:
	visible = false

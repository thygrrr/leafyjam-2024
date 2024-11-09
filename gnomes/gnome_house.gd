extends Node2D

@onready var sprite : AnimatedSprite2D = $AnimatedSprite2D

@export var house_type : int = 0

func _ready() -> void:
	sprite.set_frame_and_progress(house_type, 1.0)

extends AnimatedSprite2D

static var index : int = 0

func _ready() -> void:
	var anims := sprite_frames.get_animation_names()
	index += 1
	var random = RandomNumberGenerator.new()
	speed_scale = random.randf_range(0.75, 1.5)
	play(anims[index % anims.size()])

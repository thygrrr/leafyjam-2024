extends Node2D

@onready var chill_spots : Node2D = $ChillSpots
@onready var sprite : AnimatedSprite2D = $AnimatedSprite2D

@export var house_type : int = 0

var chill_spot_data : Dictionary = {}

func _ready() -> void:
	for chill_node in chill_spots.get_children():
		chill_spot_data[chill_node] = null
	
	sprite.set_frame_and_progress(house_type, 1.0)

func get_chill_spot(occupant : Node2D) -> Node2D:
	for chill_node in chill_spot_data.keys():
		if chill_spot_data[chill_node] == null:
			chill_spot_data[chill_node] = occupant
			
			return chill_node

	return null

func leave_chill_spot(occupant : Node2D) -> void:
	for chill_node in chill_spot_data.keys():
		if chill_spot_data[chill_node] == occupant:
			chill_spot_data[chill_node] = null
			return

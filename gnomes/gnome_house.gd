extends Node2D

@onready var chill_spots : Node2D = $ChillSpots

var chill_spot_data : Dictionary = {}

func _ready() -> void:
	for chill_node in chill_spots.get_children():
		chill_spot_data[chill_node] = null


func get_chill_spot(occupant : Node2D) -> Node2D:
	print("Get chill spot")
	for chill_node in chill_spot_data.keys():
		if chill_spot_data[chill_node] == null:
			chill_spot_data[chill_node] = occupant
			
			return chill_node

	return null

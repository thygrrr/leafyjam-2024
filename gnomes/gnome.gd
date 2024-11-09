extends CharacterBody2D

@export var movement_speed: float = 350.0
@onready var navigation_agent: NavigationAgent2D = get_node("NavigationAgent2D")

@export var home : Node2D = null
@export var target : Node2D = null


func _ready() -> void:
	navigation_agent.velocity_computed.connect(Callable(_on_velocity_computed))
	navigation_agent.max_speed = movement_speed

	if home == null:
		push_error("Home Node2D not set in gnome.gd")

	if target == null:
		push_error("Target Node2D not set in gnome.gd")
	else:
		set_movement_target(target.position)

func set_movement_target(movement_target: Vector2):
	navigation_agent.set_target_position(movement_target)

func _physics_process(_delta):
	# Do not query when the map has never synchronized and is empty.
	if NavigationServer2D.map_get_iteration_id(navigation_agent.get_navigation_map()) == 0:
		return
	if navigation_agent.is_navigation_finished():
		return

	var next_path_position: Vector2 = navigation_agent.get_next_path_position()
	var new_velocity: Vector2 = global_position.direction_to(next_path_position) * movement_speed

	if navigation_agent.avoidance_enabled:
		navigation_agent.set_velocity(new_velocity)
	else:
		_on_velocity_computed(new_velocity)

func _on_velocity_computed(safe_velocity: Vector2):
	velocity = safe_velocity
	move_and_slide()

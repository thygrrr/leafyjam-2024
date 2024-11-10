extends CharacterBody2D

@export var movement_speed: float = 275.0
@onready var navigation_agent: NavigationAgent2D = get_node("NavigationAgent2D")

@export var home : Node2D = null

var current_target = null

enum GnomeState {IDLE, TRAVELING_HOME, TRAVELING_WORK, WORKING}
var current_state : GnomeState = GnomeState.TRAVELING_HOME

func _ready() -> void:
	navigation_agent.velocity_computed.connect(Callable(_on_velocity_computed))
	navigation_agent.max_speed = movement_speed

	go_home.call_deferred()

func set_movement_target(movement_target: Vector2):
	navigation_agent.set_target_position(movement_target)
	$FootstepPlayer.start_playing_footsteps()

func go_home() -> void:
	navigation_agent.target_desired_distance = 50
	var chill_spots = get_tree().get_nodes_in_group("ChillSpots")
	if not chill_spots:
		current_state = GnomeState.IDLE
		return

	chill_spots.shuffle()
	var choosen_chill_spot : Node2D = chill_spots.front()
	set_movement_target(choosen_chill_spot.global_position)
	current_state = GnomeState.TRAVELING_HOME

func find_job() -> void:
	current_target = JobManager.consume_target()

	if current_target:
		navigation_agent.target_desired_distance = 25

		set_movement_target(current_target.global_position)
		current_state = GnomeState.TRAVELING_WORK
	else:
		go_home.call_deferred()

func _physics_process(_delta):

	if current_state == GnomeState.IDLE:
		$FootstepPlayer.stop_playing_footsteps()
		find_job.call_deferred()

	# Do not query when the map has never synchronized and is empty.
	if NavigationServer2D.map_get_iteration_id(navigation_agent.get_navigation_map()) == 0:
		return
	if navigation_agent.is_navigation_finished():
		return
		#$SoundPlayer/Woosh.play()

	var next_path_position: Vector2 = navigation_agent.get_next_path_position()
	var new_velocity: Vector2 = global_position.direction_to(next_path_position) * movement_speed

	if navigation_agent.avoidance_enabled:
		navigation_agent.set_velocity(new_velocity)
	else:
		_on_velocity_computed(new_velocity)

func _on_velocity_computed(safe_velocity: Vector2):
	velocity = safe_velocity
	move_and_slide()


func _on_navigation_agent_2d_target_reached() -> void:
	$FootstepPlayer.stop_playing_footsteps()

	if current_state == GnomeState.TRAVELING_HOME:
		await get_tree().create_timer(2).timeout
		current_state = GnomeState.IDLE

	elif current_state == GnomeState.TRAVELING_WORK:
		current_state = GnomeState.WORKING
		$SoundPlayer/Pickup1.play()
		await get_tree().create_timer(2).timeout
		if current_target and current_target.has_method("work_done"):
			current_target.work_done()

		find_job.call_deferred()

func _on_navigation_agent_2d_navigation_finished() -> void:
	pass

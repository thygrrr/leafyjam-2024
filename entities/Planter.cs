using Godot;

namespace leafy.entities;

public partial class Planter : Node2D
{
    [Export] 
    private PackedScene[] _plantables = [];
    
    private AnimatedSprite2D _sprite;
    
    private Ecosystem _ecosystem;

    private State _state;
    public override void _Ready()
    {
        base._Ready();
        _ecosystem = GetParent<Ecosystem>();
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    
    private ShroomTraits _traits;
    
    public override void _Input(InputEvent input)
    {
        if (input is InputEventMouseMotion mouseMotion)
        {
            if (_ecosystem.ClosestShroom(mouseMotion.Position, out var shroom, out var closestPoint))
            {
                Position = closestPoint;
                var traits = shroom.traits;
                
                _traits = traits;
                
                _sprite.Animation = traits switch
                {
                    ShroomTraits.Trait1 => "hint_honeymash",
                    ShroomTraits.Trait2 => "hint_toadstool",
                    ShroomTraits.Trait3 => "hint_porcini",
                    _ => "default",
                };
                
                _state = State.Planting;
            }
            else
            {
                Position = mouseMotion.Position;
                _state = State.Idle;
                _traits = default;
                _sprite.Animation = "default";
            }
        }

        switch (_state)
        {
            case State.Planting:
                if (input is InputEventMouseButton { ButtonIndex: MouseButton.Left} button)
                {
                    if (button.IsPressed())
                    {
                        var planted = _plantables[0].Instantiate<Node2D>();
                        planted.Position = Position;
                        GetParent().AddChild(planted);
                    }
                }
                break;

            default:
                break;
        }
    }
    
    public enum State
    {
        Idle,
        Planting,
    }
}
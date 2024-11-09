using System;
using Godot;

namespace leafy.entities;

public partial class Planter : Node2D
{
    private AnimatedSprite2D _sprite;
    
    private Ecosystem _ecosystem;

    private State _state;
    public override void _Ready()
    {
        base._Ready();
        _ecosystem = GetParent<Ecosystem>();
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }
    
    public override void _Input(InputEvent input)
    {
        if (input is InputEventMouseMotion mouseMotion)
        {
            if (_ecosystem.ClosestPair(mouseMotion.Position, out var pair, out var closestPoint))
            {
                Position = closestPoint;
                var traits = pair.first?.traits | pair.second?.traits;
                
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
                _state = State.Idle;
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
                        GD.Print("Planting.");
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
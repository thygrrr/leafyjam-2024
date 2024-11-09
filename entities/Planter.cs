using System;
using System.Diagnostics;
using Godot;

namespace leafy.entities;

public partial class Planter : Control
{
    private AnimatedSprite2D _sprite;
    
    private Ecosystem _ecosystem;
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
            Position = mouseMotion.Position;

            if (_ecosystem.ClosestMaturePair(mouseMotion.Position, out var pair))
            {
                var traits = pair.first.traits | pair.second?.traits;
                
                _sprite.Animation = traits switch
                {
                    ShroomTraits.Trait1 => "hint_honeymash",
                    ShroomTraits.Trait2 => "hint_toadstool",
                    ShroomTraits.Trait3 => "hint_porcini",
                    _ => "default",
                };
            }
            else
            {
                _sprite.Animation = "default";
            }
        }
        
    }
}
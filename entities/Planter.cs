using System;
using Godot;

namespace leafy.entities;

public partial class Planter : Control
{
    private Ecosystem _ecosystem;
    public override void _Ready()
    {
        base._Ready();
        _ecosystem = GetParent<Ecosystem>();
    }
    
    public override void _Input(InputEvent input)
    {
        if (input is not InputEventMouseMotion mouseMotion) return;
        
        if (_ecosystem.ClosestMaturePair(mouseMotion.Position, out var pair))
        {
            var traits = pair.first.traits | pair.second?.traits;
            
            GD.Print($"Traits: {traits}");
        }
    }
}
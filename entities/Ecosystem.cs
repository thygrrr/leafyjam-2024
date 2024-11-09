using fennecs;
using Godot;

namespace leafy.entities;

public partial class Ecosystem : Node
{
    private Stream<Mushroom> _mushrooms;
    public override void _Ready()
    {
        base._Ready();
        _mushrooms = ECS.World.Query<Mushroom>().Stream();
    }
    public override void _Process(double delta)
    {
        var dt = (float) delta;
        
        _mushrooms.For((ref Mushroom shroom) => shroom.Grow(dt));
        
    }
}
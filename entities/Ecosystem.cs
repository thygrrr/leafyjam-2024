using fennecs;
using Godot;

namespace leafy.entities;

public partial class Ecosystem : Node
{
    private Stream<Mushroom> _growing;
    public override void _Ready()
    {
        base._Ready();
        _growing = ECS.World.Query<Mushroom>().Has<Growing>().Stream();
    }
    public override void _Process(double delta)
    {
        var dt = (float) delta;
        
        _growing.For((ref Mushroom shroom) => shroom.Grow(dt));
    }
}
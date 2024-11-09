using fennecs;
using Godot;

namespace leafy.entities;

public partial class Ecosystem : Node
{
    private Stream<Mushroom> _growing;
    private Stream<Mushroom, Position> _maturePositions;
    
    public override void _Ready()
    {
        base._Ready();
        _growing = ECS.World.Query<Mushroom>().Has<Growing>().Stream();
        _maturePositions = ECS.World.Query<Mushroom, Position>().Has<Mature>().Stream();
    }
    
    public override void _Process(double delta)
    {
        var dt = (float) delta;
        
        _growing.For((ref Mushroom shroom) => shroom.Grow(dt));
    }

    public bool ClosestMaturePair(Vector2 point, out (Mushroom first, Mushroom second) pair)
    {
        Mushroom first = null;
        Mushroom second = null;

        var distance = float.PositiveInfinity;
        
        var tooClose = false;
        var tooFar = true;
        
        _maturePositions.For((ref Mushroom shroom, ref Position position) =>
        {
            var d = position.Distance(point);
         
            // Current shroom is too far away
            if (d < shroom.plantRange.Min())
            {
                tooClose = true;
                return;
            }
            
            if (d > shroom.plantRange.Max()) return;
            
            tooFar = false;
            
            // Found something closer than the current pair
            if (d >= distance) return;
            
            second = first;
            first = shroom;
            distance = d;
        });

        pair = (first, second);
        return !tooClose && !tooFar;
    }
}
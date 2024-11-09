using fennecs;
using Godot;

namespace leafy.entities;

public partial class Ecosystem : Node
{
    private Stream<Mushroom> _growing;
    private Stream<Mushroom> _immature;
    
    private Stream<Mushroom, Position> _plantPositions;
    private Stream<Mushroom, Position> _allPositions;
    private Stream<Mushroom> _fullyGrown;

    public override void _Ready()
    {
        base._Ready();
        _growing = ECS.World.Query<Mushroom>().Has<Growing>().Stream();
        _immature = ECS.World.Query<Mushroom>().Has<Growing>().Not<Mature>().Stream();
        _fullyGrown = ECS.World.Query<Mushroom>().Not<Growing>().Has<Mature>().Stream();
        _plantPositions = ECS.World.Query<Mushroom, Position>().Stream();
        _allPositions = ECS.World.Query<Mushroom, Position>().Stream();
    }

    public override void _Process(double delta)
    {
        var dt = (float)delta;

        _growing.For((ref Mushroom shroom) => shroom.Grow(dt));
        _immature.For((ref Mushroom shroom) => shroom.Mature());
        _fullyGrown.For((ref Mushroom shroom) => shroom.Idle(dt));
    }

    public bool ClosestShroom(Vector2 point, out Mushroom shroom, out Vector2 closestPoint)
    {
        Mushroom found = null;

        var distance = float.PositiveInfinity;

        var tooClose = false;

        var closest = point;

        _allPositions.For((ref Mushroom shroom, ref Position position) =>
        {
            var d = position.Distance(point);
            if (d < shroom.plantRange.Min()) tooClose = true;
        });

        if (!tooClose) _plantPositions.For((ref Mushroom shroom, ref Position position) =>
            {
                var d = position.Distance(point);

                if (d > distance) return;
                
                if (shroom.plantRange.Contains(d))
                {
                    closest = point;
                }
                else
                {
                    var dMax = shroom.plantRange.Max();
                    closest = position + dMax * (point - position).Normalized();
                }

                distance = d;
                found = shroom;
            });
        
        closestPoint = closest;
        shroom = found;

        var tooFar = distance > 150f;
        return !tooClose && !tooFar && found != null;
    }
}

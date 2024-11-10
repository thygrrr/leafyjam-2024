using fennecs;
using Godot;

namespace leafy.entities;

public partial class Ecosystem : Node
{
    private Stream<Mushroom> _growing;
    private Stream<Mushroom> _immature;
    
    private Stream<Mushroom, Position> _plantPositions;
    private Stream<Mushroom, Position> _allPositions;
    private Stream<Mushroom, Position> _fullyGrown;
    private Stream<House, Position> _housePositions;
    private Stream<Mushroom, Position> _mature;

    public override void _Ready()
    {
        base._Ready();
        _growing = ECS.World.Query<Mushroom>().Has<Growing>().Stream();
        _immature = ECS.World.Query<Mushroom>().Has<Growing>().Not<Mature>().Stream();
        _mature = ECS.World.Query<Mushroom, Position>().Has<Mature>().Stream();
        _fullyGrown = ECS.World.Query<Mushroom, Position>().Not<Growing>().Has<Mature>().Stream();
        _plantPositions = ECS.World.Query<Mushroom, Position>().Stream();
        _allPositions = ECS.World.Query<Mushroom, Position>().Stream();
        _housePositions = ECS.World.Query<House, Position>().Stream();
    }

    public override void _Process(double delta)
    {
        var dt = (float)delta;

        _growing.For((ref Mushroom shroom) => shroom.Grow(dt));
        _immature.For((ref Mushroom shroom) => shroom.Mature());
        _fullyGrown.For((ref Mushroom shroom) => shroom.Idle(dt));
    }

    public bool ClosestPlantPosition(Vector2 point, out Mushroom shroom, out Vector2 closestPoint)
    {
        Mushroom found = null;

        var distance = float.PositiveInfinity;

        var tooClose = false;

        var closest = point;

        _housePositions.For((ref House house, ref Position position) =>
        {
            var d = position.Distance(point);
            if (d < 75f) tooClose = true;
        });
        
        if (!tooClose) _allPositions.For((ref Mushroom shroom, ref Position position) =>
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
    
    public bool ClosestHarvestable(Vector2 point, out Mushroom mushroom)
    {
        Mushroom found = null;

        var distance = float.PositiveInfinity;

        _fullyGrown.For((ref Mushroom shroom, ref Position position) =>
        {
            var d = position.Distance(point);
            if (d >= distance) return;
            if (d > shroom.pickRange) return;
            found = shroom;
            distance = d;
        });

        mushroom = found;
        return found != null;
    }

    public void GatherTraits(Vector2 point, out ShroomTraits fusion)
    {
        ShroomTraits found = default;

        _mature.For((ref Mushroom mushroom, ref Position position) =>
        {
            var d = position.Distance(point);
            if (d < 100f) found |= mushroom.traits;
        });
        
        fusion = found;
    }
}

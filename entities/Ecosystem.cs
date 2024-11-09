using fennecs;
using Godot;

namespace leafy.entities;

public partial class Ecosystem : Node
{
    private Stream<Mushroom> _growing;
    private Stream<Mushroom, Position> _maturePositions;
    private Stream<Mushroom> _immature;
    private Stream<Mushroom, Position> _allShrooms;

    public override void _Ready()
    {
        base._Ready();
        _growing = ECS.World.Query<Mushroom>().Has<Growing>().Stream();
        _immature = ECS.World.Query<Mushroom>().Has<Growing>().Not<Mature>().Stream();
        _maturePositions = ECS.World.Query<Mushroom, Position>().Has<Mature>().Stream();
        _allShrooms = ECS.World.Query<Mushroom, Position>().Stream();
    }

    public override void _Process(double delta)
    {
        var dt = (float)delta;

        _growing.For((ref Mushroom shroom) => shroom.Grow(dt));
        _immature.For((ref Mushroom shroom) => shroom.Mature());
    }

    public bool ClosestPair(Vector2 point, out (Mushroom first, Mushroom second) pair, out Vector2 closestPoint)
    {
        Mushroom first = null;
        Mushroom second = null;

        var distance = float.PositiveInfinity;

        var tooClose = false;
        var tooFar = false;

        var closest = point;

        _allShrooms.For((ref Mushroom shroom, ref Position position) =>
        {
            var d = position.Distance(point);
            if (d < shroom.plantRange.Min()) tooClose = true;
        });

        if (!tooClose) _maturePositions.For((ref Mushroom shroom, ref Position position) =>
            {
                var d = position.Distance(point);

                if (d >= distance) return;

                if (shroom.plantRange.Contains(d))
                {
                    closest = point;
                }
                else
                {
                    var dMax = shroom.plantRange.Max();
                    closest = position + dMax * (point - position).Normalized();

                    d = (point - closest).Length();
                    if (d >= distance) return;
                }

                distance = d;
                first = shroom;
                second = first;
            });


        closestPoint = closest;
        pair = (first, second);

        tooFar = distance > 30f;
        return !tooClose && !tooFar && first != null;
    }
}

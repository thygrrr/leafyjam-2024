using Godot;
using fennecs;

namespace leafy.entities;

public partial class EntityNode2D : Node2D
{
    private static World World => ECS.World;

    protected Entity Entity;

    public override void _EnterTree()
    {
        base._EnterTree();

        if (Entity) return;

        Entity = World.Spawn();
        Entity.Add(this);
    }

    public override void _Notification(int what)
    {
        switch ((long) what)
        {
            case NotificationPredelete:
                if (Entity) Entity.Despawn();
                break;
        }
    }
}

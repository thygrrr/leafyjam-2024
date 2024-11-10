using Godot;
using fennecs;

namespace leafy.entities;

public partial class EntityNode2D : Node2D
{
    private static World World => ECS.World;

    protected Entity Entity;

    public bool Deleting { get; private set; }

    public override void _EnterTree()
    {
        base._EnterTree();

        if (Entity) return;

        Entity = World.Spawn();
        Entity.Add(this);
    }

    public override void _Notification(int what)
    {
        switch ((long)what)
        {
            case NotificationPredelete:
                if (!Deleting)
                {
                    //GD.Print($"NotificationPredelete for {Entity}");
                    Entity.Despawn();
                }
                Deleting = true;
                break;
        }
    }
}

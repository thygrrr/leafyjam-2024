using fennecs;
using Godot;

namespace leafy.entities;

public partial class ECS : Node
{
    public static readonly World World = new World();
    
    public override void _Ready()
    {
        base._Ready();
        

        // Pick random values
    }
}

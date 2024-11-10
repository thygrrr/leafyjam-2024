namespace leafy.entities;

public partial class House : EntityNode2D
{
    public override void _Ready()
    {
        Entity.Add(this);
        Entity.Add(new Position(Position.X, Position.Y));
    }
}

using System.Numerics;

namespace leafy.entities;

public readonly struct Growing;
public readonly struct Mature;

public readonly struct Position(float x, float y)
{
    public float Distance(Godot.Vector2 point) => Vector2.Distance(new(x, y), new(point.X, point.Y));

    public static implicit operator Godot.Vector2(Position self) => new(self.X, self.Y);
    public static implicit operator Vector2(Position self) => new(self.X, self.Y);

    public float X => x;
    public float Y => y;
}
using Godot;

namespace leafy.entities;

public static class Vector2Extension
{
    public static float Remap(this Vector2 self, float t) => Mathf.Remap(t, 0, 1, self.X, self.Y);
}

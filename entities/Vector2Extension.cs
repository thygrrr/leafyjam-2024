using Godot;

namespace leafy.entities;

public static class Vector2Extension
{
    public static float MapMinMax(this Vector2 self, float t) => Mathf.Remap(t, 0, 1, self.X, self.Y);
    public static Vector2 MapMinMaxV(this Vector2 self, float t) => Vector2.One * Mathf.Remap(t, 0, 1, self.X, self.Y);
}

using Godot;

namespace leafy.entities;

public static class Vector2Extension
{
    public static float Remap(this Vector2 self, float t) => Mathf.Remap(t, 0, 1, self.X, self.Y);
    
    public static float Max(this Vector2 self) => Mathf.Max(self.X, self.Y);
    public static float Min(this Vector2 self) => Mathf.Min(self.X, self.Y);
    
    public static bool Contains(this Vector2 self, float value) => self.Min() <= value && self.Max() >= value;
}

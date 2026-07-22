using Godot;
using twodog;
using Engine = twodog.Engine; // Godot.Engine exists too; same alias 2dog.engine hosts use

internal static class Program
{
    // STAThread: Godot's Windows display server needs an STA main thread.
    [STAThread]
    private static void Main(string[] args)
    {
        // Host is nested inside the Godot project; walk up to project.godot.
        var projectDir = AppContext.BaseDirectory;
        while (!File.Exists(Path.Combine(projectDir, "project.godot")))
            projectDir = Path.GetDirectoryName(projectDir)!;

        using var engine = new Engine("Leafyjam", projectDir, args);
        using var godot = engine.Start();

        var tree = engine.Tree;
        GD.Print((Variant)$"leafyjam on the gdext stack (scene: {tree.CurrentScene?.Name}, " +
                          $"managed {tree.CurrentScene?.GetType().FullName})");

        // Run the game loop until the engine requests quit (window close).
        while (!godot.Iteration())
        {
        }
    }
}

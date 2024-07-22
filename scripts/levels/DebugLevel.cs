using Godot;

namespace CidreDoux.scripts.levels;

/// <summary>
/// Debug class used to test the <see cref="lib.LevelManager" /> class.
/// </summary>
public partial class DebugLevel : Node
{
    /// <summary>
    /// The level that should be loaded.
    /// </summary>
    [Export(PropertyHint.File, "*.tscn")] public string? TargetLevel;

    /// <inheritdoc cref="Node._Input" />
    public override void _Input(InputEvent @event)
    {
        // Invoke the base handler.
        base._Input(@event);

        // Check if the event is a "swap-level".
        if (!@event.IsActionPressed("swap-level", true)) return;

        // Get the level manager instance.
        if (GetTree().CurrentScene is not Main main) return;

        // Load the target level.
        main.LevelRoot.LoadLevel(TargetLevel);
    }
}

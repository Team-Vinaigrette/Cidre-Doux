using CidreDoux.scripts.lib;
using Godot;

namespace CidreDoux.scripts;

/// <summary>
/// Main game manager class that should always be mounted at the root of the game.
/// </summary>
public partial class Main : Node
{
    /// <summary>
    /// The Node in the current scene used as the root for the levels.
    /// </summary>
    [Export] public LevelManager LevelRoot;
}

using System;
using Godot;

namespace CidreDoux.scripts.lib;

/// <summary>
/// Script used to manage the levels in the current scene.
/// Provides an interface to load a new <see cref="PackedScene" /> as the current level.
/// </summary>
public partial class LevelManager : Node
{
    /// <summary>
    /// Loads the given level scene from the provided path.
    /// </summary>
    /// <param name="path">The path to the <see cref="PackedScene"/> that should be loaded.</param>
    /// <param name="clearCurrentLevel">If set, clears the currently loaded level.</param>
    public void LoadLevel(string? path, bool clearCurrentLevel = true)
    {
        // Load the level.
        LoadLevel(path is null ? null : ResourceLoader.Load<PackedScene>(path), clearCurrentLevel);
    }

    /// <summary>
    /// Loads the given <see cref="PackedScene" />.
    /// </summary>
    /// <param name="scene">The <see cref="PackedScene" /> to load.</param>
    /// <param name="clearCurrentLevel">If set, clears the currently loaded level.</param>
    public void LoadLevel(PackedScene? scene, bool clearCurrentLevel = true)
    {
        // Throw an error if the scene is null.
        if (scene is null)
        {
            throw new NullReferenceException("Tried to load a PackedScene, but the provided reference was null");
        }

        // Clear the current level root.
        if (clearCurrentLevel)
        {
            _ClearLevel();
        }

        // Instantiate the new scene.
        AddChild(scene.Instantiate());
    }

    /// <summary>
    /// Clears all loaded nodes from the current level.
    /// </summary>
    private void _ClearLevel()
    {
        // Remove all the children from the current tree.
        foreach (var child in GetChildren())
        {
            RemoveChild(child);
        }
    }
}

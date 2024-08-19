using System.Collections.Generic;
using CidreDoux.scripts.model.tile;
using Godot;

namespace CidreDoux.scripts.model;

/// <summary>
/// Global parameters shared across the namespace.
/// </summary>
public static class ModelParameters
{
    /// <summary>
    /// The default speed for <see cref="package.Package"/>.
    /// </summary>
    public const int DefaultPackageSpeed = 12;

    /// <summary>
    /// Dictionary of all the crossing costs by <see cref="BackgroundType"/> variant.
    /// </summary>
    public static readonly Dictionary<BackgroundType, StringName> BackgroundTypeCrossingCostSettings = new()
    {
        [BackgroundType.Water] = new StringName("game/navigation/crossing_costs/background_type/water"),
        [BackgroundType.Grass] = new StringName("game/navigation/crossing_costs/background_type/grass"),
        [BackgroundType.Forest] = new StringName("game/navigation/crossing_costs/background_type/forest"),
        [BackgroundType.Mountain] = new StringName("game/navigation/crossing_costs/background_type/mountain")
    };
}

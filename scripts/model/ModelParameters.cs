using System.Collections.Generic;
using CidreDoux.scripts.model.building;
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
    public static readonly StringName DefaultPackageSpeedSetting = new("game/navigation/crossing_costs/integer_precision");

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

    private static readonly List<BackgroundType> GroundedBackgrounds = new List<BackgroundType>()
    {
        BackgroundType.Forest,
        BackgroundType.Grass,
        BackgroundType.Mountain
    };

    public static readonly Dictionary<BuildingType, List<BackgroundType>> BuildingTypeValidBackgrounds = new()
    {
        [BuildingType.Base] = GroundedBackgrounds,
        [BuildingType.Farm] = GroundedBackgrounds,
        [BuildingType.Field] = new List<BackgroundType>() { BackgroundType.Grass },
        [BuildingType.Mine] = new List<BackgroundType>() { BackgroundType.Mountain },
        [BuildingType.Harbor] = new List<BackgroundType>() { BackgroundType.Water },
        [BuildingType.Market] = GroundedBackgrounds,
        [BuildingType.Road] = GroundedBackgrounds,
        [BuildingType.Sawmill] = new List<BackgroundType>() { BackgroundType.Forest }
    };
}

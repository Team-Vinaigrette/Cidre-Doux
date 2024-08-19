using System.Collections.Generic;
using CidreDoux.scripts.model.tile;

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
    public static readonly Dictionary<BackgroundType, int> BackgroundTypeCrossingCosts = new()
    {
        [BackgroundType.Water] = -1,
        [BackgroundType.Grass] = 1 * DefaultPackageSpeed,
        [BackgroundType.Forest] = 2 * DefaultPackageSpeed,
        [BackgroundType.Mountain] = 3 * DefaultPackageSpeed
    };
}

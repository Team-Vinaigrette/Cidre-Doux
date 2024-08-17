using Godot.Collections;

namespace CidreDoux.scripts.model;

public static class ModelParameters
{
    public const int DefaultPackageSpeed = 12;

    public static Dictionary<BackgroundType, int> BGTypeCrossingCosts = new Dictionary<BackgroundType, int>()
    {
        [BackgroundType.Water] = -1,
        [BackgroundType.Grass] = 1 * DefaultPackageSpeed,
        [BackgroundType.Forest] = 2 * DefaultPackageSpeed,
        [BackgroundType.Mountain] = 3 * DefaultPackageSpeed
    };
}
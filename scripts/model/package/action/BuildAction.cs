using CidreDoux.scripts.model.building;
using CidreDoux.scripts.model.tile;

namespace CidreDoux.scripts.model.package.action;

/// <summary>
/// <see cref="IPackageAction"/> implementation used to build a <see cref="BuildingType"/> on a <see cref="Tile"/>.
/// </summary>
public class BuildAction: IPackageAction
{
    /// <summary>
    /// The type of building being built.
    /// </summary>
    public readonly BuildingType BuildingType;

    /// <summary>
    /// Class constructor.
    /// </summary>
    /// <param name="buildingType">The type of resource being delivered.</param>
    public BuildAction(BuildingType buildingType)
    {
        BuildingType = buildingType;
    }

    /// <inheritdoc cref="IPackageAction.PerformAction"/>
    public void PerformAction(Tile targetTile)
    {
        // Make the tile build the building.
        targetTile.Build(BuildingType);
    }
}

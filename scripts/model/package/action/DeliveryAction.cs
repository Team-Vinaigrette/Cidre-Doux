using CidreDoux.scripts.model.tile;
using CidreDoux.scripts.resources.parameters.building;

namespace CidreDoux.scripts.model.package.action;

/// <summary>
/// <see cref="IPackageAction"/> implementation used to deliver a <see cref="ResourceType"/> to a <see cref="Tile"/>.
/// </summary>
public class DeliveryAction: IPackageAction
{
    /// <summary>
    /// The type of resource being delivered.
    /// </summary>
    public readonly DeliveryBuildingParameters Parameters;

    public ResourceType ResourceType => Parameters.ProducedResource;

    /// <summary>
    /// Class constructor.
    /// </summary>
    /// <param name="resourceType">The type of resource being delivered.</param>
    public DeliveryAction(DeliveryBuildingParameters parameters)
    {
        Parameters = parameters;
    }

    /// <inheritdoc cref="IPackageAction.PerformAction"/>
    public void PerformAction(Tile targetTile)
    {
        // Make the tile consume the resource.
        targetTile.Consume(Parameters.ProducedResource);
    }
}

using CidreDoux.scripts.model.tile;

namespace CidreDoux.scripts.model.package.action;

/// <summary>
/// <see cref="IPackageAction"/> implementation used to deliver a <see cref="ResourceType"/> to a <see cref="Tile"/>.
/// </summary>
public class DeliveryAction: IPackageAction
{
    /// <summary>
    /// The type of resource being delivered.
    /// </summary>
    public readonly ResourceType ResourceType;

    /// <summary>
    /// Class constructor.
    /// </summary>
    /// <param name="resourceType">The type of resource being delivered.</param>
    public DeliveryAction(ResourceType resourceType)
    {
        ResourceType = resourceType;
    }

    /// <inheritdoc cref="IPackageAction.PerformAction"/>
    public void PerformAction(Tile targetTile)
    {
        // Make the tile consume the resource.
        targetTile.Consume(ResourceType);
    }
}

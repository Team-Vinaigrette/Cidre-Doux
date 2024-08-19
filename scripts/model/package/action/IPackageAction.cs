using CidreDoux.scripts.model.tile;

namespace CidreDoux.scripts.model.package.action;

/// <summary>
/// Interface used by <see cref="Package"/> instances to perform actions when arrived at their destination.
/// </summary>
public interface IPackageAction
{
    /// <summary>
    /// Callback used to perform the action of the package on the specified tile.
    /// </summary>
    /// <param name="targetTile">The tile that the action should be performed on.</param>
    public void PerformAction(Tile targetTile);
}

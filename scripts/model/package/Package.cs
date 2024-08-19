﻿using System.Collections.Generic;
using CidreDoux.scripts.model.package.action;
using CidreDoux.scripts.model.tile;

namespace CidreDoux.scripts.model.package;

/// <summary>
/// Class used to represent a package.
/// Packages are objects used to transmit information between <see cref="Tile"/> of the <see cref="HexMap"/>.
/// </summary>
public class Package
{
    /// <summary>
    /// The type of package being described.
    /// </summary>
    public readonly PackageType Type;

    /// <summary>
    /// <see cref="IPackageAction"/> handler used when the package is delivered to its destination <see cref="Tile"/>.
    /// </summary>
    public readonly IPackageAction ActionHandler;

    /// <summary>
    /// Path that the package will follow.
    /// </summary>
    public readonly Queue<Tile> Path;

    /// <summary>
    /// The movement speed of this package.
    /// </summary>
    public readonly int Speed;

    /// <summary>
    /// Counter used to store the movement amount left over on the current tile from the previous game turn.
    /// </summary>
    public int LeftoverMovement { get; private set; }

    /// <summary>
    /// Class constructor.
    /// </summary>
    /// <param name="type">The type of this package.</param>
    /// <param name="actionHandler">The handler used when the package has reached its destination.</param>
    /// <param name="path">The path that the package should follow.</param>
    public Package(PackageType type, IPackageAction actionHandler, List<Tile> path)
    {
        Type = type;
        Path = new Queue<Tile>(path);
        Speed = ModelParameters.DefaultPackageSpeed;
        ActionHandler = actionHandler;
        LeftoverMovement = 0;
    }

    /// <summary>
    /// Callback used to walk along the <see cref="Path"/> of this package.
    /// </summary>
    /// <returns>An enumerable with all the <see cref="Tile"/> that were traversed.</returns>
    public IEnumerable<Tile> Walk()
    {
        // Store the distance that can still be walked by this package during this game turn.
        var remainingDistance = Speed;

        // Iterate until there is some remaining speed and a path to traverse.
        while (remainingDistance > 0 && Path.Count > 0)
        {
            // Get the crossing cost for the next tile.
            var tile = Path.Peek();
            var crossingCost = tile.ComputeCrossingCost() - LeftoverMovement;

            // If the Tile can be crossed, move over it.
            if(crossingCost <= remainingDistance)
            {
                remainingDistance -= tile.ComputeCrossingCost();
                LeftoverMovement = 0;
                yield return Path.Dequeue();
            }
            else
            {
                // Store the movement left in the package.
                LeftoverMovement += remainingDistance;
                yield break;
            }
        }
    }
}

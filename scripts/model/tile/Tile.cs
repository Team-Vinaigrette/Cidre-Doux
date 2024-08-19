using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CidreDoux.scripts.model.building;
using CidreDoux.scripts.model.package;
using Godot;

namespace CidreDoux.scripts.model.tile;

/// <summary>
/// Model class used to represent a single Tile in the world map.
/// </summary>
public class Tile: ITurnExecutor
{
    /// <summary>
    /// The background type of this tile.
    /// </summary>
    public readonly BackgroundType Background;

    /// <summary>
    /// The location of this tile in the <see cref="ParentMap"/>.
    /// </summary>
    public readonly TileLocation Location;

    /// <summary>
    /// A reference to the <see cref="HexMap"/> containing this instance.
    /// </summary>
    public readonly HexMap ParentMap;

    /// <summary>
    /// The <see cref="Building"/> object currently found on this tile.
    /// </summary>
    [MaybeNull]
    public Building Building { get; private set; }

    /// <summary>
    /// A list of all the possible background type values for this tile.
    /// </summary>
    public static readonly Array BackgroundTypeValues = Enum.GetValues(typeof(BackgroundType));

    /// <summary>
    /// Relative locations for the neighbours of the odd rows in the world map.
    /// </summary>
    private static readonly TileLocation[] OddNeighborsRelativeLocations =
    {
        new(-1, 0),
        new(-1, -1),
        new(0, -1),
        new(1, 0),
        new(0, 1),
        new(-1, 1),
    };

    /// <summary>
    /// Relative locations for the neighbours of the event rows in the world map.
    /// </summary>
    private static readonly TileLocation[] EvenNeighborsRelativeLocations =
    {
        new(-1, 0),
        new(0, -1),
        new(1, -1),
        new(1, 0),
        new(1, 1),
        new(0, 1),
    };

    /// <summary>
    /// Class constructor.
    /// Initializes the properties of the tile instance.
    /// </summary>
    /// <param name="location">The location of the tile in the <see cref="HexMap"/>.</param>
    /// <param name="backgroundType">The <see cref="BackgroundType"/> of this tile.</param>
    /// <param name="parent">A reference to the <see cref="HexMap"/> containing this tile.</param>
    public Tile(TileLocation location, BackgroundType backgroundType, HexMap parent){
        Background = backgroundType;
        Building = null;
        Location = location;
        ParentMap = parent;
    }

    /// <inheritdoc cref="object.ToString"/>
    public override string ToString() {
        return $"Tile {Location}<{Background}, {Building}>";
    }

    /// <summary>
    /// Helper used to render the tile as a string.
    /// Used only for debugging purposes.
    /// </summary>
    public string ToStringMap() {
        return Location.ToString();
    }

    /// <summary>
    /// Returns all neighbours of this tile.
    /// </summary>
    /// <returns>A list of all the tiles neighbouring this tile.</returns>
    public List<Tile> GetNeighbors()
    {
        return ParentMap.GetNeighbors(Location) ?? new List<Tile>();
    }

    /// <summary>
    /// Helper used to check if a given <see cref="Tile"/> is a neighbour of this tile.
    /// </summary>
    /// <param name="tile">The tile to compare against.</param>
    /// <returns>True of the tile is a neighbour.</returns>
    public bool IsNeighbor(Tile tile)
    {
        // Check if both tiles belong to the same map.
        if (ParentMap is null || tile.ParentMap is null || tile.ParentMap != ParentMap)
        {
            GD.Print("Tried to compare tiles that do not belong to the same HexMap");
            return false;
        }

        // Compare the locations.
        var relPos = new TileLocation(Location.Column - tile.Location.Column, Location.Row - tile.Location.Row);
        return Mathf.Abs(Location.Row % 2) == 0 ? OddNeighborsRelativeLocations.Contains(relPos) : EvenNeighborsRelativeLocations.Contains(relPos);
    }

    /// <summary>
    /// Adds a new building to this tile.
    /// </summary>
    /// <param name="buildingType">The type of building to add.</param>
    public void Build(BuildingType buildingType)
    {
        // Check if a building is already present on this tile.
        if (Building is not null)
        {
            GD.Print($"Error: Tile {Location} already has a building: ${Building.Type}");
            return;
        }

        // Add the building.
        GD.Print($"Tile {Location} has a new {buildingType} {nameof(Building)}");
        Building = Building.CreateBuilding(buildingType);
    }

    /// <summary>
    /// Consumes the given <see cref="ResourceType"/>.
    /// </summary>
    /// <param name="resourceType">The type of resource being consumed.</param>
    public void Consume(ResourceType resourceType)
    {
        // Check if a building was set.
        if (Building is null)
        {
            GD.PrintErr($"Tile {Location} cannot consume {resourceType} because it is empty");
            return;
        }

        Building.Consume(resourceType);
    }

    /// <inheritdoc cref="building.Building.AssignPath"/>
    public void AssignPath(IEnumerable<Tile> path)
    {
        Building.AssignPath(path);
    }

    /// <summary>
    /// Computes the crossing cost on this tile.
    /// </summary>
    public int ComputeCrossingCost()
    {
        // Get the base cost from the background type.
        var baseCost = ModelParameters.BackgroundTypeCrossingCosts[Background];

        // Check if there is a building.
        return Building?.ComputeCrossingCost(baseCost) ?? baseCost;
    }

    /// <inheritdoc cref="ITurnExecutor.ExecuteTurn"/>
    public void ExecuteTurn()
    {
        Building.ExecuteTurn();
    }
}

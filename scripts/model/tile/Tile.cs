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
public partial class Tile: GodotObject, ITurnExecutor
{
    /// <summary>
    /// Delegate handler for OnModelUpdate signal
    /// </summary>
    [Signal] public delegate void OnModelUpdateEventHandler();

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

    /// <summary>
    /// Empty constructor required by Godot.
    /// </summary>
    public Tile(): this(new TileLocation(0, 0), BackgroundType.Grass, null)
    {
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
        return ParentMap?.GetNeighbors(Location) ?? new List<Tile>();
    }

    /// <summary>
    /// Helper used to check if a given <see cref="Tile"/> is a neighbour of this tile.
    /// </summary>
    /// <param name="tile">The tile to compare against.</param>
    /// <returns>True of the tile is a neighbour.</returns>
    public bool IsNeighbor(Tile tile)
    {
        // Check if both tiles belong to the same map.
        if (ParentMap is null || tile.ParentMap != ParentMap)
        {
            GD.Print("Tried to compare tiles that do not belong to the same HexMap");
            return false;
        }

        // Compare the locations.
        var relPos = new TileLocation(Location.Column - tile.Location.Column, Location.Row - tile.Location.Row);
        return Mathf.Abs(Location.Row % 2) == 0 ? EvenNeighborsRelativeLocations.Contains(relPos) : OddNeighborsRelativeLocations.Contains(relPos);
    }
    public bool HasBuilding()
    {
        return Building is not null;
    }

    /// <summary>
    /// Adds a new building to this tile.
    /// </summary>
    /// <param name="buildingType">The type of building to add.</param>
    public void Build(BuildingType buildingType)
    {
        // Check if a building is already present on this tile.
        if (HasBuilding())
        {
            GD.Print($"Error: Tile {Location} already has a building: ${Building.Type}");
            return;
        }

        // Add the building.
        GD.Print($"Tile {Location} has a new {buildingType} {nameof(Building)}");
        Building = Building.CreateBuilding(buildingType);
        EmitSignal(Tile.SignalName.OnModelUpdate);
    }

    /// <summary>
    /// Consumes the given <see cref="ResourceType"/>.
    /// </summary>
    /// <param name="resourceType">The type of resource being consumed.</param>
    public void Consume(ResourceType resourceType)
    {
        // Check if a building was set.
        if (!HasBuilding())
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
        EmitSignal(Tile.SignalName.OnModelUpdate);
    }

    /// <summary>
    /// Computes the crossing cost on this tile.
    /// </summary>
    public int ComputeCrossingCost()
    {
        // Get the base cost from the background type.
        var baseCost = ProjectSettings.GetSettingWithOverride(ModelParameters.DefaultPackageSpeedSetting).AsInt32() * ProjectSettings.GetSettingWithOverride(ModelParameters.BackgroundTypeCrossingCostSettings[Background]).AsInt32();

        // Check if there is a building.
        return Building?.ComputeCrossingCost(baseCost) ?? baseCost;
    }

    public Package ProducePackage()
    {
        return Building?.ProducePackage();
    }
    
    /// <inheritdoc cref="ITurnExecutor.EndTurn"/>
    public void EndTurn()
    {
        if (!HasBuilding()) return;
        if (Building.IsDestroyed) return;
        
        Building.EndTurn();

        if (Building.IsDestroyed) EmitSignal(SignalName.OnModelUpdate);
    }

    private List<Tile> ReconstructPath(Dictionary<Tile, Tile> cameFrom)
    {
        var current = this;
        var res = new Stack<Tile>();
        res.Push(current);
        while(cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            res.Push(current);
        }

        return new List<Tile>(res);
    }

    public int ManhattanDist(Tile goal)
    {
        var integerPrecision = ProjectSettings.GetSettingWithOverride(ModelParameters.DefaultPackageSpeedSetting)
            .AsInt32();
        var dx = goal.Location.Column - this.Location.Column;
        var dy = goal.Location.Row - this.Location.Column;

        if (Mathf.Sign(dx) == Mathf.Sign(dy)) return integerPrecision * Mathf.Abs(dx + dy);
        else return integerPrecision * Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
    }

    public List<Tile> AStar(Tile goal)
    {
        if (goal.ParentMap != ParentMap)
        {
            GD.PrintErr("Attempted to call AStar on two tiles from different maps");
            return null;
        }

        int goalcost = goal.ComputeCrossingCost();
        if (goalcost < 0)
        {
            if (!goal.HasBuilding())
            {
                GD.PrintErr("Attempted to call AStar on uncrossable tile");
                return null;
            }

            goalcost = ProjectSettings.GetSettingWithOverride(ModelParameters.DefaultPackageSpeedSetting).AsInt32();
        }

        var openSet = new List<Tile> { this };

        var cameFrom = new Dictionary<Tile, Tile>();

        var gScores = new Dictionary<Tile, int> { { this, 0 } };

        var fScores = new Dictionary<Tile, int>{{this, this.ManhattanDist(goal)}};

        while (openSet.Count > 0)
        {
            openSet.Sort((x, y) => fScores[x].CompareTo(fScores[y]));
            var current = openSet[0];
            if (current == goal) return goal.ReconstructPath(cameFrom);

            openSet.Remove(current);
            foreach (var neighbor in current.GetNeighbors())
            {
                // Cost of entering last tile is 1 turn if the tile has a building and blocks traffic
                var crossingCost = neighbor == goal ? goalcost : neighbor.ComputeCrossingCost();
                if(crossingCost < 0) continue;

                int currentGScore = gScores.ContainsKey(current) ? gScores[current] : int.MaxValue;
                int neighborGScore = gScores.ContainsKey(neighbor) ? gScores[neighbor] : int.MaxValue;
                int tentativeGScore = (currentGScore + crossingCost);
                if (tentativeGScore < neighborGScore)
                {
                    cameFrom[neighbor] = current;
                    gScores[neighbor] = tentativeGScore;
                    fScores[neighbor] = tentativeGScore + neighbor.ManhattanDist(goal);
                    if(!openSet.Contains(neighbor)) openSet.Add(neighbor);
                }
            }
        }

        GD.Print($"AStar found no path from {this} to {goal}");
        return null;
    }
}

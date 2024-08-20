using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CidreDoux.scripts.model.building.crossing_cost;
using CidreDoux.scripts.model.package;
using CidreDoux.scripts.model.tile;
using Godot;
using CrossingCostMultiplier = CidreDoux.scripts.model.building.crossing_cost.CrossingCostMultiplier;

namespace CidreDoux.scripts.model.building;

/// <summary>
/// Class used to describe a single building in the world.
/// </summary>
public class Building : ICrossingCostComputer, ITurnExecutor
{
    /// <summary>
    /// The type of this building.
    /// </summary>
    public readonly BuildingType Type;

    /// <summary>
    /// A flag set when the building is destroyed,
    /// i.e. when all its <see cref="ResourceConsumer"/> are destroyed.
    /// </summary>
    public bool IsDestroyed => Consumers.Any(consumer => consumer.IsDestroyed);

    /// <summary>
    /// The <see cref="PackageProducer"/> object managed by this building.
    /// </summary>
    [MaybeNull]
    public readonly PackageProducer PackageProducer;

    /// <summary>
    /// A list of all the <see cref="ResourceConsumer"/> needed by this building.
    /// </summary>
    public readonly List<ResourceConsumer> Consumers;

    /// <summary>
    /// The <see cref="ICrossingCostComputer"/> instance used to compute the crossing cost over this building.
    /// </summary>
    [MaybeNull] private readonly ICrossingCostComputer _crossingCostComputer;

    /// <summary>
    /// Creates a new <see cref="Building"/> object.
    /// </summary>
    /// <param name="type">The type of building to create.</param>
    /// <returns>The generated <see cref="Building"/> resource.</returns>
    [return: MaybeNull] public static Building CreateBuilding(BuildingType type)
    {
        switch (type)
        {
        case BuildingType.Base:
            return new Building(
                type,
                PackageProducer.CreateBuildProducer(1),
                new[]
                {
                    new ResourceConsumer(ResourceType.Stone, 1, 5),
                    new ResourceConsumer(ResourceType.Gold, 1, 20)
                },
                new CrossingBlocker()
            );
        case BuildingType.Farm:
            return new Building(
                type,
                PackageProducer.CreateResourceProducer(ResourceType.Food, 6),
                new[] { new ResourceConsumer(ResourceType.Wood, 1, 5) }
            );
        case BuildingType.Mine:
            return new Building(
                type,
                PackageProducer.CreateResourceProducer(ResourceType.Stone, 10),
                new[] { new ResourceConsumer(ResourceType.Wood, 1, 5) }
            );
        case BuildingType.Sawmill:
            return new Building(
                type,
                PackageProducer.CreateResourceProducer(ResourceType.Wood, 10),
                new[] { new ResourceConsumer(ResourceType.Stone, 1, 5) },
                new CrossingCostMultiplier(2f)
            );
        case BuildingType.Road:
            return new Building(
                type,
                null,
                Array.Empty<ResourceConsumer>(),
                new CrossingCostMultiplier(0.5f)
            );
        case BuildingType.Field:
            return new Building(
                type,
                PackageProducer.CreateResourceProducer(ResourceType.Wheat, 5),
                new[] { new ResourceConsumer(ResourceType.Wood, 1, 5) }
            );
        case BuildingType.Harbor:
            return new Building(
                type,
                PackageProducer.CreateResourceProducer(ResourceType.Food, 3),
                new[] { new ResourceConsumer(ResourceType.Stone, 1, 3) },
                new CrossingCostMultiplier(2.0f)
            );
        case BuildingType.Market:
            return new Building(
                type,
                PackageProducer.CreateResourceProducer(ResourceType.Gold, 8),
                new[] { new ResourceConsumer(ResourceType.Food, 2, 8) },
                new CrossingCostMultiplier(2.0f)
            );
        default:
            GD.PrintErr($"Generator for {type} building type not yet implemented");
            return null;
        }
    }

    /// <summary>
    /// Class constructor.
    /// </summary>
    /// <param name="type">The type of building being described.</param>
    /// <param name="packageProducer">The producer of this building.</param>
    /// <param name="consumers">The lits of consumers used by this building.</param>
    /// <param name="crossingCostComputer">The crossing cost computer for this building.</param>
    private Building(
        BuildingType type,
        PackageProducer packageProducer,
        IEnumerable<ResourceConsumer> consumers,
        ICrossingCostComputer crossingCostComputer
    )
    {
        Type = type;
        PackageProducer = packageProducer;
        Consumers = new List<ResourceConsumer>(consumers);
        // IsDestroyed = false;
        _crossingCostComputer = crossingCostComputer;
    }

    /// <summary>
    /// Class constructor.
    /// </summary>
    /// <param name="type">The type of building being described.</param>
    /// <param name="packageProducer">The producer of this building.</param>
    /// <param name="consumers">The lits of consumers used by this building.</param>
    private Building(
        BuildingType type,
        PackageProducer packageProducer,
        IEnumerable<ResourceConsumer> consumers
    ) : this(type, packageProducer, consumers, null)
    {
    }

    /// <inheritdoc cref="ICrossingCostComputer.ComputeCrossingCost"/>
    public int ComputeCrossingCost(int baseCost)
    {
        return _crossingCostComputer?.ComputeCrossingCost(ModelParameters.DefaultPackageSpeed) ?? baseCost;
    }

    /// <inheritdoc cref="PackageProducer.ProducePackage"/>
    public Package ProducePackage()
    {
        if (IsDestroyed) return null;
        return PackageProducer?.ProducePackage();
    }

    /// <inheritdoc cref="ResourceConsumer.Consume"/>
    public void Consume(ResourceType resourceType)
    {
        // Try to consume the resource.
        if (!Consumers.Any(consumer => consumer.Consume(resourceType)))
        {
            GD.PrintErr($"No consumer found for {resourceType} in {Type}");
        }
    }

    /// <summary>
    /// Helper used to assign the output path for <see cref="Package"/> generated by this building.
    /// </summary>
    /// <param name="path">The path to assign.</param>
    public void AssignPath(IEnumerable<Tile> path)
    {
        PackageProducer.AssignPath(path);
    }

    /// <inheritdoc cref="ITurnExecutor.EndTurn"/>
    public void EndTurn()
    {
        // Execute the turn for all the consumers.
        foreach (var consumer in Consumers)
        {
            consumer.EndTurn();
        }
        PackageProducer.EndTurn();
    }
}

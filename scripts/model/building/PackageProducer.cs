﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CidreDoux.scripts.model.package;
using CidreDoux.scripts.model.package.action;
using CidreDoux.scripts.model.tile;
using Godot;

namespace CidreDoux.scripts.model.building;

/// <summary>
/// Simple interface used to describe an object that can create <see cref="Package"/> objects.
/// </summary>
public interface IPackageProducer
{
    /// <summary>
    /// Callback invoked to trigger some internal process and, maybe, produce a new <see cref="Package"/> object.
    /// This is expected to be called on <see cref="ITurnExecutor.EndTurn"/>.
    /// </summary>
    /// <returns>The <see cref="Package"/> that was generated if applicable.</returns>
    [return: MaybeNull]
    public Package ProducePackage();
}

/// <summary>
/// Object used as an implementation that can produce <see cref="Package"/> objects.
/// </summary>
public class PackageProducer : ITurnExecutor, IPackageProducer
{
    /// <summary>
    /// The delay, in game turns, between every <see cref="Package"/> generation.
    /// </summary>
    public readonly int Delay;

    /// <summary>
    /// The <see cref="package.PackageType"/> generated by this producer.
    /// </summary>
    public readonly PackageType PackageType;

    /// <summary>
    /// The <see cref="IPackageAction"/> implementation for the packages generated by this producer.
    /// </summary>
    public IPackageAction PackageAction { get; private set; }

    /// <summary>
    /// An internal counter used to store how many turns are left before the next <see cref="Package"/> generation.
    /// </summary>
    public int TurnCounter { get; private set; }

    /// <summary>
    /// The path for the generated <see cref="Package"/> objects.
    /// </summary>
    public List<Tile> Path { get; private set; }

    /// <summary>
    /// Class constructor.
    /// </summary>
    /// <param name="packageType">The type of packages produced by this instance.</param>
    /// <param name="packageAction">The action passed to the <see cref="Package"/> generated by this instance.</param>
    /// <param name="delay">The delay, in game turns, between <see cref="Package"/> generations.</param>
    public PackageProducer(PackageType packageType, IPackageAction packageAction, int delay)
    {
        Delay = delay;
        PackageType = packageType;
        PackageAction = packageAction;
        Path = null;
        TurnCounter = 0;
    }

    /// <summary>
    /// Simple method used to create a new <see cref="DeliveryAction"/> <see cref="Package"/> producer.
    /// </summary>
    /// <param name="resourceType">The type of resource being created.</param>
    /// <param name="delay">The creation delay for the resources of this producer.</param>
    /// <returns>The generated <see cref="PackageProducer"/> instance.</returns>
    public static PackageProducer CreateResourceProducer(ResourceType resourceType, int delay)
    {
        return new PackageProducer(PackageType.Ressource, new DeliveryAction(resourceType), delay);
    }

    /// <summary>
    /// Simple method used to create a new <see cref="BuildAction"/> <see cref="Package"/> producer.
    /// </summary>
    /// <param name="delay">The creation delay for the resources of this producer.</param>
    /// <returns>The generated <see cref="PackageProducer"/> instance.</returns>
    public static PackageProducer CreateBuildProducer(int delay)
    {
        return new PackageProducer(PackageType.Build, null, delay);
    }

    /// <summary>
    /// Simple check used to validate that a <see cref="PackageProducer"/> can produce this turn.
    /// </summary>
    public bool CanProduce()
    {
        return Path is not null && PackageAction is not null;
    }

    /// <inheritdoc cref="IPackageProducer.ProducePackage"/>
    public Package ProducePackage()
    {
        // Check if the package can be produced.
        if (TurnCounter != 0 || !CanProduce())
        {
            return null;
        }

        // Reset the turn counter to the initial delay.
        TurnCounter = Delay;

        // Generate the package.
        var res = new Package(PackageType, PackageAction, Path);
        if (PackageType == PackageType.Build)
        {
            Path.Last().ReserveTile();
            PackageAction = null;
        }

        return res;
    }

    /// <summary>
    /// Assigns the <see cref="Path"/> of this <see cref="PackageProducer"/>.
    /// </summary>
    /// <param name="path">The new path of the <see cref="PackageProducer"/>.</param>
    public void AssignPath(IEnumerable<Tile> path)
    {
        if (path is null) Path = null;
        else Path = new List<Tile>(path);
    }

    /// <inheritdoc cref="ITurnExecutor.EndTurn"/>
    public void EndTurn()
    {
        if (CanProduce() && TurnCounter > 0) TurnCounter--;
    }

    /// <summary>
    /// Assigns a <see cref="BuildAction"/> to this <see cref="PackageProducer"/>.
    /// </summary>
    /// <param name="action">The action to assign to the producer.</param>
    public void AssignBuildAction(BuildAction action)
    {
        // Check if this is a build package producer.
        if (PackageType != PackageType.Build)
        {
            GD.PrintErr($"Attempted to assign a {nameof(BuildAction)} to a {PackageType} producer.");
            return;
        }

        PackageAction = action;
    }
}

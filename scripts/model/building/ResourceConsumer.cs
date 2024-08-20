using System.Collections.Generic;
using System.Linq;
using CidreDoux.scripts.model.package;
using CidreDoux.scripts.resources.parameters.building;

namespace CidreDoux.scripts.model.building;

/// <summary>
/// Simple interface used to describe an object that can consume a <see cref="ResourceType"/>.
/// </summary>
public interface IResourceConsumer
{
    /// <summary>
    /// Method used to consume a given <see cref="ResourceType"/>.
    /// </summary>
    /// <param name="ressource">The type of resource to consume.</param>
    /// <returns>True if the resource was consumed.</returns>
    public bool Consume(ResourceType ressource);
}

/// <summary>
/// Handler used to consume <see cref="ResourceType"/> objects.
/// </summary>
public class ResourceConsumer: ITurnExecutor, IResourceConsumer
{
    public readonly ResourceConsumerParameters Parameters;

    /// <summary>
    /// The type of resource being consumed by this instance.
    /// </summary>
    public ResourceType RequiredResourceType => Parameters.ConsumedResource;

    /// <summary>
    /// The amount of <see cref="ResourceType"/> required for this instance.
    /// </summary>
    public uint RequiredAmount => Parameters.ConsumedAmount;

    /// <summary>
    /// The time after which the resources will be consumed.
    /// </summary>
    public uint ConsumptionDelay => Parameters.ConsumptionDelay;

    /// <summary>
    /// Counter used to keep track of the <see cref="ConsumptionDelay"/> turn by turn.
    /// </summary>
    public uint TurnsLeft { get; private set; }

    /// <summary>
    /// Counter used to count how many resources of the given <see cref="ResourceType"/> have been consumed.
    /// </summary>
    public uint AmountConsumed { get; private set; }

    /// <summary>
    /// Flag set once the consumer was destroyed.
    /// A <see cref="ResourceConsumer"/> is destroyed if the <see cref="TurnsLeft"/> counter reaches zero,
    /// but  the <see cref="AmountConsumed"/> counter did not reach the required <see cref="RequiredAmount"/>.
    /// </summary>
    public bool IsDestroyed { get; private set; }

    /// <summary>
    /// Property used to check if the <see cref="AmountConsumed"/>
    /// is greater than or equal to the <see cref="RequiredAmount"/>.
    /// </summary>
    public bool IsSatisfied => AmountConsumed >= RequiredAmount;

    public ResourceConsumer(ResourceConsumerParameters parameters)
    {
        Parameters = parameters;
        IsDestroyed = false;

        // Make sure that the consumer is re-initialized.
        _Reset();
    }

    /// <inheritdoc cref="IResourceConsumer.Consume"/>
    public bool Consume(ResourceType ressource)
    {
        // If the consumer is destroyed, do nothing.
        if (IsDestroyed)
        {
            return false;
        }

        // Check if the resource type matches the required one.
        if (ressource != RequiredResourceType)
        {
            return false;
        }

        // Increment the consumed counter.
        AmountConsumed += 1;
        return true;
    }

    /// <inheritdoc cref="ITurnExecutor.EndTurn"/>
    public void EndTurn()
    {
        // If the consumer is destroyed, do nothing.
        if (IsDestroyed)
        {
            return;
        }

        // Decrement the turn counter.
        TurnsLeft--;
        if (TurnsLeft > 0)
        {
            return;
        }

        // Check if the consumer is destroyed.
        if (AmountConsumed < RequiredAmount)
        {
            IsDestroyed = true;
        }

        // Reset the consumer.
        _Reset();
    }

    /// <summary>
    /// Helper used to reset the counters of the instance.
    /// </summary>
    private void _Reset()
    {
        TurnsLeft = ConsumptionDelay;
        AmountConsumed = 0;
    }

    public static IEnumerable<ResourceConsumer> CreateConsumers(ResourceConsumerParameters[] consumerParameters)
    {
        return consumerParameters.Select(parameters => new ResourceConsumer(parameters));
    }
}

using Godot;

namespace CidreDoux.scripts.model.building.crossing_cost;

/// <summary>
/// Implementation of the <see cref="ICrossingCostComputer"/> that overrides the base cost.
/// </summary>
public class CrossingCostReplacer : ICrossingCostComputer
{
    /// <summary>
    /// The value applied as the cost.
    /// </summary>
    public readonly int Value;

    /// <summary>
    /// Class constructor.
    /// </summary>
    /// <param name="value">The value applied by this instance.</param>
    public CrossingCostReplacer(int value)
    {
        Value = value;
    }

    /// <inheritdoc cref="ICrossingCostComputer.ComputeCrossingCost"/>
    public int ComputeCrossingCost(int baseCost)
    {
        return Value;
    }
}

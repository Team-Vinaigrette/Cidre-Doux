using CidreDoux.scripts.resources.parameters.building;
using Godot;

namespace CidreDoux.scripts.model.building.crossing_cost;

/// <summary>
/// Implementation of the <see cref="ICrossingCostComputer"/> that overrides the base cost.
/// </summary>
public class CrossingCostReplacer : ICrossingCostComputer
{
    public readonly OverrideCrossingCostParameters Parameters;

    /// <summary>
    /// The value applied as the cost.
    /// </summary>
    public int Value => Parameters.Value;

    public CrossingCostReplacer(OverrideCrossingCostParameters parameters)
    {
        Parameters = parameters;
    }

    /// <inheritdoc cref="ICrossingCostComputer.ComputeCrossingCost"/>
    public int ComputeCrossingCost(int baseCost)
    {
        return Value;
    }
}

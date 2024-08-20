using CidreDoux.scripts.resources.parameters.building;
using Godot;

namespace CidreDoux.scripts.model.building.crossing_cost;

/// <summary>
/// Implementation of the <see cref="ICrossingCostComputer"/> that applies a factor to the base cost.
/// </summary>
public class CrossingCostMultiplier : ICrossingCostComputer
{
    public readonly MultiplierCrossingCostParameters Parameters;

    /// <summary>
    /// The factor applied to the cost.
    /// </summary>
    public float Factor => Parameters.Multiplier;

    public CrossingCostMultiplier(MultiplierCrossingCostParameters parameters)
    {
        Parameters = parameters;
    }

    /// <inheritdoc cref="ICrossingCostComputer.ComputeCrossingCost"/>
    public int ComputeCrossingCost(int baseCost)
    {
        return Mathf.RoundToInt(Factor * baseCost);
    }
}

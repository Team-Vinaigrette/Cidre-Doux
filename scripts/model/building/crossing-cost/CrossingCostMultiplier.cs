using Godot;

namespace CidreDoux.scripts.model.building.crossing_cost;

/// <summary>
/// Implementation of the <see cref="ICrossingCostComputer"/> that applies a factor to the base cost.
/// </summary>
public class CrossingCostMultiplier : ICrossingCostComputer
{
    /// <summary>
    /// The factor applied to the cost.
    /// </summary>
    public readonly float Factor;

    /// <summary>
    /// Class constructor.
    /// </summary>
    /// <param name="factor">The factor applied by this instance.</param>
    public CrossingCostMultiplier(float factor)
    {
        Factor = factor;
    }

    /// <inheritdoc cref="ICrossingCostComputer.ComputeCrossingCost"/>
    public int ComputeCrossingCost(int baseCost)
    {
        return Mathf.RoundToInt(Factor * baseCost);
    }
}

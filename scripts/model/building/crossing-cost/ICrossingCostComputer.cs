using CidreDoux.scripts.model.package;

namespace CidreDoux.scripts.model.building.crossing_cost;

/// <summary>
/// Interface used to describe a computer for the <see cref="Package.Walk"/> implementation.
/// </summary>
public interface ICrossingCostComputer
{
    /// <summary>
    /// Method used to compute a crossing cost.
    /// </summary>
    /// <param name="baseCost">The base crossing cost.</param>
    /// <returns>The modified crossing cost.</returns>
    public int ComputeCrossingCost(int baseCost);
}

namespace CidreDoux.scripts.model.building.crossing_cost;

/// <summary>
/// Implementation of the <see cref="ICrossingCostComputer"/> that blocks all traversal.
/// </summary>
public class CrossingBlocker: ICrossingCostComputer
{
    /// <inheritdoc cref="ICrossingCostComputer.ComputeCrossingCost"/>.
    public int ComputeCrossingCost(int baseCost)
    {
        return -1;
    }
}

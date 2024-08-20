namespace CidreDoux.scripts.model;

/// <summary>
/// Simple interface used to describe objects that can execute a turn of the simulation.
/// </summary>
public interface ITurnExecutor
{
    /// <summary>
    /// Callback used to execute a turn of the simulation.
    /// </summary>
    public void EndTurn();
}

namespace CidreDoux.scripts.model;

public interface LifespanManager
{
    public bool IsAlive();
    public bool NextTurn();
}
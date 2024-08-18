using System.Collections.Generic;

namespace CidreDoux.scripts.model;

public enum PackageType
{
    Ressource,
    Build
}

public enum RessourceType
{
    Wood,
    Iron,
    Wheat,
    Food,
    Gold
}

public interface PackageAction
{
    public void performAction(Tile targetTile);
}

public class BuildAction: PackageAction
{
    public readonly BuildingType BuildingType;

    public BuildAction(BuildingType buildType)
    {
        BuildingType = buildType;
    }
    public void performAction(Tile targetTile)
    {
        targetTile.Build(BuildingType);
    }
}

public class DeliveryAction : PackageAction
{
    public readonly RessourceType Ressource;

    public DeliveryAction(RessourceType type)
    {
        Ressource = type;
    }

    public void performAction(Tile targetTile)
    {
        targetTile.Consume(Ressource);
    }
}

public class Package
{
    public readonly PackageType Type;
    public readonly PackageAction actionHandler;
    public readonly Queue<Tile> path;
    public readonly int speed;
    public int performedAlready { get; private set; }
    
    public Package(PackageType type, PackageAction handler, List<Tile> path)
    {
        Type = type;
        this.path = new Queue<Tile>(path);
        this.speed = ModelParameters.DefaultPackageSpeed;
        this.actionHandler = handler;
        this.performedAlready = 0;
    }

    public IEnumerable<Tile> Walk()
    {
        int remainingDistance = speed;
        while (remainingDistance > 0 && path.Count > 0)
        {
            Tile tile = path.Peek();
            int crossingCost = tile.ComputeCrossingCost() - performedAlready;
            if(crossingCost <= remainingDistance)
            {
                remainingDistance -= tile.ComputeCrossingCost();
                performedAlready = 0;
                yield return path.Dequeue();
            }
            else
            {
                performedAlready += remainingDistance;
                yield break;
            }
            
        }
            
    }
}
using System.Collections.Generic;
using Godot;

namespace CidreDoux.scripts.model;

public class Producer
{
    public readonly int delay;
    public readonly PackageType packageType;
    public PackageAction packageAction { get; private set; }
    public int turnCounter { get; private set; }
    public List<Tile> path { get; private set; }

    public Producer(PackageType packageType, PackageAction action, int delay)
    {
        this.delay = delay;
        this.packageType = packageType;
        this.packageAction = action;
        this.path = null;
        this.turnCounter = 0;
    }

    public static Producer NewRessourceProducer(RessourceType type, int delay)
    {
        return new Producer(PackageType.Ressource, new DeliveryAction(type), delay);
    }

    public static Producer NewBuildProducer(int delay)
    {
        return new Producer(PackageType.Build, null, delay);
    }
    
    public bool CanProduce()
    {
        return path != null && packageAction != null;
    }
    
    public Package ProducePackage()
    {
        if (turnCounter != 0 || !CanProduce()) return null;
    
        turnCounter = delay;
        var res = new Package(packageType, packageAction, path);
        if (packageType == PackageType.Build) packageAction = null;
        return res;
    }

    public void AssignPath(List<Tile> path)
    {
        this.path = path;
    }
    public void NextTurn()
    {
        if (CanProduce() && turnCounter > 0) turnCounter--;
    }
    
    public void AssignBuildAction(BuildAction action)
    {
        if (packageType != PackageType.Build)
        {
            GD.Print("Warn: attempt to assign build action to non build-type producer");
            return;
        }

        packageAction = action;
    }
}
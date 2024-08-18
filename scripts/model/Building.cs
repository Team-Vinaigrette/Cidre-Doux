using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using Godot;

namespace CidreDoux.scripts.model;
public enum BuildingType
{
    Field,
    Farm,
    Mine,
    Sawmill,
    Harbor,
    Market,
    Road,
    Base
}

public class RessourceConsumer
{
    public readonly RessourceType neededRessource;
    public readonly int amount;
    public readonly int delay;
    public int turnsLeft { get; private set; }
    public int amountConsumed {get; private set;}
    public bool destroyed {get; private set;}

    public RessourceConsumer(RessourceType neededRessource, int amount, int delay)
    {
        this.neededRessource = neededRessource;
        this.amount = amount;
        this.delay = delay;
        this.destroyed = false;
        reset();
    }

    private void reset()
    {
        this.turnsLeft = delay;
        this.amountConsumed = 0;
    }

    public bool isSatisfied()
    {
        return amountConsumed >= amount;
    }

    public bool Consume(RessourceType ressource)
    {
        if(ressource == neededRessource)
        {
            amountConsumed += 1;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void nextTurn()
    {
        turnsLeft--;
        if (turnsLeft > 0)
        {
            return;
        }

        if (amountConsumed < amount)
        {
            destroyed = true;
        }
        else
        {
            reset();
        }
    }
}

public interface ICrossingCostComputer
{
    public int ComputeCrossingCost(int baseCost);
}

public class CrossingBlocker : ICrossingCostComputer
{
    public int ComputeCrossingCost(int baseCost)
    {
        return -1;
    }
}

public class CrossingCostMultiplier : ICrossingCostComputer
{
    public readonly float factor;

    public CrossingCostMultiplier(float factor)
    {
        this.factor = factor;
    }
    
    public int ComputeCrossingCost(int baseCost)
    {
        return (int)Math.Round(factor * baseCost);
    }
}

public class Building
{
    public readonly BuildingType Type;
    public bool destroyed { get; private set; }
    public readonly Producer producer;
    public readonly List<RessourceConsumer> consumers;
    private readonly ICrossingCostComputer crossingComputer;


    public static Building NewBuilding(BuildingType type)
    {
        Producer producer = null;
        List<RessourceConsumer> consumers = new List<RessourceConsumer>();
        ICrossingCostComputer computer = null;
        switch (type)
        {
            case BuildingType.Base:
                consumers.Add(new RessourceConsumer(RessourceType.Iron, 1, 5));
                producer = Producer.NewBuildProducer(1);
                computer = new CrossingBlocker();
                break;
            case BuildingType.Farm:
                consumers.Add(new RessourceConsumer(RessourceType.Wood, 1, 5));
                producer = Producer.NewRessourceProducer(RessourceType.Food, 6);
                break;
            case BuildingType.Mine:
                consumers.Add(new RessourceConsumer(RessourceType.Wood, 1, 5));
                producer = Producer.NewRessourceProducer(RessourceType.Iron, 10);
                break;
            case BuildingType.Sawmill:
                consumers.Add(new RessourceConsumer(RessourceType.Food, 1, 5));
                producer = Producer.NewRessourceProducer(RessourceType.Wood, 4);
                computer = new CrossingCostMultiplier(2f);
                break;
            case BuildingType.Road:
                computer = new CrossingCostMultiplier(0.5f);
                break;
            default:
                GD.PrintErr($" generator for {type} building type not yet implemented");
                break;
        }
        return new Building(type, producer, consumers, computer);
    }

    private Building(BuildingType Type, Producer Producer, List<RessourceConsumer> Consumers, ICrossingCostComputer computer)
    {
        this.Type = Type;
        this.producer = Producer;
        this.consumers = Consumers;
        this.destroyed = false;
        this.crossingComputer = computer;
    }
    
    public int ComputeCrossingCost(int baseCost)
    {
        return crossingComputer?.ComputeCrossingCost(ModelParameters.DefaultPackageSpeed) ?? baseCost;
    }

    public Package ProducePackage()
    {
        return producer.ProducePackage();
    }

    public void Consume(RessourceType ressource)
    {
        bool consumed = false;
        foreach (RessourceConsumer consumer in consumers)
        {
            consumed = consumer.Consume(ressource);
            if( consumed ){ break; }
        }

        if (!consumed)
        {
            GD.Print($"No consumer found for {ressource} in {Type}");
        }
    }

    public void AssignPath(List<Tile> path)
    {
        producer.AssignPath(path);
    }
    
    public void NextTurn()
    {
        foreach (RessourceConsumer consumer in consumers)
        {
            consumer.nextTurn();
            if (consumer.destroyed)
            {
                destroyed = true;
            }
        }
        producer.NextTurn();
    }
}
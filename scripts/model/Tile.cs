using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CidreDoux.scripts.model;

public enum BackgroundType
{
    Grass,
    Mountain,
    Forest,
    Water
}

public partial class Tile : GodotObject
{
    
    
    static Tuple<int, int>[] relativeOddNeighbors =
    {
        Tuple.Create(-1, 0),
        Tuple.Create(-1, -1),
        Tuple.Create(0, -1),
        Tuple.Create(1, 0),
        Tuple.Create(0, 1),
        Tuple.Create(-1, 1), 
    };
        
    static Tuple<int, int>[] relativeEvenNeighbors =
    {
        Tuple.Create(-1, 0),
        Tuple.Create(0, -1),
        Tuple.Create(1, -1),
        Tuple.Create(1, 0),
        Tuple.Create(1, 1),
        Tuple.Create(0, 1), 
    };
    public readonly BackgroundType Background;
    public Building Building { get; private set; }
    public static readonly Array BgValues = Enum.GetValues(typeof(BackgroundType));
    public readonly int Col, Row;
    public readonly HexMap ParentMap;

    public Tile(int col, int row, BackgroundType bg, HexMap map){
        Background = bg;
        Building = null;
        Col = col;
        Row = row;
        ParentMap = map;
    }

    public override String ToString() {
        return $"Tile[{Col},{Row}]<{Background}, {Building}>";
    }
    
    public String ToStringMap() {
        return "<" + Col + ":" + Row + ">";
        return "<" + Background.ToString().Substring(0, 2) + ":" + 
               (Building is not null ? Building.Type.ToString().Substring(0,2) : "  ") + ">";
    }

    public List<Tile> GetNeighbors()
    {
        return ParentMap?.GetNeighbors(this) ?? new List<Tile>();
    }

    public bool IsNeighbor(Tile tile)
    {
        if (ParentMap is null || tile.ParentMap != ParentMap)
        {
            GD.Print("Tring to compare tiles from different maps for neighborhood");
            return false;
        }

        Tuple<int, int> relPos = Tuple.Create(Col - tile.Col, Row - tile.Row);
        return (Mathf.Abs(Row % 2) == 0) ? relativeOddNeighbors.Contains(relPos) : relativeEvenNeighbors.Contains(relPos);
    }

    public bool HasBuilding()
    {
        return Building is not null;
    }
    
    public void Build(BuildingType buildingType)
    {
        if (HasBuilding())
        {
            GD.Print($"Error: Tile {Col}:{Row} already has a building: ${this.Building.Type}");
        }
        else
        {
            GD.Print($"<{Col}:{Row}> Building: {buildingType}");
            Building = Building.NewBuilding(buildingType);
            EmitSignal(Tile.SignalName.OnModelUpdate);
        }
        
    }

    public void Consume(RessourceType ressource)
    {
        if (Building is null)
        {
            GD.Print($"Error: Tile {Col}:{Row} cannot consume {ressource} because it is empty");
        }
        else
        {
            Building.Consume(ressource);
        }
    }

    public void AssignPath(List<Tile> path)
    {
        Building.AssignPath(path);
    }
    
    public int ComputeCrossingCost()
    {
        int BaseCost = ModelParameters.BGTypeCrossingCosts[Background];
        return Building?.ComputeCrossingCost(BaseCost) ?? BaseCost;
    }
    
    public void NextTurn()
    {
        Building.NextTurn();
    }

    private List<Tile> ReconstructPath(Dictionary<Tile, Tile> cameFrom)
    {
        var current = this;
        var res = new Stack<Tile>();
        res.Push(current);
        while(cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            res.Push(current);
        }
        
        return new List<Tile>(res);
    }

    public int ManhattanDist(Tile goal)
    {
        var dx = goal.Col - this.Col;
        var dy = goal.Row - this.Row;

        if (Mathf.Sign(dx) == Mathf.Sign(dy))
        {
            return ModelParameters.DefaultPackageSpeed + Mathf.Abs(dx + dy);
        }
        else
        {
            return ModelParameters.DefaultPackageSpeed + Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
        }
    }
    
    public List<Tile> AStar(Tile goal)
    {
        if (goal.ParentMap != ParentMap)
        {
            GD.PrintErr("Attempted to call AStar on two tiles from different maps");
            return null;
        }

        var openSet = new List<Tile> { this };

        var cameFrom = new Dictionary<Tile, Tile>();

        var gScores = new Dictionary<Tile, int> { { this, 0 } };

        var fScores = new Dictionary<Tile, int>{{this, this.ManhattanDist(goal)}};

        while (openSet.Count > 0)
        {
            openSet.Sort((x, y) => fScores[x].CompareTo(fScores[y]));
            var current = openSet[0];
            if (current == goal) return goal.ReconstructPath(cameFrom);

            openSet.Remove(current);
            foreach (var neighbor in current.GetNeighbors())
            {
                // Cost of entering last tile is always 1 turn
                var crossingCost = neighbor == goal ? ModelParameters.DefaultPackageSpeed : neighbor.ComputeCrossingCost();
                if(crossingCost < 0) continue;
                
                int currentGScore = gScores.ContainsKey(current) ? gScores[current] : int.MaxValue;
                int neighborGScore = gScores.ContainsKey(neighbor) ? gScores[neighbor] : int.MaxValue;
                int tentativeGScore = (currentGScore + crossingCost);
                if (tentativeGScore < neighborGScore)
                {
                    cameFrom[neighbor] = current;
                    gScores[neighbor] = tentativeGScore;
                    fScores[neighbor] = tentativeGScore + neighbor.ManhattanDist(goal);
                    if(!openSet.Contains(neighbor)) openSet.Add(neighbor);
                }
            }
        }
        
        GD.Print($"AStar found no path from {this} to {goal}");
        return null;
    }
}

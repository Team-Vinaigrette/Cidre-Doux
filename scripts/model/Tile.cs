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

public class Tile
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
    
    public void Build(BuildingType buildingType)
    {
        if (Building is not null)
        {
            GD.Print($"Error: Tile {Col}:{Row} already has a building: ${Building}");
        }
        else
        {
            GD.Print($"<{Col}:{Row}> Building: {buildingType}");
            Building = Building.NewBuilding(buildingType);
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
}

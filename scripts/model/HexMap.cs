using Godot;
using System;
using System.Collections.Generic;

namespace CidreDoux.scripts.model;

public class HexMap
{
    public Dictionary<Tuple<int, int>, Tile> Map;
    public int Size;
    private readonly Random _randomizer = new Random();

    /**
     * Creates a new random hexmap with indices ranging from -initSize to + initSize
     */
    public HexMap(int initSize) {
        Size = initSize;
        Map = new Dictionary<Tuple<int, int>, Tile>();
        for(int x = -initSize; x <= initSize; x++){
            for(int y = -initSize; y <= initSize; y++){
                Map.Add(new Tuple<int, int>(x, y), new Tile(x, y, GetBackgroundType(), this));
            }
        }
        GetTile(0, 0).Build(BuildingType.Base);
    }

    // Extends the HexMap, adding one row and one column on each side
    public void Grow() {
        for(int y = -Size; y <= Size; y++){
            Map.Add(new Tuple<int, int>(-Size - 1, y), new Tile(-Size - 1, y, GetBackgroundType(), this));
            Map.Add(new Tuple<int, int>(Size + 1, y), new Tile(Size + 1, y, GetBackgroundType(), this));
        }
        Size += 1;
        for(int x = -Size; x <= Size; x++){
            Map.Add(new Tuple<int, int>(x, -Size), new Tile(x, -Size, GetBackgroundType(), this));
            Map.Add(new Tuple<int, int>(x, Size), new Tile(x, Size, GetBackgroundType(), this));
        }
    }

    public BackgroundType GetBackgroundType(){
        return (BackgroundType)Tile.BgValues.GetValue(_randomizer.Next(Tile.BgValues.Length))!;
    }

    public override String ToString() {
        String res = "";
		foreach(KeyValuePair<Tuple<int, int>, Tile> kvp in Map){
			res += "[" + kvp.Key.Item1 + ":" + kvp.Key.Item2 + " -> " + kvp.Value.ToString() + "]\n";
		}
        return res;
    }

    public Tile GetTile(int col, int row){
        return Map[Tuple.Create(col, row)];
    }

    public List<Tile> GetNeighbors(Tile tile)
    {
        // Avoid getting out of map bounds
        if(Size == Mathf.Abs(tile.Col) || Size == Mathf.Abs(tile.Row)) this.Grow();
        
        int rowOffset = Mathf.Abs(tile.Row % 2);
        List<Tile> neighbors = new List<Tile>();
        neighbors.Add(GetTile(tile.Col - 1, tile.Row));
        neighbors.Add(GetTile(tile.Col -1 + rowOffset, tile.Row - 1));
        neighbors.Add(GetTile(tile.Col + rowOffset, tile.Row - 1));
        neighbors.Add(GetTile(tile.Col + 1, tile.Row));
        neighbors.Add(GetTile(tile.Col + rowOffset, tile.Row + 1));
        neighbors.Add(GetTile(tile.Col -1 + rowOffset, tile.Row + 1));
        return neighbors;
    }
    
    public String ToStringMap() {
        String res = "";
        for(int row = -Size; row <= Size; row++){
            if(row % 2 != 0){
                res += "   ";
            }
            for(int col = -Size; col <= Size; col ++){
                res += GetTile(col, row).ToStringMap() + " ";
            }
            res += "\n";
        }
        return res;
    }
}

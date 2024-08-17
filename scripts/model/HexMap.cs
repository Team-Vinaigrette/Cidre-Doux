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
                Map.Add(new Tuple<int, int>(x, y), new Tile(x, y, GetBackgroundType()));
            }
        }
        GetTile(0, 0).Building = BuildingType.Base;
    }

    // Extends the HexMap, adding one row and one column on each side
    public void Grow() {
        for(int y = -Size; y <= Size; y++){
            Map.Add(new Tuple<int, int>(-Size - 1, y), new Tile(-Size - 1, y, GetBackgroundType()));
            Map.Add(new Tuple<int, int>(Size + 1, y), new Tile(Size + 1, y, GetBackgroundType()));
        }
        Size += 1;
        for(int x = -Size; x <= Size; x++){
            Map.Add(new Tuple<int, int>(x, -Size), new Tile(x, -Size, GetBackgroundType()));
            Map.Add(new Tuple<int, int>(x, Size), new Tile(x, Size, GetBackgroundType()));
        }
    }

    public BackgroundType GetBackgroundType(){
        return (BackgroundType)Tile.BGValues.GetValue(_randomizer.Next(Tile.BGValues.Length))!;
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

    public String ToStringMap() {
        String res = "";
        for(int col = -Size; col <= Size; col++){
            if(col % 2 != Size % 2){
                res += "    ";
            }
            for(int row = -Size; row <= Size; row ++){
                res += GetTile(col, row).ToStringMap() + " ";
            }
            res += "\n";
        }
        return res;
    }
}

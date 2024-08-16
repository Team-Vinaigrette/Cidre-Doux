using Godot;
using System;
using System.Collections.Generic;

namespace CidreDoux.scripts.model;

public partial class HexMap : GodotObject
{
    Dictionary<Tuple<int, int>, Tile> map;
    int size;
    Random randomizer = new Random();

    /**
     * Creates a new random hexmap with indices ranging from -initSize to + initSize
     */
    public HexMap(int initSize) {
        size = initSize;
        map = new Dictionary<Tuple<int, int>, Tile>();
        for(int x = -initSize; x <= initSize; x++){
            for(int y = -initSize; y <= initSize; y++){
                map.Add(new Tuple<int, int>(x, y), new Tile(GetBackgroundType()));
            }
        }
        GetTile(0, 0).Building = BuildingType.Base;
    }

    // Extends the HexMap, adding one row and one column on each side
    public void Grow() {
        for(int y = -size; y <= size; y++){
            map.Add(new Tuple<int, int>(-size - 1, y), new Tile(GetBackgroundType()));
            map.Add(new Tuple<int, int>(size + 1, y), new Tile(GetBackgroundType()));
        }
        size += 1;
        for(int x = -size; x <= size; x++){
            map.Add(new Tuple<int, int>(x, -size), new Tile(GetBackgroundType()));
            map.Add(new Tuple<int, int>(x, size), new Tile(GetBackgroundType()));
        }
    }

    public BackgroundType GetBackgroundType(){
        return (BackgroundType)Tile. BGValues.GetValue(randomizer.Next(Tile.BGValues.Length));
    }

    public override String ToString() {
        String res = "";
		foreach(KeyValuePair<Tuple<int, int>, Tile> kvp in map){
			res += "[" + kvp.Key.Item1 + ":" + kvp.Key.Item2 + " -> " + kvp.Value.ToString() + "]\n";
		}
        return res;
    }

    public Tile GetTile(int col, int row){
        return map[Tuple.Create(col, row)];
    }

    public String ToStringMap() {
        String res = "";
        for(int col = -size; col <= size; col++){
            if(col % 2 != size % 2){
                res += "    ";
            }
            for(int row = -size; row <= size; row ++){
                res += GetTile(col, row).ToStringMap() + " ";
            }
            res += "\n";
        }
        return res;
    }
}

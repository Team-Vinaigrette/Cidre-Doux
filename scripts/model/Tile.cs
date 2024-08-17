using Godot;
using System;

namespace CidreDoux.scripts.model;

public enum BackgroundType
{
    Grass,
    Mountain,
    Forest    
}

public enum BuildingType
{
    Empty,
    Farm,
    Mine,
    Sawmill,
    Road,
    Base
}

public class Tile
{
    public BackgroundType Background;
    public BuildingType Building;

    public static Array BGValues = Enum.GetValues(typeof(BackgroundType));

    public readonly int _x, _y;

    public Tile(int x, int y, BackgroundType bg){
        Background = bg;
        Building = BuildingType.Empty;
        _x = x;
        _y = y;
    }

    public override String ToString() {
        return "Tile<" + Background + ", " + Building + ">";
    }

    public String ToStringMap() {
        return "<" + Background.ToString().Substring(0, 2) + ":" + Building.ToString().Substring(0,2) + ">";
    }
}

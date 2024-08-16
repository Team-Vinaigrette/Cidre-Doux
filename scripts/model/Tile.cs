using Godot;
using System;

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

public partial class Tile : GodotObject
{
    public BackgroundType Background;
    public BuildingType Building;

    public static Array BGValues = Enum.GetValues(typeof(BackgroundType));

    public Tile(BackgroundType bg){
        Background = bg;
        Building = BuildingType.Empty;
    }

    public override String ToString() {
        return "Tile<" + Background + ", " + Building + ">";
    }

    public String ToStringMap() {
        return "<" + Background.ToString().Substring(0, 2) + ":" + Building.ToString().Substring(0,2) + ">";
    }
}

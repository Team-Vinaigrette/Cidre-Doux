using CidreDoux.scripts.model.building;
using Godot;

namespace CidreDoux.scripts.resources;

/// <summary>
///
/// </summary>
[GlobalClass]
public partial class BuildingTextureMap : Resource
{

    [Export] public Texture2D Field;
    [Export] public Texture2D Farm;
    [Export] public Texture2D Mine;
    [Export] public Texture2D Sawmill;
    [Export] public Texture2D Harbor;
    [Export] public Texture2D Market;
    [Export] public Texture2D Road;
    [Export] public Texture2D Base;

    public Texture2D GetTextureForBuildingType(BuildingType buildingType)
    {
        switch (buildingType)
        {
        case BuildingType.Field:
            return Field;
        case BuildingType.Farm:
            return Farm;
        case BuildingType.Mine:
            return Mine;
        case BuildingType.Sawmill:
            return Sawmill;
        case BuildingType.Harbor:
            return Harbor;
        case BuildingType.Market:
            return Market;
        case BuildingType.Road:
            return Road;
        case BuildingType.Base:
            return Base;
        default:
            return null;
        }
    }
}

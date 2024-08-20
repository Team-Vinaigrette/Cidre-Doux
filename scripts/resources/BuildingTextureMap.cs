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
        return buildingType switch
        {
            BuildingType.Field => Field,
            BuildingType.Farm => Farm,
            BuildingType.Mine => Mine,
            BuildingType.Sawmill => Sawmill,
            BuildingType.Harbor => Harbor,
            BuildingType.Market => Market,
            BuildingType.Road => Road,
            BuildingType.Base => Base,
            _ => null
        };
    }
}

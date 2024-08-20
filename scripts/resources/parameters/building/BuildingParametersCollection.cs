using System;
using CidreDoux.scripts.model.building;
using Godot;

namespace CidreDoux.scripts.resources.parameters.building;

[GlobalClass]
public partial class BuildingParametersCollection : Resource
{
    [Export] public DeliveryBuildingParameters Field;
    [Export] public DeliveryBuildingParameters Farm;
    [Export] public DeliveryBuildingParameters Mine;
    [Export] public DeliveryBuildingParameters Sawmill;
    [Export] public DeliveryBuildingParameters Harbor;
    [Export] public DeliveryBuildingParameters Market;
    [Export] public EmptyBuildingParameters Road;
    [Export] public BaseBuildingParameters Base;

    public BuildingParameters GetParametersForBuildingType(BuildingType buildingType)
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
        default:
            return Base;
        }
    }
}

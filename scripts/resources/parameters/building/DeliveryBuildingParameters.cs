using CidreDoux.scripts.model.package;
using Godot;

namespace CidreDoux.scripts.resources.parameters.building;

[GlobalClass]
public partial class DeliveryBuildingParameters: BuildingParameters
{
    [Export] public ResourceType ProducedResource;
    [Export] public uint ProductionDelay;
}

using CidreDoux.scripts.model.package;
using Godot;

namespace CidreDoux.scripts.resources.parameters.building;

[GlobalClass]
public partial class ResourceConsumerParameters: Resource
{
    [Export] public ResourceType ConsumedResource;
    [Export] public uint ConsumedAmount;
    [Export] public uint ConsumptionDelay;
}

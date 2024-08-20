using Godot;

namespace CidreDoux.scripts.resources.parameters.building;

public abstract partial class BuildingParameters : Resource
{
    [Export] public ResourceConsumerParameters[] ConsumerParameters;
    [Export] public CrossingCostParameters CrossingCostParameters;
}

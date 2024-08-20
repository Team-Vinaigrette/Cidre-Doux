using Godot;

namespace CidreDoux.scripts.resources.parameters.building;

[GlobalClass]
public partial class OverrideCrossingCostParameters: CrossingCostParameters
{
    [Export] public int Value;
}

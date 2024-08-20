using CidreDoux.scripts.model.package;
using Godot;

namespace CidreDoux.scripts.resources;

/// <summary>
///
/// </summary>
[GlobalClass]
public partial class ResourceTextureMap : Resource
{

    [Export] public Texture2D Food;
    [Export] public Texture2D Gold;
    [Export] public Texture2D Wood;
    [Export] public Texture2D Wheat;
    [Export] public Texture2D Stone;

    public Texture2D GetTextureForResourceType(ResourceType resourceType)
    {
        return resourceType switch
        {
            ResourceType.Food => Food,
            ResourceType.Gold => Gold,
            ResourceType.Wood => Wood,
            ResourceType.Wheat => Wheat,
            ResourceType.Stone => Stone,
            _ => null
        };
    }
}

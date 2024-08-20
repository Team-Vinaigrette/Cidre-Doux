using System.Collections.Generic;
using CidreDoux.scripts.model.building;
using CidreDoux.scripts.model.package;
using CidreDoux.scripts.model.tile;
using Godot;

namespace CidreDoux.scripts.view;

public static class TextureCoords
{
    public static readonly Dictionary<BackgroundType, Vector2> Backgrounds = new Dictionary<BackgroundType, Vector2>()
    {
        [BackgroundType.Water] = new Vector2(0, 0),
        [BackgroundType.Forest] = new Vector2(0, 1),
        [BackgroundType.Grass] = new Vector2(1, 0),
        [BackgroundType.Mountain] = new Vector2(1, 1)
    };

    public static readonly Dictionary<BuildingType, Rect2> Buildings = new Dictionary<BuildingType, Rect2>(){
        [BuildingType.Base] = new Rect2(0, 0, 360, 400),
        [BuildingType.Market] = new Rect2(400, 40, 360, 360),
        [BuildingType.Mine] = new Rect2(800, 20, 380, 400),
        [BuildingType.Field] = new Rect2(0, 460, 380, 300),
        [BuildingType.Farm] = new Rect2(400, 430, 330, 320),
        [BuildingType.Sawmill] = new Rect2(830, 450, 340, 320),
        [BuildingType.Harbor] = new Rect2(0, 850, 380, 350),
        [BuildingType.Road] = new Rect2(400, 850, 360, 350),
        [BuildingType.Base] = new Rect2(0, 0, 360, 400),
    };

    public static readonly Dictionary<ResourceType, Rect2> Ressources = new Dictionary<ResourceType, Rect2>()
    {
        [ResourceType.Wood] = new Rect2(70, 90, 140, 150),
        [ResourceType.Stone] = new Rect2(330, 90, 140, 150),
        [ResourceType.Gold] = new Rect2(325, 285, 140, 150),
        [ResourceType.Wheat] = new Rect2(70, 285, 140, 150),
        [ResourceType.Food] = new Rect2(70, 495, 140, 150)
    };
}
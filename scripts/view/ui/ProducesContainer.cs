using Godot;
using System;
using CidreDoux.scripts.model.building;
using CidreDoux.scripts.model.package.action;
using CidreDoux.scripts.resources;

public partial class ProducesContainer : VBoxContainer
{
    [Export] public ResourceTextureMap TextureMap;
    [Export] public Label TurnCounter;
    [Export] public TextureRect TextureRect;

    public void AssignProducer(PackageProducer producer)
    {
        // If the producer is not a delivery producer, hide the element.
        if (producer.PackageAction is not DeliveryAction deliveryAction)
        {
            Visible = false;
            return;
        }

        // Update the icon and counter.
        TurnCounter.Text = producer.TurnCounter.ToString();
        TextureRect.Texture = TextureMap.GetTextureForResourceType(deliveryAction.ResourceType);
        Visible = true;
    }
}

using Godot;
using System;
using System.Text;
using CidreDoux.scripts.model.building;
using CidreDoux.scripts.resources;

public partial class ConsumesInfo : HBoxContainer
{
    [Export] public ResourceTextureMap TextureMap;
    [Export] public Label Amount;
    [Export] public Label TurnCounter;
    [Export] public TextureRect TextureRect;

    public void AssignConsumer(ResourceConsumer consumer)
    {
        var builder = new StringBuilder();
        builder.Append(consumer.AmountConsumed);
        builder.Append(' ');
        builder.Append('/');
        builder.Append(' ');
        builder.Append(consumer.RequiredAmount);

        Amount.Text = builder.ToString();
        TurnCounter.Text = consumer.TurnsLeft.ToString();
        TextureRect.Texture = TextureMap.GetTextureForResourceType(consumer.RequiredResourceType);
        Visible = true;
    }
}

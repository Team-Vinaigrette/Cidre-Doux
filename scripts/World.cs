using System;
using System.Diagnostics.CodeAnalysis;
using CidreDoux.scripts.view;
using Godot;

namespace CidreDoux.scripts;

public partial class World : Node2D
{
    [Export, MaybeNull] public PackedScene TileScene;
    [Export] public int Width = 10;
    [Export] public int Height = 10;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Check if the tile scene was set.
        if (TileScene == null)
        {
            GD.PushError("You forgot to set the Tile PackedScene for the World instance.");
            return;
        }

        // Generate a list of NxM tiles.
        for (var i = -Width; i <= Width; i++)
        {
            for (var j = -Height; j <= Height; j++)
            {
                // Instantiate a new tile.
                var tile = TileScene.Instantiate<Tile>();
                tile.Name = $"Tile ({i}, {j})";

                // Create a colour based on the distance to the center.
                var red = Math.Abs(i) / (2.0f * Width);
                var green = Math.Abs(j) / (2.0f * Height);

                tile.ChangeTileColor(new Color(red, green, 1));

                // Attach the tile to the world.
                AddChild(tile);

                // Set the position of the tile.
                tile.Position = Tile.GetHexagonCenterWorldPosition(i, j);
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}

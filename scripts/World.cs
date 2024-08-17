using System;
using System.Diagnostics.CodeAnalysis;
using CidreDoux.scripts.model;
using Godot;
using Tile = CidreDoux.scripts.view.Tile;

namespace CidreDoux.scripts;

public partial class World : Node2D
{
    /// <summary>
    /// The packed scene used to instantiate <see cref="view.Tile"/> objects.
    /// </summary>
    [Export, MaybeNull] public PackedScene TileScene;

    /// <summary>
    /// The number of tiles to render horizontally.
    /// </summary>
    [Export] public int Width = 10;

    /// <summary>
    /// The number of tiles to render vertically.
    /// </summary>
    [Export] public int Height = 10;

    /// <summary>
    /// The camera that is currently active in the world.
    /// </summary>
    [Export, MaybeNull] public Camera2D Camera;

    /// <summary>
    /// Signal emitted when the selected tile is changed.
    /// </summary>
    [Signal]
    public delegate void OnSelectedTileChangeEventHandler(int column, int row);

    /// <summary>
    /// The currently selected tile.
    /// </summary>
    public Tuple<int, int> SelectedTile { get; private set; }

    /// <inheritdoc cref="Node._Ready"/>
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
                // Create a new model for the tile.
                var model = new model.Tile(i, j, BackgroundType.Grass);

                // Instantiate a new tile.
                var tile = TileScene.Instantiate<Tile>();
                tile.Name = $"Tile ({i}, {j})";
                tile.Model = model;
                tile.World = this;

                // Attach the tile to the world.
                AddChild(tile);

                // Set the position of the tile.
                tile.Position = Tile.GetHexagonCenterWorldPosition(i, j);
            }
        }
    }

    /// <inheritdoc cref="Node._Process"/>
    public override void _Process(double delta)
    {
        // Get the active camera in the scene.
        if (Camera is null)
        {
            GD.PrintErr("Failed to find a camera in the current world");
            return;
        }

        // Get the world position of the mouse cursor.
        var viewport = GetViewport();
        var worldPosition = viewport.GetMousePosition() - GetViewportRect().Size / 2 + Camera.Position;

        // Update the location of the hovered tile.
        SelectedTile = Tile.GetHexagonMapCoordinates(Tile.FindClosestHexCenter(worldPosition));

        // Send a "hover changed" event.
        EmitSignal(SignalName.OnSelectedTileChange, SelectedTile.Item1, SelectedTile.Item2);
    }
}

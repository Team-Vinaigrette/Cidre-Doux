using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CidreDoux.scripts.model.tile;
using CidreDoux.scripts.view;
using Godot;

namespace CidreDoux.scripts.controller;

/// <summary>
/// Controller class used to instantiate the world.
/// This class is in charge of instantiating a <see cref="HexMap"/> and initializing <see cref="ViewTile"/> objects.
/// </summary>
public partial class World : Node2D
{
    /// <summary>
    /// The packed scene used to instantiate <see cref="ViewTile"/> objects.
    /// </summary>
    [Export, MaybeNull] public PackedScene TileScene;

    /// <summary>
    /// The size of the generated map.
    /// </summary>
    [Export] public int Size = 10;

    /// <summary>
    /// Dictionary of all the tiles found in the world.
    /// </summary>
    public readonly Dictionary<TileLocation, ViewTile> Tiles = new();

    /// <summary>
    /// The currently selected tile.
    /// </summary>
    [MaybeNull]
    public ViewTile SelectedTile { get; private set; }

    /// <summary>
    /// Helper function used to retrieve the <see cref="ViewTile"/> at the given coordinates.
    /// </summary>
    /// <param name="location">The location of the tile.</param>
    /// <returns>The requested <see cref="ViewTile"/> or null if the tile was not found.</returns>
    public ViewTile GetViewTile(TileLocation location)
    {
        // Convert the column and row into an index.
        var column = location.Column + Size;
        var row = location.Row + Size;
        var index = column * (2 * Size + 1) + row;

        // Get the child at the specified location.
        var children = GetChildren();
        if (index < 0 || index >= children.Count)
        {
            GD.PrintErr($"Tried to access a ViewTile at {location} that does not exist");
            return null;
        }
        var node = children[index];

        // Check if the node is a ViewTile instance.
        if (node is ViewTile tile)
        {
            return tile;
        }

        // Print an error to the console.
        GD.PrintErr($"Found a child at index {index} that was not a ViewTile!");
        return null;
    }

    /// <inheritdoc cref="Node._Ready"/>
    public override void _Ready()
    {
        // Check if the tile scene was set.
        if (TileScene == null)
        {
            GD.PushError("You forgot to set the Tile PackedScene for the World instance.");
            return;
        }

        // Initialize a new HexMap.
        var map = new HexMap(Size);

        // Create the ViewTiles for all the elements in the hex map.
        foreach(var tile in map.Map.Values)
        {
            // Instantiate a new tile.
            Tiles.Add(tile.Location, ViewTile.Instantiate(TileScene, tile, this));
        }
    }

    /// <inheritdoc cref="Node._Process"/>
    public override void _Process(double delta)
    {
        // Get the world position of the mouse cursor.
        var viewport = GetViewport();
        var worldPosition = viewport.GetMousePosition();
        worldPosition -= GetViewportRect().Size / 2;
        worldPosition += GameController.GetController().Player.Camera.GlobalPosition;

        // Update the location of the hovered tile.
        var selectedLocation = ViewTile.GetHexagonMapCoordinates(ViewTile.FindClosestHexCenter(worldPosition));

        // Get the selected tile.
        SelectedTile = GetViewTile(selectedLocation);
    }
}

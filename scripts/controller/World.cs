using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CidreDoux.scripts.model.building;
using CidreDoux.scripts.model.package.action;
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
    /// The currently selected tile.
    /// </summary>
    [MaybeNull]
    public ViewTile SelectedTile { get; private set; }

    /// <summary>
    /// Bounds of the world.
    /// </summary>
    public Rect2 Bounds => new(
        -Size * ViewTile.CenterToCenterHorizontalDistance - ViewTile.CenterToCenterHorizontalDistance / 2f,
        -Size * ViewTile.CenterToCenterVerticalDistance,
        2 * Size * ViewTile.CenterToCenterHorizontalDistance + ViewTile.CenterToCenterHorizontalDistance,
        2 * Size * ViewTile.CenterToCenterVerticalDistance
    );

    /// <summary>
    /// <see cref="HexMap"/> object used to store the world map of this view.
    /// </summary>
    private HexMap _map;

    public void SelectTile(ViewTile tile)
    {
        if(SelectedTile is not null) SelectedTile.OnTileDeselected();
        SelectedTile = tile;
        tile.OnTileSelected();
    }

    public void ProducePackages()
    {
        foreach (var tile in _map.Map)
        {
            var package = tile.Value.ProducePackage();
            if (package is not null) GameController.GetController().AddMessenger(package);
        }
    }

    public void EndTurn()
    {
        _map.EndTurn();
    }

    /// <summary>
    /// List of all the <see cref="ViewTile"/> stored in the world.
    /// </summary>
    private Dictionary<TileLocation, ViewTile> _viewTiles;

    /// <summary>
    /// Helper function used to retrieve the <see cref="ViewTile"/> at the given coordinates.
    /// </summary>
    /// <param name="location">The location of the tile.</param>
    /// <returns>The requested <see cref="ViewTile"/> or null if the tile was not found.</returns>
    public ViewTile GetViewTile(TileLocation location)
    {
        return _viewTiles.GetValueOrDefault(location);
    }

    /// <summary>
    /// Return the center tile of the map that contains the player's base
    /// </summary>
    /// <returns></returns>
    public Tile GetBaseTile()
    {
        return _map.GetTile(0, 0);
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
        _map = new HexMap(Size);
        _viewTiles = new Dictionary<TileLocation, ViewTile>();

        // Create the ViewTiles for all the elements in the hex map.
        foreach(var tile in _map.Map.Values)
        {
            // Instantiate a new tile.
            var view = ViewTile.Instantiate(TileScene, tile);
            _viewTiles.Add(tile.Location, view);
            AddChild(view);
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
        var newSelectedTile = GetViewTile(selectedLocation);
        if (newSelectedTile != SelectedTile) SelectTile(newSelectedTile);
    }
}

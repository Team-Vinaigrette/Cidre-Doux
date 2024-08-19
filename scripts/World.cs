using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CidreDoux.scripts.model;
using CidreDoux.scripts.view;
using Godot;

namespace CidreDoux.scripts;

public enum GameState
{
    Idle,
    Build,
    AssignPath,
    TurnEnd,
}

public partial class World : Node2D
{
    /// <summary>
    /// The packed scene used to instantiate <see cref="ViewTile"/> objects.
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

    [Export] public Path PathPreviewer;

    /// <summary>
    /// Signal emitted when the selected tile is changed.
    /// </summary>
    [Signal]
    public delegate void OnSelectedTileChangeEventHandler(int column, int row);

    public GameState CurrentState { get; private set; }

    public void ChangeState(GameState newState)
    {
        PathPreviewer.SetVisible(newState == GameState.Build);
        CurrentState = newState;
    }
    
    /// <summary>
    /// The currently selected tile.
    /// </summary>
    public Tuple<int, int> HoveredTile { get; private set; }
    public Tuple<int, int> ActiveTile { get; private set; }
    private HexMap _map;
    public void RequestBuild(BuildingType type)
    {
        var tile = _map.GetTile(HoveredTile.Item1, HoveredTile.Item2);
        tile.Build(type);
    }

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

        _map = new HexMap(Width);

        foreach(Tile tile in _map.Map.Values)
        {
            // Instantiate a new tile.
            var viewTile = TileScene.Instantiate<ViewTile>();
            viewTile.Name = $"Tile ({tile.Col}, {tile.Row})";
            viewTile.Model = tile;
            viewTile.World = this;
        
            // Attach the tile to the world.
            AddChild(viewTile);
        
            // Set the position of the tile.
            viewTile.Position = ViewTile.GetHexagonCenterWorldPosition(tile.Col, tile.Row);
            viewTile._OnModelUpdate();
        }

        // GD.Print(_map.ToStringMap());
        
        // var path = _map.GetTile(0, 0).AStar(_map.GetTile(0, 1));
        // path = _map.GetTile(0, 0).AStar(_map.GetTile(3, 3));
        // path = _map.GetTile(3, 3).AStar(_map.GetTile(0, 0));
        //
        //
        // GD.Print("out");
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
        HoveredTile = ViewTile.GetHexagonMapCoordinates(ViewTile.FindClosestHexCenter(worldPosition));

        // Send a "hover changed" event.
        EmitSignal(SignalName.OnSelectedTileChange, HoveredTile.Item1, HoveredTile.Item2);
    }
}

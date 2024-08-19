using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CidreDoux.scripts.controller;
using CidreDoux.scripts.model;
using CidreDoux.scripts.model.building;
using CidreDoux.scripts.model.tile;
using Godot;

namespace CidreDoux.scripts.view;

/// <summary>
/// Class used to render a tile on the screen.
/// </summary>
public partial class ViewTile : Node2D
{
    private static Vector2 BaseOffset = new Vector2(0, -30);
    
    /// <summary>
    /// The background polygon of this tile.
    /// </summary>
    [Export, MaybeNull] public Polygon2D Background;

    /// <summary>
    /// The sprite displaying this tile's building
    /// </summary>
    [Export] public Sprite2D BuildingSprite;
    
    /// <summary>
    /// Reference to the <see cref="World"/> that owns this tile.
    /// </summary>
    public static World World => GameController.GetController().World;

    /// <summary>
    /// The inner tile model manipulated by this view.
    /// </summary>
    public Tile Model { get; private set; }

    /// <summary>
    /// Wrapper for the <see cref="Tile.Location"/> property.
    /// </summary>
    public TileLocation Location => Model.Location;

    /// <summary>
    /// Distance between the center of a hexagon and any of its edges.
    /// </summary>
    public const int CenterToEdgeDistance = 87;

    /// <summary>
    /// Distance between the center of a hexagon and any of its points.
    /// </summary>
    public const int CenterToVertexDistance = 100;

    /// <summary>
    /// Distance between two edges of the hexagon.
    /// </summary>
    public const int EdgeToEdgeDistance = 2 * CenterToEdgeDistance;

    /// <summary>
    /// Distance between two vertices of the hexagon.
    /// </summary>
    public const int VertexToVertexDistance = 2 * CenterToVertexDistance;

    /// <summary>
    /// Vertical offset between the centers of two hexagons.
    /// This takes into account the fact that the rows are offset from one another.
    /// </summary>
    public const int CenterToCenterVerticalDistance = 150;

    /// <summary>
    /// Horizontal offset between the centers of two hexagons.
    /// </summary>
    public const int CenterToCenterHorizontalDistance = EdgeToEdgeDistance;

    /// <summary>
    /// Vector used to get from one hexagon's center to another.
    /// </summary>
    public static readonly Vector2 CenterToCenterVector = new(
        CenterToCenterHorizontalDistance,
        CenterToCenterVerticalDistance
    );

    /// <inheritdoc cref="Node._Ready"/>
    public override void _Ready()
    {
        base._Ready();
        
        Background.TextureOffset = TextureCoords.Backgrounds[Model.Background];


        // Watch for updates to the selected tile.
        World.OnSelectedTileChange += _OnSelectedTileChange;
        Model.OnModelUpdate += _OnModelUpdate;
    }

    public void _OnModelUpdate()
    {
        if (Model.HasBuilding())
        {
            BuildingSprite.Position = Model.Building.Type == BuildingType.Base ? BaseOffset : Vector2.Zero;
            BuildingSprite.SetRegionRect(TextureCoords.Buildings[Model.Building.Type]);
            BuildingSprite.Visible = true;
        }
        else BuildingSprite.Visible = false;
    }
    
    /// <summary>
    /// Callback triggered by the <see cref="World"/> class when the selected tile changes.
    /// </summary>
    /// <param name="column">The column number of the selected tile.</param>
    /// <param name="row">The row number of the selected tile.</param>
    private void _OnSelectedTileChange(int column, int row)
    {
        // Check if the tile was selected.
        if (!(Model.Location.Column == column && Model.Location.Row == row))
        {
            ChangeTileColor(Colors.White);
            return;
        }
        
        ChangeTileColor(Colors.RebeccaPurple);
        switch (GameController.GetController().CurrentState)
        {
            case GameState.Idle: break;
            case GameState.Build: BuildTileChangeHandler();
                break;
            case GameState.AssignPath: AssignPathChangeHandler();
                break;
            case GameState.TurnEnd: break;
        }
    }

    public void BuildTileChangeHandler()
    {
        var controller = GameController.GetController();
        if (Model.HasBuilding())
        {
            ChangeTileColor(Colors.Red);
            controller.PathPreviewer.UpdatePath(null);
        }
        else
        {
            var preview = controller.World.GetBaseTile().AStar(Model);
            controller.PathPreviewer.UpdatePath(preview);   
        }
    }

    public void AssignPathChangeHandler()
    {
        if (Model.ComputeCrossingCost() < 0) return;

        var previewer = GameController.GetController().PathPreviewer;
        var index = previewer.TilePath.IndexOf(Model);
        if (index < 0)
        {
            if(previewer.TilePath.Last()?.IsNeighbor(Model) == true) previewer.TilePath.Add(Model);
        }
        else previewer.TilePath.RemoveRange(index, previewer.TilePath.Count-index);

        previewer.UpdatePath(previewer.TilePath);
    }

    /// <summary>
    /// Simple method used to update the color of a Tile.
    /// </summary>
    /// <param name="color">The new color of the Tile.</param>
    public void ChangeTileColor(Color color)
    {
        // If the polygon is not set, log an error.
        if (Background == null)
        {
            GD.PushError("A Tile object did not have an assigned Polygon2D");
            return;
        }

        // Set the tile colour.
        Background.SetColor(color);
    }

    /// <summary>
    /// Helper used to instantiate a new <see cref="ViewTile"/> instance.
    /// Does all the setup for the generated object internally before returning it.
    /// </summary>
    /// <param name="scene">The <see cref="PackedScene"/> that should be instanced.</param>
    /// <param name="model">The <see cref="Tile"/> that is being represented by this tile.</param>
    /// <param name="parent">The <see cref="controller.World"/> that this tile will be attached to.</param>
    /// <returns></returns>
    public static ViewTile Instantiate(PackedScene scene, Tile model)
    {
        // Initialize the game object.
        var tile = scene.Instantiate<ViewTile>();

        // Set the properties of the tile.
        tile.Model = model;
        tile.Name = $"Tile ({model.Location.Column}, {model.Location.Row})";

        // Set the location of the tile.
        tile.Position = GetHexagonCenterWorldPosition(model.Location.Column, model.Location.Row);

        tile._OnModelUpdate();
        return tile;
    }

    /// <summary>
    /// Retrieves the world position for the center of a hexagon with the given map coordinates.
    /// </summary>
    /// <param name="position">A column/row tuple of coordinates.</param>
    /// <returns>The world coordinates of the given hex.</returns>
    public static Vector2 GetHexagonCenterWorldPosition(TileLocation position)
    {
        return GetHexagonCenterWorldPosition(position.Column, position.Row);
    }

    /// <summary>
    /// Retrieves the world position for the center of a hexagon with the given map coordinates.
    /// </summary>
    /// <param name="column">The column of the hexagon in the world map.</param>
    /// <param name="row">The row of the hexagon in the world map.</param>
    /// <returns>The world coordinates of the given hex.</returns>
    public static Vector2 GetHexagonCenterWorldPosition(int column, int row)
    {
        // Compute the base x and y offset of the column and row.
        var x = column * CenterToCenterHorizontalDistance;
        var y = row * CenterToCenterVerticalDistance;

        // If the row is odd, add an offset to the horizontal position.
        if (row % 2 != 0)
        {
            x += CenterToEdgeDistance;
        }

        // Return the computed position.
        return new Vector2(x, y);
    }

    /// <summary>
    /// Retrieves the world position for the center of a hexagon with the given map coordinates.
    /// </summary>
    /// <param name="tile">The world map tile</param>
    /// <returns></returns>
    public static Vector2 GetHexagonCenterWorldPosition(Tile tile)
    {
        return GetHexagonCenterWorldPosition(tile.Location.Column, tile.Location.Row);
    }


    /// <summary>
    /// Retrieves the column/row map coordinates from a hex's center.
    /// </summary>
    /// <param name="position">The position of the hex in the world.</param>
    /// <returns>A column/row tuple with the map coordinates of the hex.</returns>
    public static TileLocation GetHexagonMapCoordinates(Vector2 position)
    {
        // Return the computed position.
        return new TileLocation(
            Mathf.FloorToInt(position.X / CenterToCenterHorizontalDistance),
            Mathf.FloorToInt(position.Y / CenterToCenterVerticalDistance)
        );
    }

    /// <summary>
    /// Finds the closest hex to a given world position.
    /// </summary>
    /// <param name="position">The position to compare against.</param>
    /// <returns>The row and column of the hex closest to the given position.</returns>
    public static Vector2 FindClosestHexCenter(Vector2 position)
    {
        // Normalize the position into the non-shifted row coordinates.
        var normalizedPosition = position / CenterToCenterVector;

        // Handle the tests differently if we start from a shifted row.
        if (Mathf.RoundToInt(normalizedPosition.Y) % 2 != 0)
        {
            // Offset the closest position by half a unit.
            normalizedPosition.X -= 0.5f;
        }

        // Return the closest hex center found.
        return normalizedPosition.Round() * CenterToCenterVector;
    }
}

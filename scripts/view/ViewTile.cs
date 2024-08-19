using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CidreDoux.scripts.model;
using Godot;

namespace CidreDoux.scripts.view;

/// <summary>
/// Class used to render a tile on the screen.
/// </summary>
public partial class ViewTile : Node2D
{
    private static Dictionary<BackgroundType, Vector2> _BGCoords = new Dictionary<BackgroundType, Vector2>()
    {
        [BackgroundType.Water] = new Vector2(0, 0),
        [BackgroundType.Forest] = new Vector2(0, 1),
        [BackgroundType.Grass] = new Vector2(1, 0),
        [BackgroundType.Mountain] = new Vector2(1, 1)
    };

    private static Dictionary<BuildingType, Rect2> _BuildingCoords = new Dictionary<BuildingType, Rect2>(){
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

    private static Vector2 BaseOffset = new Vector2(0, -30);
    
    /// <summary>
    /// The polygon manipulated by this tile class.
    /// </summary>
    [Export, MaybeNull] public Polygon2D Polygon;

    [Export] public Sprite2D BuildingSprite;

    /// <summary>
    /// Reference to the <see cref="World"/> that owns this tile.
    /// </summary>
    public World World;

    /// <summary>
    /// The inner tile model manipulated by this view.
    /// </summary>
    public model.Tile Model;

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

        // Watch for updates to the selected tile.
        World.OnSelectedTileChange += _OnSelectedTileChange;
        Model.OnModelUpdate += _OnModelUpdate;
    }

    public void _OnModelUpdate()
    {
        GD.Print($"ModelUpdate {Model}");
        Polygon.TextureOffset = _BGCoords[Model.Background];
        if (Model.HasBuilding())
        {
            if(Model.Building.Type == BuildingType.Base) BuildingSprite.Position = BaseOffset; else BuildingSprite.Position = Vector2.Zero;
            BuildingSprite.SetRegionRect(_BuildingCoords[Model.Building.Type]);
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
        if (!(Model.Col == column && Model.Row == row))
        {
            ChangeTileColor(Colors.White);
            return;
        }

        if (World.CurrentState == GameState.Build && Model.HasBuilding()){
            ChangeTileColor(Colors.Red);
            World.PathPreviewer.UpdatePath(null);
        }
        else
        {
            var preview = World.GetBaseTile().AStar(Model);
            World.PathPreviewer.UpdatePath(preview);
            ChangeTileColor(Colors.RebeccaPurple);
        }
    
    }

    /// <summary>
    /// Simple method used to update the color of a Tile.
    /// </summary>
    /// <param name="color">The new color of the Tile.</param>
    public void ChangeTileColor(Color color)
    {
        // If the polygon is not set, log an error.
        if (Polygon == null)
        {
            GD.PushError("A Tile object did not have an assigned Polygon2D");
            return;
        }

        // Set the tile colour.
        Polygon.SetColor(color);
    }

    /// <summary>
    /// Retrieves the world position for the center of a hexagon with the given map coordinates.
    /// </summary>
    /// <param name="position">A column/row tuple of coordinates.</param>
    /// <returns>The world coordinates of the given hex.</returns>
    public static Vector2 GetHexagonCenterWorldPosition(Tuple<int, int> position)
    {
        return GetHexagonCenterWorldPosition(position.Item1, position.Item2);
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
    public static Vector2 GetHexagonCenterWorldPosition(model.Tile tile)
    {
        return GetHexagonCenterWorldPosition(tile.Col, tile.Row);
    }


    /// <summary>
    /// Retrieves the column/row map coordinates from a hex's center.
    /// </summary>
    /// <param name="position">The position of the hex in the world.</param>
    /// <returns>A column/row tuple with the map coordinates of the hex.</returns>
    public static Tuple<int, int> GetHexagonMapCoordinates(Vector2 position)
    {
        // Return the computed position.
        return new Tuple<int, int>(
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

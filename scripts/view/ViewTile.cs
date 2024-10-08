using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CidreDoux.scripts.controller;
using CidreDoux.scripts.model;
using CidreDoux.scripts.model.building;
using CidreDoux.scripts.model.tile;
using CidreDoux.scripts.view.ui;
using Godot;

namespace CidreDoux.scripts.view;

/// <summary>
/// Class used to render a tile on the screen.
/// </summary>
public partial class ViewTile : Node2D
{
    private static readonly Vector2 BaseOffset = new Vector2(0, -30);

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

    [Export] public PackedScene PathScene;
    [Export] public Color PathPreviewColor;
    [Export] public Color BuildingDestroyedColor;
    [Export] public TurnCounter OutputTurnCounter;
    [Export] public ValidatedTurnCounter InputTurnCounter;

    [MaybeNull] public Path ProducerPath;

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
        Model.OnModelUpdate += _OnModelUpdate;
    }

    public void _OnModelUpdate()
    {
        // If the model has no building, hide its sprite and UI elements.
        if (!Model.HasBuilding())
        {
            InputTurnCounter.Visible = false;
            OutputTurnCounter.Visible = false;
            BuildingSprite.Visible = false;
            return;
        }

        // If the building is destroyed, hide the UI.
        if (Model.Building.IsDestroyed)
        {
            BuildingSprite.Modulate = BuildingDestroyedColor;
            InputTurnCounter.Visible = false;
            OutputTurnCounter.Visible = false;
            UpdatePathPreview();
            return;
        }

        // Place the sprite at the correct location and with the correct texture.
        BuildingSprite.Position = Model.Building.Type == BuildingType.Base ? BaseOffset : Vector2.Zero;
        BuildingSprite.SetRegionRect(TextureCoords.Buildings[Model.Building.Type]);
        BuildingSprite.Visible = true;

        // Find the most urgent consumer.
        if (Model.Building.Consumers.Count > 0)
        {
            var mostUrgentConsumer = Model.Building.Consumers.Aggregate(
                (accumulator, current) =>
                {
                    // Check if the current consumer is invalid.
                    if (accumulator.IsSatisfied && !current.IsSatisfied)
                    {
                        return current;
                    }

                    // Compare the turns left for each.
                    return accumulator.TurnsLeft > current.TurnsLeft ? current : accumulator;
                }
            );

            // Apply the information from the most urgent consumer.
            InputTurnCounter.SetCounter(mostUrgentConsumer.TurnsLeft);
            InputTurnCounter.SetValidity(mostUrgentConsumer.IsSatisfied);
            InputTurnCounter.Visible = true;
        }

        // Apply the information from the producer.
        if (Model.Building.PackageProducer is not null)
        {
            OutputTurnCounter.SetCounter(Model.Building.PackageProducer.TurnCounter);
            OutputTurnCounter.Visible = true;
        }

        // Update the path preview.
        UpdatePathPreview();
    }

    public void OnTileSelected()
    {
        ChangeTileColor(Colors.RebeccaPurple);
        if (ProducerPath is not null) ProducerPath.Visible = true;
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

    public void OnTileDeselected()
    {
        // GD.Print($"Left tile({Model.Location})");

        if (ProducerPath is not null) ProducerPath.Visible = false;
        ChangeTileColor(Colors.White);
    }

    public void BuildTileChangeHandler()
    {
        var controller = GameController.GetController();
        Debug.Assert(controller.SelectedBuildingType != null, "controller.SelectedBuildingType != null");
        if (!Model.CanPlaceBuilding((BuildingType)controller.SelectedBuildingType)){
            ChangeTileColor(Colors.Red);
            controller.PathPreviewer.UpdatePath(null);
        }
        else
        { var preview = controller.World.GetBaseTile().AStar(Model);
            controller.PathPreviewer.UpdatePath(preview);
        }
    }

    public void AssignPathChangeHandler()
    {
        if (Model.ComputeCrossingCost() < 0 && !Model.HasBuilding()) return;

        var previewer = GameController.GetController().PathPreviewer;
        var index = previewer.TilePath.IndexOf(Model);
        switch (index)
        {
            case < 0:
            {
                GD.Print($"tile {Model.Location} not in path.");
                var last = previewer.TilePath.Last();
                var lastCrossable = last;
                if (last.ComputeCrossingCost() < 0 && previewer.TilePath.Count > 1) lastCrossable = previewer.TilePath[^2];
                if (lastCrossable.IsNeighbor(Model))
                {
                    if (last != lastCrossable) previewer.TilePath.Remove(last);
                    previewer.TilePath.Add(Model);
                }
                else GD.Print($"tile {Model.Location} is not a neighbor of {previewer.TilePath.Last().Location}");
                break;
            }
            default:
                GD.Print($"tile {Model.Location} already in path");
                if (index != previewer.TilePath.Count - 1) previewer.TilePath.RemoveRange(index + 1, previewer.TilePath.Count-index-1);
                break;
        }

        previewer.UpdatePath(previewer.TilePath);
    }

    public void UpdatePathPreview()
    {
        if (Model.Building is null) return;
        if (Model.Building.PackageProducer is null) return;
        if (Model.Building.IsDestroyed && ProducerPath is null) return;

        if(Model.Building.IsDestroyed)
        {
            GD.Print("Destroying building path");
            GameController.GetController().PathLayer.RemoveChild(ProducerPath);
            ProducerPath.QueueFree();
            ProducerPath = null;
            return;
        }

        if (ProducerPath is null)
        {
            ProducerPath = PathScene.Instantiate<Path>();
            ProducerPath.Line.DefaultColor = PathPreviewColor;
            ProducerPath.Visible = false;
            GameController.GetController().PathLayer.AddChild(ProducerPath);
        }

        ProducerPath.UpdatePath(Model.Building.PackageProducer.Path);
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

    public new void QueueFree()
    {
        GameController.GetController().PathLayer.RemoveChild(ProducerPath);
        base.QueueFree();
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

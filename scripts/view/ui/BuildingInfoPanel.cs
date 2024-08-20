using System.Collections.Generic;
using CidreDoux.scripts.controller;
using CidreDoux.scripts.model.package.action;
using CidreDoux.scripts.model.tile;
using CidreDoux.scripts.resources;
using Godot;

namespace CidreDoux.scripts.view.ui;

public partial class BuildingInfoPanel : PanelContainer
{
    [Export] public BuildingTextureMap BuildingTextureMap;

    [Export] public PackedScene ConsumesInfoScene;

    [Export] public TextureRect BuildingIcon;
    [Export] public Label BuildingLabel;
    [Export] public Container Container;
    [Export] public ProducesContainer ProducesContainer;

    private Tile _previousTile;

    private List<ConsumesInfo> _containers = new();

    public void OnTileSelected()
    {
        // Clear visibility by default.
        Visible = false;

        // Unregister the previous listener.
        if (_previousTile is not null)
        {
            _previousTile.OnModelUpdate -= _OnTileModelUpdate;
            _previousTile = null;
        }

        // Get the selected tile.
        var tile = GameController.GetController().World.SelectedTile;
        if (tile is null)
        {
            return;
        }

        // Listen for updates on the new tile.
        _previousTile = tile.Model;
        _previousTile.OnModelUpdate += _OnTileModelUpdate;

        // Check if the tile has a building.
        if (!tile.Model.HasBuilding())
        {
            return;
        }

        Visible = true;

        BuildingIcon.Texture = BuildingTextureMap.GetTextureForBuildingType(tile.Model.Building.Type);
        BuildingLabel.Text = tile.Model.Building.Type.ToString();
        ProducesContainer.AssignProducer(tile.Model.Building.PackageProducer);

        // Resize the list.
        var unneededControlCount = _containers.Count - tile.Model.Building.Consumers.Count;
        if (unneededControlCount > 0)
        {
            for (var i = 0; i < unneededControlCount; i++)
            {
                Container.RemoveChild(_containers[tile.Model.Building.Consumers.Count + i]);
            }
            _containers.RemoveRange(tile.Model.Building.Consumers.Count, unneededControlCount);
        }

        // Assign all the consumers.
        for (var index = 0; index < tile.Model.Building.Consumers.Count; index++)
        {
            ConsumesInfo consumesInfo;
            if (index < _containers.Count)
            {
                consumesInfo = _containers[index];
            }
            else
            {
                consumesInfo = ConsumesInfoScene.Instantiate<ConsumesInfo>();
                Container.AddChild(consumesInfo);
                _containers.Add(consumesInfo);
            }

            consumesInfo.AssignConsumer(tile.Model.Building.Consumers[index]);
        }
    }

    private void _OnTileModelUpdate()
    {
        OnTileSelected();
    }

    public override void _Process(double delta)
    {
        // Get the selected tile.
        var tile = GameController.GetController().World.SelectedTile;
        if (tile is null || !tile.Model.HasBuilding())
        {
            return;
        }

        // Get the viewport coordinates of the tile.
        Position = _ToViewportCoordinates(tile.GlobalPosition);
    }

    private static Vector2 _ToViewportCoordinates(Vector2 worldCoordinates)
    {
        // Get the position of the camera in the world.
        return GameController.GetController().Player.Camera.GetCanvasTransform() * worldCoordinates;
    }
}

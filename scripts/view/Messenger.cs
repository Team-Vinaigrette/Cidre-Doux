using Godot;
using System;
using System.Linq;
using CidreDoux.scripts.controller;
using CidreDoux.scripts.model.package;
using CidreDoux.scripts.model.package.action;
using CidreDoux.scripts.model.tile;
using CidreDoux.scripts.view;

public partial class Messenger : Node2D
{
    private static int _id = 0;
    private static Tween _tween;
    
    [Export] public Sprite2D ResourceSprite;
    [Export] public Sprite2D BuildingSprite;
    [Export] public Path MessengerPath;
    [Export] public Control MessengerControl;

    public Package Model;

    private Tween GetTween()
    {
        if (_tween == null)
        {
            _tween = GetTree().CreateTween();
        }

        return _tween;
    }
    
    /// <summary>
    /// Helper used to instantiate a new <see cref="ViewTile"/> instance.
    /// Does all the setup for the generated object internally before returning it.
    /// </summary>
    /// <param name="scene">The <see cref="PackedScene"/> that should be instanced.</param>
    /// <param name="model">The <see cref="Tile"/> that is being represented by this tile.</param>
    /// <param name="parent">The <see cref="World"/> that this tile will be attached to.</param>
    /// <returns></returns>
    public static Messenger Instantiate(PackedScene scene, Package model)
    {
        // Initialize the game object.
        var messenger = scene.Instantiate<Messenger>();

        // Set the properties of the tile.
        messenger.Model = model;
        messenger.Name = $"Package{_id}";
        _id++;
        
        return messenger;
    }
    
    public override void _Ready()
    {
        base._Ready();
        
        MessengerPath.GetParent().RemoveChild(MessengerPath);
        GameController.GetController().World.AddChild(MessengerPath);
        
        if (Model.Type == PackageType.Ressource)
        {
            var handler = (DeliveryAction)Model.ActionHandler;
            ResourceSprite.SetRegionRect(TextureCoords.Ressources[handler.ResourceType]);
            ResourceSprite.Visible = true;
        }
        else
        {
            var handler = (BuildAction)Model.ActionHandler;
            BuildingSprite.SetRegionRect(TextureCoords.Buildings[handler.BuildingType]);
            BuildingSprite.Visible = true;
        }

        MessengerPath.UpdatePath(Model.CompletePath);
        Position = ViewTile.GetHexagonCenterWorldPosition(Model.CompletePath[0]);

        MessengerControl.Connect(Control.SignalName.MouseEntered, new Callable(this, MethodName.HandleMouseEntered));
        MessengerControl.Connect(Control.SignalName.MouseExited, new Callable(this, MethodName.HandleMouseExited));
    }

    public new void QueueFree()
    {
        GameController.GetController().World.RemoveChild(MessengerPath);
        base.QueueFree();
    }
    
    public void HandleMouseEntered()
    {
        MessengerPath.Visible = true;
    }

    public void HandleMouseExited()
    {
        MessengerPath.Visible = false;
    }

    public void Walk()
    {
        var crossed_tiles = Model.Walk();
        var tween = GetTree().CreateTween();
        tween.Pause();
        
        foreach (var tile in crossed_tiles)
        {
            tween.TweenProperty(this, "global_position", ViewTile.GetHexagonCenterWorldPosition(tile), 0.5f);
        }


        if (Model.LeftoverMovement != 0)
        {
            var nextTile = Model.RemainingPath.Peek();
            tween.TweenProperty(this, "global_position", 
                    this.GlobalPosition.Lerp(ViewTile.GetHexagonCenterWorldPosition(nextTile), 0.5f),
                0.5f);
        }

        tween.TweenCallback(new Callable(this, MethodName.WalkEnd));
        tween.Play();
    }

    public void WalkEnd()
    {
        if (!Model.RemainingPath.Any())
        {
            Model.ActionHandler.PerformAction(Model.CompletePath[Model.CompletePath.Count() - 1]);
            QueueFree();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CidreDoux.scripts.model.building;
using CidreDoux.scripts.model.package;
using CidreDoux.scripts.model.package.action;
using CidreDoux.scripts.model.tile;
using Godot;

namespace CidreDoux.scripts.controller;

/// <summary>
/// Singleton object that is accessible from anywhere.
/// The reference is stored on the <see cref="Node.NotificationParented"/> notification.
/// </summary>
public partial class GameController : Node
{
    /// <summary>
    /// Internal pointer to the <see cref="GameController"/> instance in the <see cref="Tree"/>.
    /// </summary>
    [MaybeNull] private static GameController _instance;

    [Export] public PackedScene MessengerScene;
        
    /// <summary>
    /// Reference to the <see cref="World"/> instance in the tree.
    /// </summary>
    [Export] public World World;

    /// <summary>
    /// Reference to the <see cref="Player"/> instance in the tree.
    /// </summary>
    [Export] public Player Player;

    [Export] public Node2D PathLayer;
    [Export] public Node2D MessengerLayer;
    [Export] public Path PathPreviewer;

    public GameState CurrentState { get; private set; }

    public Tile ActiveTile = null;
    
    public void ChangeState(GameState newState)
    {
        PathPreviewer.SetVisible(newState == GameState.Build || newState == GameState.AssignPath);
        CurrentState = newState;
    }
    
    /// <summary>
    /// Static accessor for the singleton instance.
    /// </summary>
    /// <returns>The singleton <see cref="GameController"/> instance in the world.</returns>
    /// <exception cref="NullReferenceException">When the instance was not found in the world.</exception>
    public static GameController GetController()
    {
        // Check if the instance is set.
        if (_instance is null)
        {
            throw new NullReferenceException($"The static {nameof(GameController)} reference was never set !");
        }

        return _instance;
    }

    /// <inheritdoc cref="Node._Notification"/>.
    public override void _Notification(int what)
    {
        // Call the base method.
        base._Notification(what);

        // Check the notification type.
        switch ((long) what)
        {
        case NotificationParented:
            // Check if the instance was already set.
            if (_instance is not null)
            {
                // Delete the instance.
                GD.PrintErr($"Tried to add two {nameof(GameController)} instances to the scene!");
                Free();
                return;
            }

            // Store the instance.
            _instance = this;

            break;
        case NotificationUnparented:
            // Clear the instance.
            _instance = null;
            break;
        }
    }
    
    /// <summary>
    /// Configures the base to send a build package at the end of the turn
    /// </summary>
    /// <param name="type"></param>
    public void RequestBuild(BuildingType type)
    {
        var @base = World.GetBaseTile();
        @base.Building.PackageProducer.AssignBuildAction(new BuildAction(type));
        @base.AssignPath(@base.AStar(World.SelectedTile.Model));
    }

    public void AddMessenger(Package package)
    {
        var messenger = Messenger.Instantiate(MessengerScene, package);
        MessengerLayer.AddChild(messenger);
    }
    
    public void EndTurn()
    {
        World.ProducePackages();
        
        foreach (var messenger in MessengerLayer.GetChildren().OfType<Messenger>())
        {
            messenger.Walk();
        }

        World.EndTurn();
    }

    public override void _Process(double delta)
    {
        switch (CurrentState)
        {
            case GameState.Idle:
                IdleHandler();
                break;
            case GameState.AssignPath:
                AssignPathHandler();
                break;
        }
    }

    public void IdleHandler()
    {
        if (Input.IsActionJustPressed("space")) EndTurn();

        if (Input.IsActionJustPressed("click") && World.SelectedTile.Model.HasBuilding())
        {
            if (World.SelectedTile.Model.Building.PackageProducer is not null)
            {
                if (World.SelectedTile.Model.Building.PackageProducer.PackageType == PackageType.Ressource)
                {
                    ActiveTile = World.SelectedTile.Model;
                    PathPreviewer.UpdatePath(new List<Tile>{ActiveTile});
                    ChangeState(GameState.AssignPath);
                }
            }
        }
    }

    public void AssignPathHandler()
    {
        if (Input.IsActionJustReleased("click"))
        {
            ActiveTile.AssignPath(new List<Tile>(PathPreviewer.TilePath));
            ChangeState(GameState.Idle);
        }
    }
}

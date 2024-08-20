using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CidreDoux.scripts.model;
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
    
    [Export] public Button EndTurnButton;
    /// <summary>
    /// Reference to the <see cref="World"/> instance in the tree.
    /// </summary>
    [Export] public World World;

    /// <summary>
    /// Reference to the <see cref="Player"/> instance in the tree.
    /// </summary>
    [Export] public Player Player;

    /// <summary>
    /// Layer that should collect all path previewers
    /// </summary>
    [Export] public Node2D PathLayer;

    /// <summary>
    /// Layer that should collect all messenger objects
    /// </summary>
    [Export] public Node2D MessengerLayer;

    /// <summary>
    /// Reference to the <see cref="PathPreviewer"/> instance in the tree.
    /// </summary>
    [Export] public Path PathPreviewer;

    /// <summary>
    /// Reference to the <see cref="BuildPanel"/> instance in the tree.
    /// </summary>
    [Export] public view.ui.UiView Ui;

    /// <summary>
    /// Current value for the state of the game.
    /// </summary>
    /// <seealso cref="GameState"/>
    public GameState CurrentState { get; private set; }

    /// <summary>
    /// Active tile for AssignPath action
    /// </summary>
    public Tile ActiveTile = null;

    public BuildingType? SelectedBuildingType { get; private set; } = null;

    public double EndturnTimer;
    
    public void SetSelectedBuildingType(BuildingType building)
    {
        SelectedBuildingType = building;
    }
    public void ChangeState(GameState newState)
    {
        PathPreviewer.SetVisible(newState == GameState.Build || newState == GameState.AssignPath);
        CurrentState = newState;
    }

    public int TurnCounter { get; private set; } = 0;

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

    /// <inheritdoc cref="Node._Ready"/>
    public override void _Ready()
    {
        ChangeState(GameState.Idle);
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
    public void RequestBuild()
    {
        Debug.Assert(SelectedBuildingType != null, nameof(SelectedBuildingType) + " != null");
        var targetTile = World.SelectedTile.Model;
        var buildingType = (BuildingType)SelectedBuildingType;
        
        if (targetTile.CanPlaceBuilding(buildingType)){
            var @base = World.GetBaseTile();
            @base.Building.PackageProducer.AssignBuildAction(new BuildAction(buildingType));
            @base.AssignPath(@base.AStar(World.SelectedTile.Model));
        }
        
        SelectedBuildingType = null;
        ChangeState(GameState.Idle);
    }

    public void AddMessenger(Package package)
    {
        var messenger = Messenger.Instantiate(MessengerScene, package);
        MessengerLayer.AddChild(messenger);
    }

    public void EndTurn()
    {
        if (CurrentState == GameState.TurnEnd) return;
        
        EndturnTimer = double.MaxValue;
        CurrentState = GameState.TurnEnd;
        EndTurnButton.Disabled = true;
        World.ProducePackages();

        foreach (var messenger in MessengerLayer.GetChildren().OfType<Messenger>())
        {
            messenger.Walk();
        }
        
        World.EndTurn();
        TurnCounter += 1;
        if (World.GetBaseTile().Building.IsDestroyed) GameManager.GetInstance().EndGame(TurnCounter);
        EndturnTimer = .5f;
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
            case GameState.TurnEnd:
                EndturnTimer -= delta;
                if (EndturnTimer < 0)
                {
                    EndTurnButton.Disabled = false;
                    ChangeState(GameState.Idle);
                }
                break;
        }
    }

    public void IdleHandler()
    {
        if (Input.IsActionJustPressed("space")) EndTurn();

        if (Input.IsActionJustPressed("click") && World.SelectedTile.Model.HasBuilding())
        {
            var building = World.SelectedTile.Model.Building;
            if (building.PackageProducer is not null && !building.IsDestroyed)
            {
                if (building.PackageProducer.PackageType == PackageType.Ressource)
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
            if(PathPreviewer.TilePath.Count > 1) ActiveTile.AssignPath(PathPreviewer.TilePath);
            else ActiveTile.AssignPath(null);
            ChangeState(GameState.Idle);
        }
    }
}

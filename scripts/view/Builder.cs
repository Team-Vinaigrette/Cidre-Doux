using System.Collections.Generic;
using CidreDoux.scripts.controller;
using CidreDoux.scripts.model.building;
using CidreDoux.scripts.resources;
using Godot;

namespace CidreDoux.scripts.view;

public partial class Builder : Control
{
    /// <summary>
    /// Reference to the object used to render the texture of the building.
    /// </summary>
    [Export] public TextureRect Texture;

    /// <summary>
    /// The texture map used to render the correct texture in this instance.
    /// </summary>
    [Export] public BuildingTextureMap TextureMap;

    /// <summary>
    /// The type of building built by this controller.
    /// </summary>
    [Export] public BuildingType BuildingType;

    public bool IsHovered { get; private set; }
    public bool IsDragged { get; private set; }
    private Vector2 _offset = Vector2.Zero;
    private Vector2 _startPos = Vector2.Zero;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        MouseEntered += HandleMouseEntered;
        MouseExited += HandleMouseExit;
        Texture.SetTexture(TextureMap.GetTextureForBuildingType(BuildingType));
    }

    public void HandleMouseEntered()
    {
        IsHovered = true;
    }

    public void HandleMouseExit()
    {
        IsHovered = false;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (IsDragged)
        {
            GlobalPosition = GetGlobalMousePosition() - _offset;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (!IsHovered) return;
        var controller = GameController.GetController();

        if (Input.IsActionJustPressed("click"))
        {
            IsDragged = true;
            controller.ChangeState(GameState.Build);
            _offset = GetGlobalMousePosition() - GlobalPosition;
            _startPos = GetPosition();
        }

        if (Input.IsActionJustReleased("click"))
        {
            controller.RequestBuild(BuildingType);
            Position = _startPos;
            IsDragged = false;
            controller.ChangeState(GameState.Idle);
        }
    }
}

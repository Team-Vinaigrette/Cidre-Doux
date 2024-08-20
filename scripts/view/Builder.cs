using System.Collections.Generic;
using CidreDoux.scripts.controller;
using CidreDoux.scripts.model.building;
using CidreDoux.scripts.view;
using CidreDoux.scripts.resources;
using Godot;

namespace CidreDoux.scripts.view;

public partial class Builder : Control
{
	[Export] public Control MyControl;
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
	
	private bool _isHovered = false;
	private bool _isDragged = false;
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
        _isHovered = true;
    }

    public void HandleMouseExit()
    {
        _isHovered = false;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (_isDragged)
        {
            GlobalPosition = GetGlobalMousePosition() - _offset;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (!_isHovered) return;
        var controller = GameController.GetController();

        if (Input.IsActionJustPressed("click"))
        {
            _isDragged = true;
            controller.SetSelectedBuildingType(BuildingType);
            controller.ChangeState(GameState.Build);
            _offset = GetGlobalMousePosition() - GlobalPosition;
            _startPos = GetPosition();
        }

        if (Input.IsActionJustReleased("click"))
        {
            controller.RequestBuild();
            Position = _startPos;
            _isDragged = false;
        }
    }
}

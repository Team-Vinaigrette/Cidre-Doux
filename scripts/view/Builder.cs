using Godot;
using System;
using System.Collections.Generic;
using CidreDoux.scripts;
using CidreDoux.scripts.controller;
using CidreDoux.scripts.model;
using CidreDoux.scripts.model.building;
using CidreDoux.scripts.view;

public partial class Builder : Node2D
{
	[Export] public Control MyControl;

	[Export] public Sprite2D MySprite;

	[Export] public BuildingType BuildingType;
	
	private bool _isHovered = false;
	private bool _isDragged = false;
	private Vector2 _offset = Vector2.Zero;
	private Vector2 _startPos = Vector2.Zero;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MyControl.Connect(Control.SignalName.MouseEntered, new Callable(this, MethodName.HandleMouseEntered));
		MyControl.Connect(Control.SignalName.MouseExited, new Callable(this, MethodName.HandleMouseExit));
		MySprite.SetRegionRect(TextureCoords.Buildings[BuildingType]);
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

		if (controller.CurrentState == GameState.Idle)
		{


			if (Input.IsActionJustPressed("click"))
			{
				_isDragged = true;
				controller.ChangeState(GameState.Build);
				_offset = GetGlobalMousePosition() - GlobalPosition;
				_startPos = GetPosition();
			}
		}
		else if (controller.CurrentState == GameState.Build)
		{
			if (Input.IsActionJustReleased("click"))
			{
				controller.RequestBuild(BuildingType);
				Position = _startPos;
				_isDragged = false;
				controller.ChangeState(GameState.Idle);
			}
		}
	}
}

using Godot;
using System;
using System.Collections.Generic;
using CidreDoux.scripts;
using CidreDoux.scripts.model;

public partial class Builder : Node2D
{
	private static Dictionary<BuildingType, Rect2> _textureCoords = new Dictionary<BuildingType, Rect2>(){
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

	[Export] public Control MyControl;

	[Export] public Sprite2D MySprite;

	[Export] public World MyWorld;

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
		MySprite.SetRegionRect(_textureCoords[BuildingType]);
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
		
		if (Input.IsActionJustPressed("click"))
		{
			_isDragged = true;
			MyWorld.ChangeState(GameState.Build);
			_offset = GetGlobalMousePosition() - GlobalPosition;
			_startPos = GetPosition();
		}

		if (Input.IsActionJustReleased("click"))
		{
			MyWorld.RequestBuild(BuildingType);
			Position = _startPos;
			_isDragged = false;
			MyWorld.ChangeState(GameState.Idle);
		}
	}
}

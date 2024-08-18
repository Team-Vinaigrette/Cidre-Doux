using Godot;
using System;
using CidreDoux.scripts.model;

public partial class UiBuilding : Control
{
	[Export] public BuildingType BuildingType;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override Variant _GetDragData(Vector2 atPosition)
	{
		GD.Print("Trying to drag here: " + atPosition);
		
		return base._GetDragData(atPosition);
	}

	
	
	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		GD.Print("Cheking if I can drop");
		return false;
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		
	}
}

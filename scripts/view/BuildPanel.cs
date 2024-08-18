using Godot;
using System;
using System.Linq;
using CidreDoux.scripts;

public partial class BuildPanel : Node2D
{
	[Export] public World World;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (var child in GetChildren().OfType<Builder>())
		{
			child.MyWorld = World;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

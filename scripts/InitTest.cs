using Godot;
using System;
using System.Collections.Generic;

namespace CidreDoux.scripts.model;

public partial class InitTest : Node2D
{
	public HexMap gameMap;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameMap = new HexMap(3);
		GD.Print(gameMap.ToStringMap());
		var farmTile = gameMap.GetTile(-1, -1);

		foreach (Tile t in farmTile.GetNeighbors())
		{
			GD.Print(t.ToStringMap());
		}
		
		foreach (Tile t in gameMap.GetTile(-1, 0).GetNeighbors())
		{
			GD.Print(t.ToStringMap());
		}

		farmTile.Build(BuildingType.Farm);
		var mineTile = gameMap.GetTile(-1, 0);
		mineTile.Build(BuildingType.Mine);
		var sawTile = gameMap.GetTile(-2, -1);
		sawTile.Build(BuildingType.Sawmill);
		
		GD.Print(gameMap.ToStringMap());
		
		Random randomizer = new Random();
		List<Tile> dynamicPath = new List<Tile>();
		String sequence = "";
		var currentTile = farmTile;
		for (int i = 0; i < 10; i++)
		{
			var candidates = currentTile.GetNeighbors();
			do
			{
				currentTile = candidates[randomizer.Next(candidates.Count)];
			} while (currentTile.ComputeCrossingCost() < 0);

			dynamicPath.Add(currentTile);
			GD.Print($"[{currentTile.Col},{currentTile.Row} ({currentTile.ComputeCrossingCost()})]");
			sequence += $" -> [{currentTile.Col},{currentTile.Row} ({currentTile.ComputeCrossingCost()})]";
		}
		
		GD.Print(sequence);
		
		farmTile.AssignPath(new List<Tile>(dynamicPath));
		Package pack = farmTile.Building.ProducePackage();
		sawTile.NextTurn();
		sawTile.NextTurn();
		pack.actionHandler.performAction(sawTile);

		while (pack.path.Count > 0)
		{
			foreach (var Tile in pack.Walk())
			{
				GD.Print(Tile);
			}
			GD.Print(pack.performedAlready);
			
		}
		
		GD.Print("Out");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

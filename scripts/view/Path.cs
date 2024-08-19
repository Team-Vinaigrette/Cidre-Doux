using Godot;
using System;
using System.Collections.Generic;
using CidreDoux.scripts.model;
using CidreDoux.scripts.model.tile;
using CidreDoux.scripts.view;

public partial class Path : Node2D
{
    [Export] public Line2D Line;
    
    public List<Tile> TilePath { get; private set; }

    public override void _Ready()
    {
        
    }

    public void UpdatePath(List<Tile> path)
    {
        this.TilePath = path;
        if (path is null)
        {
            Line.Points = null;
            return;
        }
        var points = new List<Vector2>{/*this.GlobalPosition*/};
        foreach (var tile in path)
        {
            points.Add(ViewTile.GetHexagonCenterWorldPosition(tile));
        }
        
        Line.Points = new List<Vector2>(points).ToArray();
    }
}

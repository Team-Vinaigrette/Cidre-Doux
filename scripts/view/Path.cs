using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using CidreDoux.scripts.model;
using CidreDoux.scripts.model.tile;
using CidreDoux.scripts.view;

public partial class Path : Node2D
{
    [Export] public Line2D Line;
    [Export] public Texture2D ArrowTexture;

    private Dictionary<int, Sprite2D> Arrows = new Dictionary<int, Sprite2D>();
    
    public List<Tile> TilePath { get; private set; }

    public override void _Ready()
    {
        
    }

    private Sprite2D GetArrow(int index)
    {
        if (Arrows.ContainsKey(index)) return Arrows[index];

        var arrow = new Sprite2D();
        arrow.Texture = ArrowTexture;
        arrow.Scale = new Vector2(.05f, .05f);
        Arrows.Add(index, arrow);
        AddChild(arrow);
        return arrow;
    }

    public void UpdatePath(List<Tile> path)
    {
        this.TilePath = path;
        if (path is null)
        {
            Line.Points = null;
            foreach (var arrow in Arrows.Values)
            {
                arrow.Visible = false;
            }
            return;
        }
        var points = new List<Vector2>{/*this.GlobalPosition*/};
        
        foreach (var tile in path)
        {
            points.Add(ViewTile.GetHexagonCenterWorldPosition(tile));
            if (points.Count > 1)
            {
                var arrow = GetArrow(points.Count - 1);
                arrow.Position = points[^2].Lerp(points[^1], 0.5f);
                arrow.Visible = true;
                arrow.Rotation = points[^2].AngleToPoint(points[^1]);
            }
        }
        
        foreach (var arrow in Arrows.Where(arrow => arrow.Key >= points.Count))
        {
            arrow.Value.Visible = false;
        }
        
        Line.Points = new List<Vector2>(points).ToArray();
    }
}

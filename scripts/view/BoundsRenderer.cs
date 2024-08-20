using CidreDoux.scripts.controller;
using Godot;

namespace CidreDoux.scripts.view;

public partial class BoundsRenderer: Node2D
{
    public override void _Draw()
    {
        DrawRect(GameController.GetController().World.Bounds, Colors.Red, false, 2f);
    }
}

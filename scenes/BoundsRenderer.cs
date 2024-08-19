using CidreDoux.scripts.controller;
using Godot;

namespace CidreDoux.scenes;

public partial class BoundsRenderer : Node2D
{

    /// <inheritdoc cref="CanvasItem._Draw"/>
    public override void _Draw()
    {
        DrawRect(GameController.GetController().World.Bounds, Colors.Red, false, 16f);
    }
}

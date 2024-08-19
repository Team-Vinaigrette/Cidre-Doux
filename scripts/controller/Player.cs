using System.Diagnostics.CodeAnalysis;
using Godot;

namespace CidreDoux.scripts.controller;

/// <summary>
/// Simple class used to move a player camera on the screen.
/// </summary>
public partial class Player : Node2D
{
    /// <summary>
    /// Reference to the camera of the player instance.
    /// </summary>
    [Export] public Camera2D Camera;

    /// <summary>
    /// Distance to the edge of the screen under which a scroll will be triggered.
    /// </summary>
    [Export] public int EdgeScrollingThreshold;

    /// <summary>
    /// Movement speed of the camera, in units/second.
    /// </summary>
    [Export] public float Speed;

    /// <summary>
    /// Statically stored <see cref="StringName"/> for the "move_up" name.
    /// </summary>
    private static readonly StringName MoveUpInput = new("move_up");

    /// <summary>
    /// Statically stored <see cref="StringName"/> for the "move_down" name.
    /// </summary>
    private static readonly StringName MoveDownInput = new("move_down");

    /// <summary>
    /// Statically stored <see cref="StringName"/> for the "move_left" name.
    /// </summary>
    private static readonly StringName MoveLeftInput = new("move_left");

    /// <summary>
    /// Statically stored <see cref="StringName"/> for the "move_right" name.
    /// </summary>
    private static readonly StringName MoveRightInput = new("move_right");

    /// <inheritdoc cref="Node._PhysicsProcess"/>
    public override void _PhysicsProcess(double delta)
    {
        // Get the input value from the keyboard.
        var movement = Input.GetVector(MoveLeftInput, MoveRightInput, MoveUpInput, MoveDownInput);

        // Enable mouse movements only if the ui is not hovered.
        if (movement.IsZeroApprox() && !GameController.GetController().Ui.IsHovered)
        {
            movement = _HandleEdgeScrolling();
        }

        // Apply the movement to the player node.
        Position += movement * Speed * (float)delta;

        // Get the actual viewport (minus the Ui) rect.
        var viewportRect = Camera.GetViewportRect();
        viewportRect = new(
            new Vector2(GameController.GetController().Ui.GetRect().Size.X, 0),
            viewportRect.Size
        );

        // Get the camera rect in world coordinates.
        var cameraWorldRect = new Rect2(Position - viewportRect.Size / 2f, viewportRect.Size);

        // Replace the camera within the bounds of the world.
        Position = cameraWorldRect.Intersection(GameController.GetController().World.Bounds).GetCenter();
    }

    /// <summary>
    /// Method used to handle the mouse-cursor-based edge scrolling.
    /// </summary>
    /// <returns></returns>
    private Vector2 _HandleEdgeScrolling()
    {
        var movement = Vector2.Zero;

        // Get the position of the UI on the screen.
        var uiRect = GameController.GetController().Ui.GetRect();

        // Get the mouse position on the viewport.
        var mousePosition = GetViewport().GetMousePosition();
        var viewportRect = GetViewportRect();

        // Check if the mouse is inside one of the thresholds.
        if (mousePosition.X < EdgeScrollingThreshold + uiRect.Size.X)
        {
            movement += Vector2.Left;
        }

        if (mousePosition.X > viewportRect.Size.X - EdgeScrollingThreshold)
        {
            movement += Vector2.Right;
        }

        if (mousePosition.Y < EdgeScrollingThreshold)
        {
            movement += Vector2.Up;
        }

        if (mousePosition.Y > viewportRect.Size.Y - EdgeScrollingThreshold)
        {
            movement += Vector2.Down;
        }

        // Return a zero vector if there is no movement.
        return movement;
    }
}

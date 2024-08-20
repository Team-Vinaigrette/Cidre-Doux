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

    /// <summary>
    /// Flag disabled when the mouse is not within the bounds of the game window.
    /// </summary>
    private bool _isMouseInWindow;

    /// <summary>
    /// Grows the maximum camera size to a new maximum.
    /// </summary>
    public void Grow()
    {
        // Get the size of the world.
        var size = GameController.GetController().World.Size;

        // Convert the world size to a new camera zoom.

    }

    /// <inheritdoc cref="Node._Ready"/>
    public override void _Ready()
    {
        // Listen to the mouse entered and exited events.
        GetWindow().MouseEntered += () => _isMouseInWindow = true;
        GetWindow().MouseExited += () => _isMouseInWindow = true;
    }

    /// <inheritdoc cref="Node._PhysicsProcess"/>
    public override void _PhysicsProcess(double delta)
    {
        // Get the input value from the keyboard.
        var movement = Input.GetVector(MoveLeftInput, MoveRightInput, MoveUpInput, MoveDownInput);

        // Enable mouse movements only if the ui is not hovered.
        if (movement.IsZeroApprox() && !GameController.GetController().Ui.BuildPanel.IsHovered)
        {
            movement = _HandleEdgeScrolling();
        }

        // Apply the movement to the player node.
        Position += movement * Speed * (float)delta;

        // Get the actual viewport (minus the Ui) rect.
        var viewportRect = Camera.GetViewportRect();
        viewportRect = new Rect2(
            new Vector2(GameController.GetController().Ui.BuildPanel.GetRect().Size.X, 0),
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

        // Do nothing if the mouse is not inside the current window.
        if (!GetWindow().GetVisibleRect().HasPoint(GetWindow().GetMousePosition()))
        {
            return movement;
        }

        // Get the position of the UI on the screen.
        var uiRect = GameController.GetController().Ui.BuildPanel.GetRect();

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

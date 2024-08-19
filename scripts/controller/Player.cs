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
        // Get the input value as a 2D vector.
        var input = Input.GetVector(MoveLeftInput, MoveRightInput, MoveUpInput, MoveDownInput);

        // Apply the movement to the player node.
        Position += input * Speed * (float)delta;
    }
}

using Godot;

namespace CidreDoux.scripts.scenes.prefab;


public partial class Player : CharacterBody2D
{
    public const float Speed = 300.0f;

    public override void _PhysicsProcess(double delta)
    {
        // Get the input direction and handle the movement/deceleration.
        Vector2 direction = Input.GetVector("character_left", "character_right", "character_up", "character_down");

        // Apply the movement to the character.
        Velocity = direction * Speed;

        MoveAndSlide();
    }
}
using Godot;

namespace CidreDoux.scripts.scenes.prefab;


public partial class Player : CharacterBody2D
{
    /// Movement speed of the player character.
    [Export]
    public float Speed = 300.0f;
    
    /// Sprite used by the player.
    [Export]
    public AnimatedSprite2D Sprite;

    public override void _PhysicsProcess(double delta)
    {
        // Get the input direction and handle the movement/deceleration.
        Vector2 direction = Input.GetVector("character_left", "character_right", "character_up", "character_down");

        // Apply the movement to the character.
        Velocity = direction * Speed;
        MoveAndSlide();
        
        // Update the animation.
        _UpdateAnimation();
    }
    
    /// Updates the animation of the player based on their current velocity.
    private void _UpdateAnimation()
    {
        // Normalize the velocity of the player.
        Vector2 normalizedVelocity = Velocity.Normalized();
        
        // Apply the correct direction for the animation.
        if (normalizedVelocity.Y < -0.5)
        {
            Sprite.Play("walk-up");
        }
        else if (normalizedVelocity.Y > 0.5)
        {
            Sprite.Play("walk-down");
        }
        else if (normalizedVelocity.X < -0.5)
        {
            Sprite.Play("walk-left");
        }
        else if (normalizedVelocity.X > 0.5)
        {
            Sprite.Play("walk-right");
        }
        else
        {
            Sprite.Pause();
        }
    }
}
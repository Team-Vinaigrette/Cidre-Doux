using System;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace CidreDoux.scripts.controller;

/// <summary>
/// Singleton object that is accessible from anywhere.
/// The reference is stored on the <see cref="Node.NotificationParented"/> notification.
/// </summary>
public partial class GameController : Node
{
    /// <summary>
    /// Internal pointer to the <see cref="GameController"/> instance in the <see cref="Tree"/>.
    /// </summary>
    [MaybeNull] private static GameController _instance;

    /// <summary>
    /// Reference to the <see cref="World"/> instance in the tree.
    /// </summary>
    [Export] public World World;

    /// <summary>
    /// Reference to the <see cref="Player"/> instance in the tree.
    /// </summary>
    [Export] public Player Player;

    /// <summary>
    /// Static accessor for the singleton instance.
    /// </summary>
    /// <returns>The singleton <see cref="GameController"/> instance in the world.</returns>
    /// <exception cref="NullReferenceException">When the instance was not found in the world.</exception>
    public static GameController GetController()
    {
        // Check if the instance is set.
        if (_instance is null)
        {
            throw new NullReferenceException($"The static {nameof(GameController)} reference was never set !");
        }

        return _instance;
    }

    /// <inheritdoc cref="Node._Notification"/>.
    public override void _Notification(int what)
    {
        // Call the base method.
        base._Notification(what);

        // Check the notification type.
        switch ((long) what)
        {
        case NotificationParented:
            // Check if the instance was already set.
            if (_instance is not null)
            {
                // Delete the instance.
                GD.PrintErr($"Tried to add two {nameof(GameController)} instances to the scene!");
                Free();
                return;
            }

            // Store the instance.
            _instance = this;

            break;
        case NotificationUnparented:
            // Clear the instance.
            _instance = null;
            break;
        }
    }

    /// <inheritdoc cref="Node._EnterTree"/>.
    public override void _ExitTree()
    {
        // Clear the instance.
        _instance = null;
    }
}

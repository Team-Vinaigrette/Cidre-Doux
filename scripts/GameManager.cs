using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

public partial class GameManager : Node2D
{
    [MaybeNull] private static GameManager _Instance;
    [Export] public PackedScene GameScene;
    [Export] public PackedScene GameOverScene;

    public static GameManager GetInstance()
    {
        if (_Instance is null)
        {
            throw new NullReferenceException($"The static {nameof(GameManager)} reference was never set !");
        }
        else return _Instance;
    }

    public override void _Notification(int what)
    {
        // Call the base method.
        base._Notification(what);

        // Check the notification type.
        switch ((long)what)
        {
            case NotificationParented:
                // Check if the instance was already set.
                if (_Instance is not null)
                {
                    // Delete the instance.
                    GD.PrintErr($"Tried to add two {nameof(GameManager)} instances to the scene!");
                    Free();
                    return;
                }

                // Store the instance.
                _Instance = this;

                break;
            case NotificationUnparented:
                // Clear the instance.
                _Instance = null;
                break;
        }
    }

    public void StartGame()
    {
        foreach (var child in GetChildren())
        {
            RemoveChild(child);
        }
        AddChild(GameScene.Instantiate());
    }

    public void EndGame(int score)
    {
        foreach (var child in GetChildren())
        {
            RemoveChild(child);
        }

        var gameOverScene = GameOverScene.Instantiate<GameOver>();
        AddChild(gameOverScene);
        gameOverScene.SetScore(score);
    }
}

using Godot;
using System;

public partial class GameOver : Node2D
{
    [Export] private Label scoreLabel;

    public void SetScore(int score)
    {
        scoreLabel.Text = "You lasted " + score.ToString() + " turns!";
        scoreLabel.SetAnchorsPreset(Control.LayoutPreset.Center, false);
    }
}

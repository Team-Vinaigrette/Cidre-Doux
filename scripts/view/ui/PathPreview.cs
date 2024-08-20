using Godot;
using System;
using CidreDoux.scripts.controller;

public partial class PathPreview : PanelContainer
{
    [Export] public Label MessengerLabel;
    public Messenger messenger;
    public void RequestDisplay(Messenger messenger)
    {
        this.messenger = messenger;
        Visible = true;
    }

    public void EndDisplay(Messenger messenger)
    {
        if (messenger != this.messenger) return;
        
        Visible = false;
        this.messenger = null;
    }

    public override void _Process(double delta)
    {
        if (this.messenger is null) return;
        
        GD.Print("Position");
        Position = GameController.GetController().Player.Camera.GetCanvasTransform() * messenger.GlobalPosition;
        MessengerLabel.Text = messenger.Model.GetRemainingPathDuration().ToString();
    }
}

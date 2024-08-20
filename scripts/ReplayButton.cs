using Godot;
using System;

public partial class ReplayButton : Button
{
    public override void _Pressed()
    {
        GameManager.GetInstance().StartGame();
    }
}

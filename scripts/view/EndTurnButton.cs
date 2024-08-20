using CidreDoux.scripts.controller;

namespace CidreDoux.scripts.view;

public partial class EndTurnButton : Godot.Button
{
    public override void _Pressed()
    {
        GameController.GetController().EndTurn();
    }
}
using Godot;
using System;

public partial class BuildPanel : PanelContainer
{
    public bool IsHovered { get; private set; }

    public override void _Ready()
    {
        MouseEntered += _OnMouseEntered;
        MouseExited += _OnMouseExited;
    }

    private void _OnMouseEntered()
    {
        IsHovered = true;
    }

    private void _OnMouseExited()
    {
        IsHovered = false;
    }
}

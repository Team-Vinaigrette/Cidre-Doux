using Godot;

namespace CidreDoux.scripts.view.ui;

public partial class UiView : CanvasLayer
{
    [Export] public BuildingInfoPanel InfoPanel;
    [Export] public BuildPanel BuildPanel;
    [Export] public PathPreview PathPreview;
}

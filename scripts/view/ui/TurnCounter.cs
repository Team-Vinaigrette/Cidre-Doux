using Godot;

namespace CidreDoux.scripts.view.ui;

/// <summary>
/// Ele
/// </summary>
public partial class TurnCounter : Panel
{
    /// <summary>
    /// The counter rendered by this element.
    /// </summary>
    [Export] public Label Counter;

    /// <summary>
    /// Method used to update the text of the <see cref="Counter"/>.
    /// </summary>
    /// <param name="value">The value of the <see cref="Counter"/>.</param>
    public void SetCounter(int value)
    {
        Counter.Text = value.ToString();
    }
}

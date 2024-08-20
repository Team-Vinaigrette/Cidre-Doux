using Godot;

namespace CidreDoux.scripts.view.ui;

/// <summary>
/// Ele
/// </summary>
public partial class ValidatedTurnCounter : TurnCounter
{
    /// <summary>
    /// <see cref="StringName"/> for the invalid variant of the counter.
    /// </summary>
    private static readonly StringName InvalidTurnCounterName = new("InvalidTurnPanel");

    /// <summary>
    /// <see cref="StringName"/> for the valid variant of the counter.
    /// </summary>
    private static readonly StringName ValidTurnCounterName = new("ValidTurnPanel");

    /// <summary>
    /// Helper used to set the validity status of the counter.
    /// </summary>
    /// <param name="isValid">The validity status of the counter.</param>
    public void SetValidity(bool isValid)
    {
        SetThemeTypeVariation(isValid ? ValidTurnCounterName : InvalidTurnCounterName);
    }
}

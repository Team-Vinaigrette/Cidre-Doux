using System.Diagnostics.CodeAnalysis;
using Godot;

namespace CidreDoux.scripts.view;

/// <summary>
/// Class used to render a tile on the screen.
/// </summary>
public partial class Tile : Node2D
{
    /// <summary>
    /// The polygon manipulated by this tile class.
    /// </summary>
    [Export, MaybeNull] public Polygon2D Polygon;

    /// <summary>
    /// Distance between the center of a hexagon and any of its edges.
    /// </summary>
    public const int CenterToEdgeDistance = 87;

    /// <summary>
    /// Distance between the center of a hexagon and any of its points.
    /// </summary>
    public const int CenterToVertexDistance = 100;

    /// <summary>
    /// Constant <see cref="StringName"/> used for the name of the shader colour parameter.
    /// </summary>
    private static readonly StringName TileShaderColourParameterName = new("tile_colour");


    /// <summary>
    ///
    /// </summary>
    public void ChangeTileColor(Color color)
    {
        // If the polygon is not set, log an error.
        if (Polygon == null)
        {
            GD.PushError("A Tile object did not have an assigned Polygon2D");
            return;
        }

        // Set the tile colour.
        Polygon.SetColor(color);
    }

    /// <summary>
    /// Retrieves the world position for the center of a hexagon with the given map coordinates.
    /// </summary>
    /// <param name="column">The column of the hexagon in the world map.</param>
    /// <param name="row">The row of the hexagon in the world map.</param>
    /// <returns></returns>
    public static Vector2 GetHexagonCenterWorldPosition(int column, int row)
    {
        // Compute the base x and y offset of the column and row.
        var x = column * 2 * CenterToEdgeDistance;
        var y = row * 3 * CenterToVertexDistance / 2;

        // If the row is odd, add an offset to the vertical position.
        if (row % 2 == 0)
        {
            x += CenterToEdgeDistance;
            // y += CenterToVertexDistance / 2;
        }

        // Return the computed position.
        return new Vector2(x, y);
    }
}

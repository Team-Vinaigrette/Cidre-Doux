using System.Collections.Generic;

namespace CidreDoux.scripts.model.tile;

/// <summary>
/// Simple struct used to describe the location of a <see cref="Tile"/>.
/// </summary>
public struct TileLocation
{
    /// <summary>
    /// The column of the tile.
    /// </summary>
    public int Column;

    /// <summary>
    /// The row of the tile.
    /// </summary>
    public int Row;

    /// <summary>
    /// Constructor helper.
    /// </summary>
    /// <param name="column">The column of the location.</param>
    /// <param name="row">The row of the location.</param>
    public TileLocation(int column, int row)
    {
        Column = column;
        Row = row;
    }

    /// <inheritdoc cref="object.ToString"/>
    public override string ToString()
    {
        return $"({Column},{Row})";
    }
}

/// <summary>
/// <see cref="IComparer{T}"/> class that can be used for a <see cref="SortedDictionary{TKey,TValue}"/> of <see cref="TileLocation"/>.
/// </summary>
public class TileLocationComparer : IComparer<TileLocation>
{
    /// <inheritdoc cref="IComparer{T}.Compare"/>
    public int Compare(TileLocation lhs, TileLocation rhs)
    {

        // Compare the rows of the locations.
        if (lhs.Row < rhs.Row)
        {
            return -1;
        }


        if (lhs.Row > rhs.Row)
        {
            return 1;
        }

        // Compare the column of the locations.
        if (lhs.Column < rhs.Column)
        {
            return -1;
        }

        if (lhs.Column > rhs.Column)
        {
            return 1;
        }

        return 0;
    }
}

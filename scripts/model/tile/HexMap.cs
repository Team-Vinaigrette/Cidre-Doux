using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using CidreDoux.scripts.model.building;
using Godot;

namespace CidreDoux.scripts.model.tile;

/// <summary>
/// Object used to store a map of <see cref="Tile"/> objects.
/// </summary>
public class HexMap
{
    /// <summary>
    /// Dictionary of all the <see cref="Tile"/> objcets found in the map.
    /// </summary>
    public readonly SortedDictionary<TileLocation, Tile> Map;

    /// <summary>
    /// Size of the map.
    /// </summary>
    public int Size { get; private set; }

    /// <summary>
    /// Randomizer used when generating the map.
    /// </summary>
    private readonly Random _randomizer = new Random();

    /// <summary>
    /// Creates a new random hexmap with indices ranging from - to +size.
    /// </summary>
    /// <param name="size">The size of the initialized map</param>
    public HexMap(int size)
    {
        Size = size;
        Map = new SortedDictionary<TileLocation, Tile>(new TileLocationComparer());

        // Generate the map.
        _Generate();
    }

    // Extends the HexMap, adding one row and one column on each side
    public void Grow()
    {
        // Create a new column on each side.
        for (var row = -Size; row <= Size; row++)
        {
            _GenerateRandomTile(new TileLocation(-Size - 1, row));
            _GenerateRandomTile(new TileLocation(+Size + 1, row));
        }

        // Create a new row on each side.
        for (var column = -Size - 1; column <= Size + 1; column++)
        {
            _GenerateRandomTile(new TileLocation(column, -Size - 1));
            _GenerateRandomTile(new TileLocation(column, +Size + 1));
        }

        // Increment the size value.
        Size++;
    }

    /// <inheritdoc cref="object.ToString"/>
    public override string ToString()
    {
        // Prepare a new string builder instance.
        var builder = new StringBuilder();

        // Render all the tiles in the map.
        foreach (var tile in Map.Values)
        {
            builder.Append($"{tile.Location} -> {tile}");
        }

        return builder.ToString();
    }

    /// <summary>
    /// Returns the tile at the given location.
    /// </summary>
    /// <param name="column">The column of the requested tile</param>
    /// <param name="row">The row of the requested tile</param>
    /// <returns>The tile found at the given location.</returns>
    [return: MaybeNull]
    public Tile GetTile(int column, int row)
    {
        return GetTile(new TileLocation(column, row));
    }

    /// <summary>
    /// Returns the tile at the given location.
    /// </summary>
    /// <param name="location">The location of the requested tile</param>
    /// <returns>The tile found at the given location.</returns>
    [return: MaybeNull]
    public Tile GetTile(TileLocation location)
    {
        // Check if the tile exists in the map.
        if (!Map.ContainsKey(location))
        {
            return null;
        }

        return Map[location];
    }

    /// <summary>
    /// Retrieves the six neighbouring tiles around the given tile.
    /// </summary>
    /// <param name="location">The location of the tile to get the neighbours for.</param>
    /// <returns>A list of neighbouring tiles.</returns>
    public List<Tile> GetNeighbors(TileLocation location)
    {
        // Check if the row has an offset.
        var rowOffset = Mathf.Abs(location.Row % 2);
        var neighbors = new[]
        {
            GetTile(location.Column - 1, location.Row),
            GetTile(location.Column - 1 + rowOffset, location.Row - 1),
            GetTile(location.Column + rowOffset, location.Row - 1),
            GetTile(location.Column + 1, location.Row),
            GetTile(location.Column + rowOffset, location.Row + 1),
            GetTile(location.Column - 1 + rowOffset, location.Row + 1)
        };

        // Filter out any "null" neighbors.
        return new List<Tile>(neighbors.Where(tile => tile is not null));
    }

    /// <summary>
    /// Helper used to render the map as a single string.
    /// </summary>
    /// <returns>A string representation of the map.</returns>
    public string ToStringMap()
    {
        StringBuilder builder = new();

        // Iterate over all the tiles.
        foreach (var tile in Map.Values)
        {
            // Add a newline for the first item in a row.
            if (tile.Location.Column == -Size && tile.Location.Row != -Size)
            {
                builder.Append('\n');
            }

            // Add an offset for odd rows.
            if (tile.Location.Column == -Size && tile.Location.Row % 2 != 0)
            {
                builder.Append(' ');
                builder.Append(' ');
                builder.Append(' ');
                builder.Append(' ');
            }

            // Render the tile.
            builder.Append(tile.ToStringMap());
            builder.Append(' ');
        }

        return builder.ToString();
    }

    /// <summary>
    /// Helper used to generate the <see cref="Map"/>.
    /// </summary>
    private void _Generate()
    {
        // Clear the map.
        Map.Clear();

        // Initialize the tiles.
        for (var column = -Size; column <= Size; column++)
        {
            for (var row = -Size; row <= Size; row++)
            {
                // Create the tile.
                var tile = _GenerateRandomTile(new TileLocation(column, row));

                // If we are at the center of the map, build a base here.
                if (column == 0 && row == 0)
                {
                    tile.Build(BuildingType.Base);
                }
            }
        }
    }

    /// <summary>
    /// Helper used to generate a new random <see cref="Tile"/>.
    /// </summary>
    /// <param name="location">The location of the generated <see cref="Tile"/>.</param>
    /// <returns>The generated tile</returns>
    private Tile _GenerateRandomTile(TileLocation location)
    {
        // Create the tile.
        var tile = new Tile(location, _GenerateRandomBackgroundType(), this);

        // Add the tile to the map.
        Map.Add(location, tile);

        return tile;
    }

    /// <summary>
    /// Helper used to generate a random <see cref="BackgroundType"/> enum value.
    /// </summary>
    /// <returns>A random value from the <see cref="BackgroundType"/> enum.</returns>
    private BackgroundType _GenerateRandomBackgroundType()
    {
        return (BackgroundType)Tile.BackgroundTypeValues.GetValue(_randomizer.Next(Tile.BackgroundTypeValues.Length))!;
    }
}

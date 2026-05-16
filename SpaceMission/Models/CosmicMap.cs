namespace SpaceMission.Models;

/// <summary>
/// Represents the full cosmic navigation map and provides grid operations.
/// </summary>
public class CosmicMap
{
    private readonly Cell[,] _grid;

    public int Rows { get; }
    public int Cols { get; }

    public Cell this[int row, int col] => _grid[row, col];

    public CosmicMap(Cell[,] grid)
    {
        _grid = grid;
        Rows = grid.GetLength(0);
        Cols = grid.GetLength(1);
    }

    public bool InBounds(int row, int col) =>
        row >= 0 && row < Rows && col >= 0 && col < Cols;

    /// <summary>Returns all astronauts found on the map.</summary>
    public IReadOnlyList<Astronaut> GetAstronauts()
    {
        var list = new List<Astronaut>();
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                if (_grid[r, c].IsAstronaut)
                    list.Add(new Astronaut(_grid[r, c].Symbol, r, c));
        return list;
    }

    /// <summary>Returns the position of the Space Station ('F').</summary>
    public (int Row, int Col)? GetStation()
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                if (_grid[r, c].IsStation)
                    return (r, c);
        return null;
    }

    /// <summary>Deep-copies the grid symbols into a 2-D string array for display.</summary>
    public string[,] ToDisplayGrid()
    {
        var display = new string[Rows, Cols];
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                display[r, c] = _grid[r, c].Symbol;
        return display;
    }
}